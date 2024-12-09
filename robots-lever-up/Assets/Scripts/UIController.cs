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
    

    private void OnEnable()
    {
        BulletBehavior.OnEnemyDeath += UpdateScore;
        JaegerBehavior.OnPlayerDeath += ShowGameOver;
    }

    private void OnDisable()
    {
        BulletBehavior.OnEnemyDeath -= UpdateScore;
        JaegerBehavior.OnPlayerDeath -= ShowGameOver;
    }

    void UpdateScore()
    {
        score++;
        scoreTextbox.text = $"Score: {score}";
    }

    void ShowGameOver()
    {
        scoreTextbox.gameObject.SetActive(false);
        gameOverPanel.SetActive(true);
        Invoke(nameof(ResetGame), 3.0f);
    }

    private void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
