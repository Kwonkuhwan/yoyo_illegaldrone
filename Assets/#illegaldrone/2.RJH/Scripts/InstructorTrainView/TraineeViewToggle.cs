using ExitGames.Client.Photon;
using KKH;
using Photon.Realtime;
using RJH.UI;
using SMW;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RJH
{
    public class TraineeViewToggle : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI traineeName;
        [SerializeField] private TextMeshProUGUI traineeID;
        [SerializeField] private Image readyIcon;
        [SerializeField] private TextMeshProUGUI defenseArea;
        [SerializeField] private TextMeshProUGUI weaponType;
        [SerializeField] private RawImage traineeView;

        //[SerializeField] private Sprite[] buttonSprites; // 0 : Default, 1 : Activate
        [SerializeField] private Sprite[] readySprites; // 0 : NotReady, 1 : Ready

        [HideInInspector]public UserProperties userProperties;

        private readonly string[] weapon = { "재밍 (Jamming)", "재밍 (Jamming)", "소총 (Rifle)" };//{ "재밍 (Jamming)", "그물망 (Nets)", "소총 (Rifle)" };
        private readonly string[] area = { "경계지대", "주방어지대", "핵심방어지대" };

        private Player _player;

        private void Update()
        {
            if (_player == null)
                return;

            Hashtable ht = _player.CustomProperties;

            bool isPlayerReady = false;
            if (ht != null && ht.ContainsKey("PlayerReady"))
            {
                isPlayerReady = (bool)ht["PlayerReady"];
            }
            SetReady(isPlayerReady);
        }

        /// <summary>
        /// 훈련에 참여하는 훈련생 정보 저장 및 이름, 아이디 텍스트 출력
        /// </summary>
        /// <param name="userProperties">훈련생 정보</param>
        public void SetTraineeInfo(Player player, UserProperties userProperties)
        {
            this.userProperties = userProperties;
            traineeName.text = userProperties.userName;
            traineeID.text = userProperties.id;
        }

        /// <summary>
        /// 훈련생이 장비하고 있는 안티 드론건 타입 텍스트 출력
        /// </summary>
        /// <param name="weaponType">장비하고 있는 무기</param>
        public void SetWeaponType(int weaponType)
        {
            weaponindex = weaponType;
            this.weaponType.text = weapon[weaponType];
        }

        /// <summary>
        /// 훈련생이 배치된 경계 구역 텍스트 출력
        /// </summary>
        /// <param name="defenseArea">배치된 경계 구역 타입</param>
        public void SetDefenseArea(int defenseArea) 
        {
            areaindex = defenseArea;
            this.defenseArea.text = area[defenseArea];
            SetWeaponType(defenseArea);
        }

        /// <summary>
        /// 훈련생 준비 상태 표시
        /// </summary>
        /// <param name="isReady">준비 되었는지 체크</param>
        public void SetReady(bool isReady) 
        {
            if (isReady)
            {
                readyIcon.sprite = readySprites[1];
                readyIcon.GetComponentInChildren<TextMeshProUGUI>().text = "준비완료";
            }
            else
            {
                readyIcon.sprite = readySprites[0];
                readyIcon.GetComponentInChildren<TextMeshProUGUI>().text = "준비 중";
            }
        }

        // SMW 추가
        // ===========================================================================
        int weaponindex = 0;
        int areaindex = 0;
        public PlayerStartEquipment PlayerStartEquipment;
        public PlayerReplayData GetPlayerData()
        {
            PlayerReplayData data = new PlayerReplayData();
            data.name = userProperties.userName;
            data.id = userProperties.id;
            data.weapon = weaponindex;
            data.area = areaindex;

            PlayerStartEquipment.SetEquipment(weaponindex);
            return data;
        }
        // ===========================================================================

        /// <summary>
        /// 훈련생 버튼을 클릭하면 훈련생 화면을 교관의 메인 뷰 화면으로 출력
        /// </summary>
        /// <returns>훈련생 화면 텍스쳐</returns>
        public Texture OnClick()
        {
            UTILS.Log("실행");
            return traineeView.texture;
        }
    }
}


