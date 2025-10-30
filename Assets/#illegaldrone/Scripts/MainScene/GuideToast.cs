using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RJH
{
    public class GuideToast : MonoBehaviour
    {
        [SerializeField] private GameObject toast;
        [SerializeField] private TextMeshProUGUI massageText;
        [SerializeField] private Image warningIcon;
        private static GuideToast instance;
        public static GuideToast Instance { get { return instance; } }

        private void Awake()
        {
            instance = this;
        }

        public void PopupMSG(MSG msg)
        {
            StopAllCoroutines();
            toast.SetActive(true);
            switch (msg)
            {
                case MSG.SELECTSTARTPOSITION:
                    massageText.text = "불법 드론의 출발 지점을 지도에서 선택해 주세요.";
                    break;
                case MSG.NOTPOSSIBLE:
                    warningIcon.gameObject.SetActive(true);
                    massageText.text = "방어 3지대에는 드론의 위치를 설정할 수 없습니다.";
                    break;
                case MSG.NEEDDEFENSEOBJECT:
                    warningIcon.gameObject.SetActive(true);
                    massageText.text = "방어 건물이 지정되지 않았습니다.";
                    StartCoroutine(ToastPopupClose());
                    break;
                case MSG.TRAINEEOUTOFRANGE:
                    warningIcon.gameObject.SetActive(true);
                    massageText.text = "훈련생이 배치 가능한 구역 밖에 있습니다.";
                    StartCoroutine(ToastPopupClose());
                    break;
                case MSG.DRONEINOFRANGE:
                    warningIcon.gameObject.SetActive(true);
                    massageText.text = "드론이 배치 불가능한 구역 안에 있습니다.";
                    StartCoroutine(ToastPopupClose());
                    break;
                case MSG.NEEDTRAINEEPOS:
                    warningIcon.gameObject.SetActive(true);
                    massageText.text = "위치 설정이 안 된 훈련생이 있습니다.";
                    StartCoroutine(ToastPopupClose());
                    break;
            }
        }

        public void Close()
        {
            warningIcon.gameObject.SetActive(false);
            toast.SetActive(false);
        }

        private IEnumerator ToastPopupClose()
        {
            yield return new WaitForSeconds(2);
            Close();
        }
    }

    public enum MSG
    {
        SELECTSTARTPOSITION,    // 드론 출발지점 배치
        NOTPOSSIBLE,            // 드론은 방어 3지대에 배치 불가
        NEEDDEFENSEOBJECT,      // 방어목표 설정 안됨
        TRAINEEOUTOFRANGE,      // 훈련생이 방어 구역 밖에 있음
        DRONEINOFRANGE,         // 드론이 방어 구역 안에 있음
        NEEDTRAINEEPOS          // 훈련생 좌표 설정 필요
    }
}
