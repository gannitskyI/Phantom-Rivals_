using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EventGame : MonoBehaviour
{
    public TextMeshProUGUI lapTimesText;

    private bool gameStarted = false;
    private float startDelay = 3f;
    private float startTime;
    private int currentLapNumber = 0; // Added to track lap count

    private void Update()
    {
        if (!gameStarted && Time.time >= startDelay)
        {
            gameStarted = true;
            startTime = Time.time;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameStarted && other.CompareTag("Player"))
        {
            PlayerControl playerControl = other.GetComponent<PlayerControl>();

            if (playerControl != null)
            {
                playerControl.IncrementLapCount();
                playerControl.SetPlayerCars(GameObject.FindObjectOfType<PlayerCars>());
            }

            TextMeshProUGUI timerText = GameObject.Find("Canvas").GetComponent<CanvasFunction>().timerText;
            string formattedTime = FormatTime(timerText.text);

            lapTimesText.text += formattedTime + "\n";

            currentLapNumber++; // Increment lap count

            if (currentLapNumber == 6) // Clear text when 6th lap is reached
            {
                ClearLapTimesText();
                currentLapNumber = 0; // Reset lap count for subsequent laps
            }
        }
    }

    private string FormatTime(string timeText)
    {
        // Existing code for formatting time remains unchanged
        int lapNumber = lapTimesText.text.Split('\n').Length;
        string[] timeComponents = timeText.Split(':');
        int minutes = int.Parse(timeComponents[0]);
        int seconds = int.Parse(timeComponents[1]);
        int milliseconds = int.Parse(timeComponents[2]);
        string formattedTime = string.Format("{0}. {1:00}:{2:00}:{3:000}", lapNumber, minutes, seconds, milliseconds);

        return formattedTime;
    }

    public void ClearLapTimesText()
    {
        lapTimesText.text = "";
    }
}