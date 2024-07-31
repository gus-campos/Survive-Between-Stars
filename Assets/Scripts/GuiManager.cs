using UnityEngine;
using TMPro;

public class GuiManager : MonoBehaviour {

    private float _survivingSince = 0f;
    private int _score = 0;
    // GUI elements
    [SerializeField] private TextMeshProUGUI TimeText;
    [SerializeField] private TextMeshProUGUI scoreText;
    
    
    void Update() {

        // Updates formated time in GUI
        TimeText.text = FormatTime(Time.time - _survivingSince);
    }

    private string FormatTime(float time) {

        // Retornar número fomatado em 00:00
        return ((int)time/60).ToString("00")  + ":" + ((int)time%60).ToString("00");
    }

    public void ResetScore() {

        // Resets score
        _score = 0;
        // Set new initial time for surviving (now)
        _survivingSince = Time.time;
        // Update survived time to zero
        TimeText.text = FormatTime(Time.time - _survivingSince);
        // Update score to zero
        scoreText.text = _score.ToString("0");
    }


    public void AddToScore(int number) {

        // Increments score points
        _score += number;
        // Update score showed in GUI accordingly 
        scoreText.text = _score.ToString();
    }

}
