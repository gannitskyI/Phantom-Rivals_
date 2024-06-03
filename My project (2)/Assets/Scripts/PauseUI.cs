using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    // ������ �� UI ��������
    public GameObject pauseMenu; // ���� �����
    public TextMeshProUGUI countdownText; // ����� ��������� �������

    // ���������� ��� ���������� ������
    private bool isPaused = false; // ���� �����
    private float countdown = 3f; // ����� ��������� �������
    public bool isCountingDown = false; // ���� ��������� �������

    // �����, ������� ���������� ��� ������� �� ������ �����
    public void PauseGame()
    {
        if (!isPaused) // ���� ���� �� �� �����
        {
            isPaused = true; // ���������� ���� �����
            Time.timeScale = 0f; // ���������� ����� � ����
            pauseMenu.SetActive(true); // ������������ ���� �����

            // �������� ���������� ��������� �������
            isCountingDown = false;
            countdownText.gameObject.SetActive(false);
            countdown = 3f;
        }
    }

    // �����, ������� ���������� ��� ������� �� ������ ���������� ����
    public void ResumeGame()
    {
        if (isPaused) // ���� ���� �� �����
        {
            isPaused = false; // ����� ���� �����
            pauseMenu.SetActive(false); // �������������� ���� �����
            isCountingDown = true; // ���������� ���� ��������� �������
            countdownText.gameObject.SetActive(true); // ������������ ����� ��������� �������

            AudioListener.volume = 1f;
        }
    }

    // �����, ������� ���������� ������ ����
    private void Update()
    {
        if (isCountingDown) // ���� ���� �������� ������
        {
            countdown -= Time.unscaledDeltaTime; // ��������� ����� ��������� �������
            countdownText.text = Mathf.Ceil(countdown).ToString(); // ���������� ����� ����� ������
            if (countdown <= 0f) // ���� �������� ������ ����������
            {
                isCountingDown = false; // ����� ���� ��������� �������
                Time.timeScale = 1f; // ����������� ����� � ����
                countdownText.gameObject.SetActive(false); // �������������� ����� ��������� �������
                countdown = 3f; // �������� ����� ��������� �������
            }
        }
    }
}
