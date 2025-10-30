using System;

namespace KKH.MySQL
{
    /// <summary>
    /// 훈련 정보
    /// </summary>
    [Serializable]
    public class TrainingInfo
    {
        public DateTime playScenarioDateTime;
        public int missionMap;
        public int weather;
        public int timeZone;
        public int playMode;
        public DateTime playTime;
        public int teamMissionResult;
        public bool btnActive;                      // 사후강평 버튼
        public string createUser;

        public TrainingInfo() { }

        public TrainingInfo(DateTime playScenarioDateTime, int missionMap, int weather, int timeZone, int playMode, DateTime playTime, int teamMissionResult, bool btnActive, string createUser)
        {
            this.playScenarioDateTime = playScenarioDateTime;
            this.missionMap = missionMap;
            this.weather = weather;
            this.timeZone = timeZone;
            this.playMode = playMode;
            this.playTime = playTime;
            this.teamMissionResult = teamMissionResult;
            this.btnActive = btnActive;
            this.createUser = createUser;
        }
    }
}