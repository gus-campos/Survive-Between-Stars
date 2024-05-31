using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Unity.VisualScripting;
using UnityEngine;

public class MineGenerator : MonoBehaviour {

    // Foguete
    private Rocket rocket;
    // Posição raltiva-aleatória da mina gerada
    private Vector3 randMine;
    // Lista de minas geradas
    private List<GameObject> minesGenerated = new List<GameObject>();
    // Timer de geração das minas
    private float timer;
    // Prefab da mina
    [SerializeField] private GameObject minePrefab;
    // Tempo do timer
    [SerializeField] private float timerCriteria;
    // Distância da nave
    [SerializeField] private float distanceFromRocket;
    // Radius of generation
    [SerializeField] private float generationRadius;
    // Contador de minas geradas - é decrementado pela classe Mine
    public int mineCounter = 0;
    [SerializeField] private int mineMaxCount;

    // Velocidade variada
    public float mineVelocityNorm;
    [SerializeField] private float InitialMineVelocityNorm;
    private float initialTime;
    [SerializeField] private float normScaling;
    
    public void ResetMineVelocity() {

        // Salvando tempo inicial
        mineVelocityNorm = InitialMineVelocityNorm;
    }

    private void Start() {

        // Capturar objeto do foguete
        rocket = GameObject.FindObjectOfType<Rocket>();

        // Salvando tempo inicial
        ResetMineVelocity();
    }

    private void Update() {

        // Atualizando velocidade global das minas
        mineVelocityNorm += Time.deltaTime * normScaling;

        // Gerando minas
        if (mineCounter < mineMaxCount) {

            // Incrementando o timer
            timer += Time.deltaTime;
            // Gerando posição aleatória
            randMine = Random.insideUnitCircle * generationRadius;
            // Se tiver passado 2 segundos e tiver distante o suficiente da nave
            if ((timer > timerCriteria) && (randMine.magnitude > distanceFromRocket)) {

                // Gerar e adionar à lista de minas geradas 
                minesGenerated.Add(Object.Instantiate(minePrefab, 
                                                    rocket.transform.position + randMine, 
                                                    Quaternion.identity, transform));

                // Incrementar contador de minas
                mineCounter += 1;

                // Reiniciar timer
                timer = 0;
            }
        }
    }

    // Método que destrói todas as minas geradas
    public void DestroyAll() {

        // Para cada mina gerada na lista
        foreach ( GameObject mine in minesGenerated ) {

            // Destruir
            Destroy(mine);
        }

        // Reinicializando contador de minas
        mineCounter = 0;
    }
}
