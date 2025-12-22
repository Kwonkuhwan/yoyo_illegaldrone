using UnityEngine;
using Photon.Pun;

namespace KKH
{
    public enum EndPopupType : int
    {
        startCount = 0,
        pause,
        end
    }

    public class UIManager : MonoBehaviourPun
    {
        public PhotonView pv;
        public static UIManager Inst { get; set; }

        public GameObject background;
        public GameObject startCountPopup;
        public GameObject pausePopup;
        public GameObject endPopup;

        private void Awake()
        {
            if (Inst == null)
            {
                Inst = this;
            }

            pv = GetComponent<PhotonView>();
        }

        private void Start()
        {
            AllPopupHide();
        }

        public void AllPopupHide()
        {
            background.SetActive(false);
            startCountPopup.SetActive(false);
            pausePopup.SetActive(false);
            endPopup.SetActive(false);
        }

        public void ShowPopup(EndPopupType type)
        {
            AllPopupHide();

            background.SetActive(true);
            if (type == EndPopupType.startCount)
            {
                startCountPopup.SetActive(true);
            }
            else if (type == EndPopupType.pause)
            {
                pausePopup.SetActive(true);
            }
            else if (type == EndPopupType.end)
            {
                endPopup.SetActive(true);
            }
        }

        public void ShowStartCount()
        {
            pv.RPC("PunShowStartCount", RpcTarget.Others, null);
        }

        public void ShowPause()
        {
            pv.RPC("PunShowPause", RpcTarget.Others, null);
        }

        public void ShowEnd()
        {
            pv.RPC("PunShowEnd", RpcTarget.Others, null);
        }

        [PunRPC]
        public void PunShowStartCount()
        {
            UTILS.Log("ShowStartCount");
            ShowPopup(EndPopupType.startCount);
        }

        [PunRPC]
        public void PunShowPause()
        {
            UTILS.Log("PunShowPause");
            ShowPopup(EndPopupType.pause);
        }

        [PunRPC]
        public void PunShowEnd()
        {
            UTILS.Log("PunShowEnd");
            ShowPopup(EndPopupType.end);
        }
    }
}