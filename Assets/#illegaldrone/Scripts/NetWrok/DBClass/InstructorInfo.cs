
using System;

namespace KKH.MySQL
{
    public class InstructorInfo
    {
        public string userName;
        public Gender gender;
        public UserGroup userGroup;
        public string id;
        public int totalCount;
        public int missionResultTotalCount;
        public DateTime endAccessDate;

        public InstructorInfo() { }

        public InstructorInfo(string userName, Gender gender, UserGroup userGroup, string id, int totalCount, int missionResultTotalCount, DateTime endAccessDate)
        {
            this.userName = userName;
            this.gender = gender;
            this.userGroup = userGroup;
            this.id = id;
            this.totalCount = totalCount;
            this.missionResultTotalCount = missionResultTotalCount;
            this.endAccessDate = endAccessDate;
        }
    }
}