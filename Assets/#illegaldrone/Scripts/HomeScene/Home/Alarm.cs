using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace SMW
{
    public class Alarm : MonoBehaviour
    {
        public static Alarm instance;

        [SerializeField] GameObject Image_Check;
        [SerializeField] TMP_Text Text_Content;
        [SerializeField] Transform background;

        void Awake()
        {
            instance = this;
        }

        /// <summary>
        /// 알람 세팅
        /// </summary>
        /// <param name="isCheck"> true = Check, false = Warning </param>
        /// <param name="content"> Change Text Content </param>
        public void SetAlarm(bool isCheck, string content)
        {
            background.gameObject.SetActive(true);
            Check(isCheck);
            SetText(content);
            RebuildTransform();
        }

        // Change Text Content
        void SetText(string content)
        {
            Text_Content.text = content;
        }

        // true = Check, false = Warning
        void Check(bool isCheck)
        {
            Image_Check.SetActive(isCheck);
        }

        // Rebuild ContentSizeFitter
        void RebuildTransform()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(background as RectTransform);
        }
    }
}