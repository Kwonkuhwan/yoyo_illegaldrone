using System;

namespace KKH.MySQL
{
    /// <summary>
    /// 유저 정보
    /// </summary>
    [Serializable]
    public class UserInfo
    {
        public UserGroup userGroup;
        public string id;
        public string pw;
        public string userName;
        public Gender gender;
        public DateTime createIDDate;
        public bool isDelete;
        public DateTime endAccessDate;
        public DateTime totalPlayTime;
        public DateTime modifyPW;
        public Tutorialcomplete totorialComplete;
        public int missionResult_count;
        public int teamMissionResult_count;
        public double result_Percentage;
        public string createAdminName;

        public UserInfo() { }

        public UserInfo(UserGroup userGroup, string id, string pw, string userName, Gender gender,
            DateTime createIDDate, bool isDelete, DateTime endAccessDate, DateTime totalPlayTime, DateTime modifyPW,
            Tutorialcomplete totorialComplete, int missionResult_count, int teamMissionResult_count, double result_Percentage, string createAdminName)
        {
            this.userGroup = userGroup;
            this.id = id;
            this.pw = pw;
            this.userName = userName;
            this.gender = gender;
            this.createIDDate = createIDDate;
            this.isDelete = isDelete;
            this.endAccessDate = endAccessDate;
            this.totalPlayTime = totalPlayTime;
            this.modifyPW = modifyPW;
            this.totorialComplete = totorialComplete;
            this.missionResult_count = missionResult_count;
            this.teamMissionResult_count = teamMissionResult_count;
            this.result_Percentage = result_Percentage;
            this.createAdminName = createAdminName;
        }

        public void Clear()
        {
            this.userGroup = UserGroup.Trainee;
            this.id = string.Empty;
            this.pw = string.Empty;
            this.userName = string.Empty;
            this.gender = Gender.Man;
            this.createIDDate = DateTime.MinValue;
            this.isDelete = false;
            this.endAccessDate = DateTime.MinValue;
            this.totalPlayTime = DateTime.MinValue;
            this.modifyPW = DateTime.MinValue;
            this.totorialComplete = Tutorialcomplete.NotCompletion;
            this.missionResult_count = 0;
            this.teamMissionResult_count = 0;
            this.result_Percentage = 0;
            this.createAdminName = string.Empty;
        }
    }
}