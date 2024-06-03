using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sixTaskCheck : MonoBehaviour
{
    public static bool playerTouching = false; // ����������� ����������
    private static bool triggerActivated = false;
    private TaskChecker checker;
    private ModeChange modeChange; // ��������� ������ �� ModeChange

    private void Awake()
    {
        checker = FindObjectOfType<TaskChecker>();
        modeChange = FindObjectOfType<ModeChange>(); // ���� ��������� ModeChange
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (checker != null && checker.gameObject.activeInHierarchy) // ���������, ��� TaskChecker ���������� � �������
        {
            PlayerControl playerControl = collision.gameObject.GetComponent<PlayerControl>();

            if (!triggerActivated && playerControl != null && modeChange.currentIndex == 1)
            {
                playerTouching = true;
                triggerActivated = true;
                Debug.Log($"����� �������� ����! botTouching: {playerTouching}");
                checker.SixTask();
            }
        }
    }
}
