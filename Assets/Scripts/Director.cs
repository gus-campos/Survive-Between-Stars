using UnityEngine;

public class Director : MonoBehaviour {

    private AudioSource ost;
    [SerializeField] private Spaceship spaceship;
    [SerializeField] private Spawner spawner;
    [SerializeField] private GuiManager guiManager;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject instructionsPanel;

    private bool paused = false;
    public bool gameOver = false;

    public void Start() {
        
        // Soundtrack, first audiosource component
        ost = GetComponents<AudioSource>()[0];

        // Start paused
        Pause();
    }

    public void Pause() {

        // If not in game over mode
        if (!gameOver) {

            // If not in pause mode, pause
            if (!paused) {

                // Stop time, ost, show pause panel
                Time.timeScale = 0;
                ost.Stop();
                // GUI
                instructionsPanel.SetActive(true);
                pausePanel.SetActive(true);
                // Pause state
                paused = true;
            }

            // If in pause mode, unpause
            else {

                // Running time, ost, show pause panel
                Time.timeScale = 1;
                ost.Play();
                instructionsPanel.SetActive(false);
                pausePanel.SetActive(false);
                // Pause state
                paused = false;
            }
        }
    }

    public void EndGame() {

        // Stop time, ost, show game over panel
        Time.timeScale = 0;
        ost.Stop();
        gameOverPanel.SetActive(true);
        // GameOver State
        gameOver = true;
    }

    public void RestartGame() {

        // Running time, play ost, reset camera and others
        Time.timeScale = 1;
        ost.Play();
        // Camera
        mainCamera.GetComponent<CameraControl>().MoveCamera(spaceship.transform.position);
        // Spawner
        spawner.DestroyAll();
        spawner.ResetChaserVelocity();
        // spaceship
        spaceship.Reset();
        //Gui 
        gameOverPanel.SetActive(false);
        guiManager.ResetScore();
        // GameOver State
        gameOver = false;
    }
}
