using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrainingEndType : int
{
    None = -1,
    Start,
    Pause,
    Success,
    Fail
}

public class TrainingEndCanvas : MonoBehaviour
{
    public TrainingEndType trainingEndType = TrainingEndType.None;

    public List<GameObject> panels = new List<GameObject>();

    private void Awake()
    {
        trainingEndType = TrainingEndType.None;
    }

    public void SetTrainingEndType(TrainingEndType type)
    {
        trainingEndType = type;
    }

    public void ShowPanel()
    {
        if (trainingEndType == TrainingEndType.None)
        {
            return;
        }

        foreach (var panel in panels)
        {
            panel.SetActive(false);
        }

        panels[(int)trainingEndType].SetActive(true);
    }
}
