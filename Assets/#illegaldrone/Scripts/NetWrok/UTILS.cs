using Illegaldrone;
using KKH.MySQL;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Net;
using UnityEngine;

namespace KKH
{
    public static class UTILS
    {
        #region Log 관련
        public static void LogColor(object msg, Color color)
        {
#if UNITY_EDITOR
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{msg}</color>");
#endif
        }

        public static void Log(object msg)
        {
#if UNITY_EDITOR
            LogColor(msg, Color.green);
#endif
        }

        public static bool DBLog(string instructID, string traineeID, string msg)
        {
            LogColor($"훈련생[{traineeID}]_{msg}_교관[{instructID}] 로그 등록", Color.green);
            return MySQLManager.SetLogMessage(instructID, traineeID, msg);
        }

        public static void LogWarning(object msg)
        {
#if UNITY_EDITOR
            //Debug.LogWarning($"{msg}");
            Debug.LogWarning($"<color=#{ColorUtility.ToHtmlStringRGB(Color.yellow)}>{msg}</color>");
#endif
        }

        public static void LogError(object msg)
        {
#if UNITY_EDITOR
            //Debug.LogError($"{msg}");
            Debug.LogError($"<color=#{ColorUtility.ToHtmlStringRGB(Color.red)}>{msg}</color>");
#endif
        }
        #endregion

        #region Json 관련
        public static T LoadJson<T>(string jsonName)
        {
            string dirpath = Path.Combine(Application.dataPath, "JsonData");
            if (!Directory.Exists(dirpath))
            {
                Directory.CreateDirectory(dirpath);
            }

            if (!File.Exists(Path.Combine(dirpath, $"{jsonName}.json")))
            {
                return default;
            }

            if (File.Exists(Path.Combine(dirpath, $"{jsonName}.json")))
            {
                string json = File.ReadAllText(Path.Combine(dirpath, $"{jsonName}.json"));
                return JsonUtility.FromJson<T>(json);
            }

            return default;
        }

        public static void SaveJson(string jsonName, object obj)
        {
            string json = JsonUtility.ToJson(obj);

            string dirpath = Path.Combine(Application.dataPath, "JsonData");
            if (!Directory.Exists(dirpath))
            {
                Directory.CreateDirectory(dirpath);
            }

            File.WriteAllText(Path.Combine(dirpath, $"{jsonName}.json"), json);
        }
        #endregion

        /// <summary>
        /// 자신의 IP 가져오기
        /// </summary>
        /// <returns></returns>
        public static List<string> GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            List<string> ips = new List<string>();
            foreach (var ip in host.AddressList)
            {
                // IPv4 주소만 반환
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ips.Add(ip.ToString());
                }
            }            

            if (ips.Count > 0)
            {
                return ips;
            }
            else
            {
                UTILS.LogError("IPv4 주소를 찾을 수 없습니다.");
                return null;
            }
        }
    }
}