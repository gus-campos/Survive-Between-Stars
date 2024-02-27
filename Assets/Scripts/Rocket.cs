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
//using UnityEditor.Experimental.GraphView;

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
    private Vector3 rocketVector;
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

    // Sons
    private AudioSource rocketAudioSource;
    [SerializeField] private AudioClip Boom;
    [SerializeField] private AudioClip pewPew;

    // Declarando uma propriedade desta classe
    // Um objeto "vazio" da classe RigidBody2D
    private Rigidbody2D fisica;
    private Director director;
   
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
    }

    private void Start() {

        // Capturando GameObject Director
        director = GameObject.FindObjectOfType<Director>();
    }


    void FixedUpdate() {

        // Atualizar rotação
        UpdateRotation();
    }

    // Update é chamado pela Unity em cada frame do jogo
    void Update() {

        // Acelerar
        if (Input.GetMouseButton(1)) { Accel(); }

        // Freiar
        if (Input.GetKey(KeyCode.Space)) { Brake(); }

        // Shoot Particle
        if (Input.GetMouseButtonDown(0)) { ShootParticle(); }

         // Vetor com a mesma orientação que o foguete
        rocketVector = Quaternion.Euler(transform.eulerAngles) * Vector3.right;
        
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
        
        // Ligar simulação física
        fisica.simulated = true;
    }


    private void Accel() {

        // Se left shift tiver apertado, usar modo turbo (multiplicar do boost)
        if (Input.GetKey("left shift")) { accelMultiplier = boostMultiplier; } else { accelMultiplier = 1; }

        // Acelerar linearmente
        fisica.AddForce(accelMultiplier * accelIntensity * rocketVector, ForceMode2D.Impulse);
            
        // Limitador de velocidade: se velocidade for maior que o permitido
        if (fisica.velocity.magnitude > accelMultiplier * speedLimit) {
            
            // Velocidade atual
            Vector3 current = fisica.velocity;
            // Valocidade alvo (limitada)
            Vector3 target = Vector3.ClampMagnitude(fisica.velocity, accelMultiplier * speedLimit);
            // Desaceleração suave
            fisica.velocity = Vector3.SmoothDamp(current, target, ref accelCurrentVariation, speedLimiterSmoothTime);

        }
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

            // Ângulo do mouse 
            debug1 = Vector3.SignedAngle(rocketToMousePosic, Vector3.right, Vector3.back);
            // Ângulo do foguete
            debug2 = Vector3.SignedAngle(transform.rotation * Vector3.right, Vector3.right, Vector3.back);
            // SmoothDamp entre os dois
            debug3 = Mathf.SmoothDampAngle(current: debug1, target: debug2, currentVelocity: ref debug4, smoothTime: 1000f);
            // Debug
            //Debug.Log(new Vector3(debug1, debug2, debug3));

            // Apontando nave em direção ao mouse
            fisica.MoveRotation(debug1);

        }
    }

    private void ShootParticle() {

        // 10 é o comprimento da ponta da nave, para n detectar o tiro com si
        shooterPosition = transform.position + rocketVector * rocketLenth;
        // Criar bala
        Instantiate(bullet, shooterPosition, transform.rotation);
        // Tocar som de itro
        rocketAudioSource.PlayOneShot(pewPew, 1F);

    }

}