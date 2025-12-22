using RJH.UI;
using TMPro;
using UnityEngine;

namespace RJH
{
    public enum ObjType
    {
        None = -1,
        Drone,
        Trinee,
        Defense
    }

    public class ObjectController : MonoBehaviour
    {
        public ObjType objType = ObjType.None;        

        public GameObject distanceText; // 주요 목표와의 거리
        public GameObject pressedIcon;


        //public void OnDrag()
        //{
        //    // 현재 화면에 있는 마우스 커서의 x,y 좌표와 카메라를 통해 보는 이 스크립트가 실행되는 오브젝트의 z좌표를 사용해 ScreenPoint Vector3 position 값 생성
        //    Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(transform.position).z);
        //    // 오브젝트를 이동할 때 움직일 x,z 좌표를 가진 WorldPoint Vector3 position 생성
        //    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
        //    Debug.DrawLine(Camera.main.transform.position, worldPosition);

        //    if (Vector3.Distance(worldPosition, InstructorScenarioPage.Instance.GetMainTargetPosition()) < 8 && isDrone)
        //    {
        //        Vector3 vector3 = worldPosition - InstructorScenarioPage.Instance.GetMainTargetPosition();
        //        if (vector3.magnitude < 1.1f)
        //        {
        //            return;
        //        }

        //        vector3 = vector3.normalized;
        //        worldPosition = InstructorScenarioPage.Instance.GetMainTargetPosition() + vector3 * 8f;
        //        GuideToast.Instance.PopupMSG(MSG.NOTPOSSIBLE);
        //        InstructorScenarioPage.Instance.RedLineOn();
        //    }
        //    else
        //    {
        //        GuideToast.Instance.Close();
        //        InstructorScenarioPage.Instance.RedLineOff();
        //    }

        //    if (Vector3.Distance(worldPosition, InstructorScenarioPage.Instance.GetMainTargetPosition()) > 5.7f && isDrone == false)
        //    {
        //        Vector3 vector3 = worldPosition - InstructorScenarioPage.Instance.GetMainTargetPosition();
        //        vector3 = vector3.normalized;
        //        worldPosition = InstructorScenarioPage.Instance.GetMainTargetPosition() + vector3 * 5.7f;
        //    }

        //    // 위 worldPosition의 x,z 좌표를 사용하고 직접 y좌표를 설정해 오브젝트 이동
        //    transform.parent.position = new Vector3(worldPosition.x, 1f, worldPosition.z);
        //    TextMeshProUGUI text = distanceText?.GetComponentInChildren<TextMeshProUGUI>();
        //    text.text = $"{LineController.Instance.distance:F0}m";
        //}

        public void MouseDown()
        {
            DronesUIActive();
        }

        public void MouseUp()
        {
            LineController.Instance.gameObject.SetActive(false);
            if (distanceText != null)
            {
                distanceText?.SetActive(false);
            }
            GuideToast.Instance.Close();
            InstructorScenarioPage.Instance.RedLineOff();

            if (pressedIcon != null)
            {
                pressedIcon?.SetActive(false);
            }
        }

        /// <summary>
        /// 드론 UI 활성화
        /// </summary>
        /// <param name="isUI"></param>
        public void DronesUIActive(bool isUI = false)
        {
            DronesActiveClear();

            if (objType != ObjType.Defense)
            {
                LineController.Instance.gameObject.SetActive(true);
                LineController.Instance.SetTarget(transform.parent);

                if (pressedIcon != null)
                {
                    pressedIcon?.SetActive(true);
                }

                if (distanceText != null)
                {
                    distanceText?.SetActive(true);
                }
            }

            // 드론이고, 오브젝트를 클릭했을경우
            if (objType == ObjType.Drone && !isUI)
            {
                DroneSpawnPoint droneObj = GetComponentInParent<DroneSpawnPoint>();
                //droneObj.SetDroneOption();
                InstructorScenarioPage.Instance.DroneButtonActivate(droneObj.droneOption.droneType);
            }
        }

        public void DronesUIDeActive()
        {
            if (objType != ObjType.Defense)
            {
                LineController.Instance.gameObject.SetActive(false);
                pressedIcon?.SetActive(false);
                distanceText?.SetActive(false);
            }
        }

        /// <summary>
        /// 드론 선택 활성화 초기화
        /// </summary>
        private void DronesActiveClear()
        {
            foreach (var drone in InstructorScenarioPage.Instance.mapController.Drones)
            {
                if (drone == null) continue;
                if (drone.activeSelf)
                    drone.GetComponent<DroneSpawnPoint>().objController.DronesUIDeActive();
            }
        }
    }
}


