using Illegaldrone;
using KKH;
using Photon.Pun;
using UnityEngine;

public class ObjectPhotonSerializeView : MonoBehaviourPun, IPunObservable
{
    //[SerializeField] private GameObject orgObject;
    //[SerializeField] private GameObject copyObject;

    public PhotonView pv;

    //도착 위치
    Vector3 receivePos;
    //회전되야 하는 값
    Quaternion receiveRot;
    // 보간 속력
    public float lerpSpeed = 100;

    public bool isUseSerializeView = true;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();

        receivePos = transform.position;
        receiveRot = transform.rotation;
    }

    void Update()
    {
        // 만약에 내것이 아니라면 함수를 나가겠다
        // if (photonView.IsMine == false) return;
        // 변경!

        
    }

    private void FixedUpdate()
    {
        if (!isUseSerializeView && PhotonNetwork.IsMasterClient)
        {
            pv.RPC("SetObject", RpcTarget.Others, transform.position, transform.rotation);
        }

        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient && !GameManager.instance.isInstructor)
        {
            //Lerp를 이용해서 목적지, 목적방향까지 이동 및 회전
            transform.position = Vector3.Lerp(transform.position, receivePos, lerpSpeed * Time.fixedDeltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, receiveRot, lerpSpeed * Time.fixedDeltaTime);
        }
        //UTILS.Log($"{gameObject.name} : {receivePos}");
    }


    [PunRPC]
    public void SetObject(Vector3 pos, Quaternion rot)
    {
        try
        {
            receivePos = pos;
            receiveRot = rot;
        }
        catch { }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsConnected && isUseSerializeView) return; // 연결되지 않은 경우 조기 반환

        // 데이터 보내기: 마스터 클라이언트인 경우
        if (stream.IsWriting && PhotonNetwork.IsMasterClient && GameManager.instance.isInstructor)
        {
            // 위치와 회전 정보를 전송
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }

        // 모든 클라이언트가 데이터를 받도록 설정
        if (stream.IsReading && !PhotonNetwork.IsMasterClient && !GameManager.instance.isInstructor)
        {
            // 위치와 회전 정보를 수신
            receivePos = (Vector3)stream.ReceiveNext();
            receiveRot = (Quaternion)stream.ReceiveNext();

            // 수신한 데이터를 사용하여 오브젝트의 위치와 회전 업데이트
            // copyObject.transform.position = receivePos; // 필요에 따라 주석 해제
            // copyObject.transform.rotation = receiveRot; // 필요에 따라 주석 해제
        }
    }
}
