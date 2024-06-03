using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactsCheck : MonoBehaviour
{
    public static bool botTouching = false; // ����������� ����������
    private static bool triggerActivated = false;
    private TaskChecker checker;

    private void Awake()
    {
        checker = FindObjectOfType<TaskChecker>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ���������, ��� ������� �������� �� ������� � ����������� PlayerControl
        PlayerControl playerControl = other.GetComponent<PlayerControl>();
        if (!triggerActivated && playerControl != null)
        {
            botTouching = true;
            triggerActivated = true;
            Debug.Log($"Player touched the bot! obstacleTouching: {botTouching}");
            
        }
    }
}
