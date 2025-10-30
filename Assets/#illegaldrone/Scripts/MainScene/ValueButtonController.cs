using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RJH
{
    public class ValueButtonController : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Button button;
        [SerializeField] private Image buttonImage;
        [SerializeField] private Image buttonIconImage;
        [SerializeField] private ButtonSprite sprites;

        public bool isPresable;
        

        private void Awake()
        {
            button = GetComponent<Button>();
            buttonImage = button.image;
            
            if(button.interactable == false)
            {
                buttonImage.sprite = sprites.interactableSprite;
                buttonIconImage.sprite = sprites.interactableIconSprite;
            }
            else if(isPresable == true)
            {
                buttonImage.sprite = sprites.defaultSprite;
                buttonIconImage.sprite = sprites.defaultIconSprite;
            }
            else
            {
                buttonImage.sprite = sprites.disableSprite;
                buttonIconImage.sprite = sprites.disableIconSprite;
            }
        }

        public void Check(bool isPressable)
        {
            this.isPresable = isPressable;
            if(isPressable == false)
            {
                buttonImage.sprite = sprites.disableSprite;
                buttonIconImage.sprite = sprites.disableIconSprite;
            }
            else
            {
                buttonImage.sprite = sprites.defaultSprite;
                buttonIconImage.sprite = sprites.defaultIconSprite;
            }
        }

        public void SetInteractable(bool interactable)
        {
            button.interactable = interactable;
            if(interactable == false)
            {
                buttonImage.sprite = sprites.interactableSprite;
                buttonIconImage.sprite = sprites.interactableIconSprite;   
            }
            else
            {
                if (isPresable == true)
                {
                    buttonImage.sprite = sprites.defaultSprite;
                    buttonIconImage.sprite = sprites.defaultIconSprite;
                }
                else
                {
                    buttonImage.sprite = sprites.disableSprite;
                    buttonIconImage.sprite = sprites.disableIconSprite;
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isPresable == true && button.interactable == true)
            {
                buttonImage.sprite = sprites.hoverSprite;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isPresable == true && button.interactable == true)
            {
                buttonImage.sprite = sprites.defaultSprite;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (isPresable == true && button.interactable == true)
            {
                buttonImage.sprite = sprites.pressedSprite;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (isPresable == true && button.interactable == true)
            {
                buttonImage.sprite = sprites.hoverSprite;
            }
        }
    }
    [Serializable]
    public class ButtonSprite
    {
        public Sprite defaultSprite;
        public Sprite hoverSprite;
        public Sprite pressedSprite;
        public Sprite disableSprite;
        public Sprite interactableSprite;

        public Sprite defaultIconSprite;
        public Sprite disableIconSprite;
        public Sprite interactableIconSprite;
    }

}


