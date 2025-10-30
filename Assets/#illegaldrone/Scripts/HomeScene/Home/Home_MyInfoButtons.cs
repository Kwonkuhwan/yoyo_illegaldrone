using Illegaldrone;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KKH.HOME.UI
{
    public class Home_MyInfoButtons : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject panel_MyInfos;

        [Header("Buttons")]
        [SerializeField] private Button btn_MyInfo;
        [SerializeField] private Button btn_LogOut;

        private void Awake()
        {
            if (btn_MyInfo != null)
            {
                btn_MyInfo.onClick.AddListener(() => MyInfoShowOrHide());
                btn_MyInfo.GetComponentInChildren<TMP_Text>().text = $"{GameManager.instance.userInfo.userName} 님";
            }

            if (btn_LogOut != null)
            {
                btn_LogOut.onClick.AddListener(() => LogOutBtnClick());
            }
        }

        private void MyInfoShowOrHide()
        {
            if (panel_MyInfos.activeInHierarchy)
            {
                panel_MyInfos.SetActive(false);
            }
            else
            {
                panel_MyInfos.SetActive(true);
            }
        }

        private void LogOutBtnClick()
        {
            PhotonManager_.Inst.LogOut();
        }
    }
}
