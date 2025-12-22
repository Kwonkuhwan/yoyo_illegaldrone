
using KKH.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldPin : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button viewPasswordButton;
    
    [SerializeField] private List<GameObject> pinImages = new List<GameObject>();

    // [SMW][추가][2024.09.06]
    [SerializeField] Sprite Image_Gray;
    [SerializeField] Sprite Image_Blue;
    [SerializeField] Sprite Image_Black;
    [SerializeField] Sprite Image_Red;

    private void Awake()
    {
        if (viewPasswordButton != null)
        {
            viewPasswordButton.onClick.AddListener(ViewPassword);
        }
    }

    private void Start()
    {
        // [SMW][추가][2024.09.06]
        inputField.onSelect.AddListener((str)=>
        {
            for (int i = 0; i < inputField.text.Length; i++)
            {
                pinImages[i].transform.GetComponent<Image>().sprite = Image_Blue;
            }
        });
        inputField.onDeselect.AddListener((str)=>
        {
            for (int i = 0; i < inputField.text.Length; i++)
            {
                pinImages[i].transform.GetComponent<Image>().sprite = Image_Black;
            }
        });
    }

    private void OnEnable()
    {
        UpdatePin();
    }

    //private void OnDisable()
    //{
    //    // 입력된 글자 수에 따라 이미지 생성
    //    for (int i = 0; i < pinImages.Count; i++)
    //    {
    //        pinImages[i].SetActive(false);
    //    }
    //}

    public void UpdatePin(string str = "")
    {
        for (int i = 0; i < pinImages.Count; i++)
        {
            //pinImages[i].SetActive(false);

            // [SMW][변경][2024.09.06]
            pinImages[i].transform.GetComponent<Image>().sprite = Image_Gray;
        }

        for (int i = 0; i < inputField.text.Length; i++)
        {
            //pinImages[i].SetActive(true);

            // [SMW][변경][2024.09.06]
            if (inputField.isFocused)
            {
                pinImages[i].transform.GetComponent<Image>().sprite = Image_Blue;
            }
            else
            {
                pinImages[i].transform.GetComponent<Image>().sprite = Image_Black;
            }
        }
    }

    // [KKH][추가][2024.09.27]
    public void SetErrorImage()
    {
        for (int i = 0; i < inputField.text.Length; i++)
        {
            pinImages[i].transform.GetComponent<Image>().sprite = Image_Red;
        }
    }

    private void ViewPassword()
    {
        if (inputField.contentType == TMP_InputField.ContentType.Pin)
        {
            inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
            inputField.textComponent.SetAllDirty();
            inputField.GetComponent<InputFieldManager>().SetPinObject(false);
        }
        else
        {
            inputField.contentType = TMP_InputField.ContentType.Pin;
            inputField.textComponent.SetAllDirty();
            inputField.GetComponent<InputFieldManager>().SetPinObject(true);
        }

        viewPasswordButton.GetComponent<ButtonManager>().SetChangeForcusAndDefaultImage();
    }
}
