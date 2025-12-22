using KKH;
using KKH.MySQL;
using Photon.Pun;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SMW
{
    public class Chart_Traniee : Chart
    {
        [Header("검색")]
        [SerializeField] TMP_InputField InputField_Name;

        [Header("총 훈련생 수")]
        [SerializeField] TMP_Text Text_TranieeNumber;

        [Header("Empty")]
        [SerializeField] TMP_Text Text_Empty;

        [Header("훈련생 정보")]
        [SerializeField] GameObject Form2;
        [SerializeField] Chart_TraineeInfo Chart_TraineeInfo;

        // 훈련 정보들
        List<TraineeInfo> infos = new List<TraineeInfo>();

        // 검색 공백 두번체크
        bool isDoubleCheck;

        string _serachName = "";
        int offsetCnt;

        protected override void Awake()
        {
            base.Awake();

            InputField_Name.onEndEdit.AddListener(SearchName);
        }

        private void OnEnable()
        {
            MySQLManager.GetUsersCumulativeCount("", UserGroup.Trainee, out AllCount);
            Text_TranieeNumber.text = $"총 훈련생 수 ({AllCount})";

            isDoubleCheck = false;

            ResetPage();
        }

        protected override void GetData()
        {
            try
            {
                if (currentPage != 1)
                {
                    offsetCnt = (currentPage - 1) * maxCount + 1;
                }
                else offsetCnt = 1;

                infos = MySQLManager.GetTraineeInfos(offsetCnt, maxCount, isAsc, _serachName, true, UserGroup.Trainee);

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

        /// <summary>
        /// 차트 데이터 클릭
        /// </summary>
        void OnClickChartContent(TraineeInfo info)
        {
            Form2.GetComponent<Profile>().SetProfile(info.id);
            Form2.SetActive(true);
        }

        /// <summary>
        /// Row에 데이터 대입
        /// </summary>
        /// <param name="content"> 표 </param>
        /// <param name="info"> 데이터 </param>
        void SetData(GameObject content, TraineeInfo info, int index)
        {
            content.GetComponent<Traniee_Content>().Set(info, CalculatorIndex(index));

            content.GetComponent<Button>().onClick.RemoveAllListeners();
            content.GetComponent<Button>().onClick.AddListener(()=>
            {
                OnClickChartContent(info);
            });

        }

        /// <summary>
        /// 이름 검색
        /// </summary>
        void SearchName(string name)
        {
            if(name == "")
            {
                // 공백이 두번 입력되었습니다.
                if (isDoubleCheck) return;

                isDoubleCheck = true;
                _serachName = "";
                Text_Empty.text = "등록된 훈련생이 없습니다.\r\n훈련생을 등록해 주세요.";
            }
            else
            {
                isDoubleCheck = false;
                _serachName = name;
                Text_Empty.text = $"<color=#047AFF>{name}</color>와 일치하는 결과가 없습니다.";
            }

            MySQLManager.GetUsersCumulativeCount(name, UserGroup.Trainee, out AllCount);
            Text_TranieeNumber.text = $"총 훈련생 수 ({AllCount})";

            ResetPage();
        }
    }
}