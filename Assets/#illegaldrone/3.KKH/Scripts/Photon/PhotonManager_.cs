using ExitGames.Client.Photon;
using Illegaldrone;
using Photon.Pun;
using Photon.Realtime;
using RJH.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace KKH
{
    public class PhotonManager_ : MonoBehaviourPunCallbacks
    {
        static private PhotonManager_ instance;
        static public PhotonManager_ Inst => instance;

        public string roomName = string.Empty;
        public int nMaxPlayer = 5;

        public List<RoomInfo> RoomList = new List<RoomInfo>();

        #region Events
        /// <summary> 로비에 입장했을때 실행하는 event </summary>
        public UnityEvent JoinLobbyAction = null;

        /// <summary> 로비에서 룸의 정보가 변경되었을때 실행하는 event </summary>
        public UnityEvent RoomUpdateAction = null;

        /// <summary> 플레이어가 룸에 들어왔을때 event </summary>
        public UnityEvent<Player> PlayerJoinRoomAction = null;

        /// <summary> 플레이어가 룸을 떠났을때 event </summary>
        public UnityEvent<Player> PlayerLeaveRoomAction = null;
        #endregion
        public bool isRejoinRoom = false;
        public GameObject Alert;

        public List<Player> list_inPlayers = new List<Player>();

        private void Awake()
        {

            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
                PhotonNetwork.SerializationRate = 60;
                DontDestroyOnLoad(gameObject);
            }
        }

        private void Start()
        {
            UTILS.Log("포톤 연결 시작");
            PhotonNetwork.ConnectUsingSettings();

        }

        private void Update()
        {
#if UNITY_EDITOR
            if (!PhotonNetwork.IsConnected)
            {
                Alert.SetActive(true);
            }
            else
            {
                Alert.SetActive(false);
            }
#endif
        }

        public override void OnEnable()
        {
            base.OnEnable();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnApplicationQuit()
        {
            LeaveLobby();
            PhotonNetwork.Disconnect();
        }

        #region 생성 함수
        /// <summary>
        /// 로비 진입
        /// </summary>
        public void JoinLobby()
        {
            if (!PhotonNetwork.InLobby)
                PhotonNetwork.JoinLobby();
        }

        /// <summary>
        /// 로비 떠나기
        /// </summary>
        public void LeaveLobby()
        {
            if (PhotonNetwork.InRoom)
            {
                LeaveRoom();
            }
            PhotonNetwork.LeaveLobby();
        }

        /// <summary>
        /// 방 진입
        /// </summary>
        /// <param name="isCreate">생성 유무</param>
        public void JoinRoom(bool isCreate)
        {
            if (isCreate)
            {
                PhotonNetwork.JoinOrCreateRoom(
                    roomName,
                    new RoomOptions
                    {
                        IsOpen = true,
                        IsVisible = true,
                        MaxPlayers = nMaxPlayer,
                        //2025-04-17 RJH 훈련인원 모드설정 추가========================
                        CustomRoomProperties = new Hashtable { { "IsSoloMode", false } },
                        CustomRoomPropertiesForLobby = new string[] { "IsSoloMode" }
                        // ============================================================
                    },
                    null);
            }
            else
            {
                if(CheckFullCapacity())
                {
                    // 2025-04-17 로그인 이후 방 참여까지 시간이 있어서 룸의 모드와 참여 인원 다시 확인 필요
                    LogOut();
                    return;
                }    
                PhotonNetwork.JoinRoom(roomName);
            }
        }

        /// <summary>
        /// 현재 방의 모드와 참여가능 인원이 끝난는지 확인
        /// </summary>
        /// <returns>솔로 모드이고 이미 인원이 2명 이상이라면 참</returns>
        public bool CheckFullCapacity()
        {
            // 2025-04-17 RJH 입장하기전 룸에 모드가 뭔지 확인 ======
            foreach (RoomInfo roomInfo in RoomList)
            {
                if (roomInfo.Name.Equals(roomName))
                {
                    if (roomInfo.CustomProperties.ContainsKey("IsSoloMode"))
                    {
                        if ((bool)roomInfo.CustomProperties["IsSoloMode"] && roomInfo.PlayerCount >= 2)
                            return true;
                    }
                }
            }
            return false;
            // ======================================================
        }

        /// <summary>
        /// 방 떠나기
        /// </summary>
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        #region 프로퍼티 저장
        public bool SetPlayerCustomProperty(string key, object value, Player player = null)
        {
           
            try
            {
                if (player == null)
                {
                    player = PhotonNetwork.LocalPlayer;
                }

                //Hashtable hash = player.CustomProperties;
                Hashtable hash = new Hashtable();
                
                if (hash.ContainsKey(key))
                {
                    hash[key] = value;
                }
                else
                {
                    hash.Add(key, value);
                }

                player.SetCustomProperties(hash);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SetRoomCustomProerty(string key, object value)
        {
            try
            {
                if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
                {
                    Hashtable hash = PhotonNetwork.CurrentRoom.CustomProperties;

                    if (hash.ContainsKey(key))
                    {
                        hash[key] = value;
                    }
                    else
                    {
                        hash.Add(key, value);
                    }

                    if (PhotonNetwork.CurrentRoom.SetCustomProperties(hash))
                    {
                        UTILS.Log($"SetCustomProperties Done : {PhotonNetwork.CurrentRoom.CustomProperties[key]}");
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 씬 관련
        public void SceneLoad(string _sceneName)
        {
            PhotonNetwork.LoadLevel(_sceneName);
            UTILS.Log(_sceneName + " 씬 로드");
        }
        #endregion

        #region 로그인 관련
        public void LoginSuccess(int userGroup)
        {
            //[2025-04-09][RJH] 처음 시작했을때 ConnectUsingSettings() 호출
            //PhotonNetwork.ConnectUsingSettings();
            //PhotonUIManager.Instance.NextUI();
            if (userGroup == 2) // 훈련생
            {
                PhotonNetwork.AutomaticallySyncScene = false;
                SceneLoad("01_2.Home_Trainee");
            }
            else // 교관, 최고관리자
            {
                PhotonNetwork.AutomaticallySyncScene = false;
                SceneLoad("01_1.Home_instructor");
            }

        }

        public void LogOut()
        {
            UTILS.Log("로그아웃");
            PhotonNetwork.Disconnect();
            GameManager.instance.userInfo.Clear();

            //SceneLoad("00.Login");
            GameManager.instance.reason = DISCONNECTEDREASON.LOGOUT;
        }

        public void DupicateLogin()
        {
            PhotonNetwork.Disconnect();
            GameManager.instance.userInfo.Clear();

            //SceneLoad("00.Login");
            GameManager.instance.reason = DISCONNECTEDREASON.DUPLICATELOGIN;
        }

        public bool CheckRoom()
        {
            JoinLobby();

            bool isFind = false;
            foreach (RoomInfo roomInfo in RoomList)
            {
                if (roomInfo.Name.Equals(roomName))
                {
                    isFind = true;
                    break;
                }
            }
            return isFind;
        }

        #endregion

        #region 플레이어 검색
        public List<Player> GetPlayerList()
        {
            return PhotonNetwork.PlayerListOthers.ToList();
        }

        public Player GetLastPlayer()
        {
            Player[] players = PhotonNetwork.PlayerListOthers;
            return players[players.Length - 1];
        }

        public Player GetSelectPlayer(int actorNum)
        {
            foreach (Player player in PhotonNetwork.PlayerListOthers)
            {
                //if (player.ActorNumber == actorNum)
                //{
                //    return player;
                //}
                // 2025-04-28 RJH CustomProperties["TraineeNumber"] 사용
                if ((int)player.CustomProperties["TraineeNumber"] == actorNum)
                {
                    return player;
                }
            }

            return null;
        }

        public Player GetSelectPlayer(string userID)
        {
            foreach (Player player in PhotonNetwork.PlayerListOthers)
            {
                Hashtable ht = player.CustomProperties;
                UserProperties userProperties = null;
                if (ht.ContainsKey("UserProperties"))
                {
                    userProperties = JsonUtility.FromJson<UserProperties>(ht["UserProperties"].ToString());
                }

                if (userProperties != null && userProperties.id == userID)
                {
                    return player;
                }
            }

            return null;
        }
        #endregion

        public void SetPlayerName(string name)
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = name;
            }
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name.Equals("01_1.Home_instructor") || scene.name.Equals("01_2.Home_Trainee"))
            {
                // 관리자면 생성, 훈련생이면 입장만
                // [2025-04-09][RJH] 룸안이 아닐때만 실행 조건 추가
                if (!PhotonNetwork.InRoom)
                {
                    JoinRoom(GameManager.instance.isInstructor);
                }
            }
            else if (scene.name.Equals("00.Login"))
            {
                if (GameManager.instance.isInstructor)
                {
                    list_inPlayers.Clear();
                }
            }
        }
        #endregion

        #region 포톤 함수

        public override void OnDisconnected(DisconnectCause cause)
        {
            UTILS.Log($"연결 끊어짐 : {cause.ToString()}");
            if (cause.ToString() == "DisconnectByClientLogic")
            {
                SceneLoad("00.Login");
                PhotonNetwork.LocalPlayer.CustomProperties.Clear();
            }
            UTILS.Log("포톤 연결 재시작");
            PhotonNetwork.Reconnect();
        }

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            UTILS.Log($"포톤 서버 연결 완료");
            JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            base.OnJoinedLobby();
            UTILS.Log($"로비 진입 성공");
            if(isRejoinRoom)
            {
                JoinRoom(GameManager.instance.isInstructor);
                isRejoinRoom = false;
            }
        }

        public override void OnLeftLobby()
        {
            base.OnLeftLobby();
            UTILS.Log($"로비 떠나기 성공");
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            base.OnCreateRoomFailed(returnCode, message);
            UTILS.Log($"{PhotonNetwork.CurrentRoom.Name} 방 생성 실패 - 에러 코드 : {returnCode}, 실패 이유 : {message}");
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            base.OnCreateRoomFailed(returnCode, message);
            UTILS.LogError($"{PhotonNetwork.CurrentRoom.Name} 방 진입 실패 - 에러 코드 : {returnCode}, 실패 이유 : {message}");
        }

        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();
            UTILS.Log($"{PhotonNetwork.CurrentRoom.Name} 방 생성 성공");
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            UTILS.Log($"{PhotonNetwork.CurrentRoom.Name} 방 진입 성공");
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            UTILS.Log($"로비 떠나기 성공");
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            // 방 목록 업데이트
            RoomList = roomList;
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            PlayerJoinRoomAction?.Invoke(newPlayer);
            // [RJH][추가][2024.9.9]
            // 중복 로그인 체크 여기서 실행 
            string localUserInfo = PhotonNetwork.LocalPlayer.CustomProperties["UserProperties"].ToString();
            string newUserInfo = newPlayer.CustomProperties["UserProperties"].ToString();
            if (localUserInfo == newUserInfo)
            {
                UTILS.Log("중복입니다.");
                DupicateLogin();
                return;
            }

            list_inPlayers.Add(newPlayer);
            UTILS.Log($"{newPlayer.ActorNumber} | {newPlayer.NickName} 입장");
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            //[RJH][수정] 방을 나간 플레이어가 교관이면 모든 훈련생 로그아웃
            Hashtable ht = otherPlayer.CustomProperties;
            UserProperties userProperties = null;
            if (ht.ContainsKey("UserProperties"))
            {
                UTILS.Log("유저 정보 있음");
                userProperties = JsonUtility.FromJson<UserProperties>(ht["UserProperties"].ToString());
                if (userProperties.userGroup != MySQL.UserGroup.Trainee)
                {
                    UTILS.Log("교관");
                    LogOut();
                    return;
                }
                UTILS.Log("훈련생");
            }
            //[RJH][수정] PlayerLeaveRoomAction 실행
            PlayerLeaveRoomAction?.Invoke(otherPlayer);
            list_inPlayers.Remove(otherPlayer);
            UTILS.Log($"{otherPlayer.ActorNumber} | {otherPlayer.NickName} 퇴장");
        }

        /// <summary>
        /// 어떤 플레이어의 Property가 변경되었을때 실행하는 함수
        /// </summary>
        /// <param name="targetPlayer">Property가 변경된 플레이어</param>
        /// <param name="changedProps">변경된 Property</param>
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps.Keys.Contains("PlayerReady"))
            {
                //PlayerJoinRoomAction?.Invoke();
            }

            // [SMW][추가][2024.09.04]
            // 강퇴기능
            if (targetPlayer == PhotonNetwork.LocalPlayer)
            {
                if (changedProps["isKicked"] != null && (bool)changedProps["isKicked"])
                {
                    //PhotonNetwork.LeaveRoom();
                    //[RJH][변경][2024.09.09]
                    //Disconnect로 변경
                    // 프로퍼티 정리
                    PhotonNetwork.Disconnect();
                    //SceneLoad("00.Login");
                    //GameManager.instance.reason = DISCONNECTEDREASON.NONE;
                }
            }
        }
        #endregion
    }
}