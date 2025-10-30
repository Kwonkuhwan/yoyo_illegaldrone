using Illegaldrone;
using KKH;
using KKH.MySQL;
using Photon.Pun;
using RJH;
using RJH.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DroneSpawnPoint : MonoBehaviour
{
    [SerializeField] private ObjectController objectController;
    public ObjectController objController => objectController;
    [SerializeField] private Button deleteButton;
    public GameObject scoreObject;
    private Coroutine currentCoroutin;

    [Space]

    [SerializeField] private DroneType droneType;
    [SerializeField] private Transform mainTarget;
    public DroneOption droneOption;

    [Space]
    [Header("Photon Drone")]
    public GameObject go_DroneObj;
    public DroneName droneName;

    [Space]
    public List<GameObject> drones = new List<GameObject>();

    [Space]
    private bool isCreate = false;
    public float coolTime = 0;
    public float createTime = 0;
    public int createCnt = 0;
    public int createMaxCnt = 0;

    [Space]
    [SerializeField] private GameObject[] go_Fomations;

    private void Awake()
    {
        objectController = GetComponentInChildren<ObjectController>();
        deleteButton.onClick.AddListener(Delete);

        createTime = Random.Range(3.0f, 5.0f);
        createMaxCnt = 1;
        //createMaxCnt = Random.Range(3, 20);
    }

    private void Update()
    {
        drones.RemoveAll(item => item == null);

        if (isCreate)
        {
            if (createCnt >= createMaxCnt) return;

            coolTime += Time.deltaTime;
            if (coolTime >= createTime)
            {
                NetWorkDroneCreate();
                coolTime = 0.0f;
                createCnt++;
            }
        }
    }

    public void SetIsCreate(bool value)
    {
        isCreate = value;
        if (isCreate)
        {
            DroneStart();
        }
        else
        {
            DroneStop();
        }
    }

    private void DroneStart()
    {
        StartCoroutine(EDroneStart());
    }

    private void DroneStop()
    {
        StartCoroutine (EDroneStop());
    }

    public IEnumerator EDroneStart()
    {
        yield return new WaitForSeconds(4); // 3,2,1,start까지 생각 해서 4초 로 수정 2025-04-15 RJH
        foreach (GameObject obj in drones)
        {
            try
            {
                //obj.GetComponent<DroneSpawnPoint>().isCreate = false;
                if (!obj.activeInHierarchy) continue;
                obj.GetComponent<DroneObj>().go_DroneObj.GetComponent<DroneNavMeshAgent>().isStart = true;
            }
            catch
            {
                continue;
            }
        }
    }

    public IEnumerator EDroneStop()
    {
        yield return null;
        foreach (GameObject obj in drones)
        {
            try
            {
                //obj.GetComponent<DroneSpawnPoint>().isCreate = false;
                if (!obj.activeInHierarchy) continue;
                obj.GetComponent<DroneObj>().go_DroneObj.GetComponent<DroneNavMeshAgent>().isStart = false;
            }
            catch
            {
                continue;
            }
        }
    }

    public void NetWorkDroneCreate()
    {
        GameObject formation = go_Fomations[droneOption.flyingType];
        foreach(Transform pos in formation.GetComponent<SpawnPoint>().Points)
        {
            GameObject drone = null;
            if (GameManager.instance.isInstructor && PhotonNetwork.IsConnected)
            {
                drone = PhotonNetwork.Instantiate($"Drones/{droneType.ToString()}", new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
            }
            else
            {
                drone = Instantiate(go_DroneObj);
            }

            //drone.transform.position = transform.position;
            drone.transform.position = pos.position;

            if (drone != null)
            {
                DroneObj droneObj = drone.GetComponent<DroneObj>();
                if (droneObj)
                {
                    droneObj.SetMainTargetPosition(mainTarget);
                    droneObj.SetDroneOption(droneOption);
                }
                drones.Add(drone);
            }

            // SMW추가
            UltimateReplay.ReplayManager.AddReplayObjectToRecordScenes(drone);
        }       
    }

    public void SetDroneInfo(DroneOption option)
    {
        // [KKH][수정][24.10.16] number, droneType 필요 없어서 삭제 DroneOption으로 병합
        droneOption = option;
        //number = droneOption.flyingOrder;
        //SetFormationSprites(droneOption.flyingType);
    }

    public void Delete()
    {
        InstructorScenarioPage.Instance.DroneDeletePopup(droneOption.droneType);
    }

    public void HideDeleteButton()
    {
        deleteButton.gameObject.SetActive(false);
    }

    public void ShowDeleteButton()
    {
        //if(droneType != 2)
        if (droneOption.droneType != 2)
        {
            deleteButton.gameObject.SetActive(true);
        }
    }

    public void SetScore()
    {
        scoreObject.SetActive(true);

        if (currentCoroutin != null)
        {
            StopCoroutine(currentCoroutin);
        }

        currentCoroutin = StartCoroutine(HideScore());
    }

    private IEnumerator HideScore()
    {
        yield return new WaitForSeconds(2);
        scoreObject.SetActive(false);
        currentCoroutin = null;
    }
}
