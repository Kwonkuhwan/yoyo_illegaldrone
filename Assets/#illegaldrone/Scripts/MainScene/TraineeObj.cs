using Illegaldrone;
using KKH;
using Photon.Pun;
using Photon.Realtime;
using RJH.UI;
using SMW;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UltimateReplay;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RJH
{
    public class TraineeObj : MonoBehaviourPun
    {
        static private TraineeObj instance;
        static public TraineeObj Inst => instance;

        public PhotonView pv;

        public int TraineeNumber;
        public TextMeshProUGUI nameText;
        public GameObject scoreObject;

        //public GameObject playerPrefab;
        public GameObject playerCharacter;
        public GameObject remoteCharacter;
        public Camera playerCam;
        public GameObject traineeCam;

        private Coroutine currentCoroutin;

        public List<GameObject> go_Weathers;
        // 2025-05-09 RJH 훈련생 아이콘 무조건 한번은 드래그 해야함
        public bool isTraineeSetPos = false;
        private void Awake()
        {
            instance = this;

            pv = GetComponent<PhotonView>();
            if (playerCharacter != null && remoteCharacter == null && !GameManager.instance.isInstructor)
            {
                //playerCharacter = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
                remoteCharacter = PhotonNetwork.Instantiate(GameManager.instance.RemotePlayerObjectName, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
                BNG.NetworkPlayer np = remoteCharacter.GetComponent<BNG.NetworkPlayer>();
                if (np)
                {
                    np.SetNetWorkPlayerName(TraineeNumber, "_MyRemotePlayer");
                    //np.transform.name = $"{TraineeNumber}_MyRemotePlayer";
                    np.AssignPlayerObjects();
                }
            }
            else
            {
                playerCharacter.SetActive(false);
            }

            if (traineeCam == null)
            {
                traineeCam = transform.Find("TraineeCam").gameObject;
            }
        }

        private void Update()
        {
            if (GameManager.instance.isInstructor && remoteCharacter == null)
            {
                remoteCharacter = GameObject.Find($"{TraineeNumber}_MyRemotePlayer");
                if (remoteCharacter != null)
                {
                    Transform headTrans = remoteCharacter.transform.Find("Head");
                    UTILS.Log($"Find {headTrans.name}");
                    traineeCam.transform.parent = headTrans;
                    traineeCam.transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;
                    transform.localScale = Vector3.one;

                    traineeCam.SetActive(true);

                    // SMW 추가
                    // 리모트 플레이어 총 생성
                    // ================================================================
                    StartCoroutine(PrepareRecording(remoteCharacter));
                    transform.GetComponent<PlayerStartEquipment>().PrePareEquipment();

                    // 플레이어의 고유번호 저장
                    SMW.ReplayManager.Instance.playerReplayDatas[TraineeNumber].identity = remoteCharacter.GetComponent<ReplayObject>().ReplayIdentity.ID;
                    // ================================================================
                }
            }
        }

        /// <summary>
        /// 녹화 시작 후 캐릭터 리플레이에 저장
        /// </summary>
        IEnumerator PrepareRecording(GameObject remotePlayer)
        {
            while(true)
            {
                if(GamePlay.Instance.isStart)
                {
                    UltimateReplay.ReplayManager.AddReplayObjectToRecordScenes(remoteCharacter);
                    Debug.Log(remotePlayer.name + ": 리플레이에 저장");
                    break;
                }
                yield return null;
            }
            yield return null;
        }

        public void SetScore()
        {
            scoreObject.SetActive(true);

            if (currentCoroutin != null)
            {
                StopCoroutine(currentCoroutin);
            }
            currentCoroutin = StartCoroutine(HideScore());
        }

        public void SetName(Player player)
        {
            string userInfo = player.CustomProperties["UserProperties"].ToString();
            UserProperties userProperties = JsonUtility.FromJson<UserProperties>(userInfo);
            nameText.text = userProperties.userName;
        }

        public void SetPos(Vector3 pos)
        {
            if (playerCharacter != null)
            {
                //Vector3 characterPos = playerCharacter.transform.position;
                //characterPos = new Vector3(0, pos.y, 0);

                //playerCharacter.transform.position = pos;
                pos += new Vector3(0, 2, 0);
                playerCharacter.transform.position = pos;
                
            }
            if (!GameManager.instance.isInstructor)
            {
                traineeCam.GetComponent<Camera>().depth = 0;
            }
            //traineeCam.transform.position = pos;
        }

        #region [RJH][2024.10.15] 더이상 사용하지 않음
        //public void SetOffPosition()
        //{
        //    Debug.Log("!!!!!!");
        //    photonView.RPC("SetOffPositionRPC", RpcTarget.Others);
        //}

        //[PunRPC]
        //private void SetOffPositionRPC()
        //{
        //    this.gameObject.SetActive(false);
        //}
        #endregion

        private void GetPhotonOwner()
        {
            photonView.RequestOwnership();
        }

        private IEnumerator HideScore()
        {
            yield return new WaitForSeconds(2);
            scoreObject.SetActive(false);
            currentCoroutin = null;
        }
    }
}


