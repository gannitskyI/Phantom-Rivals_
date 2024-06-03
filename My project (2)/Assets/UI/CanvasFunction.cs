using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using IJunior.TypedScenes;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Collections;
using YG;

[System.Serializable]
public class BestTimeData
{
    public float bestTime;
}

public class CanvasFunction : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI countdownText;

    private float startTime;
    private float countdownStartTime;
    private bool isTimerRunning;
    private bool isCountdownRunning;
    private int countdownDuration = 3;
    public bool isCountdownFinished = false;

    public GameObject SelectorControl;
    public GameObject MobileInput;
    public GameObject PausePanel;
    private AudioManagers audioManager;
  
    public float bestTime = float.MaxValue;

    private EventGame eventGame;
    private bool isEndGameShown;
    private bool isGameRunning; // New state to control game running

    public AudioClip buttonSound;
    public AudioClip takeMoneySound;

    private AudioSource audioSource;

    public PauseUI pauseUI;

   
    private void Start()
    { 
        AudioListener.volume = 0f;
        audioManager = FindObjectOfType<AudioManagers>();
       
        eventGame = FindObjectOfType<EventGame>();
       
        PauseGame();
        LoadGame();
        LoadBestTime();// Load the control data at the start of the game
    }
    public void SaveBestTime()
    { // Создаем экземпляр BestTimeData и устанавливаем лучшее время
        BestTimeData bestTimeData = new BestTimeData();
        bestTimeData.bestTime = bestTime;

        // Сохраняем данные BestTimeData в savesData
        YandexGame.savesData.bestTimeData = bestTimeData;

        // Теперь остаётся сохранить данные
        YandexGame.SaveProgress();
    }
    public void LoadBestTime()
    {
        // Check if the best time data exists
        if (YandexGame.savesData.bestTimeData != null)
        {
            // Load the best time from saved data
            bestTime = YandexGame.savesData.bestTimeData.bestTime;
        }
    }

    public void PlayButtonSound()
    {
        // Проверка, что компонент AudioSource и звук кнопки установлены
        if (audioSource != null && buttonSound != null)
        {
            // Проигрывание звука кнопки
            audioSource.PlayOneShot(buttonSound);
        }
    }
    public void BuyButtonSound()
    {
        // Проверка, что компонент AudioSource и звук кнопки установлены
        if (audioSource != null && takeMoneySound != null)
        {
            // Проигрывание звука кнопки
            audioSource.PlayOneShot(takeMoneySound);
        }
    }

    public void ShowPausePanel()
    { 
        // Проверяем условия, при которых можно показать панель паузы
        if (!isCountdownRunning && !pauseUI.isCountingDown && Time.timeScale == 1f)
        {
            PlayButtonSound();
            AudioListener.volume = 0f;
            PausePanel.SetActive(true);
            PauseGame();
        }
    }
    public void ClosePausePanel()
    {
        AudioListener.volume = 1f;
        PausePanel.SetActive(false);
        PlayButtonSound();
        StartCoroutine(ResumeAfterDelay(3)); // Запускаем корутину с задержкой в 3 секунды
    }

    private IEnumerator ResumeAfterDelay(int delay)
    {
        for (int i = delay; i > 0; i--)
        {
            countdownText.gameObject.SetActive(true);
            countdownText.text = i.ToString(); // Обновляем текст обратного отсчета
            yield return new WaitForSecondsRealtime(1); // Ждем 1 секунду в реальном времени
        }
        countdownText.text = ""; // Очищаем текст обратного отсчета
        countdownText.gameObject.SetActive(false);
        ResumeGame();
    }

    public void BackToMenu()
    {
        YandexGame.FullscreenShow();
        Menu.LoadAsync();

    }    
        
     
    

    public void RestartGame()
    {
        YandexGame.FullscreenShow();
        Game.Load();
    }
   
    // A class to store the control data
    [Serializable]
    public class ControlData
    {
        public bool isPCControl;
        public bool isMobileControl;
    }

    // An instance of the class to store the current control data
    private ControlData controlData = new ControlData();

    // A path to the file where the control data will be saved
    private string controlDataFilePath;

    public void OnPCControlButtonClick()
    {
        audioManager.LoadAudioData();
        ResumeGame();
        var playerControl = FindObjectOfType<PlayerControl>();
        if (playerControl != null) playerControl.OnPCControl();
        SelectorControl.SetActive(false);
        StartCountdown();
        isGameRunning = true; // Start the game when the player selects a control method
        controlData.isPCControl = true; // Set the control data to PC
        controlData.isMobileControl = false;

        AudioListener.volume = 1f;
        PlayButtonSound();
    }

    public void OnMobileButtonClick()
    {
        audioManager.LoadAudioData();
        ResumeGame();
        MobileInput.SetActive(true);
        SelectorControl.SetActive(false);
        StartCountdown();
        isGameRunning = true; // Start the game when the player selects a control method
        controlData.isPCControl = false; // Set the control data to mobile
        controlData.isMobileControl = true;

        AudioListener.volume = 1f;
        PlayButtonSound();
    }

    // A method to save the control data to a file
    public void SaveGame()
    {
        // Create a binary formatter
        BinaryFormatter bf = new BinaryFormatter();
        // Create a byte array to store the serialized data
        byte[] data;
        // Use a memory stream to serialize the data
        using (MemoryStream ms = new MemoryStream())
        {
            // Serialize the control data to the memory stream
            bf.Serialize(ms, controlData);
            // Get the byte array from the memory stream
            data = ms.ToArray();
        }
        // Write the byte array to the file
        File.WriteAllBytes(controlDataFilePath, data);
    }

    // A method to load the control data from a file
    public void LoadGame()
    {
        // Check if the file exists
        if (File.Exists(controlDataFilePath))
        {
            // Read the byte array from the file
            byte[] data = File.ReadAllBytes(controlDataFilePath);
            // Create a binary formatter
            BinaryFormatter bf = new BinaryFormatter();
            // Use a memory stream to deserialize the data
            using (MemoryStream ms = new MemoryStream(data))
            {
                // Deserialize the data to the control data instance
                controlData = (ControlData)bf.Deserialize(ms);
            }
            // Apply the control data to the settings
            if (controlData.isPCControl)
            {
                OnPCControlButtonClick();
            }
            else if (controlData.isMobileControl)
            {
                OnMobileButtonClick();
            }
        }
    }

    // A method to reset the control data by deleting the file
    public void ResetData()
    {
        // Check if the file exists
        if (File.Exists(controlDataFilePath))
        {
            // Delete the file
            File.Delete(controlDataFilePath);
        }
    }


    private void Update()
    {
        if (!isGameRunning || isEndGameShown) return;

        if (isCountdownRunning)
        {
            var countdownElapsedTime = Time.time - countdownStartTime;
            countdownText.text = (countdownDuration - (int)countdownElapsedTime).ToString();

            if (countdownElapsedTime >= countdownDuration)
            {
                isCountdownRunning = false;
                countdownText.gameObject.SetActive(false);
                StartTimer();
            }
        }

        if (isTimerRunning && !isEndGameShown)
        {
            var elapsedTime = Time.time - startTime;
            var minutes = (int)(elapsedTime / 60);
            var seconds = (int)(elapsedTime % 60);
            var milliseconds = (int)((elapsedTime * 1000) % 1000);
            timerText.text = $"{minutes:00}:{seconds:00}:{milliseconds:000}";
 
        }
    }
    public void CountdownRunning()
    {
        isCountdownFinished = false; 
    }

    private void StartCountdown()
    {

        countdownStartTime = Time.time;
        isCountdownRunning = true;
        StartCoroutine(CountdownCoroutine());
    }
    private IEnumerator CountdownCoroutine()
    {
        yield return new WaitForSeconds(countdownDuration);
        isCountdownFinished = true;
        isCountdownRunning = false;
    }

    public void PauseGame()
    {
        Time.timeScale = 0f; // Останавливаем время в игре
        isGameRunning = false; // Помечаем игру как остановленную
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; // Возобновляем время в игре
        isGameRunning = true; // Помечаем игру как возобновленную
    }
    public float GetCurrentTime() => isTimerRunning ? Time.time - startTime : 0f;

    private void StartTimer()
    {
        startTime = Time.time;
        isTimerRunning = true;
    }

    public void StopTimer() => isTimerRunning = false;

    public void RestartCountdown()
    {
        countdownText.gameObject.SetActive(true);
        countdownStartTime = Time.time;
        isCountdownRunning = true;
        StartCoroutine(CountdownCoroutine());
    }

    public string FormatTime(float time)
    {
        var minutes = (int)(time / 60);
        var seconds = (int)(time % 60);
        var milliseconds = (int)((time * 1000) % 1000);
        return $"{minutes:00}:{seconds:00}:{milliseconds:000}";
    }
}