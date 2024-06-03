using UnityEngine;

public class TraningFunction : MonoBehaviour
{
    [SerializeField]
    private GameObject fonTraning;

    [SerializeField]
    private GameObject buttonTraning;

    [SerializeField]
    private GameObject menuTraning;

    [SerializeField]
    private GameObject selectorTraning;

    [SerializeField]
    private GameObject gameTraning;

    [SerializeField]
    private GameObject buttonCloseTraning;

    [SerializeField]
    private GameObject moneyYG;

    [SerializeField]
    private GameObject newsPanel;

    private ModeSwitcher modeSwitcher;

    private MainMenu mainMenu;
    private void Start()
    {
        mainMenu = FindObjectOfType<MainMenu>();
    }
        private bool IsObjectActive(GameObject target)
    {
        return target != null && target.activeSelf;
    }

    private void SetObjectsVisibility(GameObject target, bool isActive)
    {
        fonTraning.SetActive(isActive);
        buttonTraning.SetActive(!isActive);
        buttonCloseTraning.SetActive(isActive);
        newsPanel.SetActive(!isActive);
        moneyYG.SetActive(!isActive);
        if (target != null)
        {
            target.SetActive(isActive);
        }
    }

    public void ShowMenuTraning()
    {
        mainMenu.PlayButtonSound();
        bool isActive = !IsObjectActive(menuTraning);
        SetObjectsVisibility(menuTraning, isActive);
        
    }

    public void ShowSelcetorTraning()
    {
        mainMenu.PlayButtonSound();
        bool isActive = !IsObjectActive(selectorTraning);
        SetObjectsVisibility(selectorTraning, isActive);
        modeSwitcher = FindObjectOfType<ModeSwitcher>();
        modeSwitcher.SwipeDown();
    }

    public void ShowGameTraning()
    {
        mainMenu.PlayButtonSound();
        bool isActive = !IsObjectActive(gameTraning);
        SetObjectsVisibility(gameTraning, isActive);
    }
}
