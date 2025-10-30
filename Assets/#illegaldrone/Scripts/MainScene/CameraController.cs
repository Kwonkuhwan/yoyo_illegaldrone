using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RJH
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Button zoomInButton;
        [SerializeField] private Button zoomOutButton;
        [SerializeField] private float ScrollSpeed;
        [SerializeField] private TextMeshProUGUI altitudeText;
        public Camera mainCamera;
        public float cam_ZoomMax = 900;
        public float cam_ZoomMin = 300;

        public float speed = 30.0f;
        public bool isCameraCanMove = true;

        public Vector2 minBounds; // 월드 최소 경계 (x, y)
        public Vector2 maxBounds; // 월드 최대 경계 (x, y)


        private void Awake()
        {
            zoomInButton.onClick.AddListener(() => CameraZoomInOut(-1));
            zoomOutButton.onClick.AddListener(() => CameraZoomInOut(1));

            if (mainCamera == null)
            {
                Camera[] cameras = Camera.allCameras;

                foreach (Camera camera in cameras)
                {
                    if (camera.targetTexture != null)
                    {
                        mainCamera = camera;
                        mainCamera.orthographicSize = 900;
                        break;
                    }
                }
            }

            if (mainCamera == null)
            {
                Debug.LogWarning("No camera with OutputTexture found.");
            }
        }

        private void Update()
        {
            if (mainCamera == null)
            {
                return;
            }
            if (!Application.isFocused)
                return;
            if (isCameraCanMove == false)
                return;

            CameraZoomInOutScrolling();

            //if (mainCamera.fieldOfView >= 70 && Input.GetAxis("Mouse ScrollWheel") < 0) // 카메라 줌아웃 제한
            //    return;

            //if (mainCamera.fieldOfView <= 20 && Input.GetAxis("Mouse ScrollWheel") > 0) // 카메라 줌인 제한
            //    return;

            //mainCamera.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * ScrollSpeed;

            if (Input.GetKey(KeyCode.Mouse1))
            {
                float rotateHorizontal = -Input.GetAxis("Mouse Y");
                float rotateVertical = -Input.GetAxis("Mouse X");
                mainCamera.transform.position += new Vector3(rotateVertical * speed, 0, rotateHorizontal * speed);

                CameraBoundary();
            }

            if (altitudeText != null)
            {
                altitudeText.text = $"{mainCamera.orthographicSize}M";
            }
        }

        public void CameraZoomInOutScrolling()
        {
            if (mainCamera == null)
            {
                return;
            }

            if (mainCamera.orthographicSize >= cam_ZoomMax && Input.GetAxis("Mouse ScrollWheel") < 0) // 카메라 줌아웃 제한
            {
                mainCamera.orthographicSize = cam_ZoomMax;
                return;
            }

            if (mainCamera.orthographicSize <= cam_ZoomMin && Input.GetAxis("Mouse ScrollWheel") > 0) // 카메라 줌인 제한
            {
                mainCamera.orthographicSize = cam_ZoomMin;
                return;
            }

            mainCamera.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * ScrollSpeed * 10;

            CameraBoundary();
        }

        public void CameraZoomInOut(int Value)
        {
            if (mainCamera.orthographicSize >= cam_ZoomMax && Value == -1) // 카메라 줌아웃 제한
            {
                mainCamera.orthographicSize = cam_ZoomMax;
                return;
            }

            if (mainCamera.orthographicSize <= cam_ZoomMin && Value == 1) // 카메라 줌인 제한
            {
                mainCamera.orthographicSize = cam_ZoomMin;
                return;
            }

            mainCamera.orthographicSize -= Value * ScrollSpeed;

            CameraBoundary();
        }

        private void CameraBoundary()
        {
            Vector3 position = mainCamera.transform.position;

            float cameraHeight = mainCamera.orthographicSize - 600;
            float cameraWidth = cameraHeight * mainCamera.aspect;


            position.x = Mathf.Clamp(
            position.x,
            minBounds.x + cameraWidth, // 왼쪽 경계 + 카메라 가로 반경
            maxBounds.x - cameraWidth  // 오른쪽 경계 - 카메라 가로 반경
        );


            position.z = Mathf.Clamp(
            position.z,
            minBounds.y + cameraHeight, // 아래 경계 + 카메라 세로 반경
            maxBounds.y - cameraHeight  // 위 경계 - 카메라 세로 반경
        );


            mainCamera.transform.position = position;
        }

        private void OnEnable()
        {

            if (mainCamera == null)
            {
                Camera[] cameras = Camera.allCameras;

                foreach (Camera camera in cameras)
                {
                    if (camera.targetTexture != null)
                    {
                        mainCamera = camera;
                        mainCamera.orthographicSize = 900;
                        break;
                    }
                }
            }

        }
    }
}
