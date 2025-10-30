using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFog : MonoBehaviour
{
    private void OnEnable()
    {
        RenderSettings.fogDensity = 0.001f;
        RenderSettings.fog = true;
    }

    private void OnDisable()
    {
        RenderSettings.fogDensity = 0;
        RenderSettings.fog = false;
    }
}
