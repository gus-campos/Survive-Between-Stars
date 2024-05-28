using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

using System.Globalization;
using System.Data;

public class GuiManager : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI TimeText;
    [SerializeField] private TextMeshProUGUI scoreText;
    // Tempo desde o início da sessão
    private float retryTime = 0f;
    // Contador de pontos do score
    private int scoreCount = 0;
    
    
    void Update() {

        // Atualizar o texto do relógio com o tempo formatado
        TimeText.text = FormatTime(Time.time - retryTime);
    }

    private string FormatTime(float time) {

        // Retornar número fomatado em 00:00
        return ((int)time/60).ToString("00")  + ":" + ((int)time%60).ToString("00");
    }

    public void ResetScore() {

        // Zerar score
        scoreCount = 0;
        // marca novo ponto 0 de contagem do tempo
        retryTime = Time.time;

        // Mostra tempo atualizado
        TimeText.text = FormatTime(Time.time - retryTime);
        // Mostra score atualizado
        scoreText.text = scoreCount.ToString("0");
    }


    public void AddToScore(int number) {

        // Adicionar número ao Score
        scoreCount += number;
        // Mudar o texto do score de acordo
        scoreText.text = scoreCount.ToString();
    }

}
