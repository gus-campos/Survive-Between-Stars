using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using UnityEngine.Video;

public class Mine : MonoBehaviour {

    // Frequência da animação
    [SerializeField] private float BPM = 140f;
    // Multiplicador da escala
    [SerializeField] private float scaleMultiplier = 0.05f;
    // Escala inicial
    [SerializeField] private Vector3 initialScale;

    // Timer de auto destruição 
    private float timer = 1.2f;

    // Bool do timer
    private bool timerActive = false;

    // Colisor
    private CircleCollider2D circleCollider2D;

    // Animação - objeto explosão
    [SerializeField] private GameObject mineExplosion;
    private SpriteRenderer spriteRenderer;
    private GameObject buildingInstantiated;
    private GameObject explosionInstantiated;

    // Animação - construção
    [SerializeField] private GameObject mineBuilding;

    // Efeitos sonoros
    private AudioSource mineAudio;
    [SerializeField] private AudioClip boom;    

    // Pontuação
    private GuiManager guiManager;

    // Perseguição
    private Rocket rocket;
    private Rigidbody2D rigidBody;
    private Vector3 impulseVector = Vector3.zero;

    // Comunicar destruição ao generator
    private MineGenerator mineGenerator;

    // Velocidade variável
    private float mineVelocityNorm;


    void Start() {

        // Definindo a tag para a mina (constante entre as instâncias)
        gameObject.tag = "Mine";
        // Capturando AudioSource
        mineAudio = GetComponent<AudioSource>();
        // Capturando sprite renderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Capturando colisor
        circleCollider2D = GetComponent<CircleCollider2D>();
        // Capturando rigidbody
        rigidBody = GetComponent<Rigidbody2D>();
        // Encontrando objeto rocket
        rocket = GameObject.FindObjectOfType<Rocket>();
        // Encontrando objeto Score
        guiManager = GameObject.FindObjectOfType<GuiManager>();
        // Encontrando objeto Score
        mineGenerator = GameObject.FindObjectOfType<MineGenerator>();
        // Salavando escala para uso posterior
        initialScale = transform.localScale;

        // Começar construção
        StartBuilding();
    }

    void StartBuilding() {

        // Desligar simulação física
        rigidBody.simulated = false;
        // Desativar sprite
        spriteRenderer.enabled = false;
        // Instanciar construção da animação de building
        buildingInstantiated = Instantiate(mineBuilding, 
                                           transform.position, 
                                           Quaternion.identity, 
                                           transform);
    }

    public void EndBuilding () {

        // Destruir objeto da animação de construção
        Destroy(buildingInstantiated);
        // Ligar simulação física
        rigidBody.simulated = true;
        // Ativar sprite
        spriteRenderer.enabled = true;
    }

    
    void FixedUpdate() {

        // Lendo velocidade global
        mineVelocityNorm = mineGenerator.mineVelocityNorm;

        // Atualizando movimento das minas
        if (spriteRenderer.enabled) {

            // Neutralizando força
            rigidBody.AddForce(-impulseVector, ForceMode2D.Impulse);
            // Salvando força
            impulseVector = (rocket.transform.position - transform.position).normalized * rigidBody.mass * mineVelocityNorm;
            // Aplicando força
            rigidBody.AddForce(impulseVector, ForceMode2D.Impulse);
        }
    }

    void Update() {

        UpdateScale();

        UpdateSelfDestruct();
    }

    private void UpdateScale() {

        // A escala varia com uma senoide, com a passágem do tempo
        // scaling Frequency dá a frequência em BPM
        float scaling = scaleMultiplier * Mathf.Sin(BPM/60 * Time.time * 2*Mathf.PI);
        // Sobre suas propriedades (armazenadas em "tranform") aplicamos o escalonamento
        transform.localScale = initialScale + Vector3.one * scaling;
    }


    // O beam é um trigger, e essa opção permite que se atinja as minas
    private void OnTriggerEnter2D(Collider2D collisionInfo) { 

        // Adicionar 1 à pontuação
        guiManager.AddToScore(1);
        // Se destruir   
        SelfDestruct(); 
    }

    // Se colidir, se destruir, mas não adicionar pontos
    void OnCollisionEnter2D(Collision2D collisionInfo) { 
        
        // Se a colisão foi com uma mina, ignorar
        if (collisionInfo.gameObject.tag != "Mine") { 
    
            // Autodestruit
            SelfDestruct(); 
        }
    }

    void SelfDestruct() {

        // Instanciar explosão
        explosionInstantiated = Instantiate(mineExplosion, 
                                            transform.position, 
                                            Quaternion.Euler(0,0, Random.Range(0,360)), 
                                            transform);

        // Interromper simulação física
        rigidBody.simulated = false;
        // Tocar som de explosão
        mineAudio.PlayOneShot(boom, 0.8F);
        // Desativar sprite
        spriteRenderer.enabled = false;
        // Desativar a colisão
        circleCollider2D.enabled = false;
        // Ligar timer de autodestruição
        timerActive = true;
        // Decrementar contador de minas no mineGenerator
        mineGenerator.mineCounter -= 1;

    }

    private void UpdateSelfDestruct() {

        // Se timer pós autodestruição foi zerado
        if (timerActive) {

            // Descrementar o timer
            timer -= Time.deltaTime;

            // Se timer for zerado
            if (timer <= 0) {

                // Destruitr explosão
                Destroy(explosionInstantiated);
                // Auto destruir
                Destroy(gameObject);
            }
        }
    }
}