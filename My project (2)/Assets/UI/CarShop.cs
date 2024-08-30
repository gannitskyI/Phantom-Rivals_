using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc; // Добавляем пространство имен для I2Localize
using YG;

public class CarShop : MonoBehaviour
{
    public static CarShop Instance;

    [Header("Scriptable Objects")]
    public List<CarDataSO> carDataList; // Используем ScriptableObject

    public TMP_Text carNameText;
    public TMP_Text taskNameText;
    public TMP_Text carPriceText;
    public GameObject buyButton;
    public int selectedCarIndex = -1;
    private int score;
    private List<GameObject> instantiatedPrefabs = new List<GameObject>();
    private SelectorCars selector;
    private MainMenu mainMenu;
 
    private void Awake()
    {
        Instance = this;
        selector = SelectorCars.Instance;
 
    }
 
    public void SaveData()
    { 
        YandexGame.SaveProgress();
    }

    private void Start()
    {
        mainMenu = FindObjectOfType<MainMenu>();
        LoadScoreData();

        if (selector != null)
        {
            foreach (CarDataSO carDataSO in carDataList)
            {
                selector.UpdateCarStatus(carDataList.IndexOf(carDataSO), carDataSO.isPurchased);
            }
        }
        else
        {
            Debug.LogWarning("SelectorCars.Instance is not available.");
        }
        ClearAndDisplayCar();

        buyButton.GetComponent<Button>().interactable = false;
    }

    public void LoadScoreData()
    {
        ScoreData scoreData = YandexGame.savesData.scoreData;

        if (scoreData != null)
        {
            score = scoreData.Score;
        }
        else
        {
            Debug.LogWarning("Score data does not exist.");
        }
    }

    public void DisplayCar(int carIndex)
    {
        if (carIndex < 0 || carIndex >= carDataList.Count)
        {
            Debug.LogError("Invalid car index!");
            return;
        }

        CarDataSO currentCarDataSO = carDataList[carIndex];
        carNameText.text = currentCarDataSO.names;
        carPriceText.text = currentCarDataSO.price.ToString() + "$";

        taskNameText.text = LocalizationManager.GetTranslation(currentCarDataSO.taskCheckKey);
        taskNameText.gameObject.SetActive(!currentCarDataSO.isTaskAwarded);

        bool isCarPurchased = currentCarDataSO.isPurchased;
        bool hasEnoughMoney = score >= currentCarDataSO.price;

        buyButton.GetComponent<Button>().interactable = !isCarPurchased && hasEnoughMoney && currentCarDataSO.isTaskAwarded;

        foreach (GameObject instantiatedPrefab in instantiatedPrefabs)
        {
            Destroy(instantiatedPrefab);
        }
        instantiatedPrefabs.Clear();

        // Используем префаб из CarDataSO
        if (currentCarDataSO.carPrefab != null)
        {
            GameObject newPrefab = Instantiate(currentCarDataSO.carPrefab, transform);
            newPrefab.SetActive(true);
            instantiatedPrefabs.Add(newPrefab);
        }
    }

    public void ClearAndDisplayCar()
    {
        mainMenu.PlayButtonSound();
        foreach (GameObject instantiatedPrefab in instantiatedPrefabs)
        {
            Destroy(instantiatedPrefab);
        }
        instantiatedPrefabs.Clear();

        DisplayCar(selectedCarIndex);
    }

    public void NextCar()
    {
        selectedCarIndex = (selectedCarIndex + 1) % carDataList.Count;
        ClearAndDisplayCar();
    }

    public void PreviousCar()
    {
        selectedCarIndex = (selectedCarIndex - 1 + carDataList.Count) % carDataList.Count;
        ClearAndDisplayCar();
    }

    public void BuyCar()
    {
        mainMenu.BuyButtonSound();
        if (selectedCarIndex < 0 || selectedCarIndex >= carDataList.Count)
        {
            Debug.LogError("Invalid car index!");
            return;
        }

        CarDataSO selectedCarSO = carDataList[selectedCarIndex];
        int carPrice = selectedCarSO.price;

        ScoreData scoreData = YandexGame.savesData.scoreData;

        if (scoreData != null && scoreData.Score >= carPrice)
        {
            scoreData.Score -= carPrice;

            YandexGame.savesData.scoreData = scoreData;
            YandexGame.SaveProgress();

            selectedCarSO.isPurchased = true;

            SaveData(); // Save all cars after purchase
            LoadScoreData();
            SaveSelectedCar();
            mainMenu.UpdateCoinText();
            ClearAndDisplayCar();
            MainMenu.Instance.CheckCarPurchases();
            Debug.Log("Car purchased: " + selectedCarSO.key);
            Debug.Log("Player money: " + scoreData.Score);

            foreach (CarDataSO carDataSO in carDataList)
            {
                selector.UpdateCarStatus(carDataList.IndexOf(carDataSO), carDataSO.isPurchased);
            }
        }
        else
        {
            Debug.Log("Insufficient funds to buy the car: " + selectedCarSO.key);
        }
    }

    public void SaveSelectedCar()
    {
        foreach (CarDataSO carDataSO in carDataList)
        {
            carDataSO.isSelected = false;
        }

        if (selectedCarIndex >= 0 && selectedCarIndex < carDataList.Count)
        {
            CarDataSO selectedCarSO = carDataList[selectedCarIndex];
            selectedCarSO.isSelected = true;
 
            YandexGame.SaveProgress();
        }
    }

    public bool IsCarPurchased(string carKey)
    {
        foreach (CarDataSO carDataSO in carDataList)
        {
            if (carDataSO.key == carKey)
            {
                return carDataSO.isPurchased;
            }
        }
        return false;
    }

    public void SetTaskAwarded(string carKey)
    {
        foreach (CarDataSO carDataSO in carDataList)
        {
            if (carDataSO.key == carKey)
            {
                carDataSO.isTaskAwarded = true;
                break;
            }
        }
    }

    public void LoadAndUpdateShop()
    { 
        LoadScoreData();
        ClearAndDisplayCar();
    }
}
