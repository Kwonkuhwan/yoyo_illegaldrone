using Illegaldrone;
using Photon.Pun;
using RJH;
using RJH.UI;
using UnityEngine;

namespace KKH
{
    public class GameSettingManager : MonoBehaviour
    {

        [SerializeField] private GameObject go_ScenarioSettingCam;
        [SerializeField] private GameObject go_TrainingScenarioSetting;
        [SerializeField] private GameObject go_InstructorTrainView;

        [SerializeField] private MapObjectController mapObjectController;

        [SerializeField] private MapData mapData;

        [SerializeField] private GameObject go_playerCharacter;
                
        private void Awake()
        {
            if (!GameManager.instance.isInstructor)
            {
                go_playerCharacter.SetActive(true);

                if (go_ScenarioSettingCam != null && go_ScenarioSettingCam.activeInHierarchy)
                {
                    go_ScenarioSettingCam.SetActive(false);
                }

                if (go_TrainingScenarioSetting != null && go_TrainingScenarioSetting.activeInHierarchy)
                {
                    go_TrainingScenarioSetting.SetActive(false);
                }

                if (go_InstructorTrainView != null && go_InstructorTrainView.activeInHierarchy)
                {
                    go_InstructorTrainView.SetActive(false);
                }

                mapObjectController.gameObject.SetActive(true);

                // ActorNumber이 1부터 시작한다. 0번 배열 접근하려면 -2 하면된다.
                //mapObjectController.SetTraineeOn(PhotonNetwork.LocalPlayer.ActorNumber - 2);
                // PlayerCustomProperty의 "TraineeNumber" 받아서 쓰도록 수정
                mapObjectController.SetTraineeOn((int)PhotonNetwork.LocalPlayer.CustomProperties["TraineeNumber"]);
            }
        }
    }
}