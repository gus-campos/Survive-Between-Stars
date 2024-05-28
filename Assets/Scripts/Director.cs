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
    // Indicador de pause
    private bool paused = false;
    // Indicador de jogo terminado
    private bool gameOver = false;

    void Update() {

        // Se tocar tecla esc
        if (Input.GetKeyDown(KeyCode.Escape)) {
            
            Pause();
            //RestartGame();  
        }


    }

    public void Pause() {

        // Se não tiver em estado de game over
        if (!gameOver) {

            // Se não tiver pausado
            if (!paused) {

                // Congelar tempo
                Time.timeScale = 0;
                // Ativar o painel de pause
                pausePanel.SetActive(true);
                // Declarar que está pausado
                paused = true;
                // Interromper trilha sonora
                soundtrack.Stop();

            }

            // Se estiver pausado
            else {

                // Descongelar tempo
                Time.timeScale = 1;
                // Dessativar o painel de pause
                pausePanel.SetActive(false);
                // Declarar que não está pausado
                paused = false;
                // Voltar trilha sonora
                soundtrack.Play();
            }
        }
    }

    public void Start() {
        
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
