using KKH;
using UnityEngine;

namespace Illegaldrone
{
    public class UserSetting
    {
        public string instructorIP;
    }

    public enum DroneName : int
    {
        DJI = 0,
        DJI_Avata_2
    }

    public partial class GameManager : MonoBehaviour
    {

        //[HideInInspector]
        public bool isInstructor = false;

        public UserSetting userSetting { get; private set; }

        public string RemotePlayerObjectName = "RemotePlayer";

        public void GetUserSetting()
        {
            userSetting = UTILS.LoadJson<UserSetting>("UserSetting");
            if (userSetting == null)
            {
                SetUserSetting();
            }
        }

        public void SetUserSetting()
        {
            userSetting = new UserSetting();
            userSetting.instructorIP = $"";
            UTILS.SaveJson("UserSetting", userSetting);
        }       
    }
}
