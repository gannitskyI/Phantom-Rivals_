using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleCheck : MonoBehaviour
{
    public static bool obstacleTouching = false; // Статическая переменная
    private static bool triggerActivated = false;
    private TaskChecker checker;
    private ModeChange modeChange; // Добавляем ссылку на ModeChange

    private void Awake()
    {
        checker = FindObjectOfType<TaskChecker>();
        modeChange = FindObjectOfType<ModeChange>(); // Ищем экземпляр ModeChange
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, что триггер сработал на объекте с компонентом PlayerControl и currentIndex == 1
        PlayerControl playerControl = other.GetComponent<PlayerControl>();
        if (!triggerActivated && playerControl != null && modeChange.currentIndex == 1)
        {
            obstacleTouching = true;
            triggerActivated = true;
            Debug.Log($"Player touched the fence! obstacleTouching: {obstacleTouching}");
            checker.FourTask();
        }
    }
}
