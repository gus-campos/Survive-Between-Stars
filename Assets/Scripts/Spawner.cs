using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    private GameObject _dynamic;
    private GameObject _spaceship;

    [SerializeField] private GameObject spaceshipPrefab;

    // Pooling
    [SerializeField] private GameObject chaserPrefab;
    [SerializeField] private int chasersPooled;
    private List<GameObject> chasersPool = new List<GameObject>();
    
    // Chaser activation and deactivation criteria
    [SerializeField] private float timerCriteria;
    [SerializeField] private int chaserMaxCount;
    [SerializeField] private float distanceFromRocket;
    [SerializeField] private float generationRadius;
    private float timer;
    public int chaserCounter = 0;

    // Varying speed
    public float chaserVelocityNorm = 0;
    [SerializeField] private float InitialChaserVelocityNorm;
    [SerializeField] private float normScaling;
    

    void Awake() {
        
        _dynamic = GameObject.Find("_Dynamic");
        
        _spaceship = Object.Instantiate(spaceshipPrefab, 
                                        Vector3.zero, 
                                        Quaternion.identity,
                                        _dynamic.transform); 
    }

    public void ResetChaserVelocity() {

        chaserVelocityNorm = InitialChaserVelocityNorm;
    }

    private void Start() {

        for (int i=0; i<chasersPooled; i++) {

            chasersPool.Add(Instantiate(chaserPrefab, Vector3.zero, Quaternion.identity, _dynamic.transform));
            chasersPool[^1].SetActive(false);
        }

        ResetChaserVelocity();
    }

    private void Update() {

        chaserVelocityNorm += Time.deltaTime * normScaling;

        // Gerando minas
        if (chaserCounter < chaserMaxCount) {

            timer += Time.deltaTime;
            
            // Random position
            Vector3 randPosition = Random.insideUnitCircle * generationRadius;

            // Se tiver passado 2 segundos e tiver distante o suficiente da nave
            if ((timer > timerCriteria) && (randPosition.magnitude > distanceFromRocket)) {

                GameObject tmpChaser = GetPooledChaser();
                tmpChaser.transform.position = _spaceship.transform.position + randPosition;
                tmpChaser.SetActive(true);

                chaserCounter += 1;
                timer = 0;
            }
        }
    }

    private GameObject GetPooledChaser() {

        for (int i=0; i<chasersPooled; i++) {

            if (!chasersPool[i].activeSelf) {

                return chasersPool[i];
            }
        }
        return null;
    }

    public void DestroyAll() {

        foreach ( GameObject chaser in chasersPool ) {

            chaser.SetActive(false);
        }

        chaserCounter = 0;
    }
}
