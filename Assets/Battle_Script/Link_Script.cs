using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class Link_Script : MonoBehaviour
{

    public bool New_Char_Cheat_Check = false;







    Network_Battle_Script Net_Script = null;

    public Transform Effect_OJ_Set;

    public String Server_IP = "NONE";
    public int Server_Port = 0;

    public BattleKind Play_Mode;
    public float Base_OJ_Max_HP = 0.0f;
    public float[] Base_OJ_Now_HP = new float[2];    

    public byte MapIndex = 0;

    public int Player_Diamond = 0;
    public int Player_Gold = 0;

    public byte GameOver_Kill_Max = 0;

    public ushort Game_Versoin;
    public uint User_ID;
    public String User_NickName = "";
    public String Killing_Message = "";
    public String Suicide_Message = "";
    public byte User_Team;
    public ushort User_Clan_Mark;
    public string User_Clan_Name;
    public byte[] User_Country_Mark = new byte[2];
    public float Char_Now_HP;
    public float Char_Move_Speed;
    public int Char_Index;
    public byte Char_Level;
    public int Costume_Kind_1;
    public int Costume_Kind_2;
    public int Costume_Kind_3;
    public int Main_Gun_Index;
    public byte Main_Gun_Level;
    public byte Main_Gun_Type;
    public int Sub_Gun_Index;
    public byte Sub_Gun_Level;
    public byte Sub_Gun_Type;
    public byte Start_Pos_Index;
    
    public int[] W_Index;
    public int[] W_Left_Index;
    public int W_Launchar_Index;
    public int W_FlameThrower_Index;
    public int W_Rocket_Index;
    public int W_Grenade_Index;
    public int W_ThroughShot_Index;
    public int[] I_Index;

    //유저 정보 클레스
    public class User_Data_class
    {
        public uint User_ID;
        public String User_NickName;
        public String Killing_Message;
        public byte User_Team;
        public byte Start_Pos_Index;
        public ushort User_Clan_Mark;
        public byte[] User_Country_Mark;
    }

    public Dictionary<uint, User_Data_class> User_Data = new Dictionary<uint, User_Data_class>();

    //유저가 소유한 모든 캐릭터 정보
    public Dictionary<uint, User_Units> Get_ALL_Char_Data;

    //지정된 캐릭터가 소유한 총의 모든 정보
    public Dictionary<uint, User_weapon> Get_Char_ALL_Gun_Data;

    //3개 슬롯에 셋팅된 캐릭터 정보
    public uint[] Get_UseCharacterIdx;

    //1초당 캐릭터 움직임 데이터 날리는 횟수
    public float Char_Data_BPS = 0.1f;

    //1초당 수류탄 움직임 데이터 날리는 횟수
    public float Grenade_Data_BPS = 0.1f;

    //1초당 캐릭터 스킬관련 데이터 날리는 횟수
    public float Char_Skill_Data_BPS = 1.0f;

    //===================================================================================================================================================================================================================

    void Awake()
    {        
        //-----------------------------------------------------------------------------------------------------------------

        //Application.targetFrameRate = 60;
        //Screen.sleepTimeout = SleepTimeout.NeverSleep;

        //-----------------------------------------------------------------------------------------------------------------

        Instantiate(Resources.Load("Battle_Prefab/1_Game_OJ_Folder/Network_Script") as GameObject).name = "Network_Script";
        Net_Script = GameObject.Find("Network_Script").GetComponent<Network_Battle_Script>();
        DontDestroyOnLoad(GameObject.Find("Network_Script"));

        Instantiate(Resources.Load("Battle_Prefab/1_Game_OJ_Folder/Effect_OJ_Set") as GameObject).name = "Effect_OJ_Set";
        Effect_OJ_Set = GameObject.Find("Effect_OJ_Set").GetComponent<Transform>();
        DontDestroyOnLoad(GameObject.Find("Effect_OJ_Set"));

        //-----------------------------------------------------------------------------------------------------------------

//#if UNITY_EDITOR
//        New_Char_Cheat_Check = false;
//#else
//        New_Char_Cheat_Check = false;
//#endif

    }

    public void LinkScript_Start()
    {
    }

    //===================================================================================================================================================================================================================

    //소켓 접속 확인
    public bool IsConnected()
    {
        return SendManager.Instance.IsConnectNet();
    }
    
    //퀵조인 요청 보내기(호정이가 호출해준다.)
    public void GamePlay_Send_Quick_Join(BattleKind _BattleKind)
    {
        Net_Script.Network_State = NETWORK_STATE.QUICKJOIN_START;
        Play_Mode = _BattleKind;
                
        //Play_Mode = BattleKind.WAR_OF_POSITION;
    }

    //게임 플레이에 필요한 프로토콜 받기(호정이가 호출해준다.)
    public void GamePlay_Receive_Data(byte[] Packet_Data)
    {
        Net_Script.Receive_Delegate(Packet_Data);
    }

    //게임 결과후 종료시 호정이가 로비 씬전환 전에 호출해주는 함수
    public void GameOver_Init()
    {
        if (GameObject.Find("Script_Object") == null) return;

        //게임 나가기 셋팅
        GameObject.Find("Script_Object").GetComponent<GamePlay_Script>().GameOver_Init(false);
    }

    //게임중 네트웍 끊김으로 로비로 강제로 나가기 전에 호정이가 선행으로 처리해주는 함수
    public void Network_Disconnect_GamePlay_Setting()
    {
        if (GameObject.Find("Script_Object") == null) return;

        //게임 나가기 셋팅
        GameObject.Find("Script_Object").GetComponent<GamePlay_Script>().GamePlay_Out_Setting();
    }

    //===================================================================================================================================================================================================================

    //플레이어 기본 정보 셋팅
    public void User_Data_Init()
    {
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        Server_IP = SendManager.Instance.Get_GameServerInfo().PubIp;
        Server_Port = SendManager.Instance.Get_GameServerInfo().CnPort;

        Game_Versoin = SendManager.Instance.Get_version();

        User_ID = SendManager.Instance.Get_userInfo().UserID;
        User_NickName = SendManager.Instance.Get_userInfo().NkNm;
        Killing_Message = SendManager.Instance.Get_KillingMessage(USERWORD_TYPE.WRD_KILL);
        Suicide_Message = SendManager.Instance.Get_KillingMessage(USERWORD_TYPE.WRD_SUICIDE);

        //현재 재화 셋팅
        Player_Diamond = SendManager.Instance.Get_userGoods(ITEMTYPE.GEM);
        Player_Gold = SendManager.Instance.Get_userGoods(ITEMTYPE.GOLD);
                        
        User_Team = 0;
        User_Clan_Mark = SendManager.Instance.Get_userInfo().ClanMark;
        User_Clan_Name = SendManager.Instance.Get_userInfo().ClanName;
        User_Country_Mark = SendManager.Instance.Get_UserCountryCode();

        //현재 지정되어있는 캐릭터 인덱스
        //Char_Index = (int)SendManager.Instance.Get_useCharacterIdx();
        Get_UseCharacterIdx = SendManager.Instance.Get_useCharacterIdx();
        Char_Index = (int)Get_UseCharacterIdx[0];

        //유저가 소유한 모든 캐릭터 정보
        Get_ALL_Char_Data = SendManager.Instance.Get_userCharacters();

        //플레이어가 소유한 총의 모든 정보
        Get_Char_ALL_Gun_Data = SendManager.Instance.Get_userWeapons();

        //지정된 캐릭터의 모든 정보
        User_Units Char_Data = Get_ALL_Char_Data[(uint)Char_Index];

        Char_Level = Char_Data.RefLv;
        Char_Now_HP = SendManager.Instance.Get_ReinfCharacter((uint)Char_Index, Char_Level).Hp;
        Char_Move_Speed = SendManager.Instance.Get_ReinfCharacter((uint)Char_Index, Char_Level).MvSpeed;

        Costume_Kind_1 = Char_Data.DecoIdx1;
        Costume_Kind_2 = Char_Data.DecoIdx2;
        Costume_Kind_3 = Char_Data.DecoIdx3;

        User_weapon Main_Gun_Data = Get_Char_ALL_Gun_Data[Char_Data.MainWpnIdx];

        Main_Gun_Index = (int)Main_Gun_Data.WpnIdx;
        Main_Gun_Level = Main_Gun_Data.RefLv;
        Main_Gun_Type = (byte)SendManager.Instance.Get_WeaponType(Main_Gun_Index);

        if (Get_Char_ALL_Gun_Data.ContainsKey(Char_Data.SubWpnIdx))
        {
            User_weapon Sub_Gun_Data = Get_Char_ALL_Gun_Data[Char_Data.SubWpnIdx];

            Sub_Gun_Index = (int)Sub_Gun_Data.WpnIdx;
            Sub_Gun_Level = Sub_Gun_Data.RefLv;
            Sub_Gun_Type = (byte)SendManager.Instance.Get_WeaponType(Sub_Gun_Index);
        }
        

       
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        //총 무기의 인덱스
        int[] Weapon_Index = SendManager.Instance.GetAll_WeaponIdx();
        W_Index = new int[Weapon_Index.Length];
        Array.Copy(Weapon_Index, 0, W_Index, 0, W_Index.Length);
        
        //총 무기중의 더블핸드건 체크
        int Count = 0;
        for (int i = 0; i < W_Index.Length; i++)
        {
            if ((GUN_TYPE)SendManager.Instance.Get_WeaponType(W_Index[i]) == GUN_TYPE.DOUBLE_HANDGUN) Count++;
        }
        W_Left_Index = new int[Count];
        Count = 0;
        for (int i = 0; i < W_Index.Length; i++)
        {
            if ((GUN_TYPE)SendManager.Instance.Get_WeaponType(W_Index[i]) == GUN_TYPE.DOUBLE_HANDGUN) W_Left_Index[Count++] = W_Index[i];
        }

        //총 무기중 런처,화염방사기 체크
        for (int i = 0; i < W_Index.Length; i++)
        {
            if ((GUN_TYPE)SendManager.Instance.Get_WeaponType(W_Index[i]) == GUN_TYPE.LAUNCHER) W_Launchar_Index = W_Index[i];
            else if ((GUN_TYPE)SendManager.Instance.Get_WeaponType(W_Index[i]) == GUN_TYPE.FLAMER_SKILL) W_FlameThrower_Index = W_Index[i];
            else if ((GUN_TYPE)SendManager.Instance.Get_WeaponType(W_Index[i]) == GUN_TYPE.ROCKET_SKILL) W_Rocket_Index = W_Index[i];
            else if ((GUN_TYPE)SendManager.Instance.Get_WeaponType(W_Index[i]) == GUN_TYPE.THROUGH_SHOT_SKILL) W_ThroughShot_Index = W_Index[i];            
        }
                
        
        //수류탄 인덱스 
        W_Grenade_Index = 21000;

        //총 코스튬 인덱스
        int[] Item_Index = SendManager.Instance.GetAll_DecoIdx();
        I_Index = new int[Item_Index.Length];
        Array.Copy(Item_Index, 0, I_Index, 0, I_Index.Length);

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    }

    //===================================================================================================================================================================================================================

    private static Link_Script s_Instance = null;
    public static Link_Script i
    {
        get
        {
            if (s_Instance == null) s_Instance = FindObjectOfType(typeof(Link_Script)) as Link_Script;

            if (s_Instance == null)
            {
                GameObject obj = new GameObject("Link_Script");
                s_Instance = obj.AddComponent(typeof(Link_Script)) as Link_Script;
                DontDestroyOnLoad(obj);
            }

            return s_Instance;
        }
        //set {
        //}
    }

    ////싱글톤 없애기
    //void OnApplicationQuit()
    //{
    //    s_Instance = null;
    //}

    ////싱글톤 시작하기
    //public void OnApplicationStart()
    //{
    //    instance.Awake();
    //}

    //===================================================================================================================================================================================================================
}
