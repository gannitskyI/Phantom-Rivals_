using UnityEngine;
using TMPro;
using I2.Loc;

public class PlayerCountManager : MonoBehaviour
{
    public TextMeshProUGUI playerCountText;

    public int playerCount = 1;
    private SelectorCars selector;
    private MainMenu mainMenu;

    public static PlayerCountManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        SelectorCars selectorCars = SelectorCars.Instance;
        if (selectorCars != null)
        {
            selectorCars.UpdatePlayerCount(playerCount);
            selectorCars.SelectorCarsAppeared += SelectorCarsAppearedHandler;
        }
        else
        {
            Debug.LogWarning("SelectorCars instance is not available.");
        }
        mainMenu = FindObjectOfType<MainMenu>();
        // ������������� �� ������� ��������� �����
        I2.Loc.LocalizationManager.OnLocalizeEvent += OnLanguageChanged;
    }

    private void OnDestroy()
    {
        // ������������ �� ������� ��� ����������� �������
        I2.Loc.LocalizationManager.OnLocalizeEvent -= OnLanguageChanged;
    }

    private void OnLanguageChanged()
    {
        // ��������� ����� �����, ��������, ����� UpdatePlayerCountText()
        UpdatePlayerCountText();
    }

    private void SelectorCarsAppearedHandler()
    {
        SelectorCars selectorCars = SelectorCars.Instance;
        if (selectorCars != null)
        {
            selectorCars.UpdatePlayerCount(playerCount);
            selectorCars.SelectorCarsAppeared -= SelectorCarsAppearedHandler;
        }
    }

    public void IncrementPlayerCount()
    {

        if (playerCount < 2)
        {
            mainMenu.PlayButtonSound();
            playerCount++;
            UpdatePlayerCountText();

            SelectorCars selectorCars = SelectorCars.Instance;
            if (selectorCars != null)
            {
                selectorCars.UpdatePlayerCount(playerCount);
            }
            else
            {
                Debug.LogWarning("SelectorCars instance is not available.");
            }
        }
    }

    public void DecrementPlayerCount()
    {
        if (playerCount > 1)
        {
            mainMenu.PlayButtonSound();
            playerCount--;
            UpdatePlayerCountText();

            SelectorCars selectorCars = SelectorCars.Instance;
            if (selectorCars != null)
            {
                selectorCars.UpdatePlayerCount(playerCount);
            }
            else
            {
                Debug.LogWarning("SelectorCars instance is not available.");
            }
        }
    }

    private void UpdatePlayerCountText()
    {
        playerCountText.text = LocalizationManager.GetTranslation("PLAYER_COUNT") + " " + playerCount.ToString();
    }
}
