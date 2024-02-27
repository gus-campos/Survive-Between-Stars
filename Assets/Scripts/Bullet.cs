using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

using Vector3 = UnityEngine.Vector3;

public class Bullet : MonoBehaviour {

    private Rigidbody2D fisica;
    private Vector3 startPosition;

    private float bulletImpulseIntensity = 800f;
    private float maxDistanceFromRocket = 500f;

    void Start() {

        // Salvar sua posição inicial
        startPosition = transform.position;

        // Capturar RigidBody2D
        fisica = GetComponent<Rigidbody2D>();
        // Iniciar com força já aplicada
        fisica.AddForce(transform.rotation * Vector3.right * bulletImpulseIntensity, ForceMode2D.Impulse);

    }

    // Se colidir
    void OnCollisionEnter2D(Collision2D collisionInfo) { 
        
        // Destruir objeto
        Destroy(gameObject); 
    }


     // Se colidir
    void OnTriggerEnter2D(Collider2D collisionInfo) { 
        
        // Destruir objeto
        Destroy(gameObject); 
    }

    void Update() {
        
        // Caso se afaste muito da nave
        if ((transform.position - startPosition).magnitude > maxDistanceFromRocket) {

            // Se destruir
            Destroy(gameObject);
        }

    }
}
