using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageScale : MonoBehaviour
{
    private RectTransform rectTransform;
    private Image imageComponent;

    // Изначальные размеры изображения
    private Vector2 initialSize;

    // Start is called before the first frame update
    void Start()
    {
        // Получаем компоненты RectTransform и Image
        rectTransform = GetComponent<RectTransform>();
        imageComponent = GetComponent<Image>();

        // Сохраняем изначальные размеры изображения
        initialSize = rectTransform.sizeDelta;

        // Вызываем метод для изменения размера изображения при запуске
        ResizeImage();
    }

    // Update is called once per frame
    void Update()
    {
        // Вызываем метод для изменения размера изображения при изменении размера экрана
        ResizeImage();
    }

    void ResizeImage()
    {
        // Получаем текущие размеры экрана
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Рассчитываем соотношение сторон экрана к изначальному размеру (1920x1080)
        float widthRatio = screenWidth / 1920f;
        float heightRatio = screenHeight / 1080f;

        // Выбираем наименьший коэффициент масштабирования, чтобы изображение помещалось на экран
        float scaleRatio = Mathf.Min(widthRatio, heightRatio);

        // Применяем масштабирование к изначальным размерам изображения
        Vector2 newSize = initialSize * scaleRatio;

        // Применяем новый размер к RectTransform изображения
        rectTransform.sizeDelta = newSize;
    }
}
