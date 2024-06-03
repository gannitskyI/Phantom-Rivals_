using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndCheck : MonoBehaviour
{
    [SerializeField] private GameObject[] banners;
    [SerializeField] private GameObject noTask; // Reference to the TextMeshPro object for displaying "no task" message

    private TaskChecker taskChecker;

    private void Start()
    {
        taskChecker = FindObjectOfType<TaskChecker>();
        if (taskChecker == null)
        {
            Debug.LogWarning("TaskChecker не найден");
            return;
        }
    }

    // Метод для открытия баннеров на основе статуса завершения задачи
    public void OpenBanners(int taskNumber, bool taskCompleted, int taskCompletionCount)
    {
        if (taskChecker == null)
        {
            Debug.LogWarning("TaskChecker не найден. Невозможно проверить статус задачи.");
            return;
        }

        int bannerIndex = taskNumber - 1;
        if (bannerIndex >= 0 && bannerIndex < banners.Length && banners[bannerIndex] != null)
        {
            if (taskCompleted)
            {
                // Code to open the banner (you can reuse your banner animation code here)
                banners[bannerIndex].SetActive(true);
                // Deactivate the "noTask" object if any banner becomes active
                if (noTask != null)
                {
                    noTask.SetActive(false);
                }
            }
        }
        else
        {
            Debug.LogError($"Invalid banner index for task {taskNumber}");
        }

        // Check if no banners are active, then activate the "noTask" object
        bool anyBannerActive = false;
        foreach (GameObject banner in banners)
        {
            if (banner != null && banner.activeSelf)
            {
                anyBannerActive = true;
                break;
            }
        }
        if (!anyBannerActive && noTask != null)
        {
            noTask.SetActive(true);
        }
    }
}
