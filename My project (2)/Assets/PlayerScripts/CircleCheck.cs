using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleCheck : MonoBehaviour
{
    public GameObject[] objects;
    private int currentObjectIndex;
    private bool gameInProgress;
   

    public GameObject loseWindow; // Ссылка на окно поражения

    private void Start()
    {
        gameInProgress = true;
        currentObjectIndex = 0;
        HideLastObject();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (!gameInProgress)
            return;

        GameObject collidedObject = collision.gameObject;

        if (IsInObjects(collidedObject))
        {
            if (collidedObject == objects[currentObjectIndex])
            {
                collidedObject.SetActive(false);
                currentObjectIndex++;

                if (currentObjectIndex >= objects.Length)
                {
                    currentObjectIndex = 0;
                    ResetObjects();
                    HideLastObject();
                    
                }
                else if (currentObjectIndex == objects.Length - 1)
                {
                    ShowLastObject();
                }
            }
            else
            {
                LoseGame(); // Вызов функции поражения
                gameInProgress = true;
                ResetCurrentObjectIndex();

            }
        }
        
    }

    private bool IsInObjects(GameObject obj)
    {
        foreach (GameObject gameObj in objects)
        {
            if (gameObj == obj)
            {
                return true;
            }
        }
        return false;
    }

    public void ResetCurrentObjectIndex()
    {
        currentObjectIndex = 0;
    }

    private void ResetObjects()
    {
        foreach (GameObject obj in objects)
        {
            obj.SetActive(true);
        }
    }

    private void HideLastObject()
    {
        objects[objects.Length - 1].SetActive(false);
    }

    private void ShowLastObject()
    {
        objects[objects.Length - 1].SetActive(true);
    }

    private void LoseGame()
    {
        Time.timeScale = 0f;
        gameInProgress = false;
        loseWindow.SetActive(true); // Показываем окно поражения
    }
 
}
