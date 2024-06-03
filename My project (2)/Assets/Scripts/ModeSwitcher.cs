using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;

public enum GameMode
{
    Mode1,
    Mode2
}

[System.Serializable]
public class GameModeData
{
    public GameMode currentMode;
    public bool isTask;
}

public class ModeSwitcher : MonoBehaviour
{
    public List<TextMeshProUGUI> gameModes;
    public float duration = 0.3f;
    public GameMode currentMode = GameMode.Mode1;

    private Vector2 touchStartPos;
    private const float SwipeAnimationDistance = 100f;
    private bool isAnimating = false;
    private bool isTask = false;

    private void Start()
    {
        DOTween.Init();
        InitializeGameModes();
        ClearGameModeData();
    }

    private void InitializeGameModes()
    {
        for (int i = 1; i < gameModes.Count; i++)
        {
            SetTextMeshProAlpha(gameModes[i], 0);
        }
    }

    private void SetTextMeshProAlpha(TextMeshProUGUI textMeshPro, float alpha)
    {
        Color color = textMeshPro.color;
        color.a = alpha;
        textMeshPro.color = color;
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveGameModeData();
        }
    }
    public void ResetToDefaults()
{
     
    isTask = false;

    
    // Удаление файла сохраненных данных, если он существует
    string filePath = Path.Combine(Application.persistentDataPath, "GameModeData.json");
    if (File.Exists(filePath))
    {
        File.Delete(filePath);
    }
     
}

    public void SaveGameModeData()
    {
        GameModeData data = new GameModeData
        {
            currentMode = currentMode,
            isTask = isTask
        };

        string jsonData = JsonUtility.ToJson(data);

        string filePath = Path.Combine(Application.persistentDataPath, "GameModeData.json");
        try
        {
            File.WriteAllText(filePath, jsonData);
        }
        catch (IOException e)
        {
            Debug.LogError("Failed to save game data: " + e.Message);
        }
    }

    public void ClearGameModeData()
    {
        // Очищаем данные GameModeData
        string filePath = Path.Combine(Application.persistentDataPath, "GameModeData.json");
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    private void Update()
    {
        if (!isAnimating && (Input.GetKeyDown(KeyCode.UpArrow) || SwipeDirectionDetected(false)))
        {
            SwipeUp();
        }
        else if (!isAnimating && (Input.GetKeyDown(KeyCode.DownArrow) || SwipeDirectionDetected(true)))
        {
            SwipeDown();
        }
    }


    private bool SwipeDirectionDetected(bool isSwipeUp)
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                float swipeDistance = touch.position.y - touchStartPos.y;
                return isSwipeUp ? swipeDistance > 0 : swipeDistance < 0;
            }
        }
        return false;
    }

    private void AnimateGameModeSwitch(int targetIndex, float swipeDirection)
    {
        if (isAnimating)
        {
            return;
        }

        isAnimating = true;

        TextMeshProUGUI currentGameMode = gameModes[(int)currentMode];

        Sequence sequence = DOTween.Sequence();
        sequence.Append(currentGameMode.transform.DOLocalMoveY(-swipeDirection, duration).SetEase(Ease.InOutQuad));
        sequence.Join(currentGameMode.DOFade(0, duration));

        sequence.AppendCallback(() =>
        {
            currentMode = (GameMode)targetIndex;
            TextMeshProUGUI nextGameMode = gameModes[(int)currentMode];

            nextGameMode.transform.localPosition = new Vector3(nextGameMode.transform.localPosition.x, swipeDirection, 0);
            nextGameMode.transform.DOLocalMoveY(0, duration).SetEase(Ease.InOutQuad);
            nextGameMode.DOFade(1, duration).OnComplete(() => isAnimating = false);

            if (PlayerCountManager.Instance.playerCount < 2)
            {
                if (currentMode == GameMode.Mode2)
                {
                    FindObjectOfType<ToggleFunction>().ToggleCheckbox(true);
                }
                else
                {
                    FindObjectOfType<ToggleFunction>().ToggleCheckbox(false);
                }
            }
        });
    }

    public void SwipeUp()
    {
        if ((int)currentMode > 0)
        {
            AnimateGameModeSwitch((int)currentMode - 1, SwipeAnimationDistance);
        }
    }

    public void SwipeDown()
    {
        if ((int)currentMode < gameModes.Count - 1)
        {
            AnimateGameModeSwitch((int)currentMode + 1, -SwipeAnimationDistance);
        }
    }

    public void TaskChange()
    {
        isTask = !isTask;
        SaveGameModeData();
    }
}
