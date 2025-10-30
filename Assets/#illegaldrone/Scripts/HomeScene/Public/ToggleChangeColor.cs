using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SMW
{
    public class ToggleChangeColor : MonoBehaviour
    {
        [SerializeField] Color Color_Active;
        [SerializeField] Color Color_NonActive;

        [SerializeField] TMP_Text _text;

        private void Start()
        {
            _text = GetComponentInChildren<TMP_Text>();
        }

        public void ChangeColor(bool isOn)
        {
            if (isOn)
            {
                _text.color = Color_Active;
            }
            else
            {
                _text.color = Color_NonActive;
            }
        }
    }
}