using Illegaldrone;
using KKH;
using KKH.MySQL;
using Photon.Pun;
using Photon.Realtime;
using SMW;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace RJH.UI
{
    [Serializable]
    public class MapData
    {
        public int missionMap;
        public int weather;
        public int timeZone;

        public bool isSuicideDrone;
        public bool isAttackDrone;
        public bool isScoutDrone;
        public bool isJammerDrone;

        public ExitGames.Client.Photon.Hashtable ToHashtable()
        {
            ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
            hashtable["missionMap"] = missionMap;
            hashtable["weather"] = weather;
            hashtable["timeZone"] = timeZone;
            hashtable["isSuicideDrone"] = isSuicideDrone;
            hashtable["isAttackDrone"] = isAttackDrone;
            hashtable["isScoutDrone"] = isScoutDrone;
            hashtable["isJammerDrone"] = isJammerDrone;
            return hashtable;
        }

        public static MapData FromHashtable(ExitGames.Client.Photon.Hashtable hashtable)
        {
            return new MapData
            {
                missionMap = (int)hashtable["missionMap"],
                weather = (int)hashtable["weather"],
                timeZone = (int)hashtable["timeZone"],
                isSuicideDrone = (bool)hashtable["isSuicideDrone"],
                isAttackDrone = (bool)hashtable["isAttackDrone"],
                isScoutDrone = (bool)hashtable["isScoutDrone"],
                isJammerDrone = (bool)hashtable["isJammerDrone"]
            };
        }
    }

    public class InstructorTrainViewController : MonoBehaviourPun
    {
        private static InstructorTrainViewController inst;
        public static InstructorTrainViewController Inst => inst;

        [SerializeField] private MapObjectController mapObjectController;
        public MapObjectController mapObjController => mapObjectController;

        [SerializeField] private TextMeshProUGUI trainingDate;
        [SerializeField] private TextMeshProUGUI instructorName;
        [SerializeField] private RawImage mainView;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Toggle mainCameraButton;

        [SerializeField] private TraineeViewToggle[] traineeViews;
        //[SerializeField] private List<Camera> traineeCameraList = new List<Camera>();

        [SerializeField] private Button pauseButton;
        [SerializeField] private Button endButton;
        [SerializeField] private Button commentButton;

        [SerializeField] private GameObject gameEndPopup;
        [SerializeField] private CameraController cameraController;
        //[SerializeField] private Button zoomIn;
        //[SerializeField] private Button zoomOut;
        private NewScnearioInfo scenario;
        private int currentTraineeView;
        private readonly string[] missionMaps = { "공항", "광화문", "원자력발전소" };

        public bool isFristStart = true;
        public bool isPause = true;

        public void Awake()
        {
            if (inst == null)
            {
                inst = this;
            }

            isFristStart = true;

            //if (!PhotonManager.instance.IsMasterClent())
            //{
            //    //Camera.main.GetComponent<CameraController>().enabled = true;
            //    gameObject.SetActive(false);
            //    return;
            //}
            scenario = GameManager.instance.scenario;
            //gameObject.SetActive(true);
            //SetTraineeAndDrones();
            SetTrainingCameraButton();

            mainCameraButton.onValueChanged.AddListener(OnMainCameraButton);

            pauseButton.onClick.AddListener(OnStartOrPause);
            endButton.GetComponent<ButtonManager>().interactable = false;
            endButton.onClick.AddListener(OnEnd);
            commentButton.onClick.AddListener(OnComment);

            //zoomIn.onClick.AddListener(ZoomIn);
            //zoomOut.onClick.AddListener(ZoomOut);
            trainingDate.text = DateTime.Now.ToString("yyyy-MM-dd");
            instructorName.text = GameManager.instance.userInfo.userName;
            mainCameraButton.GetComponentInChildren<TextMeshProUGUI>().text = missionMaps[(int)InstructorScenarioPage.Instance.GetScenarioMissionMap()];


            if (PhotonNetwork.IsMasterClient)
            {
                MapData data = new MapData();
                data.missionMap = (int)scenario.missionMap;
                data.weather = (int)scenario.weather;
                data.timeZone = (int)scenario.timeZone;
                data.isSuicideDrone = mapObjController.Drones[0].gameObject.activeInHierarchy;
                data.isAttackDrone = mapObjController.Drones[1].gameObject.activeInHierarchy;
                data.isScoutDrone = mapObjController.Drones[2].gameObject.activeInHierarchy;
                data.isJammerDrone = mapObjController.Drones[3].gameObject.activeInHierarchy;

                mapObjController.mapData = data;
            }
        }

        private void SetTrainingCameraButton()
        {
            List<Player> playerList = new List<Player>();
            int count = 0;

            if (PhotonNetwork.IsConnected)
            {
                playerList = PhotonManager_.Inst.GetPlayerList();

                //foreach (var player in playerList)
                //{
                //    string str = player.CustomProperties["UserProperties"].ToString();
                //    UserProperties userProperties = JsonUtility.FromJson<UserProperties>(str);
                //    int index = count;
                //    traineeViews[index].SetTraineeInfo(player, userProperties);
                //    traineeViews[index].GetComponent<Toggle>().onValueChanged.AddListener(delegate { OnCameraToggle(index); });
                //    traineeViews[index].SetDefenseArea(mapObjectController.GetTraineeDefenseArea(index));
                //    PhotonManager_.Inst.SetPlayerCustomProperty("TraineeNumber", index, player);
                //    count++;
                //    // 2025-04-18 RJH 훈련 정보 전달을 위한 훈련생 번호 설정 추가
                //}
                foreach (var traineeView in traineeViews)
                {
                    int index = count;
                    count++;
                    Player player = playerList.FirstOrDefault(p =>
                                                        p.CustomProperties.ContainsKey("TraineeNumber") &&
                                                        (int)p.CustomProperties["TraineeNumber"] == index);
                    if (player == null)
                        continue;

                    string str = player.CustomProperties["UserProperties"].ToString();
                    UserProperties userProperties = JsonUtility.FromJson<UserProperties>(str);
                    traineeView.SetTraineeInfo(player, userProperties);
                    traineeView.GetComponent<Toggle>().onValueChanged.AddListener(delegate { OnCameraToggle(index); });
                    traineeView.SetDefenseArea(mapObjectController.GetTraineeDefenseArea(index));
                    //PhotonManager_.Inst.SetPlayerCustomProperty("TraineeNumber", index, player);
                    // 2025-04-18 RJH 훈련 정보 전달을 위한 훈련생 번호 설정 추가
                    // 2025-04-24 RJH TraningReady.cs로 이동
                }
            }
            else
            {
                foreach (var traineeView in traineeViews)
                {
                    int index = count;
                    traineeView.GetComponent<Toggle>().onValueChanged.AddListener(delegate { OnCameraToggle(index); });
                    count++;
                }
            }

            // 리플레이에 필요한 훈련생 DB추가
            SetPlayerReplayData(playerList.Count);

            // 훈련생이 위치한 방어구역 
        }

        // SMW 추가
        // ===========================================================
        public void SetPlayerReplayData(int playerCount)
        {
            if (playerCount < 1)
            {
                ReplayManager.Instance.playerReplayDatas = null;
            }

            PlayerReplayData[] data = new PlayerReplayData[playerCount];
            for (int i = 0; i < playerCount; i++)
            {
                data[i] = traineeViews[i].GetPlayerData();
                data[i].CamNumber = i;
            }
            ReplayManager.Instance.playerReplayDatas = data;
        }
        // ===========================================================


        //private void SetTraineeAndDrones()
        //{
        //[삭제][RJH][2024.10.08]훈련 설정씬과 훈련씬이 합쳐져서 필요없음
        //// 드론 세팅
        //mapObjectController.SetDronePos(0, GameManager.instance.dronePosList[0]);
        //mapObjectController.SetDronePos(1, GameManager.instance.dronePosList[1]);
        //mapObjectController.SetDronePos(2, GameManager.instance.dronePosList[2]);
        //mapObjectController.SetDronePos(3, GameManager.instance.dronePosList[3]);
        //// 훈련생 세팅
        //mapObjectController.SetTraineePos(0, scenario.firstPlayerLocationPoint);
        //mapObjectController.SetTraineePos(1, scenario.secondPlayerLocationPoint);
        //mapObjectController.SetTraineePos(2, scenario.thirdPlayerLocationPoint);
        //mapObjectController.SetTraineePos(3, scenario.fourthPlayerLocationPoint);
        //}

        private void OnMainCameraButton(bool isOn)
        {
            if (isOn)
            {
                mainView.texture = mainCamera.targetTexture;
                cameraController.isCameraCanMove = true;
            }
        }

        private void OnCameraToggle(int index)
        {
            UTILS.Log($"{index}번 훈련생 카메라");
            mainView.texture = traineeViews[index].OnClick();
            cameraController.isCameraCanMove = false;
        }

        private void OnStartOrPause()
        {
            if (!endButton.GetComponent<ButtonManager>().interactable)
            {
                endButton.GetComponent<ButtonManager>().interactable = true;
                //foreach(GameObject drone in mapObjController.Drones)
                //{
                //    if (drone.activeInHierarchy)
                //    {
                //        drone.GetComponent<DroneObj>().NetWorkDroneCreate();
                //    }
                //}
            }

            //TODO 훈련 일시 정지
            if (isPause)
            {
                if (isFristStart)
                {
                    isFristStart = false;
                    UIManager.Inst.ShowStartCount();
                }
                else
                {
                    //UIManager.Inst.ShowPause();
                    UIManager.Inst.ShowStartCount();
                }
                // SMW 추가
                // ==========================================================================
                //GamePlay.Instance.StartGame(); // 2025-04-24 유지환 DroneStart()안으로 위치 이동
                GamePlay.Instance.StartGame();
                // ==========================================================================
                StartCoroutine(DroneStart());
                isPause = false;
            }
            else
            {
                UIManager.Inst.ShowPause();
                StartCoroutine(DroneStop());
                isPause = true;
            }
        }

        public IEnumerator DroneStart()
        {
            yield return new WaitForSeconds(4); // 3,2,1,start까지 생각 해서 4초 로 수정 2025-04-15 RJH
            foreach (GameObject obj in mapObjectController.Drones)
            {
                if (!obj.activeInHierarchy) continue;
                obj.GetComponent<DroneSpawnPoint>().SetIsCreate(true);
                //try
                //{
                //    //obj.GetComponent<DroneObj>().go_DroneObj.GetComponent<DroneNavMeshAgent>().isStart = true;
                //}
                //catch
                //{
                //    continue;
                //}
            }
        }

        public IEnumerator DroneStop()
        {
            yield return null;
            foreach (GameObject obj in mapObjectController.Drones)
            {
                try
                {
                    obj.GetComponent<DroneSpawnPoint>().SetIsCreate(false);
                    GamePlay.Instance.isStart = false;
                    //obj.GetComponent<DroneObj>().go_DroneObj.GetComponent<DroneNavMeshAgent>().isStart = false;
                }
                catch
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// 훈련종료 시 호출
        /// </summary>
        public void OnEnd()
        {
            //TODO 훈련 정지 팝업 생성
            // 공통 팝업 생성?
            //gameEndPopup.SetActive(true);
            commentButton.gameObject.SetActive(true);

            // 2025-04-15 RJH 사후 강평 외의 버튼 비활성화
            pauseButton.interactable = false;
            endButton.interactable = false;

            // SMW 추가 && 플레이어DB추가
            // ==========================================================================
            if (GameManager.instance.isInstructor)
            {
                GamePlay.Instance.End();
            }
            // ==========================================================================
        }

        private void OnComment()
        {
            // 사후강평 씬으로 전환
            // SMW 추가
            // ==========================================================================
            if (PhotonNetwork.IsConnected)
            {
                // 방에 있는 훈련생들 강퇴 -> 훈련생들 로그아웃
                Dictionary<int, Player> list = PhotonNetwork.CurrentRoom.Players;

                if (list != null || list.Count > 1)
                {
                    foreach (var key in list)
                    {
                        if (PhotonNetwork.LocalPlayer == key.Value)
                        {
                            continue;
                        }
                        PhotonManager_.Inst.SetPlayerCustomProperty("isKicked", true, key.Value);
                    }
                }
                PhotonManager_.Inst.SetPlayerCustomProperty("IsScenarioReady", null, PhotonNetwork.LocalPlayer);
                PhotonManager_.Inst.SetPlayerCustomProperty("IsTriningReady", null, PhotonNetwork.LocalPlayer);
                PhotonNetwork.LoadLevel("03.Replay");
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("03.Replay");
            }
            // ==========================================================================
        }

        public void SetTraineeWeaponType(int number, int weapon)
        {
            traineeViews[number].SetWeaponType(weapon);
        }

        public void SetTraineeDefenseArea(int number, int area)
        {
            traineeViews[number].SetDefenseArea(area);
        }

        public void SetTraineeReady(Player trainee)
        {
            string traineeInfo = trainee.CustomProperties["UserProperties"].ToString();
            UserProperties userProperties = JsonUtility.FromJson<UserProperties>(traineeInfo);

            foreach (var traineeView in traineeViews)
            {
                if (traineeView.userProperties == userProperties)
                {
                    traineeView.SetReady(true);
                }
            }
        }

        public void TraineeOutOfTrain(Player trainee)
        {
            string traineeInfo = trainee.CustomProperties["UserProperties"].ToString();
            UserProperties userProperties = JsonUtility.FromJson<UserProperties>(traineeInfo);

            foreach (var traineeView in traineeViews)
            {
                if (traineeView.userProperties == userProperties)
                {
                    //ToDo 훈련생 퇴장
                }
            }
        }

        /// <summary>
        /// 줌인 기능 실행
        /// </summary>
        public void ZoomIn()
        {
            if (cameraController.mainCamera.fieldOfView <= 20)
            {
                return;
            }
            cameraController.mainCamera.fieldOfView -= 7.5f;
        }

        /// <summary>
        /// 줌 아웃 기능 실행
        /// </summary>
        public void ZoomOut()
        {
            if (cameraController.mainCamera.fieldOfView >= 70)
            {
                return;
            }
            cameraController.mainCamera.fieldOfView += 7.5f;
        }
    }
}


