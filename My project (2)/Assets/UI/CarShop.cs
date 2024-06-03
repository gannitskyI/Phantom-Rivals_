using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using I2.Loc; // Добавляем пространство имен для I2Localize
using YG;
 

public class CarShop : MonoBehaviour
{
    public static CarShop Instance;

    [System.Serializable]
    public class CarData
    {
        public string taskCheckKey;
        public string key;
        public int price;
        public string name;
        public bool isPurchased = false;
        public bool isSelected = false;
        public bool isTaskAwarded = false;
        public string taskCheck; // Добавляем переменную taskCheck
  
    }

    public List<CarData> cars = new List<CarData>();
    public TMP_Text carNameText;
    public TMP_Text taskNameText;
    public TMP_Text carPriceText;
    public GameObject buyButton;
    public int selectedCarIndex = -1;
    private int score;
    private List<GameObject> instantiatedPrefabs = new List<GameObject>();
    private SelectorCars selector;
    private MainMenu mainMenu;

    private void OnEnable() => YandexGame.GetDataEvent += GetData;
    private void OnDisable() => YandexGame.GetDataEvent -= GetData;

    private void Awake()
    {
        Instance = this;
        selector = SelectorCars.Instance;

        if (YandexGame.SDKEnabled == true)
        {
            // Если запустился, то запускаем Ваш метод
            GetData();
 
        }

    }
    public void GetData()
    {
        // Загружаем список CarData
        List<CarData> carDataList = YandexGame.savesData.carDataList;

        // Если данных нет, создаем их
        if (carDataList == null)
        {
            carDataList = new List<CarData>();
            YandexGame.savesData.carDataList = carDataList;
        }

        // Обновляем данные в cars
        foreach (CarData carData in carDataList)
        {
            int index = cars.FindIndex(c => c.key == carData.key);
            if (index != -1)
            {
                cars[index] = carData;
            }
            else
            {
                cars.Add(carData); // Если машина была куплена после последней загрузки, добавляем её в список
            }
        }
    }

    public void SaveData()
    {
        YandexGame.savesData.carDataList = new List<CarData>(cars);
        YandexGame.SaveProgress();
    }

    private void Start()
    {
        mainMenu = FindObjectOfType<MainMenu>();
        LoadScoreData();
      
        if (selector != null)
        {
            foreach (CarData carData in cars)
            {
                selector.UpdateCarStatus(cars.IndexOf(carData), carData.isPurchased);
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
        // Загружаем данные из YandexGame.savesData.scoreData
        ScoreData scoreData = YandexGame.savesData.scoreData;

        // Если данные существуют, обновляем счет
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
        if (carIndex < 0 || carIndex >= cars.Count)
        {
            Debug.LogError("Invalid car index!");
            return;
        }

        CarData currentCarData = cars[carIndex];
        carNameText.text = currentCarData.name;
        carPriceText.text = currentCarData.price.ToString() + "$";

        // Получаем перевод по ключу из I2 Localization
        taskNameText.text = LocalizationManager.GetTranslation(currentCarData.taskCheckKey);

        taskNameText.gameObject.SetActive(!currentCarData.isTaskAwarded);

        bool isCarPurchased = currentCarData.isPurchased;
        bool hasEnoughMoney = score >= currentCarData.price;

        // Обновляем условие для активации кнопки покупки машины
        buyButton.GetComponent<Button>().interactable = !isCarPurchased && hasEnoughMoney && currentCarData.isTaskAwarded;

        foreach (GameObject instantiatedPrefab in instantiatedPrefabs)
        {
            Destroy(instantiatedPrefab);
        }
        instantiatedPrefabs.Clear();

        GameObject carPrefab = Resources.Load<GameObject>(currentCarData.key);
        if (carPrefab != null)
        {
            GameObject newPrefab = Instantiate(carPrefab, transform);
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
        selectedCarIndex = (selectedCarIndex + 1) % cars.Count;
        ClearAndDisplayCar();
    }

    public void PreviousCar()
    {
        selectedCarIndex = (selectedCarIndex - 1 + cars.Count) % cars.Count;
        ClearAndDisplayCar();
    }

    public void BuyCar()
    {
        mainMenu.BuyButtonSound();
        if (selectedCarIndex < 0 || selectedCarIndex >= cars.Count)
        {
            Debug.LogError("Invalid car index!");
            return;
        }

        CarData selectedCar = cars[selectedCarIndex];
        int carPrice = selectedCar.price;

        // Access score data from YandexGame.savesData.scoreData
        ScoreData scoreData = YandexGame.savesData.scoreData;

        if (scoreData != null && scoreData.Score >= carPrice)
        {
            scoreData.Score -= carPrice;

            // Update score data directly within YandexGame.savesData.scoreData
            YandexGame.savesData.scoreData = scoreData;
            YandexGame.SaveProgress(); // Save the updated score data

            selectedCar.isPurchased = true;
            cars[selectedCarIndex] = selectedCar;

            SaveData(); // Save all cars after purchase
            LoadScoreData();
            SaveSelectedCar();
            mainMenu.UpdateCoinText();
            ClearAndDisplayCar();
            // MainMenu.Instance.UpdateCoinText();
            MainMenu.Instance.CheckCarPurchases();
            Debug.Log("Car purchased: " + selectedCar.key);
            Debug.Log("Player money: " + scoreData.Score);

            foreach (CarData car in cars)
            {
                selector.UpdateCarStatus(cars.IndexOf(car), car.isPurchased);
            }
        }
        else
        {
            Debug.Log("Insufficient funds to buy the car: " + selectedCar.key);
        }
    }

    public void SaveSelectedCar()
    {
        foreach (CarData carData in cars)
        {
            carData.isSelected = false;
        }

        if (selectedCarIndex >= 0 && selectedCarIndex < cars.Count)
        {
            // Обновляем данные о выбранной машине для каждой машины
            foreach (CarData carData in cars)
            {
                if (cars.IndexOf(carData) == selectedCarIndex)
                {
                    carData.isSelected = true;
                    YandexGame.savesData.carDataList[cars.IndexOf(carData)] = carData;
                    break;
                }
            }

            // Теперь остаётся сохранить данные
            YandexGame.SaveProgress();
        }
    }


    public bool IsCarPurchased(string carKey)
    {
        foreach (CarData carData in cars)
        {
            if (carData.key == carKey)
            {
                return carData.isPurchased;
            }
        }
        return false;
    }

    // Метод для установки флага получения машины по заданию
    public void SetTaskAwarded(string carKey)
    {
        foreach (CarData carData in cars)
        {
            if (carData.key == carKey)
            {
                carData.isTaskAwarded = true;
                break;
            }
        }
    }
    public void LoadAndUpdateShop()
    {
        GetData();
        LoadScoreData();
        ClearAndDisplayCar();
    }

}
