using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using static TaskChecker;
using YG;

public class MenuTask : MonoBehaviour
{
    [SerializeField] private Text[] taskCountTexts;
    [SerializeField] private int[] taskCompletionGoals = new int[6]; // Массив с количеством раз для каждой задачи
    [SerializeField] private Image[] taskFillImages; // Массив изображений для заполнения
    [SerializeField] public GameObject[] taskCompletionObjects; // Массив объектов для завершения задачи
    [SerializeField] public GameObject[] taskComplited;

    private AwardTask awardTask;
    private void Start()
    {
        LoadTaskCompletionCounts();
        LoadButtonTaskStatus();
        awardTask = FindObjectOfType<AwardTask>();
        awardTask.InitializeButtonTaskStatus();
    }

    private void LoadButtonTaskStatus()
    {
        // Загружаем данные из YandexGame.savesData.buttonTaskStatus
        ButtonTaskStatus buttonTaskStatus = YandexGame.savesData.buttonTaskStatus;

        // Если данные существуют
        if (buttonTaskStatus != null)
        {
            // Перебираем все задачи
            for (int i = 0; i < taskCompletionObjects.Length; i++)
            {
                // Если задача завершена
                if (buttonTaskStatus.TaskCompleted[i])
                {
                    // Деактивируем объект задачи
                    taskCompletionObjects[i].SetActive(false);
                    // Активируем объект завершения задачи
                    taskComplited[i].SetActive(true);
                }
            }
        }
    }

    public void SaveButtonTaskStatus ()
    {
        ButtonTaskStatus buttonTaskStatus = YandexGame.savesData.buttonTaskStatus;
        // Теперь остаётся сохранить данные
        YandexGame.SaveProgress();
    }

    private void LoadTaskCompletionCounts()
    {
        // Загружаем данные из YandexGame.savesData.taskCountData
        TaskCountData taskCountData = YandexGame.savesData.taskCountData;

        // Если данные существуют, обновляем счетчик задач
        if (taskCountData != null && taskCountData.TaskCompletionCounts.Length == taskCountTexts.Length)
        {
            for (int i = 0; i < taskCountTexts.Length; i++)
            {
                int completedCount = Mathf.Min(taskCountData.TaskCompletionCounts[i], taskCompletionGoals[i]);
                taskCountTexts[i].text = $"{completedCount}/{taskCompletionGoals[i]}";
                float fillAmount = (float)completedCount / taskCompletionGoals[i];
                taskFillImages[i].fillAmount = fillAmount;

                // Проверяем, достиг ли игрок цели
                if (completedCount >= taskCompletionGoals[i])
                {
                    taskCompletionObjects[i].SetActive(true); // Активируем объект завершения задачи
                }
            }
        }
        else
        {
            Debug.LogWarning("Task count data does not exist.");
        }
    }

}