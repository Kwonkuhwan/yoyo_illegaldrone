using KKH;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SMW
{
    public class SwicthButton : MonoBehaviour
    {
        List<Button> buttons = new List<Button>();

        // 버튼과 이미지 수 맞춰서 넣기
        [Header("Active")]
        [SerializeField] Sprite[] Image_Active;
        [Header("Hover")]
        [SerializeField] Sprite[] Image_Hover;
        [Header("Pressed")]
        [SerializeField] Sprite[] Image_Pressed;
        [Header("NonActive")]
        [SerializeField] Sprite[] Image_NonActive;
        [Header("NonActive_Hover")]
        [SerializeField] Sprite[] Image_NonActive_Hover;
        [Header("NonActive_Pressed")]
        [SerializeField] Sprite[] Image_NonActive_Pressed;

        // N개의 스위치 버튼
        bool[] isOn;
        public int IsOn
        {
            get { return FindSelectButton(); }
        }

        void Start()
        {
            // 자식오브젝트에 있는 버튼들 가져오기
            foreach(Transform button in transform)
            {
                buttons.Add(button.GetComponent<Button>());
            }

            // 첫번째 버튼은 true상태로 시작
            isOn = new bool[buttons.Count];
            for (int i = 0; i < buttons.Count; i++)
            {
                if(i == 0) isOn[i] = true;
                else isOn[i] = false;
            }

            // 각 버튼들 이벤트 추가
            for (int i = 0; i < buttons.Count; i++)
            {
                int index = i;
                buttons[i].onClick.AddListener(()=>
                {
                    OnClickSwicth(index);
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

        // PointerDown 이벤트 함수
        void OnTriggerPointerDown(PointerEventData data, int index)
        {
            if (isOn[index])
            {
                buttons[index].image.sprite = Image_Pressed[index];
            }
            else
            {
                buttons[index].image.sprite = Image_NonActive_Pressed[index];
            }
        }

        // PointerEnter 이벤트 함수
        void OnTriggerPointerEnter(PointerEventData data, int index)
        {
            if (isOn[index])
            {
                buttons[index].image.sprite = Image_Hover[index];
            }
            else
            {
                buttons[index].image.sprite = Image_NonActive_Hover[index];
            }
        }

        // PointerExit 이벤트 함수
        void OnTriggerPointerExit(PointerEventData data, int index)
        {
            if (isOn[index])
            {
                buttons[index].image.sprite = Image_Active[index];
            }
            else
            {
                buttons[index].image.sprite = Image_NonActive[index];
            }
        }

        /// <summary>
        /// 스위치버튼 클릭 이벤트
        /// </summary>
        void OnClickSwicth(int index)
        {
            for (int i = 0; i < isOn.Length; i++)
            {
                if (i == index) isOn[i] = true;
                else isOn[i] = false;

                buttons[i].image.sprite = isOn[i] ? Image_Active[i] : Image_NonActive[i];
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