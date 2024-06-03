using TMPro;
using UnityEngine;
using YG;

public class AwardTask : MonoBehaviour
{
    [SerializeField]
    private Reward[] rewards; // ������ ������ ��� ������ ������
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
        // ��������� ������� �� ���������� ������
        AwardTaskCompletion(taskIndex, rewards[taskIndex]);
    }
    public void InitializeButtonTaskStatus()
    {
        buttonTaskStatus = YandexGame.savesData.buttonTaskStatus;
        if (buttonTaskStatus == null || buttonTaskStatus.TaskCompleted.Length != 6)
        {
            // ���� ������ ������ �� ��� �������� ��� ����� ������� ��������, ������� ����� ������
            buttonTaskStatus = new ButtonTaskStatus();
            YandexGame.savesData.buttonTaskStatus = buttonTaskStatus;
            YandexGame.SaveProgress();
        }
    }
    private void AwardTaskCompletion(int taskIndex, Reward reward)
    {
        
        buttonTaskStatus.TaskCompleted[taskIndex] = true;
        menuTask.taskCompletionObjects[taskIndex].SetActive(false); // �������� ������
        menuTask.taskComplited[taskIndex].SetActive(true); // ���������� ����������� ������

        // ��������� �������
        ApplyReward(taskIndex, reward);
        carShop.LoadAndUpdateShop();
    }

    private void ApplyReward(int taskIndex, Reward reward)
    {
        // ��������� ������ ���������� ������
        buttonTaskStatus.TaskCompleted[taskIndex] = true;
        YandexGame.SaveProgress(); // ��������� ����������� ������ � ������� ������

        if (reward.Type == RewardType.Money)
        {
            // ����������� ���� �� ��������� ���������� �����
            ScoreData scoreData = YandexGame.savesData.scoreData;
            scoreData.Score += reward.Amount;
            YandexGame.SaveProgress(); // ��������� ����������� ������ � �����
            coinText.text = scoreData.Score.ToString() + "$"; // ��������� ����� ����� � ����
            carShop.LoadScoreData();
        }
        else if (reward.Type == RewardType.Car)
        {
            // ������ ������
            carShop.SetTaskAwarded(reward.CarID);
            carShop.SaveData();
        }

    }

}

[System.Serializable]
public class ButtonTaskStatus
{
    public bool[] TaskCompleted = new bool[6]; // ������ �������� ���������� ������
}

[System.Serializable]
public class Reward
{
    public RewardType Type;
    public int Amount; // ���������� ����� ��� ���� �������� � ����������� �� ���� �������
    public string CarID; // ������������� ������, ���� ��� ������� - ������
}

public enum RewardType
{
    Money,
    Car,
    // �������� ������ ���� ������ �� ���� �������������
}
