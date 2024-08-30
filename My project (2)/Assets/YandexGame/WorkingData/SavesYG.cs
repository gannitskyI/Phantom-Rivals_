
using static CarShop;
using static TaskChecker;
using static AudioManagers;
using static MoneyCounter;
using System.Collections.Generic;
using UnityEngine;

namespace YG
{
    [System.Serializable]
    public class SavesYG
    {
        // "Технические сохранения" для работы плагина (Не удалять)
        public int idSave;
        public bool isFirstSession = true;
        public string language = "ru";
        public bool promptDone;
         
        [Header("Scriptable Objects")]
        public List<CarDataSO> carDataList;  

        public ScoreData scoreData;
        public BestTimeData bestTimeData;
        public TaskCountData taskCountData;
        public ButtonTaskStatus buttonTaskStatus;
        public Reward reward;
        public AudioData audioData;
        public bool tutorialCompleted;
    }
}
