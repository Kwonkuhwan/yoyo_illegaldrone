using System;

namespace KKH.MySQL
{
    /// <summary>
    /// 캘린더 훈련 정보
    /// </summary>
    [Serializable]
    public class ClaendarTrainInfo
    {
        public int missionResult;
        public DateTime playScenarioDateTime;
        public int playMode;
        public int missionMap;
        public int playerCnt;

        public ClaendarTrainInfo() { }

        public ClaendarTrainInfo(int missionResult, DateTime playScenarioDateTime, int playMode, int missionMap, int playerCnt)
        {
            this.missionResult = missionResult;
            this.playScenarioDateTime = playScenarioDateTime;
            this.playMode = playMode;
            this.missionMap = missionMap;
            this.playerCnt = playerCnt;
        }
    }
}