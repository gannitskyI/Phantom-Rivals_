using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalerImage : MonoBehaviour
{
    private RectTransform rectTransform;
    private Canvas canvas;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        // �������� ����� ��� ��������� �������� ������� Image ��� ������� ����
        ResizeImage();
    }

    void Update()
    {
        // ���������, ���������� �� ���������� ������
        if (Screen.width != canvas.pixelRect.width || Screen.height != canvas.pixelRect.height)
        {
            // �������� ����� ��� ��������� �������� ������� Image ��� ��������� ���������� ������
            ResizeImage();
        }
    }

    void ResizeImage()
    {
        // ������������� ������� ������� Image ���, ����� �� ������� ��� ��������� ������� ������
        rectTransform.sizeDelta = new Vector2(canvas.pixelRect.width, canvas.pixelRect.height);
    }
}
