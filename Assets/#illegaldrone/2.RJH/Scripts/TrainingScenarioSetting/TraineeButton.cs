using ExitGames.Client.Photon;
using Photon.Realtime;
using RJH.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RJH
{
    public class TraineeButton : MonoBehaviour
    {
        
        [SerializeField] private TextMeshProUGUI traineeName;
        [SerializeField] private Image traineeIcon;
        [SerializeField] private Image readyicon;
        [SerializeField] private Button exitButton;
        public int number;
        public bool isReady;

        private Button button;
        

        [SerializeField] private Sprite[] readyCheckSprite; // 0: readyIcon, 1: NotReadyIcon
        [SerializeField] private Sprite[] traineeJoinSprite; // 0: traineeNumber, 1: plusIcon

        private Player trainee;

        private void Awake()
        {
            button = GetComponent<Button>();
            TraineeNotReady();
            exitButton.onClick.AddListener(TrainKickPopup);
        }

        private void Update()
        {
            // 2025-01-07 유지환 
            if(trainee == null)
                return;

            Hashtable ht = trainee.CustomProperties;

            bool isPlayerReady = false;

            if (ht != null && ht.ContainsKey("PlayerReady"))
            {
                isPlayerReady = (bool)ht["PlayerReady"];
            }

            if (isPlayerReady)
            {
                TraineeReady();
            }
            else
            {
                TraineeNotReady();
            }
        }

        public void SetTrainee(Player trainee)
        {
            this.trainee = trainee;
            traineeName.gameObject.SetActive(true);
            exitButton.gameObject.SetActive(true);
            traineeIcon.sprite = traineeJoinSprite[0];
            traineeIcon.GetComponentInChildren<TextMeshProUGUI>().enabled = true;
            string userInfo = trainee.CustomProperties["UserProperties"].ToString();
            UserProperties userProperties = JsonUtility.FromJson<UserProperties>(userInfo);

            traineeName.text = userProperties.userName;

        }

        public Player GetTrainee()
        {
            return trainee;
        }
        public int GetNumber()
        {
            return number;
        }

        public void TraineeReady()
        {
            readyicon.sprite = readyCheckSprite[0];
        }

        public void TraineeNotReady()
        {
            readyicon.sprite = readyCheckSprite[1];
        }

        private void TrainKickPopup()
        {
            InstructorScenarioPage.Instance.TraineeKickPopup(trainee, this);
        }

        public void TraineeOut()
        {
            // 훈련생 퇴장
            //traineeObj.gameObject.SetActive(false);
            //traineeObj.transform.position = Vector3.zero;
            traineeIcon.sprite = traineeJoinSprite[1];
            traineeIcon.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
            exitButton.gameObject.SetActive(false);
            traineeName.gameObject.SetActive(false);
        }
    }

}


