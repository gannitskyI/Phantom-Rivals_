using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sixTaskCheck : MonoBehaviour
{
    public static bool playerTouching = false; // Статическая переменная
    private static bool triggerActivated = false;
    private TaskChecker checker;
    private ModeChange modeChange; // Добавляем ссылку на ModeChange

    private void Awake()
    {
        checker = FindObjectOfType<TaskChecker>();
        modeChange = FindObjectOfType<ModeChange>(); // Ищем экземпляр ModeChange
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (checker != null && checker.gameObject.activeInHierarchy) // Проверяем, что TaskChecker существует и включен
        {
            PlayerControl playerControl = collision.gameObject.GetComponent<PlayerControl>();

            if (!triggerActivated && playerControl != null && modeChange.currentIndex == 1)
            {
                playerTouching = true;
                triggerActivated = true;
                Debug.Log($"Игрок коснулся бота! botTouching: {playerTouching}");
                checker.SixTask();
            }
        }
    }
}
