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
    [SerializeField] private float scalingFrequency = 5f;
    // Multiplicador da escala
    [SerializeField] private float scaleMultiplier = 0.05f;
    // Escala inicial
    [SerializeField] private float initialScale = 2.7f;

    // Timer de auto destruição 
    private float timer = 1.2f;
    // Bool do timer
    private bool timerActive = false;
    // Armazenamento da referência
    private GameObject explosionGenerated;
    // Colisor
    private CircleCollider2D circleCollider2D;
    // Animação - objeto explosão
    [SerializeField] private GameObject mineExplosion;
    private SpriteRenderer spriteRenderer;
    // Efeitos sonoros
    private AudioSource mineAudio;
    [SerializeField] private AudioClip boom;

    // Perseguição
    private Rocket rocket;
    private Rigidbody2D rigidBody;
    private Vector3 forceVector = Vector3.zero;


    void Start() {
        
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
    }

    void FixedUpdate() {

        // Neutralizando força
        rigidBody.AddForce(-forceVector, ForceMode2D.Force);
        // Salvando força
        forceVector = (rocket.transform.position - transform.position).normalized * 1500F;
        // Aplicando força
        rigidBody.AddForce(forceVector, ForceMode2D.Force);

    }

    void Update() {

        // A escala varia com uma senoide, com a passágem do tempo
        float scaling = scaleMultiplier * Mathf.Sin(scalingFrequency * Time.time);
        // Sobre suas propriedades (armazenadas em "tranform") aplicamos o escalonamento
        transform.localScale = Vector3.one * (initialScale + scaling);
    
        // Se timer foi ativado
        if (timerActive) {

            // Descrementar o timer
            timer -= Time.deltaTime;

            // Se tiver for zerado
            if (timer <= 0) {

                // Destruitr explosão
                Destroy(explosionGenerated);
                // Auto destruir
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collisionInfo) { AutoDestruir(); }

    void OnCollisionEnter2D(Collision2D collisionInfo) { AutoDestruir(); }

    void AutoDestruir() {

        // Instanciar explosão
        explosionGenerated = Instantiate(mineExplosion, transform.position, Quaternion.identity, transform);
        // Tocar som de explosão
        mineAudio.PlayOneShot(boom, 0.8F);
        // Desativar sprite
        spriteRenderer.enabled = false;
        // Desativar a colisão
        circleCollider2D.enabled = false;
        // Ligar timer de autodestruição
        timerActive = true;

    }
}