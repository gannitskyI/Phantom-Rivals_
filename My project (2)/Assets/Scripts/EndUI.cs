using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using YG;
public class EndUI : MonoBehaviour
{
    [Header("References")]


    public TextMeshProUGUI bestTimeText;
     
    private CanvasFunction canvasFunction;
    private TaskChecker taskChecker;
    private ModeChange modeChange;

    private void Awake()
    {
        modeChange = FindObjectOfType<ModeChange>();
        canvasFunction = FindObjectOfType<CanvasFunction>();
        canvasFunction.PauseGame();
    }
    private void Start()
    {
        if (modeChange.currentIndex == 1)

        {
            taskChecker = FindObjectOfType<TaskChecker>();
        }
        LoadBestTime();
        if (taskChecker != null)
        {
            taskChecker.OpenBaner();
        }
 
    }

    public void LoadBestTime()
    {
        // Проверяем, существуют ли данные bestTimeData в savesData
        if (YandexGame.savesData.bestTimeData != null)
        {
            // Получаем лучшее время из savesData
            float bestTime = YandexGame.savesData.bestTimeData.bestTime;

            // Форматируем время
            int minutes = Mathf.FloorToInt(bestTime / 60);
            int seconds = Mathf.FloorToInt(bestTime % 60);
            int milliseconds = Mathf.FloorToInt((bestTime * 1000) % 1000);
            string formattedTime = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);

            // Обновляем текст лучшего времени
            bestTimeText.text = formattedTime;
        }
    }
}
