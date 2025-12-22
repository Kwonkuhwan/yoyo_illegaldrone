using UnityEngine;

namespace RJH
{
    public class LineController : MonoBehaviour
    {
        private static LineController instance;
        public static LineController Instance { get { return instance; } }
        
        private LineRenderer lr;
        public Transform mainTarget;
        public Transform target;
        public float distance;

        private void Awake()
        {
            lr = GetComponent<LineRenderer>();
            instance = this;
            gameObject.SetActive(false);
        }

        private void Update()
        {
            SetUpLine();
            GetDistance();
        }
        /// <summary>
        ///  라인 생성
        /// </summary>
        private void SetUpLine()
        {
            lr.SetPosition(0, mainTarget.position);
            lr.SetPosition(1, target.position);
        }
        /// <summary>
        /// 두 포인트 사이의 거리를 계산
        /// </summary>
        private void GetDistance()
        {
            distance = Vector3.Distance(mainTarget.position, target.position);
        }

        /// <summary>
        /// 목표(랜드마크) 트랜스폼 세팅
        /// </summary>
        /// <param name="mTarget">세팅할 목표 트랜스폼</param>
        public void SetMainTarget(Transform mTarget)
        {
            this.mainTarget = mTarget;
        }
        
        /// <summary>
        /// 타겟(드론) 트랜스폼 세팅
        /// </summary>
        /// <param name="target">세팅할 타겟 트랜스폼</param>
        public void SetTarget(Transform target)
        {
            this.target = target;
        }
         
    }
}



