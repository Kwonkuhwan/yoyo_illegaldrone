using Illegaldrone;
using RJH;
using System.Linq;
using UnityEngine;

public class ScenarioResultManager : MonoBehaviour
{
    public GameObject panel_Finish;

    private void Awake()
    {
    }

    private void Update()
    {
        bool isAllDronesDestroyed = MapObjectController.Inst.Drones
        .Where(drone => drone.activeSelf) // 활성화된 게임 오브젝트만 선택
        .Select(drone => drone.GetComponent<DroneObj>())
        .All(droneObj => droneObj != null && droneObj.isDroneDestroy);

        if (isAllDronesDestroyed)
        {
            if (!GameManager.instance.isInstructor)
            {
                SetSuccessful(true);
            }
        }
    }

    public void SetSuccessful(bool isSuccessful)
    {
        panel_Finish.SetActive(true);
        panel_Finish.GetComponent<TrainingEndCanvas>().SetTrainingEndType(TrainingEndType.Success);
    }
}
