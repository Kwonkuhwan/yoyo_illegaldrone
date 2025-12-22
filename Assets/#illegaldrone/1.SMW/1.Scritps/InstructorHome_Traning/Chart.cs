using KKH.MySQL;
using Photon.Pun;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SMW
{
    public class Chart : MonoBehaviour
    {
        List<GameObject> layouts = new List<GameObject>();
        // 차트 데이터 리스트
        List<GameObject> contents = new List<GameObject>();
        // 페이지
        List<Transform> numbers = new List<Transform>();

        [Header("Prefab")]
        [SerializeField] GameObject Chart_content;
        [Header("Scroll View")]
        [SerializeField] ScrollRect ScrollRect;
        [Header("PageNumber")]
        [SerializeField] GameObject PageNumber;
        [SerializeField] Button Button_Pre;
        [SerializeField] Button Button_Next;
        RectTransform Page;
        Color Color_Page_NonActive;

        [Header("MaxCount")]
        [SerializeField] TMP_Dropdown Dropdown_MaxCount;
        [Header("Asc")]
        [SerializeField] Button Dropdown_Array;
        [SerializeField] TMP_Text Text_Array;
        [SerializeField] Image Image_Array;
        [SerializeField] Sprite Sprite_Asc;
        [SerializeField] Sprite Sprite_Des;


        // 페이지 인덱스 값
        protected int pageIndex = 0;
        int content_Index = 0;

        // 훈련 수
        protected int MineCount = 0;        // 내 훈련 개수
        protected int AllCount = 0;         // 모든 훈련 개수

        // 페이지 마지막 넘버
        int pageLastNumber = 1;

        [Header("Send Data")]
        protected int currentPage = 1;
        protected int maxCount = 10;        // 최대 목록 개수
        protected bool isAsc = false;
        protected bool isMine = false;
        protected string creator = "";

        protected virtual void Awake()
        {
            ColorUtility.TryParseHtmlString("#414449", out Color_Page_NonActive);
            creator = PhotonNetwork.LocalPlayer.NickName;
            List_Init();
            Bind();
        }

        /// <summary>
        /// 리스트 대입
        /// </summary>
        void List_Init()
        {
            foreach (Transform obj in transform)
            {
                layouts.Add(obj.gameObject);
            }

            foreach (Transform num in PageNumber.transform)
            {
                numbers.Add(num);
                int index = numbers.IndexOf(num);       // 버튼 리스트의 인덱스 값
                num.GetComponent<Button>().onClick.AddListener(() =>
                {
                    SelectPageNum(index);
                });
            }

            Page = PageNumber.transform.parent.GetComponent<RectTransform>();
        }

        /// <summary>
        /// UI 이벤트
        /// </summary>
        void Bind()
        {
            // 이전 페이지
            Button_Pre.onClick.AddListener(()=>
            {
                if (pageIndex < 1) return;

                pageIndex--;
                currentPage = (pageIndex * 5) + 1;

                SetPage();
                SelectPageNum(0);
            });
            // 다음 페이지
            Button_Next.onClick.AddListener(()=>
            {
                pageIndex++;
                currentPage = (pageIndex * 5) + 1;

                SetPage();
                SelectPageNum(0);
            });

            // 최대 목록 수
            if (Dropdown_MaxCount != null)
            {
                Dropdown_MaxCount.onValueChanged.AddListener((data) =>
                {
                    switch (data)
                    {
                        case 0:
                            {
                                maxCount = 10;
                            }
                            break;
                        case 1:
                            {
                                maxCount = 25;
                            }
                            break;
                        case 2:
                            {
                                maxCount = 50;
                            }
                            break;
                    }
                    ResetPage();
                });
            }
            
            // 오름차순 내림차순
            Dropdown_Array.onClick.AddListener(() =>
            {
                isAsc = !isAsc;
                if(isAsc)
                {
                    Image_Array.sprite = Sprite_Des;
                    Text_Array.text = "오래된 훈련일순";
                }
                else
                {
                    Image_Array.sprite = Sprite_Asc;
                    Text_Array.text = "최근 훈련일순";
                    
                }
                GetData();
            });
        }

        /// <summary>
        /// 페이지 선택
        /// </summary>
        /// <param name="index"> 0, 1, 2, 3, 4 총 5개의 인덱스 값이 들어간다 </param>
        void SelectPageNum(int index)
        {
            for (int i = 0; i < numbers.Count; i++)
            {
                if(i == index)
                {
                    numbers[i].GetComponent<ButtonEvent>().Activate();
                    numbers[i].GetComponentInChildren<TMP_Text>().color = Color.white;
                }
                else
                {
                    numbers[i].GetComponent<ButtonEvent>().Deactivate();
                    numbers[i].GetComponentInChildren<TMP_Text>().color = Color_Page_NonActive;
                }
            }
            currentPage = int.Parse(numbers[index].GetComponentInChildren<TMP_Text>().text);
            GetData();
        }

        void SetPage()
        {
            // 최대 페이지 수 ex) 6page
            if(isMine)
            {
                pageLastNumber = MineCount / maxCount;
                if (MineCount % maxCount > 0) pageLastNumber += 1;
            }
            else
            {
                pageLastNumber = AllCount / maxCount;
                if (AllCount % maxCount > 0) pageLastNumber += 1;
            }

            // 현재 목록 ex) 0 : 1,2,3,4,5 page
            // 이전,다음 버튼 활성화 여부
            if(pageIndex < 1)
            {
                Button_Pre.gameObject.SetActive(false);
            }
            else
            {
                Button_Pre.gameObject.SetActive(true);
            }
            if((pageIndex + 1) * 5 >= pageLastNumber)
            {
                Button_Next.gameObject.SetActive(false);
            }
            else
            {
                Button_Next.gameObject.SetActive(true);
            }

            int index = pageIndex * 5 + 1;
            // 페이지 활성화 여부 및 번호 갱신
            for (int i = 0; i < numbers.Count; i++)
            {
                if (index + i > pageLastNumber)
                {
                    numbers[i].gameObject.SetActive(false);
                }
                else
                {
                    numbers[i].gameObject.SetActive(true);
                    numbers[i].GetComponentInChildren<TMP_Text>().text = (index + i).ToString();
                }
            }

            // ContentSizeFitter 제대로 작동 안될경우 사용
            LayoutRebuilder.ForceRebuildLayoutImmediate(PageNumber.transform.GetComponent<RectTransform>());
            Page.gameObject.SetActive(false);
            Page.gameObject.SetActive(true);
        }

        /// <summary>
        /// 훈련 정보들 불러오기
        /// </summary>
        protected virtual void GetData() { SetPage(); }

        /// <summary>
        /// 차트에 데이터 만들기
        /// </summary>
        protected GameObject CreateContent()
        {
            GameObject content = null;
            if(contents.Count > content_Index)
            {
                content = contents[content_Index];
                content.SetActive(true);
            }
            else
            {
                content = Instantiate(Chart_content, ScrollRect.content, false);
                contents.Add(content);
                
            }
            content_Index++;
            return content;
        }

        protected GameObject CreateContent(SearchUserInfo info)
        {
            GameObject content = null;
            if (contents.Count > content_Index)
            {
                content = contents[content_Index];
                content.SetActive(true);
            }
            else
            {
                content = Instantiate(Chart_content, ScrollRect.content, false);
                contents.Add(content);

            }
            content_Index++;
            return content;
        }

        /// <summary>
        /// 데이터 반환
        /// </summary>
        protected void ReturnContent()
        {
            content_Index = 0;
            for (int i = 0; i < contents.Count; i++)
            {
                contents[i].SetActive(false);
            }
        }

        /// <summary>
        /// 차트보기
        /// </summary>
        protected void ShowChart()
        {
            for (int i = 0; i < layouts.Count; ++i)
            {
                if(i == 2)
                {
                    layouts[i].SetActive(false);
                }
                else
                {
                    layouts[i].SetActive(true);
                }
            }
            SetTitle("#303134");
        }

        /// <summary>
        /// 차트 숨기기
        /// </summary>
        protected void HideChart()
        {
            for (int i = 0; i < layouts.Count; ++i)
            {
                if (i == 1)
                {
                    layouts[i].SetActive(false);
                }
                else
                {
                    layouts[i].SetActive(true);
                }
            }
            SetTitle("#AEB4B8");
        }

        protected void SetTitle(string colorHexa)
        {
            Color newColor;
            if (ColorUtility.TryParseHtmlString(colorHexa, out newColor))
            {
                foreach (TMP_Text text in layouts[0].GetComponentsInChildren<TMP_Text>())
                {
                    text.color = newColor;
                }
            }
        }

        protected void ResetPage()
        {
            pageIndex = 0;
            currentPage = 1;
            SetPage();
            SelectPageNum(0);
        }

        /// <summary>
        /// 데이터들의 번호 계산
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected int CalculatorIndex(int index)
        {
            // 오름차순 내림차순 계산
            if (isAsc)
            {
                index = (currentPage - 1) * maxCount + (index + 1);
            }
            else
            {
                if (currentPage == 1)
                {
                    index = isMine ? MineCount - index : AllCount - index;
                }
                else
                {
                    index = isMine ? MineCount - ((currentPage - 1) * maxCount) - index : AllCount - ((currentPage - 1) * maxCount) - index;
                }
            }
            return index;
        }
    }
}