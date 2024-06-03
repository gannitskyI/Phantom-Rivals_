using UnityEngine;

public class lifeCar : MonoBehaviour
{
    public int maxLives = 3; // Максимальное количество жизней игрока
    private int currentLives; // Текущее количество жизней игрока
    public GameObject loseWindow;
    public GameObject[] obstacles; // Массив препятствий
    private GhostTrail ghostTrail;

    private void Start()
    {
        ghostTrail = GetComponent <GhostTrail>();
        currentLives = maxLives; // Устанавливаем начальное количество жизней
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Проверяем столкновение с препятствием, сравнивая с массивом obstacles
        foreach (GameObject obstacle in obstacles)
        {
            if (collision.gameObject == obstacle)
            {
                LoseLife(); // Вызываем метод потери жизни
                break; // Выходим из цикла, так как нашли препятствие
            }
        }
    }

    private void LoseLife()
    {
        currentLives--; // Уменьшаем количество жизней на 1

        if (currentLives <= 0)
        {
            EndGame(); // Если жизней не осталось, вызываем метод завершения игры
        }
    }

    private void EndGame()
    {
        loseWindow.SetActive(true);
    }
}
