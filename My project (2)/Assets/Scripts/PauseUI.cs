using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    // Ссылки на UI элементы
    public GameObject pauseMenu; // Окно паузы
    public TextMeshProUGUI countdownText; // Текст обратного отсчета

    // Переменные для управления паузой
    private bool isPaused = false; // Флаг паузы
    private float countdown = 3f; // Время обратного отсчета
    public bool isCountingDown = false; // Флаг обратного отсчета

    // Метод, который вызывается при нажатии на кнопку паузы
    public void PauseGame()
    {
        if (!isPaused) // Если игра не на паузе
        {
            isPaused = true; // Установить флаг паузы
            Time.timeScale = 0f; // Остановить время в игре
            pauseMenu.SetActive(true); // Активировать окно паузы

            // Сбросить переменные обратного отсчета
            isCountingDown = false;
            countdownText.gameObject.SetActive(false);
            countdown = 3f;
        }
    }

    // Метод, который вызывается при нажатии на кнопку продолжить игру
    public void ResumeGame()
    {
        if (isPaused) // Если игра на паузе
        {
            isPaused = false; // Снять флаг паузы
            pauseMenu.SetActive(false); // Деактивировать окно паузы
            isCountingDown = true; // Установить флаг обратного отсчета
            countdownText.gameObject.SetActive(true); // Активировать текст обратного отсчета

            AudioListener.volume = 1f;
        }
    }

    // Метод, который вызывается каждый кадр
    private void Update()
    {
        if (isCountingDown) // Если идет обратный отсчет
        {
            countdown -= Time.unscaledDeltaTime; // Уменьшить время обратного отсчета
            countdownText.text = Mathf.Ceil(countdown).ToString(); // Отобразить целое число секунд
            if (countdown <= 0f) // Если обратный отсчет закончился
            {
                isCountingDown = false; // Снять флаг обратного отсчета
                Time.timeScale = 1f; // Возобновить время в игре
                countdownText.gameObject.SetActive(false); // Деактивировать текст обратного отсчета
                countdown = 3f; // Сбросить время обратного отсчета
            }
        }
    }
}
