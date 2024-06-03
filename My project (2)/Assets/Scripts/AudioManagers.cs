using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO; // Добавить эту библиотеку для работы с файлами
using System;
using YG;

public class AudioManagers : MonoBehaviour
{
    public static AudioManagers Instance;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private Image musicButtonImage;
    [SerializeField] private Image soundButtonImage;

    public bool isSoundOn = true;
    private bool isMusicOn = true;

    // Создать класс для хранения данных об аудио
    [Serializable]
    public class AudioData
    {
        public bool isSoundOn;
        public bool isMusicOn;
    }

    private void Awake()
    {

        // Получаем компонент AudioSource
        musicSource = GetComponent<AudioSource>();

        if (musicSource == null)
        {
            // Если компонент AudioSource отсутствует на этом объекте, добавляем его
            musicSource = gameObject.AddComponent<AudioSource>();
        }
  
    }
     
    // Метод для обновления изображения на кнопке в зависимости от состояния музыки
    private void UpdateMusicButtonImage()
    {
        if (musicButtonImage != null)
        {
            musicButtonImage.color = new Color(1f, 1f, 1f, isMusicOn ? 1f : 0f);
        }
    }
    private void UpdateSoundButtonImage()
    {
        if (soundButtonImage != null)
        {
            soundButtonImage.color = new Color(1f, 1f, 1f, isSoundOn ? 1f : 0f);
        }
    }

    public void RepeatCheckSound()
    {
        // Проверка включения звука
        if (!isSoundOn)
        {
            // Отключить все звуки
            foreach (AudioSource audioSource in FindObjectsOfType<AudioSource>())
            {
                if (audioSource != musicSource)
                    audioSource.enabled = false;
            }
        }
    }

    public void ToggleAllSounds()
    {
        // Найти все аудиоисточники в сцене
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

        // Пройти по каждому аудиоисточнику
        foreach (AudioSource audioSource in allAudioSources)
        {
            // Проверить, является ли текущий аудиоисточник музыкой
            if (audioSource == musicSource)
            {
                // Пропустить музыкальный источник
                continue;
            }

            // Включить или выключить аудиоисточник в зависимости от текущего состояния
            audioSource.enabled = !isSoundOn;
        }

        // Инвертировать состояние
        isSoundOn = !isSoundOn;

        // Обновить изображение на кнопке музыки
        UpdateSoundButtonImage();

        // Сохранить данные об аудио в файл
        SaveAudioData();
        Debug.Log("isSoundOn: " + isSoundOn);
        Debug.Log("isMusicOn: " + isMusicOn);
    }
    
    // Метод для отключения/включения музыки
    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;

        if (isMusicOn)
        {
            // Включаем музыку, если она была отключена
            musicSource.Play();
        }
        else
        {
            // Отключаем музыку
            musicSource.Stop();
        }

        // Обновить изображение на кнопке музыки
        UpdateMusicButtonImage();

        // Сохранить данные об аудио в файл
        SaveAudioData();
        Debug.Log("isSoundOn: " + isSoundOn);
        Debug.Log("isMusicOn: " + isMusicOn);
    }

    // Метод для сохранения данных об аудио в файл
    public void SaveAudioData()
    {
        // Создаем объект AudioData с текущими значениями переменных
        AudioData data = new AudioData
        {
            isSoundOn = isSoundOn,
            isMusicOn = isMusicOn
        };

        // Сохраняем данные в YandexGame.savesData.audioData
        YandexGame.savesData.audioData = data;

        // Теперь остаётся сохранить данные
        YandexGame.SaveProgress();
    }

    // Метод для загрузки данных об аудио из файла
    // Метод для загрузки данных об аудио из файла и включения/выключения звуков
    public void LoadAudioData()
    {
        // Загружаем данные из YandexGame.savesData.audioData
        AudioData data = YandexGame.savesData.audioData;

        // Если данные существуют, обновляем аудио настройки
        if (data != null)
        {
            isSoundOn = data.isSoundOn;
            isMusicOn = data.isMusicOn;

            // Включаем или выключаем музыку в зависимости от значения isMusicOn
            if (isMusicOn)
            {
                musicSource.Play();
                UpdateMusicButtonImage();
            }
            else
            {
                musicSource.Stop();
                UpdateMusicButtonImage();
            }

            // Находим все аудиоисточники в сцене
            AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

            // Проходим по каждому аудиоисточнику
            foreach (AudioSource audioSource in allAudioSources)
            {
                // Проверяем, является ли текущий аудиоисточник музыкой
                if (audioSource == musicSource)
                {
                    // Пропускаем музыкальный источник
                    continue;
                }

                // Включаем или выключаем аудиоисточник в зависимости от значения isSoundOn
                audioSource.enabled = isSoundOn;
            }

            // Обновляем изображение на кнопке звука
            UpdateSoundButtonImage();
        }
        else
        {
            Debug.LogWarning("Audio data does not exist.");
        }



    }


}
