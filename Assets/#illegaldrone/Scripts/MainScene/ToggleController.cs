using KKH;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RJH
{
    public class ToggleController : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Toggle toggle;

        [SerializeField] private ToggleImage defaultSprites;
        [SerializeField] private ToggleImage activeSprites;
        [SerializeField] private ToggleImage disableSprites;
        [Space(10)]
        [SerializeField] private Image boxImage;
        [SerializeField] private Image[] markerImages;


        private ToggleImage currentSprites;

        private void Awake()
        {
            toggle = GetComponent<Toggle>();
            if (toggle.interactable == false)
            {
                currentSprites = disableSprites;

            }
            else if (toggle.isOn)
            {
                currentSprites = activeSprites;
            }
            else
            {
                currentSprites = defaultSprites;
            }

            if(currentSprites.defaultSprite != null) 
                boxImage.sprite = currentSprites.defaultSprite;

            for (int i = 0; i < markerImages.Length; i++)
            {
                if (currentSprites.markerSprite[i] != null)
                    markerImages[i].sprite = currentSprites.markerSprite[i];
            }

            UTILS.Log("작동함");
            toggle.onValueChanged.AddListener(OnSelected);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (toggle.interactable)
                boxImage.sprite = currentSprites.hoverSprite;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (toggle.interactable)
                boxImage.sprite = currentSprites.defaultSprite;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (toggle.interactable)
                boxImage.sprite = currentSprites.pressedSprite;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (toggle.interactable)
                boxImage.sprite = currentSprites.defaultSprite;
        }

        public void OnSelected(bool isOn)
        {
            if (isOn)
            {
                currentSprites = activeSprites;
            }
            else
            {
                currentSprites = defaultSprites;
            }
            boxImage.sprite = currentSprites.defaultSprite;
            for (int i = 0; i < markerImages.Length; i++)
            {
                if (currentSprites.markerSprite[i] != null)
                    markerImages[i].sprite = currentSprites.markerSprite[i];
            }
            UTILS.Log($"Selected {isOn}");
        }

        public void SetInteractable(bool onInteractable)
        {
            toggle.interactable = onInteractable;
            if (toggle.interactable)
            {
                if (toggle.isOn)
                {
                    currentSprites = activeSprites;
                }
                else
                {
                    currentSprites = defaultSprites;
                }
                boxImage.sprite = currentSprites.defaultSprite;
                for (int i = 0; i < markerImages.Length; i++)
                {
                    if (currentSprites.markerSprite[i] != null)
                        markerImages[i].sprite = currentSprites.markerSprite[i];
                }
            }
            else
            {
                boxImage.sprite = disableSprites.defaultSprite;
                for (int i = 0; i < markerImages.Length; i++)
                {
                    if (disableSprites.markerSprite[i] != null)
                        markerImages[i].sprite = disableSprites.markerSprite[i];
                }
            }

        }
    }

    [Serializable]
    public class ToggleImage
    {
        public Sprite defaultSprite;
        public Sprite hoverSprite;
        public Sprite pressedSprite;
        public Sprite[] markerSprite;
    }

}



