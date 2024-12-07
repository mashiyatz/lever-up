using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JaegerMovement : MonoBehaviour
{
    public enum LeftLegState { Q, W, E, R, T, Y, U, REST }
    public enum RightLegState { Z, X, C, V, B, N, M, REST }
    public enum MechAction { RIGHT, LEFT, FORWARD, BACKWARD, IDLE }

    private LeftLegState currentLeftLegState = LeftLegState.REST;
    private RightLegState currentRightLegState = RightLegState.REST;
    private MechAction lastAction = MechAction.IDLE;

    private KeyCode[] leftLegKeys;
    private KeyCode[] rightLegKeys;

    private int lastLeftLeg;
    private int lastRightLeg;

    private int leftLeg;
    private int rightLeg;

    private float leftLegTimer = 0;
    private float rightLegTimer = 0;
    [SerializeField] private float legHoldTime = 0.1f;

    private Queue<MechAction> actionQueue;
    private bool isActionRegistered;

    void Start()
    {
        // initialize keys
        KeyCode[] leftKeys = { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T, KeyCode.Y, KeyCode.U };
        leftLegKeys = leftKeys;
        KeyCode[] rightKeys = { KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V, KeyCode.B, KeyCode.N, KeyCode.M };
        rightLegKeys = rightKeys;

        actionQueue = new Queue<MechAction>();

        StartCoroutine(ExecuteAction());
    }

    // handle input
    void Update()
    {
        GetCurrentLegInput(leftLegKeys, false);
        GetCurrentLegInput(rightLegKeys, true);

        if (leftLeg == lastLeftLeg) leftLegTimer += Time.deltaTime;
        else
        {
            isActionRegistered = false;
            leftLegTimer = 0;
            lastLeftLeg = leftLeg;
        }
        if (rightLeg == lastRightLeg) rightLegTimer += Time.deltaTime;
        else
        {
            isActionRegistered = false;
            rightLegTimer = 0;
            lastRightLeg = rightLeg;
        }

        if (leftLegTimer > legHoldTime && rightLegTimer > legHoldTime) return;
        else if (leftLegTimer > legHoldTime || (rightLegTimer > legHoldTime))
        {
            if (isActionRegistered) return;
            currentLeftLegState = (LeftLegState)leftLeg;
            currentRightLegState = (RightLegState)rightLeg;
            QueueAction();
            isActionRegistered = true;
        }
    }

    // render input
    IEnumerator ExecuteAction()
    {
        while (true)
        {
            while (actionQueue.Count > 0)
            {
                MechAction _action = actionQueue.Dequeue();
                Debug.Log(_action);
                // actionQueue.TryPeek(out MechAction _nextAction);
                if (_action == MechAction.RIGHT)
                {
                    if (lastAction == MechAction.LEFT) Debug.Log("step forward");
                    else if (lastAction == MechAction.FORWARD) Debug.Log("rotate clockwise");
                }
                else if (_action == MechAction.LEFT)
                {
                    if (lastAction == MechAction.RIGHT) Debug.Log("step forward");
                    else if (lastAction == MechAction.FORWARD) Debug.Log("rotate counterclockwise");
                }
                else if (_action == MechAction.FORWARD)
                {
                    if (lastAction == MechAction.RIGHT) Debug.Log("rotate clockwise");
                    else if (lastAction == MechAction.LEFT) Debug.Log("rotate counterclockwise");
                    else if (lastAction == MechAction.BACKWARD) Debug.Log("aiming...");
                }

                yield return null;
            }
            yield return null;
        }
    }

    void QueueAction()
    {
        if (currentLeftLegState == LeftLegState.Q && currentRightLegState == RightLegState.M) actionQueue.Enqueue(MechAction.RIGHT);
        else if (currentLeftLegState == LeftLegState.Q && currentRightLegState == RightLegState.Z) actionQueue.Enqueue(MechAction.FORWARD);
        else if (currentLeftLegState == LeftLegState.U && currentRightLegState == RightLegState.Z) actionQueue.Enqueue(MechAction.LEFT);
    }

    void GetCurrentLegInput(KeyCode[] legKeys, bool isRightLeg)
    {
        for (int i = 0; i < legKeys.Length; i++)
        {
            if (Input.GetKeyDown(legKeys[i]))
            {
                if (isRightLeg) rightLeg = i;
                else leftLeg = i;
                
                return;
            }
        }
    }
}
