using KKH;
using KKH.MySQL;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace SMW
{
    public class Calendar : MonoBehaviour
    {
        [Header("Calendar")]
        [SerializeField] TextMeshProUGUI Text_MonthYear;
        [SerializeField] Button Button_Pre;
        [SerializeField] Button Button_Next;

        [Header("Date")]
        [SerializeField] Transform Date;
        List<Day> Days = new List<Day>();

        [Header("Imformation")]
        [SerializeField] GameObject ImformationBoxPrefab;
        [SerializeField] ScrollRect Scroll_Imformation;
        [SerializeField] TextMeshProUGUI Text_Date;
        [SerializeField] TextMeshProUGUI Text_Guide;
        List<GameObject> list_Imformation = new List<GameObject>();
        List<GameObject> pool_imformation = new List<GameObject>();

        int current_year = 0;
        int current_month = 0;

        public static Action<int> EventSelectDay;

        private void Awake()
        {
            EventSelectDay += SelectDay;
        }

        private void OnDestroy()
        {
            EventSelectDay -= SelectDay;
        }

        void Start()
        {
            BindButton();

            // Days리스트에 값 대입
            foreach (Transform day in Date)
            {
                Days.Add(day.GetComponent<Day>());
            }

            current_year = DateTime.Today.Year;
            current_month = DateTime.Today.Month;

            InitCalendar();
            ViewInformation(DateTime.Today.Day);
        }

        void BindButton()
        {
            if (Button_Pre != null)
                Button_Pre.onClick.AddListener(OnClickPreButton);
            if (Button_Next != null)
                Button_Next.onClick.AddListener(OnClickNextButton);
        }

        // 다음달
        public void OnClickNextButton()
        {
            if (current_month >= 12)
            {
                current_year++;
                current_month = 1;
            }
            else
            {
                current_month++;
            }

            InitCalendar();
        }

        // 이전달
        public void OnClickPreButton()
        {
            if (current_month <= 1)
            {
                current_year--;
                current_month = 12;
            }
            else
            {
                current_month--;
            }

            InitCalendar();
        }

        // 캘린더 세팅 초기화
        void InitCalendar()
        {
            int start_day = (int)new DateTime(current_year, current_month, 1).DayOfWeek;
            int max_dayMonth = DateTime.DaysInMonth(current_year, current_month);
            int count = 1;

            // 캘린더 날짜 체크
            for (int i = 0; i < Days.Count; i++)
            {
                if (i >= start_day && i < (max_dayMonth + start_day))
                {
                    DateTime dt = new DateTime(current_year, current_month, count);
                    Days[i].SetLine(GetExistInfoData(dt));
                    Days[i].SetText(count.ToString());
                    count++;
                }
                else
                {
                    Days[i].SetText("");
                }
                // 초기화
                Days[i].ChanageImage(TYPE.NONE);
            }

            // 오늘 일자 체크
            if (DateTime.Today.Year == current_year && DateTime.Today.Month == current_month)
            {
                Days[DateTime.Today.Day + start_day - 1].ChanageImage(TYPE.TODAY);
            }

            SetTextMonthYear();
        }

        // 년월 텍스트 변경
        void SetTextMonthYear()
        {
            Text_MonthYear.text = current_month + " " + current_year;
        }

        void SelectDay(int _day)
        {
            // 예외처리
            if (_day == 0) return;

            for (int i = 0; i < Days.Count; i++)
            {
                // 오늘 일자 체크
                if (DateTime.Today.Year == current_year && DateTime.Today.Month == current_month)
                {
                    if (Days[i].day == DateTime.Today.Day)
                    {
                        if (Days[i].day == _day) ViewInformation(_day);
                        continue;
                    }
                }

                // 선택한 날짜
                if (Days[i].day == _day)
                {
                    Days[i].ChanageImage(TYPE.SELECT);
                    ViewInformation(_day);
                }
                else
                {
                    Days[i].ChanageImage(TYPE.NONE);
                }
            }
        }


        // 데이터 불러오기 send :
        // [KKH][추가][2024.08.27] - DB 연결
        List<ClaendarTrainInfo> GetInfoCalendarData(DateTime dt)
        {
            int resultCount;    // 총 건수
            string resultMsg;   // 리턴 메시지
            List<ClaendarTrainInfo> calendarTrainInfos = MySQLManager.GetCalendarData(dt.ToString("yyyy-MM-dd"), out resultCount, out resultMsg);

            if (calendarTrainInfos != null || calendarTrainInfos.Count > 0)
            {
                return calendarTrainInfos;
            }
            else
            {
                UTILS.LogError("Error GetInfoCalendarData");
                return null;
            }
        }

        // 훈련이력 여부 조회
        bool GetExistInfoData(DateTime dt)
        {
            int resultCount;    // 총 건수
            string resultMsg;   // 리턴 메시지
            List<ClaendarTrainInfo> calendarTrainInfos = MySQLManager.GetCalendarData(dt.ToString("yyyy-MM-dd"), out resultCount, out resultMsg);

            if(calendarTrainInfos != null && calendarTrainInfos.Count > 0)
            {
                return true;
            }
            else if(calendarTrainInfos == null)
            {
                UTILS.LogError("Error GetExistInfoData");
            }
            return false;
        }

        // 선택한 날짜 정보보기
        void ViewInformation(int _day)
        {
            // 선택한 날짜
            DateTime dt = new DateTime(current_year, current_month, _day);
            Text_Date.text = _day + "." + dt.DayOfWeek.ToString();

            try
            {
                // 훈련이력 UI표시
                List<ClaendarTrainInfo> list_Calendarinfo = GetInfoCalendarData(dt);
                ResetListImformation();

                if (list_Calendarinfo.Count > 0)
                {
                    Text_Guide.text = "";
                    for (int i = 0; i < list_Calendarinfo.Count; i++)
                    {
                        // 오브젝트 풀
                        if (pool_imformation.Count > 0)
                        {
                            ImformationBox box = pool_imformation[0].GetComponent<ImformationBox>();
                            pool_imformation[0].gameObject.SetActive(true);
                            list_Imformation.Add(pool_imformation[0]);
                            pool_imformation.Remove(pool_imformation[0]);
                            SetInfoData(box, list_Calendarinfo[i]);
                        }
                        // 오브젝트 생성
                        else
                        {
                            ImformationBox box = Instantiate(ImformationBoxPrefab, Scroll_Imformation.content, false).GetComponent<ImformationBox>();
                            list_Imformation.Add(box.gameObject);
                            SetInfoData(box, list_Calendarinfo[i]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UTILS.LogError(ex);
            }
        }

        // 훈련이력 세팅
        void SetInfoData(ImformationBox _box, ClaendarTrainInfo _info)
        {
            _box.SetTrainResult(_info.missionResult);
            _box.SetTime(_info.playScenarioDateTime);
            _box.SetNumber(_info.playerCnt);
            _box.SetPlace(_info.missionMap);
        }

        // 훈련리스트 초기화
        void ResetListImformation()
        {
            Text_Guide.text = "훈련 이력이 없습니다.";

            for (int i = 0; i < list_Imformation.Count; i++)
            {
                pool_imformation.Add(list_Imformation[i]);
                list_Imformation[i].gameObject.SetActive(false);
            }
            list_Imformation.Clear();
        }
    }
}