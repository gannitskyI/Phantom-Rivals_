using UnityEngine;

public class lifeCar : MonoBehaviour
{
    public int maxLives = 3; // ������������ ���������� ������ ������
    private int currentLives; // ������� ���������� ������ ������
    public GameObject loseWindow;
    public GameObject[] obstacles; // ������ �����������
    private GhostTrail ghostTrail;

    private void Start()
    {
        ghostTrail = GetComponent <GhostTrail>();
        currentLives = maxLives; // ������������� ��������� ���������� ������
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ��������� ������������ � ������������, ��������� � �������� obstacles
        foreach (GameObject obstacle in obstacles)
        {
            if (collision.gameObject == obstacle)
            {
                LoseLife(); // �������� ����� ������ �����
                break; // ������� �� �����, ��� ��� ����� �����������
            }
        }
    }

    private void LoseLife()
    {
        currentLives--; // ��������� ���������� ������ �� 1

        if (currentLives <= 0)
        {
            EndGame(); // ���� ������ �� ��������, �������� ����� ���������� ����
        }
    }

    private void EndGame()
    {
        loseWindow.SetActive(true);
    }
}
