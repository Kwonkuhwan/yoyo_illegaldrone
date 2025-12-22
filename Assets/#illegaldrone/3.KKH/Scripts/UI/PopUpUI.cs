using UnityEngine;
using UnityEngine.UI;

namespace KKH.UI
{
    public class PopUpUI : MonoBehaviour, IPopUpUI
    {
        [Header("버튼 UI")]
        [SerializeField] protected Button done_Button;
        [SerializeField] protected Button cancel_Button;

        [Header("팝업 오브젝트")]
        [SerializeField] protected GameObject popUpParent;

        public virtual void Awake()
        {
            if (done_Button != null)
            {
                done_Button.onClick.AddListener(() => DoneButtonClick());
            }

            if (cancel_Button != null)
            {
                cancel_Button.onClick.AddListener(() => CancelButtonClick());
            }
        }

        public virtual void ShowPopUp()
        {
            if (popUpParent != null)
                popUpParent.SetActive(true);
            gameObject.SetActive(true);
        }

        public virtual void HidePopUp()
        {
            if (popUpParent != null)
            {
                popUpParent.SetActive(false);
            }
            gameObject.SetActive(false);
        }

        public virtual void DoneButtonClick()
        {
            HidePopUp();
        }

        public virtual void CancelButtonClick()
        {
            HidePopUp();
        }
    }
}