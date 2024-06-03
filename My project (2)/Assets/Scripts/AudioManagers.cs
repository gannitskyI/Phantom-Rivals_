using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO; // �������� ��� ���������� ��� ������ � �������
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

    // ������� ����� ��� �������� ������ �� �����
    [Serializable]
    public class AudioData
    {
        public bool isSoundOn;
        public bool isMusicOn;
    }

    private void Awake()
    {

        // �������� ��������� AudioSource
        musicSource = GetComponent<AudioSource>();

        if (musicSource == null)
        {
            // ���� ��������� AudioSource ����������� �� ���� �������, ��������� ���
            musicSource = gameObject.AddComponent<AudioSource>();
        }
  
    }
     
    // ����� ��� ���������� ����������� �� ������ � ����������� �� ��������� ������
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
        // �������� ��������� �����
        if (!isSoundOn)
        {
            // ��������� ��� �����
            foreach (AudioSource audioSource in FindObjectsOfType<AudioSource>())
            {
                if (audioSource != musicSource)
                    audioSource.enabled = false;
            }
        }
    }

    public void ToggleAllSounds()
    {
        // ����� ��� �������������� � �����
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

        // ������ �� ������� ��������������
        foreach (AudioSource audioSource in allAudioSources)
        {
            // ���������, �������� �� ������� ������������� �������
            if (audioSource == musicSource)
            {
                // ���������� ����������� ��������
                continue;
            }

            // �������� ��� ��������� ������������� � ����������� �� �������� ���������
            audioSource.enabled = !isSoundOn;
        }

        // ������������� ���������
        isSoundOn = !isSoundOn;

        // �������� ����������� �� ������ ������
        UpdateSoundButtonImage();

        // ��������� ������ �� ����� � ����
        SaveAudioData();
        Debug.Log("isSoundOn: " + isSoundOn);
        Debug.Log("isMusicOn: " + isMusicOn);
    }
    
    // ����� ��� ����������/��������� ������
    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;

        if (isMusicOn)
        {
            // �������� ������, ���� ��� ���� ���������
            musicSource.Play();
        }
        else
        {
            // ��������� ������
            musicSource.Stop();
        }

        // �������� ����������� �� ������ ������
        UpdateMusicButtonImage();

        // ��������� ������ �� ����� � ����
        SaveAudioData();
        Debug.Log("isSoundOn: " + isSoundOn);
        Debug.Log("isMusicOn: " + isMusicOn);
    }

    // ����� ��� ���������� ������ �� ����� � ����
    public void SaveAudioData()
    {
        // ������� ������ AudioData � �������� ���������� ����������
        AudioData data = new AudioData
        {
            isSoundOn = isSoundOn,
            isMusicOn = isMusicOn
        };

        // ��������� ������ � YandexGame.savesData.audioData
        YandexGame.savesData.audioData = data;

        // ������ ������� ��������� ������
        YandexGame.SaveProgress();
    }

    // ����� ��� �������� ������ �� ����� �� �����
    // ����� ��� �������� ������ �� ����� �� ����� � ���������/���������� ������
    public void LoadAudioData()
    {
        // ��������� ������ �� YandexGame.savesData.audioData
        AudioData data = YandexGame.savesData.audioData;

        // ���� ������ ����������, ��������� ����� ���������
        if (data != null)
        {
            isSoundOn = data.isSoundOn;
            isMusicOn = data.isMusicOn;

            // �������� ��� ��������� ������ � ����������� �� �������� isMusicOn
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

            // ������� ��� �������������� � �����
            AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

            // �������� �� ������� ��������������
            foreach (AudioSource audioSource in allAudioSources)
            {
                // ���������, �������� �� ������� ������������� �������
                if (audioSource == musicSource)
                {
                    // ���������� ����������� ��������
                    continue;
                }

                // �������� ��� ��������� ������������� � ����������� �� �������� isSoundOn
                audioSource.enabled = isSoundOn;
            }

            // ��������� ����������� �� ������ �����
            UpdateSoundButtonImage();
        }
        else
        {
            Debug.LogWarning("Audio data does not exist.");
        }



    }


}
