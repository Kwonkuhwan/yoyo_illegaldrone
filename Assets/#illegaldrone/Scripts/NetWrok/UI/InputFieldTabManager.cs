using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KKH.UI
{
    public class InputFieldTabManager : MonoBehaviour
    {
        [SerializeField] private List<TMP_InputField> list = new List<TMP_InputField>();
        public int curPos;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                CheckFocus();
            }
        }

        /// <summary>
        /// Focus 할 InputField를 설정한다.
        /// </summary>
        /// <param name="idx">Focus 할 InputField의 index 번호</param>
        public void SetFocus(int idx = 0)
        {
            if (idx >= 0 && idx < list.Count)
                list[idx].Select();
        }

        /// <summary>
        /// Tab, Shift+Tab 키를 눌렀을 때 반응 할 InputField를 추가한다.
        /// </summary>
        /// <param name="inputField">추가 할 InputField</param>
        public void Add(TMP_InputField inputField)
        {
            list.Add(inputField);
        }

        /// <summary>
        /// 현재 위치를 얻는다.
        /// </summary>
        /// <returns>현재 위치의 Index</returns>
        private int GetCurerntPos()
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].isFocused == true)
                {
                    curPos = i;
                    break;
                }
            }
            return curPos;
        }

        /// <summary>
        /// 다음 InputField로 Focus한다.
        /// </summary>
        private void MoveNext()
        {
            GetCurerntPos();
                if (curPos < list.Count - 1)
            {
                ++curPos;
            }
            else
            {
                curPos = 0;
            }
            SetFocus(curPos);
            //list[curPos].Select();
        }

        /// <summary>
        /// 이전 InputField로 Focus한다.
        /// </summary>
        private void MovePrev()
        {
            GetCurerntPos();
            if (curPos > 0)
            {
                --curPos;
                SetFocus(curPos);
                //list[curPos].Select();
            }
        }

        /// <summary>
        /// Tab키와 Shift + Tab키를 눌렀는지 체크하여 눌렀으면 Focus를 옮긴다.
        /// </summary>
        public void CheckFocus()
        {
            // 현재 InputField에서 문자열 입력을 허용
            if (list[curPos].isFocused)
            {
                // 포커스를 이동
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    MoveNext();
                }
                else
                {
                    MovePrev();
                }
            }
        }
    }
}