using UnityEngine;
using DG.Tweening;

public class CloudMovement : MonoBehaviour
{
    public float speed = 2f; // Скорость движения облака
    public float fadeDuration = 1f; // Длительность затухания облака
    public float leftEdgeOffset = 1f; // Смещение от левой границы

    private float originalY; // Исходное значение Y
    private Tweener moveTween; // Ссылка на текущую анимацию перемещения

    void Start()
    {
        // Сохраняем исходное значение Y при старте
        originalY = transform.position.y;
    }

    void Update()
    {
        MoveCloud();
    }

    void MoveCloud()
    {
        // Получаем границы экрана в мировых координатах
        float leftEdge = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        float rightEdge = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;

        // Двигаем облако вправо
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        // Проверяем, если облако выходит за пределы правой границы экрана, перемещаем его к левой границе
        if (transform.position.x > rightEdge)
        {
            FadeOutAndMove(leftEdge - leftEdgeOffset);
        }
    }

    void FadeOutAndMove(float targetX)
    {
        Renderer renderer = GetComponent<Renderer>();

        // Прерываем текущую анимацию перемещения
        if (moveTween != null)
        {
            moveTween.Kill();
        }

        // Используем DOTween для анимации затухания
        renderer.material.DOFade(0f, fadeDuration)
            .OnComplete(() => OnFadeOutComplete(targetX));
    }

    void OnFadeOutComplete(float targetX)
    {
        // Преобразуем экранные координаты в мировые
        Vector2 newPosition = Camera.main.ScreenToWorldPoint(new Vector3(targetX, Random.Range(0f, Screen.height), 0f));
        // Используем исходное значение Y
        newPosition.y = originalY;

        // Используем DOTween для анимации перемещения с нулевой длительностью
        moveTween = transform.DOMove(newPosition, 0f)
            .OnComplete(() => OnMoveComplete());
    }

    void OnMoveComplete()
    {
        Renderer renderer = GetComponent<Renderer>();

        // Используем DOTween для анимации восстановления прозрачности
        renderer.material.DOFade(1f, fadeDuration);
    }
}
