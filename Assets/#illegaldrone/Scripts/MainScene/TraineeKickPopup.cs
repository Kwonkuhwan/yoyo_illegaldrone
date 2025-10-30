using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace RJH.UI
{
    public class TraineeKickPopup : MonoBehaviour
    {
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button kickButton;

        private Player trainee;
        private int number;
        private TraineeButton traineeButton;

        private void Awake()
        {
            cancelButton.onClick.AddListener(OnCancel);
            kickButton.onClick.AddListener(OnKickTrainee);
        }

        public void SetPopup(Player player, TraineeButton traineeButton)
        {
            gameObject.SetActive(true);
            trainee = player;
            this.number = traineeButton.number;
            this.traineeButton = traineeButton;
        }

        private void OnCancel()
        {
            gameObject.SetActive(false);
        }

        private void OnKickTrainee()
        {
            InstructorScenarioPage.Instance.TraineeDelete(trainee);
            //traineeButton.TraineeOut();
            InstructorScenarioPage.Instance.TraineeOut(trainee);
            gameObject.SetActive(false);
        }
    }

}


