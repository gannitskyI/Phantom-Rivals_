using DG.Tweening.Core.Easing;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using YG;

public class PlayerControl : MonoBehaviour
{
    public static PlayerControl instance;

    [Header("Trail Renderers")]
    public List<TrailRenderer> trailRenderers;

    [Header("Movement")]
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float deceleration = 4f;
    [SerializeField] public float maxSpeed = 10f;
    [SerializeField] public float rotationSpeed = 100f;
    [SerializeField] private float driftThresholdSpeed = 5.0f;
    [SerializeField] private float driftIntensity = 0.2f;
    [SerializeField] private float rotationSmoothTime = 0.2f;
    [SerializeField] private float maxDriftTime = 2.0f;
    [SerializeField] private float maxDriftForce = 20.0f;

    [Header("Physics")]
    [SerializeField] private float frictionCoefficient = 0.1f;
    [SerializeField] private float aerodynamicResistanceCoefficient = 0.05f;
    [SerializeField] private float weight = 1.0f;
    [SerializeField] private float turnAcceleration = 10.0f;
    [SerializeField] private float collisionSpeedReduction = 0.8f;
    [SerializeField] private float invincibilityTime = 0.5f;

    [Header("References")]
    public TextMeshProUGUI lapTimeText;
    public TextMeshProUGUI bestTimePlayer1;
    public TextMeshProUGUI bestTimePlayer2;
    public GameObject loseWindow;
    public TextMeshProUGUI bestTimeText;

    private Rigidbody2D rb2d;
    public float currentSpeed;
    private float rotation;
    private float targetRotationSpeed;
    private float rotationSmoothVelocity;
    private bool accelerating;
    private bool braking;
    private bool canControl = true;
    private bool isInvincible;
    private float invincibilityTimer;
    private int lapCount;
    private PlayerCars playerCars;
    private bool isTurningLeft;
    private bool isTurningRight;
    private bool isDrifting;
    private float driftTimer;
    private bool handleKeyboard;
    public static bool isKeyboardControlSelected;
    private CanvasFunction canvasFunction;
    private TaskChecker taskChecker;
    private ModeChange modeChange;

    public TextMeshProUGUI TimeCirclePlayer1;
    public TextMeshProUGUI TimeCirclePlayer2;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        modeChange = FindObjectOfType<ModeChange>();

        if (modeChange.currentIndex == 1)
        {
            taskChecker = FindObjectOfType<TaskChecker>();
        }

        rb2d = GetComponent<Rigidbody2D>();
        canvasFunction = FindObjectOfType<CanvasFunction>();
        DisableControlForSeconds(3f);

        foreach (var renderer in trailRenderers)
        {
            renderer.enabled = false;
        }

        CameraFollow.instance.UpdateTarget();
    }

    public void IncrementLapCount()
    {
        lapCount++;

        if (lapCount >= 6)
        {
            HandleLapCompletion();
            lapCount = 0;
        }
    }

    private void HandleLapCompletion()
    {
        canvasFunction.StopTimer();
        canvasFunction.CountdownRunning();

        taskChecker?.CheckTask();

        float lastLapTime = ParseTimerText(canvasFunction.timerText.text);

        if (playerCars.currentPlayerIndex == 0)
        {
            UpdateBestTimeUI(TimeCirclePlayer1, bestTimePlayer1, lastLapTime);
        }
        else if (playerCars.currentPlayerIndex == 1)
        {
            UpdateBestTimeUI(TimeCirclePlayer2, bestTimePlayer2, lastLapTime);
        }

        lapTimeText.text = FormatTime(lastLapTime);
        ShowEndGameWindow();
    }

    private void UpdateBestTimeUI(TextMeshProUGUI timeCircle, TextMeshProUGUI bestTimePlayer, float lastLapTime)
    {
        timeCircle.text = FormatTime(lastLapTime);
        if (lastLapTime > canvasFunction.bestTime)
        {
            canvasFunction.bestTime = lastLapTime;
            bestTimeText.text = FormatTime(canvasFunction.bestTime);

            canvasFunction.SaveBestTime();
            YandexGame.NewLBScoreTimeConvert("BestForOneMap", canvasFunction.bestTime);

            bestTimePlayer.gameObject.SetActive(true);
            TogglePlayerBestTimeUI(bestTimePlayer == bestTimePlayer1);
        }
    }

    private void TogglePlayerBestTimeUI(bool isPlayer1)
    {
        bestTimePlayer1.gameObject.SetActive(isPlayer1);
        bestTimePlayer2.gameObject.SetActive(!isPlayer1);
    }

    private float ParseTimerText(string timerText)
    {
        string[] timeParts = timerText.Split(':');
        int minutes = int.Parse(timeParts[0]);
        int seconds = int.Parse(timeParts[1]);
        int milliseconds = int.Parse(timeParts[2]);

        return (minutes * 60) + seconds + (milliseconds / 1000f);
    }

    private string FormatTime(float time)
    {
        int minutes = (int)(time / 60);
        int seconds = (int)(time % 60);
        int milliseconds = (int)((time % 1) * 1000);
        return $"{minutes:00}:{seconds:00}:{milliseconds:000}";
    }

    public void SetHandleKeyboard(bool value)
    {
        handleKeyboard = value;
    }

    public void OnPCControl()
    {
        SetHandleKeyboard(true);
        isKeyboardControlSelected = true;
    }

    public void SetPCControl(bool isPCControl)
    {
        handleKeyboard = isPCControl;
    }

    public bool IsHandlingKeyboard() => handleKeyboard;

    private void FixedUpdate()
    {
        UpdateInvincibilityTimer();
        if (handleKeyboard)
        {
            HandleKeyboardInput();
        }

        if (!canControl) return;

        HandleInput();
        UpdateSpeed();
        UpdateRotation();
        ApplyFriction();
        ApplyAerodynamicResistance();
        ApplyTurnForce();
        ApplyDriftEffect();
        MoveForward();
        ManageTrailRenderers();
    }

    private void HandleKeyboardInput()
    {
        if (!canvasFunction.isCountdownFinished) return;

        accelerating = Input.GetKey(KeyCode.W);
        braking = Input.GetKey(KeyCode.S);
        isTurningLeft = Input.GetKey(KeyCode.A);
        isTurningRight = Input.GetKey(KeyCode.D);
    }

    private void HandleInput()
    {
        if (!canvasFunction.isCountdownFinished) return;

        int direction = accelerating ? 1 : (braking ? -1 : 0);

        float rotationInput = isTurningLeft ? 1f : (isTurningRight ? -1f : 0f);

        isDrifting = (isTurningLeft || isTurningRight) && currentSpeed >= driftThresholdSpeed;

        float targetSpeed = direction * maxSpeed;
        float desiredAcceleration = targetSpeed > currentSpeed ? acceleration : deceleration;

        if (direction == 0)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.Clamp(currentSpeed + desiredAcceleration * Time.deltaTime * direction, -maxSpeed, maxSpeed);
        }

        targetRotationSpeed = rotationInput == 0 ? Mathf.SmoothDamp(targetRotationSpeed, 0f, ref rotationSmoothVelocity, rotationSmoothTime) : Mathf.SmoothDamp(targetRotationSpeed, rotationInput * rotationSpeed * Mathf.Abs(currentSpeed / maxSpeed), ref rotationSmoothVelocity, rotationSmoothTime);
    }

    private void UpdateRotation()
    {
        transform.rotation = Quaternion.Euler(0, 0, transform.eulerAngles.z + targetRotationSpeed * Time.fixedDeltaTime);
    }

    private void UpdateSpeed()
    {
        if (Mathf.Abs(rotation) > 0 && Mathf.Abs(rb2d.velocity.magnitude) > 0)
        {
            rotation *= driftIntensity * (1 + (currentSpeed / maxSpeed));
        }
    }

    private void ApplyFriction()
    {
        rb2d.AddForce(-rb2d.velocity.normalized * frictionCoefficient * weight);
    }

    private void ApplyAerodynamicResistance()
    {
        rb2d.AddForce(-rb2d.velocity.normalized * rb2d.velocity.sqrMagnitude * aerodynamicResistanceCoefficient);
    }

    private void ApplyTurnForce()
    {
        rb2d.AddForce(transform.up * rotation * turnAcceleration * rb2d.velocity.magnitude);
    }

    private void ApplyDriftEffect()
    {
        if (isDrifting && Mathf.Abs(currentSpeed) > driftThresholdSpeed)
        {
            rb2d.AddForce(transform.right * Mathf.Clamp(-rotation * driftIntensity * currentSpeed, -maxDriftForce, maxDriftForce));
        }
    }

    private void MoveForward()
    {
        rb2d.MovePosition(rb2d.position + (Vector2)transform.up * currentSpeed * Time.deltaTime);
    }

    private void ManageTrailRenderers()
    {
        if (isTurningLeft || isTurningRight)
        {
            driftTimer += Time.fixedDeltaTime;
            foreach (var renderer in trailRenderers)
            {
                AdjustTrailRenderer(renderer, 1f);
            }
        }
        else
        {
            driftTimer = 0f;
            foreach (var renderer in trailRenderers)
            {
                AdjustTrailRenderer(renderer, 0f);
            }
        }
    }

    private void AdjustTrailRenderer(TrailRenderer renderer, float targetAlpha)
    {
        if (renderer.enabled)
        {
            Color trailColor = renderer.material.color;
            trailColor.a = Mathf.Lerp(trailColor.a, targetAlpha, Time.fixedDeltaTime);
            renderer.material.color = trailColor;

            if (trailColor.a < 0.1f)
            {
                renderer.enabled = false;
            }
        }
        else if (targetAlpha > 0.1f)
        {
            renderer.enabled = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isInvincible)
        {
            HandleCollision(collision);
        }
    }

    private void HandleCollision(Collision2D collision)
    {
        Vector2 normal = collision.contacts[0].normal;
        float dotProduct = Vector2.Dot(rb2d.velocity, normal);

        float collisionSpeed = dotProduct * weight;

        if (collisionSpeed > 20f)
        {
            rb2d.velocity -= (2 * dotProduct * normal);
        }
        else
        {
            rb2d.velocity *= collisionSpeedReduction;
        }

        if (collision.gameObject.CompareTag("Bot"))
        {
            currentSpeed = Mathf.Max(currentSpeed, 0f);
        }
        else
        {
            currentSpeed *= (1f - collisionSpeedReduction);
        }

        rb2d.velocity = currentSpeed * rb2d.velocity.normalized;
        isInvincible = true;
    }

    private void UpdateInvincibilityTimer()
    {
        if (isInvincible)
        {
            invincibilityTimer += Time.deltaTime;
            if (invincibilityTimer >= invincibilityTime)
            {
                isInvincible = false;
                invincibilityTimer = 0f;
            }
        }
    }

    public void SetPlayerCars(PlayerCars playerCars)
    {
        this.playerCars = playerCars;
    }

    private void ShowEndGameWindow()
    {
        Time.timeScale = 0f;
        loseWindow.SetActive(true);
    }

    public void DisableControlForSeconds(float seconds)
    {
        canControl = false;
        Invoke(nameof(EnableControl), seconds);
    }

    private void EnableControl()
    {
        canControl = true;
    }

    public void HandleGasButtonDown() => accelerating = true;
    public void HandleGasButtonUp() => accelerating = false;
    public void HandleBrakeButtonDown() => braking = true;
    public void HandleBrakeButtonUp() => braking = false;
    public void HandleLeftTurnButtonDown() => isTurningLeft = true;
    public void HandleLeftTurnButtonUp() => isTurningLeft = false;
    public void HandleRightTurnButtonDown() => isTurningRight = true;
    public void HandleRightTurnButtonUp() => isTurningRight = false;
    public bool IsTurning() => isTurningLeft || isTurningRight;
    public bool IsTurningLeft() => isTurningLeft;
    public bool IsTurningRight() => isTurningRight;
}
