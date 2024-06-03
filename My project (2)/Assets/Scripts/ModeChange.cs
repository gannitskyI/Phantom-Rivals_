using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ModeChange : MonoBehaviour
{
    public GameObject[] bots;
    public int currentIndex = 0;
    public GameObject task;

    void Start()
    {
        LoadGameModeData();
        SetBotActivation(currentIndex);
    }

    private void LoadGameModeData()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "GameModeData.json");

        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            GameModeData data = JsonUtility.FromJson<GameModeData>(jsonData);
            currentIndex = (int)data.currentMode;
        }
    }

    private void SetBotActivation(int index)
    {
        string filePath = Path.Combine(Application.persistentDataPath, "GameModeData.json");

        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            GameModeData data = JsonUtility.FromJson<GameModeData>(jsonData);

            if (index == 1)
            {
                // Проверяем значение isTask из GameModeData
                if (data.isTask)
                {
                    // Если isTask == true, активируем ботов и задание
                    foreach (GameObject bot in bots)
                    {
                        bot.SetActive(true);
                    }
                    task.SetActive(true);
                }
                else
                {
                    // Если isTask == false, активируем только ботов
                    foreach (GameObject bot in bots)
                    {
                        bot.SetActive(true);
                    }
                    task.SetActive(false);
                }
            }
            else
            {
                // Деактивируем ботов и задание
                foreach (GameObject bot in bots)
                {
                    bot.SetActive(false);
                }
                task.SetActive(false);
            }
        }
    }
}
