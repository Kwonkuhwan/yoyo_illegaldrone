using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KKH.UI
{
    public class InputFieldManager : MonoBehaviour /*, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler */
    {
        [SerializeField] private TMP_InputField inputField;
        private bool isFocused = false;
        [HideInInspector] public bool isErrored = false;

        [Header("InputField 이미지")]
        // [KKH][추가][2024.09.05] ID,PW InputField Default Image
        [SerializeField] private Sprite image_Default;
        // [KKH][추가][2024.09.05] ID,PW InputField Error Image
        [SerializeField] private Sprite image_Focus;
        // [KKH][추가][2024.09.05] ID,PW InputField Error Image
        [SerializeField] private Sprite image_Error;

        [Header("종속되는 오브젝트")]
        [SerializeField] private List<GameObject> dependent_Objects;

        [Header("종속되는 이미지")]
        [SerializeField] private List<Sprite> dependent_Image_Defaults;
        [SerializeField] private List<Sprite> dependent_Image_Focuss;
        [SerializeField] private List<Sprite> dependent_Image_Errors;

        [Header("Pin 판넬 오브젝트")]
        public GameObject pinImagePrefab;

        [Header("전체 선택 막기")]
        public bool isAllSelect = true;

        [Header("종속되는 Text")]
        [SerializeField] private bool isTextColor = false;
        [SerializeField] private TMP_Text dependent_Text;
        [SerializeField] private Color dependent_Text_Color_Default = Color.white;
        [SerializeField] private Color dependent_Text_Color_Focus = Color.black;
        [SerializeField] private Color dependent_Text_Color_Error = Color.red;

        private void Awake()
        {
            if (inputField == null)
            {
                inputField = GetComponent<TMP_InputField>();
            }

            inputField.transition = UnityEngine.UI.Selectable.Transition.None;
            inputField.onSelect.AddListener(OnInputFieldSelected);
            inputField.onDeselect.AddListener(OnInputFieldDeselected);
            inputField.onValueChanged.AddListener(OnValueChanged);

            if (!isAllSelect)
            {
                inputField.caretPosition = inputField.text.Length; // 커서를 텍스트 끝으로 이동
            }

            // 기본 이미지를 변경
            if (image_Default != null)
            {
                inputField.image.sprite = image_Default;
            }

            // 종속되는 오브젝트가 있다면
            if (dependent_Objects != null && dependent_Objects.Count > 0)
            {
                foreach (var obj in dependent_Objects)
                {
                    // 종속되는 이미지가 있다면
                    if (dependent_Image_Defaults != null && dependent_Image_Defaults.Count > 0)
                    {
                        obj.GetComponent<Image>().sprite = dependent_Image_Defaults[0];
                    }
                }
            }

            if (isTextColor)
            {
                if (dependent_Text == null) return;

                dependent_Text.color = dependent_Text_Color_Default;
            }
        }

        private void OnInputFieldSelected(string text)
        {
            isFocused = true;
            isErrored = false;

            SetFocusImage();
        }

        private void OnInputFieldDeselected(string text)
        {
            // 포커스 해제 시 기본 이미지로 복원
            isFocused = false;
            UpdateImage();
        }

        // [SMW][삭제][2024.09.06]
        /*
        public void OnPointerEnter(PointerEventData eventData)
        {
            // 마우스가 이미지 위에 있을 때 포커스 이미지로 변경
            if (!isFocused) return;
            if (image_Focus == null) return;

            SetFocusImage();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // 마우스가 이미지 밖으로 나갔을 때 기본 이미지로 변경
            UpdateImage();
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (image_Focus == null) return;

            SetFocusImage();
        }
        */

        void UpdateImage()
        {
            if (isFocused)
            {
                SetFocusImage();
            }
            else if (isErrored)
            {
                SetErrorImage();
            }
            else
            {
                SerDefaultImage();
            }
        }

        private void SetFocusImage()
        {
            if (image_Focus == null) return;

            inputField.image.sprite = image_Focus;

            if (dependent_Objects == null) return;
            for (int i = 0; i < dependent_Image_Focuss.Count; i++)
            {
                Image img = dependent_Objects[i].GetComponent<Image>();

                if (img == null || dependent_Image_Focuss == null || i >= dependent_Image_Focuss.Count) return;
                img.sprite = dependent_Image_Focuss[i];
            }

            if (isTextColor)
            {
                if (dependent_Text == null) return;

                dependent_Text.color = dependent_Text_Color_Focus;
            }
        }

        private void SetErrorImage()
        {
            if (image_Error == null) return;
            inputField.image.sprite = image_Error;

            if (dependent_Objects == null) return;
            for (int i = 0; i < dependent_Image_Errors.Count; i++)
            {
                Image img = dependent_Objects[i].GetComponent<Image>();

                if (img == null || dependent_Image_Errors == null || i >= dependent_Image_Errors.Count) return;
                img.sprite = dependent_Image_Errors[i];
            }

            if (isTextColor)
            {
                if (dependent_Text == null) return;

                dependent_Text.color = dependent_Text_Color_Error;
            }

            if (pinImagePrefab != null)
            {
                pinImagePrefab.GetComponent<InputFieldPin>().SetErrorImage();
            }
        }

        private void SerDefaultImage()
        {
            if (image_Default == null) return;
            inputField.image.sprite = image_Default;

            if (dependent_Objects == null) return;
            for (int i = 0; i < dependent_Image_Defaults.Count; i++)
            {
                Image img = dependent_Objects[i].GetComponent<Image>();

                if (img == null || dependent_Image_Defaults == null || i >= dependent_Image_Defaults.Count) return;
                img.sprite = dependent_Image_Defaults[i];
            }

            if (isTextColor)
            {
                if (dependent_Text == null) return;

                dependent_Text.color = dependent_Text_Color_Default;
            }
        }

        // [SMW][변경][2024.09.06]
        public void SetError(bool isError = true)
        {
            isErrored = isError;
            UpdateImage();
        }

        public void SetPinObject(bool isShow)
        {
            if (pinImagePrefab != null)
            {
                pinImagePrefab.SetActive(isShow);
            }
        }

        private void OnValueChanged(string input)
        {
            if(inputField.contentType == TMP_InputField.ContentType.Pin)
            {
                if (input.Length >= 0)
                {
                    if (!pinImagePrefab.activeInHierarchy) SetPinObject(true);
                    pinImagePrefab.GetComponent<InputFieldPin>().UpdatePin();
                }
            }
            else
            {
                if (pinImagePrefab != null)
                {
                    if (pinImagePrefab.activeInHierarchy) SetPinObject(false);
                }
            }
        }
    }
}