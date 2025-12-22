using System;

namespace KKH.MySQL
{
    [Serializable]
    public class TraineeInfo
    {
        public int idx;
        public DateTime createIDDate;
        public string id;
        public string userName;
        public int scenarioCount;
        public DateTime latestPlayDate;
        public int playMode;
        public int playTime;
        public int limitPlayTime;
        public int missionresult;
        public bool btnActive;
        public string createAdminName;

        public TraineeInfo() { }

        public TraineeInfo(int idx, DateTime createIDDate, string id, string userName, int scenarioCount, DateTime latestPlayDate, int playMode, int playTime, int limitPlayTime, int missionresult, bool btnActive, string createAdminName)
        {
            this.idx = idx;
            this.createIDDate = createIDDate;
            this.id = id;
            this.userName = userName;
            this.scenarioCount = scenarioCount;
            this.latestPlayDate = latestPlayDate;
            this.playMode = playMode;
            this.playTime = playTime;
            this.limitPlayTime = limitPlayTime;
            this.missionresult = missionresult;
            this.btnActive = btnActive;
            this.createAdminName = createAdminName;
        }
    }
}