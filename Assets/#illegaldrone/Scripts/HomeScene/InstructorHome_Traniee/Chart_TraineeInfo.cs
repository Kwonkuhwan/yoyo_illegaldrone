using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KKH.MySQL;
using System;
using KKH;

namespace SMW
{
    public class Chart_TraineeInfo : Chart
    {
        List<TraineeTrainInfo> infos = new List<TraineeTrainInfo>();

        string userID;      // 선택한 유저 ID
        string userName;
        int maxTraningCount;
        int maxTraningSuccessCount;

        [Header("Profile")]
        [SerializeField] Profile profile;

        bool isStop = true;

        protected override void Awake()
        {
            base.Awake();

            // 목록최대개수를 5로 고정
            maxCount = 5;
        }

        private void OnEnable()
        {
            isStop = false;
            ResetPage();
        }

        private void OnDisable()
        {
            isStop = true;
        }

        protected override void GetData()
        {
            // Log Chart와 겹치는거 방지
            if (isStop) return;

            try
            {
                infos.Clear();
                infos = MySQLManager.GetTraineeTrainInfos(userID, currentPage, maxCount, isAsc);            //maxCount = 5로 고정

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

            base.GetData();
        }

        /// <summary>
        /// 훈련생 정보 데이터 받아오기
        /// </summary>
        public void ReceiveData(UserInfo info)
        {
            userID = info.id;
            AllCount = info.missionResult_count;
        }

        /// <summary>
        /// 차트에 데이터 대입
        /// </summary>
        /// <param name="content"></param>
        /// <param name="info"></param>
        void SetData(GameObject content, TraineeTrainInfo info, int index)
        {
            content.GetComponent<TraningInfo_Content>().Set(info, CalculatorIndex(index), profile.InstructorName);
        }
    }
}