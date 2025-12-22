using KKH;
using KKH.MySQL;
using KKH.UI;
using Photon.Pun;
using RJH.UI;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace SMW
{
    public class Popup_RegistTraniee : MonoBehaviour
    {
        [Header("ID")]
        [SerializeField] TMP_InputField input_ID;
        [SerializeField] GameObject ErrorMessage_ID;
        bool isSuccessID;

        [Header("PW")]
        [SerializeField] TMP_InputField input_PW;
        [SerializeField] GameObject ErrorMessage_PW;
        [SerializeField] Button Button_HideShow;
        bool isHide = true;
        bool isSuccessPW;

        [Header("Name")]
        [SerializeField] TMP_InputField input_Name;
        [SerializeField] GameObject ErrorMessage_Name;
        bool isSuccessName;

        [Header("Gender")]
        [SerializeField] SwicthButton swicthButton;

        [Header("Confirm")]
        [SerializeField] Button Button_Confirm;
        [SerializeField] TMP_Text Text_Confirm;
        [SerializeField] Sprite[] Button_OnOff;
        [SerializeField] Color Color_Confirm;

        [Header("Title")]
        [SerializeField] TMP_Text Text_Title;

        bool isTraniee;

        void Start()
        {
            input_ID.onEndEdit.AddListener(delegate { CheckId(); });
            input_PW.onEndEdit.AddListener(delegate { CheckPassword(); });
            input_Name.onEndEdit.AddListener(delegate { CheckName(); });
            input_Name.onValueChanged.AddListener((word) => input_Name.text = Regex.Replace(word, @"[^0-9a-zA-Z가-힣]", ""));

            CheckConfirm();
            Bind();
        }

        // 초기화
        private void Reset()
        {
            input_ID.text = string.Empty;
            input_PW.text = string.Empty;
            input_Name.text = string.Empty;

            isSuccessID = false;
            isSuccessPW = false;
            isSuccessName = false;

            Button_Confirm.image.sprite = Button_OnOff[1];
            Button_Confirm.enabled = false;
            Text_Confirm.color = Color_Confirm;

            input_ID.GetComponent<InputFieldManager>().SetError(false);
            input_PW.GetComponent<InputFieldManager>().SetError(false);
            input_Name.GetComponent<InputFieldManager>().SetError(false);

            ErrorMessage_ID.SetActive(false);
            ErrorMessage_PW.SetActive(false);
            ErrorMessage_Name.SetActive(false);
        }

        private void OnEnable()
        {
            Reset();
        }

        // 팝업창 열기
        public void OnClickRegistButton(bool IsTraniee)
        {
            isTraniee = IsTraniee;

            if(isTraniee == true)
            {
                Text_Title.text = "훈련생 등록";
            }
            else
            {
                Text_Title.text = "교관 등록";
            }

            gameObject.SetActive(true);
        }

        void Bind()
        {
            Button_HideShow.onClick.AddListener(() =>
            {
                if (isHide)
                {
                    isHide = false;
                    input_PW.contentType = TMP_InputField.ContentType.IntegerNumber;
                    input_PW.GetComponent<InputFieldManager>().SetPinObject(false);
                }
                else
                {
                    isHide = true;
                    input_PW.contentType = TMP_InputField.ContentType.Pin;
                    input_PW.GetComponent<InputFieldManager>().SetPinObject(true);
                }
                input_PW.ForceLabelUpdate();
            });

            Button_Confirm.onClick.AddListener(() =>
            {
                if (Send_NewUserAdd())
                {
                    if(isTraniee)
                    {
                        Alarm.instance.SetAlarm(true, $"{input_Name.text} 훈련생이 등록되었습니다.");
                    }
                    else
                    {
                        Alarm.instance.SetAlarm(true, $"{input_Name.text} 교관이 등록되었습니다.");
                    }
                }
                else
                {
                    UTILS.Log("훈련생등록 실패");
                }
                gameObject.SetActive(false);
            });
        }

        bool CheckId()
        {
            if (string.IsNullOrEmpty(input_ID.text))
            {
                ErrorMessage_ID.SetActive(true);
                isSuccessID = false;
                input_ID.GetComponent<InputFieldManager>().SetError();
            }
            else
            {
                ErrorMessage_ID.SetActive(false);
                input_PW.Select();
                isSuccessID = true;
                input_ID.GetComponent<InputFieldManager>().SetError(false);
            }

            CheckConfirm();
            return isSuccessID;
        }

        bool CheckPassword()
        {
            if (input_PW.text.Length < 6)
            {
                ErrorMessage_PW.SetActive(true);
                isSuccessPW = false;
                input_PW.GetComponent<InputFieldManager>().SetError();
            }
            else
            {
                ErrorMessage_PW.SetActive(false);
                input_Name.Select();
                isSuccessPW = true;
                input_PW.GetComponent<InputFieldManager>().SetError(false);
            }
            CheckConfirm();
            return isSuccessPW;
        }

        bool CheckName()
        {
            if (input_Name.text.Length < 2)
            {
                ErrorMessage_Name.SetActive(true);
                isSuccessName = false;
                input_Name.GetComponent<InputFieldManager>().SetError();
            }
            else
            {
                ErrorMessage_Name.SetActive(false);
                isSuccessName = true;
                input_Name.GetComponent<InputFieldManager>().SetError(false);
            }

            CheckConfirm();
            return isSuccessName;
        }

        void CheckConfirm()
        {
            if (isSuccessID && isSuccessPW && isSuccessName)
            {
                Button_Confirm.image.sprite = Button_OnOff[0];
                Button_Confirm.enabled = true;
                Text_Confirm.color = Color.white;
            }
            else
            {
                Button_Confirm.image.sprite = Button_OnOff[1];
                Button_Confirm.enabled = false;
                Text_Confirm.color = Color_Confirm;
            }
        }

        // 등록
        bool Send_NewUserAdd()
        {
            // AdminName 가져오기
            Hashtable ht = PhotonNetwork.LocalPlayer.CustomProperties;
            UserProperties _userProperties = new UserProperties();
            _userProperties = JsonUtility.FromJson<UserProperties>(ht["UserProperties"].ToString());

            KKH.MySQL.Gender gender = (KKH.MySQL.Gender)swicthButton.IsOn;

            if (_userProperties.userName != null && _userProperties.userName != string.Empty)
            {
                if(isTraniee)
                {
                    // 훈련생등록
                    return MySQLManager.NewUserAdd(UserGroup.Trainee, input_ID.text, input_PW.text, input_Name.text, gender, _userProperties.userName);
                }
                else
                {
                    // 교관등록
                    return MySQLManager.NewUserAdd(UserGroup.Instructor, input_ID.text, input_PW.text, input_Name.text, gender, _userProperties.userName);
                }
            }
            return false;
        }
    }
}