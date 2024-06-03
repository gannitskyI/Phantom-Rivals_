using System;
using System.Collections;
using UnityEngine;
using System.IO;
using DG.Tweening;
using YG;

public class TaskChecker : MonoBehaviour
{
    public static TaskChecker Instance;

    private string filePath;

    [SerializeField] private bool[] taskStatus = new bool[6];
    [SerializeField] private int[] taskCompletionCount = new int[6];
    [SerializeField] private int[] taskCompletionGoal = new int[6]; // New array to hold completion goals
    [SerializeField] private GameObject[] banners;
    [SerializeField] private float bannerAnimationDuration = 1.5f;
    private TaskCountData taskCountData;

    private Vector3[] initialBannerPositions;
    private PlayerControl playerControl;
    private CanvasFunction canvasFunction;
    private CarAIHandler[] carAIHandlers;
    private ColiderCheck coliderCheck;
    private ObstacleCheck obstacleCheck;
    private bool isGameOngoing = true;
    private EndCheck endCheck;
    private static bool OneTaskCalled { get; set; } = false;

    private void Awake()
    {
        LoadTaskCountData();

        coliderCheck = FindObjectOfType<ColiderCheck>();
        StartCoroutine(InitializeAfterDelay());
    }

    private IEnumerator InitializeAfterDelay()
    {
        yield return new WaitForSeconds(1);
        Initialize();
    }

    private void Initialize()
    {
        carAIHandlers = FindObjectsOfType<CarAIHandler>();
        playerControl = FindObjectOfType<PlayerControl>();
        obstacleCheck = FindObjectOfType<ObstacleCheck>();
        canvasFunction = FindObjectOfType<CanvasFunction>();

        if (coliderCheck == null)
        {
            Debug.LogError("ColiderCheck не найден");
            return;
        }

        InitializeBanners();

        // Заполнение массива TaskCompletionCounts из сохраненных данных
        for (int i = 0; i < taskCompletionCount.Length; i++)
        {
            taskCompletionCount[i] = taskCountData.GetTaskCount(i + 1);
        }
    }

    private void InitializeBanners()
    {
        initialBannerPositions = new Vector3[banners.Length];

        for (int i = 0; i < banners.Length; i++)
        {
            if (banners[i] != null)
            {
                initialBannerPositions[i] = banners[i].transform.position;
                banners[i].transform.position = new Vector3(initialBannerPositions[i].x, initialBannerPositions[i].y - Screen.height, initialBannerPositions[i].z);
            }
        }
    }

    private void CheckTaskCompletion(bool taskCompleted, int taskNumber)
    {
        if (taskCompleted)
        {
            taskCountData.UpdateTaskCount(taskNumber); // Update task count
            Debug.LogWarning($"Задача выполнена {taskNumber}, {taskCountData.GetTaskCount(taskNumber)} раз");
            StartCoroutine(AnimateBanner(taskNumber - 1));
        }
        else
        {
            Debug.LogWarning($"Задаче не выполнена {taskNumber}");
        }
    }

    private IEnumerator AnimateBanner(int bannerIndex)
    {
        GameObject banner = banners[bannerIndex];
        if (banner != null && isGameOngoing)
        {
            banner.transform.DOMoveY(initialBannerPositions[bannerIndex].y, bannerAnimationDuration).SetEase(Ease.Linear);
            banner.SetActive(true);
            yield return new WaitForSeconds(3);
            banner.transform.DOMoveY(initialBannerPositions[bannerIndex].y - Screen.height, bannerAnimationDuration).SetEase(Ease.Linear);
            banner.SetActive(false);
        }
    }

    public void OneTask()
    {
        // Проверяем, выполнена ли уже задача больше, чем taskCompletionGoal для этой задачи
        if (taskCompletionCount[0] >= taskCompletionGoal[0])
        {
            Debug.LogWarning("Задача 1 уже выполнена нужное количество раз");
            return;
        }

        // **Изменение:** Проверяем, **все ли** AI-кары **не** закончили гонку
        bool allCarsNotFinished = Array.TrueForAll(carAIHandlers, carAIHandler => carAIHandler != null && !carAIHandler.raceFinished);

        CheckTaskCompletion(allCarsNotFinished, 1);
        taskStatus[0] = allCarsNotFinished;
    }

    public void TwoTask()
    {
        // Проверяем, выполнена ли уже задача больше, чем taskCompletionGoal для этой задачи
        if (taskCompletionCount[1] >= taskCompletionGoal[1])
        {
            Debug.LogWarning("Задача 2 уже выполнена нужное количество раз");
            return;
        }

        if (canvasFunction != null && canvasFunction.timerText != null) // Using timerText from CanvasFunction
        {
            if (TimeSpan.TryParseExact(canvasFunction.timerText.text, "mm':'ss':'fff", null, out TimeSpan lapTimeSpan))
            {
                bool taskCompleted = lapTimeSpan.TotalSeconds <= 120;
                taskStatus[1] = taskCompleted;
                CheckTaskCompletion(taskCompleted, 2);
            }
            else
            {
                Debug.LogError("Ошибка при парсинге времени");
            }
        }
        else
        {
            Debug.LogError("timerText не найден в CanvasFunction");
        }
    }

    public void ThreeTask()
    {
        // Проверяем, выполнена ли уже задача больше, чем taskCompletionGoal для этой задачи
        if (taskCompletionCount[2] >= taskCompletionGoal[2])
        {
            Debug.LogWarning("Задача 3 уже выполнена нужное количество раз");
            return;
        }

        bool allHandlersNotFinished = Array.TrueForAll(carAIHandlers, carAIHandler => carAIHandler != null && !carAIHandler.raceFinished);
        bool taskCompleted = !coliderCheck.playerTouching && allHandlersNotFinished;

        taskStatus[2] = taskCompleted;
        CheckTaskCompletion(taskCompleted, 3);
    }

    public void FourTask()
    {
        // Проверяем, выполнена ли уже задача больше, чем taskCompletionGoal для этой задачи
        if (taskCompletionCount[3] >= taskCompletionGoal[3])
        {
            Debug.LogWarning("Задача 4 уже выполнена нужное количество раз");
            return;
        }

        bool taskCompleted = !ObstacleCheck.obstacleTouching;

        taskStatus[3] = taskCompleted;
        CheckTaskCompletion(taskCompleted, 4);
    }

    public void FiveTask()
    {
        StartCoroutine(CheckSpeedForOneMinute());
    }

    private IEnumerator CheckSpeedForOneMinute()
    {
        float startTime = Time.time;
        float elapsedTime = 0f;

        PlayerControl playerControl = FindObjectOfType<PlayerControl>();

        while (elapsedTime < 60f)
        {
            elapsedTime = Time.time - startTime;

            // Проверяем скорость игрока в каждой итерации цикла
            if (playerControl != null && playerControl.currentSpeed < 20f)
            {
                // Если скорость меньше 20, завершаем выполнение метода
                taskStatus[4] = false;
                CheckTaskCompletion(false, 5);
                yield break;
            }

            yield return null;
        }

        // Если скорость была больше 20 на протяжении минуты, выполняем соответствующую проверку задачи
        if (playerControl != null && playerControl.currentSpeed >= 20f)
        {
            taskStatus[4] = true;
            CheckTaskCompletion(true, 5);
        }
    }



    public void SixTask()
    {
        // Проверяем, выполнена ли уже задача больше, чем taskCompletionGoal для этой задачи
        if (taskCompletionCount[5] >= taskCompletionGoal[5])
        {
            Debug.LogWarning("Задача 6 уже выполнена нужное количество раз");
            return;
        }

        if (ObstacleCheck.obstacleTouching || sixTaskCheck.playerTouching)
        {
            CheckTaskCompletion(false, 6);
            return;
        }

        bool taskCompleted = !ObstacleCheck.obstacleTouching;

        taskStatus[5] = taskCompleted;
        CheckTaskCompletion(taskCompleted, 6);
    }

    public void EndGame()
    {
        isGameOngoing = false;
    }

    public void CheckTask()
    {
       
        OneTask();
      
        TwoTask();
      
        ThreeTask();
        
        FourTask();
         
        SixTask();
        
    }
    public void OpenBaner()
    
    { 
        endCheck = FindObjectOfType<EndCheck>();

        EndGame();
        
        endCheck.OpenBanners(1, taskStatus[0], taskCompletionCount[0]);

        
        endCheck.OpenBanners(2, taskStatus[1], taskCompletionCount[1]);
        
        endCheck.OpenBanners(3, taskStatus[2], taskCompletionCount[2]);

        
        endCheck.OpenBanners(4, taskStatus[3], taskCompletionCount[3]);

        endCheck.OpenBanners(5, taskStatus[4], taskCompletionCount[4]);

        
        endCheck.OpenBanners(6, taskStatus[5], taskCompletionCount[5]);

        SaveTaskCountData();
    }

    private void LoadTaskCountData()
    {
        // Загружаем данные из YandexGame.savesData.taskCountData
        TaskCountData taskCountData = YandexGame.savesData.taskCountData;

        // Если данные существуют, обновляем счетчик задач
        if (taskCountData != null)
        {
            this.taskCountData = taskCountData;
        }
        else
        {
            this.taskCountData = new TaskCountData();
        }
    }

    public void SaveTaskCountData()
    {
        // Создаем новый экземпляр TaskCountData и сохраняем текущий счетчик задач
        TaskCountData taskCountData = new TaskCountData { TaskCompletionCounts = this.taskCountData.TaskCompletionCounts };

        // Сохраняем данные в YandexGame.savesData.taskCountData
        YandexGame.savesData.taskCountData = taskCountData;

        // Теперь остаётся сохранить данные
        YandexGame.SaveProgress();
    }

    [Serializable]
    public class TaskCountData
    {
        public int[] TaskCompletionCounts = new int[6];

        public void UpdateTaskCount(int taskNumber)
        {
            TaskCompletionCounts[taskNumber - 1]++;
        }

        public int GetTaskCount(int taskNumber)
        {
            return TaskCompletionCounts[taskNumber - 1];
        }
    }
}