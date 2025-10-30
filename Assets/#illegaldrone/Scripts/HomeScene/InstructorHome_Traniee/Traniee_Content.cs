using KKH;
using KKH.MySQL;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SMW
{
    public class Traniee_Content : MonoBehaviour
    {
        enum Name
        {
            Index = 0,
            Register_Date,
            ID,
            Name,
            Count,
            Traning_Date,
            Mode,
            Result,
            Replay,
            Admin
        }
        List<Transform> datas = new List<Transform>();

        [Header("훈련결과")]
        [SerializeField] Sprite Sprite_Fail;
        [SerializeField] Sprite Sprite_Success;

        [Header("훈련확인")]
        [SerializeField] Sprite Sprite_Play;
        [SerializeField] Sprite Sprite_Play_Disable;

        TraineeInfo _info;

        void Awake()
        {
            foreach (Transform t in transform)
            {
                datas.Add(t);
            }
        }

        public void Set(TraineeInfo info, int index)
        {
            SetData(Name.Index).text = index.ToString();
            SetData(Name.Register_Date).text = $"{info.createIDDate.Year}.{info.createIDDate.Month}.{info.createIDDate.Day}";
            SetData(Name.ID).text = info.id;
            SetData(Name.Name).text = info.userName;
            SetData(Name.Count).text = info.scenarioCount.ToString();

            if(info.latestPlayDate.Year == 1999)
            {
                SetData(Name.Traning_Date).text = "최근기록없음";
            }
            else
            {
                SetData(Name.Traning_Date).text = $"{info.latestPlayDate.Year}.{info.latestPlayDate.Month}.{info.latestPlayDate.Day}";
            }
            
            SetData(Name.Mode).text = info.playMode.ToString();

            SetResult(info.missionresult);
            SetReplay(info.btnActive);

            SetData(Name.Admin).text = info.createAdminName;

            _info = info;
        }

        void SetResult(int result)
        {
            if (result == 0)
            {
                datas[(int)Name.Result].GetChild(0).GetComponent<Image>().sprite = Sprite_Fail;
                datas[(int)Name.Result].GetComponentInChildren<TMP_Text>().text = "실패";
            }
            else
            {
                datas[(int)Name.Result].GetChild(0).GetComponent<Image>().sprite = Sprite_Success;
                datas[(int)Name.Result].GetComponentInChildren<TMP_Text>().text = "성공";
            }
        }

        void SetReplay(bool replay)
        {
            if (replay)
            {
                datas[(int)Name.Replay].GetChild(0).GetComponent<ButtonEvent>().Activate();
                datas[(int)Name.Replay].GetChild(0).GetChild(0).GetComponent<Image>().sprite = Sprite_Play;
            }
            else
            {
                datas[(int)Name.Replay].GetChild(0).GetComponent<ButtonEvent>().Deactivate();
                datas[(int)Name.Replay].GetChild(0).GetChild(0).GetComponent<Image>().sprite = Sprite_Play_Disable;
            }
            datas[(int)Name.Replay].GetChild(0).GetComponent<ButtonEvent>().AddEvent(OnClickReplay);
        }

        void OnClickReplay()
        {
            if (_info.btnActive == false)
            {
                Alarm.instance.SetAlarm(false, "훈련일로부터 3개월 이내의 훈련 확인만 가능합니다.");
            }
            else
            {
                if (PhotonNetwork.IsConnected)
                {
                    // 방에 있는 훈련생들 강퇴 -> 훈련생들 로그아웃
                    Dictionary<int, Player> list = PhotonNetwork.CurrentRoom.Players;

                    if (list != null || list.Count > 1)
                    {
                        foreach (var key in list)
                        {
                            if (PhotonNetwork.LocalPlayer == key.Value)
                            {
                                continue;
                            }
                            PhotonManager_.Inst.SetPlayerCustomProperty("isKicked", true, key.Value);
                        }
                    }

                    ReplayFile.Path = $"{Application.persistentDataPath}/Replay/{_info.createAdminName}/";
                    ReplayFile.Name = $"{_info.latestPlayDate.ToString("yyyy-MM-dd")}_{_info.latestPlayDate.ToString("HH-mm-ss")}.replay";
                    PhotonNetwork.LoadLevel("03.Replay");
                }
                else
                {
                    ReplayFile.Path = $"{Application.persistentDataPath}/Replay/{_info.createAdminName}/";
                    ReplayFile.Name = $"{_info.latestPlayDate.ToString("yyyy-MM-dd")}_{_info.latestPlayDate.ToString("HH-mm-ss")}.replay";
                    UnityEngine.SceneManagement.SceneManager.LoadScene("03.Replay");
                }
            }

#if UNITY_EDITOR
            string path = $"{Application.persistentDataPath}/Replay/{_info.createAdminName}/";
            string name = $"{_info.latestPlayDate.ToString("yyyy-MM-dd")}_{_info.latestPlayDate.ToString("HH-mm-ss")}.replay";
            UTILS.Log($"리플레이 : {path}{name}");
#endif
        }

        TMP_Text SetData(Name _name)
        {
            return datas[(int)_name].GetComponentInChildren<TMP_Text>();
        }
    }
}