using BNG;
using Illegaldrone;
using MySql.Data.MySqlClient;
using RJH;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace KKH.MySQL
{
    public static class MySQLManager
    {
        private class MySQLInfo
        {
            public string serverIP;
            public uint port;
            public string databaseName;
            public string dbID;
            public string dbPW;
        }

        private static MySQLInfo mySqlInfo;
        private static MySqlConnection sqlConn;

        public static void SetMySQLInfo()
        {
            mySqlInfo = new MySQLInfo();
            mySqlInfo.serverIP = $"127.0.0.1";
            mySqlInfo.port = 3306;
            mySqlInfo.databaseName = "lllegaldrone";
            mySqlInfo.dbID = "lllegalDrone";
            mySqlInfo.dbPW = "1234";
            UTILS.SaveJson("MySQLInfo", mySqlInfo);
        }

        private static void GetMySQLInfo()
        {
            mySqlInfo = UTILS.LoadJson<MySQLInfo>("MySQLInfo");
            if (mySqlInfo == null)
            {
                SetMySQLInfo();
            }
        }

        public static void MySQLInit()
        {
            //"lllegaldrone", "lllegalDrone", "1234"

            GetMySQLInfo();

            MySqlConnectionStringBuilder connBuilder = new MySqlConnectionStringBuilder();
//#if UNITY_EDITOR
//            connBuilder.Server = "192.168.1.14";
//#else
            connBuilder.Server = mySqlInfo.serverIP;
//#endif
            connBuilder.Database = mySqlInfo.databaseName;
            connBuilder.UserID = mySqlInfo.dbID;
            connBuilder.Password = mySqlInfo.dbPW;
            connBuilder.Pooling = false;
            connBuilder.Port = mySqlInfo.port;

            sqlConn = new MySqlConnection(connBuilder.ConnectionString);

            try
            {
                sqlConn.Open();
                UTILS.Log($"{connBuilder.Server} | {sqlConn.Database} 데이터베이스에 연결되었습니다!");
            }
            catch (MySqlException e)
            {
                UTILS.LogError($"데이터베이스에 연결 실패 : {e}");
            }
        }

        public static void MySQLQuit()
        {
            try
            {
                if (sqlConn != null)
                {
                    if (sqlConn.State != System.Data.ConnectionState.Closed)
                    {
                        sqlConn?.Close();
                    }
                    sqlConn.Dispose(); // 자원 해제
                }
            }
            catch (TypeInitializationException tie)
            {
                UTILS.LogError(tie.InnerException);
            }
            catch (MySqlException msex)
            {
                UTILS.LogError(msex);
            }
            catch (NotImplementedException nie)
            {
                UTILS.LogError(nie);
            }
            catch (Exception e)
            {
                UTILS.LogError(e);
            }
            sqlConn = null; // 연결 객체 초기화
        }

        /// <summary>
        /// DB 리턴값이 없을경우 사용
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private static bool ExecuteNonQuery(string query)
        {
            if (sqlConn == null || sqlConn.State != System.Data.ConnectionState.Open)
            {
                return false;
            }

            try
            {
                using (var cmd = new MySqlCommand(query, sqlConn))
                {
                    // OUT 파라미터를 위한 변수
                    MySqlParameter stateParam = new MySqlParameter("@return_state", MySqlDbType.Int32);
                    stateParam.Direction = System.Data.ParameterDirection.Output;
                    cmd.Parameters.Add(stateParam);

                    // 프로시저 실행
                    cmd.ExecuteNonQuery();

                    // 결과 가져오기
                    return stateParam.Value.ToString() == "1";
                }
            }
            catch
            {
                return false;
            }
        }

        private static bool ExecuteNonQuery(string query, out int resultCnt)
        {
            if (sqlConn == null || sqlConn.State != System.Data.ConnectionState.Open)
            {
                resultCnt = -1;
                return false;
            }

            try
            {
                using (var cmd = new MySqlCommand(query, sqlConn))
                {
                    // OUT 파라미터를 위한 변수
                    MySqlParameter countParam = new MySqlParameter("@return_count", MySqlDbType.Int32);
                    countParam.Direction = System.Data.ParameterDirection.Output;
                    cmd.Parameters.Add(countParam);

                    MySqlParameter stateParam = new MySqlParameter("@return_state", MySqlDbType.Int32);
                    stateParam.Direction = System.Data.ParameterDirection.Output;
                    cmd.Parameters.Add(stateParam);

                    // 프로시저 실행
                    cmd.ExecuteNonQuery();

                    // 결과 가져오기
                    resultCnt = (int)countParam.Value;
                    return stateParam.Value.ToString() == "1";
                }
            }
            catch
            {
                resultCnt = -1;
                return false;
            }
        }

        private static bool ExecuteNonQuery(string query, out double resultCnt)
        {
            if (sqlConn == null || sqlConn.State != System.Data.ConnectionState.Open)
            {
                resultCnt = -1;
                return false;
            }

            try
            {
                using (var cmd = new MySqlCommand(query, sqlConn))
                {
                    // OUT 파라미터를 위한 변수
                    MySqlParameter countParam = new MySqlParameter("@return_count", MySqlDbType.Double);
                    countParam.Direction = System.Data.ParameterDirection.Output;
                    cmd.Parameters.Add(countParam);

                    MySqlParameter stateParam = new MySqlParameter("@return_state", MySqlDbType.Int32);
                    stateParam.Direction = System.Data.ParameterDirection.Output;
                    cmd.Parameters.Add(stateParam);

                    // 프로시저 실행
                    cmd.ExecuteNonQuery();

                    // 결과 가져오기
                    resultCnt = (double)countParam.Value;
                    return stateParam.Value.ToString() == "1";
                }
            }
            catch(MySqlException mse)
            {
                UTILS.LogError(mse);
                resultCnt = -1;
                return false;
            }
        }

        /// <summary>
        /// 로그인 DB 조회
        /// </summary>
        /// <param name="userID">유저 ID</param>
        /// <param name="userPW">유저 PW</param>
        /// <param name="resultmsg">DB 리턴 메시지</param>
        /// <param name="loginSuccess">로그인 오류 메시지</param>
        public static LoginSuccess Login(string userID, string userPW, out string resultmsg)
        {
            if (sqlConn == null || sqlConn.State != System.Data.ConnectionState.Open)
            {
                resultmsg = string.Empty;
                return LoginSuccess.SystemError;
            }

            try
            {
                string query = $"CALL UserLogin('{userID}', '{userPW}', @return_msg, @return_state);";
                MySqlCommand cmd = new MySqlCommand(query, sqlConn);

                // OUT 파라미터를 위한 변수
                MySqlParameter msgParam = new MySqlParameter("@return_msg", MySqlDbType.VarChar);
                msgParam.Direction = System.Data.ParameterDirection.Output;
                cmd.Parameters.Add(msgParam);

                MySqlParameter stateParam = new MySqlParameter("@return_state", MySqlDbType.Int32);
                stateParam.Direction = System.Data.ParameterDirection.Output;
                cmd.Parameters.Add(stateParam);

                // 프로시저 실행
                cmd.ExecuteNonQuery();

                // 결과 가져오기
                resultmsg = msgParam.Value.ToString();
                return (LoginSuccess)stateParam.Value;
            }
            catch (Exception e)
            {
                UTILS.LogError($"Error: {e}");
                resultmsg = string.Empty;
                return LoginSuccess.SystemError;
            }
        }

        #region 유저 관련
        /// <summary>
        /// 유저 정보 가져오기
        /// </summary>
        /// <param name="userID">유저 ID</param>
        /// <returns></returns>
        public static UserInfo GetUserInfo(string userID)
        {
            if (sqlConn == null || sqlConn.State != System.Data.ConnectionState.Open)
            {
                return null;
            }

            try
            {
                string query = $"CALL GetUserInfo('{userID}', @return_state);";
                using (MySqlCommand cmd = new MySqlCommand(query, sqlConn))
                {
                    MySqlParameter stateParam = new MySqlParameter("@return_state", MySqlDbType.Int32);
                    stateParam.Direction = System.Data.ParameterDirection.Output;
                    cmd.Parameters.Add(stateParam);

                    UserInfo userInfo = null;

                    // 프로시저 실행
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            userInfo = new UserInfo((UserGroup)int.Parse(reader["Usergroup"].ToString()), reader["ID"].ToString(), reader["PW"].ToString(), reader["UserName"].ToString(), (Gender)int.Parse(reader["Gender"].ToString()),
                                DateTime.Parse(reader["CreateIDDate"].ToString()), bool.Parse(reader["DeleteID"].ToString()), DateTime.Parse(reader["EndAccessDate"].ToString()),
                                DateTime.Parse(reader["TotalPlayTime"].ToString()), DateTime.Parse(reader["ModifyPW"].ToString()), (Tutorialcomplete)int.Parse(reader["Totorialcomplete"].ToString()), 
                                int.Parse(reader["MissionResult_count"].ToString()), int.Parse(reader["teamMissionResult_count"].ToString()), Math.Round(double.Parse(reader["Result_percentage"].ToString()),2), 
                                reader["CreateAdminName"].ToString());
                        }
                    }

                    if (stateParam.Value.ToString() == "1")
                    {
                        return userInfo;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                UTILS.LogError($"Error: {e}");
                return null;
            }
        }

        /// <summary>
        /// 새로운 유저 추가
        /// </summary>
        /// <param name="userGroup">그룹 정보</param>
        /// <param name="userID">유저 ID</param>
        /// <param name="userPW">유저 PW</param>
        /// <param name="userName">유저 이름</param>
        /// <param name="gender">유저 성별</param>
        /// <param name="resultstate">리턴 결과</param>
        public static bool NewUserAdd(UserGroup userGroup, string userID, string userPW, string userName, Gender gender, string creatorAdminName)
        {
            try
            {
                string query = $"CALL AddUser({(int)userGroup}, '{userID}', '{userPW}','{userName}', {(int)gender}, '{creatorAdminName}', @return_state);";
                return ExecuteNonQuery(query);
            }
            catch (Exception e)
            {
                UTILS.LogError($"Error: {e}");
                return false;
            }
        }

        /// <summary>
        /// 유저 패스워드 변경
        /// </summary>
        /// <param name="userID">유저 ID</param>
        /// <param name="newPW">새로운 PW</param>
        /// <param name="resultstate">리턴 결과</param>
        public static bool UserPWModify(string userID, string newPW)
        {
            try
            {
                string query = $"CALL UserPWModify('{userID}', '{newPW}', @return_state);";
                return ExecuteNonQuery(query);
            }
            catch (Exception e)
            {
                UTILS.LogError($"Error: {e}");
                return false;
            }
        }

        /// <summary>
        /// 유저 삭제
        /// </summary>
        /// <param name="userID">유저 ID</param>
        /// <param name="resultstate">리턴 결과</param>
        public static bool UserDelete(string userID)
        {
            try
            {
                string query = $"CALL UserDelete('{userID}', @return_state);";
                return ExecuteNonQuery(query);
            }
            catch (Exception e)
            {
                UTILS.LogError($"Error: {e}");
                return false;
            }
        }

        /// <summary>
        /// 총 유저 수
        /// </summary>
        /// <param name="resultcount">리턴 유저 수</param>
        /// <param name="resultstate">리턴 결과</param>
        public static bool GetUsersCumulativeCount(string userName,UserGroup userGroup, out int resultcount)
        {
            try
            {
                string query = $"CALL GetUsersCumulativeCount('{userName}', {(int)userGroup}, @return_count, @return_state);";
                return ExecuteNonQuery(query, out resultcount);
            }
            catch (Exception e)
            {
                resultcount = -1;
                UTILS.LogError($"Error: {e}");
                return false;
            }
        }

        /// <summary>
        /// 유저 검색
        /// </summary>
        /// <param name="offsetCnt">몇번째 부터 검색할지</param>
        /// <param name="maxCnt">최대 개수</param>
        /// <param name="isAsc">정렬</param>
        /// <param name="resultstate">리턴 결과</param>
        /// <param name="searchName">검색 이름</param>
        /// <param name="isNameSearch">검색 유무</param>
        /// <returns></returns>
        public static List<SearchUserInfo> GetSearchUserInfo(int offsetCnt, int maxCnt, bool isAsc, string searchName = "", bool isNameSearch = false, UserGroup userGroup = UserGroup.Trainee)
        {
            if (sqlConn == null || sqlConn.State != System.Data.ConnectionState.Open)
            {
                return null;
            }

            try
            {
                string query = $"CALL GetSearchUserInfo({offsetCnt-1}, {maxCnt}, {isAsc}, '{searchName}', {isNameSearch}, {(int)userGroup}, @return_state);";
                using (var cmd = new MySqlCommand(query, sqlConn))
                {

                    // OUT 파라미터를 위한 변수
                    MySqlParameter stateParam = new MySqlParameter("@return_state", MySqlDbType.Int32);
                    stateParam.Direction = System.Data.ParameterDirection.Output;
                    cmd.Parameters.Add(stateParam);

                    List<SearchUserInfo> result = new List<SearchUserInfo>();

                    // 프로시저 실행
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int idx = Convert.ToInt32(reader["idx"].ToString());
                            DateTime createIDDate = DateTime.Parse(reader["CreateIDDate"].ToString());
                            string id = reader["ID"].ToString();
                            string userName = reader["UserName"].ToString();
                            int scenariocount = int.Parse(reader["Scenariocount"].ToString());
                            DateTime playScenarioDateTime = DateTime.Parse(reader["PlayScenarioDateTime"].ToString());
                            string createAdminName = reader["CreateAdminName"].ToString();

                            SearchUserInfo userInfo = new SearchUserInfo(idx, createIDDate, id, userName, scenariocount, playScenarioDateTime, createAdminName);
                            result.Add(userInfo);
                        }
                    }

                    return result;
                }
            }
            catch (Exception e)
            {
                UTILS.LogError($"Error: {e}");
                return null;
            }
        }

        /// <summary>
        /// 훈련생 정보들 가져오기
        /// </summary>
        /// <param name="offsetCnt"></param>
        /// <param name="maxCnt"></param>
        /// <param name="isAsc"></param>
        /// <param name="searchName"></param>
        /// <param name="isNameSearch"></param>
        /// <param name="userGroup"></param>
        /// <returns></returns>
        public static List<TraineeInfo> GetTraineeInfos(int offsetCnt, int maxCnt, bool isAsc, string searchName = "", bool isNameSearch = false, UserGroup userGroup = UserGroup.Trainee)
        {
            if (sqlConn == null || sqlConn.State != System.Data.ConnectionState.Open)
            {
                return null;
            }

            try
            {
                if (!isNameSearch) searchName = string.Empty;
                string query = $"CALL GetTraineeInfos({offsetCnt - 1}, {maxCnt}, {isAsc}, '{searchName}', {isNameSearch}, {(int)userGroup}, @return_state);";
                using (var cmd = new MySqlCommand(query, sqlConn))
                {
                    // OUT 파라미터를 위한 변수
                    MySqlParameter stateParam = new MySqlParameter("@return_state", MySqlDbType.Int32);
                    stateParam.Direction = System.Data.ParameterDirection.Output;
                    cmd.Parameters.Add(stateParam);

                    List<TraineeInfo> result = new List<TraineeInfo>();

                    // 프로시저 실행
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int idx = Convert.ToInt32(reader["idx"].ToString());
                            DateTime createIDDate = DateTime.Parse(reader["CreateIDDate"].ToString());
                            string id = reader["ID"].ToString();
                            string userName = reader["UserName"].ToString();
                            int scenariocount = int.Parse(reader["Scenariocount"].ToString());
                            DateTime latestPlayDate;
                            try
                            {
                                latestPlayDate = DateTime.Parse(reader["LatestPlayDate"].ToString());
                            }
                            catch
                            {
                                latestPlayDate = DateTime.Parse("1999-01-01");
                            }

                            int playMode = 0;
                            try
                            {
                                playMode = int.Parse(reader["PlayMode"].ToString());
                            }
                            catch
                            {
                                playMode = 0;
                            }

                            int playTime = 0;
                            try
                            {
                                playTime = int.Parse(reader["Playtime"].ToString());
                            }
                            catch
                            {
                                playTime = 0;
                            }

                            int limitPlaytime = 0;
                            try
                            {
                                limitPlaytime = int.Parse(reader["limitPlaytime"].ToString());
                            }
                            catch
                            {
                                limitPlaytime = 0;
                            }

                            int missionresult;
                            try
                            {
                                missionresult = int.Parse(reader["Missionresult"].ToString());
                            }
                            catch
                            {
                                missionresult = 0;
                            }

                            bool btnActive = bool.Parse(reader["btnActive"].ToString());
                            string createAdminName = reader["CreateAdminName"].ToString();

                            TraineeInfo traineeInfo = new TraineeInfo(idx, createIDDate, id, userName, scenariocount, latestPlayDate, playMode, playTime, limitPlaytime, missionresult, btnActive, createAdminName);
                            result.Add(traineeInfo);
                        }
                    }

                    return result;
                }
            }
            catch (Exception e)
            {
                UTILS.LogError($"Error: {e}");
                return null;
            }
        }

        /// <summary>
        /// 유저 튜토리얼 완료 갱신
        /// </summary>
        /// <param name="userID">유저 ID</param>
        /// <returns></returns>
        public static bool UserTotorialcompleteModify(string userID)
        {
            try
            {
                string query = $"CALL UserTotorialcompleteModify('{userID}', @return_state);";
                return ExecuteNonQuery(query);
            }
            catch (Exception e)
            {
                UTILS.LogError($"Error: {e}");
                return false;
            }
        }

        /// <summary>
        /// 유저 시나리오 카운트 증가
        /// </summary>
        /// <param name="userID">유저 ID</param>
        /// <returns></returns>
        public static bool UserSceanrioCountIncrease(string userID)
        {
            try
            {
                string query = $"CALL UserSceanrioCountIncrease('{userID}', @return_state);";
                return ExecuteNonQuery(query);
            }
            catch (Exception e)
            {
                UTILS.LogError($"Error: {e}");
                return false;
            }
        }

        /// <summary>
        /// 훈련생 시나리오 성공 확률
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="resultParcent"></param>
        /// <returns></returns>
        public static bool GetTraineeTraingParcent(string userID, out double resultParcent)
        {
            try
            {
                string query = $"CALL GetTraineeTraingParcent('{userID}', @return_count, @return_state);";
                return ExecuteNonQuery(query, out resultParcent);
            }
            catch (Exception e)
            {
                resultParcent = -1;
                UTILS.LogError($"Error: {e}");
                return false;
            }
        }


        #region 교관 정보
        public static InstructorInfo GetInstructorInfo(string userID)
        {
            try
            {
                string query = $"CALL GetInstructorInfo('{userID}', @return_state);";
                using (var cmd = new MySqlCommand(query, sqlConn))
                {
                    // OUT 파라미터를 위한 변수
                    MySqlParameter stateParam = new MySqlParameter("@return_state", MySqlDbType.Int32);
                    stateParam.Direction = System.Data.ParameterDirection.Output;
                    cmd.Parameters.Add(stateParam);

                    // 프로시저 실행
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string userName = reader["UserName"].ToString();
                            Gender gender = (Gender)(int.Parse(reader["Gender"].ToString()));
                            UserGroup userGroup = (UserGroup)(int.Parse(reader["Usergroup"].ToString()));
                            string id = reader["ID"].ToString();
                            int totalCount = int.Parse(reader["Totalcount"].ToString());
                            int missionResultTotalCount = int.Parse(reader["MissionresultTotalcount"].ToString());
                            DateTime endAccessDate = DateTime.Parse(reader["EndAccessDate"].ToString());
                            InstructorInfo userInfo = new InstructorInfo(userName, gender, userGroup, id, totalCount, missionResultTotalCount, endAccessDate);
                            return userInfo;
                        }
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                UTILS.LogError($"Error: {e}");
                return null; ;
            }
        }
        #endregion
        #endregion

        #region 훈련 정보
        /// <summary>
        /// 교관, 훈련생 홈에서 플레이 정보 조회
        /// </summary>
        /// <param name="searchDate">검색할 날짜 (yyyy-mm-dd)</param>
        /// <param name="returncount">DB 리턴 개수</param>
        /// <param name="resultmsg">DB 리턴 메시지</param>
        /// <param name="resultstate">조회 결과</param>
        /// <returns>조회한 날짜의 훈련 정보들</returns>
        public static List<ClaendarTrainInfo> GetCalendarData(string searchDate, out int returncount, out string resultmsg)
        {
            if (sqlConn == null || sqlConn.State != System.Data.ConnectionState.Open)
            {
                resultmsg = string.Empty;
                returncount = -1;
                return null;
            }

            try
            {
                string query = $"CALL GetCalendarData('{searchDate}', @return_count, @return_msg, @return_state);";
                using (var cmd = new MySqlCommand(query, sqlConn))
                {

                    // OUT 파라미터를 위한 변수
                    MySqlParameter countParam = new MySqlParameter("@return_count", MySqlDbType.Int32);
                    countParam.Direction = System.Data.ParameterDirection.Output;
                    cmd.Parameters.Add(countParam);

                    MySqlParameter msgParam = new MySqlParameter("@return_msg", MySqlDbType.VarChar);
                    msgParam.Direction = System.Data.ParameterDirection.Output;
                    cmd.Parameters.Add(msgParam);

                    MySqlParameter stateParam = new MySqlParameter("@return_state", MySqlDbType.Int32);
                    stateParam.Direction = System.Data.ParameterDirection.Output;
                    cmd.Parameters.Add(stateParam);

                    List<ClaendarTrainInfo> result = new List<ClaendarTrainInfo>();

                    // 프로시저 실행
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ClaendarTrainInfo claendarTrainInfo = new ClaendarTrainInfo
                            (
                                int.Parse(reader["Missionresult"].ToString()), DateTime.Parse(reader["PlayScenarioTime"].ToString()), int.Parse(reader["PlayMode"].ToString()), int.Parse(reader["MissionMap"].ToString()), int.Parse(reader["PlayingNumber"].ToString())
                            );

                            result.Add(claendarTrainInfo);
                        }
                    }

                    // 결과 가져오기
                    returncount = (int)countParam.Value;
                    resultmsg = msgParam.Value.ToString();

                    return result;
                }
            }
            catch (Exception e)
            {
                resultmsg = string.Empty;
                returncount = -1;
                UTILS.LogError($"Error: {e}");
                return null;
            }
        }

        /// <summary>
        /// 훈련 정보들 불러오기
        /// </summary>
        /// <param name="offsetCnt">페이지 번호 1번부터 시작</param>
        /// <param name="maxCnt">최대 가져오는 수</param>
        /// <param name="isAsc">오름차순, 내림차순</param>
        /// <param name="resultstate">결과 스테이터스</param>
        /// <returns></returns>
        public static List<TrainingInfo> GetTrainingInfos(string creator, bool isSearch, int offsetCnt, int maxCnt, bool isAsc)
        {
            if (sqlConn == null || sqlConn.State != System.Data.ConnectionState.Open)
            {
                return null;
            }

            try
            {
                // 페이지는 1번부터이니 offsetCnt -1을 넘겨주는게 맞다..
                string query = $"CALL GetTrainingInfos({(offsetCnt - 1) * maxCnt}, {maxCnt}, {isAsc}, '{creator}', {isSearch}, @return_state);";
                using (var cmd = new MySqlCommand(query, sqlConn))
                {

                    // OUT 파라미터를 위한 변수
                    MySqlParameter stateParam = new MySqlParameter("@return_state", MySqlDbType.Int32);
                    stateParam.Direction = System.Data.ParameterDirection.Output;
                    cmd.Parameters.Add(stateParam);

                    List<TrainingInfo> result = new List<TrainingInfo>();

                    // 프로시저 실행
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TrainingInfo trainingInfo = new TrainingInfo(
                                DateTime.Parse(reader["PlayScenarioDateTime"].ToString()), int.Parse(reader["MissionMap"].ToString()), int.Parse(reader["Weather"].ToString()), int.Parse(reader["TimeZone"].ToString()),
                                int.Parse(reader["PlayMode"].ToString()), DateTime.Parse(reader["PlayScenarioTime"].ToString()), int.Parse(reader["TeamMissionResult"].ToString()), bool.Parse(reader["btnActive"].ToString()), reader["CreateUser"].ToString()
                                );

                            result.Add(trainingInfo);
                        }
                    }

                    return result;
                }
            }
            catch (Exception e)
            {
                UTILS.LogError($"Error: {e}");
                return null;
            }
        }

        /// <summary>
        /// 훈련생 훈련 정보 검색
        /// </summary>
        /// <param name="userID">유저 ID</param>
        /// <param name="offsetCnt">몇번째 부터 검색할지</param>
        /// <param name="maxCnt">최대 개수</param>
        /// <param name="isAsc">정렬</param>
        /// <param name="resultstate">리턴 결과</param>
        /// <returns></returns>
        public static List<TraineeTrainInfo> GetTraineeTrainInfos(string userID, int offsetCnt, int maxCnt, bool isAsc)
        {
            if (sqlConn == null || sqlConn.State != System.Data.ConnectionState.Open)
            {
                return null;
            }

            try
            {
                // 페이지는 1번부터이니 offsetCnt -1을 넘겨주는게 맞다..
                string query = $"CALL GetTraineeTrainInfos({(offsetCnt - 1) * maxCnt}, {maxCnt}, {isAsc}, '{userID}', @return_state);";
                using (var cmd = new MySqlCommand(query, sqlConn))
                {
                    // OUT 파라미터를 위한 변수
                    MySqlParameter stateParam = new MySqlParameter("@return_state", MySqlDbType.Int32);
                    stateParam.Direction = System.Data.ParameterDirection.Output;
                    cmd.Parameters.Add(stateParam);

                    List<TraineeTrainInfo> result = new List<TraineeTrainInfo>();

                    // 프로시저 실행
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TraineeTrainInfo trainingInfo = new TraineeTrainInfo(
                                int.Parse(reader["idx"].ToString()), DateTime.Parse(reader["PlayScenarioDateTime"].ToString()),
                                int.Parse(reader["ranks"].ToString()), int.Parse(reader["MissionMap"].ToString()),
                                int.Parse(reader["Weather"].ToString()), int.Parse(reader["TimeZone"].ToString()),
                                int.Parse(reader["PlayMode"].ToString()), DateTime.Parse(reader["PlayScenarioTime"].ToString()),
                                int.Parse(reader["limitPlaytime"].ToString()), int.Parse(reader["Missionresult"].ToString()), bool.Parse(reader["btnActive"].ToString()),
                                reader["Admin"].ToString()
                                );
                                

                            result.Add(trainingInfo);
                        }
                    }

                    return result;
                }
            }
            catch (Exception e)
            {
                UTILS.LogError($"Error: {e}");
                return null;
            }
        }

        /// <summary>
        /// 누적 훈련 수
        /// </summary>
        /// <param name="returncount">리턴 누적 훈련 수</param>
        /// <param name="returnstate">리턴 성공 유무</param>
        /// <returns></returns>
        public static bool GetTrainingCumulativeCount(string creator, bool isSearch, out int resultcount)
        {
            try
            {
                string query = $"CALL GetTrainingCumulativeCount('{creator}', {isSearch}, @return_count, @return_state);";
                return ExecuteNonQuery(query, out resultcount);
            }
            catch (Exception e)
            {
                resultcount = -1;
                UTILS.LogError($"Error: {e}");
                return false;
            }
        }
        #endregion

        #region 시나리오 정보
        /// <summary>
        /// 새로운 시나리오 추가
        /// </summary>
        /// <param name="scenarioID">시나리오 ID</param>
        /// <param name="playMode">플레이 모드(0: 개인, 1: 그룹)</param>
        /// <param name="playingNumber">플레이 인원 (0: 1명, 1: 2명, 2: 3명, 3: 4명)</param>
        /// <param name="missionMap">훈련 배경(0: 공항, 1: 광화문, 2: 원전)</param>
        /// <param name="weather">날씨(0: 맑은, 1: 강설, 2: 강우, 3: 안개)</param>
        /// <param name="timeZone">시간대(0: 주간, 1: 야간)</param>
        /// <param name="defenseAreaType">방어 지역(0: 1지대, 1: 2지대, 2: 3지대)</param>
        /// <param name="defenseArea">방어 경계 면적</param>
        /// <param name="defensObjectHP">방어 타겟 HP</param>
        /// <param name="defenseobjectlocationpointX">방어 타켓 위치 X</param>
        /// <param name="defenseobjectlocationpointY">방어 타켓 위치 Y</param>
        /// <param name="defenseobjectlocationpointZ">방어 타켓 위치 Z</param>
        /// <param name="firstPlayerLocationPointX">1번 훈련생 위치 X</param>
        /// <param name="firstPlayerLocationPointY">1번 훈련생 위치 Y</param>
        /// <param name="firstPlayerLocationPointZ">1번 훈련생 위치 Z</param>
        /// <param name="secondPlayerLocationPointX">2번 훈련생 위치 X</param>
        /// <param name="secondPlayerLocationPointY">2번 훈련생 위치 Y</param>
        /// <param name="secondPlayerLocationPointZ">2번 훈련생 위치 Z</param>
        /// <param name="thirdPlayerLocationPointX">3번 훈련생 위치 X</param>
        /// <param name="thirdPlayerLocationPointY">3번 훈련생 위치 Y</param>
        /// <param name="thirdPlayerLocationPointZ">3번 훈련생 위치 Z</param>
        /// <param name="fourthPlayerLocationPointX">4번 훈련생 위치 X</param>
        /// <param name="fourthPlayerLocationPointY">4번 훈련생 위치 Y</param>
        /// <param name="fourthPlayerLocationPointZ">4번 훈련생 위치 Z</param>
        /// <param name="playTime">훈련 진행 시간</param>
        /// <param name="limitPlayTime">훈련 제한 시간</param>
        /// <param name="createUser">생성 교관 ID</param>
        /// <param name="endScenarioPlayID">마지막 훈련생 ID</param>
        /// <param name="resultstate">리턴 결과</param>
        public static bool SetNewScenarioInfo(string scenarioID, PlayMode playMode, int playingNumber, MapType missionMap, Weather weather, TimeZone timeZone, int defenseAreaType, int defenseArea, float defensObjectHP,
                                              float defenseobjectlocationpointX, float defenseobjectlocationpointY, float defenseobjectlocationpointZ,
                                              float firstPlayerLocationPointX, float firstPlayerLocationPointY, float firstPlayerLocationPointZ,
                                              float secondPlayerLocationPointX, float secondPlayerLocationPointY, float secondPlayerLocationPointZ,
                                              float thirdPlayerLocationPointX, float thirdPlayerLocationPointY, float thirdPlayerLocationPointZ,
                                              float fourthPlayerLocationPointX, float fourthPlayerLocationPointY, float fourthPlayerLocationPointZ,
                                              int playTime, int limitPlayTime, string createUser, string endScenarioPlayID)
        {
            NewScnearioInfo newScnearioInfo = new NewScnearioInfo(scenarioID, playMode, playingNumber, missionMap, weather, timeZone, defenseAreaType, defenseArea, defensObjectHP,
                                                                defenseobjectlocationpointX, defenseobjectlocationpointY, defenseobjectlocationpointZ,
                                                                firstPlayerLocationPointX, firstPlayerLocationPointY, firstPlayerLocationPointZ,
                                                                secondPlayerLocationPointX, secondPlayerLocationPointY, secondPlayerLocationPointZ,
                                                                thirdPlayerLocationPointX, thirdPlayerLocationPointY, thirdPlayerLocationPointZ,
                                                                fourthPlayerLocationPointX, fourthPlayerLocationPointY, fourthPlayerLocationPointZ,
                                                                playTime, limitPlayTime, createUser, endScenarioPlayID);

            return SetNewScenarioInfo(newScnearioInfo);
        }

        public static bool SetNewScenarioInfo(NewScnearioInfo newScnearioInfo)
        {
            try
            {
                string query = @$"CALL SetNewScenario('{newScnearioInfo.scenarioID}', {(int)newScnearioInfo.playMode}, {newScnearioInfo.playingNumber}, {(int)newScnearioInfo.missionMap}, 
                                {(int)newScnearioInfo.weather}, {(int)newScnearioInfo.timeZone}, {newScnearioInfo.defenseAreaType},{newScnearioInfo.defenseArea}, {newScnearioInfo.defensObjectHP}, 
                                {newScnearioInfo.defenseobjectlocationPoint.x}, {newScnearioInfo.defenseobjectlocationPoint.y}, {newScnearioInfo.defenseobjectlocationPoint.z},
                                {newScnearioInfo.firstPlayerLocationPoint.x}, {newScnearioInfo.firstPlayerLocationPoint.y}, {newScnearioInfo.firstPlayerLocationPoint.z},
                                {newScnearioInfo.secondPlayerLocationPoint.x}, {newScnearioInfo.secondPlayerLocationPoint.y}, {newScnearioInfo.secondPlayerLocationPoint.z},
                                {newScnearioInfo.thirdPlayerLocationPoint.x}, {newScnearioInfo.thirdPlayerLocationPoint.y}, {newScnearioInfo.thirdPlayerLocationPoint.z},
                                {newScnearioInfo.fourthPlayerLocationPoint.x}, {newScnearioInfo.fourthPlayerLocationPoint.y}, {newScnearioInfo.fourthPlayerLocationPoint.z},
                                {newScnearioInfo.playTime}, {newScnearioInfo.limitPlayTime}, '{newScnearioInfo.createUser}', '{newScnearioInfo.endScenarioPlayID}',
                                @return_state);";
                return ExecuteNonQuery(query);
            }
            catch (Exception e)
            {
                UTILS.LogError($"Error: {e}");
                return false;
            }
        }
        #endregion

        #region 드론 관련

        public static bool SetEnemyDrone(string scenarioID, int droneType, int flyingOrder, int flyingType, Vector3 startPos)
        {
            try
            {
                string query = @$"CALL SetEnemyDrone('{scenarioID}', {droneType}, {flyingOrder}, {flyingType}, {startPos.x}, {startPos.y}, {startPos.z}, @return_state);";
                return ExecuteNonQuery(query);
            }
            catch (Exception e)
            {
                UTILS.LogError($"Error: {e}");
                return false;
            }
        }


        public static List<EnemyDroneInfo> GetEnemyDrones(string scenarioID)
        {
            if (sqlConn == null || sqlConn.State != System.Data.ConnectionState.Open)
            {
                return null;
            }

            try
            {
                string query = $"CALL GetEnemyDrones('{scenarioID}', @return_state);";
                using (var cmd = new MySqlCommand(query, sqlConn))
                {
                    // OUT 파라미터를 위한 변수
                    MySqlParameter stateParam = new MySqlParameter("@return_state", MySqlDbType.Int32);
                    stateParam.Direction = System.Data.ParameterDirection.Output;
                    cmd.Parameters.Add(stateParam);

                    List<EnemyDroneInfo> result = new List<EnemyDroneInfo>();

                    // 프로시저 실행
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            EnemyDroneInfo trainingInfo = new EnemyDroneInfo(
                                int.Parse(reader["idx"].ToString()), reader["ScenarioID"].ToString(), (DroneType)int.Parse(reader["DroneType"].ToString()),
                                int.Parse(reader["FlyingOrder"].ToString()), (FlyingType)int.Parse(reader["FlyingType"].ToString()), int.Parse(reader["GroupNumber"].ToString()),
                                float.Parse(reader["StartpointX"].ToString()), float.Parse(reader["StartpointY"].ToString()), float.Parse(reader["StartpointZ"].ToString()),
                                float.Parse(reader["OffensivePower"].ToString()), float.Parse(reader["Attackerloadtime"].ToString()), float.Parse(reader["MoveSpeed"].ToString())
                                );

                            result.Add(trainingInfo);
                        }
                    }

                    return result;
                }
            }
            catch (Exception e)
            {
                UTILS.LogError($"Error: {e}");
                return null;
            }
        }

        public static bool SetPlayDB(PlayDB playDB)
        {
            try
            {
                string query = @$"CALL SetPlayDB('{playDB.scenarioID}', '{playDB.playID}', '{playDB.userID}', '{playDB.playScenarioDateTime}', '{playDB.playTime}', {playDB.userCount}, '{playDB.scenarioPlayTime}', '{playDB.killCount}', {playDB.missionResult}, {playDB.teamMissionResult},  @return_state);";
                return ExecuteNonQuery(query);
            }
            catch (Exception e)
            {
                UTILS.LogError($"Error: {e}");
                return false;
            }
        }
        #endregion

        #region 로그 정보
        /// <summary>
        /// 로그 쓰기
        /// </summary>
        /// <param name="instructorID">교관 ID</param>
        /// <param name="traineeID">훈련생 ID</param>
        /// <param name="msg">로그 내용</param>
        /// <returns></returns>
        public static bool SetLogMessage(string instructorID, string traineeID, string msg)
        {
            try
            {
                string query = $"CALL GetLogMessages('{instructorID}', '{traineeID}', '{msg}', @return_state);";
                return ExecuteNonQuery(query);
            }
            catch (Exception e)
            {
                UTILS.LogError($"Error: {e}");
                return false;
            }
        }

        /// <summary>
        /// 로그 가져오기
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="offsetCnt"></param>
        /// <param name="maxCnt"></param>
        /// <param name="isAsc"></param>
        /// <param name="resultstate"></param>
        /// <returns></returns>
        public static List<LogMessage> GetLogMessages(string userID, int offsetCnt, int maxCnt, bool isAsc, out int count)
        {
            if (sqlConn == null || sqlConn.State != System.Data.ConnectionState.Open)
            {
                count = 0;
                return null;
            }

            try
            {
                // 페이지는 1번부터이니 offsetCnt -1을 넘겨주는게 맞다..
                string query = $"CALL GetLogMessages({(offsetCnt - 1) * maxCnt}, {maxCnt}, {isAsc}, '{userID}', @return_count, @return_state);";
                using (var cmd = new MySqlCommand(query, sqlConn))
                {

                    // OUT 파라미터를 위한 변수
                    MySqlParameter stateParam = new MySqlParameter("@return_state", MySqlDbType.Int32);
                    stateParam.Direction = System.Data.ParameterDirection.Output;
                    cmd.Parameters.Add(stateParam);

                    MySqlParameter countParam = new MySqlParameter("@return_count", MySqlDbType.Int32);
                    countParam.Direction = System.Data.ParameterDirection.Output;
                    cmd.Parameters.Add(countParam);

                    List<LogMessage> result = new List<LogMessage>();

                    // 프로시저 실행
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            LogMessage trainingInfo = new LogMessage
                            (
                                reader["InstructorID"].ToString(), reader["TraineeID"].ToString(), reader["LogMessage"].ToString(), DateTime.Parse(reader["LogWriteDateTime"].ToString())
                            );

                            result.Add(trainingInfo);
                        }
                    }

                    try
                    {
                        count = int.Parse(countParam.Value.ToString());
                    }
                    catch
                    {
                        count = 0;
                    }

                    // 결과 가져오기
                    if (stateParam.Value.ToString() == "1")
                    {
                        return result;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                UTILS.LogError($"Error: {e}");
                count = 0;
                return null;
            }
        }
        #endregion

        public static bool CheckInstructorPC(string ip)
        {
            try
            {
                string query = $"CALL CheckInstructorPC('{ip}', @return_count, @return_state)";
                int cnt = 0;
                ExecuteNonQuery(query, out cnt);
                return cnt > 0;
            }
            catch (Exception e)
            {
                UTILS.LogError($"Error: {e}");
                return false;
            }
        }
    }
}