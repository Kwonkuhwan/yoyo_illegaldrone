using Illegaldrone;
using KKH;
using Photon.Pun;
using RJH.UI;
using SMW;
using System.Collections.Generic;
using UnityEngine;

namespace RJH
{
    public class MapObjectController : MonoBehaviour
    {
        static private MapObjectController instance;
        static public MapObjectController Inst => instance;

        [SerializeField] private GameObject[] missionMaps;
        public GameObject[] MissionMaps => missionMaps;
        [SerializeField] private GameObject[] weathers;
        public GameObject[] Weathers => weathers;
        public AudioSource audioSource_Weather;
        [SerializeField] private GameObject[] timeZones;
        public GameObject[] TimeZones => timeZones;
        [Space]
        [SerializeField] private Transform droneParent;
        [SerializeField] private List<string> droneNames;
        [SerializeField] private List<GameObject> drones;

        // [KKH][추가][2024.10.15] - 드론 오브젝트 접근용으로 추가함
        public List<GameObject> Drones => drones;
        [SerializeField] private GameObject[] trainees;
        [Space]
        [SerializeField] private Transform defenseArea;
        public Vector3 DefenseAreaPosition
        {
            get
            {
                return new Vector2(defenseArea.position.x, defenseArea.position.z);
            }
        }
        [SerializeField] private GameObject defenseAreaCanvas;
        // [KKH][추가][2024.10.17] - 표적 건물 스크립트
        [SerializeField] private DefenseObject defenseObject;

        [SerializeField] private GameObject currentMissionMap;
        [SerializeField] private GameObject currentWeather;
        [SerializeField] private GameObject currentTimeZone;

        public bool isDefensReady = false;

        public bool isTraineesReady
        {
            get
            {
                Vector3 defenseObjectPos = new Vector3(defenseObject.transform.position.x, 0, defenseObject.transform.position.z);
                foreach (var trainee in trainees)
                {
                    if (trainee.activeSelf)
                    {
                        Vector3 traineePos = new Vector3(trainee.transform.position.x, 0, trainee.transform.position.z);
                        float distance = Vector3.Distance(defenseObjectPos, traineePos);
                        Debug.Log(distance);
                        if (distance > 580f)
                            return false;
                    }
                }
                return true;
            }
        }

        public bool isTraineesSetPos
        {
            get
            {
                foreach(var trainee in trainees)
                {
                    if(trainee.activeSelf)
                    {
                        if(trainee.GetComponent<TraineeObj>().isTraineeSetPos == false)
                            return false;
                    }
                }
                return true;
            }
            
        }

        public bool isDronesReady
        {
            get
            {
                Vector3 defenseObjectPos = new Vector3(defenseObject.transform.position.x, 0, defenseObject.transform.position.z);
                foreach (var drone in drones)
                {
                    if (drone.activeSelf)
                    {
                        Vector3 dronePos = new Vector3(drone.transform.position.x, 0, drone.transform.position.z);
                        float distance = Vector3.Distance(defenseObjectPos, dronePos);
                        if (distance < 800)
                            return false;
                    }
                }
                return true;
            }
        }


        [SerializeField] private PhotonView pv;
        [SerializeField] private MapData map;

        public MapData mapData
        {
            get
            {
                return map;
            }
            set
            {
                map = value;
                SaveMapData();
            }
        }

        private void Awake()
        {
            // 싱글톤 인스턴스 설정
            instance = this;

            // PhotonView 컴포넌트 가져오기
            pv = GetComponent<PhotonView>();

            // 오디오 소스 설정
            if (audioSource_Weather == null)
            {
                audioSource_Weather = GetComponent<AudioSource>();
            }

            // 초기 맵, 날씨, 시간대 설정
            currentMissionMap = missionMaps[1];
            currentTimeZone = timeZones[0];
            GameManager.instance.logMessageCallBack.AddListener(SetScore);

            // 교관이 아닌 경우 드론 비활성화
            if (!GameManager.instance.isInstructor)
            {
                foreach (GameObject drone in Drones)
                {
                    drone.SetActive(false);
                }
            }
        }

        private void Start()
        {
            SetDroneOff(0);
            SetDroneOff(1);
            SetDroneOff(3);

            if (!PhotonNetwork.IsMasterClient)
            {
                LoadMapData();
            }
        }

        private void Update()
        {
            if (!trainees[0].activeInHierarchy) return;
            Transform mainTarget = defenseObject.transform;
            float distance = Vector2.Distance(new Vector2(trainees[0].transform.position.x, trainees[0].transform.position.z), new Vector2(mainTarget.position.x, mainTarget.position.z));

            //UTILS.Log(distance);
        }

        public int missionmapIdx;
        public int weatherIdx;
        public int timezoneIdx;
        /// <summary>
        /// 맵 정보 저장
        /// </summary>
        public void SetMapOptions(int missionmap, int weather, int timezone)
        {
            this.missionmapIdx = missionmap;
            this.weatherIdx = weather;
            this.timezoneIdx = timezone;
        }

        /// <summary>
        /// 미션맵, 날씨, 시간대 설정에 따라 맵 변경
        /// </summary>
        /// <param name="missionmap">미션맵</param>
        /// <param name="weather">날씨</param>
        /// <param name="timezone">시간대</param>
        public void EnableMapOption()
        {
            if (currentMissionMap != null)
            {
                currentMissionMap.SetActive(false);
            }
            currentMissionMap = missionMaps[missionmapIdx];

            if (TraineeObj.Inst != null)
            {
                currentWeather = TraineeObj.Inst.go_Weathers[weatherIdx];
            }

            currentTimeZone = TimeZones[timezoneIdx];
        }

        /// <summary>
        /// 설정된 맵, 날씨, 시간대를 활성화
        /// </summary>
        public void SetOnMap()
        {
            currentMissionMap.SetActive(true);
            currentTimeZone.SetActive(true);

            if (TraineeObj.Inst != null)
            {
                currentWeather.SetActive(true);
                if (weatherIdx == 2)
                {
                    audioSource_Weather.Play();
                }
                else
                {
                    audioSource_Weather.Stop();
                }
            }

            if (weatherIdx == 3) // 안개일때
            {
                UTILS.Log("FOG on");
                SetEnvironmentLighting(true);
                SetFog(true);
            }
            else
            {
                SetEnvironmentLighting(false);
                SetFog(false);
            }
        }

        public void SetEnvironmentLighting(bool enable)
        {
            if (enable)
            {
                RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox; // 또는 다른 AmbientMode
            }
            else
            {
                RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat; // Flat으로 설정하거나, 다른 기본값 사용
                RenderSettings.ambientLight = Color.black; // ambientLight 색상도 검정색으로 설정하여 완전히 끄도록 설정
            }
        }

        // Fog 활성화/비활성화 함수
        public void SetFog(bool enable)
        {
            RenderSettings.fogDensity = 0.001f;
            RenderSettings.fog = enable;
        }

        /// <summary>
        /// defenseArea의 포지션을 현재 맵의 MainTarget의 포지션에 맞추고 mainTarget 리턴, MainTarget을 찾을 수 없을면 null 리턴 
        /// </summary>
        /// <returns></returns>
        public Transform SetMainTarget()
        {
            //Transform mainTarget = currentMissionMap.transform.Find("MainTarget");
            Transform mainTarget = defenseObject.transform;//.Find("MainTarget");
            if (mainTarget != null)
            {
                defenseArea.position = mainTarget.position;
                return mainTarget;
            }
            else
            {
                UTILS.Log("메인 목표를 찾을 수 없다.");
                return null;
            }
        }

        /// <summary>
        /// 훈련생이 방어하고 있는 구역 확인
        /// </summary>
        /// <param name="index">확인할 훈련생 번호</param>
        /// <returns>방어 구역 코드 (0: 경계 구역, 1: 주 구역, 2: 핵심 구역)</returns>
        public int GetTraineeDefenseArea(int index)
        {
            // 메인 목표 위치 가져오기
            Transform mainTarget = defenseObject.transform;

            // 훈련생과 메인 목표 간의 거리 계산
            float distance = Vector2.Distance(
                new Vector2(trainees[index].transform.position.x, trainees[index].transform.position.z),
                new Vector2(mainTarget.position.x, mainTarget.position.z)
            );

            // 거리 기준으로 방어 구역 결정
            if (distance <= 145.0f)
            {
                UTILS.Log("핵심 구역");
                return 2; // 핵심 구역
            }
            else if (distance <= 300.0f)
            {
                UTILS.Log("주 구역");
                return 1; // 주 구역
            }
            else
            {
                UTILS.Log("경계 구역");
                return 0; // 경계 구역
            }
        }

        /// <summary>
        /// drones에서 droneNumber번째의 드론 오브젝트 리턴
        /// </summary>
        /// <param name="droneNumber">리턴할 드론 번호</param>
        /// <returns></returns>
        public GameObject GetDrone(int droneNumber)
        {
            try
            {
                return Drones[droneNumber];
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 드론 오브젝트 비활성화
        /// </summary>
        /// <param name="droneNumber">비활성화할 드론 번호</param>
        public void SetDroneOff(int droneNumber)
        {
            Drones[droneNumber].SetActive(false);
        }

        /// <summary>
        /// 드론 오브젝트 활성화
        /// </summary>
        /// <param name="droneNumber">활성화할 드론 번호</param>
        public void SetDroneOn(int droneNumber)
        {
            Drones[droneNumber].SetActive(true);
            //Drones[droneNumber].transform.position = InstructorScenarioPage.Instance.GetPointOnCircle(180f + 30f * droneNumber, 800f);
            Drones[droneNumber].transform.position = new Vector3(0, 800, 0); // 2025-04-30 RJH 타겟 오브젝트가 왼쪽에 있을때, 드론을 생성하면 화면 밖에서 생성되는 문제 수정
        }

        /// <summary>
        /// 모든 드론이 비활성화 되었는지 확인
        /// </summary>
        /// <returns>모든 드론이 비활성화 되었으면 true</returns>
        public bool CheckAllDroneOff()
        {
            foreach (var drone in Drones)
            {
                if (drone.GetComponent<DroneSpawnPoint>().drones.Count != 0)
                {
                    return false;
                }
                //if(drone.activeSelf)
                //{
                //    return false;
                //}
            }

            return true;
        }

        #region [RJH][2024.10.14] 더이상 사용하지 않음
        /// <summary>
        /// 훈련생 포지션 세팅
        /// </summary>
        /// <param name="traineeNumber">세팅할 훈련생 번호</param>
        /// <param name="traineePosition">세팅할 훈련생 포지션</param>
        //public void SetTraineePos(int traineeNumber, Vector3 traineePosition)
        //{
        //    if(traineePosition == Vector3.zero)
        //    {
        //        trainees[traineeNumber].SetActive(false);
        //        trainees[traineeNumber].GetComponent<TraineeObj>().SetOffPosition();
        //        return;
        //    }

        //    trainees[traineeNumber].transform.position = traineePosition;
        //}
        #endregion

        /// <summary>
        /// 훈련생 오브젝트 리턴
        /// </summary>
        /// <param name="traineeNumber">리턴할 훈련생 번호</param>
        /// <returns></returns>
        public GameObject GetTrainee(int traineeNumber)
        {
            return trainees[traineeNumber];
        }

        /// <summary>
        /// 훈련생 오브젝트 비활성화
        /// </summary>
        /// <param name="droneNumber">비활성화할 훈련생 번호</param>
        public void SetTraineeOff(int traineeNumber)
        {
            trainees[traineeNumber].SetActive(false);
            trainees[traineeNumber].transform.position = Vector3.zero;
        }

        /// <summary>
        /// 훈련생 오브젝트 활성화
        /// </summary>
        /// <param name="traineeNumber">활성화할 훈련생 번호</param>
        public void SetTraineeOn(int traineeNumber)
        {
            // 훈련생 오브젝트 활성화
            GameObject trainee = trainees[traineeNumber];
            trainee.SetActive(true);

            // 훈련생 위치 설정
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Position", out object position))
            {
                Vector3 pos = (Vector3)position;
                UTILS.Log($"훈련생 번호 : {traineeNumber}_Trainee || 훈련생 위치 : {pos.x}, {pos.y}, {pos.z}");
                trainee.GetComponent<TraineeObj>().SetPos(pos);
            }
            else
            {
                UTILS.LogError($"훈련생 번호 : {traineeNumber} || pos Null");
            }
        }

        public void SetScore(LOGMESSAGE message, int firstindex, int secondindex)
        {
            switch (message)
            {
                case LOGMESSAGE.ATTACK:
                    // [KKH][수정][2024.10.17] - 캔버스를 끄고 켜기 -> 데미지를 받으면 데미지를 입력하고 자동으로 Show Hide 되도록 수정
                    defenseObject.AttackedDefenseObject(10);
                    GamePlay.Instance.isDroneAttackObject = true;
                    //defenseAreaCanvas.SetActive(true);
                    break;
                case LOGMESSAGE.ARRIVE:
                    break;
                case LOGMESSAGE.INACTIVE:
                    SetDroneOff(firstindex);
                    GamePlay.Instance.KillCountToPlayer(firstindex, secondindex, (int)message);
                    break;
                case LOGMESSAGE.SHOOTING:
                    SetTraineeScore(firstindex);
                    //SetDroneScore(secondindex);
                    SetDroneOff(firstindex);
                    GamePlay.Instance.KillCountToPlayer(firstindex, secondindex, (int)message);
                    break;

            }

        }

        public void SetDroneScore(int droneType)
        {
            if (Drones[droneType] == null) return;
            Drones[droneType].GetComponent<DroneSpawnPoint>().SetScore();
        }

        public void SetTraineeScore(int traineeNumber)
        {
            trainees[traineeNumber].GetComponent<TraineeObj>().SetScore();
        }

        public void HideAllDroneDeleteButton()
        {
            foreach (var drone in Drones)
            {
                if (drone == null) continue;
                drone.GetComponent<DroneSpawnPoint>().HideDeleteButton();
            }
        }

        public void ShowAllDroneDeleteButton()
        {
            foreach (var drone in Drones)
            {
                if (drone == null) continue;
                drone.GetComponent<DroneSpawnPoint>().ShowDeleteButton();
            }
        }

        //public void SetDroneMainTarget()
        //{
        //    foreach (var drone in Drones)
        //    {
        //        if (drone == null) continue;
        //        drone.GetComponent<DroneObj>().SetMainTargetPosition(defenseArea.position);
        //        //drone.GetComponent<DroneObj>().SetMainTargetPosition(new Vector3(defenseArea.position.x, 250f, defenseArea.position.z));
        //    }
        //}

        public bool CheckAirPortMap()
        {
            if (currentMissionMap == missionMaps[0])
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /*****************************************************************************************************************************/
        /*****************************************************************************************************************************/
        /*****************************************************************************************************************************/

        public void SendMapData()
        {
            string json = JsonUtility.ToJson(mapData);
            UTILS.Log("==================================================================================");
            UTILS.Log($"SendMapData : {json}");
            UTILS.Log("==================================================================================");
            if (PhotonNetwork.IsMasterClient)
            {
                pv.RPC("ReceiveMapData", RpcTarget.OthersBuffered, json);
            }
        }

        public void SaveMapData()
        {
            if (!PhotonNetwork.IsConnected) return;

            string json = JsonUtility.ToJson(mapData);

            UTILS.Log("==================================================================================");
            UTILS.Log($"Save Mission Map: {mapData.missionMap}");
            UTILS.Log($"Weather: {mapData.weather}");
            UTILS.Log($"Time Zone: {mapData.timeZone}");
            UTILS.Log($"Is Suicide Drone: {mapData.isSuicideDrone}");
            UTILS.Log($"Is Attack Drone: {mapData.isAttackDrone}");
            UTILS.Log($"Is Scout Drone: {mapData.isScoutDrone}");
            UTILS.Log($"Is Jammer Drone: {mapData.isJammerDrone}");
            UTILS.Log($"Json Result: {json}");
            UTILS.Log("==================================================================================");

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonManager_.Inst.SetRoomCustomProerty("MapDataProerty", json);

                //PhotonManager.instance.SetLocalPlayerCustomProperty("MapDataProerty", json);
            }
        }

        [PunRPC]
        public void ReceiveMapData(string json)
        {
            mapData = JsonUtility.FromJson<MapData>(json);

            SetMap();
        }

        public void LoadMapData()
        {
            if (!PhotonNetwork.IsConnected || PhotonNetwork.IsMasterClient) return;

            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("MapDataProerty", out object json))
            {
                string jsonString = json as string;
                if (!string.IsNullOrEmpty(jsonString))
                {
                    UTILS.Log(jsonString);
                    mapData = JsonUtility.FromJson<MapData>(jsonString);
                    SetMap();
                }
                else
                {
                    UTILS.Log("json is null");
                }
            }
        }

        public void SetMap()
        {
            // 받은 데이터 처리
            UTILS.Log("==================================================================================");
            UTILS.Log($"Load Mission Map: {mapData.missionMap}");
            UTILS.Log($"Weather: {mapData.weather}");
            UTILS.Log($"Time Zone: {mapData.timeZone}");
            UTILS.Log($"Is Suicide Drone: {mapData.isSuicideDrone}");
            UTILS.Log($"Is Attack Drone: {mapData.isAttackDrone}");
            UTILS.Log($"Is Scout Drone: {mapData.isScoutDrone}");
            UTILS.Log($"Is Jammer Drone: {mapData.isJammerDrone}");
            UTILS.Log("==================================================================================");

            // mapData를 사용하여 필요한 작업 수행
            SetMapOptions(mapData.missionMap, mapData.weather, mapData.timeZone);
            EnableMapOption();
            SetOnMap();
            if (mapData.isSuicideDrone)
            {
                SetDroneOn(0);
            }

            if (mapData.isAttackDrone)
            {
                SetDroneOn(1);
            }

            if (mapData.isScoutDrone)
            {
                SetDroneOn(2);
            }

            if (mapData.isJammerDrone)
            {
                SetDroneOn(3);
            }
        }
    }
}


