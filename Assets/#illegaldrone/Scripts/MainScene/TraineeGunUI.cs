using BNG;
using Photon.Pun;
using SMW;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TraineeGunUI : MonoBehaviour
{
    [SerializeField] private GameObject traineeUIGroup;
    [SerializeField] private Image timerBar;
    [SerializeField] private Sprite defaulttimerBar;
    [SerializeField] private Sprite emergencytimerBar;
    [SerializeField] private TextMeshProUGUI timerText;

    private float endTime;

    [SerializeField] private TextMeshProUGUI totalDroneNumber;
    [SerializeField] private TextMeshProUGUI killDroneNumber;

    private int killDroneCount;
    private InputBridge inputBrige;

    private void Awake()
    {
        // 리플레이 중엔 안보이게 숨김
        if(ReplayManager.Instance.isReplaying)
        {
            gameObject.SetActive(false);
            return;
        }

        SetTimer();
        killDroneCount = 0;
        GamePlay.Instance.timerCallBack.AddListener(TimerUpdate);
        GamePlay.Instance.killCountCallBack.AddListener(KillCountUpdate);
        inputBrige = InputBridge.Instance;
    }

    private void Update()
    {
        SetUIActive();
    }

    private void SetUIActive()
    {
        if(inputBrige.AButton == true)
        {
            traineeUIGroup.SetActive(true);
        }
        else if(inputBrige.AButton == false)
        {
            traineeUIGroup.SetActive(false);
        }
    }

    private void SetTimer()
    {
        endTime = (int)PhotonNetwork.CurrentRoom.CustomProperties["LimitTime"];
        Debug.Log(endTime);
    }

    private void TimerUpdate(float minute, float secound)
    {
        timerText.text = $"{(int)minute:D2}:{(int)secound:D2}";
        
        timerBar.fillAmount = (float)((endTime * 60f) - (minute * 60f + secound)) / (float)(endTime * 60f);

        if (endTime - minute <= 5)
        {
            if(timerBar.sprite != emergencytimerBar)
            {
                timerBar.sprite = emergencytimerBar;
            }
        }
    }



    private void KillCountUpdate()
    {
        killDroneCount++;
        killDroneNumber.text = $"{killDroneCount:D2}";
    }

}
