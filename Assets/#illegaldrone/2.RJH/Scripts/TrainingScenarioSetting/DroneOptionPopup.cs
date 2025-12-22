using RJH.UI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RJH
{
    [Serializable]
    public class DroneOption
    {
        public int droneType; // 드론 타입
        public int flyingType; // 비행 형태
        public int flyingOrder; // 비행 순서
    }

    public class DroneOptionPopup : MonoBehaviour
    {
        [Header("잠입 순서")]
        [SerializeField] private TextMeshProUGUI[] titleText;
        [SerializeField] private TextMeshProUGUI orderText;
        [SerializeField] private Image backGround;
        [SerializeField] private Button rightButton;
        [SerializeField] private Button leftButton;

        [Space]
        [Header("군집 형태")]
        [SerializeField] private ToggleGroup formationGroup;
        [Space]
        [Header("스프라이트")]
        [SerializeField] private Sprite defaultBackGroundSprite;
        [SerializeField] private Sprite disableBackGroundSprite;
        [Space]
        [SerializeField] private Button confirmButton;

        public DroneOption droneOption = new DroneOption();
        private DroneButton droneButton;

        private void Awake()
        {
            confirmButton.onClick.AddListener(SendOption);
            rightButton.onClick.AddListener(RightButton);
            leftButton.onClick.AddListener(LeftButton);
            orderText.text = "";
            backGround.sprite = disableBackGroundSprite;
            foreach(var text in titleText)
            {
                text.color = new Color(115f / 255f, 128f / 255f, 127f / 255f, 1);
            }
        }
        
        /// <summary>
        /// 잡입 순서 증가
        /// </summary>
        private void RightButton()
        {
            if(droneOption.droneType == 2) // 정찰형 드론이면 무시
            {
                return;
            }

            if (droneOption.flyingOrder >= 3)
            {
                return;
            }

            leftButton.GetComponent<ValueButtonController>().Check(true);
            droneOption.flyingOrder++;

            orderText.text = (droneOption.flyingOrder + 1).ToString();

            if(droneOption.flyingOrder == 3)
            {
                rightButton.GetComponent<ValueButtonController>().Check(false);
            }
        }

        /// <summary>
        /// 잡입 순서 감소
        /// </summary>
        private void LeftButton()
        {
            if (droneOption.droneType == 2) // 정찰형 드론이면 무시
            {
                return;
            }

            if (droneOption.flyingOrder <= 0)
            {
                return;
            }

            rightButton.GetComponent<ValueButtonController>().Check(true);
            droneOption.flyingOrder--;
            
            orderText.text = (droneOption.flyingOrder + 1).ToString();

            if (droneOption.flyingOrder == 0)
            {
                leftButton.GetComponent<ValueButtonController>().Check(false);
            }
        }

        public void SetDroneInfo(DroneOption dOption)
        {
            droneOption = dOption;
        }

        /// <summary>
        /// 드론 옵션팝업 활성화, 드론에 이미 적용되어있는 값 팝업에 적용
        /// </summary>
        /// <param name="droneButton">옵션팝업 활성화한 드론버튼</param>
        public void SetOn(DroneButton droneButton, DroneOption droneOption)
        {
            if(this.droneButton != null && this.droneButton != droneButton)
            {
                // 버튼 디폴트 상태로 전환
                this.droneButton.ButtonDeActivate();
            }

            this.droneButton = droneButton;

            leftButton.GetComponent<ValueButtonController>().SetInteractable(true);
            rightButton.GetComponent<ValueButtonController>().SetInteractable(true);
            leftButton.GetComponent<ValueButtonController>().Check(true);
            rightButton.GetComponent<ValueButtonController>().Check(true);
            //{ 2025-01-07 유지환 수정 후
            if (droneOption.droneType == 2 || droneOption.flyingOrder == 0)
            {
                leftButton.GetComponent<ValueButtonController>().Check(false);
            }

            if(droneOption.droneType == 2 || droneOption.flyingOrder == 3)
            {
                rightButton.GetComponent<ValueButtonController>().Check(false);
            }
            //}
            #region 2025-01-07 유지환 수정 전
            //if (droneOption.droneType != 2 && droneOption.flyingOrder != 0)
            //{
            //    leftButton.GetComponent<ValueButtonController>().Check(true);
            //}
            //else
            //{
            //    leftButton.GetComponent<ValueButtonController>().Check(false);
            //}
            //if (droneOption.droneType != 2 && droneOption.flyingOrder != 3)
            //{
            //    rightButton.GetComponent<ValueButtonController>().Check(true);
            //}
            //else
            //{
            //    rightButton.GetComponent<ValueButtonController>().Check(false);
            //}
            #endregion
            confirmButton.GetComponent<ButtonController>().SetOn();

            SetDroneInfo(droneOption);

            orderText.text = (droneOption.flyingOrder + 1).ToString();
            backGround.sprite = defaultBackGroundSprite;
            foreach (var text in titleText)
            {
                text.color = new Color(1, 1, 1, 1);
            }
            
            int count = 0;
            foreach(var toggle in formationGroup.GetComponentsInChildren<Toggle>())
            {
                toggle.GetComponent<ToggleController>().SetInteractable(true);

                int index = count;
                if(droneOption.flyingType == index)
                {
                    toggle.isOn = true;
                }
                count++;
            }
            GuideToast.Instance.PopupMSG(MSG.SELECTSTARTPOSITION);
        }

        public void SetOff()
        {

            droneButton?.ButtonDeActivate();
            leftButton.GetComponent<ValueButtonController>().Check(false);
            rightButton.GetComponent<ValueButtonController>().Check(false);
            leftButton.GetComponent<ValueButtonController>().SetInteractable(false);
            rightButton.GetComponent<ValueButtonController>().SetInteractable(false);
            confirmButton.GetComponent<ButtonController>().SetOff();

            foreach (var toggle in formationGroup.GetComponentsInChildren<Toggle>())
            {
                toggle.GetComponent<ToggleController>().SetInteractable(false);
            }
            orderText.text = "";
            backGround.sprite = disableBackGroundSprite;
            foreach (var text in titleText)
            {
                text.color = new Color(115f / 255f, 128f / 255f, 127f / 255f, 1);
            }
        }

        /// <summary>
        /// 변경할 드론 옵션 전달
        /// </summary>
        public void SendOption()
        {
            foreach(var toggle in formationGroup.GetComponentsInChildren<Toggle>())
            {
                if(toggle.isOn)
                {
                    switch (toggle.name)
                    {
                        case "TriangleButton":
                            droneOption.flyingType = 0;
                            break;
                        case "DiamondButton":
                            droneOption.flyingType = 1;
                            break;
                        case "HalfCircleButton":
                            droneOption.flyingType = 2;
                            break;
                    }

                }
            }
            //droneButton.SetDrone(droneOption);
            InstructorScenarioPage.Instance.SetDroneOptionChange(droneOption);
            SetOff();
            droneOption = null;
            droneButton = null;
            GuideToast.Instance.Close();
        }
    }
}

