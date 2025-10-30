using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SMW
{
    public class Trainee : MonoBehaviour
    {
        [Serializable]
        public struct UI
        {
            public TMP_Text name;
            public TMP_Text id;
            public TMP_Text defenseArea;
            public TMP_Text weaponType;
            public RawImage view;
            public GameObject noData;
        }
        public UI ui;

        readonly string[] Weapon = { "재밍 (Jamming)", "재밍 (Jamming)", "소총 (Rifle)" };//{ "재밍 (Jamming)", "그물망 (Nets)", "소총 (Rifle)" };
        readonly string[] Area = { "경계지대", "주방어지대", "핵심방어지대" };

        public bool isView = false;

        /// <summary>
        /// 리플레이 데이터 세팅
        /// </summary>
        /// <param name="data"></param>
        public void Set(PlayerReplayData data)
        {
            ui.name.text = data.name;
            ui.id.text = data.id;
            ui.defenseArea.text = Area[data.area];
            ui.weaponType.text = Weapon[data.weapon];
            ui.noData.SetActive(false);
        }

        /// <summary>
        /// 데이터가 없을 때 UI 세팅
        /// </summary>
        /// <param name="rt"></param>
        public void SetNoData(Texture2D rt)
        {
            ui.view.texture = rt;
            ui.noData.SetActive(true);
            isView = false;
        }

        /// <summary>
        /// 리플레이 재생시 카메라 뷰 세팅
        /// </summary>
        /// <param name="rt"></param>
        public void SetView(RenderTexture rt)
        {
            ui.view.texture = rt;
            ui.noData.SetActive(false);
            isView = true;
        }

        public void Reset()
        {
            ui.name.text = "";
            ui.id.text = "";
            ui.defenseArea.text = "";
            ui.weaponType.text = "-";
            ui.noData.SetActive(true);
            isView = false;
        }
    }
}