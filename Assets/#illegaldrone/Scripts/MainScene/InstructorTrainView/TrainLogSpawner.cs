using Illegaldrone;
using UnityEngine;

namespace RJH
{
    public class TrainLogSpawner : MonoBehaviour
    {
        
        [SerializeField] private GameObject[] trainLogObjects;

        private void Awake()
        {
            GameManager.instance.logMessageCallBack.AddListener(SpawnLog);
        }

        /// <summary>
        /// 랜드마크가 공격 받았을 경우 (MESSAGE.ATTACK)
        /// 드론이 구역을 침범했을 경우 (MESSAGE.ARRIVE, 드론 번호, 방어 구역 번호)
        /// 드론을 무력화했을 경우 (MESSAGE.INACTIVE, 드론 번호)
        /// 훈련생이 드론을 격추했을 경우 (MESSAGE.SHOOTING, 훈련생 번호(0번 부터), 드론 번호)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="firstindex"></param>
        /// <param name="secondIndex"></param>
        public void SpawnLog(LOGMESSAGE message, int firstindex = -1, int secondIndex = -1)
        { 
            for(int i = trainLogObjects.Length - 1; i >= 0; i--)
            {
                if(trainLogObjects[i].activeSelf == false)
                {
                    trainLogObjects[i].transform.SetSiblingIndex(0);
                    trainLogObjects[i].GetComponent<TrainLog>().LogMessage(message, firstindex, secondIndex);
                    return;
                }
            }

            GameObject lastElement = trainLogObjects[trainLogObjects.Length - 1];  

            for (int i = trainLogObjects.Length - 1; i > 0; i--)
            {
                trainLogObjects[i] = trainLogObjects[i - 1];
            }

            lastElement.transform.SetSiblingIndex(0);
            trainLogObjects[0] = lastElement;

            trainLogObjects[0].GetComponent<TrainLog>().LogMessage(message, firstindex, secondIndex);
        }

        private void OnDisable()
        {
            GameManager.instance.logMessageCallBack.RemoveListener(SpawnLog);
        }
    }
}


