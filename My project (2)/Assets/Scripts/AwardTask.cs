using TMPro;
using UnityEngine;
using YG;

public class AwardTask : MonoBehaviour
{
    [SerializeField]
    private Reward[] rewards; // Массив наград для каждой задачи
    private int score;
    private CarShop carShop;
    private MoneyCounter moneyCounter;
    private MenuTask menuTask;
    private ButtonTaskStatus buttonTaskStatus;
    private MainMenu mainMenu;
    private Coin coin;
    public TMP_Text coinText;
    private void Awake()
    {
        carShop = FindObjectOfType<CarShop>();
        moneyCounter = FindObjectOfType<MoneyCounter>();
        menuTask = FindObjectOfType<MenuTask>();
        mainMenu = FindObjectOfType<MainMenu>();
    }

    public void AwardTaskWithReward(int taskIndex)
    {
        mainMenu.PlayButtonSound();
        // Добавляем награду за выполнение задачи
        AwardTaskCompletion(taskIndex, rewards[taskIndex]);
    }
    public void InitializeButtonTaskStatus()
    {
        buttonTaskStatus = YandexGame.savesData.buttonTaskStatus;
        if (buttonTaskStatus == null || buttonTaskStatus.TaskCompleted.Length != 6)
        {
            // Если статус кнопок не был загружен или длина массива неверная, создаем новый объект
            buttonTaskStatus = new ButtonTaskStatus();
            YandexGame.savesData.buttonTaskStatus = buttonTaskStatus;
            YandexGame.SaveProgress();
        }
    }
    private void AwardTaskCompletion(int taskIndex, Reward reward)
    {
        
        buttonTaskStatus.TaskCompleted[taskIndex] = true;
        menuTask.taskCompletionObjects[taskIndex].SetActive(false); // Скрываем кнопку
        menuTask.taskComplited[taskIndex].SetActive(true); // Показываем завершенную задачу

        // Применяем награду
        ApplyReward(taskIndex, reward);
        carShop.LoadAndUpdateShop();
    }

    private void ApplyReward(int taskIndex, Reward reward)
    {
        // Обновляем статус выполнения задачи
        buttonTaskStatus.TaskCompleted[taskIndex] = true;
        YandexGame.SaveProgress(); // Сохраняем обновленные данные о статусе задачи

        if (reward.Type == RewardType.Money)
        {
            // Увеличиваем счет на указанное количество денег
            ScoreData scoreData = YandexGame.savesData.scoreData;
            scoreData.Score += reward.Amount;
            YandexGame.SaveProgress(); // Сохраняем обновленные данные о счете
            coinText.text = scoreData.Score.ToString() + "$"; // Обновляем текст счета в игре
            carShop.LoadScoreData();
        }
        else if (reward.Type == RewardType.Car)
        {
            // Выдаем машину
            carShop.SetTaskAwarded(reward.CarID);
            carShop.SaveData();
        }

    }

}

[System.Serializable]
public class ButtonTaskStatus
{
    public bool[] TaskCompleted = new bool[6]; // Массив статусов выполнения кнопок
}

[System.Serializable]
public class Reward
{
    public RewardType Type;
    public int Amount; // Количество денег или иное значение в зависимости от типа награды
    public string CarID; // Идентификатор машины, если тип награды - машина
}

public enum RewardType
{
    Money,
    Car,
    // Добавьте другие типы наград по мере необходимости
}
