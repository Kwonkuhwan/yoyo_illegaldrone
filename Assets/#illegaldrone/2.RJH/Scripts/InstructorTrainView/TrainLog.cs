using KKH;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace RJH
{
    public class TrainLog : MonoBehaviour
    {
        [Tooltip("로그 배경 이미지")]
        [SerializeField] private Image backGroundImage;
        [Tooltip("훈련생 번호")]
        [SerializeField] private TextMeshProUGUI traineeNumberText;

        [SerializeField] private Image line;
        [Tooltip("메세지")]
        [SerializeField] private TextMeshProUGUI messageText;
        [Tooltip("메세지 아이콘")]
        [SerializeField] private Image messageIconImage;

        [SerializeField] private Sprite[] backGroundSprites; // 0 : 일반 로그 배경, 경고 로그 배경
        [SerializeField] private Sprite[] iconSprite; // 0: 공격받음, 1: 방어지대 도달, 2: 무력화됨, 3: 격추함
        [SerializeField] private Color[] nameColors;
        private readonly string[] droneNames = { "자폭", "소총", "정찰", "전자전" };
        private readonly string[] defanseAreas = { "경계 지대", "주 방어지대", "핵심 방어지대" };
        private Coroutine currentCoroutine;

        public void LogMessage(LOGMESSAGE message, int firstIndex = -1, int secondIndex = -1)
        {
            try
            {
                switch (message)
                {
                    case LOGMESSAGE.ATTACK:
                        AttackLog(firstIndex);
                        break;
                    case LOGMESSAGE.ARRIVE:
                        ArriveLog(firstIndex, secondIndex);
                        break;
                    case LOGMESSAGE.INACTIVE:
                        InactiveLog(firstIndex, secondIndex);
                        break;
                    case LOGMESSAGE.SHOOTING:
                        ShootingLog(firstIndex, secondIndex);
                        break;
                }

                if (currentCoroutine != null)
                {
                    StopCoroutine(currentCoroutine);
                }

                currentCoroutine = StartCoroutine(CloseLog());
            }
            catch
            {
                UTILS.Log("생성할 수 없는 로그입니다.");
            }
        }

        private void AttackLog(int damege)
        {
            traineeNumberText.gameObject.SetActive(false);
            line.gameObject.SetActive(false);
            backGroundImage.sprite = backGroundSprites[1];
            messageText.text = $"랜드마크가 공격을 받고 있습니다. ({damege})";
            messageIconImage.sprite = iconSprite[0];
            gameObject.SetActive(true);
        }

        private void ArriveLog(int dronType, int defenseArea)
        {
            backGroundImage.sprite = backGroundSprites[1];
            messageText.text = $"{droneNames[dronType]}형 불법 드론이 {defanseAreas[defenseArea]}에 도달했습니다.";
            messageIconImage.sprite = iconSprite[1];
            gameObject.SetActive(true);
        }

        private void InactiveLog(int traineeNumber, int droneType)
        {
            backGroundImage.sprite = backGroundSprites[0];
            traineeNumberText.gameObject.SetActive(true);
            line.gameObject.SetActive(true);
            traineeNumberText.text = $"{traineeNumber + 1}번 훈련생";
            traineeNumberText.color = nameColors[traineeNumber];
            messageText.text = $"{droneNames[droneType]}형 불법 드론을 무력화했습니다. ({droneNames[droneType]} -1)";
            messageIconImage.sprite = iconSprite[2];
            gameObject.SetActive(true);
        }

        private void ShootingLog(int traineeNumber,int droneType)
        {
            backGroundImage.sprite = backGroundSprites[0];
            traineeNumberText.gameObject.SetActive(true);
            line.gameObject.SetActive(true);
            traineeNumberText.text = $"{traineeNumber+1}번 훈련생";
            traineeNumberText.color = nameColors[traineeNumber];
            messageText.text = $"{droneNames[droneType]}형 불법 드론을 격추했습니다. ({droneNames[droneType]} -1)";
            messageIconImage.sprite = iconSprite[3];
            gameObject.SetActive(true);
        }

        private IEnumerator CloseLog()
        {

            yield return new WaitForSeconds(5);
            currentCoroutine = null;
            traineeNumberText.gameObject.SetActive(false);
            line.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    public enum LOGMESSAGE
    {
        ATTACK,
        ARRIVE,
        INACTIVE,
        SHOOTING
    }

}

