using UnityEngine;

public class Beam : MonoBehaviour {

    private Rigidbody2D _rb;
    private Vector3 _firePosition;
    [SerializeField] private float bulletImpulseIntensity;
    [SerializeField] private float maxDistanceTraveled;


    void Awake() {  

        _rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable() {
        
        // Saving position where it was fired
        _firePosition = transform.position;
        
        // Reseting velocity and addind impulse
        _rb.velocity = Vector3.zero;
        _rb.AddForce(transform.rotation * Vector3.right * bulletImpulseIntensity, ForceMode2D.Impulse);
    }

    void Update() {

        // Deactivate if too far from firing position
        if ((transform.position - _firePosition).magnitude > maxDistanceTraveled) {
            
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D collisionInfo) { 
        
        // Deactivate if collide with any obect, except player
        if (collisionInfo.gameObject.tag != "Player") {

            gameObject.SetActive(false);
        }   
    }
}