using KKH.MySQL;
using SMW;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SMW
{
    public class log_Content : MonoBehaviour
    {
        enum Name
        {
            Index = 0,
            Date,
            Log
        }
        List<Transform> datas = new List<Transform>();

        void Awake()
        {
            foreach (Transform t in transform)
            {
                datas.Add(t);
            }
        }

        public void Set(LogMessage info, int index)
        {
            SetData(Name.Index).text = index.ToString();
            SetData(Name.Date).text = $"{info.logWriteDateTime.Year}.{info.logWriteDateTime.Month}.{info.logWriteDateTime.Day} {info.logWriteDateTime.Hour}:{info.logWriteDateTime.Minute}";
            SetData(Name.Log).text = $"훈련생 [{info.traineeID}]_{info.logMsg}_교관 [{info.instructorID}]";
        }

        TMP_Text SetData(Name _name)
        {
            return datas[(int)_name].GetComponentInChildren<TMP_Text>();
        }
    }
}