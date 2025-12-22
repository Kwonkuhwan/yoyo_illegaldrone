using UnityEngine;
using UnityEngine.UI;

namespace RJH.UI
{
    public class DroneTypeDeletePopup : MonoBehaviour
    {
        [SerializeField] private Button deleteButton;
        [SerializeField] private Button refuseButton;

        private int droneType = 0;

        private void Awake()
        {
            deleteButton.onClick.AddListener(OnDelete);
            refuseButton.onClick.AddListener(OnRefuse);
        }

        public void SetDeletePopup(int number)
        {
            gameObject.SetActive(true);
            droneType = number;
        }

        private void OnRefuse()
        {
            gameObject.SetActive(false);
        }

        private void OnDelete()
        {
            InstructorScenarioPage.Instance.DroneObjDelete(droneType);
            gameObject.SetActive(false);
        }
    }
}
