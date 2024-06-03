 
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

    // Подписываемся на событие открытия рекламы в OnEnable
    private void OnEnable() => YandexGame.RewardVideoEvent += Rewarded;

    // Отписываемся от события открытия рекламы в OnDisable
    private void OnDisable() => YandexGame.RewardVideoEvent -= Rewarded;

    // Подписанный метод получения награды
    void Rewarded(int id)
    { 
        ScoreData scoreData = YandexGame.savesData.scoreData;
        scoreData.Score += 300;
        YandexGame.SaveProgress(); // Сохраняем обновленные данные о счете
        coinText.text = scoreData.Score.ToString() + "$"; // Обновляем текст счета в игре
        carShop.LoadScoreData();
    }

    // Метод для вызова видео рекламы
    public void OpenRewardAd(int id)
    {
        // Вызываем метод открытия видео рекламы
        YandexGame.RewVideoShow(id);
    }
}
