using System;
using UnityEngine;

namespace KKH.MySQL
{
    /// <summary>
    /// 새로운 시나리오 정보
    /// </summary>
    [Serializable]
    public class EnemyDroneInfo
    {
        public int idx;
        public string scenarioID;
        public DroneType droneType;
        public int flyingOrder;
        public FlyingType flyingType;
        public int groupNumber;
        public Vector3 startPoint;
        public float offensivePower;
        public float attackerloadtime;
        public float moveSpeed;

        public EnemyDroneInfo() { }

       public EnemyDroneInfo(int idx, string scenarioID, DroneType droneType, int flyingOrder, FlyingType flyingType, int groupNumber, float startPointX, float startPointY, float startPointZ, float offensivePower, float attackerloadtime , float moveSpeed) 
        { 
            this.idx = idx;
            this.scenarioID = scenarioID;
            this.droneType = droneType;
            this.flyingOrder = flyingOrder;
            this.flyingType = flyingType;
            this.groupNumber = groupNumber;
            this.startPoint = new Vector3(startPointX, startPointY, startPointZ);
            this.offensivePower = offensivePower;
            this.attackerloadtime = attackerloadtime;
            this.moveSpeed = moveSpeed;
        }

        public EnemyDroneInfo(int idx, string scenarioID, DroneType droneType, int flyingOrder, FlyingType flyingType, int groupNumber, Vector3 startPoint, float offensivePower, float attackerloadtime, float moveSpeed)
        {
            this.idx = idx;
            this.scenarioID = scenarioID;
            this.droneType = droneType;
            this.flyingOrder = flyingOrder;
            this.flyingType = flyingType;
            this.groupNumber = groupNumber;
            this.startPoint = startPoint;
            this.offensivePower = offensivePower;
            this.attackerloadtime = attackerloadtime;
            this.moveSpeed = moveSpeed;
        }
    }
}
