using Illegaldrone;
using KKH;
using KKH.MySQL;
using Photon.Pun;
using RJH;
using RJH.UI;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace SMW
{
    public class GamePlay : MonoBehaviour, IPunObservable
    {
        public static GamePlay Instance { get; private set; }

        PhotonView pv;

        private float playtime_second = 0f;
        private float playtime_miniute = 0f;
        private float playtime_hour = 0f;
        public bool isStart = false;

        public UnityEvent<float, float> timerCallBack = new UnityEvent<float, float>();
        public UnityEvent killCountCallBack = new UnityEvent();
        public DateTime StartTime => startTime;
        private DateTime startTime;



        private void Awake()
        {
            Instance = this;

            pv = GetComponent<PhotonView>();
        }

        void Update()
        {
            // 플레이 시작
            if (isStart)
            {
                playtime_second += Time.deltaTime;

                if (playtime_second >= 60)
                {
                    playtime_second = 0;
                    playtime_miniute += 1;
                }

                if (playtime_miniute >= 60)
                {
                    playtime_miniute = 0;
                    playtime_hour += 1;
                }
                // 2025-04-24 타이머가 설정한 훈련시간이 되면 훈련 종료

            }
        }

        /// <summary>
        /// 훈련생UI 타이머 동기화
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="info"></param>
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(playtime_miniute);
                stream.SendNext(playtime_second);
            }
            else
            {
                float old_minute = playtime_miniute;
                float old_secound = playtime_second;

                playtime_miniute = (float)stream.ReceiveNext();
                playtime_second = (float)stream.ReceiveNext();

                if (playtime_miniute != old_minute || playtime_second != old_secound)
                {
                    timerCallBack?.Invoke(playtime_miniute, playtime_second);
                }
            }
        }

        /// <summary>
        /// 게임 종료
        /// </summary>
        public void End()
        {
            if (isStart == false) return;
            isStart = false;

            player = new PlayDB();
            player.scenarioID = GameManager.instance.scenario.scenarioID;
            player.playID = GameManager.instance.scenario.scenarioID;
            player.userID = GameManager.instance.userInfo.id;
            player.playScenarioDateTime = StartTime.ToString("yyyy-MM-dd HH:mm:ss");
            player.playTime = $"{playtime_hour.ToString("00")}:{playtime_miniute.ToString("00")}:{playtime_second.ToString("00")}";
            player.userCount = PhotonNetwork.CurrentRoom.PlayerCount;
            player.scenarioPlayTime = $"00:{GameManager.instance.scenario.limitPlayTime.ToString("00"):00}";
            player.killCount = "0";
            player.missionResult = 0;
            if (isDroneAllDestroyed && !isDroneAttackObject) // 드론이 전부 파괴되었고, 드론이 공격을 성공한적 없으면 1
                player.teamMissionResult = 1;   // 팀 미션 목표 1 = 드론전부다 파괴 아니면 0
            else                                            // 아니라면 0
                player.teamMissionResult = 0;
            string json = JsonUtility.ToJson(player);
            pv.RPC("PlayerToDB", RpcTarget.Others, json);

            // 녹화종료
            ReplayManager.Instance.StopRecording();

            // 교관도 DB추가
            try
            {
                if (MySQLManager.SetPlayDB(player))
                {
                    UTILS.Log($"교관 DB 추가 완료 : {player.ToString()}");
                }
                else
                {
                    UTILS.LogError("교관 DB 추가 실패");
                }
            }
            catch (Exception e)
            {
                UTILS.LogError("교관 DB 추가 실패 : " + e.Message);
            }
        }

        /// <summary>
        /// 게임 시작
        /// </summary>
        public void StartGame()
        {
            isStart = true;
            startTime = DateTime.Now;
            ReplayManager.Instance.StartRecording();        // 녹화시작
        }

        public void KillCountToPlayer(int traineeNumber, int droneType, int logMessage)
        {
            if (GameManager.instance.isInstructor)
            {
                pv.RPC("RPC_KillCountToPlayer", RpcTarget.Others, traineeNumber, droneType, logMessage);
            }
        }

        [PunRPC]
        private void RPC_Timer(float minute, float secound)
        {
            timerCallBack?.Invoke(playtime_miniute, playtime_second);
        }

        [PunRPC]
        private void RPC_KillCountToPlayer(int traineeNumber, int droneType, int logMessage)
        {
            if (logMessage == (int)LOGMESSAGE.INACTIVE)
                GameManager.instance.DroneInactive(traineeNumber, droneType);
            else if (logMessage == (int)LOGMESSAGE.SHOOTING)
                GameManager.instance.TraineeShoot(traineeNumber, droneType);

                if ((int)PhotonNetwork.LocalPlayer.CustomProperties["TraineeNumber"] != traineeNumber)
                return;
            killCountCallBack?.Invoke();
            killCount++;
            missionResult = 1;
        }

        public PlayDB player;

        // 로컬 훈련생 전용 변수
        private int killCount = 0;
        private int missionResult = 0;

        public bool isDroneAllDestroyed = false;
        public bool isDroneAttackObject = false;

        public Canvas_Result resultCanvas;

        /// <summary>
        /// SMW 추가 / 플레이어 DB 추가
        /// </summary>
        [PunRPC]
        void PlayerToDB(string data)
        {
            player = JsonUtility.FromJson<PlayDB>(data);
            player.userID = GameManager.instance.userInfo.id;


            player.killCount = killCount.ToString();
            player.missionResult = missionResult;       // 훈련생 개인 미션 목표 1 = 1킬이라도 할시 아니면 0

            // 2025-04-21 RJH 결과 화면 띄우고, 팀 미션 결과 실행
            resultCanvas.gameObject.SetActive(true);
            resultCanvas.SetTitle(player.teamMissionResult == 1);
            //

            // 2025-04-21 RJH 훈련생 훈련 정보를 RPC로 각 로컬에 전송
            if (!GameManager.instance.isInstructor)
            {
                pv.RPC("PlayerToResult", RpcTarget.All, GameManager.instance.userInfo.userName, (int)PhotonNetwork.LocalPlayer.CustomProperties["TraineeNumber"], InstructorScenarioPage.Instance.GetTraineeDefenceArea((int)PhotonNetwork.LocalPlayer.CustomProperties["TraineeNumber"]), killCount);
                //PlayerToResult(GameManager.instance.userInfo.userName, InstructorScenarioPage.Instance.GetTraineeDefenceArea((int)PhotonNetwork.LocalPlayer.CustomProperties["TraineeNumber"]), killCount);
            }

            try
            {
                if (MySQLManager.SetPlayDB(player))
                {
                    UTILS.Log($"플레이어 DB 추가 완료 : {player.ToString()}");
                }
                else
                {
                    UTILS.LogError("플레이어 DB 추가 실패");
                }
            }
            catch (Exception e)
            {
                UTILS.LogError("플레이어 DB 추가 실패 : " + e.Message);
            }
        }

        [PunRPC]
        private void PlayerToResult(string name,int traineeNumber, string area, int kill)
        {
            if (GameManager.instance.isInstructor)
            {
                return;
            }

            resultCanvas.SetData(name, traineeNumber, area, kill);
        }
    }
}
