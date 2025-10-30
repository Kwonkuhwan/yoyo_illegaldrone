using System;
using UnityEngine;

namespace KKH.MySQL
{
    /// <summary>
    /// 새로운 시나리오 정보
    /// </summary>
    [Serializable]
    public class PlayDB
    {
        public string scenarioID;
        public string playID;
        public string userID;
        public string playScenarioDateTime;     // 플레이 시작 시간
        public string playTime;                 // 진행 시간
        public int userCount;
        public string scenarioPlayTime;         // 진행 제한 시간
        public string killCount;
        public int missionResult;               // 개인 미션 성공여부
        public int teamMissionResult;           // 팀미션 성공 여뷰
        public PlayDB() { }

        public PlayDB(string scenarioID, string playID, string userID, string playScenarioDateTime, string playTime, int userCount, string scenarioPlayTime, string killCount, int missionResult, int teamMissionResult)
        {
            this.scenarioID = scenarioID;
            this.playID = playID;
            this.userID = userID;
            this.playScenarioDateTime = playScenarioDateTime;
            this.playTime = playTime;
            this.userCount = userCount;
            this.scenarioPlayTime = scenarioPlayTime;
            this.killCount = killCount;
            this.missionResult = missionResult;
            this.teamMissionResult = teamMissionResult;
        }
    }
}