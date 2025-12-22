using RJH.UI;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;
using SMW;
using KKH;
using Illegaldrone;

namespace RJH
{
    public class DroneObj : MonoBehaviourPun
    {
        [SerializeField] private ObjectController objectController;
        public ObjectController objController => objectController;

        [SerializeField] private Sprite[] formationSprites;
        [SerializeField] private Image formationImage;

        // [KKH][삭제][24.10.16] number, droneType 필요 없어서 삭제 DroneOption으로 병합
        //public int droneType;
        //public int number;

        public Transform formationObject;
        public Transform droneIconObj;
        public Transform pressedIconObj;
        public Transform distanceObj;

        [SerializeField] private Transform mainTarget;

        public DroneOption droneOption;

        public bool isDroneDestroy = false;

        [Space]
        [Header("Photon Drone")]
        public GameObject prefab_Drone;
        public GameObject go_DroneObj;
        public DroneName droneName;

        public void SetMainTargetPosition(Transform maintarget)
        {
            mainTarget = maintarget;
        }

        private void Awake()
        {
            objectController = GetComponentInChildren<ObjectController>();
            formationImage = formationObject.GetComponent<Image>();
        }

        private void Start()
        {
            //SetDroneOption(false);
        }

        private void Update()
        {
            // SMW 추가
            // 교관이 아닐 시 드론 컨트롤 X
            if (!GameManager.instance.isInstructor) return;

            //if (isStart) return;
            //movable.transform.localPosition = new Vector3(0, 800 - transform.position.y, 0);

            Vector3 direction = mainTarget.position - transform.position;
            // Z축 기준 각도 계산
            float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
            // Z축 회전 적용
            //deleteButton.transform.rotation = Quaternion.Euler(90, 0, angle - 90);
            //scoreObject.transform.rotation = Quaternion.Euler(90, 0, angle - 90);
            formationObject.rotation = Quaternion.Euler(90, 0, angle - 90);
            droneIconObj.rotation = Quaternion.Euler(90, 0, angle - 90);
        }

        #region [RJH][2024.10.15] 더이상 사용하지 않음
        //public void SetPosition()
        //{
        //    photonView.RPC("SetPositionRPC", RpcTarget.Others);
        //}

        //[PunRPC]
        //private void SetPositionRPC()
        //{
        //    this.gameObject.SetActive(false);
        //}
        #endregion

        public void NetWorkDroneCreate()
        {
            if (go_DroneObj == null && GameManager.instance.isInstructor && PhotonNetwork.IsConnected)
            {
                //playerCharacter = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
                go_DroneObj = PhotonNetwork.Instantiate($"Drones/{droneName.ToString()}", new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
                NetworkDrone nd = go_DroneObj.GetComponent<NetworkDrone>();
                DroneState ds = go_DroneObj.GetComponent<DroneState>();
                DroneNavMeshAgent droneNavMeshAgent = go_DroneObj.GetComponent<DroneNavMeshAgent>();
                if (nd)
                {
                    nd.SetNetWorkDroneName($"{droneName.ToString()}");
                    nd.SetDrone(transform);
                    nd.SetTarget(mainTarget.position);
                    //nd.SetParent(transform);
                }
                if (ds)
                {
                    ds.SetDroneOption(droneOption);
                }
                if (droneNavMeshAgent)
                {
                    droneNavMeshAgent.isStart = true;
                }
            }
            else
            {
                go_DroneObj = Instantiate(prefab_Drone);
            }

            // SMW
            // 드론 리플레이 파일에 저장
            UltimateReplay.ReplayManager.AddReplayObjectToRecordScenes(go_DroneObj);
        }

        private void OnDisable()
        {
            gameObject.transform.position = Vector3.zero;
        }

        public void SetDroneInfo(DroneOption option)
        {
            // [KKH][수정][24.10.16] number, droneType 필요 없어서 삭제 DroneOption으로 병합
            droneOption = option;
            //number = droneOption.flyingOrder;
            SetFormationSprites(droneOption.flyingType);
        }

        /// [KKH][추가][24.10.16]
        /// <summary>
        /// 드론 초기 UI 버튼 설정
        /// </summary>
        /// <param name="isOn"></param>
        public void SetDroneOption(DroneOption droneOption)
        {
            //InstructorScenarioPage.Instance.SetDroneOptionChane(droneOption, isOn);
            SetFormationSprites(droneOption.flyingType);
            NetWorkDroneCreate();
        }

        /// [KKH][추가][24.10.16]
        /// <summary>
        /// 포메이션 이미지 변경
        /// </summary>
        /// <param name="formationType"></param>
        private void SetFormationSprites(int formationType)
        {
            formationImage.sprite = formationSprites[formationType];
        }

        // ====================================================================================================
        // [SMW][추가][25.03.12]

        float duration_jamming = 0.5f;
        bool isJamming = false;

        public void Hit(장비 _name)
        {
            switch (_name)
            {
                case 장비.K2C1:
                    {
                        UTILS.Log("라이플에 피격");
                    }
                    break;
                case 장비.JammingGun:
                    {
                        if (isJamming) return;

                        StartCoroutine(Jamming());
                        UTILS.Log("재밍건에 피격");
                    }
                    break;
                case 장비.NetGun:
                    {
                        UTILS.Log("네트건에 피격");
                    }
                    break;
            }
        }

        IEnumerator Jamming()
        {
            isJamming = true;
            yield return new WaitForSeconds(duration_jamming);
            isJamming = false;
        }

        // ====================================================================================================
    }
}


