using UnityEngine;
using UnityEngine.UI;

namespace RJH
{
    public class InstructorTrainEndPopup : MonoBehaviour
    {
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button confirmButton;

        private void Awake()
        {
            cancelButton.onClick.AddListener(OnCancel);
            confirmButton.onClick.AddListener(OnConfirm);
        }

        private void OnCancel()
        {
            this.gameObject.SetActive(false);

        }

        private void OnConfirm()
        {
            //ToDo 훈련 종료 
            //�Ʒû� HMD �Ʒ� ���� ǥ��
        }
    }
}


