using KKH.MySQL;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SMW
{
    public class TraningStatus : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI Text_TraningNumber;
        [SerializeField] TextMeshProUGUI Text_TrainessNumber;

        void Start()
        {
            Text_TraningNumber.text = Get_TraningNumber().ToString() + "회";
            Text_TrainessNumber.text = Get_TrainessNumber().ToString() + "명";
        }

        /// <summary>
        /// 누적 훈련 수
        /// </summary>
        /// <returns></returns>
        int Get_TraningNumber()
        {
            int count;

            if(MySQLManager.GetTrainingCumulativeCount("", false, out count))
            {
                return count;
            }
            else
            {
                Debug.LogError("Error Get_TraningNumber");
                return 0;
            }
        }

        /// <summary>
        /// 누적 훈련생 수
        /// </summary>
        /// <returns></returns>
        int Get_TrainessNumber()
        {
            int count = 0;

            if (MySQLManager.GetUsersCumulativeCount("", UserGroup.Trainee, out count))
            {
                return count;
            }
            else
            {
                Debug.LogError("Error Get_TrainessNumber");
                return 0;
            }
        }
    }
}