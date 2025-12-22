using Photon.Pun;
using RJH;
using RJH.UI;
using UnityEngine;

namespace KKH.Photon
{
    public class PhotonFunction : MonoBehaviourPun
    {
        private static PhotonFunction inst;
        public static PhotonFunction Inst => inst;

        private PhotonView pv;
        public PhotonView PV => pv;

        private void Awake()
        {
            //[2025-04-09][RJH] PhotonManager가 PhotonView가 달려있을때 룸을 나가면 파괴되어버려서 PhotonManager와 분리
            // 분리된 PhotonFunction DontDestroyOnLoad 설정
            // 이 스크립트를 사용하지 않는다면 빼는게 좋을 것 같습니다.
            if(inst != null)
            {
                Destroy(gameObject);
                return;
            }

            if (pv == null)
            {
                inst = this;
                pv = GetComponent<PhotonView>();
                if (pv == null)
                {
                    pv = gameObject.AddComponent<PhotonView>();
                }
                DontDestroyOnLoad(this.gameObject);
            }

            //if (pv == null)
            //{
            //    inst = this;
            //    pv = GetComponent<PhotonView>();
            //    if (pv == null)
            //    {
            //        pv = gameObject.AddComponent<PhotonView>();
            //    }
            //}
        }

        public void RPC(string funcName, RpcTarget target, params object[] parameters)
        {
            UTILS.Log($"{funcName} RPC 실행예정");
            pv.RPC(funcName, target, parameters);
        }

        /// <summary>
        /// 훈련생만 사용하는 함수(관리자가 임무 전달 하는 용도)
        /// </summary>
        [PunRPC]
        public void GetScenarioInfo()
        {
            Home_Trainee_VR.Inst.SetScenarioUI(true);
        }

        // 서버에서 받은 데이터 처리
        [PunRPC]
        //public void SetTrainingMap(int clientId, int missionMap, int weather, int timeZone)
        //{
        //    //scenario = GameManager.instance.scenario;
        //    if (clientId == PhotonNetwork.LocalPlayer.ActorNumber)
        //    {
        //        // 미션 지역 세팅, 날씨 세팅, 시간대 세팅
        //        InstructorTrainViewController.Inst.mapObjController.SetMapOption(missionMap, weather, timeZone);
        //        InstructorTrainViewController.Inst.mapObjController.SetMainTarget();

        //        InstructorTrainViewController.Inst.mapObjController.SetDroneOn();
        //    }
        //}
        public void SetTrainingMap(/*int clientId, */MapData data)
        {
            //scenario = GameManager.instance.scenario;
            //if (clientId == PhotonNetwork.LocalPlayer.ActorNumber)
            //{
            UTILS.Log("RPC SetTrainingMap 동작");

            if (pv.IsMine)
            {
                // 미션 지역 세팅, 날씨 세팅, 시간대 세팅
                //InstructorTrainViewController.Inst.mapObjController.SetMapOption(data.missionMap, data.weather, data.timeZone);
                //InstructorTrainViewController.Inst.mapObjController.SetOnMap();
                //InstructorTrainViewController.Inst.mapObjController.SetMainTarget();
            }
            //InstructorTrainViewController.Inst.mapObjController.SetDroneOn();
            //}
        }

        [PunRPC]
        public void tSetTrainingMap(int clientId)
        {
            UTILS.Log("RPC tSetTrainingMap 동작");
        }
    }
}
