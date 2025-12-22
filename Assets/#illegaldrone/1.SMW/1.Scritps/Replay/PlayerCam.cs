using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SMW
{
    public class PlayerCam : MonoBehaviour
    {
        [SerializeField] ReplayCanvas replayCanvas;
        int index = -1;

        public void SetIndex(int index)
        {
            this.index = index;
        }

        /// <summary>
        /// 카메라가 활성화 되었을 때
        /// </summary>
        private void OnEnable()
        {
            replayCanvas.CameraOn(index);
        }

        /// <summary>
        /// 카메라가 비활성화 되었을 때
        /// </summary>
        private void OnDisable()
        {
            replayCanvas.CameraOff(index);
        }
    }
}