using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleCheck : MonoBehaviour
{
    public static bool obstacleTouching = false; // ����������� ����������
    private static bool triggerActivated = false;
    private TaskChecker checker;
    private ModeChange modeChange; // ��������� ������ �� ModeChange

    private void Awake()
    {
        checker = FindObjectOfType<TaskChecker>();
        modeChange = FindObjectOfType<ModeChange>(); // ���� ��������� ModeChange
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ���������, ��� ������� �������� �� ������� � ����������� PlayerControl � currentIndex == 1
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
