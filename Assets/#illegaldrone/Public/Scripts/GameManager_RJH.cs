using KKH.MySQL;
using RJH;
using SMW;
using UnityEngine;
using UnityEngine.Events;

namespace Illegaldrone
{
    public enum DISCONNECTEDREASON
    {
        NONE,
        LOGOUT,
        DUPLICATELOGIN,
        PASSWORDCHANGE,
    }

    public partial class GameManager : MonoBehaviour
    {
        public DISCONNECTEDREASON reason = DISCONNECTEDREASON.NONE;
        [HideInInspector] public NewScnearioInfo scenario = new NewScnearioInfo(); // 임시
        public UnityEvent<LOGMESSAGE, int, int> logMessageCallBack = new UnityEvent<LOGMESSAGE, int, int>();

        //[HideInInspector] public int traineeNumber = -1;
        //[HideInInspector] public int droneType = -1;

        public void PauseOrStart()
        {
            if (Time.timeScale != 0)
                Time.timeScale = 0f;
            else
                Time.timeScale = 1f;
        }
        /// <summary>
        /// 랜드마크가 공격 받았을 경우 (MESSAGE.ATTACK)
        /// 드론이 구역을 침범했을 경우 (MESSAGE.ARRIVE, 드론 번호, 방어 구역 번호)
        /// 드론을 무력화했을 경우 (MESSAGE.INACTIVE, 드론 번호)
        /// 훈련생이 드론을 격추했을 경우 (MESSAGE.SHOOTING, 훈련생 번호(0번 부터), 드론 번호)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        public void InstructorViewEvent(LOGMESSAGE message, int index1, int index2)
        {
            logMessageCallBack?.Invoke(message, index1, index2);
        }

        public void DroneAttack()
        {
            Debug.Log("드론 공격");
            logMessageCallBack?.Invoke(LOGMESSAGE.ATTACK, -1, -1);
            //GamePlay.Instance.isDroneAttackObject = true;
        }

        public void DroneArrive(int droneType, int defenseArea)
        {
            logMessageCallBack?.Invoke(LOGMESSAGE.ATTACK, droneType, defenseArea);
        }

        public void DroneInactive(int trainee, int droneType)
        {
            Debug.Log("드론 무력화");
            logMessageCallBack?.Invoke(LOGMESSAGE.INACTIVE, trainee, droneType);
            //GamePlay.Instance.KillCountToPlayer(trainee);
        }

        //public void DroneInactive()
        //{
        //    if(traineeNumber != -1 && droneType != -1)
        //    {
        //        logMessageCallBack?.Invoke(LOGMESSAGE.INACTIVE, traineeNumber, droneType);
        //        traineeNumber = -1;
        //        droneType = -1;
        //    }
        //}

        public void TraineeShoot(int trainee, int droneType)
        {
            Debug.Log("드론 파괴");
            logMessageCallBack?.Invoke(LOGMESSAGE.SHOOTING, trainee, droneType);
            //GamePlay.Instance.KillCountToPlayer(trainee);
        }
    }
}


