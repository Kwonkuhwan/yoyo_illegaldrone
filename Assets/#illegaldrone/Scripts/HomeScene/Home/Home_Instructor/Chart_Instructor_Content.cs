using KKH.MySQL;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KKH.HOME.Instructor_Chart
{
    public class Chart_Instructor_Content : MonoBehaviour
    {
        enum Name
        {
            Index = 0,
            Date,
            ID,
            Name,
            ScenarioCount,
            PlaySceanrioDateTime
        }

        [SerializeField] private Button button;
        [SerializeField] private GameObject userInfo_Canvas;

        private List<Transform> datas = new List<Transform>();

        void Awake()
        {
            foreach (Transform t in transform)
            {
                datas.Add(t);
            }

            if (button == null)
            {
                button = GetComponent<Button>();
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => BtnClick());
        }

        public void Set(SearchUserInfo info)
        {
            SetData(Name.Index).text = info.idx.ToString();
            SetData(Name.Date).text = $"{info.createIDDate.Year}.{info.createIDDate.Month}.{info.createIDDate.Day}";
            SetData(Name.ID).text = info.id.ToString();
            SetData(Name.Name).text = info.userName.ToString();
            SetData(Name.ScenarioCount).text = info.scenarioCount.ToString();
            SetData(Name.PlaySceanrioDateTime).text = info.playSceanrioDateTime.ToString();
        }

        private TMP_Text SetData(Name _name)
        {
            return datas[(int)_name].GetComponentInChildren<TMP_Text>();
        }

        private void BtnClick()
        {
            if (userInfo_Canvas != null && !userInfo_Canvas.activeInHierarchy)
            {
                userInfo_Canvas.SetActive(true);
            }
        }
    }
}