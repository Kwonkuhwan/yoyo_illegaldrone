using Illegaldrone;
using KKH;
using Photon.Pun;
using RJH;
using RJH.UI;
using System.Collections;
using UnityEngine;

namespace SMW
{
    public class DroneState : MonoBehaviourPun
    {
        [Range(10, 100)] public int health = 100;                            // 체력
        [Range(0.1f, 1f)] public float duration_jamming = 0.5f;             // 재밍 지속시간
        float time_Jamming = 0;
        DroneOption droneOption;                                            // 드론 정보
        public DroneOption droneoption => droneOption;

        [Range(1, 10)] public int rifleDamage = 10;                          // 라이플 데미지

        [SerializeField] ParticleSystem DestroyEffect;                      // 라이플 폭파 이펙트
        [SerializeField] GameObject DroneModel;                             // 드론 모델

        bool isEnd = false;                                                 // 작동중지 여부

        DroneNavMeshAgent NavMeshAgent;                                     // 드론 내비게이션 에이전트

        private int jammingTrainee;                 // 2025-04-11 RJH 드론을 재밍하고 있는 훈련생 리스트
        public int jammingtrainee => jammingTrainee;

        private void Start()
        {
            NavMeshAgent = GetComponent<DroneNavMeshAgent>();

            if(MapCanvas.Instance != null)
            {
                MapCanvas.Instance.AddDroneTransform(transform);
            }
        }

        private void Update()
        {
            Jamming();
        }

        void Jamming()
        {
            if (NavMeshAgent.isJamming == false) return;
            if (time_Jamming > 0)
            {
                time_Jamming -= Time.deltaTime;
            }
            else
            {
                time_Jamming = 0;
                NavMeshAgent.isJamming = false;
            }
        }

        // 2025-04-11 RJH 드론 정보 저장
        public void SetDroneOption(DroneOption drone)
        {
            droneOption = drone;
            if (droneOption != null)
            {
                photonView.RPC("RPC_SetDroneOption", RpcTarget.Others, droneOption.droneType, droneOption.flyingType, droneOption.flyingOrder);
            }
        }

        [PunRPC]
        public void RPC_SetDroneOption(int droneType, int flyingType, int flyingOrder)
        {
            droneOption = new DroneOption()
            {
                droneType = droneType,
                flyingType = flyingType,
                flyingOrder = flyingOrder
            };
        }

        /// <summary>
        /// 피격 처리(훈련생 정보, 드론 정보 추가) 2025-04-11 RJH 
        /// </summary>
        public void Hit(장비 _name, int traineeNumber)
        {

            if (isEnd) return;

            switch (_name)
            {
                case 장비.K2C1:
                    {
                        health -= rifleDamage;

                        if (PhotonNetwork.IsConnected)
                        {
                            photonView.RPC("RPC_RifleOnDamaged", RpcTarget.All, health, traineeNumber);
                        }
                        else
                        {
                            RPC_RifleOnDamaged(health, traineeNumber);
                        }
                    }
                    break;
                case 장비.JammingGun:
                    {
                        if (PhotonNetwork.IsConnected)
                        {
                            photonView.RPC("RPC_Jamming", RpcTarget.All, traineeNumber);
                        }
                        else
                        {
                            RPC_Jamming(traineeNumber);
                        }
                    }
                    break;
                case 장비.NetGun:
                    {
                        // 구현준비중
                    }
                    UTILS.Log("네트건에 피격");
                    break;
            }
        }

        /// <summary>
        /// 재밍 처리 (교관만) 2025-04-14 RJH
        /// </summary>
        [PunRPC]
        void RPC_Jamming(int traineeNumber) // 재밍 처리 할때 훈련생 번호 입력 추가 
        {
            if(NavMeshAgent.isJamming)
            {
                return; // 이미 재밍 당하고 있으면 넘어감
            }

            if (GameManager.instance.isInstructor)
            {
                time_Jamming = duration_jamming;
                NavMeshAgent.isJamming = true;
                jammingTrainee = traineeNumber; // 재밍을 건 훈련생 정보 저장
            }
            UTILS.Log("재밍건에 피격");
        }

        /// <summary>
        /// 라이플에 데미지를 입음 / HP 동기화 (훈련생 정보 추가) 2025-04-11 RJH 
        /// </summary>
        /// <param name="hp"> 드론 현재 체력 </param>
        [PunRPC]
        void RPC_RifleOnDamaged(int hp, int trainee)
        {
            health = hp;
            // 드론 파괴
            if (health <= 0)
            {
                isEnd = true;
                DestroyByRifle();
                GameManager.instance.TraineeShoot(trainee, droneOption.droneType);
            }
            UTILS.Log("라이플에 피격");
        }

        /// <summary>
        /// 라이플에 의한 폭파
        /// </summary>
        void DestroyByRifle()
        {
            if (DestroyEffect == null) return;

            if(GameManager.instance.isInstructor)
            {
                NavMeshAgent.agent.isStopped = true;
            }

            DestroyEffect.Play();
            GetComponent<DroneNavMeshAgent>().isStart = false; // 2025-04-18 RJH 이동 멈춤
            StartCoroutine(DestoryDrone());
        }

        IEnumerator DestoryDrone()
        {
            DroneModel.SetActive(false);
            yield return new WaitForSeconds(3.0f);
            if (PhotonNetwork.IsMasterClient)
            {
                GameObject canvas = gameObject.GetComponent<NetworkDrone>().parent.gameObject;
                canvas.SetActive(false);
                
                // SMW
                // 드론 Canvas 파괴 시 리플레이 녹화데이터에서 삭제.
                UltimateReplay.ReplayManager.RemoveReplayObjectFromRecordScenes(canvas);

                foreach (GameObject obj in MapObjectController.Inst.Drones)
                {
                    obj.GetComponent<DroneSpawnPoint>().drones.Remove(canvas);
                }

                InstructorScenarioPage.Instance.DroneDestroy(droneOption.droneType);
                PhotonNetwork.Destroy(gameObject);
                UTILS.LogColor($"{gameObject.name} 드론 지워져라....", Color.red);
            }
        }

        /// <summary>
        /// 충돌로 인한 폭파 2025-04-15 RJH 
        /// </summary>
        public void DestroyByCrush(bool isTarget)
        {
            photonView.RPC("RPC_DestroyByCrush", RpcTarget.All, isTarget);
        }

        public bool isDestroyed()
        {
            return DroneModel.activeSelf;
        }

        [PunRPC]
        void RPC_DestroyByCrush(bool isTarget)
        {
            if (DestroyEffect == null) return;

            //DroneModel.SetActive(false);
            DestroyEffect.Play();
            if(isTarget)
                GameManager.instance.DroneAttack();
            StartCoroutine(DestoryDrone());
        }
    }
}