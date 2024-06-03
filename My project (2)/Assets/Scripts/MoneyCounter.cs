using UnityEngine;
using TMPro;
using System.IO;
using DG.Tweening;
using YG;

public class MoneyCounter : MonoBehaviour
{
    public static MoneyCounter Instance;
    public TextMeshProUGUI coinText;
    public int moneyAmount = 5;
    private static int score = 0;
 
    public float moveSpeed = 1f; // �������� ����������� �������
    public float oscillationDistance = 1f; // ���������� ���������
    public Axis axis = Axis.X; // ����� ���

    private Tween oscillationTween; // ������ �� ���� ��� ����������� �������

    public delegate void MoneyUpdatedHandler(int newScore);
    public static event MoneyUpdatedHandler OnMoneyUpdated;

    public AudioClip coinPickupSound; // ��������� ��� ����� ����� ������
    public AudioSource audioSource; // �������������
 

    private void Start()
    { 
        
        LoadScore();
       
        StartOscillation();
    }
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            AddMoney(moneyAmount);
            // ������������� �������� ����� ������������ �������
            if (oscillationTween != null)
                oscillationTween.Kill();

            // ������������� ���� ����� ������, ������ ���� AudioSource �������
            if (audioSource != null && audioSource.enabled)
                audioSource.PlayOneShot(coinPickupSound);

            Destroy(gameObject);
        }
    }
    private void OnApplicationQuit()
    {
        SaveScore();
    }

    public void AddMoney(int amount)
    {
        score += amount;
        UpdateScoreText();
        SaveScore();
        OnMoneyUpdated?.Invoke(score);
    }

    public void UpdateScoreText()
    { 
        coinText.text = score.ToString() + " " + "$";
    }

    private void SaveScore()
    {
        // ������� ����� ��������� ScoreData � ��������� ������� ����
        ScoreData data = new ScoreData { Score = score };

        // ��������� ������ � YandexGame.savesData.scoreData
        YandexGame.savesData.scoreData = data;

        // ������ ������� ��������� ������
        YandexGame.SaveProgress();
    }

    private void LoadScore()
    {
        // ��������� ������ �� YandexGame.savesData.scoreData
        ScoreData data = YandexGame.savesData.scoreData;

        // ���� ������ ����������, ��������� ����
        if (data != null)
        {
            score = data.Score;
            coinText.text = score.ToString() + " " + "$";
        }
    }

    // ����� ��� ������� �������� ����������� �������
    private void StartOscillation()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = axis == Axis.X ? startPos + Vector3.right * oscillationDistance : startPos + Vector3.up * oscillationDistance;
        // ��������� ������ �� ���� ��� ����������� �������
        oscillationTween = transform.DOMove(endPos, moveSpeed).SetLoops(-1, LoopType.Yoyo);
    }
}


[System.Serializable]
public class ScoreData
{
    public int Score;
}

public enum Axis
{
    X,
    Y
}
