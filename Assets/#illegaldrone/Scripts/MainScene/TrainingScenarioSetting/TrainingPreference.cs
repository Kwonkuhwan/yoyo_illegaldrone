using ExitGames.Client.Photon;
using KKH;
using KKH.MySQL;
using Photon.Pun;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RJH.UI
{
    public class TrainingPreference : MonoBehaviour
    {
        [SerializeField] private Image frameImage;
        [SerializeField] private Sprite[] frameSprites;

        [Space(10)]

        [SerializeField] private ToggleGroup placeToggleGroup;
        [SerializeField] private ToggleGroup weatherToggleGroup;
        [SerializeField] private ToggleGroup timeZoneGroup;
        [SerializeField] private ToggleGroup difficultyGroup;
        [Space(10)]
        [SerializeField] private MapObjectController mapOptionControl;
        [Space(10)]
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI minMaxText;
        [SerializeField] private Image minMaxTextImage;
        [SerializeField] private Sprite[] minMaxSprites;
        [SerializeField] private Button rightButton;
        [SerializeField] private Button leftButton;
        [Space(10)]
        [SerializeField] private Button nextButton;
        [SerializeField] private Button cancelButton;

        private const int MAX_MINUTE = 20;
        private const int MIN_MINUTE = 10;

        private int minute;
        public int Limit_Minute => minute;

        private MapType missionMap = MapType.Gwanghwamun;
        public MapType MissionMap => missionMap;
        private Weather weather = Weather.Sunshine;
        public Weather Weather => weather;
        private KKH.MySQL.TimeZone timeZone = KKH.MySQL.TimeZone.Day;
        public KKH.MySQL.TimeZone TimeZone => timeZone;
        private Difficulty difficulty = Difficulty.Easy;
        public Difficulty Difficulty => difficulty;

        private readonly string[] missionMaps = { "Airport", "Gwanghwamun", "Nuclearplant" }; 
        private readonly string[] weathers = { "Sunshine", "Snow", "Rain", "Fog" };
        private readonly string[] timeZones = { "Day", "Night" };
        private readonly string[] difficulties = { "Easy", "Normal" };

        private void Awake()
        {
            minute = 10;

            foreach(var toggle in placeToggleGroup.GetComponentsInChildren<Toggle>())
            {
                toggle.onValueChanged.AddListener(delegate { missionMapChanged(toggle); });
            }

            foreach(var toggle in weatherToggleGroup.GetComponentsInChildren<Toggle>())
            {
                toggle.onValueChanged.AddListener(delegate { WeatherChanged(toggle); });
            }

            foreach(var toggle in timeZoneGroup.GetComponentsInChildren<Toggle>())
            {
                toggle.onValueChanged.AddListener(delegate { TimeZoneChanged(toggle); });
            }

            foreach (var toggle in difficultyGroup.GetComponentsInChildren<Toggle>())
            {
                toggle.onValueChanged.AddListener(delegate { DifficultyChanged(toggle); });
            }

            rightButton.onClick.AddListener(OnRightButton);
            leftButton.onClick.AddListener(OnLeftButton);

            nextButton.onClick.AddListener(OnNext);
            cancelButton.onClick.AddListener(OnCancel);
        }

        /// <summary>
        /// 훈련 제한 시간 감소, 10으로 감소하면 더 이상 감소하지 않고 min 텍스트 출력
        /// </summary>
        private void OnLeftButton()
        {
            if (minute == MIN_MINUTE) return;


            minute -= 1;
            timeText.text = minute.ToString();

            UTILS.Log($"제한시간 : {minute}분 설정 완료.");
            if (minute == MIN_MINUTE)
            {
                leftButton.GetComponent<ValueButtonController>().Check(false);
                minMaxTextImage.gameObject.SetActive(true);
                minMaxTextImage.sprite = minMaxSprites[0];
                minMaxText.text = "min";
                UTILS.Log($"제한시간 : {minute}분 최솟값으로 설정됨.");

            }
            else
            {
                rightButton.GetComponent<ValueButtonController>().Check(true);
                minMaxTextImage.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 훈련 제한 시간 증가, 20으로 증가하면 더 이상 증가하지 않고 max 텍스트 출력, 10으로 감소하면 더 이상 감소하지 않고 min 텍스트 출력
        /// </summary>
        private void OnRightButton()
        {
            if (minute == MAX_MINUTE) return;


            minute += 1;
            timeText.text = minute.ToString();
            UTILS.Log($"제한시간 : {minute}분 설정 완료.");

            if (minute == MAX_MINUTE)
            {
                rightButton.GetComponent<ValueButtonController>().Check(false);
                minMaxTextImage.gameObject.SetActive(true);
                minMaxTextImage.sprite = minMaxSprites[1];
                minMaxText.text = "max";
                timeText.text = "<color=red>" + timeText.text + "</color>";
                UTILS.Log($"제한시간 : {minute}분 최대값으로 설정됨.");
            }
            else
            {
                leftButton.GetComponent<ValueButtonController>().Check(true);
                minMaxTextImage.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 다음 버튼 클릭, 훈련 환경 설정을 완료하고 훈련 시나리오 설정 화면으로 전환
        /// </summary>
        private void OnNext()
        {
            #region 훈련생 로딩 화면
            Hashtable playerProperties = new Hashtable();
            playerProperties["IsScenarioReady"] = true; // 준비 상태를 true로 설정
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
            #endregion

            UTILS.Log($"missionMap = {missionMap}, weather = {weather}, timeZone = {timeZone}, limitTime = {minute}");
            InstructorScenarioPage.Instance.mainTarget = mapOptionControl.SetMainTarget();
            LineController.Instance.mainTarget = mapOptionControl.SetMainTarget();
            InstructorScenarioPage.Instance.trainingScenario.SetActive(true);
            InstructorScenarioPage.Instance.mapObject.SetActive(true);
            InstructorScenarioPage.Instance.SetScenario(); 
            //InstructorScenarioPage.Instance.SetScenarioInfo_Preference(missionMap, weather, timeZone, minute);

            mapOptionControl.SetMapOptions((int)missionMap, (int)weather, (int)timeZone);
            mapOptionControl.EnableMapOption();
            mapOptionControl.SetOnMap();

            InstructorScenarioPage.Instance.SetSuicideDroneButton(); // 2025-04-30 맵 옵션 설정 보다 나중에 진행되야함

            this.gameObject.SetActive(false);
        }

        /// <summary>
        /// 훈련 취소 버튼 클릭, 교관 홈 화면으로 이동
        /// </summary>
        private void OnCancel()
        {
            PhotonManager_.Inst.SceneLoad("01_1.Home_instructor");
        }

        /// <summary>
        /// 훈련 진행 장소 변경, MapSetting으로 변경값 적용
        /// </summary>
        /// <param name="changedToggle">훈련 장소 토글</param>
        private void missionMapChanged(Toggle changedToggle)
        {
            if (changedToggle.isOn)
            {
                string missionMap = changedToggle.name;

                this.missionMap = (MapType)Array.IndexOf(missionMaps, missionMap);
                MapSetting();
            }
        }
        
        /// <summary>
        /// 훈련 날씨 변경, MapSetting으로 변경값 적용
        /// </summary>
        /// <param name="changedToggle">훈련 날씨 토글</param>
        private void WeatherChanged(Toggle changedToggle)
        {
            if (changedToggle.isOn)
            {
                string weather = changedToggle.name;

                this.weather = (Weather)Array.IndexOf(weathers, weather);
                MapSetting();
            }
            
        }

        /// <summary>
        /// 시간대 변경, MapSetting으로 변경값 적용 
        /// </summary>
        /// <param name="changedToggle">훈련 시간대 토글</param>
        private void TimeZoneChanged(Toggle changedToggle)
        {
            if (changedToggle.isOn)
            {
                string timeZone = changedToggle.name;

                this.timeZone = (KKH.MySQL.TimeZone)Array.IndexOf(timeZones, timeZone);
                MapSetting();
            }
            
        }

        private void DifficultyChanged(Toggle changedToggle)
        {
            if(changedToggle.isOn)
            {
                string difficulty = changedToggle.name;

                this.difficulty = (Difficulty)Array.IndexOf(difficulties, difficulty);
                
            }
        }

        private void MapSetting()
        {
            UTILS.Log($"MissionMap : {missionMap}, Weather : {weather}, TimeZone : {timeZone}");
            /* TODO
             * 맵 혹은 맵 이미지를 선택한 설정대로 변경
             */
            frameImage.sprite = frameSprites[(int)missionMap];
            mapOptionControl.SetMapOptions((int)missionMap, (int)weather, (int)timeZone);
            //mapOptionControl.SetMapOption((int)missionMap, (int)weather, (int)timeZone);             
        }
    }
}




