using System.Collections.Generic;
using UnityEngine;

using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Object = UnityEngine.Object;
using Quaternion = UnityEngine.Quaternion;

public class BackgroundMover : MonoBehaviour {

    [SerializeField] private GameObject baseBackground;
    private GameObject _spaceship;

    private Sprite _sprite;
    private Vector3 _size;
    private Vector3 _initialPosition;
    private List<GameObject> _backgrounds = new List<GameObject>();
    private List<Vector3> _backgroundsInitialPosition = new List<Vector3>();

    // --------------------------------------------- Public ------------------------------------------------------------
    void Awake() {

        
    }

    void Start() {

        _spaceship = GameObject.FindGameObjectWithTag("Player");
        _sprite = baseBackground.GetComponent<SpriteRenderer>().sprite;

        _size = new Vector3((_sprite.rect.size/_sprite.pixelsPerUnit).x, 
                            (_sprite.rect.size/_sprite.pixelsPerUnit).y,
                             1.0f);

        _initialPosition = _spaceship.transform.position;

        // Generating initial positions of sprites in grid, and instantiating
        for (int i=-2; i<3; i++) {
            for (int j=-2; j<3; j++) {

                _backgroundsInitialPosition.Add(_initialPosition + new Vector3(i*_size.x, j*_size.y, 0f));

                _backgrounds.Add(Object.Instantiate(baseBackground, _backgroundsInitialPosition[^1], Quaternion.identity, 
                                                   transform));
            }
        }
    }

    void Update() {
        
        // Change in position since spawning
        Vector3 deltaPosition = _spaceship.transform.position - _initialPosition;

        // Find current gridIndex and update _sprite to it
        updateSprites(Snap(new Vector2(deltaPosition.x / _size.x,
                                       deltaPosition.y / _size.y)));
    }

    // --------------------------------------------- Private -----------------------------------------------------------
    private Vector2 Snap(Vector2 vec) {

        return new Vector2(Mathf.Round(vec.x),
                           Mathf.Round(vec.y));
    }

    private void updateSprites(Vector2 gridIndex) {

        Vector2 deltaCellPosition = _size * gridIndex;
        
        // Update each background position
        for (int i=0; i<_backgrounds.Count; i++) {

            _backgrounds[i].transform.position = _backgroundsInitialPosition[i] 
                                                + new Vector3(deltaCellPosition.x, 
                                                              deltaCellPosition.y, 
                                                              0.0f);
        }
    }
}
