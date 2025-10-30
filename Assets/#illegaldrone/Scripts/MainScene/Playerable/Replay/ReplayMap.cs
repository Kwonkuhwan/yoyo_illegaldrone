using SMW;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ReplayMap : MonoBehaviour
{
    [SerializeField] GameObject[] Map;
    [SerializeField] GameObject[] TimeZone;
    [SerializeField] GameObject[] Weather;

    [SerializeField] Camera Weather_Cam;

    public void Set(int map, int timeZone, int weather)
    {
        SetMap(map);
        SetTime(timeZone);
        SetWeather(weather);
    }

    /// <summary>
    /// 맵 설정
    /// </summary>
    void SetMap(int index)
    {
        for(int i = 0; i < Map.Length; i++)
        {
            if (i == index)
            {
                Map[i].SetActive(true);
            }
            else
            {
                Map[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// 시간대 설정
    /// </summary>
    void SetTime(int index)
    {
        for (int i = 0; i < TimeZone.Length; i++)
        {
            if (i == index)
            {
                TimeZone[i].SetActive(true);
            }
            else
            {
                TimeZone[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// 날씨 설정
    /// </summary>
    void SetWeather(int index)
    {
        for(int i = 0; i < Weather.Length; i++)
        {
            if (i == index)
            {
                Weather[i].SetActive(true);
            }
            else
            {
                Weather[i].SetActive(false);
            }
        }

        for(int i = 0; i < ReplayManager.Instance.replayCanvas.PlayerCam.Length; i++)
        {
            ReplayManager.Instance.replayCanvas.PlayerCam[i].GetUniversalAdditionalCameraData().cameraStack.Add(Weather_Cam);
        }
    }
}
