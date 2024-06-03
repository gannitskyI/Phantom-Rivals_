using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RaceCheckpoint : MonoBehaviour
{
    public Transform[] checkpoints;
    public Transform[] players;
    public TextMeshProUGUI leaderboardText;

    private int[] scores;
    private int numberOfCheckpoints;
    private int numberOfPlayers;

    void Start()
    {
        numberOfCheckpoints = checkpoints.Length;
        numberOfPlayers = players.Length;

        scores = new int[numberOfPlayers];

        for (int i = 0; i < numberOfPlayers; i++)
        {
            scores[i] = 0;
        }
    }

    void Update()
    {
        UpdateLeaderboard();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Checkpoint"))
        {
            CheckpointReached(collision.transform);
            Debug.LogError("CanvasFunction не найден на сцене. Убедитесь, что он находится на другом объекте.");
        }
    }

    void CheckpointReached(Transform checkpoint)
    {
        int checkpointIndex = System.Array.IndexOf(checkpoints, checkpoint);

        if (checkpointIndex == scores[checkpoint.parent.GetSiblingIndex()])
        {
            scores[checkpoint.parent.GetSiblingIndex()]++;

            if (checkpointIndex == numberOfCheckpoints - 1)
            {
                Debug.Log("Player/Bot " + checkpoint.parent.name + " finished the race!");
            }
            else
            {
                Debug.Log("Player/Bot " + checkpoint.parent.name + " reached checkpoint " + checkpointIndex);
            }
        }
        else
        {
            Debug.Log("Incorrect checkpoint order for Player/Bot " + checkpoint.parent.name);
        }
    }

    void UpdateLeaderboard()
    {
        int[] sortedScores = (int[])scores.Clone();
        System.Array.Sort(sortedScores);

        leaderboardText.text = "Leaderboard\n";
        for (int i = numberOfPlayers - 1; i >= 0; i--)
        {
            int playerIndex = System.Array.IndexOf(scores, sortedScores[i]);
            leaderboardText.text += (numberOfPlayers - i) + ". " + players[playerIndex].name + ": " + scores[playerIndex] + " points\n";
        }
    }
}