using UnityEngine;
using UnityEngine.UI;

public class MobilePlayerControl : MonoBehaviour
{
    public float acceleration = 5f;
    public float deceleration = 2f;
    public float maxSpeed = 10f;
    public float rotationSpeed = 100f;
    public float driftIntensity = 0.5f;
    public float altDeceleration = 3f;
    public Rigidbody2D rb2d;
    public float currentSpeed;
    public float rotation; // Add this line to declare the 'rotation' variable
    public int direction = 1;
    public GameObject loseWindow;
    private CanvasFunction canvasFunction;
    private bool canControl = true;
    private float controlDisableTime = 3f;
    private int lapCount = 0;
    private PlayerCars playerCars;
 
    // References to UI elements
    public RectTransform steeringWheel;
    public Button accelerateButton;
    public Button brakeButton;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        DisableControlForSeconds(controlDisableTime);

        canvasFunction = FindObjectOfType<CanvasFunction>();

        // Attach event handlers to buttons
        accelerateButton.onClick.AddListener(OnAccelerateButtonClick);
        brakeButton.onClick.AddListener(OnBrakeButtonClick);
    }

    public void OnAccelerateButtonClick()
    {
        direction = 1;
    }

    public void OnBrakeButtonClick()
    {
        direction = -1;
    }

    private void FixedUpdate()
    {
        if (!canControl)
            return;

        // Handle rotation using the steering wheel's rotation
        float rotationInput = -steeringWheel.eulerAngles.z;
        rotation = rotationInput * rotationSpeed * Time.deltaTime * Mathf.Abs(currentSpeed / maxSpeed);
        if (Mathf.Abs(rotationInput) > 0 && Mathf.Abs(rb2d.velocity.magnitude) > 0)
            rotation *= driftIntensity * (1 + (currentSpeed / maxSpeed));

        // Handle acceleration and deceleration
        float brake = 0f;
        if (direction < 0)
            brake = deceleration;
        if (direction > 0 && direction < 0)
            currentSpeed = Mathf.Clamp(currentSpeed + acceleration * Time.deltaTime * direction * (1 + (currentSpeed / maxSpeed)), -maxSpeed, maxSpeed);
        else
            currentSpeed = Mathf.Clamp(currentSpeed - brake * Time.deltaTime - deceleration * Time.deltaTime * Mathf.Sign(currentSpeed) * (1 + (currentSpeed / maxSpeed)), -maxSpeed, maxSpeed);
        if (direction < 0)
            currentSpeed = Mathf.Clamp(currentSpeed - altDeceleration * Time.deltaTime * Mathf.Sign(currentSpeed) * (1 + (currentSpeed / maxSpeed)), -maxSpeed, maxSpeed);

        if (currentSpeed != 0)
        {
            transform.Rotate(0, 0, rotation);
        }

        if (currentSpeed == 0 && Mathf.Abs(rotationInput) == 0)
            rb2d.velocity = Vector2.zero;
        else
            rb2d.MovePosition(rb2d.position + (Vector2)transform.up * currentSpeed * Time.deltaTime);
    }

    public void SetPlayerCars(PlayerCars playerCars)
    {
        this.playerCars = playerCars;
    }

    public void IncrementLapCount()
    {
        lapCount++;

        if (lapCount >= 3)
        {
            // Выводим окно конца игры
            ShowEndGameWindow();
            canvasFunction.StopTimer();
        }
    }

    private void DisableControl()
    {
        canControl = false;
    }

    private void ShowEndGameWindow()
    {
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
}