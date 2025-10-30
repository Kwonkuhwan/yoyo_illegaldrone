using System;

namespace KKH.MySQL
{
    [Serializable]
    public class LogMessage
    {
        public string instructorID;
        public string traineeID;
        public string logMsg;
        public DateTime logWriteDateTime;

        public LogMessage() { }

        public LogMessage(string instructorID, string traineeID, string logMsg, DateTime logWriteDateTime)
        {
            this.instructorID = instructorID;
            this.traineeID = traineeID;
            this.logMsg = logMsg;
            this.logWriteDateTime = logWriteDateTime;
        }
    }
}