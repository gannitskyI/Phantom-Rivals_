using UnityEngine;
using IJunior.TypedScenes;
using TMPro;
using UnityEngine.UI;
using System.IO;
using DG.Tweening;
using YG;
using System.Collections;
public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI coinText;
    private float YGTime;
    public Image playButtonImage;
   
    public GameObject pausePanel;
    public GameObject pausePanelBackground;

    private SelectorCars selector;
    public GameObject[] playerLabels;
    public GameObject[] carIcons;
    public ModeSwitcher modeSwitcher;
    public GameObject buttonTask;
    public AudioClip buttonSound;
    public AudioClip buySound;
    public ToggleFunction toggleFunction;
    private AudioSource audioSource;

    private AudioManagers audioManager;
    private PanelMover panelMover;
    public float animationDuration = 0.5f;

    public TextMeshProUGUI bestTimeText;

    public static MainMenu Instance { get; private set; }

    private void Awake()
    {
       
        Instance = this;
        selector = SelectorCars.Instance;
        HideSettingWithAnimation();
    }

    private void Start()
    {
       
        Application.targetFrameRate = 60;
        audioManager = FindObjectOfType<AudioManagers>();
        panelMover = FindObjectOfType<PanelMover>();
        audioManager.LoadAudioData();
        HideSettingWithAnimation();
        AudioListener.volume = 1f;
        UpdateCoinText();
        CheckCarPurchases();
        LoadBestTime();

        audioSource = GetComponent<AudioSource>();

    }
    public void PlayButtonSound()
    {
        // Проверка, что компонент AudioSource включен и звук кнопки установлен
        if (audioSource != null && audioSource.enabled && buttonSound != null)
        {
            // Проигрывание звука кнопки
            audioSource.PlayOneShot(buttonSound);
        }
    }
    public void BuyButtonSound()
    {
        // Проверка, что компонент AudioSource включен и звук кнопки установлен
        if (audioSource != null && audioSource.enabled && buySound != null)
        {
            // Проигрывание звука кнопки
            audioSource.PlayOneShot(buySound);
        }
    }
    public void LoadBestTime()
    {  // Проверяем, существуют ли данные bestTimeData в savesData
        if (YandexGame.savesData.bestTimeData != null)
        {
            // Получаем лучшее время из savesData
            float bestTime = YandexGame.savesData.bestTimeData.bestTime;

            // Форматируем время
            int minutes = Mathf.FloorToInt(bestTime / 60);
            int seconds = Mathf.FloorToInt(bestTime % 60);
            int milliseconds = Mathf.FloorToInt((bestTime * 1000) % 1000);
            string formattedTime = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);

            // Обновляем текст лучшего времени
            bestTimeText.text = formattedTime;
        }

    }

    public void ShowSelector()
    {
        
        PlayButtonSound();
        panelMover.MoveDown();
        modeSwitcher.SwipeUp();
    }
    public void ShowSetting()
    {
        
        PlayButtonSound();
        if (pausePanel.activeSelf)
        {
            // Если уже активен, скрываем с анимацией
            HideSettingWithAnimation();
            buttonTask.SetActive(true);
            pausePanelBackground.SetActive(false);
        }
        else
        {
            // Если не активен, показываем с анимацией
            ShowSettingWithAnimation();
            buttonTask.SetActive(false);
            pausePanelBackground.SetActive(true);
        }
    }

    private void ShowSettingWithAnimation()
    {

        pausePanel.SetActive(true);

        // Анимация выезда панели и движения правой части экрана
        pausePanel.transform.DOMoveX(Screen.width / 2f, animationDuration);
    }

    private void HideSettingWithAnimation()
    {
        // Анимация уезда панели и возвращения правой части экрана
        pausePanel.transform.DOMoveX(Screen.width + pausePanel.GetComponent<RectTransform>().rect.width, animationDuration)
            .OnComplete(() => pausePanel.SetActive(false));
    }

    public void BackSelectorCar()
    {
        toggleFunction = FindObjectOfType<ToggleFunction>();
        // Проверяем, включен ли toggle, и вызываем метод IsOn() из ToggleFunction, если это так
        if (toggleFunction.toggle.isOn)
        {
            toggleFunction.IsOn();
        }
        modeSwitcher.ResetToDefaults();
        PlayButtonSound();
       
        selector.ResetPlayerData();
        modeSwitcher.SwipeUp();
        panelMover.MoveUp();

    }

    public void UpdateCoinText()
    {
        // Загружаем данные из YandexGame.savesData.scoreData
        ScoreData scoreData = YandexGame.savesData.scoreData;

        // Если данные существуют, обновляем текст монет
        if (scoreData != null)
        {
            coinText.text = scoreData.Score.ToString() + "$";
        }
        else
        {
            // Если данных нет, создаем их
            scoreData = new ScoreData();
            // Здесь вы можете добавить код для инициализации данных ScoreData
            // Например, вы можете установить начальное количество монет
            scoreData.Score = 0; // Начальное количество монет

            // Сохраняем данные
            YandexGame.savesData.scoreData = scoreData;
            YandexGame.SaveProgress();

            // Обновляем текст монет
            coinText.text = scoreData.Score.ToString() + "$";
        }
    }

    public void CheckCarPurchases()
    {
        bool anyCarPurchased = false;

        foreach (CarShop.CarData carData in CarShop.Instance.cars)
        {
            if (CarShop.Instance.IsCarPurchased(carData.key))
            {
                anyCarPurchased = true;
                break;
            }
        }
 
    }


    public void OnPlayButtonClick()
    {
        YandexGame.FullscreenShow();
        PlayButtonSound();
        modeSwitcher.SaveGameModeData();
        DOTween.Clear(true);
        Game.Load();
    }
}
