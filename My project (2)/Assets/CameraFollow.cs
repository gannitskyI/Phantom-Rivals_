using Cinemachine;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow instance;

    [SerializeField] private Transform target; // ������ �� ������, �� ������� ������ ������
    [SerializeField] private float accelerationThreshold = 5f; // ����� �������� ��� �������
    [SerializeField] private float decelerationThreshold = 2f; // ����� �������� ��� ����������
    [SerializeField] private float zoomInSpeed = 0.1f; // �������� �����������
    [SerializeField] private float zoomOutSpeed = 0.1f; // �������� ���������
    [SerializeField] private float minOrthographicSize = 2f; // ����������� �������� Orthographic Size
    [SerializeField] private float maxOrthographicSize = 15f; // ������������ �������� Orthographic Size
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    private float initialOrthographicSize; // ��������� ������ ��� Orthographic Size
    private int lastScreenHeight;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        lastScreenHeight = Screen.height;
        // ������������� ��������� �������� Orthographic Size � ����������� �� ������ ������
        UpdateCameraSize(Screen.height);
    }

    private void LateUpdate()
    {
        float currentSpeed = PlayerControl.instance.currentSpeed; // �������� ������� �������� ������

        // ���������� ������� ������ � ����������� �� ������� ��������
        UpdateCameraZoom(currentSpeed);

        // ��������� ��������� ������ ������
        if (Screen.height != lastScreenHeight)
        {
            UpdateCameraSize(Screen.height);
            lastScreenHeight = Screen.height;
        }
    }

    private void UpdateCameraSize(int screenHeight)
    {
        initialOrthographicSize = screenHeight > 800 ? 10f : 6f;
        virtualCamera.m_Lens.OrthographicSize = initialOrthographicSize;
    }

    private void UpdateCameraZoom(float speed)
    {
        float targetOrthographicSize = initialOrthographicSize;

        if (speed > accelerationThreshold)
        {
            targetOrthographicSize = Mathf.Lerp(initialOrthographicSize, maxOrthographicSize, (speed - accelerationThreshold) / (maxOrthographicSize - accelerationThreshold));
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(virtualCamera.m_Lens.OrthographicSize, targetOrthographicSize, Time.deltaTime * zoomOutSpeed);
        }
        else if (speed < decelerationThreshold)
        {
            targetOrthographicSize = Mathf.Lerp(minOrthographicSize, initialOrthographicSize, speed / decelerationThreshold);
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(virtualCamera.m_Lens.OrthographicSize, targetOrthographicSize, Time.deltaTime * zoomInSpeed);
        }
    }

    public void UpdateTarget()
    {
        foreach (GameObject car in PlayerCars.instance.cars)
        {
            if (car.activeInHierarchy)
            {
                target = car.transform;
                virtualCamera.Follow = target; // ��������� ������� ��� �������� � Cinemachine
                break;
            }
        }
    }
}
