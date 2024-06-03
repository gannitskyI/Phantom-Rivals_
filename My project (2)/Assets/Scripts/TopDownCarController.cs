using System.Collections;
using UnityEngine;

public class TopDownCarController : MonoBehaviour
{
    [Header("Car settings")]
    public float driftFactor = 0.95f;
    public float accelerationFactor = 30.0f;
    public float turnFactor = 3.5f;
    public float maxSpeed = 20;
    public float roadFriction = 0.8f;
    public float maxTurnSpeed = 5.0f;
    public float turnSmoothness = 5.0f;
    public float collisionForce = 500.0f;
    public float minDistanceToAvoidSticking = 0.5f;

    [Header("Sprites")]
    public SpriteRenderer carSpriteRenderer;
    public SpriteRenderer carShadowRenderer;

    private Rigidbody2D carRigidbody2D;
    private CarSfxHandler carSfxHandler;
    private bool botControlStarted = false;

    private float accelerationInput = 0;
    private float steeringInput = 0;
    private float rotationAngle = 0;
    private float velocityVsUp = 0;
    private bool isJumping = false;

    private void Awake()
    {
        carRigidbody2D = GetComponent<Rigidbody2D>();
        carSfxHandler = GetComponent<CarSfxHandler>();
    }

    private void Start()
    {
        rotationAngle = transform.eulerAngles.z;
        StartCoroutine(StartBotControlDelayed(0.0f));
    }

    private IEnumerator StartBotControlDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        botControlStarted = true;
    }

    private void FixedUpdate()
    {
        if (!botControlStarted) return;

        ApplyEngineForce();
        KillOrthogonalVelocity();
        ApplySteering();

        RaycastHit2D[] hitInfo = Physics2D.RaycastAll(transform.position, transform.up, 1.0f);
        foreach (RaycastHit2D hit in hitInfo)
        {
            if (hit.collider != null && (hit.collider.CompareTag("Player") || hit.collider.CompareTag("Bot")))
            {
                Rigidbody2D otherRigidbody = hit.collider.GetComponent<Rigidbody2D>();
                if (otherRigidbody != null && otherRigidbody != carRigidbody2D)
                {
                    Vector2 pushDirection = otherRigidbody.position - carRigidbody2D.position;
                    float distance = pushDirection.magnitude;
                    if (distance < minDistanceToAvoidSticking)
                    {
                        carRigidbody2D.AddForce(-pushDirection.normalized * collisionForce);
                    }
                    otherRigidbody.AddForce(pushDirection.normalized * collisionForce);
                }
            }
        }
    }

    private void ApplyEngineForce()
    {
        if (isJumping && accelerationInput < 0)
        {
            accelerationInput = 0;
        }

        carRigidbody2D.drag = Mathf.Lerp(carRigidbody2D.drag, accelerationInput == 0 ? 3.0f : 0, Time.deltaTime * 3);

        velocityVsUp = Vector2.Dot(transform.up, carRigidbody2D.velocity);
        velocityVsUp = Mathf.Clamp(velocityVsUp, -maxSpeed * 0.5f, maxSpeed);

        if (carRigidbody2D.velocity.sqrMagnitude > maxSpeed * maxSpeed && accelerationInput > 0 && !isJumping)
        {
            velocityVsUp *= 0.75f;
        }

        if (accelerationInput == 0)
        {
            velocityVsUp *= 0.9f;
        }

        carRigidbody2D.AddForce(transform.up * accelerationInput * accelerationFactor * roadFriction, ForceMode2D.Force);
    }

    private void ApplySteering()
    {
        float minSpeedBeforeAllowTurningFactor = carRigidbody2D.velocity.magnitude / maxTurnSpeed;
        minSpeedBeforeAllowTurningFactor = Mathf.Clamp01(minSpeedBeforeAllowTurningFactor);

        float modifiedTurnFactor = turnFactor * (1 + minSpeedBeforeAllowTurningFactor);
        float targetRotationAngle = rotationAngle - steeringInput * modifiedTurnFactor;
        rotationAngle = Mathf.Lerp(rotationAngle, targetRotationAngle, Time.deltaTime * turnSmoothness);

        carRigidbody2D.MoveRotation(rotationAngle);
    }

    private void KillOrthogonalVelocity()
    {
        Vector2 forwardVelocity = Vector2.Dot(carRigidbody2D.velocity, transform.up) * transform.up;
        Vector2 rightVelocity = Vector2.Dot(carRigidbody2D.velocity, transform.right) * transform.right;
        carRigidbody2D.velocity = forwardVelocity + rightVelocity * driftFactor;
    }

    private float GetLateralVelocity()
    {
        return Vector2.Dot(transform.right, carRigidbody2D.velocity);
    }

    public bool IsTireScreeching(out float lateralVelocity, out bool isBraking)
    {
        lateralVelocity = GetLateralVelocity();
        isBraking = false;

        if (isJumping) return false;

        if (accelerationInput < 0 && velocityVsUp > 0)
        {
            isBraking = true;
            return true;
        }

        return Mathf.Abs(GetLateralVelocity()) > 4.0f;
    }

    public void SetInputVector(Vector2 inputVector)
    {
        steeringInput = inputVector.x;
        accelerationInput = inputVector.y;
    }

    public float GetVelocityMagnitude()
    {
        return carRigidbody2D.velocity.magnitude;
    }
}
