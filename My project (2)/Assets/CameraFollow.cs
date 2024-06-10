using Cinemachine;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow instance;

    [SerializeField] private Transform target; // Ссылка на объект, за которым следит камера
    [SerializeField] private float accelerationThreshold = 5f; // Порог скорости для разгона
    [SerializeField] private float decelerationThreshold = 2f; // Порог скорости для замедления
    [SerializeField] private float zoomInSpeed = 0.1f; // Скорость приближения
    [SerializeField] private float zoomOutSpeed = 0.1f; // Скорость отдаления
    [SerializeField] private float minOrthographicSize = 2f; // Минимальное значение Orthographic Size
    [SerializeField] private float maxOrthographicSize = 15f; // Максимальное значение Orthographic Size
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    private float initialOrthographicSize; // Начальный размер для Orthographic Size
    private int lastScreenHeight;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        lastScreenHeight = Screen.height;
        // Устанавливаем начальные значения Orthographic Size в зависимости от высоты экрана
        UpdateCameraSize(Screen.height);
    }

    private void LateUpdate()
    {
        float currentSpeed = PlayerControl.instance.currentSpeed; // Получаем текущую скорость машины

        // Обновление размера камеры в зависимости от текущей скорости
        UpdateCameraZoom(currentSpeed);

        // Проверяем изменение высоты экрана
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
                virtualCamera.Follow = target; // Установка объекта для слежения в Cinemachine
                break;
            }
        }
    }
}
