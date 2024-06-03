 
using UnityEngine;
using TMPro;
using YG;

public class RewardYG : MonoBehaviour
{
    [SerializeField]
   
    private int score;
    private CarShop carShop;
    private MoneyCounter moneyCounter;
    private MenuTask menuTask;
   
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

    // ������������� �� ������� �������� ������� � OnEnable
    private void OnEnable() => YandexGame.RewardVideoEvent += Rewarded;

    // ������������ �� ������� �������� ������� � OnDisable
    private void OnDisable() => YandexGame.RewardVideoEvent -= Rewarded;

    // ����������� ����� ��������� �������
    void Rewarded(int id)
    { 
        ScoreData scoreData = YandexGame.savesData.scoreData;
        scoreData.Score += 300;
        YandexGame.SaveProgress(); // ��������� ����������� ������ � �����
        coinText.text = scoreData.Score.ToString() + "$"; // ��������� ����� ����� � ����
        carShop.LoadScoreData();
    }

    // ����� ��� ������ ����� �������
    public void OpenRewardAd(int id)
    {
        // �������� ����� �������� ����� �������
        YandexGame.RewVideoShow(id);
    }
}
