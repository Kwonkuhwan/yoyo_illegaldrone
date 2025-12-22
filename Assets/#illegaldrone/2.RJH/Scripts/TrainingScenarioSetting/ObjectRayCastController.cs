using Illegaldrone;
using KKH;
using Photon.Realtime;
using RJH.UI;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace RJH
{
    public class ObjectRayCastController : MonoBehaviour
    {
        [SerializeField] private GameObject selectedObject;  // 선택된 오브젝트
        private bool isDragging = false;   // 드래그 중인지 여부
        private Vector3 offset;            // 마우스와 오브젝트의 초기 위치 차이
        private float fixedY;              // 고정된 y축 값
        private Plane dragPlane;           // 드래그 평면
        private Camera mainCamera;
        private float navMeshMaxDistance = 10.0f; // NavMesh 샘플링 최대 거리
        public Transform TraineeNaviMesh;
        public LayerMask targetLayerMask;

        public GameObject DefensTargetPoint;

        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (!GameManager.instance.isInstructor) return;

            // 마우스 클릭 처리
            if (Input.GetMouseButtonDown(0))
            {
                HandleMouseDown();
            }

            // 마우스 드래그 처리
            if (isDragging && Input.GetMouseButton(0))
            {
                HandleMouseDrag();
            }

            // 마우스 버튼을 놓았을 때 처리
            if (Input.GetMouseButtonUp(0))
            {
                HandleMouseUp();
            }
        }

        /// <summary>
        /// 마우스 클릭 시 실행되는 함수
        /// </summary>
        private void HandleMouseDown()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // 특정 태그를 가진 오브젝트만 이동 가능
                if (hit.collider.CompareTag("Movable"))
                {
                    selectedObject = hit.collider.transform.parent.gameObject;

                    // 방어 오브젝트인 경우 기존 목표 지점 삭제
                    if (selectedObject.GetComponentInChildren<ObjectController>().objType == ObjType.Defense)
                    {
                        Destroy(DefensTargetPoint);
                    }

                    selectedObject.GetComponentInChildren<ObjectController>().MouseDown();

                    // 드래그 평면 생성 (y축 고정)
                    fixedY = selectedObject.transform.position.y;
                    dragPlane = new Plane(Vector3.up, new Vector3(0, 800, 0));

                    isDragging = true;
                }
            }
        }

        /// <summary>
        /// 마우스 드래그 시 실행되는 함수
        /// </summary>
        private void HandleMouseDrag()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            // 레이와 드래그 평면의 교차점 계산
            if (dragPlane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);

                // 고정된 y축 값 유지
                Vector3 targetPosition = new Vector3(hitPoint.x, selectedObject.transform.position.y, hitPoint.z);
                
                // 드래그 중인 오브젝트가 화면 밖으로 나가지 않도록 
                Vector3 pos = mainCamera.WorldToViewportPoint(targetPosition);

                if (pos.x < 0f) pos.x = 0f;
                if (pos.x > 1f) pos.x = 1f;
                if (pos.y < 0f) pos.y = 0f;
                if (pos.y > 1f) pos.y = 1f;

                targetPosition = mainCamera.ViewportToWorldPoint(pos);

                // 드론 오브젝트인 경우 최소 거리 유지
                if (selectedObject.GetComponentInChildren<ObjectController>().objType == ObjType.Drone)
                {
                    targetPosition = MaintainMinDistance(targetPosition);
                }
                // 훈련생 오브젝트인 경우
                else if (selectedObject.GetComponentInChildren<ObjectController>().objType == ObjType.Trinee)
                {
                    targetPosition = hitPoint;
                }

                
                

                if (targetPosition == Vector3.zero) return;

                // 마우스 위치를 따라가도록 오브젝트 위치 업데이트
                selectedObject.transform.position = targetPosition;
            }

            try
            {
                TextMeshProUGUI text = selectedObject.GetComponentInChildren<ObjectController>().distanceText?.GetComponentInChildren<TextMeshProUGUI>();
                text.text = $"{LineController.Instance.distance:F0}m";
            }
            catch { }
        }

        /// <summary>
        /// 마우스 버튼을 놓았을 때 실행되는 함수
        /// </summary>
        private void HandleMouseUp()
        {
            if (isDragging)
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                if (dragPlane.Raycast(ray, out float enter))
                {
                    // 방어 오브젝트 처리
                    if (selectedObject.GetComponentInChildren<ObjectController>().objType == ObjType.Defense)
                    {
                        HandleDefenseObject(ray);
                    }
                    // 훈련생 오브젝트 처리
                    else if (selectedObject.GetComponentInChildren<ObjectController>().objType == ObjType.Trinee)
                    {
                        selectedObject.GetComponent<TraineeObj>().isTraineeSetPos = true;
                        HandleTraineeObject(ray);
                    }
                    else if(selectedObject.GetComponentInChildren<ObjectController>().objType == ObjType.Drone)
                    {
                        //selectedObject.transform.position = GetValidNavMeshPosition(selectedObject.transform.position);
                    }
                }
                
                selectedObject.GetComponentInChildren<ObjectController>().MouseUp();
                isDragging = false;
                selectedObject = null;
            }
        }

        /// <summary>
        /// 방어 오브젝트 처리
        /// </summary>
        /// <param name="ray">마우스 클릭 위치의 레이</param>
        private void HandleDefenseObject(Ray ray)
        {
            int layerMask = LayerMask.GetMask("Building");
            if (Physics.Raycast(ray, out RaycastHit hit, 2000f, layerMask))
            {
                // 방어 목표 지점 생성 및 설정
                GameObject targetPoint = new GameObject("DefensTargetPoint");
                targetPoint.transform.SetParent(hit.collider.transform);
                targetPoint.transform.localPosition = Vector3.zero;
                targetPoint.transform.rotation = Quaternion.identity;
                targetPoint.transform.localScale = Vector3.zero;

                DefensTargetPoint = targetPoint;

                UTILS.Log("방어 목표 지점 설정: " + hit.collider.name);
                selectedObject.GetComponent<SpriteRenderer>().color = Color.white;

                // 모든 드론의 목표 지점 설정
                foreach (GameObject drone in InstructorScenarioPage.Instance.mapController.Drones)
                {
                    try
                    {
                        Vector3 targetPos = new Vector3(hit.collider.transform.position.x, 250.0f, hit.collider.transform.position.z);
                        drone.GetComponent<DroneNavMeshAgent>().target = targetPos;
                    }
                    catch
                    {
                        continue;
                    }
                }
                InstructorScenarioPage.Instance.mapController.isDefensReady = true;
            }
            else
            {
                SetObjectErrorColor();
            }
        }

        /// <summary>
        /// 훈련생 오브젝트 처리
        /// </summary>
        /// <param name="ray">마우스 클릭 위치의 레이</param>
        private void HandleTraineeObject(Ray ray)
        {
            int buildingMask = LayerMask.GetMask("Building");
            int groundMask = LayerMask.GetMask("Ground");
            if (Physics.Raycast(ray, out RaycastHit hit, 2000f, buildingMask | groundMask))
            {
                int tNum = selectedObject.GetComponent<TraineeObj>().TraineeNumber;
                //Player player = PhotonManager_.Inst.GetSelectPlayer(tNum + 2);
                // 2025-04-28 RJH CustomProperties["TraineeNumber"] 사용
                Player player = PhotonManager_.Inst.GetSelectPlayer(tNum);
                if (player != null)
                {
                    PhotonManager_.Inst.SetPlayerCustomProperty("Position", hit.point, player);
                    UTILS.Log($"Photon 활성 번호 : {player.ActorNumber} 훈련생 번호 : {tNum}_Trainee || 훈련생 위치 : {hit.point}");
                }
            }
        }


        /// <summary>
        /// 오브젝트 색상 초기화
        /// </summary>
        private void SetObjectErrorColor()
        {
            selectedObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.549f, 0.549f);
            InstructorScenarioPage.Instance.mapController.isDefensReady = false;

            // 모든 드론의 목표 지점 초기화
            foreach (GameObject drone in InstructorScenarioPage.Instance.mapController.Drones)
            {
                try
                {
                    drone.GetComponent<DroneNavMeshAgent>().target = Vector3.zero;
                }
                catch
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// 최소 거리를 유지하며 목표 위치를 반환
        /// </summary>
        /// <param name="targetPosition">목표 위치</param>
        /// <returns>유효한 목표 위치</returns>
        private Vector3 MaintainMinDistance(Vector3 targetPosition)
        {
            // 고정된 y축 값 유지
            targetPosition = new Vector3(targetPosition.x, 800, targetPosition.z);

            // 목표 위치와 메인 목표 위치 간의 거리 계산
            Vector3 direction = targetPosition - InstructorScenarioPage.Instance.GetMainTargetPosition();
            float distance = direction.magnitude;

            // 최소 거리보다 가까운 경우
            if (distance < 800)
            {
                targetPosition = Vector3.zero;
                GuideToast.Instance.PopupMSG(MSG.NOTPOSSIBLE);
                InstructorScenarioPage.Instance.RedLineOn();
            }
            else
            {
                GuideToast.Instance.Close();
                InstructorScenarioPage.Instance.RedLineOff();
            }

            return targetPosition;
        }

        private Vector3 GetValidNavMeshPosition(Vector3 position)
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 vector3 = new Vector3();

            if (Physics.Raycast(ray, out RaycastHit hit, 1200f, targetLayerMask))
            {
                vector3 = hit.point;
                UTILS.Log(vector3);
            }

            if (NavMesh.SamplePosition(vector3, out NavMeshHit navMeshHit, navMeshMaxDistance, NavMesh.AllAreas))
            {
                UTILS.Log("배치 가능");
                return new Vector3(navMeshHit.position.x, position.y, navMeshHit.position.z); // NavMesh 위의 위치 반환
            }
            else
            {
                UTILS.Log("배치 불가능");
                return new Vector3(0, 0, 0); // NavMesh 밖이면 기존 위치 유지
            }
        }
    }
}