using Illegaldrone;
using KKH;
using Photon.Pun;
using SMW;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UltimateReplay;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReplayCanvas : MonoBehaviour
{
    [Serializable]
    public struct UI
    {
        [Header("Main")]
        public RawImage MainView;
        public RenderTexture MainRenderTexture;

        [Header("Top")]
        public TMP_Text Date;
        public TMP_Text InstructorName;

        [Header("Bottom")]
        public Button Out;

        [Header("Right")]
        public ToggleGroup CamToggle;
        public TMP_Text map;
        public Trainee[] Trainee;
        public Toggle[] toggles;
    }
    [SerializeField] UI ui;

    readonly string[] Map = { "광화문", "인천공항", "원자력" };

    public Camera[] PlayerCam;
    RenderTexture[] PlayerRT;
    public Texture2D noData;

    int totalPlayer = 0;
    int display = 0;

    private void Awake()
    {
        if(ui.Out != null) ui.Out.onClick.AddListener(BackToLogin);
        for(int i = 0; i < ui.toggles.Length; i++)
        {
            int index = i;
            ui.toggles[index].onValueChanged.AddListener((bool value) => { OnToggle(index, value); });
        }
    }

    /// <summary>
    /// 리플레이시 데이터 세팅 초기화
    /// </summary>
    public void init()
    {
        // top
        ui.InstructorName.text = GameManager.instance.userInfo.userName + " 님";
        ui.Date.text = ReplayFile.Name.Replace(".replay", "");

        // Cam & RenderTexture Setting
        PlayerRT = new RenderTexture[4];
        for (int i = 0; i < PlayerRT.Length; i++)
        {
            int index = i;
            PlayerRT[index] = new RenderTexture(1920, 1080, 24);
            PlayerCam[index].targetTexture = PlayerRT[index];
            PlayerCam[index].gameObject.SetActive(false);
            PlayerCam[index].GetComponent<PlayerCam>().SetIndex(index + 1);     // 카메라 인덱스 설정
        }
    }

    public void CameraOn(int i)
    {
        if (i == -1) return;

        ui.Trainee[i - 1].SetView(PlayerRT[i - 1]);
        if (display == i)
        {
            ui.MainView.texture = PlayerRT[display - 1];
        }
    }

    public void CameraOff(int i)
    {
        if(i == -1) return;

        ui.Trainee[i - 1].SetNoData(noData);
        if (display == i)
        {
            ui.MainView.texture = ui.MainRenderTexture;
        }
    }

    private void Reset()
    {
        StopAllCoroutines();
    }

    IEnumerator FindRemotePlayer(int cam, int identity)
    {
        //RemotePlayer
        while (true)
        {
            GameObject player = GameObject.Find("RemotePlayer(Clone)");

            if(player != null && player.GetComponent<ReplayObject>().ReplayIdentity.ID == identity)
            {
                player.name = $"{cam}_RemotePlayer";
                Transform headTrans = player.transform.Find("Head");
                PlayerCam[cam].transform.parent = headTrans;
                PlayerCam[cam].transform.localPosition = Vector3.zero;
                PlayerCam[cam].transform.localRotation = Quaternion.identity;
                PlayerCam[cam].transform.localScale = Vector3.one;
                PlayerCam[cam].gameObject.SetActive(true);

                MapCanvas.Instance.AddTraineeTransform(headTrans, cam);
                yield break;
            }
            yield return null;
        }
    }

    /// <summary>
    /// 훈련생 목록 업데이트
    /// </summary>
    public void SetPlayer(PlayerReplayData[] data)
    {
        for(int i = 0; i < 4; i++)
        {
            if (data.Any(x => x.CamNumber == i))
            {
                Debug.Log($"리플레이 {data[i].CamNumber}번 카메라 설정 : {data[i].name}");
                ui.Trainee[data[i].CamNumber].Set(data[i]);
                MapCanvas.Instance.AddTranieeName(data[i].CamNumber, data[i].name);
                StartCoroutine(FindRemotePlayer(data[i].CamNumber, data[i].identity));
            }
            else
            {
                ui.Trainee[i].Reset();
            }
        }
    }

    /// <summary>
    /// 리플레이 카메라 선택 / 0: 메인 카메라 / 1~4: 훈련생 카메라
    /// </summary>
    /// <param name="index"></param>
    /// <param name="isOn"></param>
    void OnToggle(int index, bool isOn)
    {
        if (isOn == false) return;

        display = index;
        if (display == 0)
        {
            ui.MainView.texture = ui.MainRenderTexture;
        }
        else
        {
            if(ui.Trainee[display - 1].isView)
            {
                ui.MainView.texture = PlayerRT[display - 1];
            }
            else
            {
                ui.MainView.texture = ui.MainRenderTexture;
            }
        }
    }

    /// <summary>
    /// map UI 설정
    /// </summary>
    public void SetMap(int map)
    {
        ui.map.text = Map[map];
    }

    /// <summary>
    /// 리플레이 씬 나가기
    /// </summary>
    void BackToLogin()
    {
        PhotonManager_.Inst.isRejoinRoom = true;
        PhotonManager_.Inst.LeaveRoom();
        SceneManager.LoadScene("01_1.Home_instructor");
    }
}
