using Illegaldrone;
using UnityEngine;

namespace RJH
{
    public class TestTrainLog : MonoBehaviour
    {
        public TrainLogSpawner spawner;

        public LOGMESSAGE message;
        public int first;
        public int second;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameManager.instance.InstructorViewEvent(message, first, second);
            }
        }

    }
}