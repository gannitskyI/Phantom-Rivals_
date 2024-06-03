
using static CarShop;
using static TaskChecker;
using static AudioManagers;
using static MoneyCounter;
using System.Collections.Generic;

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
 

        // Вы можете выполнить какие то действия при загрузке сохранений
        
        public List<CarData> carDataList = new List<CarData>();

        public ScoreData scoreData;
        public BestTimeData bestTimeData;
        public TaskCountData taskCountData;
        public ButtonTaskStatus buttonTaskStatus;
        public Reward reward;
        public AudioData audioData;
        public bool tutorialCompleted;
    }
}
