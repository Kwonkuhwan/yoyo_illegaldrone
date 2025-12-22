using KKH;
using KKH.HOME.UI;
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
    public class Profile : MonoBehaviour
    {
        [Header("남자")]
        [SerializeField] Sprite Sprite_Man;

        [Header("여자")]
        [SerializeField] Sprite Sprite_Woman;

        [Header("Left_Profile Set")]
        [SerializeField] Image Image_Sex;
        [SerializeField] TMP_Text Text_ID;
        [SerializeField] TMP_Text Text_Name;
        [SerializeField] Button Button_PWEdit;

        [Header("Right_Profile Set")]
        [SerializeField] TMP_Text Text_TraningCount;
        [SerializeField] TMP_Text Text_TraningSuccessCount;

        [Header("Date")]
        [SerializeField] TMP_Text Text_Date;

        [Header("Password Popup")]
        [SerializeField] PassWordModifyPopUp PassWordModifyPopUp;

        [Header("Account Delete")]
        [SerializeField] Button Button_AccountDelete;
        [SerializeField] AccountDeletePopup AccountDeletePopup;

        [Header("Chart_Log")]
        [SerializeField] Chart_Log Chart_Log;

        [Header("Chart_TranieeInfo")]
        [SerializeField] Chart_TraineeInfo Chart_TraineeInfo;

        UserInfo info;
        public string InstructorName
        {
            get { return info.createAdminName; }
        }

        int traningTotalCount;

        // 비밀번호 변경 가능 여부 변수
        bool isEditPassword = false;

        private void Awake()
        {
            Button_AccountDelete.onClick.AddListener(OnClickDeleteAccount);
            Button_PWEdit.onClick.AddListener(OnClickPasswordEdit);
        }

        private void OnEnable()
        {
            AccountDeletePopup.EventDeleteAccount += EventDeleteAccount;
        }

        private void OnDisable()
        {
            AccountDeletePopup.EventDeleteAccount -= EventDeleteAccount;
        }

        /// <summary>
        /// 패스워드 변경
        /// </summary>
        void OnClickPasswordEdit()
        {
            if (!isEditPassword) return;

            PassWordModifyPopUp.ShowPopUp(info.id);
        }

        /// <summary>
        /// 프로필 세팅
        /// </summary>
        public void SetProfile(string userID)
        {
            // 유저 정보 가져오기
            info = MySQLManager.GetUserInfo(userID);

            // Left Profile Set
            SetImageSex(info.gender);
            SetID(info.id);
            SetName(info.userName);
            SetPasswordEdit(info.isDelete);
            SetCreateDate(info.createIDDate);

            // Right Profile Set
            SetTraningCount(info.missionResult_count);
            SetTraningSuccessCount(info.teamMissionResult_count, info.result_Percentage);

            // Chart Send Data
            Chart_Log.ReceiveData(info);
            Chart_TraineeInfo.ReceiveData(info);
        }

        /// <summary>
        /// 계정삭제 이벤트 발생시
        /// </summary>
        void EventDeleteAccount()
        {
            SetPasswordEdit(true);
        }

        void OnClickDeleteAccount()
        {
            if (info.isDelete) return;

            AccountDeletePopup.GetComponent<PopUpUI>().ShowPopUp();
        }

        /// <summary>
        /// 계정삭제 버튼 활성화/비활성화 & 삭제할 유저이름 전달
        /// </summary>
        /// <param name="isDelete"></param>
        void SetPasswordEdit(bool isDelete)
        {
            if(isDelete)
            {
                isEditPassword = false;
                Button_AccountDelete.GetComponent<ButtonEvent>().Activate();
                Button_PWEdit.transform.GetChild(0).GetComponent<TMP_Text>().color = new Color(115 / 225f, 120 / 255f, 127 / 255f, 255 / 255f);
                Button_PWEdit.GetComponent<ButtonEvent>().Activate();

                AccountDeletePopup.SetUser("");
            }
            else
            {
                isEditPassword = true;
                Button_AccountDelete.GetComponent<ButtonEvent>().Deactivate();
                Button_PWEdit.transform.GetChild(0).GetComponent<TMP_Text>().color = new Color(80 / 255f, 190 / 255f, 255 / 255f, 255 / 255f);
                Button_PWEdit.GetComponent<ButtonEvent>().Deactivate();

                AccountDeletePopup.SetUser(info.id);
            }
        }

        void SetImageSex(KKH.MySQL.Gender gender)
        {
            if (gender == KKH.MySQL.Gender.Man)
            {
                Image_Sex.sprite = Sprite_Man;
            }
            else
            {
                Image_Sex.sprite = Sprite_Woman;
            }
        }

        void SetID(string id)
        {
            Text_ID.text = id;
        }

        void SetName(string name)
        {
            Text_Name.text = name;

        }

        void SetTraningCount(int count)
        {
            Text_TraningCount.text = $"{count}회";
        }

        void SetTraningSuccessCount(int count, double percent)
        {
            Text_TraningSuccessCount.text = $"{count}회 ({percent}%)";
        }

        void SetCreateDate(DateTime time)
        {
            Text_Date.text = $"등록일자 : {time.Year}.{time.Month}.{time.Day}. {time.Hour}:{time.Minute}";
        }
    }
}