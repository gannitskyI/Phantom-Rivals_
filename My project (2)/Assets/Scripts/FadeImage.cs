using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeImage : MonoBehaviour
{
    public Image imageToFade; // Объект Image, прозрачность которого нужно изменить
    public float duration = 1f; // Продолжительность анимации

    void Start()
    {
        // Убедитесь, что у вас есть ссылка на компонент Image
        if (imageToFade == null)
        {
            imageToFade = GetComponent<Image>();
        }

        // Проверьте, был ли найден компонент Image
        if (imageToFade != null)
        {
            imageToFade.color = new Color(imageToFade.color.r, imageToFade.color.g, imageToFade.color.b, 1f);
            // Измените прозрачность изображения до 0 в течение заданного времени
            imageToFade.DOFade(0f, duration);
        }
        else
        {
            Debug.LogError("Не удалось найти компонент Image для изменения прозрачности.");
        }
    }
}
