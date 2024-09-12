using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using YG;

[System.Serializable]
public struct PlayerCarSelection
{
    public int playerIndex;
    public int selectedCarIndex;
}

[System.Serializable]
public class PlayerData
{
    public List<PlayerCarSelection> playerDataList = new List<PlayerCarSelection>();
    public int currentPlayerIndex;
}

public class PlayerCars : MonoBehaviour
{
    public static PlayerCars instance;

    private GhostTrail ghostTrail;
    private GhostPlayer ghostPlayer;
    public GameObject[] cars;
    public int currentPlayerIndex = 0;
    private int lapCount = 0;
    private Dictionary<int, int> selectedCarsIndices = new Dictionary<int, int>();
    private int maxPlayerIndex = 0;
    private CircleCheck ÒircleCheck;
    public GameObject endWindow;
    public TextMeshProUGUI placeText;

    void Awake()
    {
        instance = this;

    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        endWindow.SetActive(false);
       ghostTrail = FindObjectOfType<GhostTrail>();
        ghostPlayer = FindObjectOfType<GhostPlayer>();
        ÒircleCheck = FindObjectOfType<CircleCheck>();
        if (ghostTrail == null)
        {
            Debug.LogError("GhostTrail not found in the scene. Make sure it is present in the scene and enabled.");
        }
        lapCount = 0;

        string filename = "player_data.json";
        string filePath = Path.Combine(Application.temporaryCachePath, filename);
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(jsonData);
            playerData.currentPlayerIndex = 0;

            foreach (var playerCarSelection in playerData.playerDataList)
            {
                selectedCarsIndices[playerCarSelection.playerIndex] = playerCarSelection.selectedCarIndex;

                if (playerCarSelection.playerIndex > maxPlayerIndex)
                {
                    maxPlayerIndex = playerCarSelection.playerIndex;
                }
            }

            currentPlayerIndex = playerData.currentPlayerIndex; // ”ÒÚ‡ÌÓ‚ËÏ ÚÂÍÛ˘Â„Ó Ë„ÓÍ‡
            ActivateCurrentPlayerCar(currentPlayerIndex, selectedCarsIndices);
        }
        else
        {
            Debug.LogWarning($"File {filename} not found in the temporary directory!");
        }
 
    }

    public void ShowAllGhostTrails(Dictionary<int, int> selectedCarsIndices)
    {
        List<int> selectedCarIndices = selectedCarsIndices.Values.ToList();
        ghostTrail.ShowGhostTrail(selectedCarIndices);
    }

    public void ActivateCurrentPlayerCar(int currentPlayerIndex, Dictionary<int, int> selectedCarsIndices)
    {
        int selectedCarIndex;
        if (selectedCarsIndices.TryGetValue(currentPlayerIndex, out selectedCarIndex))
        {
            if (selectedCarIndex >= 0 && selectedCarIndex < cars.Length)
            {
                for (int i = 0; i < cars.Length; i++)
                {
                    cars[i].SetActive(i == selectedCarIndex);
                }
                CarResetter carResetter = cars[selectedCarIndex].GetComponent<CarResetter>();
                if (carResetter != null)
                {
                    carResetter.ResetCar();
                }
            }
            else
            {
                Debug.LogWarning($"Player {currentPlayerIndex} has selected an invalid car index!");
            }
        }
        else
        {
            Debug.LogWarning($"No car selection information found for player {currentPlayerIndex}!");
        }
    }

    public void DeactivateCurrentPlayerCar(int currentPlayerIndex)
    {
        int selectedCarIndex;
        if (selectedCarsIndices.TryGetValue(currentPlayerIndex, out selectedCarIndex))
        {
            if (selectedCarIndex >= 0 && selectedCarIndex < cars.Length)
            {
                cars[selectedCarIndex].SetActive(false);
            }
            else
            {
                Debug.LogWarning($"Player {currentPlayerIndex} has not selected a car or selected an invalid car!");
            }
        }
        else
        {
            Debug.LogWarning($"No car selection information found for player {currentPlayerIndex}!");
        }
    }

    public void SwitchToNextPlayer()
    { 
        DeactivateCurrentPlayerCar(currentPlayerIndex);
 
        currentPlayerIndex++;
 
        if (currentPlayerIndex > maxPlayerIndex)
        {
            endWindow.SetActive(true);
            Debug.Log("Game over!");
            return;
        }
 
        ActivateCurrentPlayerCar(currentPlayerIndex, selectedCarsIndices);
 
        ShowAllGhostTrails(selectedCarsIndices);
 
        if (PlayerControl.isKeyboardControlSelected)
        {
            PlayerControl playerControl = FindObjectOfType<PlayerControl>();
            if (playerControl != null)
            {
                playerControl.SetPCControl(true);
            }
        }

        if (currentPlayerIndex <= maxPlayerIndex)
        {
            YandexGame.FullscreenShow();
        }
    }
}