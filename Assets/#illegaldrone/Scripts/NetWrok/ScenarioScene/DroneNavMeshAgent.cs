using Illegaldrone;
using Photon.Pun;
using RJH;
using RJH.UI;
using SMW;
using UnityEngine;
using UnityEngine.AI;

namespace KKH
{
    public class DroneNavMeshAgent : MonoBehaviourPun
    {
        public Vector3 target; // 목표 위치
        public Vector3 oldtarget; // 이전 목표 위치
        public NavMeshAgent agent; // NavMesh 에이전트
        public float fSpeed = 10.0f;

        public bool isStart = false;
        public bool isJamming
        {
            get { return isjamming; }
            set
            {
                isjamming = value;
                if (isFalling) return; // 드론이 떨어지지 않았을 때만 처리
                if (isjamming)
                {
                    agent.avoidancePriority = 1;
                    agent.isStopped = true; // 에이전트 정지
                    //agent.ResetPath(); // 경로 초기화
                }
                else
                {
                    agent.avoidancePriority = 50;
                    agent.isStopped = false; // 에이전트 재개
                }
            }
        }
        bool isjamming = false;

        private float jammingTime;
        private bool isFalling;

        private Rigidbody Rigidbody;

        public AudioSource Audio;

        protected virtual void Start()
        {
            // 강사 모드일 때 NavMesh 에이전트 가져오기
            if (GameManager.instance.isInstructor && agent == null)
            {
                agent = GetComponent<NavMeshAgent>();
                agent.speed = fSpeed;
            }

            Rigidbody = GetComponent<Rigidbody>();
        }

        // 매 프레임마다 호출되는 업데이트 메서드
        protected virtual void Update()
        {
            if (GameManager.instance.isInstructor == false) return; // 강사 모드가 아닐 때 리턴
            if (agent.enabled == false) // 재밍되거나 파괴되어서 더이상 못 움직일때 리턴
            {
                return;
            }
            SetAgentPath();

            if (isjamming)
            {
                Jamming();
            }
            else
            {
                if(jammingTime > 0)
                {
                    jammingTime -= Time.deltaTime;
                }
                else
                {
                    jammingTime = 0;
                }
            }

            if (agent.pathStatus != NavMeshPathStatus.PathComplete && !isJamming) // 드론이 네비메쉬필드 위에서 생성되지 않았을때(네비메쉬 높이보다 높은 건물에서 생성되었을때) 파괴되지만 점수에는 영향을 안주도록 추가
            {
                UTILS.Log("엉뚱한 목표에 충동");
                agent.enabled = false;
                this.enabled = false;
                GetComponent<DroneState>().DestroyByCrush(false);
            }

            if (agent.pathStatus == NavMeshPathStatus.PathComplete && target == oldtarget && agent.pathPending == false && (agent.remainingDistance < 30f || (agent.remainingDistance < 100f&& agent.velocity.magnitude <10))) // 2025-04-14 드론이 목표에 도착 했을때 (목표와의 거리 15 미만 일때)
            {
                Debug.Log("충돌");
                Debug.Log(oldtarget);
                Debug.Log(agent.remainingDistance);
                agent.enabled = false;
                this.enabled = false;
                int droneType = GetComponent<DroneState>().droneoption.droneType;
                //GameManager.instance.DroneAttack(); // 2025-04-18 RJH 드론이 공격한 정보를 동기화하기 위해 DroneState.RPC_DestroyByCrush로 이동
                GetComponent<DroneState>().DestroyByCrush(true);
            }
        }

        void SetAgentPath()
        {
            try
            {
                // 드론이 시작되지 않았을 때 현재 위치로 설정
                if (!isStart)
                {
                    oldtarget = transform.position;
                    agent.SetDestination(transform.position);
                }
                else
                {
                    // 목표 위치가 이전 목표 위치와 같으면 리턴
                    if (target == oldtarget) return;

                    // 목표 위치 갱신 및 설정
                    oldtarget = target;
                    agent.SetDestination(target);
                }
            }
            catch
            {
                // 예외 발생 시 처리
            }
        }

        void Jamming()
        {
            if (isFalling == true)
            {
                return;
            }

            if(jammingTime >= 3f)
            {
                isFalling = true;
                agent.enabled = false;
                Rigidbody.useGravity = true;
                // 2025-04-11 RJH 드론 재밍 로그 추가
                int trainee = GetComponent<DroneState>().jammingtrainee;
                int droneType = GetComponent<DroneState>().droneoption.droneType;
                GameManager.instance.DroneInactive(trainee, droneType);

                GameObject canvas = gameObject.GetComponent<NetworkDrone>().parent.gameObject;
                foreach (GameObject obj in MapObjectController.Inst.Drones)
                {
                    obj.GetComponent<DroneSpawnPoint>().drones.Remove(canvas);
                }

                photonView.RPC("MuteAudio", RpcTarget.All);

                canvas.SetActive(false);
                InstructorScenarioPage.Instance.DroneDestroy(droneType);
            }
            else
            {
                jammingTime += Time.deltaTime;
            }
        }

        public void NavMeshReset()
        {
            agent.enabled = false;
            agent.enabled = true;
        }

        [PunRPC]
        void MuteAudio()
        {
            Audio.mute = true;
        }
    }
}