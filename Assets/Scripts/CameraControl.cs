using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

using Vector3 = UnityEngine.Vector3;

public class CameraControl : MonoBehaviour {
    
    // Velocidade atual (pro smoothtime)
    private Vector3 currentVelocity = Vector3.zero;
    // Coordenada Z da câmera
    private float cameraZcoordinate = -10f;
    // Tempo do smooth
    [SerializeField] private float smoothTime = 0.1f;
    
    [SerializeField] private GameObject rocket;

    public void Move(Vector3 position) {
        
        transform.position = new Vector3(position.x, position.z, cameraZcoordinate);
    }

    void LateUpdate() {
    
        // Alvo: posição da nave, com o Z modificado - parece que a câmera tem que ficar atrás da cena
        Vector3 target = new Vector3(rocket.transform.position.x, rocket.transform.position.y, cameraZcoordinate);

        // Posição da cêmera segue suavemente a posição da nave
        transform.position = Vector3.SmoothDamp(current : transform.position, 
                                                target : target, 
                                                currentVelocity : ref currentVelocity, 
                                                smoothTime : smoothTime);
        
    }
}
