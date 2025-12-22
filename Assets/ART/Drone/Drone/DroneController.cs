using UnityEngine;
using UnityEngine.UI;

public class DroneController : MonoBehaviour
{
    [Header("드론 날개 (프로펠러)")]
    public GameObject wing1;
    public GameObject wing2;
    public GameObject wing3;
    public GameObject wing4;

    [Header("이동 속도 설정")]
    public float moveSpeed = 5f;            // 기본 이동 속도
    public float accelerationFactor = 2f;   // 가속 배율
    public float ascendSpeed = 3f;          // 상승/하강 속도
    public float rotationSpeed = 70f;       // 방향키 회전 속도

    [Header("날개 회전 속도")]
    public float wingRotationSpeed = 300f;  // 프로펠러 회전 속도

    [Header("가속 시 드론 기울임 (Z축)")]
    public float tiltAngle = 10f;           // Shift+이동 시 기울일 각도
    public float tiltSmooth = 5f;           // 기울임 보간 속도
    private float currentTiltZ = 0f;        // 내부적으로 Z축 기울임 추적

    [Header("연료 게이지 설정")]
    public float fuel = 100f;                  // 연료 최대치 (100%)
    public float fuelConsumptionRate = 50f;    // 대쉬 시 초당 소모 연료량 (높은 값이면 빨리 소모됨)
    public float fuelRecoveryRate = 10f;       // 대쉬 중이 아닐 때 초당 회복 연료량
    public Image fuelImage;                    // Filled 타입의 Image 컴포넌트 연결

    void Update()
    {
        //--------------------------------------------------
        // 0) 연료 게이지 UI 업데이트 (fillAmount: 0 ~ 1)
        //--------------------------------------------------
        if (fuelImage != null)
        {
            fuelImage.fillAmount = fuel / 100f;
        }

        //--------------------------------------------------
        // 1) 날개(프로펠러) 회전
        //--------------------------------------------------
        float spin = wingRotationSpeed * Time.deltaTime;
        if (wing1) wing1.transform.Rotate(0f, spin, 0f);
        if (wing2) wing2.transform.Rotate(0f, spin, 0f);
        if (wing3) wing3.transform.Rotate(0f, spin, 0f);
        if (wing4) wing4.transform.Rotate(0f, spin, 0f);

        //--------------------------------------------------
        // 2) 입력 받기 (WASD)
        //--------------------------------------------------
        float horizontal = Input.GetAxis("Horizontal"); // A/D
        float vertical = Input.GetAxis("Vertical");       // W/S

        // Shift 키 입력 확인
        bool isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        // 연료가 남아있어야 대쉬 가능
        bool canDash = fuel > 0f;

        // 기본 이동 속도 설정
        float currentSpeed = moveSpeed;

        //--------------------------------------------------
        // 3) 연료 소모 & 회복 로직
        //--------------------------------------------------
        if (isShiftPressed && canDash)
        {
            // 가속 시 연료 소모
            currentSpeed *= accelerationFactor;
            fuel -= fuelConsumptionRate * Time.deltaTime;
            fuel = Mathf.Clamp(fuel, 0f, 100f);
        }
        else
        {
            // 대쉬 중이 아닐 때 연료 회복
            fuel += fuelRecoveryRate * Time.deltaTime;
            fuel = Mathf.Clamp(fuel, 0f, 100f);
        }

        //--------------------------------------------------
        // 4) 이동 방향 설정
        //--------------------------------------------------
        // 드론 전진/측면 방향 (원래 코드의 설정)
        Vector3 droneForward = -transform.right;
        Vector3 droneRight = transform.forward;
        Vector3 moveDir = (droneForward * vertical) + (droneRight * horizontal);
        if (moveDir.magnitude > 1f)
            moveDir.Normalize();
        moveDir.y = 0; // WASD는 Y축 이동 제외

        // 이동 적용
        transform.position += moveDir * (currentSpeed * Time.deltaTime);

        //--------------------------------------------------
        // 5) Space / LeftCtrl로 상승/하강
        //--------------------------------------------------
        if (Input.GetKey(KeyCode.Space))
        {
            transform.position += Vector3.up * ascendSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            transform.position += Vector3.down * ascendSpeed * Time.deltaTime;
        }

        //--------------------------------------------------
        // 6) 방향키(←/→)로 회전
        //--------------------------------------------------
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(0f, -rotationSpeed * Time.deltaTime, 0f, Space.World);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.World);
        }

        //--------------------------------------------------
        // 7) 가속 시 Z축 틸팅 (연료가 없으면 틸팅 없음)
        //--------------------------------------------------
        bool isMoving = (Mathf.Abs(horizontal) > 0.01f || Mathf.Abs(vertical) > 0.01f);
        // 연료가 남아있을 때만 틸팅 적용
        float targetTilt = (isShiftPressed && canDash && isMoving) ? tiltAngle : 0f;
        currentTiltZ = Mathf.Lerp(currentTiltZ, targetTilt, Time.deltaTime * tiltSmooth);
        Quaternion tiltRotation = Quaternion.Euler(0f, 0f, -currentTiltZ);
        transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * tiltRotation;
    }
}
