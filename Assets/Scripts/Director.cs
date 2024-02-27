using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Director : MonoBehaviour {

    // Audio source da trilha
    private AudioSource soundtrack;
    // Panel de game over
    [SerializeField] private GameObject gameOverPanel;
    // Foguete
    [SerializeField] private Rocket rocket;
    // Gerador de minas
    [SerializeField] private MineGenerator mineGenerator;

    public void Start() {
        
        // Capturando nave
        rocket = GameObject.FindObjectOfType<Rocket>();
        // Capturando gerador de minas
        mineGenerator = GameObject.FindObjectOfType<MineGenerator>();
        // Capturando AudioSource
        soundtrack = GetComponent<AudioSource>();

    }

    public void EndGame() {

        // Congelar tempo
        Time.timeScale = 0;
        // Mostrar painel de Game Over
        gameOverPanel.SetActive(true);
        // Interromper trilha sonora
        soundtrack.Stop();
    
    }

    public void RestartGame() {

        // Esconder imagem de Game Over
        gameOverPanel.SetActive(false);
        // Descongelar tempo
        Time.timeScale = 1;
        // Reiniciar nave
        rocket.Reset();
        // Destruir todas as minas
        mineGenerator.DestroyAll();
        // Voltar trilha sonora
        soundtrack.Play();
    }
}
