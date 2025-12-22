using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ZstdSharp.Unsafe;

namespace SMW
{
    public enum TYPE
    {
        NONE,
        SELECT,
        TODAY
    }

    public class Day : MonoBehaviour
    {
        TYPE _type;
        TextMeshProUGUI _Text;
        Button _button;

        // 배경 이미지
        Image Bg_Image;

        // 라인 이미지
        Image Line_Image;

        // 날짜
        [HideInInspector] public int day;

        [Header("Default")]
        [SerializeField] Sprite Image_Defalut;
        [SerializeField] Sprite Image_DefalutHover;
        [SerializeField] Sprite Image_DefalutPressed;
        [SerializeField] Sprite Image_DefalutFocus;

        [Header("Today")]
        [SerializeField] Sprite Image_Today;
        [SerializeField] Sprite Image_TodayPressed;
        [SerializeField] Sprite Image_TodayHover;

        [Header("Color")]
        [SerializeField] Color Color_Default;

        private void Awake()
        {
            _Text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            Bg_Image = GetComponent<Image>();
            Line_Image = transform.GetChild(1).GetComponent<Image>();
            _button = GetComponent<Button>();

            _button.onClick.AddListener(OnClickSelected);
            AddEventTrigger();
        }

#region 이벤트 추가

        void AddEventTrigger()
        {
            var trigger = _button.GetOrAddComponent<EventTrigger>();

            EventTrigger.Entry[] entry = new EventTrigger.Entry[3];
            entry[0] = new EventTrigger.Entry();
            entry[1] = new EventTrigger.Entry();
            entry[2] = new EventTrigger.Entry();

            entry[0].eventID = EventTriggerType.PointerDown;
            entry[1].eventID = EventTriggerType.PointerEnter;
            entry[2].eventID = EventTriggerType.PointerExit;

            // PointerDown
            entry[0].callback.AddListener((data) =>
            {
                OnTriggerPointerDown();
            });

            // PointerEnter
            entry[1].callback.AddListener((data) =>
            {
                OnTriggerPointerEnter();
            });

            // PointerExit
            entry[2].callback.AddListener((data) =>
            {
                OnTriggerPointerExit();
            });

            trigger.triggers.Add(entry[0]);
            trigger.triggers.Add(entry[1]);
            trigger.triggers.Add(entry[2]);
        }

        // PointerDown 이벤트 함수
        void OnTriggerPointerDown()
        {
            if (_type == TYPE.SELECT) return;
            if (_type == TYPE.TODAY)
            {
                Bg_Image.sprite = Image_TodayPressed;
            }
            else
            {
                Bg_Image.sprite = Image_DefalutPressed;
            }
        }

        // PointerEnter 이벤트 함수
        void OnTriggerPointerEnter()
        {
            if (_type == TYPE.SELECT) return;
            if (_type == TYPE.TODAY)
            {
                Bg_Image.sprite = Image_TodayHover;
            }
            else
            {
                Bg_Image.sprite = Image_DefalutHover;
            }
        }

        // PointerExit 이벤트 함수
        void OnTriggerPointerExit()
        {
            if (_type == TYPE.SELECT) return;
            if (_type == TYPE.TODAY)
            {
                Bg_Image.sprite = Image_Today;
            }
            else
            {
                Bg_Image.sprite = Image_Defalut;
            }
        }

#endregion

        public void ChanageImage(TYPE type)
        {
            switch (type)
            {
                case TYPE.NONE:
                    {
                        _type = TYPE.NONE;
                        Bg_Image.sprite = Image_Defalut;
                        _Text.color = Color_Default;
                    }
                    break;
                case TYPE.SELECT:
                    {
                        _type = TYPE.SELECT;
                        Bg_Image.sprite = Image_DefalutFocus;
                    }
                    break;
                case TYPE.TODAY:
                    {
                        _type = TYPE.TODAY;
                        Bg_Image.sprite = Image_Today;
                        _Text.color = Color.white;
                    }
                    break;
            }
        }

        /// <summary>
        /// 날짜 텍스트 변경
        /// </summary>
        /// <param name="text"></param>
        public void SetText(string text)
        {
            _Text.text = text;

            if(text == "")
            {
                day = 0;
                Bg_Image.enabled = false;
                Line_Image.enabled = false;
            }
            else
            {
                day = int.Parse(text);
                Bg_Image.enabled = true;
            }
        }

        /// <summary>
        /// 훈련이력 여부에 따른 라인 이미지 변경
        /// </summary>
        /// <param name="isExisInfo"></param>
        public void SetLine(bool isExisInfo)
        {
            if (isExisInfo)
            {
                Line_Image.enabled = true;
            }
            else
            {
                Line_Image.enabled = false;
            }
        }

        /// <summary>
        /// 날짜 클릭 이벤트 발생
        /// </summary>
        public void OnClickSelected()
        {
            Calendar.EventSelectDay(day);
        }
    }
}