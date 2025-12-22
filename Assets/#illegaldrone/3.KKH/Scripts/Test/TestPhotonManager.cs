using KKH;
using Photon.Pun;
using Photon.Realtime;

public class TestPhotonManager : MonoBehaviourPunCallbacks
{

    private TestPhotonManager instance;
    public TestPhotonManager Inst => instance;

    public ObjectPhotonSerializeView objectPhotonSerializeView;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;            
        }
    }

    void Start()
    {
        //PhotonNetwork.AutomaticallySyncScene = true;

        // 1. OnPhotonSerializeView 호출빈도
        PhotonNetwork.SerializationRate = 60;

        // 2. Rpc(원격 프로시저 호출)호출빈도 //단발성 원할 때 한번
        PhotonNetwork.SendRate = 60;

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        RoomOptions roomOptions = new RoomOptions
        {
            IsOpen = true,
            IsVisible = true,
            MaxPlayers = 5
        };
        PhotonNetwork.JoinOrCreateRoom("RoomName", roomOptions, null); // 이미 진행 중인 룸에 참여
    }

    public override void OnJoinedRoom()
    {
        UTILS.Log("포톤 연결 성공");
        //objectPhotonSerializeView.SetObject();
        // 룸에 참여했을 때의 로직
        // 현재 게임 상태를 동기화하는 코드 추가
    }
}
