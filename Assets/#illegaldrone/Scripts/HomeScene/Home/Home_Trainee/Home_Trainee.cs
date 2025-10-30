using Illegaldrone;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using KKH.UI;
using RJH.UI;

namespace KKH.HOME.Trainee
{
    public class Home_Trainee : MonoBehaviour
    {
        [Header("유저 정보")]
        [SerializeField] private TMP_Text name_Text;

        [Space(10)]

        [Header("로그아웃")]
        [SerializeField] private Button logOut_Button;

        [Header("팝업")]
        [SerializeField] private GameObject logOut_PopUp;

        [Header("테스트 시작 버튼")]
        [SerializeField] private Button test_Button;
        [SerializeField] private Home_Trainee_VR home_Trainee_VR;

        private void Awake()
        {
            if(logOut_Button != null)
            {
                logOut_Button.onClick.AddListener(() => LogOutBtnClick());
            }

            if (GameManager.instance != null)
            {
                if (name_Text == null)
                {
                    name_Text = GetComponentInChildren<TMP_Text>();
                }
                name_Text.text = $"<b><#0008FF>{GameManager.instance.userInfo.userName}</color></b> 훈련생님";
            }

//#if UNITY_EDITOR
            test_Button.onClick.AddListener(() => home_Trainee_VR.StartBtnClick());
//#else
            //test_Button.gameObject.SetActive(false);
//#endif
        }

        private void LogOutBtnClick()
        {
            ShowPopUp();
        }

        private void ShowPopUp()
        {
            HidePopUp();

            if (logOut_PopUp == null) return;
            PopUpUI logOut_popUpUI = logOut_PopUp.GetComponent<PopUpUI>() as Home_Trainee_LogOutPopUp;
            if (logOut_popUpUI != null)
            {
                logOut_popUpUI.ShowPopUp();
            }
        }

        private void HidePopUp()
        {
            if (logOut_PopUp == null) return;
            PopUpUI logOut_popUpUI = logOut_PopUp.GetComponent<PopUpUI>() as Home_Trainee_LogOutPopUp;
            if (logOut_popUpUI != null)
            {
                logOut_popUpUI.HidePopUp();
            }
        }
    }
}
