using Illegaldrone;
using KKH;
using KKH.MySQL;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SMW
{
    public class Chart_Traning : Chart
    {
        [Header("isMine")]
        [SerializeField] Button Button_Mine;
        [SerializeField] Button Button_All;
        [SerializeField] TMP_Text Text_Mine;
        [SerializeField] TMP_Text Text_All;
        [SerializeField] Color Color_NonActive;

        // 훈련 정보들
        List<TrainingInfo> infos = new List<TrainingInfo>();

        protected override void Awake()
        {
            base.Awake();

            Bind();
            isMine = true;      // 내 훈련보기
            SetPeopleNumber();
        }

        private void OnEnable()
        {
            SetPeopleNumber();
            ResetPage();
        }

        void Bind()
        {
            // 내 훈련 보기
            Button_Mine.onClick.AddListener(() =>
            {
                isMine = true;

                ResetPage();

                Button_Mine.GetComponent<Image>().enabled = true;
                Button_All.GetComponent<Image>().enabled = false;
                Text_Mine.color = Color.white;
                Text_All.color = Color_NonActive;
            });
            // 모든 훈련 보기
            Button_All.onClick.AddListener(() =>
            {
                isMine = false;

                ResetPage();

                Button_Mine.GetComponent<Image>().enabled = false;
                Button_All.GetComponent<Image>().enabled = true;
                Text_Mine.color = Color_NonActive;
                Text_All.color = Color.white;
            });
        }

        protected override void GetData()
        {
            try
            {
                infos = MySQLManager.GetTrainingInfos(GameManager.instance.userInfo.id, isMine, currentPage, maxCount, isAsc);

                if (infos.Count > 0)
                {
                    ReturnContent();
                    ShowChart();
                    for (int i = 0; i < infos.Count; i++)
                    {
                        SetData(CreateContent(), infos[i], i);
                    }
                }
                else
                {
                    HideChart();
                }

                base.GetData();
            }
            catch (Exception e)
            {
                UTILS.LogError($"Error: {e}");
            }
        }

        void SetPeopleNumber()
        {
            MySQLManager.GetTrainingCumulativeCount(GameManager.instance.userInfo.id, true, out MineCount);
            Button_Mine.GetComponentInChildren<TMP_Text>().text = $"내 훈련 ({MineCount})";

            MySQLManager.GetTrainingCumulativeCount("", false, out AllCount);
            Button_All.GetComponentInChildren<TMP_Text>().text = AllCount > 999 ? $"모든 훈련 ({AllCount}+)" : $"모든 훈련 ({AllCount})";
        }

        /// <summary>
        /// 테이블에 데이터 기입
        /// </summary>
        /// <param name="content"> 테이블 오브젝트 </param>
        /// <param name="info"> 데이터 정보 </param>
        void SetData(GameObject content, TrainingInfo info, int index)
        {
            content.GetComponent<Chart_Content>().Set(info, CalculatorIndex(index));
        }
    }
}