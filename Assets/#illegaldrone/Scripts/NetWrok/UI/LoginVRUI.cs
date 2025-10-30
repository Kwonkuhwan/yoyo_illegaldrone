using Illegaldrone;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginVRUI : MonoBehaviour
{
    [Header("로그인")]
    public string str_LoginTitle;
    public Sprite sprite_Login;
    public string str_Login;

    [Header("로그아웃")]
    public string str_LogoutTitle;
    public Sprite sprite_Logout;
    public Sprite sprite_LogoutIcon;
    public string str_Logout;

    [Header("중복 로그인")]
    public string str_DuplicateLoginTitle;
    public Sprite sprite_DuplicateLogin;
    public Sprite sprite_DuplicateLoginIcon;
    public string str_DuplicateLogin;

    [Header("패스워드 변경")]
    public string str_ChangePasswordTitle;
    public Sprite sprite_ChangePassword;
    public Sprite sprite_ChangePasswordIcon;
    public string str_ChangePassword;

    [Header("UI")]
    public TMP_Text text_Title;
    public Image image_Logo;
    public Image image_Icon;
    public TMP_Text text_Coment;

    private void Start()
    {
        if(GameManager.instance.reason == DISCONNECTEDREASON.DUPLICATELOGIN)
        {
            SetDuplicateLoginUI();
        }
        else if(GameManager.instance.reason == DISCONNECTEDREASON.LOGOUT)
        {
            SetChangePassWord();
        }
        else if (GameManager.instance.reason == DISCONNECTEDREASON.PASSWORDCHANGE)
        {
            SetLogout();
        }
        else
        {
            SetLoginUI();
        }
    }

    private void OnEnable()
    {
        StartCoroutine(InitCortoutine());
    }

    private void SetLoginUI()
    {
        text_Title.text = str_LoginTitle;
        image_Logo.sprite = sprite_Login;
        image_Icon.gameObject.SetActive(false);
        text_Coment.text = str_Login;
    }

    private void SetDuplicateLoginUI()
    {
        text_Title.text = str_DuplicateLoginTitle;
        image_Logo.sprite = sprite_DuplicateLogin;
        image_Icon.gameObject.SetActive(false);
        text_Coment.text = str_DuplicateLogin;
    }

    private void SetChangePassWord()
    {
        text_Title.text = str_ChangePasswordTitle;
        image_Logo.sprite = sprite_ChangePassword;
        image_Icon.gameObject.SetActive(true);
        image_Icon.sprite = sprite_ChangePasswordIcon;
        text_Coment.text = str_ChangePassword;
    }

    private void SetLogout()
    {
        text_Title.text = str_LogoutTitle;
        image_Logo.sprite = sprite_Logout;
        image_Icon.gameObject.SetActive(true);
        image_Icon.sprite = sprite_LogoutIcon;
        text_Coment.text = str_Logout;
    }

    private IEnumerator InitCortoutine()
    {
        yield return new WaitForSeconds(3.0f);

        if (GameManager.instance.reason != DISCONNECTEDREASON.NONE)
        {
            SetLoginUI();
        }
    }
}
