namespace KKH.MySQL
{
    public enum LoginSuccess
    {
        Success = 0,
        PasswordError,
        IDDelete,
        IDNone,
        SystemError,
        NoneServer
    }

    #region 유저 관련
    public enum Gender : int
    {
        Man = 0,
        Female
    }

    public enum UserGroup : int
    {
        Admin = 0,
        Instructor,
        Trainee
    }

    public enum Tutorialcomplete : int
    {
        Completion = 0,
        NotCompletion
    }
    #endregion

    #region 시나리오 관련
    public enum PlayMode : int
    {
        Solo = 0,
        Multi
    }

    public enum MapType : int
    {
        NONE = -1,
        Airport = 0,
        Gwanghwamun,
        Nuclearplant
    }

    public enum Weather : int
    {
        NONE = -1,
        Sunshine = 0,
        Snow,
        Rain,
        Fog
    }

    public enum TimeZone : int
    {
        NONE = -1,
        Day = 0,
        Night
    }
    // 2025-01-08 RJH 난이도 Enum
    public enum Difficulty : int
    {
        None = -1,
        Easy = 0,
        Normal
    }

    #endregion

    #region 적드론 관련
    public enum DroneType : int
    {
        SuicideDrone = 0,   // 자폭형
        AttackDrone,                 // 소총형
        ScoutDrone,        // 정찰형
        JammerDrone,            // 전자전형
    }

    public enum FlyingType : int
    {
        Triangle = 0,               // 트라이앵글
        Diamond,                    // 다이아
        SemiCircle                  // 반원
    }
    #endregion

    #region 유저 무기
    public enum AntiDroneWeapon : int
    {
        Spoofing = 0,
        Funny,
        HiJacking,
        Net,
        MachineGun
    }
    #endregion
}