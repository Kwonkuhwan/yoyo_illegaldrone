using SMW;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MapCanvas : MonoBehaviour
{
    public static MapCanvas Instance;

    [Serializable]
    public struct UI
    {
        public Transform defenseObj;
        public Traniee[] traniee;
        public Transform[] Drone;

        [Serializable]
        public struct Traniee
        {
            public Transform trs;
            public TMP_Text name;
        }
    }
    public UI ui;

    // UI 고정 높이값
    readonly int height = 800;

    private List<Transform> list_DronePosition = new List<Transform>();
    private Dictionary<int, Transform> dic_TranieePosition = new Dictionary<int, Transform>();

    private void Awake()
    {
        Instance = this;
    }

    private void Reset()
    {
        list_DronePosition.Clear();
        dic_TranieePosition.Clear();
    }

    void Update()
    {
        // 훈련생 위치 동기화
        foreach(var item in dic_TranieePosition)
        {
            SetTrinee(item.Key, new Vector2(item.Value.position.x, item.Value.position.z));
        }
    }

    public void AddTraineeTransform(Transform obj, int index)
    {
        if(dic_TranieePosition.ContainsKey(index))
        {
            dic_TranieePosition[index] = obj;
            return;
        }
        else
        {
            dic_TranieePosition.Add(index, obj);
        }
    }

    public void AddTranieeName(int index, string name)
    {
        ui.traniee[index].name.text = name;
        ui.traniee[index].trs.gameObject.SetActive(true);
    }

    public void AddDroneTransform(Transform obj)
    {
        return;
        if (list_DronePosition.Contains(obj))
        {
            Debug.LogError("이미 추가된 드론입니다.");
            return;
        }
        list_DronePosition.Add(obj);

        for(int i = 0; i < ui.Drone.Length; i++)
        {
            if(i < list_DronePosition.Count)
            {
                ui.Drone[i].gameObject.SetActive(true);

            }
            else
            {
                ui.Drone[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 방어건물 위치 세팅
    /// </summary>
    public void SetDefenseObj(Vector2 position)
    {
        ui.defenseObj.transform.position = new Vector3(position.x, height, position.y);
    }

    /// <summary>
    /// 훈련생 위치 세팅
    /// </summary>
    /// <param name="index"> 0~3번 훈련생 </param>
    void SetTrinee(int index, Vector2 position)
    {
        ui.traniee[index].trs.position = new Vector3(position.x, height, position.y);
    }

    /// <summary>
    /// 드론 위치 세팅
    /// </summary>
    /// <param name="index"> 0자폭,1공격,2재밍,3정찰 </param>
    void SetDrone(int index, Vector2 position)
    {
        ui.Drone[index].transform.position = new Vector3(position.x, height, position.y);
    }
}
