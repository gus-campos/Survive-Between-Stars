using UnityEngine;

public class CameraControl : MonoBehaviour {
    
    private float cameraZcoordinate = -10f;
    private Vector3 currentVelocity;

    [SerializeField] private float smoothTime = 0.1f;
    [SerializeField] private GameObject spaceship;

    public void MoveCamera(Vector3 position) {
        
        transform.position = new Vector3(position.x, position.z, cameraZcoordinate);
    }

    void LateUpdate() {

        // Target for the smooth MoveCamerament of the camera    
        Vector3 target = new Vector3(spaceship.transform.position.x, 
                                     spaceship.transform.position.y,
                                     cameraZcoordinate);

        // Update smoothly the MoveCamerament of the camera
        transform.position = Vector3.SmoothDamp(current: transform.position, 
                                                target: target, 
                                                currentVelocity: ref currentVelocity, 
                                                smoothTime: smoothTime);
    }
}
