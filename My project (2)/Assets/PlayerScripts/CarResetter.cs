using System.Collections.Generic;
using UnityEngine;

public class CarResetter : MonoBehaviour
{

    private Vector2 initialPosition;
    private Quaternion initialRotation;
    private PlayerControl playerControl;
    private void Awake()
    {
        // ��������� ��������� ��������� � �������� ������ ��� ������� ����
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }
     // ����� ��� ������ ������ �� ��������� �����
    public void ResetCar()
    { 
        // ���������� ������ �� ��������� ��������� � ��������
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        // ���� � ������ ���� Rigidbody2D, ���������� � ��������
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
        float secondsToDisable = 3f;
        playerControl = FindObjectOfType<PlayerControl>();
        playerControl.DisableControlForSeconds(secondsToDisable);
    }
}