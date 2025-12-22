using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SMW                      
{
    public class Switch_Log : MonoBehaviour
    {
        [Header("Chart")]
        [SerializeField] GameObject Chart_TranieeInfo;
        [SerializeField] GameObject Chart_Log;

        List<Button> buttons = new List<Button>();

        Color Color_Disable;

        private void Awake()
        {
            int index = 0;
            foreach(Transform t in transform)
            {
                int i = index;
                buttons.Add(t.GetComponent<Button>());
                t.GetComponent<Button>().onClick.AddListener(() =>
                {
                    OnClickSwicthButton(i);
                });
                index++;
            }

            ColorUtility.TryParseHtmlString("#5D6168", out Color_Disable);
        }

        void OnClickSwicthButton(int index)
        {
            // 데이터베이스 표 변환
            if (index == 0)
            {
                Chart_TranieeInfo.SetActive(true);
                Chart_Log.SetActive(false);
            }
            else
            {
                Chart_TranieeInfo.SetActive(false);
                Chart_Log.SetActive(true);
            }

            // 버튼 활성화/비활성화
            for (int i = 0; i < buttons.Count; i++)
            {
                if(i == index)
                {
                    buttons[i].GetComponent<Image>().enabled = true;
                    buttons[i].transform.GetChild(0).GetComponent<TMP_Text>().color = Color.white;
                }
                else
                {
                    buttons[i].GetComponent<Image>().enabled = false;
                    buttons[i].transform.GetChild(0).GetComponent<TMP_Text>().color = Color_Disable;
                }
            }
        }
    }
}