using Cinemachine;
using UnityEngine;

public class SelfCamera : MonoBehaviour
{
    private PlayerControl carController;
    public Rigidbody2D rb;
    public CinemachineVirtualCamera virtualCamera;
    private CinemachinePOV povComponent;
    private CinemachineImpulseSource impulseSource;
    public float zoomFactor = 1f;
    public float zoomSpeed = 1f;
    public float maxZoom = 10f;
    public float minZoom = 1f;
    public float shakeIntensity = 2f;
    public float shakeDuration = 0.1f;
    private float shakeTimer = 0f;

    private void Start()
    {
        carController = GetComponent<PlayerControl>();
        povComponent = virtualCamera.GetCinemachineComponent<CinemachinePOV>();
        impulseSource = virtualCamera.GetComponent<CinemachineImpulseSource>();
    }

    private void Update()
    {
        // Smoothly follow the player's Rigidbody
        virtualCamera.Follow = rb.transform;

        // Zoom effect based on player speed
        float currentZoom = Mathf.Lerp(minZoom, maxZoom, rb.velocity.magnitude / carController.maxSpeed * zoomFactor); 
        virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(virtualCamera.m_Lens.OrthographicSize, currentZoom, Time.deltaTime * zoomSpeed);

        // Panning effect based on player rotation
        if (rb.angularVelocity != 0)
        {
            float angleDelta = Mathf.Clamp(rb.angularVelocity / carController.rotationSpeed * 30f, -30f, 30f);
            povComponent.m_HorizontalAxis.Value += angleDelta; // Change VerticalAxis to HorizontalAxis
        }

        // Apply camera shake
        if (shakeTimer > 0)
        {
            impulseSource.GenerateImpulse(shakeIntensity);
            shakeTimer -= Time.deltaTime;
        }
    }

    public void ShakeCamera()
    {
        shakeTimer = shakeDuration;
    }
}