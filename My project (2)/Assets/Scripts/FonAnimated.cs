using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FonAnimated : MonoBehaviour
{
    public Color targetColor;
    public Image image;
    public float animationSpeed = 1.0f;

    private void Start()
    {
        // �������� ������� ���������� Image
        if (image == null)
        {
            Debug.LogError("Image component is missing!");
            return;
        }

        // ������ �������� ������ ���� ��������� RectTransform ����������
        RectTransform rectTransform = image.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("RectTransform component is missing!");
            return;
        }

        // ������ ��������
        PlayFillAnimation();
    }

    private void PlayFillAnimation()
    {
        // ����������� ���������� � ��������� �������� �������
        float startFill = 255f;
        float endFill = 168f;

        // �������� ������������������ �������� � ������� DOTween
        Sequence sequence = DOTween.Sequence();

        // ���������� �������� ��������� �������
        sequence.Append(image.DOColor(new Color(1, 1, 1, endFill / 255f), animationSpeed))
                .AppendInterval(1.0f) // ����� ����� �������� ��������� (����� �������� �� �������������)
                .Append(image.DOColor(new Color(1, 1, 1, startFill / 255f), animationSpeed))
                .SetLoops(-1) // ����������� ����������

                // ��������� ����������� ��������
                .SetEase(Ease.InOutQuad);
    }
}
