using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class Director : MonoBehaviour {

    // Audio source da trilha
    private AudioSource soundtrack;
    // Panel de game over
    [SerializeField] private GameObject gameOverPanel;
    // Panel de pause
    [SerializeField] private GameObject pausePanel;
    // Foguete
    [SerializeField] private Rocket rocket;
    // Gerador de minas
    [SerializeField] private MineGenerator mineGenerator;
    // GuiManager
    [SerializeField] private GuiManager guiManager;

    [SerializeField] private GameObject mainCamera;

    [SerializeField] private GameObject instructionsPanel;

    // Indicador de pause
    private bool paused = false;
    // Indicador de jogo terminado
    private bool gameOver = false;

    private bool aux = true;

    void Update() {

        // Apenas no primeiro update, pausar o jogo
        if (aux) { Pause(); aux = false; }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            
            Pause();
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            
            RestartGame();  
        }
    }

    public void Pause() {

        // Se não tiver em estado de game over
        if (!gameOver) {

            // Se não tiver pausado
            if (!paused) {

                Time.timeScale = 0;
                pausePanel.SetActive(true);
                paused = true;
                soundtrack.Stop();
                instructionsPanel.SetActive(true);
            }

            else {

                Time.timeScale = 1;
                pausePanel.SetActive(false);
                paused = false;
                soundtrack.Play();
                instructionsPanel.SetActive(false);
            }
        }
    }

    public void Start() {
        
        // Inicia pausado
        //Pause();

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
        // Indicar que acabou
        gameOver = true;
    }

    public void RestartGame() {

        // Esconder imagem de Game Over
        gameOverPanel.SetActive(false);
        // Descongelar tempo
        Time.timeScale = 1;
        // Reiniciar nave
        rocket.Reset();
        // Destruir tiros
        rocket.DestroyShots();
        // Reposicionando câmera
        mainCamera.GetComponent<CameraControl>().Move(rocket.transform.position);
        // Destruir todas as minas
        mineGenerator.DestroyAll();
        // Voltar trilha sonora
        soundtrack.Play();
        // Resetar score
        guiManager.ResetScore();
        // Indicar que voltou
        gameOver = false;
        // Mudando tempo inicial das minas
        mineGenerator.ResetMineVelocity();
    }
}
