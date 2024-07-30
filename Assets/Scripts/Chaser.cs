using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

public class Chaser : MonoBehaviour {

    // Animation cicles per minute ans scale settings
    [SerializeField] private float BPM = 140f;
    [SerializeField] private float scaleMultiplier = 0.05f;
    [SerializeField] private Vector3 initialScale;

    // Collision
    private CircleCollider2D circleCollider2D;

    // Explosion
    [SerializeField] private GameObject chaserExplosion;
    private SpriteRenderer spriteRenderer;
    private GameObject buildingInstantiated;
    private Animator buildingAnimator;
    private GameObject explosionInstantiated;
    private Animator explosionAnimator;

    // Building
    [SerializeField] private GameObject chaserBuilding;

    // SFX
    private AudioSource chaserAudioSource;
    [SerializeField] private AudioClip boom;    

    // Score
    private GuiManager guiManager;

    // Following
    private Spaceship spaceship;
    private Rigidbody2D rb;

    // Spawn and speed
    private Spawner spawner;
    private float chaserVelocityNorm;

    // --------------------------------------------------- Public ------------------------------------------------------
    void Awake() {

        // Explosion
        explosionInstantiated = Instantiate(chaserExplosion, transform.position, 
                                            Quaternion.Euler(0,0, Random.Range(0,360)), transform);

        // Building
        buildingInstantiated = Instantiate(chaserBuilding, transform.position, Quaternion.identity, transform);

        // Components
        chaserAudioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        explosionAnimator = explosionInstantiated.GetComponent<Animator>();
        buildingAnimator = buildingInstantiated.GetComponent<Animator>();                                   
        
        // Game Objects
        spaceship = GameObject.FindObjectOfType<Spaceship>();
        guiManager = GameObject.FindObjectOfType<GuiManager>();
        spawner = GameObject.FindObjectOfType<Spawner>();

        // Scale
        initialScale = transform.localScale;
        
        // States
        explosionInstantiated.SetActive(false);
    }

    void OnEnable() {

        Reanimate();
        StartBuilding();
    }

    void Update() {

        UpdateBuilding();
        UpdateScale();
        UpdateSelfDestruct();
    }
    
    void FixedUpdate() {

        UpdateRotation();

        // Reading global valocity
        chaserVelocityNorm = spawner.chaserVelocityNorm;

        // Updating velocity
        if (spriteRenderer.enabled) {

            rb.velocity = Vector3.zero;

            rb.AddForce(rb.mass 
                        * chaserVelocityNorm
                        * (spaceship.transform.position - transform.position).normalized, 
                        ForceMode2D.Impulse);
        }
    }

    // --------------------------------------------------- Private -----------------------------------------------------
    private void UpdateScale() {

        float scaling = scaleMultiplier * Mathf.Sin(BPM/60 * Time.time * 2*Mathf.PI);
        transform.localScale = initialScale + Vector3.one * scaling;
    }

    private void UpdateRotation() {

        rb.MoveRotation(Vector3.SignedAngle(spaceship.transform.position - gameObject.transform.position, 
                                            Vector3.right, Vector3.back));
    }

    private void UpdateBuilding() {

        if (buildingAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f) { 
           
            EndBuilding(); 
        }
    }

    private void UpdateSelfDestruct() {

        if (explosionAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f) {

            explosionInstantiated.SetActive(false);
            gameObject.SetActive(false);
        }
    
    }

    private void StartBuilding() {

        circleCollider2D.enabled = false;
        spriteRenderer.enabled = false;
        buildingInstantiated.SetActive(true);
        
    }

    private void EndBuilding () {

        circleCollider2D.enabled = true;
        spriteRenderer.enabled = true;
        buildingInstantiated.SetActive(false);
    }

    private void SelfDestruct() {

        chaserAudioSource.PlayOneShot(boom, 0.8F);

        rb.simulated = false;
        spriteRenderer.enabled = false;
        circleCollider2D.enabled = false;

        explosionInstantiated.SetActive(true);       

        spawner.chaserCounter -= 1;
    }

    private void Reanimate() {

        rb.simulated = true;
        spriteRenderer.enabled = true;
        circleCollider2D.enabled = true;
    }

    // --------------------------------------- Collisions --------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D collisionInfo) { 

        if (collisionInfo.gameObject.tag == "Beam") {

            guiManager.AddToScore(1);
            SelfDestruct(); 
        }
    }

    private void OnCollisionEnter2D(Collision2D collisionInfo) { 
        
        if (collisionInfo.gameObject.tag != "Chaser") { 
    
            SelfDestruct(); 
        }
    }
}