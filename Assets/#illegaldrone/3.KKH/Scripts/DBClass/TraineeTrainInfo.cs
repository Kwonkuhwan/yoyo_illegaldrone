using System;

namespace KKH.MySQL
{
    /// <summary>
    /// 훈련생 훈련 정보
    /// </summary>
    [Serializable]
    public class TraineeTrainInfo
    {
        public int idx;
        public DateTime playScenarioDateTime;
        public int missionMap;
        public int weather;
        public int timeZone;
        public int playMode;
        public DateTime playTime;
        public int limitPlaytime;
        public int missionResult;
        public int ranks;
        public bool isbtnActive;
        public string admin;

        public TraineeTrainInfo() { }

        public TraineeTrainInfo(int idx, DateTime playScenarioDateTime, int ranks, int missionMap, int weather, int timeZone, int playMode, DateTime playTime, int limitPlaytime, int missionResult, bool isbtnActive, string admin)
        {
            this.idx = idx;
            this.playScenarioDateTime = playScenarioDateTime;
            this.ranks = ranks;
            this.missionMap = missionMap;
            this.weather = weather;
            this.timeZone = timeZone;
            this.playMode = playMode;
            this.playTime = playTime;
            this.limitPlaytime = 
            this.missionResult = missionResult;
            this.isbtnActive = isbtnActive;
            this.admin = admin;
        }
    }
}