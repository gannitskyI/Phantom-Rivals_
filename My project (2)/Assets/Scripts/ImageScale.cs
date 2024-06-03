using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageScale : MonoBehaviour
{
    private RectTransform rectTransform;
    private Image imageComponent;

    // ����������� ������� �����������
    private Vector2 initialSize;

    // Start is called before the first frame update
    void Start()
    {
        // �������� ���������� RectTransform � Image
        rectTransform = GetComponent<RectTransform>();
        imageComponent = GetComponent<Image>();

        // ��������� ����������� ������� �����������
        initialSize = rectTransform.sizeDelta;

        // �������� ����� ��� ��������� ������� ����������� ��� �������
        ResizeImage();
    }

    // Update is called once per frame
    void Update()
    {
        // �������� ����� ��� ��������� ������� ����������� ��� ��������� ������� ������
        ResizeImage();
    }

    void ResizeImage()
    {
        // �������� ������� ������� ������
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // ������������ ����������� ������ ������ � ������������ ������� (1920x1080)
        float widthRatio = screenWidth / 1920f;
        float heightRatio = screenHeight / 1080f;

        // �������� ���������� ����������� ���������������, ����� ����������� ���������� �� �����
        float scaleRatio = Mathf.Min(widthRatio, heightRatio);

        // ��������� ��������������� � ����������� �������� �����������
        Vector2 newSize = initialSize * scaleRatio;

        // ��������� ����� ������ � RectTransform �����������
        rectTransform.sizeDelta = newSize;
    }
}
