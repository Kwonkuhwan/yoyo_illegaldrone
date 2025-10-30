namespace KKH.UI
{
    public class Home_Trainee_LogOutPopUp : PopUpUI, IPopUpUI
    {
        public override void Awake()
        {
            base.Awake();
        }

        public override void CancelButtonClick()
        {
            base.CancelButtonClick();
        }

        public override void DoneButtonClick()
        {
            base.DoneButtonClick();

            PhotonManager_.Inst.LogOut();
        }

        public override void HidePopUp()
        {
            base.HidePopUp();
        }

        public override void ShowPopUp()
        {
            base.ShowPopUp();
        }
    }
}
