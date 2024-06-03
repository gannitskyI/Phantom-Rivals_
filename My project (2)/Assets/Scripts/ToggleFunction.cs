using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class ToggleFunction : MonoBehaviour
{
    public Toggle toggle;
    public TMP_Text toggleName;
    public Button toggleButton;
    private ModeSwitcher modeSwitcher;
    // ������������, ��� � checkmark ���� ��������� Graphic (Image ��� RawImage).
    public Graphic checkmark;

    private void Awake()
    {
        // ������������, ��� ��������� ��������� toggle - ���������.
        ToggleCheckbox(false);
    }
    private void Start()
    {
        modeSwitcher = FindObjectOfType<ModeSwitcher>();
    }
    public void ToggleCheckbox(bool isChecked)
    {
        float targetAlpha = isChecked ? 1 : 0;
        float duration = 0.3f;

        // ���������� DOTween ��� �������� ��������� �����-������ ��� toggle � ��� �����
        toggle.image.DOFade(targetAlpha, duration);
        toggleName.DOFade(targetAlpha, duration);

        // ���������� DOTween ��� �������� ��������� �����-������ ��� ������� (checkmark)
        if (checkmark != null)
        {
            checkmark.DOFade(targetAlpha, duration);
        }

        // �������� ��� ��������� ���������� ������ � ����������� �� �������� isChecked
        toggleButton.interactable = isChecked;
    }

    public void IsOn()
    {
        modeSwitcher.TaskChange();
        // ����������� ������� �������� toggle.isOn
        toggle.isOn = !toggle.isOn;
    }
}