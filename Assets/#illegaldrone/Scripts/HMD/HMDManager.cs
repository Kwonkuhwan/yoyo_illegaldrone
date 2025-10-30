using BNG;
using Illegaldrone;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace KKH.OVR_HMD
{
    public class HMDManager : MonoBehaviour
    {
        public static HMDManager Instance { get; private set; }

        /// <summary>
        /// HMD 연결 상태
        /// </summary>
        public bool IsHMDConnected { get; private set; }

        /// <summary>
        /// HMD 착용 상태
        /// </summary>
        public bool IsHMDMounted { get; private set; }

        public GameObject canvas_PC;
        public GameObject canvas_VR;
        public GameObject eventSystem;

        public bool isDontDestroyOnLoad = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                if (isDontDestroyOnLoad)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            if (GameManager.instance.isInstructor)
            {
                canvas_VR.SetActive(false);
                gameObject.SetActive(false);
                eventSystem.GetComponent<VRUISystem>().enabled = false;
            }

#if !UNITY_EDITOR
            IsHMDConnected = OVRManager.isHmdPresent;
            OVRManager.HMDMounted += HandleHMDMounted;
            OVRManager.HMDUnmounted += HandleHMDUnmounted;
            OVRManager.HMDLost += HandleHMDLost;
            OVRManager.HMDAcquired += HandleHMDAcquired;
#endif
        }

        private void Update()
        {
            UTILS.LogColor($"HMD Connected : {IsHMDConnected}", Color.cyan);
            UTILS.LogColor($"HMD Mounted : {IsHMDMounted}", Color.yellow);

            if (PhotonNetwork.IsConnected && !GameManager.instance.isInstructor)
            {
#if UNITY_EDITOR
                if (Input.GetKeyDown(KeyCode.Home))
                {
                    if (canvas_PC.GetComponent<GraphicRaycaster>().enabled)
                    {
                        canvas_PC.GetComponent<GraphicRaycaster>().enabled = false;
                        eventSystem.GetComponent<InputSystemUIInputModule>().enabled = false;
                        GetComponent<VREmulator>().enabled = true;

                        IsHMDConnected = true;
                        IsHMDMounted = true;

                        if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["PlayerReady"] == false)
                        {
                            PhotonManager_.Inst.SetPlayerCustomProperty("PlayerReady", true);
                        }
                    }
                    else
                    {
                        canvas_PC.GetComponent<GraphicRaycaster>().enabled = true;
                        eventSystem.GetComponent<InputSystemUIInputModule>().enabled = true;
                        GetComponent<VREmulator>().enabled = false;

                        IsHMDConnected = false;
                        IsHMDMounted = false;

                        if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["PlayerReady"] == true)
                        {
                            PhotonManager_.Inst.SetPlayerCustomProperty("PlayerReady", false);
                        }
                    }
                }
#else
                if (IsHMDConnected && IsHMDMounted)
                {
                    canvas_PC.GetComponent<GraphicRaycaster>().enabled = false;
                    eventSystem.GetComponent<InputSystemUIInputModule>().enabled = false;
                    //eventSystem.GetComponent<VRUISystem>().enabled = true;

                    if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["PlayerReady"] == false)
                    {
                        PhotonManager_.Inst.SetPlayerCustomProperty("PlayerReady", true);
                    }
                }
                else
                {
                    canvas_PC.GetComponent<GraphicRaycaster>().enabled = true;
                    eventSystem.GetComponent<InputSystemUIInputModule>().enabled = true;
                    //eventSystem.GetComponent<VRUISystem>().enabled = false;

                    if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["PlayerReady"] == true)
                    {
                        PhotonManager_.Inst.SetPlayerCustomProperty("PlayerReady", false);
                    }
                }
#endif
                //PhotonNetwork_.LocalPlayer.SetCustomProperties(currentProperties);
                UTILS.LogColor($"PlayerReady : {PhotonNetwork.LocalPlayer.CustomProperties["PlayerReady"]}", Color.blue);
            }
        }

        private void HandleHMDMounted()
        {
            IsHMDMounted = true;
            UTILS.LogColor("HMD Mounted", Color.green);
        }

        private void HandleHMDUnmounted()
        {
            IsHMDMounted = false;
            UTILS.LogColor("HMD Unmounted", Color.red);
        }

        private void HandleHMDLost()
        {
            IsHMDConnected = false;
            UTILS.LogColor("HMD Lost", Color.red);
        }

        private void HandleHMDAcquired()
        {
            IsHMDConnected = true;
            UTILS.LogColor("HMD Acquired", Color.yellow);
        }

        private void OnDestroy()
        {
            OVRManager.HMDMounted -= HandleHMDMounted;
            OVRManager.HMDUnmounted -= HandleHMDUnmounted;
            OVRManager.HMDLost -= HandleHMDLost;
            OVRManager.HMDAcquired -= HandleHMDAcquired;
        }
    }
}