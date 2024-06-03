using System;
using System.Collections.Generic;
using UnityEngine;
using URandom = UnityEngine.Random;

public class BotSpawner : MonoBehaviour
{
    // Массив ссылок на префабы гонщиков ботов
    public GameObject[] botPrefabs;

    // Массив точек спавна
    public Transform[] spawnPoints;

    // Количество гонщиков ботов
    public int botCount;

    // Массив для хранения изначальных точек спавна
    private Transform[] originalSpawnPoints;

    // Массив для хранения изначальных префабов ботов
    private GameObject[] originalBotPrefabs;

    // Список для хранения созданных ботов
    public List<GameObject> spawnedBots = new List<GameObject>();

    // Списки для отслеживания использованных точек спауна и префабов ботов
    private List<Transform> usedSpawnPoints = new List<Transform>();
    private List<GameObject> usedBotPrefabs = new List<GameObject>();

    // Метод, вызываемый при старте сцены
    private void Start()
    {
        // Сохраняем изначальные данные
        SaveOriginalData();

        // Спауним ботов
        SpawnBot();
    }

    // Метод для сохранения изначальных данных
    private void SaveOriginalData()
    {
        originalSpawnPoints = new Transform[spawnPoints.Length];
        Array.Copy(spawnPoints, originalSpawnPoints, spawnPoints.Length);

        originalBotPrefabs = new GameObject[botPrefabs.Length];
        Array.Copy(botPrefabs, originalBotPrefabs, botPrefabs.Length);
    }

    public void SpawnBot()
    {
        // Проверяем, что количество гонщиков ботов не превышает количество точек спавна
        if (botCount > originalSpawnPoints.Length)
        {
            Debug.LogWarning("Количество гонщиков ботов больше, чем количество точек спавна");
            botCount = originalSpawnPoints.Length;
        }

        // Проверяем, что количество гонщиков ботов не превышает количество префабов ботов
        if (botCount > originalBotPrefabs.Length)
        {
            Debug.LogWarning("Количество гонщиков ботов больше, чем количество префабов ботов");
            botCount = originalBotPrefabs.Length;
        }

        // Спавним гонщиков ботов в изначальных точках спавна
        for (int i = 0; i < botCount; i++)
        {
            // Выбираем случайную точку спавна, которая еще не была использована
            int index = GetUnusedSpawnPointIndex();

            // Выбираем префаб бота из изначальных данных, который еще не был использован
            int j = GetUnusedBotPrefabIndex();

            GameObject bot = Instantiate(originalBotPrefabs[j], originalSpawnPoints[index].position, Quaternion.Euler(0, 0, -277));

            spawnedBots.Add(bot);

            // Добавляем использованные точку спавна и префаб бота в списки
            usedSpawnPoints.Add(originalSpawnPoints[index]);
            usedBotPrefabs.Add(originalBotPrefabs[j]);
        }
    }

    // Метод для получения индекса неиспользованной точки спавна
    private int GetUnusedSpawnPointIndex()
    {
        int index = URandom.Range(0, originalSpawnPoints.Length);

        // Проверяем, что точка спавна еще не использована
        while (usedSpawnPoints.Contains(originalSpawnPoints[index]))
        {
            index = URandom.Range(0, originalSpawnPoints.Length);
        }

        return index;
    }

    // Метод для получения индекса неиспользованного префаба бота
    private int GetUnusedBotPrefabIndex()
    {
        int index = URandom.Range(0, originalBotPrefabs.Length);

        // Проверяем, что префаб бота еще не использован
        while (usedBotPrefabs.Contains(originalBotPrefabs[index]))
        {
            index = URandom.Range(0, originalBotPrefabs.Length);
        }

        return index;
    }

    public void DespawnBots()
    {
        // Записываем количество ботов перед удалением
        Debug.Log("Количество ботов перед удалением: " + spawnedBots.Count);

        // Перебираем каждого бота и уничтожаем его
        foreach (var bot in spawnedBots)
        {
            Destroy(bot);
        }

        // Записываем количество ботов после удаления
        Debug.Log("Количество ботов после удаления: " + spawnedBots.Count);

        // Очищаем списки
        spawnedBots.Clear();
        usedSpawnPoints.Clear();
        usedBotPrefabs.Clear();
    }

    public void RespawnBots()
    {
        // Удаляем все текущие боты
        DespawnBots();

        // Пересоздаем списки изначальных данных перед респауном
        usedSpawnPoints.Clear();
        usedBotPrefabs.Clear();

        // Спавним новых ботов
        SpawnBot();
    }
}