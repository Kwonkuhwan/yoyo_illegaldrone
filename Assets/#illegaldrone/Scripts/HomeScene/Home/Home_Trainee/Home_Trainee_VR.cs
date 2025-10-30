using ExitGames.Client.Photon;
using KKH;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class Home_Trainee_VR : MonoBehaviourPunCallbacks
{
    private static Home_Trainee_VR inst;
    public static Home_Trainee_VR Inst => inst;

    [SerializeField] private GameObject go_Lobby;
    [SerializeField] private GameObject go_ScenarioStart;
    [SerializeField] private GameObject go_FadeCanvas;

    [SerializeField] private Button btn_Training;
    [SerializeField] private Button btn_Start;
    [SerializeField] private Button btn_Cancel;

    [SerializeField] private bool isScanrioReady = false;
    [SerializeField] private bool isTriningReady = false;

    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
        }

        btn_Cancel.onClick.AddListener(() => CancleBtnClick());
        btn_Start.onClick.AddListener(() => StartBtnClick());
    }

    /// <summary>
    /// SMW 추가 
    /// 프로퍼티 변경 시 호출되는 함수
    /// </summary>
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        // "IsScenarioReady" 프로퍼티가 변경되었는지 확인
        if (changedProps.ContainsKey("IsScenarioReady"))
        {
            if (targetPlayer.CustomProperties.TryGetValue("IsScenarioReady", out object isReady) && (bool)isReady)
            {
                Debug.Log("Scenario Ready");
                SetScenarioUI((bool)isReady);
            }
            else // 2025-04-30 RJH 준비가 풀렸을 경우 추가
            {
                Debug.Log("Scenario not Ready");
                SetScenarioUI((bool)isReady);
            }
        }

        // "IsTriningReady" 프로퍼티가 변경되었는지 확인
        if (changedProps.ContainsKey("IsTriningReady"))
        {
            if (targetPlayer.CustomProperties.TryGetValue("IsTriningReady", out object isReady) && (bool)isReady)
            {
                Debug.Log("Training Ready");
                go_FadeCanvas.SetActive(true);
            }
        }
    }

    //private void Update()
    //{
    //    if (!isScanrioReady)
    //    {
    //        foreach (Player player in PhotonNetwork.PlayerList)
    //        {
    //            if (!player.IsMasterClient) continue;

    //            if (player.CustomProperties.TryGetValue("IsScenarioReady", out object isReady))
    //            {
    //                if ((bool)isReady)
    //                {
    //                    SetScenarioUI();
    //                    // 초대 메시지 처리 로직 추가
    //                }
    //            }
    //        }
    //    }

    //    if(!isTriningReady)
    //    {
    //        foreach (Player player in PhotonNetwork.PlayerList)
    //        {
    //            if (!player.IsMasterClient) continue;

    //            if (player.CustomProperties.TryGetValue("IsTriningReady", out object isReady))
    //            {
    //                if ((bool)isReady)
    //                {
    //                    go_FadeCanvas.SetActive(true);
    //                }
    //            }
    //        }
    //    }
    //}

    public void SetScenarioUI(bool value) 
    {
        isScanrioReady = value;

        go_Lobby.SetActive(!value);
        go_ScenarioStart.SetActive(value);
    }

    public void SetFadeCanvase()
    {

    }

    public void StartBtnClick()
    {
        PhotonManager_.Inst.SceneLoad("02.ScenarioScene");
        //SceneManager.LoadScene("ScenarioScene");
    }

    public void CancleBtnClick()
    {
        PhotonManager_.Inst.SceneLoad("00.Login");
        //SceneManager.LoadScene("Login");
    }
}
