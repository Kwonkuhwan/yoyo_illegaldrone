using KKH;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SMW
{
    public class SelectButton : MonoBehaviour
    {
        List<Button> buttons = new List<Button>();

        // 이미지
        [Header("Defalut")]
        [SerializeField] Sprite Image_Defalut;
        [Header("Active")]
        [SerializeField] Sprite Image_Active;
        [Header("Hover")]
        [SerializeField] Sprite Image_Hover;

        [Header("Icon")]
        [SerializeField] Sprite[] Icons_Active;
        [SerializeField] Sprite[] icons_NonActive;
        List<Image> icons = new List<Image>();


        // 눌린 버튼 체크용
        bool[] isOn;

        public int IsOn
        {
            get { return FindSelectButton(); }
        }

        /// <summary>
        /// 버튼들 다 끄기
        /// </summary>
        public void All_OFF()
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                isOn[i] = false;

                buttons[i].image.sprite = Image_Defalut;
                icons[i].sprite = icons_NonActive[i];
            }
        }

        void Start()
        {
            // 자식오브젝트에 있는 버튼들 가져오기
            foreach (Transform button in transform)
            {
                buttons.Add(button.GetComponent<Button>());

                try
                {
                    icons.Add(button.GetChild(0).GetComponent<Image>());
                }
                catch
                {
                    UTILS.Log("아이콘 오브젝트의 위치를 확인해주세요.");
                }
            }

            // 첫번째 버튼은 true상태로 시작
            isOn = new bool[buttons.Count];
            OnClickButton(0);

            // 각 버튼들 이벤트 추가
            for (int i = 0; i < buttons.Count; i++)
            {
                int index = i;
                buttons[i].onClick.AddListener(() =>
                {
                    OnClickButton(index);
                });
                var v = buttons[i].GetOrAddComponent<EventTrigger>();
                AddTrigger(i, v);
            }
        }

        /// <summary>
        /// 이벤트 트리거 추가
        /// </summary>
        /// <param name="index"> 번호 </param>
        /// <param name="trigger"> 버튼 EventTrigger 컴포넌트 </param>
        void AddTrigger(int index, EventTrigger trigger)
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
                OnTriggerPointerDown((PointerEventData)data, index);
            });

            // PointerEnter
            entry[1].callback.AddListener((data) =>
            {
                OnTriggerPointerEnter((PointerEventData)data, index);
            });

            // PointerExit
            entry[2].callback.AddListener((data) =>
            {
                OnTriggerPointerExit((PointerEventData)data, index);
            });

            trigger.triggers.Add(entry[0]);
            trigger.triggers.Add(entry[1]);
            trigger.triggers.Add(entry[2]);
        }

        #region 이벤트

        // PointerDown 이벤트 함수
        void OnTriggerPointerDown(PointerEventData data, int index)
        {
            return;
        }

        // PointerEnter 이벤트 함수
        void OnTriggerPointerEnter(PointerEventData data, int index)
        {
            if (isOn[index]) return;

            buttons[index].image.sprite = Image_Hover;
        }

        // PointerExit 이벤트 함수
        void OnTriggerPointerExit(PointerEventData data, int index)
        {
            if (isOn[index]) return;

            buttons[index].image.sprite = Image_Defalut;
        }

        #endregion

        /// <summary>
        /// 스위치버튼 클릭 이벤트
        /// </summary>
        void OnClickButton(int index)
        {
            for (int i = 0; i < isOn.Length; i++)
            {
                if (i == index) isOn[i] = true;
                else isOn[i] = false;

                buttons[i].image.sprite = isOn[i] ? Image_Active : Image_Defalut;
                icons[i].sprite = isOn[i] ? Icons_Active[i] : icons_NonActive[i];
            }
        }

        int FindSelectButton()
        {
            for (int i = 0; i < isOn.Length; i++)
            {
                if (isOn[i] == true)
                {
                    return i;
                }
            }
            return 0;
        }
    }
}