using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeImage : MonoBehaviour
{
    public Image imageToFade; // ������ Image, ������������ �������� ����� ��������
    public float duration = 1f; // ����������������� ��������

    void Start()
    {
        // ���������, ��� � ��� ���� ������ �� ��������� Image
        if (imageToFade == null)
        {
            imageToFade = GetComponent<Image>();
        }

        // ���������, ��� �� ������ ��������� Image
        if (imageToFade != null)
        {
            imageToFade.color = new Color(imageToFade.color.r, imageToFade.color.g, imageToFade.color.b, 1f);
            // �������� ������������ ����������� �� 0 � ������� ��������� �������
            imageToFade.DOFade(0f, duration);
        }
        else
        {
            Debug.LogError("�� ������� ����� ��������� Image ��� ��������� ������������.");
        }
    }
}
