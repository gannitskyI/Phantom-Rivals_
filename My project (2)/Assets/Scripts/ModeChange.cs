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
                // ��������� �������� isTask �� GameModeData
                if (data.isTask)
                {
                    // ���� isTask == true, ���������� ����� � �������
                    foreach (GameObject bot in bots)
                    {
                        bot.SetActive(true);
                    }
                    task.SetActive(true);
                }
                else
                {
                    // ���� isTask == false, ���������� ������ �����
                    foreach (GameObject bot in bots)
                    {
                        bot.SetActive(true);
                    }
                    task.SetActive(false);
                }
            }
            else
            {
                // ������������ ����� � �������
                foreach (GameObject bot in bots)
                {
                    bot.SetActive(false);
                }
                task.SetActive(false);
            }
        }
    }
}
