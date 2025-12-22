using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SMW
{
    public class Bar : MonoBehaviour
    {
        [Header("ButtonGroup")]
        [SerializeField] Transform Button_Groups;
        List<Button> buttons = new List<Button>();

        [Header("PageGroup")]
        [SerializeField] Transform Page_Groups;
        List<GameObject> pages = new List<GameObject>();

        GameObject current_page;
        GameObject previous_page;

        private void Awake()
        {
            init();
        }

        void init()
        {
            foreach (Transform t in Button_Groups)
            {
                buttons.Add(t.GetComponent<Button>());
            }

            for (int i = 0; i < buttons.Count; i++)
            {
                int index = i;
                buttons[i].onClick.AddListener(() =>
                {
                    OnClickMenuButton(index);
                });
            }

            foreach (Transform t in Page_Groups)
            {
                pages.Add(t.gameObject);
            }
        }

        /// <summary>
        /// 메뉴 바 버튼 클릭 이벤트
        /// </summary>
        /// <param name="number"></param>
        void OnClickMenuButton(int number)
        {
            for (int i = 0; i < pages.Count; i++)
            {
                if(i == number)
                {
                    pages[i].SetActive(true);
                }
                else
                {
                    pages[i].SetActive(false);
                }
            }
        }
    }
}