using System.Collections.Generic;
using TMPro;
using UnityEngine;
using YG;

public class PlayerControl : MonoBehaviour
{
    [Header("Trail Renderers")]
    public List<TrailRenderer> trailRenderers;
    private float newTime;
    [Header("Movement")]
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float deceleration = 4f;
    [SerializeField] public float maxSpeed = 10f;
    [SerializeField] public float rotationSpeed = 100f;
    [SerializeField] private float driftThresholdSpeed = 5.0f;
    [SerializeField] private float driftIntensity = 0.2f;
    [SerializeField] private float rotationSmoothVelocity = 0f;
    [SerializeField] private float targetRotationSpeed = 0f;
    [SerializeField] private float rotationSmoothTime = 0.2f;
    [SerializeField] private bool isDrifting = false;
    [SerializeField] private float driftTimer = 0f;
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
    private bool isBraking = false;
    private bool accelerating = false;
    private bool braking = false;
    private bool hasTaskBeenChecked = false;

    [SerializeField] private bool playerIsfinised = false;
    private bool canControl = true;
    private float controlDisableTime = 3f;
    private bool isInvincible = false;
    private float invincibilityTimer = 0f;
    public int lapCount;
    private CanvasFunction canvasFunction;
    private PlayerCars playerCars;
    private bool isTurningLeft = false;
    private bool isTurningRight = false;
    private bool HandleKeyboard = false;
    public static bool isKeyboardControlSelected = false;

    public TextMeshProUGUI TimeCirclePlayer1;
    public TextMeshProUGUI TimeCirclePlayer2;
    private TaskChecker taskChecker;
    private ModeChange modeChange;
 

    private void Start()
    {
        modeChange = FindObjectOfType<ModeChange>();

       if (modeChange.currentIndex == 1) 

        {
            taskChecker = FindObjectOfType<TaskChecker>();
        }
      
        rb2d = GetComponent<Rigidbody2D>();
        canvasFunction = FindObjectOfType<CanvasFunction>();
        DisableControlForSeconds(controlDisableTime);

        // Disable all TrailRenderers at the start
        foreach (var renderer in trailRenderers)
        {
            renderer.enabled = false;
        }
    }
 

    private void Update()
    {
        UpdateInvincibilityTimer();
        if (HandleKeyboard)
        {
            HandleKeyboardInput();
        }
 
            if (currentSpeed >= 20f)
            {
                if (taskChecker != null)
                {
                    taskChecker.FiveTask();
                    hasTaskBeenChecked = true;
                }
               
            }
        
    }
    public void IncrementLapCount()
    {
        lapCount++;

        if (lapCount >= 6)
        {
            playerIsfinised = true;
            canvasFunction.StopTimer();
            canvasFunction.CountdownRunning();
            // Проверяем наличие taskChecker перед его вызовом
            if (taskChecker != null)
            {
                taskChecker.CheckTask();
            }

            string timerText = canvasFunction.timerText.text;
            string[] timeParts = timerText.Split(':');
            int minutes = int.Parse(timeParts[0]);
            int seconds = int.Parse(timeParts[1]);
            int milliseconds = int.Parse(timeParts[2]);

            float lastLapTime = (minutes * 60) + seconds + (milliseconds / 1000f);

            // Добавляем условие для проверки номера игрока
            if (playerCars.currentPlayerIndex == 0)
            {
                // Если это первый игрок, то сохраняем его время в TimeCirclePlayer1
                TimeCirclePlayer1.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
                if (lastLapTime < canvasFunction.bestTime)
                {
                    canvasFunction.bestTime = lastLapTime;

                    int bestMinutes = (int)(canvasFunction.bestTime / 60);
                    int bestSeconds = (int)(canvasFunction.bestTime % 60);
                    int bestMilliseconds = (int)((canvasFunction.bestTime % 1) * 1000);
                    bestTimeText.text = string.Format("{0:00}:{1:00}:{2:000}", bestMinutes, bestSeconds, bestMilliseconds);

                    canvasFunction.SaveBestTime();
                    YandexGame.NewLBScoreTimeConvert("BestForOneMap", canvasFunction.bestTime);

                    // Устанавливаем bestTimePlayer1 в активное состояние
                    bestTimePlayer1.gameObject.SetActive(true);
                    // Выключаем плашку для второго игрока
                    bestTimePlayer2.gameObject.SetActive(false);
                }
            }
            else if (playerCars.currentPlayerIndex == 1)
            {
                // Если это второй игрок, то сохраняем его время в TimeCirclePlayer2
                TimeCirclePlayer2.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
                if (lastLapTime < canvasFunction.bestTime)
                {
                    canvasFunction.bestTime = lastLapTime;

                    int bestMinutes = (int)(canvasFunction.bestTime / 60);
                    int bestSeconds = (int)(canvasFunction.bestTime % 60);
                    int bestMilliseconds = (int)((canvasFunction.bestTime % 1) * 1000);
                    bestTimeText.text = string.Format("{0:00}:{1:00}:{2:000}", bestMinutes, bestSeconds, bestMilliseconds);

                    canvasFunction.SaveBestTime();
                    YandexGame.NewLBScoreTimeConvert("BestForOneMap", canvasFunction.bestTime);

                    // Устанавливаем bestTimePlayer2 в активное состояние
                    bestTimePlayer2.gameObject.SetActive(true);
                    // Выключаем плашку для первого игрока
                    bestTimePlayer1.gameObject.SetActive(false);
                }
            }

            int lapMinutes = (int)(lastLapTime / 60);
            int lapSeconds = (int)(lastLapTime % 60);
            int lapMilliseconds = (int)((lastLapTime % 1) * 1000);
            lapTimeText.text = string.Format("{0:00}:{1:00}:{2:000}", lapMinutes, lapSeconds, lapMilliseconds);
            ShowEndGameWindow();
            
            lapCount = 0;
        }
    }
    public void SetHandleKeyboard(bool value)
    {
        HandleKeyboard = value;
    }

    public void OnPCControl()
    {
        SetHandleKeyboard(true);
        isKeyboardControlSelected = true;
    }

    public void SetPCControl(bool isPCControl)
    {
        HandleKeyboard = isPCControl;
    }

    public bool IsHandlingKeyboard() => HandleKeyboard;

    private void FixedUpdate()
    {
        if (!canControl)
            return;

        HandleInput();
        UpdateSpeed();
        UpdateRotation();
        ApplyFriction();
        ApplyAerodynamicResistance();
        ApplyTurnForce();
        ApplyDriftEffect();
        MoveForward();

        // Check for long turns and enable/disable the trails
        if (isTurningLeft || isTurningRight)
        {
            driftTimer += Time.fixedDeltaTime;
            foreach (var renderer in trailRenderers)
            {
                if (renderer.enabled)
                {
                    // Increase the alpha channel of the trails
                    Color trailColor = renderer.material.color;
                    trailColor.a = Mathf.Lerp(trailColor.a, 1f, Time.fixedDeltaTime);
                    renderer.material.color = trailColor;
                }
                else
                {
                    renderer.enabled = true;
                }
            }
        }
        else
        {
            driftTimer = 0f;
            foreach (var renderer in trailRenderers)
            {
                if (renderer.enabled)
                {
                    // Decrease the alpha channel of the trails
                    Color trailColor = renderer.material.color;
                    trailColor.a = Mathf.Lerp(trailColor.a, 0f, Time.fixedDeltaTime);
                    renderer.material.color = trailColor;

                    // Disable the TrailRenderer when the alpha is low enough
                    if (trailColor.a < 0.1f)
                    {
                        renderer.enabled = false;
                    }
                }
            }
        }
    }

    private void HandleKeyboardInput()
    {
        if (!canvasFunction.isCountdownFinished)
            return;

        accelerating = Input.GetKey(KeyCode.W);
        braking = Input.GetKey(KeyCode.S);
        isTurningLeft = Input.GetKey(KeyCode.A);
        isTurningRight = Input.GetKey(KeyCode.D);
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

        // Проверяем скорость столкновения
        float collisionSpeed = dotProduct * weight;

        if (collisionSpeed > 20f) // Проверяем высокая ли скорость
        {
            rb2d.velocity -= (2 * dotProduct * normal);
        }
        else // Уменьшаем силу столкновения при низкой скорости
        {
            rb2d.velocity *= collisionSpeedReduction;
        }

        // Проверяем, является ли столкновение с другим ботом
        if (collision.gameObject.CompareTag("Bot"))
        {
            // Если да, то убеждаемся, что текущая скорость не станет меньше 0. С этим изменением машина не теряет скорость при столкновении с ботом.
            currentSpeed = Mathf.Max(currentSpeed, 0f);
        }
        else
        {
            // Если столкновение не с ботом, то применяем обычное уменьшение скорости
            currentSpeed *= (1f - collisionSpeedReduction);
        }

        rb2d.velocity = currentSpeed * rb2d.velocity.normalized;
        isInvincible = true;
    }
    private void HandleInput()
    {
        if (!canvasFunction.isCountdownFinished)
            return;

        int direction = (accelerating) ? 1 : ((braking) ? -1 : 0);

        float rotationInput = 0f;
        if (isTurningLeft)
        {
            rotationInput = 1f;
        }
        else if (isTurningRight)
        {
            rotationInput = -1f;
        }
        if (isTurningLeft || isTurningRight)
        {
            if (currentSpeed >= driftThresholdSpeed)
            {
                isDrifting = true;
            }
            else
            {
                isDrifting = false;
            }
        }
        else
        {
            isDrifting = false;
        }
        float targetSpeed = direction * maxSpeed;
        float desiredAcceleration = (targetSpeed > currentSpeed) ? acceleration : deceleration;

        if (direction == 0)
        {
            if (currentSpeed > 0)
            {
                currentSpeed = Mathf.Max(currentSpeed - deceleration * Time.deltaTime, 0f);
            }
            else if (currentSpeed < 0)
            {
                currentSpeed = Mathf.Min(currentSpeed + deceleration * Time.deltaTime, 0f);
            }
        }
        else
        {
            currentSpeed = Mathf.Clamp(currentSpeed + desiredAcceleration * Time.deltaTime * direction, -maxSpeed, maxSpeed);
        }

        // Здесь добавим плавное затухание поворота
        if (rotationInput == 0f)
        {
            targetRotationSpeed = Mathf.SmoothDamp(targetRotationSpeed, 0f, ref rotationSmoothVelocity, rotationSmoothTime);
        }
        else
        {
            float targetRotationSpeedInput = rotationInput * rotationSpeed * Mathf.Abs(currentSpeed / maxSpeed);
            targetRotationSpeed = Mathf.SmoothDamp(targetRotationSpeed, targetRotationSpeedInput, ref rotationSmoothVelocity, rotationSmoothTime);
        }
    }

    private void UpdateRotation()
    {
        float newRotation = transform.eulerAngles.z + targetRotationSpeed * Time.fixedDeltaTime;
        transform.rotation = Quaternion.Euler(0, 0, newRotation);
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
        Vector2 frictionForce = -rb2d.velocity.normalized * frictionCoefficient * weight;
        rb2d.AddForce(frictionForce);
    }

    private void ApplyAerodynamicResistance()
    {
        Vector2 aerodynamicResistanceForce = -rb2d.velocity.normalized * Mathf.Pow(rb2d.velocity.magnitude, 2) * aerodynamicResistanceCoefficient;
        rb2d.AddForce(aerodynamicResistanceForce);
    }

    private void ApplyTurnForce()
    {
        float turnForceMagnitude = rotation * turnAcceleration * rb2d.velocity.magnitude;
        Vector2 turnForce = transform.up * turnForceMagnitude;
        rb2d.AddForce(turnForce);
    }

    private void ApplyDriftEffect()
    {
        float currentSpeed = rb2d.velocity.magnitude;

        if (isDrifting && Mathf.Abs(currentSpeed) > driftThresholdSpeed)
        {
            float driftForceMagnitude = -rotation * driftIntensity * currentSpeed;
            Vector2 driftForce = transform.right * driftForceMagnitude;
            rb2d.AddForce(driftForce);

            // Ограничьте максимальную силу дрифта
            if (driftForce.magnitude > maxDriftForce)
            {
                driftForce = driftForce.normalized * maxDriftForce;
            }
        }
    }

    private void MoveForward()
    {
        rb2d.MovePosition(rb2d.position + (Vector2)transform.up * currentSpeed * Time.deltaTime);
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

    public void HandleGasButtonDown()
    {
        accelerating = true;
    }

    public void HandleGasButtonUp()
    {
        accelerating = false;
    }

    public void HandleBrakeButtonDown()
    {
        braking = true;
    }

    public void HandleBrakeButtonUp()
    {
        braking = false;
    }

    public void HandleLeftTurnButtonDown()
    {
        isTurningLeft = true;
    }

    public void HandleLeftTurnButtonUp()
    {
        isTurningLeft = false;
    }

    public void HandleRightTurnButtonDown()
    {
        isTurningRight = true;
    }

    public void HandleRightTurnButtonUp()
    {
        isTurningRight = false;
    }
    public bool IsTurning()
    {
        return isTurningLeft || isTurningRight;
    }

    public bool IsTurningLeft()
    {
        return isTurningLeft;
    }

    public bool IsTurningRight()
    {
        return isTurningRight;
    }
}
