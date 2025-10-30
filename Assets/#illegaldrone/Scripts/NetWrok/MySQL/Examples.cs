using KKH.MySQL;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KKH
{
    public class Examples : MonoBehaviour
    {
        [Header("로그인")]
        public Button testLoginBtn;

        [Space(10)]

        [Header("유저")]
        public Button testNewUserAddBtn;
        public Button testUserPWModifyBtn;
        public Button testUserDeleteBtn;
        public Button testGetUsersCumulativeCountBtn;
        public Button testGetSearchUserInfoBtn;
        public Button testUserTotorialcompleteModify;
        public Button testUserSceanrioCountIncrease;

        [Space(10)]
        [Header("교관")]
        public Button testGetInstructorInfo;

        [Space(10)]

        [Header("훈련")]
        public Button testSetCalenderBtn;
        public Button testGetTrainingInfosBtn;
        public Button testGetTrainingCumulativeCountBtn;
        public Button testGetTraineeTrainInfosBtn;

        [Space(10)]

        [Header("시나리오")]
        public Button testSetNewScenarioInfoBtn;
        public Button testSetPlayDB;

        [Header("적 드론")]
        public Button testSetEnemyDroneBtn;
        public Button testGetEnemyDroneBtn;

        [Space(10)]

        [Header("로그")]
        public Button testSetLogMessageBtn;
        public Button testGetLogMessageBtn;

        private void Awake()
        {
            testLoginBtn.onClick.AddListener(() => Login());

            testNewUserAddBtn.onClick.AddListener(() => NewUserAdd());
            testUserPWModifyBtn.onClick.AddListener(() => UserPWModify());
            testUserDeleteBtn.onClick.AddListener(() => UserDelete());
            testGetUsersCumulativeCountBtn.onClick.AddListener(() => GetUsersCumulativeCount());
            testGetSearchUserInfoBtn.onClick.AddListener(() => GetSearchUserInfo());
            testUserTotorialcompleteModify.onClick.AddListener(() => UserTotorialcompleteModify());
            testUserSceanrioCountIncrease.onClick.AddListener(() => UserSceanrioCountIncrease());

            testGetInstructorInfo.onClick.AddListener(() => GetInstructorInfo());

            testSetCalenderBtn.onClick.AddListener(() => SetCalender());
            testGetTrainingInfosBtn.onClick.AddListener(() => GetTrainingInfos());
            testGetTrainingCumulativeCountBtn.onClick.AddListener(() => GetTrainingCumulativeCount());
            testGetTraineeTrainInfosBtn.onClick.AddListener(() => GetTraineeTrainInfos());

            testSetNewScenarioInfoBtn.onClick.AddListener(() => SetNewScenarioInfo());
            testSetPlayDB.onClick.AddListener(() => SetPlayDB());

            testSetEnemyDroneBtn.onClick.AddListener(() => SetEnemyDrone());
            testGetEnemyDroneBtn.onClick.AddListener(() => GetEnemyDrones());

            testSetLogMessageBtn.onClick.AddListener(() => SetLogMessage());
            testGetLogMessageBtn.onClick.AddListener(() => GetLogMessages());
        }

        private void Start()
        {
            MySQLManager.MySQLInit();
        }

        private void OnApplicationQuit()
        {
            MySQLManager.MySQLQuit();
        }

        #region 로그인
        private void Login()
        {
            string id = "123456789";
            string pw = "000000";
            string msg;

            LoginSuccess loginSuccess = MySQLManager.Login(id, pw, out msg);

            if (loginSuccess == LoginSuccess.IDNone)
            {
                UTILS.Log(msg);
            }
            else if (loginSuccess == LoginSuccess.PasswordError)
            {
                UTILS.Log(msg);
            }
            else if (loginSuccess == LoginSuccess.IDDelete)
            {
                UTILS.Log(msg);
            }
            else if (loginSuccess == LoginSuccess.SystemError)
            {
                UTILS.LogError("DB 연결 전에 실행됨.");
            }
            else if (loginSuccess == LoginSuccess.Success)
            {
                UserInfo userInfo = MySQLManager.GetUserInfo(id);

                UTILS.Log(userInfo.id);
                UTILS.Log(userInfo.userName);
            }
        }

        #endregion

        #region 유저
        private void NewUserAdd()
        {
            UserGroup userGroup = UserGroup.Admin;
            string userID = "555555555";
            string userPW = "000000";
            string userName = "아무개";
            Gender gender = Gender.Man;

            if (MySQLManager.NewUserAdd(userGroup, userID, userPW, userName, gender, "권구환"))
            {
                UTILS.Log($"{userGroup} | {userID} | {userPW} | {userName} | {gender} 유저 추가 완료.");
            }
            else
            {
                UTILS.Log($"{userGroup} | {userID} | {userPW} | {userName} | {gender} 유저 추가 실패.");
            }
        }

        private void UserPWModify()
        {
            string userID = "111111111";
            string userPW = "000000";

            UTILS.Log($"UserPWModify : {MySQLManager.UserPWModify(userID, userPW)}");
        }

        private void UserDelete()
        {
            string userID = "111111111";

            UTILS.Log($"UserPWModify : {MySQLManager.UserDelete(userID)}");
        }

        private void GetUsersCumulativeCount()
        {
            int count = 0;

            UTILS.Log($"GetUsersCumulativeCount is {MySQLManager.GetUsersCumulativeCount("랜덤",UserGroup.Admin, out count)}: {count}");
        }

        private void GetSearchUserInfo()
        {
            List<SearchUserInfo> searchUserInfos = MySQLManager.GetSearchUserInfo(0, 1, true, "", false);

            // [KKH][수정][2024.09.06] - UserGroup 추가
            // 기본값이 훈련생 검색으로 잡혀있음.
            // 교관이랑 최고 관리자 검색할때는 UserGroup 추가해서 사용해야함.
            //List<SearchUserInfo> searchUserInfos = MySQLManager.GetSearchUserInfo(0, 1, true, "", false, UserGroup.Admin);

            if (searchUserInfos == null || searchUserInfos.Count <= 0)
            {
                return;
            }

            foreach (var userInfo in searchUserInfos)
            {
                UTILS.Log($"{userInfo.idx} | {userInfo.id} | {userInfo.userName} | {userInfo.scenarioCount} | {userInfo.playSceanrioDateTime} | {userInfo.createAdminName}");
            }
        }

        private void UserTotorialcompleteModify()
        {
            string userID = "123456789";

            UTILS.Log($"UserTotorialcompleteModify is {MySQLManager.UserTotorialcompleteModify(userID)}");
        }

        private void UserSceanrioCountIncrease()
        {
            string userID = "123456789";

            UTILS.Log($"UserSceanrioCountIncrease is {MySQLManager.UserSceanrioCountIncrease(userID)}");
        }

        #region 교관
        private void GetInstructorInfo()
        {
            string userID = "123456789";

            InstructorInfo instructorInfo = MySQLManager.GetInstructorInfo(userID);

            UTILS.Log($"{instructorInfo.userName} | {instructorInfo.gender} | {instructorInfo.userGroup} | {instructorInfo.id} | {instructorInfo.totalCount} | {instructorInfo.missionResultTotalCount}");
        }
        #endregion
        #endregion

        #region 훈련
        private void SetCalender()
        {
            string searchDate = "2024-08-26";
            int count;
            string msg;
            List<ClaendarTrainInfo> claendarTrainInfos = MySQLManager.GetCalendarData(searchDate, out count, out msg);

            if (claendarTrainInfos == null || claendarTrainInfos.Count <= 0)
            {
                return;
            }

            UTILS.Log("플레이 개수: " + count);
            UTILS.Log("결과 메시지: " + msg);

            foreach (var claendarTrainInfo in claendarTrainInfos)
            {
                UTILS.Log($"{claendarTrainInfo.missionResult} | {claendarTrainInfo.playScenarioDateTime} | {claendarTrainInfo.playMode} | {claendarTrainInfo.missionMap}");
            }
        }

        private void GetTrainingInfos()
        {
            List<TrainingInfo> trainingInfos = MySQLManager.GetTrainingInfos("권구환", true, 1, 15, true);

            if (trainingInfos == null || trainingInfos.Count <= 0)
            {
                return;
            }
            foreach (TrainingInfo info in trainingInfos)
            {
                UTILS.Log($"{info.playScenarioDateTime} | {info.missionMap} | {info.weather} | {info.timeZone} | {info.playMode} | {info.playTime} | {info.teamMissionResult} | {info.btnActive} |{info.createUser}");
            }
        }

        private void GetTrainingCumulativeCount()
        {
            int count = 0;
            if (MySQLManager.GetTrainingCumulativeCount("서민우", true, out count))
            {
                UTILS.Log($"GetTrainingCumulativeCount is true : {count}");
            }
            else
            {
                UTILS.LogError($"GetTrainingCumulativeCount is false");
            }

            if (MySQLManager.GetTrainingCumulativeCount("", false, out count))
            {
                UTILS.Log($"GetTrainingCumulativeCount is true : {count}");
            }
            else
            {
                UTILS.LogError($"GetTrainingCumulativeCount is false");
            }
        }

        private void GetTraineeTrainInfos()
        {
            List<TraineeTrainInfo> traineeTrainInfos = MySQLManager.GetTraineeTrainInfos("333333333", 1, 1, true);

            if (traineeTrainInfos == null || traineeTrainInfos.Count <= 0)
            {
                return;
            }

            foreach (var traineeTrainInfo in traineeTrainInfos)
            {
                UTILS.Log($"{traineeTrainInfo.idx} | {traineeTrainInfo.ranks} | {traineeTrainInfo.playTime} | {traineeTrainInfo.admin}");
            }
        }
        #endregion

        #region 시나리오
        private void SetNewScenarioInfo()
        {
            NewScnearioInfo newScnearioInfo = new NewScnearioInfo("1244151", MySQL.PlayMode.Multi, 4, MapType.Nuclearplant, Weather.Rain, TimeZone.Day, 1, 1, 2000.0f,
                                                                    10.0f, 20.0f, 5.0f,
                                                                    1.0f, 1.0f, 0.0f,
                                                                    2.0f, 2.0f, 0.0f,
                                                                    3.0f, 3.0f, 0.0f,
                                                                    4.0f, 4.0f, 0.0f,
                                                                    60, 120, "권구환", "아무개"
                );
            if (MySQLManager.SetNewScenarioInfo(newScnearioInfo))
            {
                UTILS.Log($"SetNewScenarioInfo: true");
            }
        }
        private void SetPlayDB()
        {
            PlayDB playDB = new PlayDB($"secn_{Random.Range(0,9999999)}", $"playID_{Random.Range(0,9999999)}", "yoyo5678", "2025-04-17 11:59:00", "13:23:57", 1, "17:00:00", "0", 0, 0);
            if (MySQLManager.SetPlayDB(playDB))
            {
                UTILS.Log($"SetPlayDB: true");
            }
        }
        #endregion

        #region 적 드론
        private void SetEnemyDrone()
        {
            if (MySQLManager.SetEnemyDrone("2321412", 3, 0, 0, new Vector3(25.1f, 1.5f, 205.2f)))
            {
                UTILS.Log("SetEnemyDrone: true");
            }
        }

        private void GetEnemyDrones()
        {
            List<EnemyDroneInfo> enemyDroneInfos = MySQLManager.GetEnemyDrones("2321412");

            if (enemyDroneInfos == null || enemyDroneInfos.Count <= 0)
            {
                return;
            }
            foreach (EnemyDroneInfo info in enemyDroneInfos)
            {
                UTILS.Log($"{info.idx} | {info.scenarioID} | {info.droneType} | {info.flyingOrder} | {info.groupNumber} | {info.startPoint.x}, {info.startPoint.y}, {info.startPoint.z} | {info.offensivePower} | {info.attackerloadtime} | {info.moveSpeed}");
            }
        }
        #endregion

        #region 로그
        private void SetLogMessage()
        {
            string instructID = "123456789";
            string traineeID = "000000000";
            string msg = "테스트 메시지";

            UTILS.DBLog(instructID, traineeID, msg);
        }

        private void GetLogMessages()
        {
            string traineeID = "yoyo5678";
            int offset = 1;
            int maxCnt = 10;

            int count = 0;

            List<LogMessage> logMessages = MySQLManager.GetLogMessages(traineeID, offset, maxCnt, true, out count);

            if (logMessages == null || logMessages.Count <= 0)
            {
                return;
            }

            UTILS.Log($"{count}");

            foreach (var logMessage in logMessages)
            {
                UTILS.Log($"{logMessage.logWriteDateTime} | 훈련생[{logMessage.traineeID}]_{logMessage.logMsg}_교관[{logMessage.instructorID}]");
            }
        }
        #endregion
    }
}