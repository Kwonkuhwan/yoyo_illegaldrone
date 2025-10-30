using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SMW
{
    public class LaserPoint : MonoBehaviour
    {
        [SerializeField] LayerMask target_layer;        // 타겟 레이어

        LineRenderer lineRenderer;

        Ray ray;
        public RaycastHit Hit
        {
            get { return hit; }
        }
        RaycastHit hit;

        [Range(1, 100)]
        public readonly int pointer_max_range = 1000;           // 레이저포인터 최대거리

        public bool IsDetect;
        public GameObject Target;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        private void Update()
        {
            ray = new Ray(transform.position, transform.forward);

            if (Physics.Raycast(ray, out hit, pointer_max_range))
            {
                FixedLayserPointRange(hit.distance);
                Detect(hit.transform.gameObject);
            }
            else
            {
                FixedLayserPointRange(pointer_max_range);
                IsDetect = false;
                Target = null;
            }
        }

        /// <summary>
        /// 레이저포인터 On/Off
        /// </summary>
        public void TurnLaserPoint(bool isOn)
        {
            lineRenderer.enabled = isOn;
        }

        /// <summary>
        /// 타겟과의 거리 계산
        /// </summary>
        /// <returns></returns>
        public int CalculatorDistance()
        {
            if(IsDetect)
            {
                return (int)Vector3.Distance(transform.position, Target.transform.position);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 타겟 감지
        /// </summary>
        void Detect(GameObject target)
        {
            IsDetect = target_layer == (1 << target.layer);
            Target = target;
        }

        /// <summary>
        /// 레이저포인터 거리 변경
        /// </summary>
        void FixedLayserPointRange(float _distance)
        {
            if (lineRenderer.enabled == false) return;
            lineRenderer.SetPosition(1, new Vector3(0, 0, _distance));
        }
    }
}