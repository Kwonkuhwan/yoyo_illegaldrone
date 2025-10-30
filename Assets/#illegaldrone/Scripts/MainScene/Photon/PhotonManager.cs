using ExitGames.Client.Photon;
using Illegaldrone;
using KKH;
using OVRSimpleJSON;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace RJH.Photon
{
    public class PhotonManager : MonoBehaviourPunCallbacks
    {
        #region Singleton
        private static PhotonManager _instance;
        public static PhotonManager instance { get { Init(); return _instance; } }

        private static void Init()
        {
            if (_instance == null)
            {
                PhotonManager instance = GameObject.FindObjectOfType<PhotonManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("PhotonManager");
                    instance = go.AddComponent<PhotonManager>();
                }

                _instance = instance;
                DontDestroyOnLoad(_instance);
                _instance.OnInit();
            }
        }

        private void Start()
        {
            //if (transform.parent != null)
            //    transform.SetParent(null);

            //if (_instance != null && _instance != this)
            //{
            //    DestroyImmediate(gameObject);
            //    return;
            //}
            //else if (_instance == null)
            //{
            //    _instance = this;
            //    DontDestroyOnLoad(_instance);
            //    OnInit();
            //}
        }
        #endregion

        private Dictionary<string, RoomInfo> roomDict = new Dictionary<string, RoomInfo>();
        public int maxPlayer = 5;
        #region[RJH][제거][2024.09.24] GameManager에서 관리, 로그인씬 팝업 사용
        //public DISCONNECTEDREASON reason = DISCONNECTEDREASON.NONE;
        //[SerializeField] GameObject duplicatePopup;
        #endregion
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

        /// <summary> 
        /// PhotonManager가 처음 시작될때 실행하는 함수 로그인 UI를 사용하지 않으면 이 단계에서 포톤 서버 연결 
        /// </summary>
        private void OnInit()
        {

            //if (PhotonUIManager.Instance.isLoginEnable)
                //return;

            //PhotonNetwork.ConnectUsingSettings();

        }

        /// <summary> 로그인 성공시 실행 함수 서버 연결 실행, 다음 UI로 전환 </summary>
        public void LoginSuccess(int userGroup)
        {
            PhotonNetwork.ConnectUsingSettings();
            //PhotonUIManager.Instance.NextUI();
            if (userGroup == 2) // 훈련생
            {
                PhotonNetwork.AutomaticallySyncScene = true;
                SceneLoad("01_2.Home_Trainee");
            }
            else // 교관, 최고관리자
            {
                SceneLoad("01_1.Home_instructor");
            }

        }

        /// <summary>
        /// 서버 연결을 끊어버리는 함수 (로그아웃)
        /// </summary>
        public void LogOut()
        {
            PhotonNetwork.Disconnect();
            GameManager.instance.userInfo.Clear();

            SceneLoad("00.Login");
            GameManager.instance.reason = DISCONNECTEDREASON.LOGOUT;
        }

        /// <summary>
        /// Lobby UI가 있는 씬에서 다른 씬으로 넘어갈때 RoomInfo를 저장한 dictionary 클리어 
        /// </summary>
        public void ClearRoomDict()
        {
            roomDict.Clear();
        }

        /// <summary> 로비 입장를 시도하는 함수 </summary>
        public void JoinLobby()
        {
            PhotonNetwork.JoinLobby();
        }

        /// <summary> 로비 퇴장을 시도하는 함수 </summary>
        public void LeaveLobby()
        {
            PhotonNetwork.LeaveLobby();
        }

        /// <summary> 플레이어의 준비 상태를 로컬 플레이어 커스텀 프로퍼티에 저장 </summary>
        public void SetPlayerReady()
        {
            bool isReady = (bool)PhotonNetwork.LocalPlayer.CustomProperties["PlayerReady"];
            PhotonManager_.Inst.SetRoomCustomProerty("PlayerReady", !isReady);

            //if (isReady)
            //    PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "PlayerReady", false } });
            //else
            //    PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "PlayerReady", true } });
        }

        /// <summary> 방을 생성하는 함수 </summary>
        /// <param name="_roomName">생성하는 방의 이름</param>
        public void CreateRoom(string _roomName)
        {
            Debug.Log("방 생성");
            RoomOptions roomOptions = new RoomOptions
            {
                IsOpen = true,
                IsVisible = true,
                MaxPlayers = maxPlayer
            };

            PhotonNetwork.CreateRoom(_roomName, roomOptions, null);
        }

        /// <summary> 플레이어 이름 정하는 함수 </summary>
        /// <param name="_name">플레이어 이름</param>
        public void SetPlayerName(string _name)
        {
            PhotonNetwork.NickName = _name;
        }

        /// <summary> 플레이어 오브젝트 생성, 플레이어 오브젝트이거나 플레이어가 생성한 오브젝트만 생성하도록 사용,
        /// 룸을 나가면 사라짐 </summary>
        /// <param name="_objectName">생성할 오브젝트의 이름</param>
        /// <param name="_spawnPosition">생성될 좌표</param>
        /// <returns></returns>
        public GameObject CreatePlayer(string _objectName, Vector3 _spawnPosition)
        {
            return PhotonNetwork.Instantiate(_objectName, _spawnPosition, Quaternion.identity);
        }

        /// <summary> 룸 오브젝트 생성, 룸 오브젝트만 생성하도록 상용, 룸이 남아있는한 계속 남아있음 </summary>
        /// <param name="_objectName">생성할 오브젝트의 이름</param>
        /// <param name="_spawnPosition">생성될 좌표</param>
        /// <returns></returns>
        public GameObject CreateRoomObject(string _objectName, Vector3 _spawnPosition)
        {
            return PhotonNetwork.InstantiateRoomObject(_objectName, _spawnPosition, Quaternion.identity);
        }

        public void SetLocalPlayerCustomProperty(string key, string value)
        {
            Hashtable hash = new Hashtable { { key, value } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }

        /// <summary> 씬을 불러오는 함수 (마스터 클라이언트에서만 실행, 로컬 클라이언트에서는 동기화) </summary>
        /// <param name="_sceneName"></param>
        public void SceneLoad(string _sceneName)
        {
            PhotonNetwork.LoadLevel(_sceneName);
        }

        /// <summary> 룸에 참여를 시도하는 함수 </summary>
        /// <param name="_roomName"> 참여할 룸의 이름</param>
        public void JoinRoom(string _roomName)
        {
            PhotonNetwork.JoinRoom(_roomName, null);
        }

        /// <summary> 룸 퇴장을 시도하는 함수 </summary>
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        /// <summary>
        /// 마스터 클라이언트인지 아닌지의 값을 반환하는 함수
        /// </summary>
        /// <returns></returns>
        public bool IsMasterClent()
        {
            return PhotonNetwork.IsMasterClient;
        }

        /// <summary>
        /// 룸 리스트를 체크하는 함수
        /// </summary>
        /// <returns>생성되어 있는 RoomInfo 리스트를 반환</returns>
        public List<RoomInfo> GetRoomList()
        {
            return roomDict.Values.ToList();
        }

        /// <summary>
        /// 룸에 참가하고 있는 Player 리스트를 반환하는 함수(실행하는 본인 제외)
        /// </summary>
        /// <returns>현재 룸에 있는 Player 리스트 반환(실행하는 본인 제외)</returns>
        public List<Player> GetPlayerList()
        {
            List<Player> list = PhotonNetwork.PlayerListOthers.ToList();
            return list;
        }

        public Player GetLastPlayer()
        {
            Player[] players = PhotonNetwork.PlayerListOthers;
            return players[players.Length - 1];
        }

        /// <summary> 서버 입장을 성공했을때 실행되는 함수, 룸에 입장했을때 마스터 클라이언트와 씬이 동기화되는 AutomaticallySyncScene를 true로 전환 </summary>
        public override void OnConnectedToMaster()
        {
            //PhotonNetwork.AutomaticallySyncScene = true;

            //if (PhotonUIManager.Instance.isLobbyEnable == false) // 로비가 없는 상황이면 실행
            PhotonNetwork.JoinLobby();
            //if (PhotonUIManager.Instance.isLoginEnable) // 로그인이 있는 상황이면 실행

        }

        /// <summary>
        /// 포톤 연결이 끊어지면 실행되는 함수, 로그아웃 버튼을 눌러 서버 연결이 끊어져서 실행된 경우, 이전 UI로 이동
        /// </summary>
        /// <param name="cause"> 연결이 끊어진 원인</param>
        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log(cause);
            if (cause.ToString() == "DisconnectByClientLogic")
            {
                //[RJH][추가][2024.09.24] 로직으로 인한 Disconnect는 로그인씬으로 이동
                SceneLoad("00.Login");
                #region [RJH][제거][2024.09.24] 로그인씬에서 팝업관리

                //switch (GameManager.instance.reason)
                //{
                //    case DISCONNECTEDREASON.LOGOUT:
                //        break;
                //    case DISCONNECTEDREASON.DUPLICATELOGIN:
                //        Instantiate(duplicatePopup);
                //        break;
                //    case DISCONNECTEDREASON.NONE:
                //        break;
                //}
                #endregion
                // [RJH][추가][2025.04.03] 강퇴되고 커스텀 프로퍼티남아있는거 삭제
                PhotonNetwork.LocalPlayer.CustomProperties.Clear();
            }
            
        }

        /// <summary>
        /// 로비에 연결되었을 경우 실행되는 함수, 로비 UI가 없는 상황이라면 바로 룸을 생성하고 참여
        /// </summary>
        public override void OnJoinedLobby()
        {

            //if (PhotonUIManager.Instance.isLobbyEnable == false)
            {

                RoomOptions roomOptions = new RoomOptions
                {
                    IsOpen = true,
                    IsVisible = true,
                    MaxPlayers = maxPlayer,
                    EmptyRoomTtl = 0
                };
                PhotonNetwork.JoinOrCreateRoom("OneRoom", roomOptions, null);
            }

        }

        /// <summary>
        /// 룸이 생성, 삭제 혹은 어떤 변화가 생겼을때 실행되는 함수 
        /// 처음에는 생성된 모든 RoomInfo를 가져오지만 그 다음에는 변화가 있는 RoomInfo만 가져오므로 roomList를 따로 저장해 두는 것이 좋다.
        /// </summary>
        /// <param name="roomList"> 변화가 있는 RoomInfo리스트 </param>
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.Log("실행");
            List<string> roomName = new List<string>();
            RoomInfo roomInfo = null;

            foreach (var info in roomList)
            {
                if (info.RemovedFromList == true)
                {
                    roomDict.TryGetValue(info.Name, out roomInfo);
                    roomDict.Remove(info.Name);
                }
                else
                {

                    if (roomDict.ContainsKey(info.Name) == false)
                    {
                        roomDict.Add(info.Name, info);
                    }
                    else
                    {
                        roomDict[info.Name] = info;
                    }
                }
            }

            foreach (var room in roomDict.Keys)
            {
                roomName.Add(room);
            }
            RoomUpdateAction?.Invoke();
        }

        /// <summary>
        /// 룸 접속을 성공했을때 실행되는 함수 
        /// </summary>
        public override void OnJoinedRoom()
        {
            UTILS.Log("룸 입장");

            Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
            if (hash.ContainsKey("PlayerReady"))
            {
                hash["PlayerReady"] = false;
            }
            else
            {
                hash.Add("PlayerReady", false);
            }
            PhotonManager_.Inst.SetPlayerCustomProperty("PlayerReady", false); // 플레이어가 플레이할 준비가 되었는지 확인하기 위한 커스텀프로퍼티

            //PhotonUIManager.Instance.NextUI(); // 다음 UI로 이동(룸 UI)
            //PlayerJoinRoomAction?.Invoke(); // 플레이어가 룸에 들어왔을때 실행할 이벤트
        }

        /// <summary>
        /// 룸 퇴장을 성골했을때 실행되는 함수
        /// </summary>
        public override void OnLeftRoom()
        {
            //[RJH][주석처리][2024.9.9]
            // 로비가 없어서 사용 안함
            //if (PhotonUIManager.Instance.isLobbyEnable)
            //    JoinLobbyAction?.Invoke();
            //PhotonUIManager.Instance.PreviousUI();
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
                    PhotonNetwork.Disconnect();
                    SceneLoad("00.Login");
                    //GameManager.instance.reason = DISCONNECTEDREASON.NONE;
                }
            }
        }

        /// <summary>
        /// 다른 플레이어가 룸에 들어왔을때 실행되는 함수
        /// </summary>
        /// <param name="newPlayer"> 룸에 새로 들어온 플레이어 </param>
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
                PhotonNetwork.Disconnect();
                GameManager.instance.reason = DISCONNECTEDREASON.DUPLICATELOGIN;
            }
        }

        /// <summary>
        /// 다른 플레이어가 룸을 나갔을때 실행되는 함수
        /// </summary>
        /// <param name="otherPlayer">방을 나간 플레이어</param>
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            PlayerLeaveRoomAction?.Invoke(otherPlayer);
        }

        
    }

    #region[RJH][삭제][2024.09.24] GameManager에서 관리
    //public enum DISCONNECTEDREASON
    //{
    //    NONE,
    //    LOGOUT,
    //    DUPLICATELOGIN
    //}
    #endregion
}


