//
// Shader Graph "Scene Color" support for Built-In pipeline
// Works only in Differed rendering mode
// Attach this script to Main Camera
//

using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TGGameEngine
{

    [ExecuteAlways]
    [RequireComponent(typeof(Camera))]
    public class TGSceneColorSetup : MonoBehaviour
    {

        CommandBuffer cmdSet, cmdRelease;


        private void Update()
        {
            if (cmdSet == null)
            {
                Camera cam = GetComponent<Camera>();
                int propID = Shader.PropertyToID("_CameraOpaqueTexture");

                cmdSet = new CommandBuffer();
                cmdSet.name = "Set";
                cmdSet.GetTemporaryRT(propID, cam.pixelWidth, cam.pixelHeight, 0);
                cmdSet.Blit(null, propID);

                cmdRelease = new CommandBuffer();
                cmdRelease.name = "Release";
                cmdRelease.ReleaseTemporaryRT(propID);
                // add to game view camera
                cam.AddCommandBuffer(CameraEvent.AfterLighting, cmdSet);
                cam.AddCommandBuffer(CameraEvent.AfterEverything, cmdRelease);
                // add to scene view cameras
#if UNITY_EDITOR
                if (cam.tag == "MainCamera")
                {
                    Camera[] sceneCameras = UnityEditor.SceneView.GetAllSceneCameras();
                    foreach (Camera c in sceneCameras)
                    {
                        c.AddCommandBuffer(CameraEvent.AfterLighting, cmdSet);
                        c.AddCommandBuffer(CameraEvent.AfterEverything, cmdRelease);
                    }
                }
#endif
            }
        }

        void OnDisable()
        {
            if (cmdSet != null)
            {
                Camera cam = GetComponent<Camera>();
                cam.RemoveCommandBuffer(CameraEvent.AfterLighting, cmdSet);
                cam.RemoveCommandBuffer(CameraEvent.AfterEverything, cmdRelease);

                // Scene View
#if UNITY_EDITOR
                if (cam.tag == "MainCamera")
                {
                    Camera[] sceneCameras = UnityEditor.SceneView.GetAllSceneCameras();
                    foreach (Camera c in sceneCameras)
                    {
                        c.RemoveCommandBuffer(CameraEvent.AfterLighting, cmdSet);
                        c.RemoveCommandBuffer(CameraEvent.AfterEverything, cmdRelease);
                    }
                }
#endif
            }

        }


    }
}