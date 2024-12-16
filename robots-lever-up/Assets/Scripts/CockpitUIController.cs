using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CockpitUIController : MonoBehaviour
{
    private int cockpitScore = 0;
    public UIController cockpitHook;
    [SerializeField] private TextMeshProUGUI scoreTextCockpit;
    //grab the bar images in inspector
    [SerializeField] private Image leftBar;
    [SerializeField] private Image rightBar;

    private KeyCode[] leftLegKeys;
    private KeyCode[] rightLegKeys;
    [SerializeField] private float[] barsUI;

    private int leftLegUI;
    private int rightLegUI;

    void Start()
    {
        //initialize keys, cribbed from JaegerBehavior
        KeyCode[] leftKeys = { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T, KeyCode.Y, KeyCode.U };
        KeyCode[] rightKeys = { KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V, KeyCode.B, KeyCode.N, KeyCode.M };
        leftLegKeys = leftKeys;
        rightLegKeys = rightKeys;

        //encode percentage fills 0 to 100 for the Cockpit UI bars  !!!setting to inspector array instead
        //barsUI =  { 0.0f, 0.166f, 0.333f, 0.5f, 0.667f, 0.833f, 1.0f };
    }

    void Update()
    {
        //handle Cockpit UI bars
        //left bar
        GetCurrentInput(leftLegKeys, false);
        //right bar
        GetCurrentInput(rightLegKeys, true);

        //draw the bars
        //leftBar.fillAmount = barsUI[leftLegUI];
        //rightBar.fillAmount = barsUI[rightLegUI];
    }

    void GetCurrentInput(KeyCode[] legKeys, bool isRightLeg)
    {
        for (int i = 0; i < legKeys.Length; i++)
        {
            if (Input.GetKeyDown(legKeys[i]))
            {
                if (isRightLeg)
                {
                    rightLegUI = i;
                    rightBar.fillAmount = barsUI[rightLegUI];
                }
                else
                {
                    leftLegUI = i;
                    leftBar.fillAmount = barsUI[leftLegUI];
                }
                return;
            }
        }
    }
    
    private void OnEnable()
    {
        BulletBehavior.OnEnemyDeath += CockpitUpdateScore;
    }

    private void OnDisable()
    {
        BulletBehavior.OnEnemyDeath -= CockpitUpdateScore;
    }

    void CockpitUpdateScore()
    {
        cockpitScore++;
        scoreTextCockpit.text = $"Score: {cockpitScore}";
    }
}
