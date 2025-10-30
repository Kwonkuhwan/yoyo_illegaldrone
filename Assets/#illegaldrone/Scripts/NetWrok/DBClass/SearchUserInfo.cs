using System;

namespace KKH.MySQL
{
    /// <summary>
    /// 검색 유저 정보
    /// </summary>
    [Serializable]
    public class SearchUserInfo
    {
        public int idx;
        public DateTime createIDDate;
        public string id;
        public string userName;
        public int scenarioCount;
        public DateTime playSceanrioDateTime;
        public string createAdminName;

        public SearchUserInfo() { }

        public SearchUserInfo(int idx, DateTime createIDDate, string id, string userName, int scenarioCount, DateTime playSceanrioDateTime, string createAdminName)
        {
            this.idx = idx;
            this.createIDDate = createIDDate;
            this.id = id;
            this.userName = userName;
            this.scenarioCount = scenarioCount;
            this.playSceanrioDateTime = playSceanrioDateTime;
            this.createAdminName = createAdminName;
        }
    }
}