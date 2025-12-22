using Illegaldrone;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KKH.HOME.Trainee
{
    public class UserInfo_Image : MonoBehaviour
    {
        [SerializeField] private TMP_Text name_Text;
        [SerializeField] private Image gender_Image;

        public List<Sprite> gender_Sprites = new List<Sprite>();

        private void Awake()
        {
            if (GameManager.instance != null)
            {
                if (name_Text == null)
                {
                    name_Text = GetComponentInChildren<TMP_Text>();
                }
                name_Text.text = $"{GameManager.instance.userInfo.userName} 님";

                if(gender_Image == null)
                {
                    gender_Image = GetComponentInChildren<Image>();
                }

                if(GameManager.instance.userInfo.gender == MySQL.Gender.Man)
                {
                    gender_Image.sprite = gender_Sprites[0];
                    gender_Image.color = Color.blue;
                }
                else
                {
                    gender_Image.sprite = gender_Sprites[1];
                    gender_Image.color = Color.red;
                }

            }        
        }
    }
}