using BNG;
using Illegaldrone;
using KKH;
using Photon.Pun;
using Photon.Realtime;
using RJH;
using System.Collections;
using UltimateReplay;
using UnityEngine;

namespace SMW
{
    public class PlayerStartEquipment : MonoBehaviourPun
    {
        enum 장비
        {
            JammingGun,
            NetGun,
            K2C1
        }

        [SerializeField] GameObject player_VR;
        [SerializeField] Grabber Grabber;
        [SerializeField] 장비 Equipment;
        [SerializeField] ControllerHand HandType;

        TraineeObj traineeObj;
        PhotonView pv;

        GameObject Gun;

        private void Awake()
        {
            if(Grabber == null)
            {
                FindGrabber();
            }

            pv = transform.GetComponent<PhotonView>();
            traineeObj = transform.GetComponent<TraineeObj>();
        }

        public void SetEquipment(int type)
        {
            //2025-04-16 RJH 네트건을, 재밍건으로 변경
            type = type == 1 ? 0 : type;
            Equipment = (장비)type;
        }

        /// <summary>
        /// 교관이 게임 시작을 눌렀을 경우 장비착용하라는 이벤트
        /// </summary>
        public void PrePareEquipment()
        {
            StartCoroutine(StartGameEquipment());
        }

        IEnumerator StartGameEquipment()
        {
            while(true)
            {
                if(GamePlay.Instance.isStart)
                {
                    break;
                }
                yield return null;
            }
            pv.RPC("RPC_SetEquipment", RpcTarget.Others, (int)Equipment);
        }

        [PunRPC]
        public void RPC_SetEquipment(int type)
        {
            Equipment = (장비)type;
            SetEquipment();
        }

        /// <summary>
        /// 장비 타입 정하기
        /// </summary>
        /// <param name="type"> 0 = Rifle, 1 = JammingGun, 2 = NetGun </param>
        public void SetEquipmentType(int type)
        {
            Equipment = (장비)type;
        }

        // 그랩 찾기
        void FindGrabber()
        {
            Grabber[] _grabber = player_VR.transform.GetComponentsInChildren<Grabber>();

            for(int i = 0; i < _grabber.Length; i++)
            {
                // 오른손 or 왼손
                if (_grabber[i].HandSide == HandType)
                {
                    Grabber = _grabber[i];
                }
            }
        }

        // 장비 착용 및 생성
        void SetEquipment()
        {
            if (GameManager.instance.isInstructor) return;

            Gun = PhotonNetwork.Instantiate(Equipment.ToString(), Vector3.zero, Quaternion.identity);
            Gun.transform.SetParent(transform);
            Grabber.GrabGrabbable(Gun.GetComponent<Grabbable>());
            Gun.GetComponent<Rifle>().Set_impulseSource(traineeObj.TraineeNumber + 1);
        }

        void SetGun()
        {
            if(HandType == ControllerHand.Right)
            {
                traineeObj.GetComponent<BNG.NetworkPlayer>().RightGrabber.GrabGrabbable(Gun.GetComponent<Grabbable>());
                //transform.GetComponent<TraineeObj>().remoteCharacter.GetComponent<BNG.NetworkPlayer>().RightGrabber.EquipGrabbableOnStart = Gun.GetComponent<Grabbable>();
            }
            else
            {
                traineeObj.GetComponent<BNG.NetworkPlayer>().LeftGrabber.GrabGrabbable(Gun.GetComponent<Grabbable>());
                //transform.GetComponent<TraineeObj>().remoteCharacter.GetComponent<BNG.NetworkPlayer>().LeftGrabber.EquipGrabbableOnStart = Gun.GetComponent<Grabbable>();
            }
        }
    }
}