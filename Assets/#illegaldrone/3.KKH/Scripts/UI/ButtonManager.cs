using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour, /*IPointerEnterHandler,*/ IPointerExitHandler, IPointerMoveHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Button button;

    public bool interactable
    {
        get { return button.interactable; }
        set 
        { 
            button.interactable = value;
            if (button.interactable)
            {
                SetActiveImage();
            }
            else
            {
                SetDisableImage();
            }
        }
    }

    private bool isFocused = false;
    [HideInInspector] public bool isErrored = false;

    [Header("Button 이미지")]
    // [KKH][추가][2024.09.05] Button Default Image
    [SerializeField] private Sprite image_Default;
    // [KKH][추가][2024.09.05] Button Active Image
    [SerializeField] private Sprite image_Active;
    // [KKH][추가][2024.09.05] Button Error Image
    [SerializeField] private Sprite image_Focus;
    // [KKH][추가][2024.09.05] Button Error Image
    [SerializeField] private Sprite image_Error;
    // [KKH][추가][2024.09.11] Button Down Image
    [SerializeField] private Sprite image_Down;
    // [KKH][추가][2024.09.11] Button Up Image
    [SerializeField] private Sprite image_Up;
    // [KKH][추가][2024.10.11] Button Disable Image
    [SerializeField] private Sprite image_Disable;

    [Header("종속되는 오브젝트")]
    [SerializeField] private List<GameObject> dependent_Objects;

    [Header("종속되는 이미지")]
    [SerializeField] private List<Sprite> dependent_Image_Defaults;
    [SerializeField] private List<Sprite> dependent_Image_Actives;
    [SerializeField] private List<Sprite> dependent_Image_Focuss;
    [SerializeField] private List<Sprite> dependent_Image_Errors;
    [SerializeField] private List<Sprite> dependent_Image_Downs;
    [SerializeField] private List<Sprite> dependent_Image_Ups;
    [SerializeField] private List<Sprite> dependent_Image_Disables;

    [Header("종속되는 Text")]
    [SerializeField] private bool isTextColor = false;
    [SerializeField] private TMP_Text dependent_Text;
    [SerializeField] private Color dependent_Text_Color_Default = Color.white;
    [SerializeField] private Color dependent_Text_Color_Active = Color.white;
    [SerializeField] private Color dependent_Text_Color_Focus = Color.black;
    [SerializeField] private Color dependent_Text_Color_Error = Color.red;
    [SerializeField] private Color dependent_Text_Color_Down = Color.red;
    [SerializeField] private Color dependent_Text_Color_Up = Color.red;
    [SerializeField] private Color dependent_Text_Color_Disable = Color.gray;

    private void Awake()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }

        button.transition = Selectable.Transition.None;

        // 기본 이미지를 변경
        if (image_Default != null)
        {
            button.image.sprite = image_Default;
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

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    // 마우스가 이미지 위에 있을 때 포커스 이미지로 변경
    //    SetFocusImage();
    //}

    public void OnPointerExit(PointerEventData eventData)
    {
        // 마우스가 이미지 밖으로 나갔을 때 기본 이미지로 변경
        //UpdateImage();
        if (interactable)
        {
            SetActiveImage();
        }
        else
        {
            SetDefaultImage();
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        SetFocusImage();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetDownImage();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SetUpImage();
    }

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
            SetDefaultImage();
        }
    }

    private void SetDisableImage()
    {
        if(image_Disable == null)
        {
            SetDefaultImage();
        }
        else
        {
            button.image.sprite = image_Disable;
        }

        if (dependent_Objects != null)
        {
            for (int i = 0; i < dependent_Image_Defaults.Count; i++)
            {
                Image img = dependent_Objects[i].GetComponent<Image>();

                if (img == null || dependent_Image_Disables == null || i >= dependent_Image_Disables.Count) return;
                img.sprite = dependent_Image_Disables[i];
            }
        }

        if (isTextColor)
        {
            if (dependent_Text == null) return;

            dependent_Text.color = dependent_Text_Color_Disable;
        }
    }

    private void SetDefaultImage()
    {
        //if (!interactable) return;

        if (image_Default != null)
        {
            button.image.sprite = image_Default;
        }

        if (dependent_Objects != null)
        {
            for (int i = 0; i < dependent_Image_Defaults.Count; i++)
            {
                Image img = dependent_Objects[i].GetComponent<Image>();

                if (img == null || dependent_Image_Defaults == null || i >= dependent_Image_Defaults.Count) return;
                img.sprite = dependent_Image_Defaults[i];
            }
        }

        if (isTextColor)
        {
            if (dependent_Text == null) return;

            dependent_Text.color = dependent_Text_Color_Default;
        }
    }

    private void SetActiveImage()
    {
        if (!interactable) return;

        if (image_Active != null)
        {
            button.image.sprite = image_Active;
        }

        if (dependent_Objects != null)
        {
            for (int i = 0; i < dependent_Image_Actives.Count; i++)
            {
                Image img = dependent_Objects[i].GetComponent<Image>();

                if (img == null || dependent_Image_Actives == null || i >= dependent_Image_Actives.Count) return;
                img.sprite = dependent_Image_Actives[i];
            }
        }

        if (isTextColor)
        {
            if (dependent_Text == null) return;

            dependent_Text.color = dependent_Text_Color_Active;
        }
    }

    private void SetFocusImage()
    {
        if (!interactable) return;

        if (image_Focus != null)
        {
            button.image.sprite = image_Focus;
        }

        if (dependent_Objects != null)
        {
            for (int i = 0; i < dependent_Image_Focuss.Count; i++)
            {
                Image img = dependent_Objects[i].GetComponent<Image>();

                if (img == null || dependent_Image_Focuss == null || i >= dependent_Image_Focuss.Count) return;
                img.sprite = dependent_Image_Focuss[i];
            }
        }

        if (isTextColor)
        {
            if (dependent_Text == null) return;

            dependent_Text.color = dependent_Text_Color_Focus;
        }
    }

    private void SetErrorImage()
    {
        if (!interactable) return;

        if (image_Error != null)
        {
            button.image.sprite = image_Error;
        }

        if (dependent_Objects != null)
        {
            for (int i = 0; i < dependent_Image_Errors.Count; i++)
            {
                Image img = dependent_Objects[i].GetComponent<Image>();

                if (img == null || dependent_Image_Errors == null || i >= dependent_Image_Errors.Count) return;
                img.sprite = dependent_Image_Errors[i];
            }
        }

        if (isTextColor)
        {
            if (dependent_Text == null) return;

            dependent_Text.color = dependent_Text_Color_Error;
        }
    }

    private void SetDownImage()
    {
        if (!interactable) return;

        if (image_Down != null)
        {
            button.image.sprite = image_Down;
        }

        if (dependent_Objects != null)
        {
            for (int i = 0; i < dependent_Image_Downs.Count; i++)
            {
                Image img = dependent_Objects[i].GetComponent<Image>();

                if (img == null || dependent_Image_Downs == null || i >= dependent_Image_Downs.Count) return;
                img.sprite = dependent_Image_Downs[i];
            }
        }

        if (isTextColor)
        {
            if (dependent_Text == null) return;

            dependent_Text.color = dependent_Text_Color_Down;
        }
    }

    private void SetUpImage()
    {
        if (!interactable) return;

        if (image_Up != null)
        {
            button.image.sprite = image_Up;
        }

        if (dependent_Objects != null)
        {
            for (int i = 0; i < dependent_Image_Ups.Count; i++)
            {
                Image img = dependent_Objects[i].GetComponent<Image>();

                if (img == null || dependent_Image_Ups == null || i >= dependent_Image_Ups.Count) return;
                img.sprite = dependent_Image_Ups[i];
            }
        }

        if (isTextColor)
        {
            if (dependent_Text == null) return;

            dependent_Text.color = dependent_Text_Color_Up;
        }
    }

    public void SetError()
    {
        isErrored = true;

        SetErrorImage();
    }

    public void SetChangeForcusAndDefaultImage()
    {
        Sprite temp = image_Focus;
        image_Focus = image_Default;
        image_Default = temp;
    }    
}
