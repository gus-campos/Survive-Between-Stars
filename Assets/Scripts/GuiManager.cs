using UnityEngine;
using TMPro;

public class GuiManager : MonoBehaviour {

    private int score = 0;
    private float survivingSince = 0f;
    // GUI elements
    [SerializeField] private TextMeshProUGUI TimeText;
    [SerializeField] private TextMeshProUGUI scoreText;
    
    void Update() {

        // Updates formated time in GUI
        TimeText.text = FormatTime(Time.time - survivingSince);
    }

    private string FormatTime(float time) {

        // Retornar número fomatado em 00:00
        return ((int)time/60).ToString("00")  + ":" + ((int)time%60).ToString("00");
    }

    public void ResetScore() {

        // Resets score
        score = 0;
        // Set new initial time for surviving (now)
        survivingSince = Time.time;
        // Update survived time to zero
        TimeText.text = FormatTime(Time.time - survivingSince);
        // Update score to zero
        scoreText.text = score.ToString("0");
    }


    public void AddToScore(int number) {

        // Increments score points
        score += number;
        // Update score showed in GUI accordingly 
        scoreText.text = score.ToString();
    }

}
