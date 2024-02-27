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

    private void Start() {

        // Capturar objeto do foguete
        rocket = GameObject.FindObjectOfType<Rocket>();

    }

    private void Update() {
        
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

            // Reiniciar timer
            timer = 0;
        }
    }

    // Método que destrói todas as minas geradas
    public void DestroyAll() {

        // Para cada mina gerada na lista
        foreach ( GameObject mine in minesGenerated ) {

            // Destruir
            Destroy(mine);

        }
    }
}
