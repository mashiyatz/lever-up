using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public int score = 0;
    [SerializeField] private TextMeshProUGUI scoreTextbox;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private TextMeshProUGUI timeTextbox;
    [SerializeField] private TextMeshProUGUI bestScoreTextbox;
    [SerializeField] private TextMeshProUGUI yourScoreTextbox;
    

    private void OnEnable()
    {
        BulletBehavior.OnEnemyDeath += UpdateScore;
        GameManager.OnNewTime += UpdateTime;
        GameManager.OnGameOver += ShowGameOver;
        JaegerBehavior.OnHoverCam += () => scoreTextbox.enabled = true;
        JaegerBehavior.OnDashCam += () => scoreTextbox.enabled = false;
        GameManager.OnTitleScreen += ShowTitle;
        GameManager.OnPlay += SetPlayUI;
    }

    private void OnDisable()
    {
        BulletBehavior.OnEnemyDeath -= UpdateScore;
        GameManager.OnNewTime -= UpdateTime;
        GameManager.OnGameOver -= ShowGameOver;
        JaegerBehavior.OnHoverCam -= () => scoreTextbox.enabled = true;
        JaegerBehavior.OnDashCam -= () => scoreTextbox.enabled = false;
        GameManager.OnPlay -= SetPlayUI;
    }

    void UpdateScore()
    {
        score++;
        scoreTextbox.text = $"Score: {score}";
    }

    void UpdateTime(float timeRemaining)
    {
        timeTextbox.text = $"{timeRemaining:000}";
    }

    void SetPlayUI()
    {
        score = 0;
        scoreTextbox.enabled = true;
        titlePanel.SetActive(false);
        timeTextbox.enabled = true;
}

    void ShowTitle()
    {
        titlePanel.SetActive(true);
    }

    void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        scoreTextbox.enabled = false;
        timeTextbox.enabled = false;
        yourScoreTextbox.text = $"Your Score: {score}";

        int best = PlayerPrefs.GetInt("Best", 0);
        if (score > best)
        {
            bestScoreTextbox.text = $"Best: YOU!";
        } else
        {
            bestScoreTextbox.text = $"Best: {score}";
            PlayerPrefs.SetInt("Best", score);
        }
    }
}
