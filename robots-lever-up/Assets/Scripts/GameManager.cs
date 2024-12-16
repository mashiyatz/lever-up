using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum STATE { TITLE, PLAY, GAMEOVER }
    public STATE currentState;

    [SerializeField] private float timeLimit;
    public static float timeRemaining;

    public static event Action OnTitleScreen;
    public static event Action OnNewTime;
    public static event Action OnGameOver;
    public static event Action OnPlay;

    [SerializeField] JaegerBehavior playerControl;

    void Start()
    {
        playerControl.enabled = false; 
        timeRemaining = timeLimit;
        SetState(STATE.TITLE);
    }

    void Update()
    {
        if (currentState == STATE.TITLE)
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                SetState(STATE.PLAY);
            }
        }
        else if (currentState == STATE.PLAY)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                OnNewTime();
            }
            else
            {
                SetState(STATE.GAMEOVER);
            }
        }

        else if (currentState == STATE.GAMEOVER)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    public void SetState(STATE newState)
    {
        if (currentState != newState)
        {
            if (newState == STATE.TITLE) {
                OnTitleScreen();
                playerControl.enabled = false;
            }
            else if (newState == STATE.PLAY)
            {
                OnPlay();
                playerControl.enabled = true;
            }
            else if (newState == STATE.GAMEOVER)
            {
                OnGameOver();
            }
            currentState = newState;
        }
    }
}
