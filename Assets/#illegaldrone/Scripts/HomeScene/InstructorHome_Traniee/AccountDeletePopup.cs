using KKH;
using KKH.MySQL;
using KKH.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SMW
{
    public class AccountDeletePopup : MonoBehaviour
    {
        [SerializeField] Button Button_AccountDelete;
        [SerializeField] TMP_Text Text_Notification;
        [SerializeField] TMP_Text Text_Info;
        PopUpUI PopUpUI;
        string userID;

        public static Action EventDeleteAccount;

        private void Awake()
        {
            Button_AccountDelete.onClick.AddListener(DeleteAccount);
        }

        public void ChangeText(int role)
        {
            switch ((UserGroup)role)
            {
                case UserGroup.Admin:
                    {

                    }
                    break;
                case UserGroup.Instructor:
                    {
                        Text_Notification.text = "교관의 계정을 삭제하겠습니까?";
                        Text_Info.text = "삭제 선택 시, 교관의 훈련 이력(90일 이내)은\r\n확인 가능하나, 로그인은 제한됩니다.";
                    }
                    break;
                case UserGroup.Trainee:
                    {
                        Text_Notification.text = "훈련생의 계정을 삭제하겠습니까?";
                        Text_Info.text = "삭제 선택 시, 훈련생의 훈련 이력(90일 이내)은\r\n확인 가능하나, 로그인은 제한됩니다.";
                    }
                    break;
            }

            //if (isTrainee)
            //{
            //    Text_Notification.text = "훈련생의 계정을 삭제하겠습니까?";
            //    Text_Info.text = "삭제 선택 시, 훈련생의 훈련 이력(90일 이내)은\r\n확인 가능하나, 로그인은 제한됩니다.";
            //}
            //else
            //{
            //    Text_Notification.text = "교관의 계정을 삭제하겠습니까?";
            //    Text_Info.text = "삭제 선택 시, 교관의 훈련 이력(90일 이내)은\r\n확인 가능하나, 로그인은 제한됩니다.";
            //}
        }

        /// <summary>
        /// 삭제할 유저
        /// </summary>
        /// <param name="name"></param>
        public void SetUser(string id)
        {
            userID = id;
        }

        void DeleteAccount()
        {
            if (userID == "") return;

            if(MySQLManager.UserDelete(userID))
            {
                UTILS.Log(userID + " : 계정삭제 중");
                EventDeleteAccount?.Invoke();
            }
        }
    }
}