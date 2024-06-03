using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FonAnimated : MonoBehaviour
{
    public Color targetColor;
    public Image image;
    public float animationSpeed = 1.0f;

    private void Start()
    {
        // Проверка наличия компонента Image
        if (image == null)
        {
            Debug.LogError("Image component is missing!");
            return;
        }

        // Запуск анимации только если компонент RectTransform существует
        RectTransform rectTransform = image.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("RectTransform component is missing!");
            return;
        }

        // Запуск анимации
        PlayFillAnimation();
    }

    private void PlayFillAnimation()
    {
        // Определение начального и конечного значения заливки
        float startFill = 255f;
        float endFill = 168f;

        // Создание последовательности анимации с помощью DOTween
        Sequence sequence = DOTween.Sequence();

        // Добавление анимации понижения заливки
        sequence.Append(image.DOColor(new Color(1, 1, 1, endFill / 255f), animationSpeed))
                .AppendInterval(1.0f) // Пауза перед обратной анимацией (можно изменить по необходимости)
                .Append(image.DOColor(new Color(1, 1, 1, startFill / 255f), animationSpeed))
                .SetLoops(-1) // Бесконечное повторение

                // Настройка сглаживания анимации
                .SetEase(Ease.InOutQuad);
    }
}
