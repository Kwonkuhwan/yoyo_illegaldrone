using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SMW
{
    public enum PLACE
    {
        Airplane = 0,
        Gwanghwamun,
        Nuclearpower
    }
    public class ImformationBox : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI Text_Result;
        [SerializeField] TextMeshProUGUI Text_Time;
        [SerializeField] TextMeshProUGUI Text_Number;
        [SerializeField] TextMeshProUGUI Text_Place;
        [SerializeField] Button Button_Replay;
        [SerializeField] Image Image_Result;

        [Header("Sprite")]
        [SerializeField] Sprite sprite_success;
        [SerializeField] Sprite sprite_fail;
        [SerializeField] Sprite sprite_play;
        [SerializeField] Sprite sprite_playDisabled;

        private void Start()
        {
            Button_Replay.onClick.AddListener(OnClickReplay);
        }

        // 훈련 성공여부
        public void SetTrainResult(int isResult)
        {
            // 훈련성공
            if (isResult == 1)
            {
                Image_Result.sprite = sprite_success;
                Text_Result.text = "훈련 성공";
            }
            else
            {
                Image_Result.sprite = sprite_fail;
                Text_Result.text = "훈련 실패";
            }
        }

        // 시간 설정
        public void SetTime(DateTime time)
        {
            if (time.ToString("tt") == "오후")
            {
                Text_Time.text = "PM " + time.ToString("hh mm");
            }
            else
            {
                Text_Time.text = "AM " + time.ToString("hh mm");
            }
        }

        // 참여인원
        public void SetNumber(int number)
        {
            Text_Number.text = "참여인원 " + number + "명";
        }

        // 장소
        public void SetPlace(int place)
        {
            switch ((PLACE)place)
            {
                case PLACE.Airplane:
                    Text_Place.text = "공항";
                    break;
                case PLACE.Gwanghwamun:
                    Text_Place.text = "광화문";
                    break;
                case PLACE.Nuclearpower:
                    Text_Place.text = "원자력";
                    break;
            }
        }

        void OnClickReplay()
        {
            Debug.Log("Start Replay");
        }
    }
}