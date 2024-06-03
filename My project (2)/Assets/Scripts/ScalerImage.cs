using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalerImage : MonoBehaviour
{
    private RectTransform rectTransform;
    private Canvas canvas;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        // Вызываем метод для установки размеров объекта Image при запуске игры
        ResizeImage();
    }

    void Update()
    {
        // Проверяем, изменилось ли разрешение экрана
        if (Screen.width != canvas.pixelRect.width || Screen.height != canvas.pixelRect.height)
        {
            // Вызываем метод для изменения размеров объекта Image при изменении разрешения экрана
            ResizeImage();
        }
    }

    void ResizeImage()
    {
        // Устанавливаем размеры объекта Image так, чтобы он занимал всю доступную область экрана
        rectTransform.sizeDelta = new Vector2(canvas.pixelRect.width, canvas.pixelRect.height);
    }
}
