using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;
using System;

public class SelectorCars : MonoBehaviour
{
    [System.Serializable]
    public struct PlayerData
    {
        public int playerIndex;
        public int selectedCarIndex;
        public int selectedMode;
    }

    public List<PlayerData> playerDataList = new List<PlayerData>();
    public Dictionary<int, int> playerSelectedCars = new Dictionary<int, int>();
    private static SelectorCars instance;
    public MainMenu mainMenu;
    public event Action SelectorCarsAppeared;

    public static SelectorCars Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SelectorCars>();
            }
            return instance;
        }
    }

    private int[] selectedCars;
    public int currentPlayerIndex = 0;
    private Dictionary<int, bool> isPlayerSelected = new Dictionary<int, bool>();
    private bool allPlayersSelected = false;
    private bool carSelectionEnabled = true;
    private bool[] availableCars;
    public Button playButton;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        mainMenu = FindObjectOfType<MainMenu>();

        // �������� �������, ����� ���������� � ��������� SelectorCars
        SelectorCarsAppeared?.Invoke();
    }

    private void Start()
    {
        LoadData();
        selectedCars = new int[mainMenu.playerLabels.Length];
        availableCars = new bool[mainMenu.carIcons.Length];
        for (int i = 0; i < availableCars.Length; i++)
        {
            availableCars[i] = true;
        }
        playButton.interactable = false;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SaveData();
    }

    public void UpdatePlayerCount(int playerCount)
    {
        for (int i = 0; i < mainMenu.playerLabels.Length; i++)
        {
            bool isPlayerActive = i < playerCount;
            if (mainMenu.playerLabels[i] != null)
            {
                mainMenu.playerLabels[i].SetActive(isPlayerActive);

                if (isPlayerActive)
                {
                    Color labelColor = isPlayerSelected.ContainsKey(i) && isPlayerSelected[i] ? Color.green : Color.red;
                    SetPlayerLabelColor(i, labelColor);
                }
            }
        }

        if (currentPlayerIndex >= playerCount)
        {
            currentPlayerIndex = 0;
        }

        CheckAllPlayersSelected();

        if (allPlayersSelected)
        {
            DisableCarSelection();
            Debug.Log("��� ������ ������!");
        }
        else if (carSelectionEnabled)
        {
            SetPlayerLabelColor(currentPlayerIndex, Color.yellow);
        }
    }

    public void UpdateCarStatus(int carIndex, bool isPurchased)
    {
        availableCars[carIndex] = isPurchased;

        if (carIndex < mainMenu.carIcons.Length)
        {
            if (mainMenu.carIcons[carIndex] != null)
            {
                SetCarIconColor(carIndex, isPurchased ? Color.white : Color.black);
            }
            CheckAllPlayersSelected();

            if (allPlayersSelected)
            {
                DisableCarSelection();
                Debug.Log("��� ������ ������!");
            }
        }
    }

    public void SelectCarByPlayer(int carIndex)
    {
        mainMenu.PlayButtonSound();
        // ���������, ���� �� �������� label, � ���������� ������������ ���������� �������, ������� ����� ������� ������
        int maxPlayers = mainMenu.playerLabels.Count(label => label.activeSelf);

        bool isCarSelectable = !allPlayersSelected &&
                               currentPlayerIndex < maxPlayers &&
                               !isPlayerSelected.ContainsKey(currentPlayerIndex) &&
                               carSelectionEnabled;

        if (isCarSelectable)
        {
            if (availableCars[carIndex])
            {
                // ������ ������� ������ �� ������������� currentPlayerIndex ����� ��������� ��������
                int playerIndex = currentPlayerIndex % maxPlayers;

                selectedCars[playerIndex] = carIndex;
                isPlayerSelected[playerIndex] = true;

                SetPlayerLabelColor(playerIndex, Color.green);
                Debug.Log($"����� {playerIndex + 1} ������ ������ {selectedCars[playerIndex]}");

                currentPlayerIndex = (currentPlayerIndex + 1) % mainMenu.playerLabels.Length;

                if (isPlayerSelected.Count == maxPlayers)
                {
                    DisableCarSelection();
                    playButton.interactable = true;
                }
                else
                {
                    SetPlayerLabelColor(currentPlayerIndex, Color.yellow);
                }

                // ��������� ������� � ���������� �������� �������
                playerSelectedCars[playerIndex] = carIndex;

                // ��������� ������ ����� ������ ������� ������
                SaveData();
            }
        }
        else if (!carSelectionEnabled)
        {
            Debug.Log("����� ��������!");
        }
    }

    private void SetPlayerLabelColor(int labelIndex, Color color)
    {
        if (labelIndex >= 0 && labelIndex < mainMenu.playerLabels.Length)
        {
            GameObject playerLabel = mainMenu.playerLabels[labelIndex];
            if (playerLabel != null)
            {
                TextMeshProUGUI label = playerLabel.GetComponent<TextMeshProUGUI>();
                if (label != null)
                {
                    label.color = color;
                }
                else
                {
                    Debug.LogWarning("TextMeshProUGUI component is missing.");
                }
            }
            else
            {
                Debug.LogWarning("Player label object is missing.");
            }
        }
        else
        {
            Debug.LogWarning("Invalid label index.");
        }
    }

    private void SetCarIconColor(int carIndex, Color color)
    {
        if (carIndex >= 0 && carIndex < mainMenu.carIcons.Length)
        {
            Image carImage = mainMenu.carIcons[carIndex]?.GetComponent<Image>();
            if (carImage != null)
            {
                carImage.color = color;
            }
        }
    }

    private void CheckAllPlayersSelected()
    {
        allPlayersSelected = true;

        for (int i = 0; i < mainMenu.playerLabels.Length; i++)
        {
            if (!isPlayerSelected.ContainsKey(i) || !isPlayerSelected[i])
            {
                allPlayersSelected = false;
                break;
            }
        }
    }

    private void DisableCarSelection()
    {
        carSelectionEnabled = false;
    }

    public void OnCarIconClick(int carIndex)
    {
        SelectCarByPlayer(carIndex);
    }

    public void SavePlayerDataToTemporaryFile(string filename)
    {
        string json = JsonUtility.ToJson(this);

        // ������ ���� � ���������� �����
        string filePath = Path.Combine(Application.temporaryCachePath, filename);

        // ��������� ������ �� ��������� ����
        File.WriteAllText(filePath, json);
    }

    public void DeleteTemporaryDataFile(string filename)
    {
        // ������ ���� � ���������� �����
        string filePath = Path.Combine(Application.temporaryCachePath, filename);

        // ������� ��������� ����, ���� �� ����������
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    public void ResetPlayerData()
    {
        playButton.interactable = false;
        playerDataList.Clear();
        playerSelectedCars.Clear();
        isPlayerSelected.Clear();
        allPlayersSelected = false;
        carSelectionEnabled = true;

        for (int i = 0; i < mainMenu.playerLabels.Length; i++)
        {
            if (mainMenu.playerLabels[i] != null)
            {
                SetPlayerLabelColor(i, Color.red);
            }
            else
            {
                Debug.LogWarning("Label object is missing or destroyed.");
            }
        }

        currentPlayerIndex = 0;

        // ������� ���� � ������������ �������, ���� �� ����������
        DeleteTemporaryDataFile("player_data.json");

        Debug.Log("������ ������� �������.");
    }

    public void SaveData()
    {
        // ��������� ������� playerSelectedCars �� ������ (�������� �������)
        var sortedPlayerSelectedCars = new SortedDictionary<int, int>(playerSelectedCars);

        // ��������� ��������� ������ playerDataList �� ������ ���������������� ������� sortedPlayerSelectedCars
        playerDataList.Clear();
        foreach (var kvp in sortedPlayerSelectedCars)
        {
            PlayerData playerData = new PlayerData
            {
                playerIndex = kvp.Key,
                selectedCarIndex = kvp.Value
            };
            playerDataList.Add(playerData);
        }

        string json = JsonUtility.ToJson(this);

        // ������ ���� � ���������� �����
        string path = Path.Combine(Application.temporaryCachePath, "player_data.json");

        // ��������� ������ �� ��������� ����
        File.WriteAllText(path, json);
    }

    public void LoadData()
    {
        string path = Path.Combine(Application.temporaryCachePath, "player_data.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            JsonUtility.FromJsonOverwrite(json, this); // �������� ������ � ������� ������
        }
    }
}
