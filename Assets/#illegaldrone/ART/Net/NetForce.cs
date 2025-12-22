using System.Collections;
using UnityEngine;

public class NetForce : MonoBehaviour
{
    // 날라가는 속도
    public float forcePower = 5.0f;

    // 회전
    public float rotationSpeed = 0.5f; // 회전 속도
    public float targetAngle = 90.0f; // 목표 각도

    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if(rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
    }

    private void Update()
    {
        
    }

    void Start()
    {
        rb.AddForce(Vector3.forward * forcePower, ForceMode.Impulse);
        StartCoroutine(RotateOverTime(targetAngle, rotationSpeed));
    }

    private IEnumerator RotateOverTime(float angle, float duration)
    {
        float elapsedTime = 0f;
        Quaternion startingRotation = transform.rotation;
        Quaternion targetRotation = startingRotation * Quaternion.Euler(angle, 0, 0);

        while (elapsedTime < duration)
        {
            transform.rotation = Quaternion.Slerp(startingRotation, targetRotation, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        transform.rotation = targetRotation; // 최종 회전 설정
    }
}
