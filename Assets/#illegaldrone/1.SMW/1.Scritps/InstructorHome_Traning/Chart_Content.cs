using KKH;
using KKH.MySQL;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SMW
{
    public class Chart_Content : MonoBehaviour
    {
        enum Name
        {
            Index = 0,
            Date,
            Place,
            Wheather,
            Mode,
            Time,
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

        TrainingInfo _info;

        void Awake()
        {
            foreach (Transform t in transform)
            {
                datas.Add(t);
            }
        }

        public void Set(TrainingInfo info, int index)
        {
            SetData(Name.Index).text = index.ToString();
            SetData(Name.Date).text = $"{info.playScenarioDateTime.Year}.{info.playScenarioDateTime.Month}.{info.playScenarioDateTime.Day}";
            SetData(Name.Place).text = SetPlace(info.missionMap);
            SetData(Name.Wheather).text = SetWeather(info.weather);
            SetData(Name.Mode).text = (Mode)info.playMode == Mode.Solo ? "단독형" : "그룹형";
            SetData(Name.Time).text = info.playTime.ToString("HH:mm:ss");
            SetResult(info.teamMissionResult);
            SetReplay(info.btnActive);
            SetData(Name.Admin).text = info.createUser;

            _info = info;
        }

        private string SetPlace(int type)
        {
            MapType mapType = (MapType)type;
            if (mapType == MapType.Gwanghwamun)
            {
                return "도심";
            }
            else if (mapType == MapType.Airport)
            {
                return "공항";
            }
            else if (mapType == MapType.Nuclearplant)
            {
                return "원전";
            }

            return "미선택";
        }

        private string SetWeather(int type)
        {
            Weather weather = (Weather)type;

            if (weather == Weather.Sunshine)
            {
                return "맑음";
            }
            else if (weather == Weather.Rain)
            {
                return "비";
            }
            else if (weather == Weather.Fog)
            {
                return "안개";
            }
            else if (weather == Weather.Snow)
            {
                return "눈";
            }

            return "미선택";
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
            if(_info.btnActive == false)
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

                    ReplayFile.Path = $"{Application.persistentDataPath}/Replay/{_info.createUser}/";
                    ReplayFile.Name = $"{_info.playScenarioDateTime.ToString("yyyy-MM-dd_HH-mm-ss")}.replay";
                    PhotonNetwork.LoadLevel("03.Replay");
                }
                else
                {
                    ReplayFile.Path = $"{Application.persistentDataPath}/Replay/{_info.createUser}/";
                    ReplayFile.Name = $"{_info.playScenarioDateTime.ToString("yyyy-MM-dd_HH-mm-ss")}.replay";
                    UnityEngine.SceneManagement.SceneManager.LoadScene("03.Replay");
                }
            }

#if UNITY_EDITOR
            string path = $"{Application.persistentDataPath}/Replay/{_info.createUser}/";
            string name = $"{_info.playScenarioDateTime.ToString("yyyy-MM-dd_HH-mm-ss")}.replay";
            UTILS.Log($"리플레이 : {path}{name}");
#endif
        }

        TMP_Text SetData(Name _name)
        {
            return datas[(int)_name].GetComponentInChildren<TMP_Text>();
        }
    }
}