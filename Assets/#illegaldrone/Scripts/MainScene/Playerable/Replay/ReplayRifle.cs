using SMW;
using System.Collections;
using System.Collections.Generic;
using UltimateReplay;
using UltimateReplay.StatePreparation;
using UltimateReplay.Storage;
using UnityEngine;

namespace SMW
{
    public class ReplayRifle : ReplayBehaviour
    {
        // 리플레이 이펙트 이벤트 트리거
        private bool applyEffectTriggered = false;

        Rifle rifle;

        protected override void Awake()
        {
            base.Awake();
            rifle = GetComponent<Rifle>();
        }

        /// <summary>
        /// 녹화 시 이벤트 트리거 저장
        /// </summary>
        public void TriggerApplyEffect()
        {
            applyEffectTriggered = true;
        }

        /// <summary>
        /// 녹화시점에 이벤트를 기록
        /// </summary>
        protected override void OnReplayCapture()
        {
            if (applyEffectTriggered)
            {
                RecordEvent(0);
                applyEffectTriggered = false;
            }
        }

        /// <summary>
        /// 리플레이 재생 중 이벤트를 재생
        /// </summary>
        /// <param name="eventID"> 이벤트 ID </param>
        /// <param name="eventData"></param>
        protected override void OnReplayEvent(ushort eventID, ReplayState eventData)
        {
            if (eventID == 0)
            {
                ApplyEffect();
            }
        }

        private void ApplyEffect()
        {
            Debug.Log("리플레이 중 이펙트 적용");
            rifle.OnReplayAttackEvent();
        }
    }
}