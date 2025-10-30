using UnityEngine;
using UnityEngine.UI;
using RJH.UI;
using TMPro;

namespace RJH
{
    public class DroneButton : MonoBehaviour
    {
        public DroneOption droneOption;

        private Button button;
        public Button Button => (button = GetComponent<Button>());
        [SerializeField] private Image buttonImage;
        [SerializeField] private Image flyTypeImage;
        [SerializeField] private Image droneTypeImage;
        [SerializeField] private Sprite[] flyTypeSprites;
        [SerializeField] private Sprite[] flyTypeSprites_activate;
        [SerializeField] private TextMeshProUGUI flyOrderText;
        
        [Space(10)]
        [Header("Sprites")]
        [SerializeField] private Sprite buttonSprite;
        [SerializeField] private Sprite buttonSprite_activate;
        [SerializeField] private Sprite droneActive;
        [SerializeField] private Sprite droneInactive;

        [Space(10)]
        [Header("DronesInfo")]
        // [KKH][추가][2024.10.15] - 드론 오브젝트 접근용으로 추가함
        [SerializeField] private MapObjectController mapObjectController;

        private void Awake()
        {
            button = GetComponent<Button>();
            buttonImage = GetComponent<Image>();
            button.onClick.AddListener(ButtonAction);
        }

        /// <summary>
        /// 버튼을 눌렀을때 실행할 액션, 드론 오브젝트가 활성화 되어있으면 드론 옵션팝업 실행, 비활성화 되어있으면 활성화 실행
        /// </summary>
        private void ButtonAction()
        {
            //InstructorScenarioPage.Instance.DroneButtonAction(droneOption.droneType);
        }

        public void ButtonActive()
        {
            buttonImage.sprite = buttonSprite_activate;
            flyTypeImage.sprite = flyTypeSprites_activate[droneOption.flyingType];

            #region 2025-01-07 유지환 InstructorScenarioPage.cs 221줄로 이동
            // [KKH][추가][24.10.17] 드론 버튼을 눌렀을때 드론 UI 활성화 추가
            //mapObjectController.Drones[droneOption.droneType].GetComponent<DroneObj>().objController.DronesUIActive(true);
            #endregion
        }

        public void ButtonDeActivate()
        {
            buttonImage.sprite = buttonSprite;
            flyTypeImage.sprite = flyTypeSprites[droneOption.flyingType];
        }

        public void DroneDeActivate()
        {
            buttonImage.sprite = buttonSprite;
            flyTypeImage.sprite = flyTypeSprites[droneOption.flyingType];
            droneTypeImage.sprite = droneInactive;
            flyTypeImage.gameObject.SetActive(false);
        }

        public void DroneActivate()
        {
            buttonImage.sprite = buttonSprite;
            flyTypeImage.sprite = flyTypeSprites[droneOption.flyingType];
            droneTypeImage.sprite = droneActive;
            flyTypeImage.gameObject.SetActive(true);
        }
            

        /// <summary>
        /// 비행 순서 텍스트 표시 변경
        /// </summary>
        /// <param name="order">비행 순서 번호</param>
        public void SetDrone(DroneOption option)
        {
            droneOption.flyingType = option.flyingType;
            droneOption.flyingOrder = option.flyingOrder;
            #region 2025-01-07 유지환 InstructorScenarioPage의 SetDroneOptionChange로 이동
            //mapObjectController.Drones[option.droneType].GetComponent<DroneObj>().SetDroneInfo(option);
            #endregion
            ButtonDeActivate();
            flyOrderText.text = (droneOption.flyingOrder + 1).ToString();
        }
    }
}