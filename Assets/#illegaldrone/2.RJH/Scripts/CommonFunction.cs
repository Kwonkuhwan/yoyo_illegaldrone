using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using KKH;

namespace RJH.UI
{
    public class CommonFunction : MonoBehaviour
    {
        [SerializeField] private GameObject DuplicateLoginPopup; // TODO 나중에 공통으로 사용하는 팝업으로 대체
        private void Awake()
        {
            PhotonManager_.Inst.PlayerJoinRoomAction.AddListener(DuplicationCheck);
        }

        private void OnDisable()
        {
            PhotonManager_.Inst.PlayerJoinRoomAction.RemoveListener(DuplicationCheck);
        }
        /// <summary>
        /// 룸에 참여한 플레이어의 아이디와 이름 비교, 같은 값이면 중복 로그인 처리 
        /// </summary>
        /// <param name="newPlayer">룸에 참여한 플레이어</param>
        private void DuplicationCheck(Player newPlayer)
        {
            UTILS.Log("중복 테스트 시작");

            string localUserInfo = PhotonNetwork.LocalPlayer.CustomProperties["UserProperties"].ToString();
            string newUserInfo = newPlayer.CustomProperties["UserProperties"].ToString();
            if (localUserInfo == newUserInfo)
            {
                UTILS.Log("중복입니다.");
                DuplicateLoginPopup.SetActive(true);// TODO 나중에 공통으로 사용하는 팝업으로 대체
            }
        }
    }
}


