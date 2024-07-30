using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Image = UnityEngine.UI.Image;

public class Spaceship : MonoBehaviour {

    // GameObjects
    private GameObject _dynamic;

    // Components
    private Rigidbody2D _rb;
    private Director _director;

    // Movements and rotating
    [SerializeField] private float speedLimit;
    [SerializeField] private float accelIntensity;
    [SerializeField] private float ignoreMouseRadius;

    // Initial stats
    private Vector3 _startingPosition;
    private Quaternion _startingRotation;
    private Vector3 _startingVelocity;
    
    // Baams shooting
    private float _shootCooldownTimer;
    private bool _shootAvailable = true;
    [SerializeField] private GameObject beamPrefab;
    [SerializeField] private float rocketLenth;
    [SerializeField] private float shootCooldownTimeout = 0.25f;

    // Beams pooling
    private List<GameObject> _beamsPool = new List<GameObject>();
    [SerializeField] private int beamsPooled;

    // Sons
    private AudioSource _rocketAudioSource;
    [SerializeField] private AudioClip Boom;
    [SerializeField] private AudioClip pewPew;
    [SerializeField] private AudioClip dashingSound;
    
    // Dashing
    private bool _dashing = false;
    private float _dashAvailableTimer;
    private bool _dashAvailable = true;
    private float _dashDurationTimer;
    private float _rocketMomentum;
    [SerializeField] private float dashAvailableTimeOut;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashMomentum;
    [SerializeField] private Image dashBar;
    [SerializeField] private Texture2D cursorTexture;

    // Input
    private bool _accelOn = false;
    private bool _mouseInputMode = true;

    // Life system
    private float _rocketHealth;
    [SerializeField] private float initialRocketHealth = 100f;
    [SerializeField] private Image healthBar;

    private float _damage = 34f;
    private float _knockback = 100f;

    // ==========================================================================================================
    
    // Este é um método que é automaticamente chamado pela Unity quando se cria um objeto
    void Awake() {

        // Components
        _rb = GetComponent<Rigidbody2D>();
        _rocketAudioSource = GetComponent<AudioSource>();

        // Storing initial stats 
        _startingPosition = transform.position;
        _startingRotation = transform.rotation;
        _startingVelocity = _rb.velocity;
    }

    void Start() {

        // GameObjects
        _director = GameObject.FindObjectOfType<Director>();
        _dynamic = GameObject.Find("_Dynamic"); 

        // Setting cursor
        Cursor.SetCursor(cursorTexture, 
                         new Vector2 (cursorTexture.width, cursorTexture.height) / 2, 
                         CursorMode.Auto);

        // Setting health
        _rocketHealth = initialRocketHealth;

        // Pooling beams
        for (int i=0; i<beamsPooled; i++) {
            
            _beamsPool.Add(Instantiate(beamPrefab, Vector3.zero, Quaternion.identity, _dynamic.transform));
        }

        DeactivateBeams();
    }

    void FixedUpdate() {
        
        UpdateDash();
    }

    void Update() {

        UpdateBeam();
        UpdateAccel();
        UpdateHealth();
    }

    // -------------------------------- Public, Controls ---------------------------------------------------------------

    public void UpdateRotation(InputAction.CallbackContext callbackContext) {   

        if (_mouseInputMode && !_dashing) {

            Vector2 inputVec = callbackContext.ReadValue<Vector2>();

            // Getting mouse position and nullifying z component and getting difference to spaceship
            Vector3 worldMousePosic = Camera.main.ScreenToWorldPoint(inputVec);
            worldMousePosic.z = 0;
            Vector3 rocketToMousePosic = worldMousePosic - transform.position;
            
            // Se o mouse não estiver muito próximo da nave
            if (rocketToMousePosic.magnitude > ignoreMouseRadius) {

                _rb.MoveRotation(Vector3.SignedAngle(rocketToMousePosic, Vector3.right, Vector3.back));
            }
        }
    }

    public void Shoot(InputAction.CallbackContext callbackContext) {

        if (!_dashing && _shootAvailable && _rb.simulated && callbackContext.performed) {

            // Getting pooled beam and setting transform
            GameObject tmpBeam = GetPooledBeam();
            // Resetting stats ans activating
            tmpBeam.transform.position = transform.position + transform.rotation * Vector3.right * rocketLenth;
            tmpBeam.transform.rotation = transform.rotation;
            tmpBeam.SetActive(true);
            // Sound
            _rocketAudioSource.PlayOneShot(pewPew, 1F);
            // Availability
            _shootAvailable = false;
            // Resetting timer
            _shootCooldownTimer = shootCooldownTimeout;
        }
    }

    public void Accel(InputAction.CallbackContext callbackContext) {

        if (!_dashing) {

            if (callbackContext.started) {_accelOn = true;}

            else if (callbackContext.canceled) {_accelOn = false;}
        }
    }

    public void Dash(InputAction.CallbackContext callbackContext) {

        if (!_dashing && callbackContext.performed && _dashAvailable) {

            // Resetting timers
            _dashDurationTimer = dashDuration;
            _dashAvailableTimer = dashAvailableTimeOut;
            // Storing pre-dash momentum
            _rocketMomentum = _rb.velocity.magnitude * _rb.mass;
            // Removing curent momentum (from where it's going)
            _rb.AddForce(-_rb.velocity.normalized * _rocketMomentum, ForceMode2D.Impulse);
            // Adding dash momentum
            _rb.AddForce(transform.right * dashMomentum, ForceMode2D.Impulse);

            _dashing = true;
            _rocketAudioSource.PlayOneShot(dashingSound, 1F);
        }
    }

    // -------------------------------- Private, Updates ---------------------------------------------------------------

    private void UpdateAccel() {

        if (_accelOn && !_dashing) {

            // Speed limiting
            if (_rb.velocity.magnitude <= speedLimit) {
                
                _rb.AddForce(accelIntensity * transform.right, ForceMode2D.Impulse);
            }

            else {

                // Removing accelIntensity from where it was going
                _rb.AddForce(-accelIntensity * _rb.velocity.normalized, ForceMode2D.Impulse);
                // Goving accelIntensity to where it's heading
                _rb.AddForce(accelIntensity * transform.right, ForceMode2D.Impulse);
            }
        }
    }

    private void UpdateBeam() {

        if (!_shootAvailable) {

            _shootCooldownTimer -= Time.deltaTime;

            if (_shootCooldownTimer <=0 ) _shootAvailable = true;
        }
    }

    private void UpdateDash() {

        // GUI update
        if (_dashAvailable) dashBar.fillAmount = 1f;

        else dashBar.fillAmount = 1 - (_dashAvailableTimer / dashAvailableTimeOut);

        // Dash update
        if (_dashing) {
         
            _dashDurationTimer -= Time.deltaTime;

            if (_dashDurationTimer <= 0f) {

                // Removing dash momentum
                _rb.AddForce(-transform.right * dashMomentum, ForceMode2D.Impulse);
                // Giving previous momentum back (to where it's going)
                _rb.AddForce(transform.right * _rocketMomentum, ForceMode2D.Impulse);
                
                _dashing = false;
                _dashAvailable = false;
            }
        }

        else if (!_dashAvailable) {

            _dashAvailableTimer -= Time.deltaTime;

            if (_dashAvailableTimer <= 0) {

                _dashAvailableTimer = dashAvailableTimeOut;
                _dashAvailable = true;
            }
        }
    }

    private void UpdateHealth() {

        // Updating GUI
        healthBar.fillAmount = 1 - (_rocketHealth / initialRocketHealth);

        if (_rocketHealth <= 0) { 
            
            if (!_director.gameOver) { 
                
                SelfDestruct(); 
            } 
        }
    }
   
   // -------------------------------- Private ------------------------------------------------------------------------

    private GameObject GetPooledBeam() {

        for (int i=0; i<beamsPooled; i++) {

            if (!_beamsPool[i].activeSelf) {

                return _beamsPool[i];
            }
        }
        return null;
    }

    private void DeactivateBeams() {

        foreach (GameObject beam in _beamsPool) {

            beam.SetActive(false);
        }
    }
    
    private void SelfDestruct() {

        // Tocar o som de explosão
        _rocketAudioSource.PlayOneShot(Boom, 0.8F);
        // Parar de simular sua física
        _rb.simulated = false;   
        // Congelar o tempo
        _director.EndGame();
    }

    public void Reset() {

        DeactivateBeams();

        // Voltar para o status inicial
        transform.position = _startingPosition;
        transform.rotation = _startingRotation;
        _rb.velocity = _startingVelocity;
        
        // Resetando timers
        _dashAvailableTimer = 0;
        _shootCooldownTimer = 0;
        _dashDurationTimer = 0;
        
        // Resetando bools
        _dashing = false;
        _dashAvailable = true;
        _shootAvailable = true;
        _rocketHealth = initialRocketHealth;
        
        // Ligar simulação física
        _rb.simulated = true;
    }

    // ----------------------------------- Private, Collision ----------------------------------------------------------
    private void OnCollisionEnter2D(Collision2D collisionInfo) { 
            
        string tag = collisionInfo.gameObject.tag;

        if (tag != "Untagged") {

            _rocketHealth -= _damage; 
            
            if (!_dashing) {

                _rb.AddForce(_rb.mass * _knockback * (transform.position - collisionInfo.transform.position).normalized, 
                            ForceMode2D.Impulse);
            }
        }
    }
}