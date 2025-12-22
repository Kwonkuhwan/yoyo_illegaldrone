using KKH.MySQL;
using SMW;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KKH.HOME.Instructor_Chart
{
    public class Chart_Instructor : Chart
    {
        [Header("검색")]
        [SerializeField] TMP_InputField InputField_Name;

        [Header("Empty")]
        [SerializeField] TMP_Text Text_Empty;

        [Header("AddButton")]
        [SerializeField] private Button btn_Instructor_Add;
        [SerializeField] private TMP_Text text_Instructor_TotalCnt;

        [Header("관리자 정보")]
        [SerializeField] private GameObject Form2;
        [SerializeField] private Chart_TraineeInfo chart_Instructor;

        private List<SearchUserInfo> userInfos = new List<SearchUserInfo>();

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
            SetPeopleNumber();

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

                userInfos = MySQLManager.GetSearchUserInfo(offsetCnt, maxCount, isAsc, _serachName, true, UserGroup.Instructor);
                
                if (userInfos.Count > 0)
                {
                    ReturnContent();
                    ShowChart();
                    for (int i = 0; i < userInfos.Count; i++)
                    {
                        SetData(CreateContent(userInfos[i]), userInfos[i]);
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
        void OnClickChartContent(SearchUserInfo info)
        {
            Form2.GetComponent<Profile>().SetProfile(info.id);
            Form2.SetActive(true);
        }

        protected void SetPeopleNumber()
        {
            MySQLManager.GetUsersCumulativeCount("", UserGroup.Instructor, out AllCount);
            text_Instructor_TotalCnt.text = $"총 관리자 수 ({AllCount})";
        }

        void SetData(GameObject content, SearchUserInfo info)
        {
            content.GetComponent<Chart_Instructor_Content>().Set(info);

            content.GetComponent<Button>().onClick.RemoveAllListeners();
            content.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnClickChartContent(info);
            });
        }

        /// <summary>
        /// 이름 검색
        /// </summary>
        void SearchName(string name)
        {
            if (name == "")
            {
                // 공백이 두번 입력되었습니다.
                if (isDoubleCheck) return;

                isDoubleCheck = true;
                _serachName = "";
                Text_Empty.text = "등록된 관리자(이)가 없습니다.\r\n관리자(을)를 등록해 주세요.";
            }
            else
            {
                isDoubleCheck = false;
                _serachName = name;
                Text_Empty.text = $"<color=#047AFF>{name}</color>와 일치하는 결과가 없습니다.";
            }

            MySQLManager.GetUsersCumulativeCount(name, UserGroup.Instructor, out AllCount);
            text_Instructor_TotalCnt.text = $"총 관리자 수 ({AllCount})";

            ResetPage();
        }
    }
}