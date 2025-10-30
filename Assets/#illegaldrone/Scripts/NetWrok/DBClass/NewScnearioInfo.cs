using System;
using UnityEngine;

namespace KKH.MySQL
{
    /// <summary>
    /// 새로운 시나리오 정보
    /// </summary>
    [Serializable]
    public class NewScnearioInfo
    {
        public string scenarioID;
        public PlayMode playMode;
        public int playingNumber;
        public MapType missionMap;
        public Weather weather;
        public TimeZone timeZone;
        public int defenseAreaType;
        public int defenseArea;
        public float defensObjectHP;
        public Vector3 defenseobjectlocationPoint;
        public Vector3 firstPlayerLocationPoint;
        public Vector3 secondPlayerLocationPoint;
        public Vector3 thirdPlayerLocationPoint;
        public Vector3 fourthPlayerLocationPoint;
        public int playTime;
        public int limitPlayTime;
        public string createUser;
        public string endScenarioPlayID;

        public NewScnearioInfo() { }

        public NewScnearioInfo(string scenarioID, PlayMode playMode, int playingNumber, MapType missionMap, Weather weather, TimeZone timeZone, int defenseAreaType, int defenseArea, float defensObjectHP, 
            float defenseobjectlocationpointX, float defenseobjectlocationpointY, float defenseobjectlocationpointZ, 
            float firstPlayerLocationPointX, float firstPlayerLocationPointY, float firstPlayerLocationPointZ, 
            float secondPlayerLocationPointX, float secondPlayerLocationPointY, float secondPlayerLocationPointZ, 
            float thirdPlayerLocationPointX, float thirdPlayerLocationPointY, float thirdPlayerLocationPointZ, 
            float fourthPlayerLocationPointX, float fourthPlayerLocationPointY, float fourthPlayerLocationPointZ, 
            int playTime, int limitPlayTime, string createUser, string endScenarioPlayID)
        {
            this.scenarioID = scenarioID;
            this.playMode = playMode;
            this.playingNumber = playingNumber;
            this.missionMap = missionMap;
            this.weather = weather;
            this.timeZone = timeZone;
            this.defenseAreaType = defenseAreaType;
            this.defenseArea = defenseArea;
            this.defensObjectHP = defensObjectHP;
            this.defenseobjectlocationPoint = new Vector3(defenseobjectlocationpointX, defenseobjectlocationpointY, defenseobjectlocationpointZ);
            this.firstPlayerLocationPoint = new Vector3(firstPlayerLocationPointX, firstPlayerLocationPointY, firstPlayerLocationPointZ);
            this.secondPlayerLocationPoint = new Vector3(secondPlayerLocationPointX, secondPlayerLocationPointY, secondPlayerLocationPointZ);
            this.thirdPlayerLocationPoint = new Vector3(thirdPlayerLocationPointX, thirdPlayerLocationPointY, thirdPlayerLocationPointZ);
            this.fourthPlayerLocationPoint = new Vector3(fourthPlayerLocationPointX, fourthPlayerLocationPointY, fourthPlayerLocationPointZ);
            this.playTime = playTime;
            this.limitPlayTime = limitPlayTime;
            this.createUser = createUser;
            this.endScenarioPlayID = endScenarioPlayID;
        }

        public NewScnearioInfo(string scenarioID, PlayMode playMode, int playingNumber, MapType missionMap, Weather weather, TimeZone timeZone, int defenseAreaType, int defenseArea, float defensObjectHP, Vector3 defenseobjectlocationPoint, Vector3 firstPlayerLocationPoint, Vector3 secondPlayerLocationPoint, Vector3 thirdPlayerLocationPoint, Vector3 fourthPlayerLocationPoint, int playTime, int limitPlayTime, string createUser, string endScenarioPlayID)
        {
            this.scenarioID = scenarioID;
            this.playMode = playMode;
            this.playingNumber = playingNumber;
            this.missionMap = missionMap;
            this.weather = weather;
            this.timeZone = timeZone;
            this.defenseAreaType = defenseAreaType;
            this.defenseArea = defenseArea;
            this.defensObjectHP = defensObjectHP;
            this.defenseobjectlocationPoint = defenseobjectlocationPoint;
            this.firstPlayerLocationPoint = firstPlayerLocationPoint;
            this.secondPlayerLocationPoint = secondPlayerLocationPoint;
            this.thirdPlayerLocationPoint = thirdPlayerLocationPoint;
            this.fourthPlayerLocationPoint = fourthPlayerLocationPoint;
            this.playTime = playTime;
            this.limitPlayTime = limitPlayTime;
            this.createUser = createUser;
            this.endScenarioPlayID = endScenarioPlayID;
        }
    }
}