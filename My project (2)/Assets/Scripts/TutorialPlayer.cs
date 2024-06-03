using UnityEngine;
using TMPro;
using UnityEngine.UI;
using YG;

public class TutorialPlayer : MonoBehaviour
{
    public TextMeshProUGUI[] tutorialMessages;
    public Button[] tutorialButtons;
    private int currentMessageIndex = -1;

    public GameObject[] traningPanel;
    private bool tutorialHidden = false;

    void Start()
    {
        GetData();
    }

    void Update()
    {
        if (!tutorialHidden && Input.GetMouseButtonDown(0))
        {
            if (currentMessageIndex >= 0 && currentMessageIndex < tutorialMessages.Length && !tutorialButtons[0].IsInteractable() && !tutorialButtons[1].IsInteractable() && !tutorialButtons[2].IsInteractable() && !tutorialButtons[3].IsInteractable() && !tutorialButtons[4].IsInteractable() && !tutorialButtons[5].IsInteractable())
            {
                ShowNextMessage();
            }
        }
    }

    void ShowNextMessage()
    {
        if (currentMessageIndex >= 0 && currentMessageIndex < tutorialMessages.Length)
        {
            tutorialMessages[currentMessageIndex].gameObject.SetActive(false);
        }

        currentMessageIndex++;

        if (currentMessageIndex < tutorialMessages.Length)
        {
            tutorialMessages[currentMessageIndex].gameObject.SetActive(true);

            if (currentMessageIndex == 2)
            {
                tutorialButtons[0].interactable = true;
                tutorialButtons[0].onClick.AddListener(() => SkipMessage());
                foreach (var panel in traningPanel)
                {
                    panel.SetActive(false);
                }
            }

            if (currentMessageIndex == 4)
            {
                tutorialButtons[1].interactable = true;
                tutorialButtons[1].onClick.AddListener(() => ShowSixthMessage());
                foreach (var panel in traningPanel)
                {
                    panel.SetActive(false);
                }
            }

            if (currentMessageIndex == 6)
            {
                tutorialButtons[2].interactable = true;
                tutorialButtons[2].onClick.AddListener(() => ShowEighthMessage());
                foreach (var panel in traningPanel)
                {
                    panel.SetActive(false);
                }
            }

            if (currentMessageIndex == 8)
            {
                tutorialButtons[3].interactable = true;
                tutorialButtons[3].onClick.AddListener(() => ShowTenthMessage());
                foreach (var panel in traningPanel)
                {
                    panel.SetActive(false);
                }
            }

            if (currentMessageIndex == 10)
            {
                tutorialButtons[4].interactable = true;
                tutorialButtons[4].onClick.AddListener(() => ShowTwelfthMessage());
                foreach (var panel in traningPanel)
                {
                    panel.SetActive(false);
                }
            }
 
        }
        else
        {
            HideTutorial();
        }
    }

    void ShowTwelfthMessage()
    {
        if (currentMessageIndex == 10)
        {
            tutorialMessages[currentMessageIndex].gameObject.SetActive(false);
            currentMessageIndex++;
            tutorialMessages[currentMessageIndex].gameObject.SetActive(true);
            tutorialButtons[4].interactable = false;
            foreach (var panel in traningPanel)
            {
                panel.SetActive(true);
            }
        }
    }

    void ShowTenthMessage()
    {
        if (currentMessageIndex == 8)
        {
            tutorialMessages[currentMessageIndex].gameObject.SetActive(false);
            currentMessageIndex++;
            tutorialMessages[currentMessageIndex].gameObject.SetActive(true);
            tutorialButtons[3].interactable = false;
            foreach (var panel in traningPanel)
            {
                panel.SetActive(true);
            }
        }
    }

    void ShowEighthMessage()
    {
        if (currentMessageIndex == 6)
        {
            tutorialMessages[currentMessageIndex].gameObject.SetActive(false);
            currentMessageIndex++;
            tutorialMessages[currentMessageIndex].gameObject.SetActive(true);
            tutorialButtons[2].interactable = false;
            foreach (var panel in traningPanel)
            {
                panel.SetActive(true);
            }
        }
    }

    void ShowSixthMessage()
    {
        if (currentMessageIndex == 4)
        {
            tutorialMessages[currentMessageIndex].gameObject.SetActive(false);
            currentMessageIndex++;
            tutorialMessages[currentMessageIndex].gameObject.SetActive(true);
            tutorialButtons[1].interactable = false;
            foreach (var panel in traningPanel)
            {
                panel.SetActive(true);
            }
        }
    }

    void SkipMessage()
    {
        if (currentMessageIndex == 2)
        {
            tutorialMessages[currentMessageIndex].gameObject.SetActive(false);
            currentMessageIndex++;
            tutorialMessages[currentMessageIndex].gameObject.SetActive(true);
            tutorialButtons[0].interactable = false;
            foreach (var panel in traningPanel)
            {
                panel.SetActive(true);
            }
        }
    }
    public void GetData()
    {
        // «агружаем данные из сохранений
        SavesYG savesData = YandexGame.savesData;

        // ѕровер€ем, прошел ли игрок туториал
        bool tutorialCompleted = savesData.tutorialCompleted;

        // ≈сли туториал не завершен, показываем его
        if (!tutorialCompleted)
        {
            foreach (var button in tutorialButtons)
            {
                button.interactable = false;
            }

            ShowNextMessage();
        }
        else
        {
            // ≈сли туториал завершен, скрываем его
            HideTutorial();
        }
    }

        void HideTutorial()
    {
        foreach (var message in tutorialMessages)
        {
            message.gameObject.SetActive(false);
        }

        for (int i = 0; i < tutorialButtons.Length; i++)
        {
            if (i != 0)
            {
                tutorialButtons[i].interactable = true;
            }
        }
        foreach (var panel in traningPanel)
        {
            panel.SetActive(false);
        }

        tutorialHidden = true;

        // ”становка значени€ tutorialCompleted в true при завершении туториала
        YandexGame.savesData.tutorialCompleted = true;

        // —охранение данных о прогрессе
        YandexGame.SaveProgress();

    }
}
