using Illegaldrone;
using KKH.MySQL;
using KKH.UI;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace KKH.HOME.UI
{
    public class PassWordModifyPopUp : PopUpUI
    {
        [SerializeField] private string userID;
        [SerializeField] private TMP_InputField passwordInput;
        [SerializeField] private GameObject alrt_PopUp;

        public override void Awake()
        {
            base.Awake();

            passwordInput.onValueChanged.AddListener(delegate { PasswordInputValueChanged(); });
        }

        private void OnEnable()
        {
            alrt_PopUp.SetActive(false);
        }

        public override void ShowPopUp()
        {
            base.ShowPopUp();
            done_Button.GetComponent<ButtonManager>().interactable = false;
        }

        public void ShowPopUp(string userID)
        {
            this.userID = userID;

            ShowPopUp();
        }

        public override void HidePopUp()
        {
            this.userID = string.Empty;
            this.passwordInput.text = string.Empty;

            base.HidePopUp();
        }

        public override void DoneButtonClick()
        {
            if (userID != string.Empty && passwordInput != null && passwordInput.text.Length == passwordInput.characterLimit)
            {
                if(MySQLManager.UserPWModify(userID, passwordInput.text))
                {
                    UTILS.Log(userID + " 비밀번호 변경 : " + passwordInput.text);
                }
            }

            base.DoneButtonClick();
        }

        public override void CancelButtonClick()
        {
            base.CancelButtonClick();
        }

        private void PasswordInputValueChanged()
        {
            if(passwordInput.text.Length < passwordInput.characterLimit)
            {
                done_Button.GetComponent<ButtonManager>().interactable = false;
                alrt_PopUp.SetActive(true);
            }
            else
            {
                done_Button.GetComponent<ButtonManager>().interactable = true;
                alrt_PopUp.SetActive(false);
            }
        }


        /**********************************************************************************************************************************************************/
        #region KKH 추가
        public void PasswordChangePlayerKick()
        {
            if (PhotonManager_.Inst == null) return;

            Player player = PhotonManager_.Inst.GetSelectPlayer(userID);

            if(player == null) return;

            GameManager.instance.reason = DISCONNECTEDREASON.PASSWORDCHANGE;
            PhotonManager_.Inst.SetPlayerCustomProperty("isKicked", true, player);
        }
        #endregion
    }
}