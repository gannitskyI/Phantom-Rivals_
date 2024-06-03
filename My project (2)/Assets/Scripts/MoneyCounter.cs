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
 
    public float moveSpeed = 1f; // Скорость перемещения монетки
    public float oscillationDistance = 1f; // Расстояние колебания
    public Axis axis = Axis.X; // Выбор оси

    private Tween oscillationTween; // Ссылка на твин для перемещения монетки

    public delegate void MoneyUpdatedHandler(int newScore);
    public static event MoneyUpdatedHandler OnMoneyUpdated;

    public AudioClip coinPickupSound; // Аудиоклип для звука сбора монеты
    public AudioSource audioSource; // Аудиоисточник
 

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
            // Останавливаем анимацию перед уничтожением объекта
            if (oscillationTween != null)
                oscillationTween.Kill();

            // Воспроизводим звук сбора монеты, только если AudioSource активен
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
        // Создаем новый экземпляр ScoreData и сохраняем текущий счет
        ScoreData data = new ScoreData { Score = score };

        // Сохраняем данные в YandexGame.savesData.scoreData
        YandexGame.savesData.scoreData = data;

        // Теперь остаётся сохранить данные
        YandexGame.SaveProgress();
    }

    private void LoadScore()
    {
        // Загружаем данные из YandexGame.savesData.scoreData
        ScoreData data = YandexGame.savesData.scoreData;

        // Если данные существуют, обновляем счет
        if (data != null)
        {
            score = data.Score;
            coinText.text = score.ToString() + " " + "$";
        }
    }

    // Метод для запуска анимации перемещения монетки
    private void StartOscillation()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = axis == Axis.X ? startPos + Vector3.right * oscillationDistance : startPos + Vector3.up * oscillationDistance;
        // Сохраняем ссылку на твин для перемещения монетки
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
