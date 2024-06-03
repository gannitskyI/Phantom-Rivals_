using System;
using System.Collections.Generic;
using UnityEngine;
using URandom = UnityEngine.Random;

public class BotSpawner : MonoBehaviour
{
    // ������ ������ �� ������� �������� �����
    public GameObject[] botPrefabs;

    // ������ ����� ������
    public Transform[] spawnPoints;

    // ���������� �������� �����
    public int botCount;

    // ������ ��� �������� ����������� ����� ������
    private Transform[] originalSpawnPoints;

    // ������ ��� �������� ����������� �������� �����
    private GameObject[] originalBotPrefabs;

    // ������ ��� �������� ��������� �����
    public List<GameObject> spawnedBots = new List<GameObject>();

    // ������ ��� ������������ �������������� ����� ������ � �������� �����
    private List<Transform> usedSpawnPoints = new List<Transform>();
    private List<GameObject> usedBotPrefabs = new List<GameObject>();

    // �����, ���������� ��� ������ �����
    private void Start()
    {
        // ��������� ����������� ������
        SaveOriginalData();

        // ������� �����
        SpawnBot();
    }

    // ����� ��� ���������� ����������� ������
    private void SaveOriginalData()
    {
        originalSpawnPoints = new Transform[spawnPoints.Length];
        Array.Copy(spawnPoints, originalSpawnPoints, spawnPoints.Length);

        originalBotPrefabs = new GameObject[botPrefabs.Length];
        Array.Copy(botPrefabs, originalBotPrefabs, botPrefabs.Length);
    }

    public void SpawnBot()
    {
        // ���������, ��� ���������� �������� ����� �� ��������� ���������� ����� ������
        if (botCount > originalSpawnPoints.Length)
        {
            Debug.LogWarning("���������� �������� ����� ������, ��� ���������� ����� ������");
            botCount = originalSpawnPoints.Length;
        }

        // ���������, ��� ���������� �������� ����� �� ��������� ���������� �������� �����
        if (botCount > originalBotPrefabs.Length)
        {
            Debug.LogWarning("���������� �������� ����� ������, ��� ���������� �������� �����");
            botCount = originalBotPrefabs.Length;
        }

        // ������� �������� ����� � ����������� ������ ������
        for (int i = 0; i < botCount; i++)
        {
            // �������� ��������� ����� ������, ������� ��� �� ���� ������������
            int index = GetUnusedSpawnPointIndex();

            // �������� ������ ���� �� ����������� ������, ������� ��� �� ��� �����������
            int j = GetUnusedBotPrefabIndex();

            GameObject bot = Instantiate(originalBotPrefabs[j], originalSpawnPoints[index].position, Quaternion.Euler(0, 0, -277));

            spawnedBots.Add(bot);

            // ��������� �������������� ����� ������ � ������ ���� � ������
            usedSpawnPoints.Add(originalSpawnPoints[index]);
            usedBotPrefabs.Add(originalBotPrefabs[j]);
        }
    }

    // ����� ��� ��������� ������� ���������������� ����� ������
    private int GetUnusedSpawnPointIndex()
    {
        int index = URandom.Range(0, originalSpawnPoints.Length);

        // ���������, ��� ����� ������ ��� �� ������������
        while (usedSpawnPoints.Contains(originalSpawnPoints[index]))
        {
            index = URandom.Range(0, originalSpawnPoints.Length);
        }

        return index;
    }

    // ����� ��� ��������� ������� ����������������� ������� ����
    private int GetUnusedBotPrefabIndex()
    {
        int index = URandom.Range(0, originalBotPrefabs.Length);

        // ���������, ��� ������ ���� ��� �� �����������
        while (usedBotPrefabs.Contains(originalBotPrefabs[index]))
        {
            index = URandom.Range(0, originalBotPrefabs.Length);
        }

        return index;
    }

    public void DespawnBots()
    {
        // ���������� ���������� ����� ����� ���������
        Debug.Log("���������� ����� ����� ���������: " + spawnedBots.Count);

        // ���������� ������� ���� � ���������� ���
        foreach (var bot in spawnedBots)
        {
            Destroy(bot);
        }

        // ���������� ���������� ����� ����� ��������
        Debug.Log("���������� ����� ����� ��������: " + spawnedBots.Count);

        // ������� ������
        spawnedBots.Clear();
        usedSpawnPoints.Clear();
        usedBotPrefabs.Clear();
    }

    public void RespawnBots()
    {
        // ������� ��� ������� ����
        DespawnBots();

        // ����������� ������ ����������� ������ ����� ���������
        usedSpawnPoints.Clear();
        usedBotPrefabs.Clear();

        // ������� ����� �����
        SpawnBot();
    }
}