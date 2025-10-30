using KKH;
using KKH.MySQL;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SMW
{
    public class Chart_Log : Chart
    {
        List<LogMessage> logs = new List<LogMessage>();

        string userID;

        bool isStop = true;

        protected override void Awake()
        {
            // 목록최대개수를 5로 고정
            maxCount = 5;

            base.Awake();
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
            // TranieeInfo Chart와 겹치는 거 방지
            if (isStop) return;

            try
            {
                logs.Clear();
                logs = MySQLManager.GetLogMessages(userID, currentPage, maxCount, isAsc, out AllCount);

                if (logs.Count > 0)
                {
                    ReturnContent();
                    ShowChart();
                    for (int i = 0; i < logs.Count; i++)
                    {
                        SetData(CreateContent(), logs[i], i);
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

        public void ReceiveData(UserInfo info)
        {
            userID = info.id;
        }

        /// <summary>
        /// 차트에 데이터 대입
        /// </summary>
        /// <param name="content"></param>
        /// <param name="info"></param>
        void SetData(GameObject content, LogMessage info, int index)
        {
            content.GetComponent<log_Content>().Set(info, CalculatorIndex(index));
        }
    }
}