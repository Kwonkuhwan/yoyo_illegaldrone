using Illegaldrone;
using KKH;
using RJH;
using RJH.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UltimateReplay;
using UltimateReplay.StatePreparation;
using UltimateReplay.Storage;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SMW
{
    [Serializable]
    public class CustomReplayData : ReplayMetadata
    {
        public int map;
        public int timeZone;
        public int weather;
        public Vector2 defensePos;

        public PlayerReplayData[] players;
    }

    [Serializable]
    public struct PlayerReplayData
    {
        public int identity;        // 리플레이 idendity
        public int CamNumber;
        public string name;
        public string id;
        public int weapon;
        public int area;
    }

    public static class ReplayFile
    {
        public static string Path;
        public static string Name;
        public const ReplayFileType Extension = ReplayFileType.FromExtension; // 확장자 설정
    }

    public class ReplayManager : MonoBehaviour
    {
        private static ReplayManager instance;

        // 싱글톤 인스턴스 접근자
        public static ReplayManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<ReplayManager>();
                }
                return instance;
            }
        }

        protected ReplayStorage storage = new ReplayMemoryStorage();        // 저장소
        protected ReplayRecordOperation op_record;                          // 녹화 작업
        protected ReplayPlaybackOperation op_playback;                      // 리플레이 작업
        public ReplayRecordOperation OpRecord
        {
            get { return op_record; }
        }

        // 녹화 시작 여부
        public bool isRecord = false;

        // 녹화 중 여부
        public bool isRecording
        {
            get { return op_record != null && op_record.IsRecordingOrPaused; }
        }

        // 리플레이 중 여부
        public bool isReplaying
        {
            get { return op_playback != null && op_playback.IsReplayingOrPaused; }
        }

        // UI Controls
        [Serializable]
        public struct UI
        {
            public Button StartRecording;    // 녹화 시작 버튼
            public Toggle StartReplaying;    // 리플레이 시작 버튼
            public Button LiveStreaming;     // 라이브 스트리밍 버튼

            [Header("Replaying")]
            public Slider Seek;              // 리플레이 탐색 슬라이더

            [Header("Replaying")]
            public Toggle Pause;
            public Button Resume;

            [Header("Timer")]
            public TMP_Text CurrentTime;
            public TMP_Text TotalTime;
        }
        [SerializeField] UI controls;

        //string defalut_path = "";
        //[HideInInspector] public string FileName = "test.replay";

        // CustomReplayData
        public CustomReplayData meta = new CustomReplayData();

        // canvas
        public ReplayCanvas replayCanvas;

        public PlayerReplayData[] playerReplayDatas                                                                                            
        {
            get;
            set;
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            // 교관이 아닐경우 리턴
            if (GameManager.instance.isInstructor == false) return;

            // 기본 경로 설정
            // 경로 : Application.persistentDataPath/Replay/유저이름
            if(ReplayFile.Path == "" || ReplayFile.Path == null)
            {
                ReplayFile.Path = $"{Application.persistentDataPath}/Replay/{GameManager.instance.userInfo.id}/";
            }

            Debug.Log(ReplayFile.Path + ReplayFile.Name);
            // 경로가 없을 경우 생성
            if (!Directory.Exists(ReplayFile.Path))
            {
                Directory.CreateDirectory(ReplayFile.Path);
            }

            // UI 이벤트 바인딩
            Bind();
        }

        private void OnDestroy()
        {
            GoLive();
            storage.Dispose();
            storage = null;
        }

        private void OnEnable()
        {
            if(GameManager.instance.isInstructor == true)
            {
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
        }

        private void OnDisable()
        {
            if(GameManager.instance.isInstructor == true)
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Replay 씬일 경우
            if (scene.name == "03.Replay")
            {
                storage = LoadFile(ReplayFile.Path + ReplayFile.Name, ReplayFile.Extension);

                // 저장소를 읽기 작업으로 준비
                storage.Prepare(ReplayStorageAction.Read);

                // Replay Canvas 켜짐
                replayCanvas.gameObject.SetActive(true);
                replayCanvas.init();

                // 메타데이터가 있을 경우
                meta = storage.Metadata as CustomReplayData;
                if (meta != null)
                {
                    UTILS.Log("맵: " + meta.map);
                    UTILS.Log("날씨: " + meta.weather);
                    UTILS.Log("타임존: " + meta.timeZone);
                    UTILS.Log("방어건물 위치: " + meta.defensePos);
                    replayCanvas.SetMap(meta.map);
                    replayCanvas.SetPlayer(meta.players);       // 플레이어 정보

                    MapCanvas.Instance.SetDefenseObj(meta.defensePos);

                    FindObjectOfType<ReplayMap>().Set(meta.map, meta.timeZone, meta.weather);
                }
            }
            else if (scene.name == "02.ScenarioScene")
            {
                transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                Destroy(gameObject);   
            }
        }

        /// <summary>
        /// 녹화/재생 설정
        /// </summary>
        void SetRecord()
        {
            ReplayFile.Name = GamePlay.Instance.StartTime.ToString("yyyy-MM-dd_HH-mm-ss") + ".replay";
            Debug.Log($"녹화설정 완료 : {ReplayFile.Name}");
            RecordToFile(ReplayFile.Name, ReplayFileType.FromExtension);

            // 저장소를 쓰기 작업으로 준비
            storage.Prepare(ReplayStorageAction.Write);

            // 메타데이터 설정
            CustomReplayData _meta = new CustomReplayData();
            _meta.map = MapObjectController.Inst.missionmapIdx;
            _meta.timeZone = MapObjectController.Inst.timezoneIdx;
            _meta.weather = MapObjectController.Inst.weatherIdx;
            _meta.defensePos = MapObjectController.Inst.DefenseAreaPosition;

            // 훈련생 정보 데이터 리플레이에 저장
            // =====================================================
            _meta.players = playerReplayDatas;
            // =====================================================

            storage.Metadata = _meta;
        }

        // UI 이벤트 바인딩
        void Bind()
        {
            if (controls.StartRecording != null) controls.StartRecording.onClick.AddListener(StartRecording);
            //if (controls.StartReplaying != null) controls.StartReplaying.onClick.AddListener(StartReplaying);
            if (controls.StartReplaying != null) controls.StartReplaying.onValueChanged.AddListener((value) => { if (value == true) StartReplaying(); });
            if (controls.LiveStreaming != null) controls.LiveStreaming.onClick.AddListener(GoLive);
            if (controls.Seek != null) controls.Seek.onValueChanged.AddListener(SeekPlayback);
            if (controls.Pause != null) controls.Pause.onValueChanged.AddListener((value) => { if (value == true) PauseReplay(); });
            //if (controls.Pause != null) controls.Pause.onClick.AddListener(PauseReplay);
            if (controls.Resume != null) controls.Resume.onClick.AddListener(ResumeReplay);
        }

        private void Update()
        {
            if (isRecording)
            {
                UTILS.Log("녹화 중");
            }

            if (isReplaying)
            {
                if (controls.Seek != null)
                {
                    controls.Seek.SetValueWithoutNotify(op_playback.PlaybackTimeNormalized);
                }

                if(controls.CurrentTime != null)
                {
                    controls.CurrentTime.text = op_playback.PlaybackTime.ToString("00:00");
                }

                if(controls.TotalTime != null)
                {
                    controls.TotalTime.text = "/" + op_playback.Duration.ToString("00:00");
                }
                UTILS.Log("리플레이 중");
            }
        }

        #region 녹화

        /// <summary>
        /// 녹화한 데이터를 파일로 저장
        /// </summary>
        /// <param name="fileName"> 파일 이름 </param>
        /// <param name="type"> 파일 형식 </param>
        void RecordToFile(string fileName, ReplayFileType type)
        {
            try
            {
                // 기본 경로 + fileName
                string path = ReplayFile.Path + fileName;
                storage = ReplayFileStorage.FromFile(path, type, true);
                UTILS.Log("파일경로: " + path);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        /// <summary>
        /// 녹화된 파일 불러오기
        /// </summary>
        /// <param name="path"> 파일 경로 </param>
        /// <param name="type"> 파일 형식 </param>
        /// <returns></returns>
        ReplayStorage LoadFile(string path, ReplayFileType type)
        {
            ReplayStorage _storage = new ReplayMemoryStorage();

            try
            {
                if (ReplayFileStorage.IsReplayFile(path))
                {
                    _storage = ReplayFileStorage.ReadFileCompletely(path, type);
                    UTILS.Log("리플레이 파일 불러오기 성공");
                }
                else
                {
                    UTILS.LogError("리플레이 파일이 아닙니다: " + path);
                }
            }
            catch (Exception e)
            {
                UTILS.LogError("파일을 불러오는 중 오류 발생: " + e.Message);
            }

            return _storage;
        }

        /// <summary>
        /// defalut_path에 저장된 파일들을 불러오기
        /// </summary>
        public void LoadAllFiles()
        {
            try
            {
                string[] files = Directory.GetFiles(ReplayFile.Path);
                foreach (string file in files)
                {
                    //if (ReplayFileStorage.IsReplayFile(file))
                    //{
                    //    ReplayStorage storage = LoadFile(file, ReplayFileType.FromExtension);
                    //    // 필요한 추가 작업 수행
                    //}
                    UTILS.Log(file);
                }
            }
            catch (Exception e)
            {
                UTILS.LogError("파일을 불러오는 중 오류 발생: " + e.Message);
            }
        }

        // 녹화 시작
        public void StartRecording()
        {
            if (isRecording) return;

            SetRecord();

            // 리플레이 종료
            StopReplaying();

            op_record = UltimateReplay.ReplayManager.BeginRecording(storage);
        }

        // 녹화 중지
        public void StopRecording()
        {
            if (op_record != null && op_record.IsDisposed == false)
            {
                op_record.Dispose();
                op_record = null;
            }
        }

        #endregion

        #region 리플레이

        // 리플레이 시작
        void StartReplaying()
        {
            if (isReplaying)
            {
                ResumeReplay();
                return;
            }

            // 녹화 종료
            StopRecording();

            // 리플레이 시작 함수
            op_playback = UltimateReplay.ReplayManager.BeginPlayback(storage);
            op_playback.OnPlaybackEnd.AddListener(OnReplayEnd);

            // 리플레이 속도 1로 조절
            SetPlaybackSpeed(1f);
        }

        // 리플레이 종료
        public void StopReplaying()
        {
            if (op_playback != null && op_playback.IsDisposed == false)
            {
                op_playback.Dispose();
                op_playback = null;
            }
        }

        // 리플레이 시간 조정
        void SeekPlayback(float value)
        {
            if (isReplaying)
            {
                op_playback.SeekPlaybackNormalized(value);
            }

            if (controls.Seek != null) controls.Seek.SetValueWithoutNotify(value);
        }

        /// <summary>
        /// 리플레이 속도 조절
        /// </summary>
        void SetPlaybackSpeed(float value)
        {
            if (isReplaying)
            {
                op_playback.PlaybackTimeScale = value;
            }
        }

        /// <summary>
        /// 리플레이 일시정지
        /// </summary>
        void PauseReplay()
        {
            if (isReplaying)
            {
                op_playback.PausePlayback();
            }
        }

        /// <summary>
        /// 리플레이 재개
        /// </summary>
        void ResumeReplay()
        {
            if (isReplaying)
            {
                op_playback.ResumePlayback();
            }
        }

        // 리플레이 종료 시 호출
        void OnReplayEnd()
        {
            op_playback = null;
            GoLive();

            UTILS.Log("리플레이 종료");
        }

        #endregion

        // 라이브 모드로 전환
        void GoLive()
        {
            StopRecording();
            StopReplaying();
            UTILS.Log("라이브 온");
        }
    }
}