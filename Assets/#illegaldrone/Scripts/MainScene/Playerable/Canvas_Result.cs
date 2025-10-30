using Illegaldrone;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SMW
{
    public class TraineeInfo_Result
    {
        public string Name;
        public int TraineeNumber;
        public string Area;
        public int KillCount;

        public TraineeInfo_Result(string name, int traineeNumber, string area, int killCount)
        {
            Name = name;
            TraineeNumber = traineeNumber;
            Area = area;
            KillCount = killCount;
        }
    }

    public class Canvas_Result : MonoBehaviour
    {

        [Serializable]
        public struct UI
        {
            public Image Title;
            public Traniee[] Box;
            public TMP_Text Ranking;
            public TMP_Text Description;

            [Serializable]
            public struct Traniee
            {
                public GameObject obj;
                public GameObject Fail;
                public GameObject Focus;
                public TMP_Text Rank;
                public TMP_Text Name;
                public TMP_Text Area;
                public Image Drone;
                public TMP_Text Score;
            }
        }
        [SerializeField] UI ui;

        private List<TraineeInfo_Result> traineeResultsList = new List<TraineeInfo_Result>();

        public Sprite Title_Success;
        public Sprite Title_Fail;

        public Sprite DroneDefault;
        public Sprite DroneFocus;

        [SerializeField] Camera mainCam;

        private void Awake()
        {
            if (mainCam == null)
            {
                mainCam = Camera.main;
            }
        }

        private void Update()
        {
            // 화면이 항상 카메라를 바라보도록 설정
            transform.LookAt(mainCam.transform.position);

            // 화면을 카메라가 바라보는 방향으로 일정 거리만큼 떨어뜨리기
            Vector3 direction = mainCam.transform.forward.normalized;
            transform.position = mainCam.transform.position + direction * 1.5f; // 2.0f는 원하는 거리
        }

        public void SetTitle(bool isSuccess)
        {
            ui.Title.sprite = isSuccess ? Title_Success : Title_Fail;

            if (isSuccess)
            {
                ui.Description.text = "수호에 성공했습니다.\nHMD를 벗고 자리에서 잠시 대기해 주세요.";
            }
            else
            {
                ui.Description.text = "수호에 실패했습니다.\nHMD를 벗고 자리에서 잠시 대기해 주세요.";
            }
        }

        // 하얀색 #FFFFFF
        //  검정 #303134

        public void SetData(string name, int traineeNumber, string area, int kill)
        {
            traineeResultsList.Add(new TraineeInfo_Result(name, traineeNumber, area, kill));
            SortByKillCount();

            int count = 1;
            foreach (TraineeInfo_Result traineeInfo in traineeResultsList)
            {
                int index = count;
                SetRanking(index, index, traineeInfo.Name, traineeInfo.Area, traineeInfo.KillCount);
                count++;
            }
        }

        private void SortByKillCount()
        {
            traineeResultsList = traineeResultsList.OrderByDescending(x => x.KillCount).ThenBy(t => t.Name).ThenBy(t => t.TraineeNumber).ToList();
        }

        public void SetRanking(int index, int rank, string name, string area, int kill)
        {
            ui.Box[rank - 1].obj.SetActive(true);

            if (kill >= 1)
            {
                ui.Box[rank - 1].Fail.SetActive(false);
                ui.Box[rank - 1].obj.GetComponent<CanvasGroup>().alpha = 1f;
            }
            else
            {
                ui.Box[rank - 1].Fail.SetActive(true);
                ui.Box[rank - 1].obj.GetComponent<CanvasGroup>().alpha = 0.2f;
            }

            // 내 랭크
            if (name == GameManager.instance.userInfo.userName)
            {
                ui.Box[rank - 1].Rank.text = $"<color=#303134>{rank.ToString()}번</color>";
                ui.Box[rank - 1].Name.text = $"<color=#303134>{name}</color>";
                ui.Box[rank - 1].Area.text = $"<color=#303134>{area}</color>";
                ui.Box[rank - 1].Drone.sprite = DroneFocus;
                ui.Box[rank - 1].Score.text = $"<color=#303134>{kill.ToString("00")}</color>";
                ui.Box[rank - 1].Focus.SetActive(true);
                ui.Box[rank - 1].obj.GetComponent<CanvasGroup>().alpha = 1f;
                SetMyRank(rank);
            }
            else
            {
                ui.Box[rank - 1].Rank.text = $"<color=#FFFFFF>{rank.ToString()}번</color>";
                ui.Box[rank - 1].Name.text = $"<color=#FFFFFF>{name}</color>";
                ui.Box[rank - 1].Area.text = $"<color=#FFFFFF>{area}</color>";
                ui.Box[rank - 1].Drone.sprite = DroneDefault;
                ui.Box[rank - 1].Score.text = $"<color=#FFFFFF>{kill.ToString("00")}</color>";
                ui.Box[rank - 1].Focus.SetActive(false);
            }
        }

        public void SetMyRank(int rank)
        {
            ui.Ranking.text = $"내 기여도 순위 <color=#FFC700>{rank}위</color>";
        }
    }
}