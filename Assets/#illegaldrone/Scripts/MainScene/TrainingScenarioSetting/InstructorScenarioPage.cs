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
using UnityEngine;
using UnityEngine.UI;

namespace RJH.UI
{
    public class InstructorScenarioPage : MonoBehaviour
    {
        #region Instance
        private static InstructorScenarioPage instance;
        public static InstructorScenarioPage Instance { get { return instance; } }
        #endregion

        [Header("TrainingPreference")]
        [SerializeField] private TrainingPreference trainingPreference; // 훈련 환경설정 화면

        [Header("TrainingScenario")]
        public GameObject trainingScenario;  // 훈련 시나리오 설정 화면

        [Header("MapObjectController")]
        [SerializeField] private MapObjectController mapObjectController;
        public MapObjectController mapController => mapObjectController;
        public GameObject mapObject => mapObjectController.gameObject;

        [Header("InstructorTrainViewController")]
        [SerializeField] private InstructorTrainViewController trainView; // 교관 훈련 화면 

        [Space]
        [Header("Menu")]
        [SerializeField] private RectTransform rightPanel;

        [Space]
        [Header("MainTarget")]
        public Transform mainTarget;
        [SerializeField] private Transform defenseArea; //방어 구역
        [SerializeField] private Transform coreArea;    // 핵심 방어 구역
        [SerializeField] private Transform mainArea;    // 메인 방어 구역
        [SerializeField] private Transform borderArea;  // 경계 구역

        [Space]
        [Header("Drone")]
        public Dictionary<int, DroneOption> droneOptionDict = new Dictionary<int, DroneOption>();
        [SerializeField] private DroneButton[] droneButtonList;

        public DroneButton[] DroneButtonList => droneButtonList;
        //[SerializeField] private List<DroneObj> droneObjList = new List<DroneObj>();

        [Space]
        [Header("Trainee")]
        [SerializeField] private List<TraineeButton> traineeButtonList = new List<TraineeButton>();

        //[Space]
        //[Header("Zoom")]
        //[SerializeField] private Button zoomIn;
        //[SerializeField] private Button zoomOut;
        //[SerializeField] private CameraController cameraController;

        [Space]
        [Header("PageButton")]
        [SerializeField] private Button previousButton;
        [SerializeField] private Button missionDeliveryButton;
        [SerializeField] private Button MenuButton;

        [Space]
        [Header("Popup")]
        [SerializeField] private DroneOptionPopup optionChangePopup;
        public DroneOptionPopup OptionChangePopup => optionChangePopup;
        [SerializeField] private DroneTypeDeletePopup typeDeletePopup;
        [SerializeField] private TraineeKickPopup kickPopup;

        [Space]
        [Header("Controller")]
        [SerializeField] CameraController cameracontrol;
        [Space]
        [Header("Sprite")]
        [SerializeField] Sprite[] sprites;
        private NewScnearioInfo scenarioInfo;

        private bool isHideMenu = false;
        private static readonly DroneOption[] setDroneOption = new DroneOption[]
            {
                new DroneOption { droneType = 0, flyingType = 2, flyingOrder = 2 }, // 자폭드론
                new DroneOption { droneType = 1, flyingType = 1, flyingOrder = 1 }, // 공격드론
                new DroneOption { droneType = 2, flyingType = 0, flyingOrder = 0 }, // 정찰드론
                new DroneOption { droneType = 3, flyingType = 2, flyingOrder = 2 }  // 재밍드론
            };


        private void Awake()
        {
            // 싱글톤 인스턴스 설정
            instance = this;

            // 교관 모드일 경우 카메라 깊이 설정
            if (GameManager.instance.isInstructor)
            {
                Camera.main.depth = 0;
            }

            // 드론 옵션 설정
            foreach (var droneOption in setDroneOption)
            {
                // 드론 옵션 딕셔너리에 추가
                droneOptionDict[droneOption.droneType] = droneOption;

                // 드론 버튼에 값 입력
                DroneButton droneButton = droneButtonList[droneOption.droneType];
                droneButton.SetDrone(droneOption);
                droneButton.Button.onClick.AddListener(() => DroneButtonAction(droneOption.droneType));

                // 드론 오브젝트에 값 입력
                GameObject obj = mapController.GetDrone(droneOption.droneType);
                if (obj != null)
                {
                    DroneSpawnPoint droneObj = obj.GetComponent<DroneSpawnPoint>();
                    droneObj.SetDroneInfo(droneOption);
                }
            }

            // 훈련생 화면에서는 실행 안함
            if (PhotonNetwork.IsConnected && (int)GameManager.instance.userInfo.userGroup == 2)
            {
                return;
            }

            // 버튼 클릭 리스너 설정
            //zoomIn.onClick.AddListener(ZoomIn); // 2025-04-03 RJH CameraController.cs로 이동
            //zoomOut.onClick.AddListener(ZoomOut);
            previousButton.onClick.AddListener(OnPrevious);
            missionDeliveryButton.onClick.AddListener(SendScenarioInfo);
            MenuButton.onClick.AddListener(SetMenu);
            scenarioInfo = new NewScnearioInfo();

            // 포톤 네트워크 이벤트 리스너 설정
            try
            {
                PhotonManager_.Inst.PlayerJoinRoomAction.AddListener(NewTraineeJoin);
                PhotonManager_.Inst.PlayerLeaveRoomAction.AddListener(TraineeOut);
            }
            catch
            {
                UTILS.LogError("포톤 연결 문제");
            }
        }


        private void OnDisable()
        {
            PhotonManager_.Inst.PlayerJoinRoomAction.RemoveListener(NewTraineeJoin);
            PhotonManager_.Inst.PlayerLeaveRoomAction.RemoveListener(TraineeOut);
        }

        /// <summary>
        /// 훈련 시나리오 화면 세팅 (경계구역 표시를 메인목표에 맞춰 세팅, 드론 오브젝트 위치와 훈련생 오브젝트 위치 설정)
        /// 공항 맵에서만 자폭드론 생성 가능
        /// </summary>
        public void SetScenario()
        {
            // 방어 구역 위치를 메인 목표 위치로 설정
            defenseArea.position = mainTarget.position;

            // 드론 오브젝트 위치 설정
            for (int i = 0; i < droneButtonList.Length; i++)
            {
                GameObject droneObj = mapObjectController.GetDrone(i);
                
                if (droneObj != null)
                {
                    droneObj.transform.position = GetPointOnCircle(180f + 30f * i, 800f);
                }

                // 2025-05-07 RJH 정찰 드론외의 드론 비활성화
                if (i != 2 && droneObj.activeSelf) // 정찰 드론 : 2
                {
                    droneObj.SetActive(false);
                    droneButtonList[i].DroneDeActivate();
                }
                //
            }

            //droneButtonList[0].GetComponent<Button>().interactable = mapObjectController.CheckAirPortMap();

            // 훈련생 리스트 가져오기
            List<Player> playerList = new List<Player>();
            if (PhotonNetwork.IsConnected)
            {
                playerList = PhotonManager_.Inst.GetPlayerList();
            }

            // 훈련생 오브젝트 위치 설정
            for (int i = 0; i < traineeButtonList.Count; i++)
            {
                //if (playerList.Count > i)
                //{
                //    traineeButtonList[i].SetTrainee(playerList[i]);
                //    GameObject traineeObj = mapObjectController.GetTrainee(i);
                //    traineeObj.SetActive(true);
                //    traineeObj.GetComponent<TraineeObj>().SetName(playerList[i]);
                //    traineeObj.transform.position = GetPointOnCircle(90f * i, 300f);
                //}
                //foreach(var playerdd in playerList)
                //{
                //    Debug.Log(playerdd.NickName + "/////////");
                //    Debug.Log((int)playerdd.CustomProperties["TraineeNumber"]+"/////////");
                //}   
                
                Player player = playerList.FirstOrDefault(p =>
                    p.CustomProperties.ContainsKey("TraineeNumber") &&
                    (int)p.CustomProperties["TraineeNumber"] == i);
                if (player == null)
                    continue;

                traineeButtonList[i].SetTrainee(player);
                GameObject traineeObj = mapObjectController.GetTrainee(i);
                traineeObj.SetActive(true);
                traineeObj.GetComponent<TraineeObj>().SetName(playerList[i]);
                traineeObj.GetComponent<TraineeObj>().isTraineeSetPos = false; // 2025-05-09 RJH 훈련생 좌표 설정을 위한 기능 추가
                traineeObj.transform.position = GetPointOnCircle(90f * i, 300f);
            }
        }

        public void SetSuicideDroneButton()
        {
            // 공항 맵이면 자폭 드론 버튼 상호작용 활성화
            droneButtonList[0].GetComponent<Button>().interactable = mapObjectController.CheckAirPortMap();
        }

        /// <summary>
        /// 목표를 중심으로 원형을 이루는 좌표값을 반환
        /// </summary>
        /// <param name="angleInDegrees"> 각도</param>
        /// <param name="radius"> 반지름</param>
        /// <returns>계산된 좌표값</returns>
        public Vector3 GetPointOnCircle(float angleInDegrees, float radius)
        {
            // 각도를 라디안으로 변환
            float angleInRadians = angleInDegrees * Mathf.Deg2Rad;

            // 원 위의 좌표 계산
            float x = mainTarget.position.x + radius * Mathf.Cos(angleInRadians);
            float z = mainTarget.position.z + radius * Mathf.Sin(angleInRadians);

            // y 좌표는 고정값 600로 설정
            return new Vector3(x, 600, z);
        }

        /// <summary>
        /// 드론 버튼 클릭 시 실행되는 함수
        /// </summary>
        /// <param name="droneType">클릭된 드론 타입</param>
        public void DroneButtonAction(int droneType)
        {
            // 드론 오브젝트 가져오기
            GameObject droneObj = mapObjectController.GetDrone(droneType);

            // 드론 오브젝트가 활성화되어 있는지 확인
            if (droneObj != null && droneObj.activeInHierarchy)
            {
                // 드론 옵션 로그 출력
                UTILS.Log("드론 옵션");

                // 드론 버튼 활성화 및 옵션 팝업 설정
                droneButtonList[droneType].ButtonActive();
                optionChangePopup.SetOn(droneButtonList[droneType], droneOptionDict[droneType]);
                mapController.Drones[droneType].GetComponent<DroneObj>().objController.DronesUIActive(true);
            }
            else
            {
                // 드론 오브젝트 활성화 및 드론 버튼 활성화
                mapController.SetDroneOn(droneType);
                droneButtonList[droneType].DroneActivate();
                UTILS.Log("드론 생성");
            }
        }

        /// <summary>
        /// 드론 버튼 활성화
        /// </summary>
        /// <param name="droneType">활성화할 드론 타입</param>
        public void DroneButtonActivate(int droneType)
        {
            // 모든 드론 버튼 비활성화
            foreach (var droneButton in droneButtonList)
            {
                droneButton.ButtonDeActivate();
            }

            // 선택된 드론 버튼 활성화
            droneButtonList[droneType].ButtonActive();

            // 드론 옵션 팝업 설정
            optionChangePopup.SetOn(droneButtonList[droneType], droneOptionDict[droneType]);
        }

        /// <summary>
        /// 드론 옵션 팝업에서 드론 옵션값을 받으면 드론 버튼과 드론 오브젝트에 전달
        /// </summary>
        /// <param name="droneOption">변경된 드론 옵션</param>
        public void SetDroneOptionChange(DroneOption droneOption)
        {
            // 드론 옵션 딕셔너리에 업데이트
            droneOptionDict[droneOption.droneType] = droneOption;

            // 드론 버튼에 새로운 옵션 설정 및 활성화
            DroneButton droneButton = droneButtonList[droneOption.droneType];
            droneButton.SetDrone(droneOption);
            droneButton.ButtonActive();

            // 드론 오브젝트에 새로운 옵션 설정 및 UI 비활성화
            DroneSpawnPoint droneObj = mapController.Drones[droneOption.droneType].GetComponent<DroneSpawnPoint>();
            droneObj.objController.DronesUIDeActive();
            droneObj.SetDroneInfo(droneOption);
        }

        /// <summary>
        /// 드론 삭제 팝업 활성화
        /// </summary>
        /// <param name="type">삭제할 드론 타입</param>
        public void DroneDeletePopup(int type)
        {
            // 마우스 위치를 기준으로 팝업 위치 설정
            Vector3 setPosition = Input.mousePosition;
            if (setPosition.y < 212)
            {
                setPosition.y = 212;
            }
            if (setPosition.x > 1297)
            {
                setPosition.x = 1297;
            }

            // 팝업 위치 설정 및 팝업 활성화
            typeDeletePopup.transform.position = setPosition;
            typeDeletePopup.SetDeletePopup(type);
        }

        /// <summary>
        /// 드론 오브젝트 비활성화
        /// </summary>
        /// <param name="type">비활성화할 드론 번호</param>
        public void DroneObjDelete(int type)
        {
            // 드론 버튼 비활성화
            droneButtonList[type].DroneDeActivate();

            // 드론 오브젝트 비활성화
            mapObjectController.SetDroneOff(type);

            // 드론 옵션 팝업 비활성화
            OptionChangePopup.SetOff();
        }
        /// <summary>
        /// 훈련 중 드론이 파괴되어 드론 아이콘 비활성화 
        /// 모든 드론이 비활성화 되었는지 체크
        /// </summary>
        /// <param name="type">비활성화할 드론 아이콘 번호</param>
        public void DroneDestroy(int type)
        {
            //mapObjectController.SetDroneOff(type);

            if (mapObjectController.CheckAllDroneOff())
            {
                if (GameManager.instance.isInstructor)
                {
                    // 훈련 종료 5초 딜레이 추가
                    GamePlay.Instance.isDroneAllDestroyed = true;
                    StartCoroutine(DelayOnEnd());
                    //trainView.OnEnd();
                }
            }
        }

        private IEnumerator DelayOnEnd()
        {
            yield return new WaitForSeconds(5);
            trainView.OnEnd();
        }

        /// <summary>
        /// 모든 드론 버튼 비활성화
        /// </summary>
        public void DroneButtonInActivate()
        {
            foreach (var button in droneButtonList)
            {
                button.ButtonDeActivate();
            }
        }

        /// <summary>
        /// 훈련생 내보내기 팝업 활성화
        /// </summary>
        /// <param name="player">내보내기할 훈련생 정보</param>
        /// <param name="traineeButton">내보내기를 실행한 버튼 정보</param>
        public void TraineeKickPopup(Player player, TraineeButton traineeButton)
        {
            // 훈련생 내보내기 팝업 설정
            kickPopup.SetPopup(player, traineeButton);
        }

        /// <summary>
        ///  훈련생 내보내기 실행
        /// </summary>
        /// <param name="player">내보낼 훈련생 정보</param>
        public void TraineeDelete(Player player)
        {
            // 훈련생의 커스텀 속성 업데이트
            PhotonManager_.Inst.SetPlayerCustomProperty("isKicked", true, player);
        }

        /// <summary>
        /// 훈련생 퇴장 처리
        /// </summary>
        /// <param name="player">퇴장할 훈련생 정보</param>
        public void TraineeOut(Player player)
        {
            // 훈련생 버튼 리스트를 순회하며 해당 훈련생을 찾음
            foreach (var traineeButton in traineeButtonList)
            {
                if (traineeButton.GetTrainee() == player)
                {
                    // 훈련생 오브젝트 비활성화
                    mapObjectController.SetTraineeOff(traineeButton.GetNumber());

                    // 훈련생 버튼 상태 업데이트
                    traineeButton.TraineeOut();
                    break;
                }
            }
        }

        /// <summary>
        /// 시나리오 설정 진행 중 훈련생이 새로 참여하면 실행
        /// </summary>
        /// <param name="player">새로 참여하는 훈련생 정보</param>
        public void NewTraineeJoin(Player player)
        {
            // 훈련생 버튼 리스트를 순회하며 비활성화된 훈련생 오브젝트를 찾음

            foreach (var traineeButton in traineeButtonList)
            {
                GameObject traineeObj = mapObjectController.GetTrainee(traineeButton.GetNumber());
                if (!traineeObj.activeSelf)
                {
                    // 훈련생 버튼과 오브젝트 설정
                    traineeButton.SetTrainee(player);
                    traineeObj.SetActive(true);
                    traineeObj.GetComponent<TraineeObj>().SetName(player);
                    traineeObj.transform.position = GetPointOnCircle(90f * traineeButton.number, 300f);
                    PhotonManager_.Inst.SetPlayerCustomProperty("TraineeNumber", traineeButton.GetNumber(), player); // 2025-04-24 RJH 새로 입장한 훈련생에 번호 등록
                    return;
                }
            }

            // 훈련 인원이 초과된 경우 로그 출력 및 훈련생 강제 로그아웃
            TraineeDelete(player);
            //player.SetCustomProperties(new Hashtable() { { "isKicked", true } });
        }

        /// <summary>
        /// 메인 목표(랜드마크) 위치값 반환
        /// </summary>
        /// <returns>메인 목표 위치값</returns>
        public Vector3 GetMainTargetPosition()
        {
            return mainTarget.position;
        }

        /// <summary>
        /// 줌인 기능 실행
        /// </summary>
        //public void ZoomIn()
        //{
        //    cameraController.CameraZoomInOut(-1);
        //}

        /// <summary>
        /// 줌 아웃 기능 실행
        /// </summary>
        //public void ZoomOut()
        //{
        //    cameraController.CameraZoomInOut(1);
        //}

        /// <summary>
        /// 훈련 설정 화면으로 돌아가기 
        /// </summary>
        public void OnPrevious()
        {
            // 2025-04-30 RJH 교관이 드론 설정화면에서 벗어나면 IsScenarioReady = false
            #region 훈련생 로딩 화면
            ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();
            playerProperties["IsScenarioReady"] = false; // 준비 상태를 false로 설정
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
            #endregion

            trainingScenario.gameObject.SetActive(false);
            foreach (GameObject obj in mapController.MissionMaps)
            {
                obj.SetActive(false);
            }
            foreach (GameObject obj in mapController.Weathers)
            {
                obj.SetActive(false);
            }
            foreach (GameObject obj in mapController.TimeZones)
            {
                obj.SetActive(false);
            }

            mapObject.SetActive(false);
            trainingPreference.gameObject.SetActive(true);
        }

        public void RedLineOn()
        {
            defenseArea.Find("RedLine").gameObject.SetActive(true);
        }

        public void RedLineOff()
        {
            defenseArea.Find("RedLine").gameObject.SetActive(false);
        }

        public void SetMenu()
        {
            StopAllCoroutines();
            if (isHideMenu)
            {
                StartCoroutine(ShowMenu());
                // 메뉴 보여주기
                UTILS.Log("메뉴 보여주기");
                isHideMenu = false;
            }
            else
            {
                StartCoroutine(CloseMenu());
                // 메뉴 숨기기
                UTILS.Log("메뉴 숨기기");
                isHideMenu = true;
            }
        }

        private IEnumerator CloseMenu()
        {
            MenuButton.GetComponent<Image>().sprite = sprites[0];
            Vector3 target = new Vector3(300, 0, 0);
            while (rightPanel.anchoredPosition.x < target.x)
            {
                Vector3 vec = Vector3.right * 50;
                rightPanel.anchoredPosition = Vector3.SmoothDamp(rightPanel.anchoredPosition, target, ref vec, 0.05f);
                yield return null;
            }
            yield return null;
        }

        private IEnumerator ShowMenu()
        {
            MenuButton.GetComponent<Image>().sprite = sprites[1];
            Vector3 target = new Vector3(0, 0, 0);
            while (rightPanel.anchoredPosition.x > target.x)
            {
                Vector3 vec = Vector3.left * 50;
                rightPanel.anchoredPosition = Vector3.SmoothDamp(rightPanel.anchoredPosition, target, ref vec, 0.05f);
                yield return null;
            }
            yield return null;
        }

        #region ScenarioInfoSetting
        /// <summary>
        /// 훈련 환경설정 정보 저장
        /// </summary>
        /// <param name="missionMap">훈련 실행 장소</param>
        /// <param name="weather">훈련 날씨</param>
        /// <param name="timeZone">훈련 시간대</param>
        /// <param name="limitPlayTime">훈련 제한시간</param>
        public void SetScenarioInfo_Preference(MapType missionMap, Weather weather, KKH.MySQL.TimeZone timeZone, int limitPlayTime)
        {
            scenarioInfo.missionMap = missionMap;
            scenarioInfo.weather = weather;
            scenarioInfo.timeZone = timeZone;
            scenarioInfo.limitPlayTime = limitPlayTime;
        }

        /// <summary>
        /// 시나리오 정보 세팅(시나리오ID, 플레이 인원)
        /// </summary>
        public void SetScenarioInfo()
        {
            //{
            string scenarioID = DateTime.Now.ToString("yyyyMMddHHmmss");
            scenarioInfo.scenarioID = scenarioID;
            //} 시나리오 ID

            if (PhotonNetwork.IsConnected)
            {
                //{
                int traineeCount = PhotonManager_.Inst.GetPlayerList().Count;
                scenarioInfo.playingNumber = traineeCount;
                //} 플레이 인원

                //{
                string createUserInfo = PhotonNetwork.LocalPlayer.CustomProperties["UserProperties"].ToString();
                UserProperties userProperties = JsonUtility.FromJson<UserProperties>(createUserInfo);
                scenarioInfo.createUser = userProperties.id;
                //} 생성 교관

                //{
                Player endPlayer = PhotonManager_.Inst.GetLastPlayer();
                string endPlayerInfo = endPlayer.CustomProperties["UserProperties"].ToString();
                UserProperties endPlayerProperties = JsonUtility.FromJson<UserProperties>(endPlayerInfo);
                scenarioInfo.endScenarioPlayID = endPlayerProperties.id;
                //} 마지막 훈련생 ID 
            }



            //PhotonNetwork.AutomaticallySyncScene = true;
            //PhotonManager_.Inst.SceneLoad("TrainingScene");
        }

        /// <summary>
        /// 시나리오 정보에 각각의 훈련생 시작 위치 저장 
        /// </summary>
        /// <param name="firstTrainee"></param>
        /// <param name="secondTrainee"></param>
        /// <param name="thirdTrainee"></param>
        /// <param name="fourthTrainee"></param>
        public void SetScenarioInfo_Trainee(Transform firstTrainee, Transform secondTrainee, Transform thirdTrainee, Transform fourthTrainee)
        {
            scenarioInfo.firstPlayerLocationPoint = new Vector3(firstTrainee.localPosition.x, firstTrainee.localPosition.y, firstTrainee.localPosition.z);
            scenarioInfo.secondPlayerLocationPoint = new Vector3(secondTrainee.localPosition.x, secondTrainee.localPosition.y, secondTrainee.localPosition.z);
            scenarioInfo.thirdPlayerLocationPoint = new Vector3(thirdTrainee.localPosition.x, thirdTrainee.localPosition.y, thirdTrainee.localPosition.z);
            scenarioInfo.fourthPlayerLocationPoint = new Vector3(fourthTrainee.localPosition.x, fourthTrainee.localPosition.y, fourthTrainee.localPosition.z);
        }

        /// <summary>
        /// 시나리오 정보에 방어할 목표의 위치 저장
        /// </summary>
        /// <param name="defense">방어할 목표 트렌스폼</param>
        public void SetScenarioInfo_Defense(Transform defense)
        {
            scenarioInfo.defenseobjectlocationPoint = new Vector3(defense.localPosition.x, defense.localPosition.y, defense.localPosition.z);
        }

        /// <summary>
        /// 드론 정보 DB에 전송
        /// </summary>
        public void SetDroneInfo()
        {
            //int count = 0;
            foreach (var drone in droneOptionDict.Values)
            {
                GameObject droneObj = mapObjectController.GetDrone(drone.droneType);
                if (droneObj == null) continue;

                //int index = count;
                if (MySQLManager.SetEnemyDrone(scenarioInfo.scenarioID, drone.droneType, drone.flyingOrder, drone.flyingType, droneObj.transform.localPosition))
                {
                    UTILS.Log($"EnemyDroneInfo Send + No.{drone.droneType}");
                }
            }
        }

        /// <summary>
        /// 시나리오 정보 DB에 전송
        /// </summary>
        public void SendScenarioInfo()
        {
            if (!mapController.isDefensReady)
            {
                UTILS.LogError("방어 건물이 지정되지 않았습니다.");
                GuideToast.Instance.PopupMSG(MSG.NEEDDEFENSEOBJECT);
                return;
            }

            if (!mapController.isTraineesSetPos)
            {
                UTILS.LogError("훈련생의 좌표 설정이 필요합니다.");
                GuideToast.Instance.PopupMSG(MSG.NEEDTRAINEEPOS);
                return;
            }

            if(!mapController.isTraineesReady)
            {
                UTILS.LogError("훈련생이 배치 가능한 구역 밖에 있습니다.");
                GuideToast.Instance.PopupMSG(MSG.TRAINEEOUTOFRANGE);
                return;
            }

            if(!mapController.isDronesReady)
            {
                UTILS.LogError("드론이 배치 불가능한 구역 안에 있습니다.");
                GuideToast.Instance.PopupMSG(MSG.DRONEINOFRANGE);
                return;
            }

            SetScenarioInfo();
            SetScenarioInfo_Preference(trainingPreference.MissionMap, trainingPreference.Weather, trainingPreference.TimeZone, trainingPreference.Limit_Minute);
            SetScenarioInfo_Defense(mainTarget.transform);
            SetScenarioInfo_Trainee(mapObjectController.GetTrainee(0).transform, mapObjectController.GetTrainee(1).transform, mapObjectController.GetTrainee(2).transform, mapObjectController.GetTrainee(3).transform);
            SetDroneInfo();
            if (MySQLManager.SetNewScenarioInfo(scenarioInfo))
            {
                UTILS.Log($"SetNewScenarioInfo: true");
            }
            /* 훈련 생성한 교관 ID, 마지막 훈련생 ID, 시나리오 생성 날짜, 시나리오 삭제 날짜, 시나리오 만료일, 플레이 모드, 방어 경계 타입, 방어 경계 면적, 목표 타겟 HP, 플레이 타임
             */
            //[RJH][임시 테스트][2024.09.26] 
            GameManager.instance.scenario = scenarioInfo;
            trainingScenario.SetActive(false);
            trainView.gameObject.SetActive(true);
            mapObjectController.HideAllDroneDeleteButton();

            if (PhotonNetwork.IsConnected)
            {
                PhotonManager_.Inst.SetRoomCustomProerty("LimitTime", trainingPreference.Limit_Minute);
                PhotonManager_.Inst.SetPlayerCustomProperty("IsTriningReady", true);
            }
        }
        #endregion

        public MapType GetScenarioMissionMap()
        {
            return scenarioInfo.missionMap;
        }

        /// <summary>
        /// 훈련생이 어느 방어지대에 있는지 확인
        /// </summary>
        /// <param name="traineeNumber">시나리오에서 훈련생 번호</param>
        /// <returns>2 : 핵심방어지대, 1 : 주방어지대, 0 : 경계지대</returns>
        public string GetTraineeDefenceArea(int traineeNumber) 
        {
            if (mapController.GetTraineeDefenseArea(traineeNumber) == 2)
            {
                return "핵심방어지대";
            }
            else if (mapController.GetTraineeDefenseArea(traineeNumber) == 1)
            {
                return "주방어지대";
            }
            else
            {
                return "경계지대";
            }
        }
    }
}