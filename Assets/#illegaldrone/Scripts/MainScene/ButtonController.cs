using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RJH
{
    public class ButtonController : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Button button;
        private Image buttonImage;
        private TextMeshProUGUI buttonText;
        [SerializeField] private ButtonSprite buttonSprite;
        [SerializeField] private Color activeTextColor;
        [SerializeField] private Color deactiveTextColor;
        private void Awake()
        {
            button = GetComponent<Button>();
            buttonImage = GetComponent<Image>();
            buttonText = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void SetOn()
        {
            button.interactable = true;
            buttonImage.sprite = buttonSprite.defaultSprite;
            buttonText.color = activeTextColor;
        }

        public void SetOff()
        {
            button.interactable = false;
            buttonImage.sprite = buttonSprite.interactableSprite;
            buttonText.color = deactiveTextColor;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(button.interactable)
            {
                buttonImage.sprite = buttonSprite.hoverSprite;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(button.interactable)
            {
                buttonImage.sprite = buttonSprite.defaultSprite;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (button.interactable)
            {
                buttonImage.sprite = buttonSprite.pressedSprite;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (button.interactable)
            {
                buttonImage.sprite = buttonSprite.hoverSprite;
            }
        }

        public void OnDisable()
        {
            buttonImage.sprite = buttonSprite.defaultSprite;
        }
    }
}


