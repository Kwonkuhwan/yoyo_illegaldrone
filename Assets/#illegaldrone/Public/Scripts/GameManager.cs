using KKH;
using KKH.MySQL;
using UnityEngine;

namespace Illegaldrone
{
    public partial class GameManager : MonoBehaviour
    {
        #region Singleton
        private static GameManager _instance;
        public static GameManager instance { get { Init(); return _instance; } }

        private static void Init()
        {
            if (_instance == null)
            {
                GameManager instance = GameObject.FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    instance = go.AddComponent<GameManager>();
                }

                _instance = instance;
                DontDestroyOnLoad(_instance);
            }
        }

        private void Start()
        {
            if (transform.parent != null)
                transform.SetParent(null);

            if (_instance != null && _instance != this)
            {
                DestroyImmediate(gameObject);
                return;
            }
            else if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(_instance);
            }
        }
        #endregion

        private UserInfo _userInfo;
        public UserInfo userInfo { get => _userInfo; set => _userInfo = value; }

        private void Awake()
        {
            MySQLManager.MySQLInit();

            GetUserSetting();
        }

        private void OnApplicationQuit()
        {
            MySQLManager.MySQLQuit();
        }
    }
}