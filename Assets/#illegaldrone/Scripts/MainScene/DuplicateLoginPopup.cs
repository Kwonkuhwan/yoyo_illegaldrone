using UnityEngine;
using UnityEngine.UI;
using KKH;

namespace RJH.UI
{
    public class DuplicateLoginPopup : MonoBehaviour // ToDO 나중에 공통으로 사용하는 팝업으로 대체
    {
        [SerializeField] private Button confirmButton;

        private void Awake()
        {
            confirmButton.onClick.AddListener(OnConfirm);
        }

        private void OnConfirm()
        {
            //PhotonUIManager.Instance.ShowUI("LoginPage");
            //PhotonManager.instance.LogOut();
            PhotonManager_.Inst.SceneLoad("00.Login");
            gameObject.SetActive(false);
        }
    }

}

