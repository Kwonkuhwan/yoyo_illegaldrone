using BNG;
using UnityEngine;

public class MiniMapController : MonoBehaviour
{
    [SerializeField] private GameObject miniMapObj;
    private InputBridge inputBridge;

    private void Awake()
    {
        inputBridge = InputBridge.Instance;
    }

    private void Update()
    {
        SetUIActive();
    }

    private void SetUIActive()
    {
        if(inputBridge.BButton == true)
        {
            miniMapObj.SetActive(true);
        }
        else if (inputBridge.BButton == false)
        {
            miniMapObj.SetActive(false);
        }
    }
}
