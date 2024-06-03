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
    [SerializeField] private int[] taskCompletionGoals = new int[6]; // ������ � ����������� ��� ��� ������ ������
    [SerializeField] private Image[] taskFillImages; // ������ ����������� ��� ����������
    [SerializeField] public GameObject[] taskCompletionObjects; // ������ �������� ��� ���������� ������
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
        // ��������� ������ �� YandexGame.savesData.buttonTaskStatus
        ButtonTaskStatus buttonTaskStatus = YandexGame.savesData.buttonTaskStatus;

        // ���� ������ ����������
        if (buttonTaskStatus != null)
        {
            // ���������� ��� ������
            for (int i = 0; i < taskCompletionObjects.Length; i++)
            {
                // ���� ������ ���������
                if (buttonTaskStatus.TaskCompleted[i])
                {
                    // ������������ ������ ������
                    taskCompletionObjects[i].SetActive(false);
                    // ���������� ������ ���������� ������
                    taskComplited[i].SetActive(true);
                }
            }
        }
    }

    public void SaveButtonTaskStatus ()
    {
        ButtonTaskStatus buttonTaskStatus = YandexGame.savesData.buttonTaskStatus;
        // ������ ������� ��������� ������
        YandexGame.SaveProgress();
    }

    private void LoadTaskCompletionCounts()
    {
        // ��������� ������ �� YandexGame.savesData.taskCountData
        TaskCountData taskCountData = YandexGame.savesData.taskCountData;

        // ���� ������ ����������, ��������� ������� �����
        if (taskCountData != null && taskCountData.TaskCompletionCounts.Length == taskCountTexts.Length)
        {
            for (int i = 0; i < taskCountTexts.Length; i++)
            {
                int completedCount = Mathf.Min(taskCountData.TaskCompletionCounts[i], taskCompletionGoals[i]);
                taskCountTexts[i].text = $"{completedCount}/{taskCompletionGoals[i]}";
                float fillAmount = (float)completedCount / taskCompletionGoals[i];
                taskFillImages[i].fillAmount = fillAmount;

                // ���������, ������ �� ����� ����
                if (completedCount >= taskCompletionGoals[i])
                {
                    taskCompletionObjects[i].SetActive(true); // ���������� ������ ���������� ������
                }
            }
        }
        else
        {
            Debug.LogWarning("Task count data does not exist.");
        }
    }

}