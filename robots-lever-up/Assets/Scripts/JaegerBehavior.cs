using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class JaegerBehavior : MonoBehaviour
{
    public enum LeftLegState { Q, W, E, R, T, Y, U, REST }
    public enum RightLegState { Z, X, C, V, B, N, M, REST }
    public enum MechInput { RIGHT, LEFT, FORWARD, BACKWARD, IDLE }

    private LeftLegState currentLeftLegState = LeftLegState.REST;
    private RightLegState currentRightLegState = RightLegState.REST;
    private MechInput lastInput = MechInput.IDLE;

    private KeyCode[] leftLegKeys;
    private KeyCode[] rightLegKeys;

    private int lastLeftLeg;
    private int lastRightLeg;

    private int leftLeg;
    private int rightLeg;

    private float leftLegTimer = 0;
    private float rightLegTimer = 0;
    [SerializeField] private float legHoldTime = 0.1f;

    private Queue<MechInput> inputQueue;
    private bool isInputRegistered;

    private Animator controller;
    private int rightLegUp = Animator.StringToHash("Armature|rightstep");
    private int leftLegUp = Animator.StringToHash("Armature|leftstep");
    private int turnLeft = Animator.StringToHash("Armature|left turn");
    private int turnRight = Animator.StringToHash("Armature|right turn");
    private int takeDamage = Animator.StringToHash("Armature|damage");
    private int fire = Animator.StringToHash("Armature|firegun");
    private int death = Animator.StringToHash("Armature|death");
    private int idle = Animator.StringToHash("Armature|idle");

    [SerializeField] private float rotationSpeed = 10;
    [SerializeField] private float movementSpeed = 5;
    private bool isAlive = true;

    [SerializeField] Transform nozzle;
    [SerializeField] Transform bulletPrefab;

    [SerializeField] CinemachineVirtualCamera dashcam;
    [SerializeField] CinemachineVirtualCamera hovercam;
    [SerializeField] GameObject dashboard;
    [SerializeField] ParticleSystem particles;

    public static event Action OnHoverCam;
    public static event Action OnDashCam;

    [SerializeField] AudioClip leftLegUpAudio;
    [SerializeField] AudioClip rightLegUpAudio;
    [SerializeField] AudioClip leftTurnAudio;
    [SerializeField] AudioClip rightTurnAudio;
    [SerializeField] AudioClip loadFireAudio;
    [SerializeField] AudioClip[] shootAudio;
    private AudioSource audioPlayer;

    void Start()
    {
        // initialize keys
        KeyCode[] leftKeys = { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T, KeyCode.Y, KeyCode.U };
        leftLegKeys = leftKeys;
        KeyCode[] rightKeys = { KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V, KeyCode.B, KeyCode.N, KeyCode.M };
        rightLegKeys = rightKeys;

        inputQueue = new Queue<MechInput>();
        controller = GetComponent<Animator>();
        audioPlayer = GetComponent<AudioSource>();

        ToggleCamera(false);

        StartCoroutine(ExecuteAction());
        
    }

    private void OnEnable()
    {
        GameManager.OnGameOver += PowerDown;
    }

    private void PowerDown()
    {
        controller.Play(death);
        ToggleCamera(false);
        isAlive = false;
    }

    void ToggleCamera(bool aiming)
    {
        if (aiming)
        {
            PlayAudio(loadFireAudio);
            hovercam.Priority = 5;
            dashcam.Priority = 10;
            dashboard.SetActive(true);
            OnDashCam();
        } else
        {
            hovercam.Priority = 10;
            dashcam.Priority = 5;
            dashboard.SetActive(false);
            OnHoverCam();
        }
    }

    bool IsCurrentAnim(int hash)
    {
        return controller.GetCurrentAnimatorStateInfo(0).shortNameHash == hash;
    }

    bool IsCurrentAnim(string name)
    {
        return controller.GetCurrentAnimatorStateInfo(0).IsName(name);
    }

    float GetAnimTime()
    {
        return controller.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    private void PlayAudio(AudioClip clip)
    {
        audioPlayer.Stop();
        audioPlayer.PlayOneShot(clip);
    }

    // handle input
    void Update()
    {
        if (!isAlive) return;

        if (!IsCurrentAnim(idle))
        {
            if (IsCurrentAnim(rightLegUp) || IsCurrentAnim(leftLegUp))
            {
                if (GetAnimTime() > 0.5) transform.Translate(movementSpeed * transform.forward * Time.deltaTime, Space.World);
            }
            else if (IsCurrentAnim(turnLeft))
            {
                transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0);
            }
            else if (IsCurrentAnim(turnRight))
            {
                transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
            }
            if (GetAnimTime() < 0.95) return;
        }

        // debugging input
        if (Input.GetKeyDown(KeyCode.Alpha1)) inputQueue.Enqueue(MechInput.RIGHT);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) inputQueue.Enqueue(MechInput.LEFT);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) inputQueue.Enqueue(MechInput.FORWARD);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) inputQueue.Enqueue(MechInput.BACKWARD);
        //

        GetCurrentLegInput(leftLegKeys, false);
        GetCurrentLegInput(rightLegKeys, true);

        if (leftLeg == lastLeftLeg) leftLegTimer += Time.deltaTime;
        else
        {
            isInputRegistered = false;
            leftLegTimer = 0;
            lastLeftLeg = leftLeg;
        }
        if (rightLeg == lastRightLeg) rightLegTimer += Time.deltaTime;
        else
        {
            isInputRegistered = false;
            rightLegTimer = 0;
            lastRightLeg = rightLeg;
        }

        if (leftLegTimer > legHoldTime && rightLegTimer > legHoldTime) return;
        else if (leftLegTimer > legHoldTime || (rightLegTimer > legHoldTime))
        {
            if (isInputRegistered) return;
            currentLeftLegState = (LeftLegState)leftLeg;
            currentRightLegState = (RightLegState)rightLeg;
            QueueAction();
            isInputRegistered = true;
        }
    }

    // render input
    IEnumerator ExecuteAction()
    {
        
        while (true)
        {
            if (!isAlive) break;
            while (inputQueue.Count > 0)
            {
                MechInput newInput = inputQueue.Dequeue();
                // Debug.Log(newInput);
                if (newInput == MechInput.RIGHT)
                {
                    if (lastInput == MechInput.LEFT || lastInput == MechInput.IDLE)
                    {
                        controller.Play(leftLegUp);
                        PlayAudio(leftLegUpAudio);
                        ToggleCamera(false);
                    }
                }
                else if (newInput == MechInput.LEFT)
                {
                    if (lastInput == MechInput.RIGHT || lastInput == MechInput.IDLE)
                    {
                        controller.Play(rightLegUp);
                        PlayAudio(rightLegUpAudio);
                        ToggleCamera(false);
                    }
                }
                else if (newInput == MechInput.FORWARD)
                {
                    if (lastInput == MechInput.RIGHT)
                    {
                        controller.Play(turnLeft);
                        PlayAudio(leftTurnAudio);
                    }
                    else if (lastInput == MechInput.LEFT)
                    {
                        controller.Play(turnRight);
                        PlayAudio(rightTurnAudio);
                    }
                    else if (lastInput == MechInput.BACKWARD)
                    {
                        PlayAudio(shootAudio[UnityEngine.Random.Range(0, shootAudio.Length)]);
                        controller.Play(fire);
                        Invoke(nameof(Shoot), 0.25f);
                    }
                }
                else if (newInput == MechInput.BACKWARD)
                {
                    if (lastInput == MechInput.RIGHT) controller.Play(turnLeft);
                    else if (lastInput == MechInput.LEFT) controller.Play(turnRight);
                    ToggleCamera(true);
                }

                lastInput = newInput;
                yield return null;
            }
            yield return null;
        }
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, nozzle);
        particles.Play();
    }
    public void TakeDamage()
    {
        inputQueue.Clear();
        controller.Play(takeDamage);
    }

    void QueueAction()
    {
        if (currentLeftLegState == LeftLegState.Q && currentRightLegState == RightLegState.M) inputQueue.Enqueue(MechInput.RIGHT);
        else if (currentLeftLegState == LeftLegState.Q && currentRightLegState == RightLegState.Z) inputQueue.Enqueue(MechInput.BACKWARD);
        else if (currentLeftLegState == LeftLegState.U && currentRightLegState == RightLegState.Z) inputQueue.Enqueue(MechInput.LEFT);
        else if (currentLeftLegState == LeftLegState.U && currentRightLegState == RightLegState.M) inputQueue.Enqueue(MechInput.FORWARD);
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
