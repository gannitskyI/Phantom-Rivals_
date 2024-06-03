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
    // Предполагаем, что у checkmark есть компонент Graphic (Image или RawImage).
    public Graphic checkmark;

    private void Awake()
    {
        // Предполагаем, что начальное состояние toggle - выключено.
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

        // Используем DOTween для плавного изменения альфа-канала для toggle и его метки
        toggle.image.DOFade(targetAlpha, duration);
        toggleName.DOFade(targetAlpha, duration);

        // Используем DOTween для плавного изменения альфа-канала для галочки (checkmark)
        if (checkmark != null)
        {
            checkmark.DOFade(targetAlpha, duration);
        }

        // Включаем или выключаем активность кнопки в зависимости от значения isChecked
        toggleButton.interactable = isChecked;
    }

    public void IsOn()
    {
        modeSwitcher.TaskChange();
        // Инвертируем текущее значение toggle.isOn
        toggle.isOn = !toggle.isOn;
    }
}