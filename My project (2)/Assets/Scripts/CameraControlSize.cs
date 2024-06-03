using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlSize : MonoBehaviour
{
    // Ссылка на камеру, которую мы хотим контролировать
    public Camera gameCamera;

    // Ширина и высота окна при последнем обновлении
    private float lastScreenWidth;
    private float lastScreenHeight;

    void Start()
    {
        // Инициализация значений ширины и высоты экрана
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
    }

    void Update()
    {
        // Проверка, изменился ли размер экрана с момента последнего обновления
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            // Обновление значений ширины и высоты экрана
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;

            // Изменение размера камеры в соответствии с новыми размерами экрана
            UpdateCameraSize();
        }
    }

    void UpdateCameraSize()
    {
        // Расчет нового размера камеры
        float aspectRatio = (float)Screen.width / (float)Screen.height;
        float cameraSize = gameCamera.orthographicSize;

        // Обновление размера камеры
        gameCamera.orthographicSize = cameraSize * aspectRatio;
    }
}
