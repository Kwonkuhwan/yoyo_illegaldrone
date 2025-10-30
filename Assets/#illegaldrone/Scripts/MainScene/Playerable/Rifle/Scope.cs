using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SMW
{
    public class Scope : MonoBehaviour
    {
        [Range(1f, 20f)]
        public int magnification;      // 배율

        [SerializeField] Camera Scope_Camera;
        [SerializeField] MeshRenderer Material_Lens;

        float defaultFov = 60f;

        RenderTexture renderTexture;
        Material material;

        private void Reset()
        {
            ChangeScopeMagnification(1);
        }

        private void Awake()
        {
            ChangeScopeMagnification(magnification);

            renderTexture = new RenderTexture(256, 256, 24);
            Scope_Camera.targetTexture = renderTexture;

            material = new Material(Shader.Find("Unlit/Scope"));
            material.mainTexture = renderTexture;

            if(Material_Lens != null)
            {
                Material_Lens.material = material;
            }
        }

        /// <summary>
        /// 스코프 배율 설정
        /// </summary>
        /// <param name="_value"></param>
        public void ChangeScopeMagnification(int _value)
        {
            if (_value > 20 || _value < 1) return;

            // 카메라 배율 설정
            Scope_Camera.fieldOfView = defaultFov / _value;
        }
    }
}