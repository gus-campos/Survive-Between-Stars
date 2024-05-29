using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting;

using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;
using Quaternion = UnityEngine.Quaternion;
using System.Runtime.CompilerServices;
//using Microsoft.Unity.VisualStudio.Editor;
//using UnityEditor.Experimental.GraphView;

using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class Rocket : MonoBehaviour {

    // Movimentação
    private float accelMultiplier;
    [SerializeField] private float speedLimit;
    [SerializeField] private float accelIntensity;
    [SerializeField] private float boostMultiplier;
    [SerializeField] private float brakeIntensity;
    [SerializeField] private float brakingMinimumVelocity;

    // Para o SmoothDamp do speedlimiter
    private Vector3 accelCurrentVariation;
    [SerializeField] private float speedLimiterSmoothTime = 0.5F;

    // Rotação da nave
    [SerializeField] private float ignoreMouseRadius = 15F;
    
    private float debug1;
    private float debug2;
    private float debug3;
    private float debug4;
    
    // Dados iniciais da nave para reinicialização
    private Vector3 startingPosition;
    private Quaternion startingRotation;
    private Vector3 startingVelocity;
    
    // Atirando partículas
    private Vector3 shooterPosition;
    private float rocketLenth = 25f;
    [SerializeField] private GameObject bullet;
    [SerializeField] private float shootCooldownTimeout = 0.25f;
    private float shootCooldownTimer;
    private bool shootAvailable = true;
    private List<GameObject> shotsArray = new List<GameObject>();

    // Sons
    private AudioSource rocketAudioSource;
    [SerializeField] private AudioClip Boom;
    [SerializeField] private AudioClip pewPew;
    [SerializeField] private AudioClip dashingSound;

    // Declarando uma propriedade desta classe
    // Um objeto "vazio" da classe RigidBody2D
    private Rigidbody2D fisica;
    private Director director;
    // Click duplo
    private float lastClick = Mathf.NegativeInfinity;

    // Dash
    private bool dashing = false;
    private float dashAvailableTimer;
    private bool dashAvailable = true;
    [SerializeField] private float dashAvailableTimeOut;
    private float dashDurationTimer;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashForce;
    private float rocketMomentum;
    private int clickCounter = 0;
    private float multipleClickDelta = 0.3f;
    
    // Dashbar
    [SerializeField] private Image dashBar;

    // Cursor
    [SerializeField] private Texture2D cursorTexture;

   
    // ==========================================================================================================
    
    // Este é um método que é automaticamente chamado pela Unity quando se cria um objeto
    private void Awake() {

        // Capturando componente RigidBody2D
        fisica = GetComponent<Rigidbody2D>();

        // Capturando audio source
        rocketAudioSource = GetComponent<AudioSource>();

        // Guardando valores iniciais
        startingPosition = transform.position;
        startingRotation = transform.rotation;
        startingVelocity = fisica.velocity;

        // Inicializando timer 
    }

    private void Start() {

        // Definindo a tag como "Player"
        gameObject.tag = "Player";

        // Capturando GameObject Director
        director = GameObject.FindObjectOfType<Director>();

        // Definindo imagem do cursor
        Cursor.SetCursor(cursorTexture, 
                         new Vector2 (cursorTexture.width, cursorTexture.height) / 2, 
                         CursorMode.Auto);
    }

    void FixedUpdate() {
        
        // Atualizar rotação
        UpdateRotation();

        // Atualizar estado do dash
        UpdateDash();

        // Atualizar tiro
        UpdateShoot();

    }

    // Update é chamado pela Unity em cada frame do jogo
    void Update() {

        if (!dashing) {

            // Acelerar
            if (Input.GetMouseButton(1)) { Accel(); }

            // Freiar
            if (Input.GetKey(KeyCode.Space)) { Brake(); }

            // Shoot Particle
            if (Input.GetMouseButtonDown(0)) { if (shootAvailable && fisica.simulated) { Shoot(); } }

            // Dash
            if (MultClickTest()) { if (dashAvailable) { Dash(); } }
        }
    }

    // Checa por cliques múltiplos feitos - com "(clickCounter == 2)" identifica clique duplo
    private bool MultClickTest() {

        // Se botão direito foi apertado
        if (Input.GetMouseButtonDown(1)) {
            
            // Avaliar se já foi apertado muito recentemente
            if (Time.time - lastClick < multipleClickDelta) {

                // Incrementar o contador de clique
                clickCounter += 1;

                // Se a contagem estiver em 3
                if (clickCounter == 2) {

                    // Declarar que click triplo foi feito
                    return true;
                }

                // Se a contagem não está em 3
                else { 

                    // Gravar momento de tal pressionamento
                    lastClick = Time.time;
                    // Declarar que click triplo não foi feito
                    return false; 
                }
            }

            // Do contrário
            else {
                
                // Reiniciar contador, contando o primeiro clique
                clickCounter = 1;
                // Gravar momento de tal pressionamento
                lastClick = Time.time;
                // Declarar que clique duplo não foi feito
                return false;
            }
        }

        // Declarar que clique duplo não foi feito
        else { return false; }
    }

    void UpdateShoot() {

        if (!shootAvailable) {

            // Decrementar o timer
            shootCooldownTimer -= Time.deltaTime;
            // Se timer tiver zerado
            if (shootCooldownTimer <=0 ) {

                // Declarar que está disponível
                shootAvailable = true;
            }
        }
    }

    void UpdateDash() {

        // Atualizar barra
        if (dashAvailable) { dashBar.fillAmount = 1f; } 
        
        else { dashBar.fillAmount = 1 - (dashAvailableTimer / dashAvailableTimeOut); }
        
        //Debug.Log(100 * dashAvailableTimer / dashAvailableTimeOut);

        if (dashing) {
         
            // Decrementar timer de dashing
            dashDurationTimer -= Time.deltaTime;

            // Se tiver zerado o timer
            if (dashDurationTimer <= 0f) {

                // Devolvendo momento de antes do dash, na direção do dash
                fisica.AddForce(fisica.velocity.normalized * rocketMomentum, ForceMode2D.Impulse);
                // Removendo impulso
                fisica.AddForce(-fisica.velocity.normalized * dashForce, ForceMode2D.Impulse);
                // Não está mais dando dash
                dashing = false;
                // Declarar como não disponível
                dashAvailable = false;
            }
        }

        else if (!dashAvailable) {

            // Decrementar o timer de disponibilidade
            dashAvailableTimer -= Time.deltaTime;

            // Se tiver esgotado o timer para disponibilidade
            if (dashAvailableTimer <= 0) {

                // Resetar o contador
                dashAvailableTimer = dashAvailableTimeOut;
                // Declarar que está disponível
                dashAvailable = true;
            }
        }
    }


    // Se colidir, terminar jogo
    private void OnCollisionEnter2D(Collision2D collisionInfo) { 

        // Tocar o som de explosão
        rocketAudioSource.PlayOneShot(Boom, 0.8F);
        // Parar de simular sua física
        fisica.simulated = false;   
        // Congelar o tempo
        director.EndGame(); 
        
    }

    public void Reset() {

        // Voltar para o status inicial
        transform.position = startingPosition;
        transform.rotation = startingRotation;
        fisica.velocity = startingVelocity;
        
        // Resetando timers
        dashAvailableTimer = 0;
        shootCooldownTimer = 0;
        dashDurationTimer = 0;
        
        // Resetando bools
        dashing = false;
        dashAvailable = true;
        shootAvailable = true;
        
        // Ligar simulação física
        fisica.simulated = true;
    }


    private void Accel() {

        // Se left shift tiver apertado, usar modo turbo (multiplicar do boost)
        //if (false) { accelMultiplier = boostMultiplier; } else { accelMultiplier = 1; }

        accelMultiplier = 1;

        fisica.AddForce(accelMultiplier * accelIntensity * (transform.rotation * Vector3.right), ForceMode2D.Impulse);
            
        // Limitador de velocidade: se velocidade for maior que o permitido
        if (fisica.velocity.magnitude > accelMultiplier * speedLimit) {
            
            Vector3 current = fisica.velocity;

            Vector3 target = Vector3.ClampMagnitude(fisica.velocity, accelMultiplier * speedLimit);

            fisica.velocity = Vector3.SmoothDamp(current, target, ref accelCurrentVariation, speedLimiterSmoothTime);

        }
    }

    private void Dash() {

        // Resetando o timer de duração do dash
        dashDurationTimer = dashDuration;
        // Resetando o timer de disponibilidade do dash
        dashAvailableTimer = dashAvailableTimeOut;
        // Achar momento da nave, guardar
        rocketMomentum = fisica.velocity.magnitude * fisica.mass;
        // Remover momento atual da nave
        fisica.AddForce(fisica.velocity.normalized * -rocketMomentum, ForceMode2D.Impulse);
        // Adicionando momento do dash
        fisica.AddForce(transform.rotation * Vector3.right * dashForce, ForceMode2D.Impulse);
        // Declarar que dash está ativado
        dashing = true;
        // Tocar som
        rocketAudioSource.PlayOneShot(dashingSound, 1F);
    }

    private void Brake() {

        // Só até a velocidade chegar no valor mínimo definido
        // Além de impedir que fique parado, impede que "dê ré"
        if (fisica.velocity.magnitude > brakingMinimumVelocity) {
            
            // Desacelerar linearmente (acelerar pra trás)
            fisica.AddForce(-brakeIntensity * fisica.velocity, ForceMode2D.Impulse);
        }
    }

    private void UpdateRotation() {

        // Posição do mouse no mundo
        Vector3 worldMousePosic = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Zerando a componente Z
        worldMousePosic.z = 0;

        // Posição do mouse em relação ao foguete
        Vector3 rocketToMousePosic = worldMousePosic - transform.position;

        // Se o mouse não estiver muito próximo da nave
        if (rocketToMousePosic.magnitude > ignoreMouseRadius) {

            // ------> Mudar nome das variáveis

            // Ângulo do mouse 
            debug1 = Vector3.SignedAngle(rocketToMousePosic, Vector3.right, Vector3.back);
            // Ângulo do foguete
            debug2 = Vector3.SignedAngle(transform.rotation * Vector3.right, Vector3.right, Vector3.back);
            // SmoothDamp entre os dois
            debug3 = Mathf.SmoothDampAngle(current: debug1, target: debug2, currentVelocity: ref debug4, smoothTime: 1000f);

            // Apontando nave em direção ao mouse
            fisica.MoveRotation(debug1);

        }
    }

    private void Shoot() {

        shooterPosition = transform.position + Quaternion.Euler(transform.eulerAngles) * Vector3.right * rocketLenth;

        shotsArray.Add(Instantiate(bullet, shooterPosition, transform.rotation));

        rocketAudioSource.PlayOneShot(pewPew, 1F);

        shootAvailable = false;

        shootCooldownTimer = shootCooldownTimeout;
    }

    public void DestroyShots() {

        foreach (GameObject shot in shotsArray) {

            Destroy(shot);
        }
    }
}