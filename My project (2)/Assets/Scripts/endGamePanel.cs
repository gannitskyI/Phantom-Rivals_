using UnityEngine;
using I2.Loc;
using TMPro;
using YG;
public class endGamePanel : MonoBehaviour
{
 
    private PlayerControl playerControl;
    private PlayerCars playerCars;
    private CanvasFunction canvasFunction;
    private AudioManagers audioManagers;
    public GameObject endPanel;
    private EventGame eventGame;
    private CarResetter carResetter;
    [SerializeField]
    private BotSpawner botSpawner;

    public TextMeshProUGUI playerCountText;
    public int playerCount = 0;
    public static endGamePanel Instance { get; private set; }

    private void Awake()
    { 
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
    private void UpdatePlayerCountText()
    { 
        playerCountText.text = LocalizationManager.GetTranslation("PLAYER_1") + " " + playerCount.ToString();
    }
    private void Start()
    {
        
        playerCount++;

        UpdatePlayerCountText();

        // ���������, ����� �� currentIndex 1, ������ ��� ������ � ������������� BotSpawner
        if (FindObjectOfType<ModeChange>().currentIndex == 1)
        {
            // ����� ������� BotSpawner �� �����
            GameObject botSpawnerObject = GameObject.Find("Bot");

            // ��������� ���������� BotSpawner �� ���������� �������
            if (botSpawnerObject != null)
            {
                botSpawner = botSpawnerObject.GetComponent<BotSpawner>();
            }
            else
            {
                Debug.LogError("������: ������ BotSpawner �� ������.");
            }
        }

        playerControl = FindObjectOfType<PlayerControl>();
        playerCars = FindObjectOfType<PlayerCars>();
        canvasFunction = FindObjectOfType<CanvasFunction>();
        eventGame = FindObjectOfType<EventGame>();
        audioManagers = FindObjectOfType<AudioManagers>();
        
 

        if (canvasFunction == null)
        {
            Debug.LogError("CanvasFunction �� ������ �� �����. ���������, ��� �� ��������� �� ������ �������.");
        }
    }

    private void ShowEndGameWindow()
    {
        endPanel.SetActive(false);
        
    }

    public void ButtonContinue()
    { 
        playerCount++;

        UpdatePlayerCountText();
        ShowEndGameWindow();

        canvasFunction.RestartCountdown();
      
        if (FindObjectOfType<ModeChange>().currentIndex == 1)
        {
            botSpawner.RespawnBots();
           
        }

        playerCars.SwitchToNextPlayer();
        audioManagers.RepeatCheckSound();
        CameraFollow.instance.UpdateTarget();
    }
}