using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SMW
{
    public class ButtonEvent : MonoBehaviour
    {
        public bool IsActive { get { return isActive; } }
        bool isActive = false;

        [Header("ImageComponent")]
        [SerializeField] Image _image;

        [Header("Sprite Active")]
        [SerializeField] Sprite Sprite_Active;
        [SerializeField] Sprite Sprite_Active_Hover;
        [SerializeField] Sprite Sprite_Active_Pressed;

        [Header("Sprite_Defalut")]
        [SerializeField] Sprite Sprite_Defalut;
        [SerializeField] Sprite Sprite_Defalut_Hover;
        [SerializeField] Sprite Sprite_Defalut_Pressed;

        EventTrigger _trigger;

        private void Awake()
        {
            _trigger = transform.GetOrAddComponent<EventTrigger>();
            AddTrigger(_trigger);
        }

        /// <summary>
        /// 버튼 활성화
        /// </summary>
        public void Activate()
        {
            isActive = true;
            ChangeImage(Sprite_Active);
        }

        /// <summary>
        /// 버튼 비활성화
        /// </summary>
        public void Deactivate()
        {
            isActive = false;
            ChangeImage(Sprite_Defalut);
        }

        public void Toggle(bool activate)
        {
            if (activate)
            {
                Activate();
            }
            else
            {
                Deactivate();
            }
        }

        // 버튼 다운 이벤트 추가
        public void AddEvent(Action action)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();

            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) =>
            {
                action();
            });

            _trigger.triggers.Add(entry);
        }

        /// <summary>
        /// 이벤트 트리거 추가
        /// </summary>
        /// <param name="trigger"> 버튼 EventTrigger 컴포넌트 </param>
        void AddTrigger(EventTrigger trigger)
        {
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
                OnTriggerPointerDown((PointerEventData)data);
            });

            // PointerEnter
            entry[1].callback.AddListener((data) =>
            {
                OnTriggerPointerEnter((PointerEventData)data);
            });

            // PointerExit
            entry[2].callback.AddListener((data) =>
            {
                OnTriggerPointerExit((PointerEventData)data);
            });

            trigger.triggers.Add(entry[0]);
            trigger.triggers.Add(entry[1]);
            trigger.triggers.Add(entry[2]);
        }

        #region 이벤트

        // PointerDown 이벤트 함수
        void OnTriggerPointerDown(PointerEventData data)
        {
            if (isActive)
            {
                ChangeImage(Sprite_Active_Pressed);
            }
            else
            {
                ChangeImage(Sprite_Defalut_Pressed);
            }
        }

        // PointerEnter 이벤트 함수
        void OnTriggerPointerEnter(PointerEventData data)
        {
            if (isActive)
            {
                ChangeImage(Sprite_Active_Hover);
            }
            else
            {
                ChangeImage(Sprite_Defalut_Hover);
            }
        }

        // PointerExit 이벤트 함수
        void OnTriggerPointerExit(PointerEventData data)
        {
            if (isActive)
            {
                ChangeImage(Sprite_Active);
            }
            else
            {
                ChangeImage(Sprite_Defalut);
            }
        }

        #endregion

        /// <summary>
        /// 이미지 변환
        /// </summary>
        /// <param name="_sprite"> 스프라이트 이미지 </param>
        void ChangeImage(Sprite _sprite)
        {
            if (_sprite == null) return;

            if(_image == null) _image = transform.GetComponent<Image>();
            _image.sprite = _sprite;
        }
    }
}
