using Illegaldrone;
using TMPro;
using UnityEngine;

namespace KKH
{
    public class LoadingPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text text_Loading;

        [SerializeField] private string[] str_Loading;
        [SerializeField] private int nChangeIdx = 0;
        [Range(0.0f, 2.0f)][SerializeField] private float fChangeTextTime = 0.0f;
        [SerializeField] private float fChangeTimer = 0.0f;

        [Range(0.0f, 5.0f)][SerializeField] private float fMin;
        [Range(5.0f, 10.0f)][SerializeField] private float fMax;
        [SerializeField] private float fMaxCoolTime = 0.0f;
        [SerializeField] private float fCoolTime = 0.0f;
        [SerializeField] private bool isLogin = false;

        private void Start()
        {
            nChangeIdx = 0;
            fChangeTimer = 0.0f;
            fCoolTime = 0.0f;
            fMaxCoolTime = Random.Range(fMin, fMax);
            text_Loading.text = str_Loading[nChangeIdx];

            isLogin = false;
        }

        void Update()
        {
            fCoolTime += Time.deltaTime;
            fChangeTimer += Time.deltaTime;
            if (fCoolTime < fMaxCoolTime)
            {
                if (fChangeTimer >= fChangeTextTime)
                {                  
                    fChangeTimer = 0.0f;
                    nChangeIdx++;

                    if (str_Loading.Length - 1 < nChangeIdx)
                    {
                        nChangeIdx = 0;
                    }

                    text_Loading.text = str_Loading[nChangeIdx];

                }
            }
            else if(fCoolTime >= fMaxCoolTime && !isLogin)
            {
                isLogin = true;
                PhotonManager_.Inst.LoginSuccess((int)GameManager.instance.userInfo.userGroup);
            }
        }
    }
}
