using KKH;
using KKH.MySQL;
using Photon.Pun;
using Photon.Realtime;
using RJH.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace SMW
{
    public enum Mode
    {
        Solo = 0,           // 단독형
        Multiple = 1,       // 그룹형
    }

    public class TraningReady : MonoBehaviourPunCallbacks
    {
        [Header("Mode")]
        [SerializeField] Button Button_Mode_Group;
        [SerializeField] Button Button_Mode_Solo;
        [SerializeField] TMP_Text Text_Group;
        [SerializeField] TMP_Text Text_Solo;
        [SerializeField] Color color_NonActive;

        [Header("Trainee")]
        [SerializeField] TextMeshProUGUI Text_TranieeNumber;
        [SerializeField] TranieeBox[] tranieeBoxes;

        [Header("RegistTraniee")]
        [SerializeField] Button Button_TranieeAdd;
        [SerializeField] GameObject Popup_RegistTraniee;

        [Header("TrainingReady")]
        [SerializeField] Button Button_TrainingReady;
        [SerializeField] TMP_Text Text_TraningReady;
        [SerializeField] Color Color_TraningReady;


        Mode _mode;

        int current_Traniee_number = 0;

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            players.Add(newPlayer);
            UpdateTranieeInfo(newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            players.Remove(otherPlayer);
            UpdateTranieeInfo(otherPlayer);
        }

        private void Start()
        {
            // Defalut focus 그룹형
            _mode = Mode.Multiple;
            ChangeTranieeNumber(0);
            // 방에 들어와 있던 플레이어 리스트를 가져옴.
            players = PhotonManager_.Inst.GetPlayerList();
            ButtonBind();
            UpdateTranieeInfo(null);

            
            
        }

        void ButtonBind()
        {
            Button_Mode_Group.onClick.AddListener(() =>
            {
                if (_mode == Mode.Multiple) return;

                // 2025-04-17 단체모드일 때 설정====================================
                PhotonManager_.Inst.SetRoomCustomProerty("IsSoloMode", false);
                //==================================================================

                OnClickSwitchMode(Mode.Multiple);
                Button_Mode_Group.GetComponent<Image>().enabled = true;
                Button_Mode_Solo.GetComponent<Image>().enabled = false;
                Text_Group.color = Color.white;
                Text_Solo.color = color_NonActive;
            });
            Button_Mode_Solo.onClick.AddListener(() =>
            {
                if(_mode == Mode.Solo) return;

                // 2025-04-16 RJH 지금 훈련생이 두 명이상 대기 중인지 확인=========
                int count = 0;
                foreach (TranieeBox traineebox in tranieeBoxes)
                {
                    if(traineebox.GetDisplayOn())
                    {
                        count++;
                    }
                }
                if(count >= 2)
                {
                    return; //두 명이상이면 종료 
                }
                // ==================================================================
                
                // 2025-04-17 솔로모드일 때 다른 훈련생이 참여 못하도록 룸 설정 변경
                PhotonManager_.Inst.SetRoomCustomProerty("IsSoloMode", true);
                //==================================================================

                OnClickSwitchMode(Mode.Solo);
                Button_Mode_Group.GetComponent<Image>().enabled = false;
                Button_Mode_Solo.GetComponent<Image>().enabled = true;
                Text_Group.color = color_NonActive;
                Text_Solo.color = Color.white;
            });
            // 훈련생 등록 버튼 이벤트
            Button_TranieeAdd.onClick.AddListener(() =>
            {
                Popup_RegistTraniee.SetActive(true);
            });
            // 훈련 준비 버튼 이벤트
            Button_TrainingReady.onClick.AddListener(() =>
            {
                if(Button_TrainingReady.GetComponent<ButtonEvent>().IsActive)
                {
                    PhotonManager_.Inst.SceneLoad("02.ScenarioScene");
                }
            });
        }

        /// <summary>
        /// 훈련생 정보 초기화
        /// </summary>
        void ResetTranieeInfo()
        {
            if(_mode == Mode.Solo)
            {
                tranieeBoxes[0].ShowDisplay(Display.Default);
                for (int i = 1; i < tranieeBoxes.Length; i++)
                {
                    tranieeBoxes[i].ShowDisplay(Display.OFF);
                }
            }
            else
            {
                for (int i = 0; i < tranieeBoxes.Length; i++)
                {
                    tranieeBoxes[i].ShowDisplay(Display.Default);
                }
            }
        }

        List<Player> players = new List<Player>();
        /// <summary>
        /// 훈련모드 및 훈련생이 들어오거나 나갔을 때 이벤트를 받아서 훈련생정보를 업데이트
        /// </summary>
        /// <param name="player"></param>
        void UpdateTranieeInfo(Player player)
        {
            ResetTranieeInfo();
            
            // 방에 플레이어가 들어와있지 않을 경우 return
            if (players == null || players.Count == 0)
            {
                current_Traniee_number = 0;
                ChangeTranieeNumber(0);
                CheckTrainingReady();
                return;
            }

            Hashtable ht = new Hashtable();
            UserProperties _userProperties = new UserProperties();

            // 첫번째 훈련생 슬롯에 정보 입력
            if (_mode == Mode.Solo)
            {
                current_Traniee_number = 1;
                ht = players[0].CustomProperties;
                _userProperties = JsonUtility.FromJson<UserProperties>(ht["UserProperties"].ToString());
                if (_userProperties.userGroup != UserGroup.Trainee) return;
                              
                ChangeTranieeNumber(1);
                SetDataTranieeBox(0, _userProperties, players[0]);
            }
            else
            {
                current_Traniee_number = players.Count;
                for (int i = 0; i < players.Count; i++)
                {
                    ht = players[i].CustomProperties;
                    _userProperties = JsonUtility.FromJson<UserProperties>(ht["UserProperties"].ToString());
                    if (_userProperties.userGroup != UserGroup.Trainee) return;

                    ChangeTranieeNumber(i + 1);
                    SetDataTranieeBox(i, _userProperties, players[i]);
                }
            }

            UTILS.Log($"{_userProperties.userName}플레이어 입장.");
            CheckTrainingReady();
        }

        /// <summary>
        /// 훈련 모드 선택
        /// </summary>
        /// <param name="mode"></param>
        void OnClickSwitchMode(Mode mode)
        {
            _mode = mode;
            UpdateTranieeInfo(null);
        }

        /// <summary>
        /// 훈련생 정보 업데이트
        /// </summary>
        /// <param name="index"></param>
        /// <param name="properties"></param>
        void SetDataTranieeBox(int index, UserProperties properties, Player player)
        {
            tranieeBoxes[index].ChangeGender((Gender)properties.gender);
            tranieeBoxes[index].SetID(properties.id);
            tranieeBoxes[index].SetName(properties.userName);
            tranieeBoxes[index].SetPlayer(player);
            tranieeBoxes[index].ShowDisplay(Display.ON);
            PhotonManager_.Inst.SetPlayerCustomProperty("TraineeNumber", index, player); // 2025-04-24 유지환 훈련생이 중간에 없어져도 문제가 없도록 이곳에서 번호 저장
        }

        /// <summary>
        /// 훈련 인원 업데이트
        /// </summary>
        /// <param name="current"></param>
        void ChangeTranieeNumber(int current)
        {
            switch (_mode)
            {
                case Mode.Solo:
                    {
                        Text_TranieeNumber.text = $"훈련 인원 ({current} / 1)";
                    }
                    break;
                case Mode.Multiple:
                    {
                        Text_TranieeNumber.text = $"훈련 인원 ({current} / 4)";
                    }
                    break;
            }
        }

        /// <summary>
        /// 훈련준비 체크
        /// </summary>
        void CheckTrainingReady()
        {
            switch (_mode)
            {
                case Mode.Solo:
                    {
                        if(current_Traniee_number == 1)
                        {
                            /* TODO
                             * HDM상태확인
                             */
                            Button_TrainingReady.interactable = true;
                            Button_TrainingReady.GetComponent<ButtonEvent>().Activate();
                            Text_TraningReady.color = Color.white;
                        }
                        else
                        {
                            Button_TrainingReady.interactable = false;
                            Button_TrainingReady.GetComponent<ButtonEvent>().Deactivate();
                            Text_TraningReady.color = Color_TraningReady;
                        }
                    }
                    break;
                case Mode.Multiple:
                    {
                        if (current_Traniee_number > 1)
                        {
                            /* TODO
                             * HDM상태확인
                             */
                            Button_TrainingReady.interactable = true;
                            Button_TrainingReady.GetComponent<ButtonEvent>().Activate();
                            Text_TraningReady.color = Color.white;
                        }
                        else
                        {
                            Button_TrainingReady.interactable = false;
                            Button_TrainingReady.GetComponent<ButtonEvent>().Deactivate();
                            Text_TraningReady.color = Color_TraningReady;
                        }
                    }
                    break;
            }
        }
    }
}