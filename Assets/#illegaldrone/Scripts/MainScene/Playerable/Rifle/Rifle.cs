using BNG;
using KKH;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;
using RJH;
using Photon.Pun;
using UltimateReplay;
using UltimateReplay.StatePreparation;
using System;
using Illegaldrone;

namespace SMW
{
    public enum 장비
    {
        K2C1,
        JammingGun,
        NetGun
    }

    public class Rifle : MonoBehaviourPun, IPunObservable
    {
        Grabbable grabbable;            // 첫번째 그립
        bool isGrab
        {
            //get { return !grabbable.IsGrabbable() && !grabbable.SecondaryGrabbable.IsGrabbable(); }
            get { return !grabbable.IsGrabbable(); }
        }

        LaserPoint LaserPoint;
        InputBridge inputBridge;

        // 총기 옵션
        [Header("Rifle Option")]
        [SerializeField] 장비 EquipmentName;
        [Range(1,30)] public int Bullet;
        [Range(0,5f)] public float ReloadTime;
        [Range(0.1f,1f)] public float FireRate;
        float FireRange = 10;
        float timer = 0;

        // 반동 옵션
        [Header("Recoil Option")]
        [Range(0, 1)] public float recoilAmount = 0.1f;
        [Range(0, 10)] public float recoilSpeed = 5f;
        [Range(0, 10)] public float returnSpeed = 3f;
        Vector3 recoil_positon;
        CinemachineImpulseSource impulseSource;

        // 진동 옵션
        [Header("Haptics Option")]
        [Range(0, 1)] public float frequency;       // 빈도
        [Range(0, 1)] public float amplitude;       // 진폭
        [Range(0, 1)] public float duration;        // 지속

        // 오른쪽 컨트롤러 앵커
        [Header("Right Hand Anchor")]
        [SerializeField] Transform Right_Hand;

        // 스코프 화면
        [Header("Display")]
        [SerializeField] TMP_Text Text_Distance;

        // 이펙트
        [Header("Effect")]
        [SerializeField] GameObject Effect_Flash;
        [SerializeField] GameObject Effect_Impact;
        [SerializeField] LayerMask Effect_Layer;
        List<GameObject> pool_Impact = new List<GameObject>();
        List<GameObject> list_Impack = new List<GameObject>();

        // Photon
        PhotonView PV;
        int channel;

        // ==================================================
        // 리플레이 데이터 저장 클래스
        [SerializeField] ReplayRifle Replay;
        // ==================================================

        private void Awake()
        {
            PV = transform.GetComponent<PhotonView>();
            grabbable = transform.GetComponent<Grabbable>();
            LaserPoint = transform.GetComponentInChildren<LaserPoint>();
            impulseSource = transform.GetComponent<CinemachineImpulseSource>();

            FireRange = LaserPoint.pointer_max_range;
            
            // 교관은 VR컨트롤러가 필요없음
            if(GameManager.instance.isInstructor)
            {
                UltimateReplay.ReplayManager.AddReplayObjectToRecordScenes(gameObject);
                return;
            }

            try
            {
                // VR 컨트롤러 입력
                inputBridge = InputBridge.Instance;
                if (Right_Hand == null && inputBridge != null)
                {
                    Right_Hand = GameObject.FindGameObjectWithTag("Hand").transform;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("InputBridge not found: " + e.Message);
            }
        }

        private void Reset()
        {
            timer = 0;
        }

        private bool isReloading = false; // 재장전 상태 확인 변수 추가

        private void Update()
        {
            // 리플레이 재생 중에는 총기 발사 불가
            if (ReplayManager.Instance.isReplaying) return;

            UpdateTargetDistance();
            ReturnRecoil();

            InterpolateNetworkTransform();
            if (isGrab == false) return;

            timer += Time.deltaTime;
            if (timer >= FireRate && IsAttackInput())
            {
                if (Bullet > 0) // 총알이 남아있는지 확인
                {
                    timer = 0;
                    Bullet--; // 총알 수 감소
                    Attack();
                }
                else if (!isReloading) // 재장전 중이 아닐 때만 실행
                {
                    StartCoroutine(Reload()); // 총알이 없으면 재장전
                }
            }
        }

        private IEnumerator Reload()
        {
            isReloading = true; // 재장전 상태 설정
            yield return new WaitForSeconds(ReloadTime); // 재장전 시간 대기
            Bullet = 30; // 총알 수 초기화 (예: 30발로 설정)
            isReloading = false; // 재장전 상태 해제
        }

        /// <summary>
        /// 반동 위치 초기화
        /// </summary>
        void ReturnRecoil()
        {
            if (recoil_positon != Vector3.zero)
            {
                recoil_positon = Vector3.Lerp(recoil_positon, Vector3.zero, Time.deltaTime * returnSpeed);
                Right_Hand.localPosition = recoil_positon;
            }
        }

        // 총 발사
        void Attack()
        {
            // 목표물 체크
            if (LaserPoint.IsDetect && LaserPoint.CalculatorDistance() <= FireRange)
            {
                // 총기 발사
                //LaserPoint.Target.GetComponentInParent<DroneState>().Hit(EquipmentName);
                //[2025-04-11] RJH 총기 정보와 훈련생 정보 전달
                LaserPoint.Target.GetComponentInParent<DroneState>().Hit(EquipmentName, (int)PhotonNetwork.LocalPlayer.CustomProperties["TraineeNumber"]);
            }

            if (PhotonNetwork.IsConnected)
            {
                PV.RPC("RPC_Attack", RpcTarget.All);
            }
            else
            {
                RPC_Attack();
            }

            ApplyRecoil();
        }

        /// <summary>
        /// 드론 거리 측정 텍스트 업데이트
        /// </summary>
        void UpdateTargetDistance()
        {
            if (Text_Distance == null) return;

            if(LaserPoint.CalculatorDistance() == 0)
            {
                Text_Distance.text = "";
            }
            else 
            {
                Text_Distance.text = $"{LaserPoint.CalculatorDistance()}";
            }
        }

        public void Set_impulseSource(int _channel)
        {
            channel = _channel;
            impulseSource.m_ImpulseDefinition.m_ImpulseChannel = channel;
            PV.RPC("RPC_Set_impulseSource", RpcTarget.OthersBuffered, channel);
        }

        /// <summary>
        /// 라이플 효과 적용
        /// </summary>
        public void ApplyEffect()
        {
            if (Effect_Flash == null) return;
            Effect_Flash.SetActive(false);
            Effect_Flash.SetActive(true);
        }

        /// <summary>
        /// 탄흔 생성
        /// </summary>
        void CreateImpactEffect()
        {
            if (Effect_Impact == null) return;
            if (LaserPoint.Target == null) return;

            // 타겟 레이어 체크
            if ((Effect_Layer & (1 << LaserPoint.Target.layer)) == 0)
            {
                return;
            }

            // 탄흔 생성
            GameObject impact;
            if (pool_Impact.Count < 1)
            {
                impact = Instantiate(Effect_Impact, LaserPoint.Hit.point, Quaternion.identity);
            }
            else
            {
                impact = pool_Impact[0];
                pool_Impact.RemoveAt(0);
                impact.SetActive(true);
                impact.transform.position = LaserPoint.Hit.point;
            }
            impact.transform.LookAt(LaserPoint.Hit.point + LaserPoint.Hit.normal);
            list_Impack.Add(impact);
            StartCoroutine(ImpactEffect_Off(impact));
        }

        /// <summary>
        /// 반동 적용
        /// </summary>
        void ApplyRecoil()
        {
            Vector3 recoilDirection = -transform.forward * recoilAmount;
            // 월드 좌표계를 기준으로 정의된 것을 로컬 좌표계 기준으로 바꿈
            recoil_positon += Right_Hand.InverseTransformDirection(recoilDirection);
            Right_Hand.localPosition = Vector3.Lerp(Right_Hand.localPosition, Right_Hand.localPosition + recoil_positon, Time.deltaTime * recoilSpeed);

            // 햅틱 진동
            inputBridge.VibrateController(frequency, amplitude, duration, ControllerHand.Right);
            inputBridge.VibrateController(frequency, amplitude, duration, ControllerHand.Left);
        }

        #region 포톤 함수

        [PunRPC]
        void RPC_Attack()
        {
            CreateImpactEffect();
            ApplyEffect();

            // 녹화 시 라이플 발사 이벤트 기록
            if (Replay != null) Replay.TriggerApplyEffect();
        }

        [PunRPC]
        void RPC_Set_impulseSource(int _channel)
        {
            impulseSource.m_ImpulseDefinition.m_ImpulseChannel = _channel;
        }

        [PunRPC]
        void RPC_Impulse()
        {
            impulseSource.GenerateImpulse();
        }

        #endregion

        #region 리플레이

        /// <summary>
        /// 리플레이 재생 중 이벤트를 재생
        /// </summary>
        public void OnReplayAttackEvent()
        {
            ApplyEffect();
            CreateImpactEffect();
        }

        #endregion

        /// <summary>
        /// 컨트롤러 공격 입력
        /// </summary>
        bool IsAttackInput()
        {
            if(inputBridge != null)
            {
                return Input.GetMouseButton(0) || inputBridge.RightTrigger == 1 || inputBridge.LeftTrigger == 1;
            }
            return false;
        }

        IEnumerator ImpactEffect_Off(GameObject impact)
        {
            yield return new WaitForSeconds(1.5f);
            impact.SetActive(false);
            pool_Impact.Add(impact);
            list_Impack.Remove(impact);
        }

        #region 포톤 동기화
        Vector3 receivePos;
        Quaternion receiveRot;
        float lerpSpeed = 100;
        bool isFirstReceived = false;
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
            }
            else
            {
                receivePos = (Vector3)stream.ReceiveNext();
                receiveRot = (Quaternion)stream.ReceiveNext();

                if (isFirstReceived == false)
                {
                    isFirstReceived = true;
                    this.gameObject.transform.position = receivePos;
                }
            }
        }

        /// <summary>
        /// 네트워크 동기화
        /// </summary>
        void InterpolateNetworkTransform()
        {
            if (isFirstReceived)
            {
                transform.position = Vector3.Lerp(transform.position, receivePos, Time.deltaTime * lerpSpeed);
                transform.rotation = Quaternion.Lerp(transform.rotation, receiveRot, Time.deltaTime * lerpSpeed);
            }
        }
        #endregion
    }
}