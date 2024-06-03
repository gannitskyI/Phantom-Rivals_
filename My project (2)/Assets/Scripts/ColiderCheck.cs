using UnityEngine;

public class ColiderCheck : MonoBehaviour
{
    public bool playerTouching = false;
    private bool triggerActivated = false;
    private TaskChecker checker;
    private ModeChange modeChange; // Добавляем ссылку на ModeChange

    private void Awake()
    {
        checker = FindObjectOfType<TaskChecker>();
        modeChange = FindObjectOfType<ModeChange>(); // Ищем экземпляр ModeChange
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (checker != null && checker.gameObject.activeInHierarchy) // Проверяем, что TaskChecker существует и включен
        {
            // Проверяем, что триггер сработал на объекте с компонентом PlayerControl
            PlayerControl playerControl = other.GetComponent<PlayerControl>();
            if (!triggerActivated && playerControl != null && modeChange.currentIndex == 1)
            {
                playerTouching = true;
                triggerActivated = true;
                Debug.Log($"Player touched the fence! playerTouching: {playerTouching}");
                checker.ThreeTask();
            }
        }
    }
}
