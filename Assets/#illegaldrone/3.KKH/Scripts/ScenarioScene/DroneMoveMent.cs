using Illegaldrone;
using KKH;
using System.Collections.Generic;
using UnityEngine;

public class DroneMoveMent : MonoBehaviour
{
    public bool isRotateDrone = true;

    public float limitY = 3.0f;
    public float speed = 2.0f;
    public bool up = false;
    public List<Transform> rotateObjs;
    public float rotationSpeed = 0.0f;

    public LayerMask[] destroyLayerMasks;

    private void Awake()
    {
        if (!GameManager.instance.isInstructor) this.enabled = false;
    }

    void Update()
    {
        if (!GameManager.instance.isInstructor) return;

        if (isRotateDrone)
        {
            float yPos = transform.localPosition.y;

            if (yPos >= limitY)
            {
                up = false;
            }
            else if (yPos <= -limitY)
            {
                up = true;
            }

            if (up)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, yPos + (speed * Time.deltaTime), transform.localPosition.z);
            }
            else
            {
                transform.localPosition = new Vector3(transform.localPosition.x, yPos - (speed * Time.deltaTime), transform.localPosition.z);
            }

            if (rotateObjs != null && rotateObjs.Count > 0)
            {
                foreach (Transform t in rotateObjs)
                {
                    t.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                }
            }
        }
    }
    //protected virtual void OnCollisionEnter(Collision collision)
    //{

    //    // 충돌한 오브젝트의 레이어를 가져옴
    //    int collidedLayer = collision.gameObject.layer;

    //    // 각 레이어 마스크를 확인
    //    foreach (LayerMask layerMask in destroyLayerMasks)
    //    {
    //        // 레이어 마스크가 충돌한 레이어와 일치하는지 확인
    //        if ((layerMask & (1 << collidedLayer)) != 0)
    //        {
    //            UTILS.Log(collision.gameObject.name);
    //            // 부모 오브젝트를 파괴
    //            Destroy(transform.parent.gameObject);
    //            break; // 일치하는 레이어를 찾으면 루프 종료
    //        }
    //    }
    //}
}
