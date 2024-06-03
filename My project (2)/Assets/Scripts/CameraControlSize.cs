using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlSize : MonoBehaviour
{
    // ������ �� ������, ������� �� ����� ��������������
    public Camera gameCamera;

    // ������ � ������ ���� ��� ��������� ����������
    private float lastScreenWidth;
    private float lastScreenHeight;

    void Start()
    {
        // ������������� �������� ������ � ������ ������
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
    }

    void Update()
    {
        // ��������, ��������� �� ������ ������ � ������� ���������� ����������
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            // ���������� �������� ������ � ������ ������
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;

            // ��������� ������� ������ � ������������ � ������ ��������� ������
            UpdateCameraSize();
        }
    }

    void UpdateCameraSize()
    {
        // ������ ������ ������� ������
        float aspectRatio = (float)Screen.width / (float)Screen.height;
        float cameraSize = gameCamera.orthographicSize;

        // ���������� ������� ������
        gameCamera.orthographicSize = cameraSize * aspectRatio;
    }
}
