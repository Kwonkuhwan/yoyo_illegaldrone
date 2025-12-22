using KKH;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace SMW
{
    public enum Gender
    {
        Male = 0,               // 남성
        Female = 1,             // 여성
    }

    public enum Display
    {
        Default = 0,
        OFF = 1,
        ON = 2
    }

    public class TranieeBox : MonoBehaviour
    {
        [SerializeField] Image Image_HMDStatus;
        [SerializeField] Image Image_Gender;
        [SerializeField] Button Button_Kick;
        [SerializeField] TextMeshProUGUI Text_ID;
        [SerializeField] TextMeshProUGUI Text_Name;

        [Header("Sprite")]
        [SerializeField] Sprite sprite_Man;
        [SerializeField] Sprite sprite_Woman;
        [SerializeField] Sprite sprite_Noready;
        [SerializeField] Sprite sprite_ready;

        [Header("List")]
        [SerializeField] GameObject[] DisplayObjets;
        [SerializeField] TextMeshProUGUI[] Text_index;

        public string _index;

        Player _player;

        private void Awake()
        {
            SetIndex(_index);
            Button_Kick.onClick.AddListener(Kick);
            ShowDisplay(Display.Default);
        }

        private void Update()
        {
            if (_player != null)
            {
                Hashtable ht = _player.CustomProperties;

                bool isPlayerReady = false;
                if (ht != null && ht.ContainsKey("PlayerReady"))
                {
                    isPlayerReady = (bool)ht["PlayerReady"];
                }
                ChanageHMDStatus(isPlayerReady);
            }
        }

        void Kick()
        {
            PhotonManager_.Inst.SetPlayerCustomProperty("isKicked", true, _player);
            //Hashtable ht = _player.CustomProperties;
            //ht.Add("isKicked", true);
            //_player.SetCustomProperties(ht);
        }

        // index 번호 바꾸기
        void SetIndex(string index)
        {
            for (int i = 0; i < Text_index.Length; i++)
            {
                Text_index[i].text = index;
            }
        }

        public void SetPlayer(Player player)
        {
            _player = player;
        }

        /// <summary>
        /// HMD의 상태 조회
        /// </summary>
        public void ChanageHMDStatus(bool isReady)
        {
            if (isReady)
            {
                Image_HMDStatus.sprite = sprite_ready;
            }
            else
            {
                Image_HMDStatus.sprite = sprite_Noready;
            }
        }

        /// <summary>
        /// 성별 대입
        /// </summary>
        /// <param name="_type"></param>
        public void ChangeGender(Gender _type)
        {
            if (_type == Gender.Male)
            {
                Image_Gender.sprite = sprite_Man;
            }
            else
            {
                Image_Gender.sprite = sprite_Woman;
            }
        }

        /// <summary>
        /// ID 대입
        /// </summary>
        /// <param name="id"></param>
        public void SetID(string id)
        {
            Text_ID.text = id;
        }

        /// <summary>
        /// 닉네임 대입
        /// </summary>
        /// <param name="name"></param>
        public void SetName(string name)
        {
            Text_Name.text = name;
        }

        /// <summary>
        /// 대기, 준비, 비활성화 3가지의 화면 설정
        /// </summary>
        /// <param name="display"></param>
        public void ShowDisplay(Display display)
        {
            for (int i = 0; i < DisplayObjets.Length; i++)
            {
                if (i == (int)display)
                {
                    DisplayObjets[i].SetActive(true);
                }
                else
                    DisplayObjets[i].SetActive(false);
            }
        }

        /// <summary>
        /// 화면 설정이 대기인지 확인
        /// </summary>
        /// <returns></returns>
        public bool GetDisplayOn()
        {
            return DisplayObjets[(int)Display.ON].activeSelf;
        }
    }
}