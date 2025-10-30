using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SMW
{
    public class DropdownFocus : MonoBehaviour
    {
        [SerializeField] Image Image_Dropdown;
        [SerializeField] Image Image_Arrow;

        [Header("Dropdown Sprite")]
        [SerializeField] Sprite Sprite_Default;
        [SerializeField] Sprite Sprite_Focus;

        [Header("Arrow Sprite")]
        [SerializeField] Sprite Sprite_up;
        [SerializeField] Sprite Sprite_down;

        private void OnEnable()
        {
            Image_Dropdown.sprite = Sprite_Focus;
            Image_Arrow.sprite = Sprite_up;
        }

        private void OnDestroy()
        {
            Image_Dropdown.sprite = Sprite_Default;
            Image_Arrow.sprite = Sprite_down;
        }
    }
}