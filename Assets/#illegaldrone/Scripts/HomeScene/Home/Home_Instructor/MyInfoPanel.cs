using Illegaldrone;
using KKH.MySQL;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KKH.HOME.UI
{
    public class MyInfoPanel : MonoBehaviour
    {
        [SerializeField] InstructorInfo instructorInfo;

        [SerializeField] private Button pw_Modify_Btn;
        [SerializeField] private GameObject pw_Modify_Panel;

        [SerializeField] private Sprite[] profile_Images;
        [SerializeField] private Image profile_Image;

        [SerializeField] private Sprite[] profile_Icons;
        [SerializeField] private Image profile_Icon;
        [SerializeField] private TMP_Text profile_Icon_Text;

        [SerializeField] private TMP_Text profile_Name_Text;
        [SerializeField] private TMP_Text profile_ID_Text;
        [SerializeField] private TMP_Text profile_TraineeCount_Text;
        [SerializeField] private TMP_Text profile_ResultCount_Text;
        [SerializeField] private TMP_Text profile_Recentlylogin_Text;


        private void Awake()
        {
            if (pw_Modify_Btn != null)
            {
                pw_Modify_Btn.onClick.AddListener(() => ShowPWModifyPopUp());
            }
        }

        private void ShowPWModifyPopUp()
        {
            pw_Modify_Panel.GetComponent<PassWordModifyPopUp>().ShowPopUp(instructorInfo.id);
        }


        private double CalculatePercentage(double part, double total)
        {
            if (total == 0)
            {
                return 0;
            }

            return (part / total) * 100;
        }

        private void OnEnable()
        {
            instructorInfo = MySQLManager.GetInstructorInfo(GameManager.instance.userInfo.id);

            if (instructorInfo == null) gameObject.SetActive(false);

            if (profile_Image != null)
            {
                profile_Image.sprite = profile_Images[(int)instructorInfo.gender];
            }

            if (profile_Icon != null)
            {
                profile_Icon.sprite = profile_Icons[(int)instructorInfo.userGroup];
            }

            if (profile_Icon_Text != null)
            {
                profile_Icon_Text.text = instructorInfo.userGroup == UserGroup.Admin ? "최고 관리자" : "관리자";
            }

            if (profile_Name_Text != null)
            {
                profile_Name_Text.text = $"{instructorInfo.userName}님";
            }

            if (profile_ID_Text != null)
            {
                profile_ID_Text.text = instructorInfo.id;
            }

            if (profile_TraineeCount_Text != null)
            {
                profile_TraineeCount_Text.text = instructorInfo.totalCount.ToString();
            }

            if (profile_ResultCount_Text != null)
            {
                profile_ResultCount_Text.text = $"{instructorInfo.totalCount} ({CalculatePercentage(instructorInfo.totalCount, instructorInfo.missionResultTotalCount):F2}%)";
            }

            if (profile_Recentlylogin_Text != null)
            {
                profile_Recentlylogin_Text.text = instructorInfo.endAccessDate.ToString("yyyy.MM.dd HH:mm");
            }
        }

        private void OnDisable()
        {
            instructorInfo = null;

            if (profile_Icon_Text != null)
            {
                profile_Icon_Text.text = string.Empty;
            }

            if (profile_Name_Text != null)
            {
                profile_Name_Text.text = string.Empty;
            }

            if (profile_ID_Text != null)
            {
                profile_ID_Text.text = string.Empty;
            }

            if (profile_TraineeCount_Text != null)
            {
                profile_TraineeCount_Text.text = string.Empty;
            }

            if (profile_ResultCount_Text != null)
            {
                profile_ResultCount_Text.text = string.Empty;
            }

            if (profile_Recentlylogin_Text != null)
            {
                profile_Recentlylogin_Text.text = string.Empty;
            }
        }
    }
}