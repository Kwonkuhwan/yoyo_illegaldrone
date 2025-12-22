using KKH.MySQL;
using KKH;
using KKH.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using Illegaldrone;
using Photon.Pun;
using System.Collections.Generic;

namespace RJH.UI
{
    // [SMW][추가][2024.09.03]
    [Serializable]
    public class UserProperties
    {
        public UserGroup userGroup;
        public string id;
        public string userName;
        public Gender gender;
    }

    public class Login : MonoBehaviour
    {
        [SerializeField] private TMP_InputField iDInputfield;
        [SerializeField] private TMP_InputField passwordInputfield;

        [SerializeField] private Button loginButton;

        //[KKH][수정][2024.09.27] - ID 길이 제한 알림 사제
        //[SerializeField] private TextMeshProUGUI iDAlertText;
        [SerializeField] private TextMeshProUGUI passwordAlertText;
        [SerializeField] private TextMeshProUGUI loginAlertText;

        [SerializeField] private Image pcModeImage;
        [SerializeField] private TextMeshProUGUI pcModeText;

        //[SerializeField] private Button viewPasswordButton;

        [Space(10)]
        [Header("팝업 UI")]
        [SerializeField] GameObject Loading_Popup;
        [SerializeField] GameObject parentPopUp;
        [SerializeField] GameObject duplicateLogin_PopUp;
        [SerializeField] GameObject autoLogOut_PopUp;
        [SerializeField] GameObject resetTraining_PopUp;

        [Header("관리자/훈련생 로그인")]
        [SerializeField] private bool isInstructor = false;
        public bool IsInstructor
        {
            get
            {
                return isInstructor;
            }
            set
            {
                if (isInstructor != value)
                {
                    isInstructor = value;
                }
                else
                {
                    return;
                }

                if (IsInstructor)
                {
                    pcModeText.text = "본 PC는 <b><#FFC700>관리자용 PC</color></b>입니다.";
                    GameManager.instance.isInstructor = true;
                }
                else
                {
                    pcModeText.text = "본 PC는 <b><#F6F7F8>훈련생용 PC</color></b>입니다.";
                    GameManager.instance.isInstructor = false;
                }
            }
        }

        public void Awake()
        {
            ResetAlertText();
            //loginButton.onClick.AddListener(CheckIDAndPassword);

            //[KKH][수정][2024.09.0c] - ID, PW 길이 확인 onEndEdit으로 변경
            iDInputfield.onEndEdit.AddListener(delegate { CheckId(); });
            passwordInputfield.onEndEdit.AddListener(delegate { CheckPassword(); });

            //viewPasswordButton.onClick.AddListener(ViewPassword);

            loginButton.onClick.AddListener(CheckIDAndPassword);
        }

        private void OnGUI()
        {
//#if UNITY_EDITOR
            if (GUI.Button(new Rect(0, 0, 200, 40), "ChangeInstructor"))
            {
                IsInstructor = !IsInstructor;
            }
//#endif
        }

        private void Start()
        {
            SetPCMode();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Delete) && (Input.GetKey(KeyCode.LeftShift)|| Input.GetKey(KeyCode.RightShift)))
            {
                IsInstructor = !IsInstructor;
            }

            //[KKH][추가][2024.09.09] - 에러 UI 깜빡이는 현상 때문에 추가
            if (string.IsNullOrEmpty(iDInputfield.text) /*|| iDInputfield.text.Length < iDInputfield.characterLimit*/
                || string.IsNullOrEmpty(passwordInputfield.text) || passwordInputfield.text.Length < passwordInputfield.characterLimit)
            {
                if (!loginButton.GetComponent<ButtonManager>().interactable) return;
                loginButton.GetComponent<ButtonManager>().interactable = false;
            }
            else
            {
                if (loginButton.GetComponent<ButtonManager>().interactable) return;
                loginButton.GetComponent<ButtonManager>().interactable = true;
            }
        }

        //[KKH][수정][2024.09.09] - ID 길이 확인 함수화
        public bool CheckId()
        {
            #region [KKH][수정][2024.09.05] - ID 길이 확인
            //if (string.IsNullOrEmpty(iD))
            //{
            //    OnFailLogin(LoginFailReason.NOID);
            //    return;
            //}

            //[KKH][수정][2024.09.27] - ID 길이 제한 확인 삭제
            if (string.IsNullOrEmpty(iDInputfield.text)/* || iDInputfield.text.Length < iDInputfield.characterLimit*/)
            {
                OnFailLogin(LoginFailReason.NOID);
                return false;
            }
            //[KKH][수정][2024.09.27] - ID 길이 제한 알림 사제
            //else
            //{
            //    iDAlertText.gameObject.SetActive(false);
            //}
            return true;
            #endregion
        }

        //[KKH][수정][2024.09.09] - PW 길이 확인 함수화
        public bool CheckPassword()
        {
            #region [KKH][수정][2024.09.05] - password 길이 확인
            //if (string.IsNullOrEmpty(password))
            //{
            //    OnFailLogin(LoginFailReason.NOPASSWORD);
            //    return;
            //}

            if (string.IsNullOrEmpty(passwordInputfield.text) || passwordInputfield.text.Length < passwordInputfield.characterLimit)
            {
                OnFailLogin(LoginFailReason.NOPASSWORD);
                return false;
            }
            else
            {
                passwordAlertText.gameObject.SetActive(false);
            }

            return true;
            #endregion
        }

        public void CheckIDAndPassword() // 아이디와 패스워드 확인(로그인 실행)
        {
            string iD = iDInputfield.text;
            string password = passwordInputfield.text;

            // [KKH][추가][2024.08.27] - 로그인 DB 연동
            string returnMsg;
            LoginSuccess loginSuccess;
            loginSuccess = MySQLManager.Login(iD, password, out returnMsg);

            if (!GameManager.instance.isInstructor && !PhotonManager_.Inst.CheckRoom())
            {
                OnFailLogin(LoginFailReason.NONE_SERVER);
                return;
            }

            /* UserDB 필요
             */
            if (LoginSuccess.IDNone == loginSuccess)             // 계정 검증에 실패 했을때
            {
                OnFailLogin(LoginFailReason.VERIFICATION_FAIL);
                return;
            }
            else if (LoginSuccess.IDDelete == loginSuccess)      // 이용이 제한됐을때
            {
                OnFailLogin(LoginFailReason.LOGIN_RESTRICTION);
                return;
            }
            else if (LoginSuccess.PasswordError == loginSuccess) // 비밀번호 오류
            {
                // [KKH][추가][2024.08.27] - PW 오류 추가 해야함.
                /* TODO
                 * 비밀번호 오류 메시지도 표시해주세요...
                */
                OnFailLogin(LoginFailReason.WRONGPASSWORD);
                return;
            }
            else if (loginSuccess == LoginSuccess.SystemError)
            {
                UTILS.LogError("DB 연결 전에 실행됨.");
                return;
            }            
            else if (loginSuccess == LoginSuccess.Success)
            {
                OnSuccessLogin(iD);
            }
        }

        private void OnSuccessLogin(string iD) // 로그인 실행 성공
        {
            GameManager.instance.userInfo = MySQLManager.GetUserInfo(iD);

            if (GameManager.instance.isInstructor)
            {
                if (!(GameManager.instance.userInfo.userGroup == UserGroup.Admin || GameManager.instance.userInfo.userGroup == UserGroup.Instructor))
                {
                    OnFailLogin(LoginFailReason.LOGIN_INVALID);
                    return;
                }
            }
            else
            {
                if (!(GameManager.instance.userInfo.userGroup == UserGroup.Trainee))
                {
                    OnFailLogin(LoginFailReason.LOGIN_INVALID);
                    return;
                }
            }

            // 2025-04-17 RJH 현재 참여할려는 방의 모드가 단독 훈련 모드이고, 이미 훈련생이 대기중이면 더이상 참여 불가능
            if(PhotonManager_.Inst.CheckFullCapacity())
            {
                // 참여 인원 한계로 더 이상 참여 불가능
                return;
            }

            UTILS.Log($"{GameManager.instance.userInfo.userGroup} | {GameManager.instance.userInfo.id} | {GameManager.instance.userInfo.userName} | {GameManager.instance.userInfo.gender} | {GameManager.instance.userInfo.createIDDate}");
            //string userInfoStr = $"{userInfo.id} | {userInfo.userName}";
            //UTILS.Log(userInfoStr);

            // [SMW][추가][2024.09.11]
            PhotonNetwork.LocalPlayer.NickName = GameManager.instance.userInfo.userName;

            // [SMW][수정][2024.09.03]
            UserProperties userProperties = new UserProperties();
            userProperties.id = GameManager.instance.userInfo.id;
            userProperties.userName = GameManager.instance.userInfo.userName;
            userProperties.userGroup = GameManager.instance.userInfo.userGroup;
            userProperties.gender = GameManager.instance.userInfo.gender;

            var json = JsonUtility.ToJson(userProperties);
            PhotonManager_.Inst.SetPlayerCustomProperty("UserProperties", json);

            Loading_Popup.SetActive(true);
            gameObject.SetActive(false);
            /*TODO
             * 로그인 다음 화면 실행
            */
            //PhotonManager.instance.LoginSuccess((int)GameManager.instance.userInfo.userGroup);
        }

        private void OnFailLogin(LoginFailReason loginFailReason) // 로그인 실행 실패
        {
            ResetAlertText();

            switch (loginFailReason)
            {
                case LoginFailReason.NOID:
                    //[KKH][수정][2024.09.27] - ID 길이 제한 알림 사제
                    //iDAlertText.gameObject.SetActive(true);

                    // [KKH][추가][2024.09.05] Error Image Change
                    iDInputfield.GetComponent<InputFieldManager>().SetError();
                    break;
                case LoginFailReason.NOPASSWORD:
                    passwordAlertText.gameObject.SetActive(true);
                    passwordAlertText.text = "패스워드 6자리를 입력해 주세요";

                    // [KKH][추가][2024.09.05] Error Image Change
                    passwordInputfield.GetComponent<InputFieldManager>().SetError();
                    break;
                case LoginFailReason.WRONGPASSWORD:
                    #region[KKH][삭제][2024.09.09] - 에러 UI 깜빡이는 현상 때문에 삭제
                    //[KKH][삭제][2024.09.09] - 에러 UI 깜빡이는 현상 때문에 삭제
                    //passwordInputfield.text = "";
                    #endregion

                    passwordInputfield.GetComponent<InputFieldManager>().SetError();
                    passwordAlertText.gameObject.SetActive(true);
                    passwordAlertText.text = "패스워드가 일치하지 않습니다.";
                    break;
                case LoginFailReason.VERIFICATION_FAIL:
                    #region[KKH][삭제][2024.09.09] - 에러 UI 깜빡이는 현상 때문에 삭제
                    //[KKH][삭제][2024.09.09] - 에러 UI 깜빡이는 현상 때문에 삭제
                    //iDInputfield.text = "";
                    //passwordInputfield.text = "";
                    #endregion

                    iDInputfield.GetComponent<InputFieldManager>().SetError();
                    loginAlertText.gameObject.SetActive(true);
                    loginAlertText.text = "일치하는 계정이 없습니다. \n계정 정보를 확인하고 다시 로그인 해 주세요";
                    break;
                case LoginFailReason.LOGIN_RESTRICTION:
                    #region[KKH][삭제][2024.09.09] - 에러 UI 깜빡이는 현상 때문에 삭제
                    //[KKH][삭제][2024.09.09] - 에러 UI 깜빡이는 현상 때문에 삭제
                    //iDInputfield.text = "";
                    //passwordInputfield.text = "";
                    #endregion

                    loginAlertText.gameObject.SetActive(true);
                    loginAlertText.text = "이용이 제한 된 계정입니다. \n최고 관리자 또는 관리자에게 문의 바랍니다.";
                    break;

                // [KKH][추가][2024.09.09] - 잘못된 로그인
                case LoginFailReason.LOGIN_INVALID:
                    #region[KKH][삭제][2024.09.09] - 에러 UI 깜빡이는 현상 때문에 삭제
                    //[KKH][삭제][2024.09.09] - 에러 UI 깜빡이는 현상 때문에 삭제
                    //iDInputfield.text = "";
                    //passwordInputfield.text = "";
                    #endregion
                    loginAlertText.gameObject.SetActive(true);

                    // 관리자 PC 에서 훈련생이 로그인했을 경우
                    if (GameManager.instance.isInstructor)
                    {
                        loginAlertText.text = "관리자 PC에서 훈련생(이)가 로그인 했습니다.";
                    }
                    // 훈련생 PC 에서 관리자이 로그인 했을 경우
                    else
                    {
                        loginAlertText.text = "훈련생 PC에서 관리자(이)가 로그인 했습니다.";
                    }
                    break;
                case LoginFailReason.DUPLICATE_LOGIN:
                    /* TODO
                     * 먼저 로그인한 계정 로그아웃
                     */
                    break;
                case LoginFailReason.NONE_SERVER:
                    loginAlertText.gameObject.SetActive(true);
                    loginAlertText.text = "<color=#FF9C00>관리자가 서버를 활성화할때까지 대기해주세요.";
                    break;
            }
        }

        public void SelectInputField() // 아이디, 패스워드 입력 실행
        {
            ResetAlertText();
        }

        private void ResetAlertText() // 알림 텍스트 비활성화
        {
            //[KKH][수정][2024.09.27] - ID 길이 제한 알림 사제
            //iDAlertText.gameObject.SetActive(false);
            passwordAlertText.gameObject.SetActive(false);
            loginAlertText.gameObject.SetActive(false);
        }

        /// <summary>
        /// 훈련생, 관리자 구분
        /// </summary>
        private void SetInstructor()
        {
            // 자신의 IP를 가져와서 관리자, 훈련생 구분

            List<string> ips = UTILS.GetLocalIPAddress();
            foreach (string ip in ips)
            {
                if (MySQLManager.CheckInstructorPC(ip))
                {
                    IsInstructor = true;
                    break;
                }
                else
                {
                    IsInstructor = false;
                }
            }
        }

        private void SetPCMode() // PC 사용 주체 UI 설정
        {
            /*TODO
             * PC 사용 주체 확인
             */

            SetInstructor();

            //[RJH][추가][2024.09.24] 중복로그인으로 인해 로그인씬으로 돌아온 경우 체크
            if (GameManager.instance.reason == DISCONNECTEDREASON.DUPLICATELOGIN)
            {
                DuplicateLogin();
            }
        }

        private void DuplicateLogin() // 이미 로그인 중인 다른 PC에서 실행
        {
            ShowPopUp(PopUpMode.DuplicateLogin);
        }

        //private void ViewPassword()
        //{
        //    if (passwordInputfield.contentType == TMP_InputField.ContentType.Pin)
        //    {
        //        passwordInputfield.contentType = TMP_InputField.ContentType.IntegerNumber;
        //        passwordInputfield.textComponent.SetAllDirty();
        //        passwordInputfield.GetComponent<InputFieldManager>().SetPinObject(false);
        //    }
        //    else
        //    {
        //        passwordInputfield.contentType = TMP_InputField.ContentType.Pin;
        //        passwordInputfield.textComponent.SetAllDirty();
        //        passwordInputfield.GetComponent<InputFieldManager>().SetPinObject(true);
        //    }

        //    viewPasswordButton.GetComponent<ButtonManager>().SetChangeForcusAndDefaultImage();
        //}

        #region [KKH][추가][2024.09.09] - 팝업 띄우기

        /// [작성][KKH][추가][2024.09.09] - PopUp Show
        /// <summary>
        /// PopUp Show
        /// </summary>
        /// <param name="popUpMode">PopUp mode</param>
        private void ShowPopUp(PopUpMode popUpMode)
        {
            HidePopUp();

            if (popUpMode == PopUpMode.DuplicateLogin)
            {
                if (duplicateLogin_PopUp == null) return;
                duplicateLogin_PopUp.GetComponent<PopUpUI>().ShowPopUp();
            }
            else if (popUpMode == PopUpMode.AutoLogOut)
            {
                if (autoLogOut_PopUp == null) return;
                autoLogOut_PopUp.GetComponent<PopUpUI>().ShowPopUp();
            }
            else if (popUpMode == PopUpMode.ResetTraining)
            {
                if (resetTraining_PopUp == null) return;
                resetTraining_PopUp.GetComponent<PopUpUI>().ShowPopUp();
            }
        }

        /// [작성][KKH][추가][2024.09.09] - PopUp Hide
        /// <summary>
        /// PopUp Hide
        /// </summary>
        private void HidePopUp()
        {
            if (duplicateLogin_PopUp == null) return;
            duplicateLogin_PopUp.GetComponent<PopUpUI>().HidePopUp();

            if (autoLogOut_PopUp == null) return;
            autoLogOut_PopUp.GetComponent<PopUpUI>().HidePopUp();

            if (resetTraining_PopUp == null) return;
            resetTraining_PopUp.GetComponent<PopUpUI>().HidePopUp();
        }
        #endregion
    }

    public enum LoginFailReason // 로그인 실패 이유
    {
        NONE, // 로그인 성공
        NOID, // 아이디를 입력하지 않음
        NOPASSWORD, // 비밀번호를 입력하지 않음 
        WRONGPASSWORD, // 잘못된 비밀번호 입력
        VERIFICATION_FAIL, // 계정 일치 검증에 실패
        LOGIN_RESTRICTION, // 이용 제한된 계정
        LOGIN_INVALID,      // 잘못된 로그인(훈련생이 관리자 PC 로그인, 관리자이 훈련생 PC 로그인)
        DUPLICATE_LOGIN, // 중복 로그인
        NONE_SERVER
    }

    public enum PopUpMode
    {
        DuplicateLogin = 0,
        AutoLogOut,
        ResetTraining
    }
}

