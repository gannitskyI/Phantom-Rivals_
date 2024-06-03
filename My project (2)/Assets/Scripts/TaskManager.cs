using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    [SerializeField]
    private GameObject taskList;
    private MainMenu mainMenu;

    public void Start()
    {
        mainMenu = FindObjectOfType<MainMenu>();
    }
    public void ShowTaskList()
    {
        mainMenu.PlayButtonSound();
        taskList.SetActive(true);

    }
    public void CloseTaskList()
    {
        mainMenu.PlayButtonSound();
        taskList.SetActive(false);

    }
}
