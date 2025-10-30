using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KKH {
    public class TraineeStartCountPopup : MonoBehaviour
    {
        [SerializeField] private Sprite[] sprites_Number;
        [SerializeField] private Image image_Number;

        private void OnEnable()
        {
            StartCoroutine(CountNumber());
        }

        private IEnumerator CountNumber()
        {
            for (int i = 0; i < sprites_Number.Length; i++)
            {
                image_Number.sprite = sprites_Number[i];
                image_Number.SetNativeSize();
                yield return new WaitForSeconds(1.0f);
            }
            UIManager.Inst.AllPopupHide();
            gameObject.transform.gameObject.SetActive(false);
        }
    }
}
