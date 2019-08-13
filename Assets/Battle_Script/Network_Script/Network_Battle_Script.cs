using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public enum NETWORK_STATE { IDEL, 
                            CONNECT_INIT, CONNECT_START, CONNECTING, CONNECTING_OK, 
                            LOGIN_START, LOGGING, LOGIN_OK,                            
                            QUICKJOIN_START, QUICKJOINING, QUICKJOIN_OK,
                            GAMEPLAY_START, GAMEPLAYING, GAMEPLAY_END,
                            ERROR, ERROR_WAIT }

public enum DISCONNECT_STATE { IDLE, NORMALITY, ERROR }

public enum RELAY_PROTOCOL { MOVE, SHOT, SHOTGUN_SHOT, GRENADE_MOVE, GRENADE_EXPLOSION, SKILL, REVENGE_OK, BARRIER_CHECK, VOICE_CHAT, TEST };

enum RECV_GAMEOVER_SATAE { IDLE, GET_PROTOCOL, SET_INIT_END };

public class Network_Battle_Script : MonoBehaviour
{
    GamePlay_Script Game_Script = null;

    public NETWORK_STATE Network_State;

    float Send_HeartBeat_Time = 0.0f;
    float Recv_HeartBeat_Time = 0.0f;    

    public bool Network_Delay_Check = false;

    ByteData Send_Buffer;

    RECV_GAMEOVER_SATAE Recv_TimerOver_State;
    RECV_GAMEOVER_SATAE Recv_RewardEnd_State;
    ByteData Recv_TimerOver_Data = null;

    //================================================================================================================================================================================
    
    void Awake()
    {
        Network_State = NETWORK_STATE.IDEL;        
    }
       
    void Update()
    {
        Network_Operation();        
    }

    void Network_Operation()
    {
        if (!Link_Script.i.IsConnected()) return;

        switch (Network_State)
        {
            case NETWORK_STATE.IDEL:

                break;
            //case NETWORK_STATE.CONNECT_INIT:
            //    break;
            //case NETWORK_STATE.CONNECT_START:
            //    break;
            //case NETWORK_STATE.CONNECTING:
            //    break;
            //case NETWORK_STATE.CONNECTING_OK:
            //    break;
            //case NETWORK_STATE.LOGIN_START:
            //    break;
            //case NETWORK_STATE.LOGGING:
            //    break;
            //case NETWORK_STATE.LOGIN_OK:
            //    break;
            case NETWORK_STATE.QUICKJOIN_START:            

                //플레이어 기본 정보 셋팅
                Link_Script.i.User_Data_Init();

                Game_Script = null;

                Recv_TimerOver_State = RECV_GAMEOVER_SATAE.IDLE;
                Recv_RewardEnd_State = RECV_GAMEOVER_SATAE.IDLE;
                Recv_TimerOver_Data = null;

                Network_State = NETWORK_STATE.QUICKJOINING;

                Send_Quick_Join();

                //Debug.Log("퀵조인 시도");

                break;
            case NETWORK_STATE.QUICKJOINING:
                break;
            case NETWORK_STATE.QUICKJOIN_OK:

                if (!SendManager.Instance.Get_currentScene().Equals("Main"))
                {
                    //Debug.Log("씬전환 완료!!!!!!!!!!!!!!!!!");

                    Game_Script = GameObject.Find("Script_Object").GetComponent<GamePlay_Script>();

                    Network_State = NETWORK_STATE.GAMEPLAY_START;
                }

                break;
            case NETWORK_STATE.GAMEPLAY_START:

                Send_GamePlay_Start();
                                
                Send_HeartBeat_Time = 0.0f;
                Recv_HeartBeat_Time = 0.0f;

                Network_Delay_Check = false;

                Network_State = NETWORK_STATE.GAMEPLAYING;

                //Debug.Log("게임 시작 들어간다");

                break;
            case NETWORK_STATE.GAMEPLAYING:

                //하트비트 체크하기
                HeartBeat_Oeration();

                //게임오버 프로토콜 체크 연산
                Recv_GameOver_Operation();

                break;
            case NETWORK_STATE.GAMEPLAY_END:
                break;
            case NETWORK_STATE.ERROR:                                
                break;
            case NETWORK_STATE.ERROR_WAIT:
                break;
        }

    }

    //================================================================================================================================================================================  

    //하트비트 체크하기
    void HeartBeat_Oeration()
    {
        //일정 시간마다 서버에 하트비트 날리기
        Send_HeartBeat_Time += Time.deltaTime;
        if (Send_HeartBeat_Time >= 1.0f)
        {
            Send_HeartBeat_Time = 0.0f;
            Data_Send(NETKIND.CTS_CONNECTION_RETENTION, null);
        }

        //서버에서 날려주는 하트비트 체크하기
        Recv_HeartBeat_Time += Time.deltaTime;
        if (Recv_HeartBeat_Time >= 2.0f)
        {
            Network_Delay_Check = true;
        }
        else
        {
            Network_Delay_Check = false;
        }
    }

    //서버가 쏘는 하트비트 받기
    void Recv_HeartBeat()
    {
        Recv_HeartBeat_Time = 0.0f;
    }

    //클라이언트가 하트비트 안쏴서 서버가 먼저 끊었다
    void Recv_Connect_TimeOver()
    {
        Disconnect("클라이언트가 하트비트 안쏴서 서버가 먼저 끊었다");        
    }

    //================================================================================================================================================================================

    void Send_Quick_Join()
    {
        //byte : 전투 종류 (BattleKind)
        //int : 유닛 인덱스
        //byte : 유닛 강화도
        //int : 메인무기
        //int : 보조무기
        //int : 치장1 인덱스
        //int : 치장2 인덱스
        //int : 치장3 인덱스
        //byte : 서브 스킬
        
        Send_Buffer = new ByteData(512, 0);

        Send_Buffer.InPutByte((byte)Link_Script.i.Play_Mode);
        Send_Buffer.InPutByte(Link_Script.i.Char_Index);
        Send_Buffer.InPutByte(Link_Script.i.Char_Level);
        Send_Buffer.InPutByte(Link_Script.i.Main_Gun_Index);
        Send_Buffer.InPutByte(Link_Script.i.Sub_Gun_Index);
        Send_Buffer.InPutByte(Link_Script.i.Costume_Kind_1);
        Send_Buffer.InPutByte(Link_Script.i.Costume_Kind_2);
        Send_Buffer.InPutByte(Link_Script.i.Costume_Kind_3);
        Send_Buffer.InPutByte(Link_Script.i.Get_ALL_Char_Data[(uint)Link_Script.i.Char_Index].SubSkill);

        Data_Send(NETKIND.CTS_QUICK_JOIN, Send_Buffer.GetTrimByteData());
    }

    //1인 퀵조인 답변
    void Recv_Quick_Join(ByteData _Receive_data)
    {
        // byte : 전투 종류 (BattleKind)
        // byte : 맵 인덱스
        // byte : 게임종료 킬수
        // byte : 유저수
        //		uint : 방 유저 WebUserID 
        //		string : 닉네임
        //		byte : 팀 (Red:0, Blue:1, Draw:2)
        //		byte : 시작(부활 + 입장) 포인트
        //		ushort : 클랜 마크
        ///		byte[2] : 국가코드

        byte Total_User_Count = 0;

        uint _User_ID = 0;
        String _User_NickName = "";
        byte _User_Team = 0;
        byte _User_Start_Pos_Index = 0;
        ushort _User_Clan_Mark = 0;
        byte[] _User_Country_Mark = new byte[2];

        Link_Script.i.User_Data.Clear();

        //전투 종류
        byte Temp_BattleKind = 0;
        _Receive_data.OutPutVariable(ref Temp_BattleKind);
        Link_Script.i.Play_Mode = (BattleKind)Temp_BattleKind;

        //맵 인덱스
        _Receive_data.OutPutVariable(ref Link_Script.i.MapIndex);

        //게임 종료 목표 킬수
        _Receive_data.OutPutVariable(ref Link_Script.i.GameOver_Kill_Max);

        //전체 플레이어 숫자
        _Receive_data.OutPutVariable(ref Total_User_Count);

        for (int i = 0; i < Total_User_Count; i++)
        {
            _Receive_data.OutPutVariable(ref _User_ID);
            _Receive_data.OutPutVariable(ref _User_NickName);
            _Receive_data.OutPutVariable(ref _User_Team);
            _Receive_data.OutPutVariable(ref _User_Start_Pos_Index);
            _Receive_data.OutPutVariable(ref _User_Clan_Mark);
            _Receive_data.OutPutVariable(ref _User_Country_Mark[0]);
            _Receive_data.OutPutVariable(ref _User_Country_Mark[1]);

            if (Link_Script.i.User_Data.ContainsKey(_User_ID)) Debug.LogError(_User_ID + " : 이 아이디가 중복되서 들어왔다");

            Link_Script.i.User_Data.Add(_User_ID, new Link_Script.User_Data_class());
            Link_Script.i.User_Data[_User_ID].User_ID = _User_ID;
            Link_Script.i.User_Data[_User_ID].User_NickName = _User_NickName;
            Link_Script.i.User_Data[_User_ID].User_Team = _User_Team;
            Link_Script.i.User_Data[_User_ID].Start_Pos_Index = _User_Start_Pos_Index;
            Link_Script.i.User_Data[_User_ID].User_Clan_Mark = _User_Clan_Mark;
            Link_Script.i.User_Data[_User_ID].User_Country_Mark = new byte[2];
            Link_Script.i.User_Data[_User_ID].User_Country_Mark[0] = _User_Country_Mark[0];
            Link_Script.i.User_Data[_User_ID].User_Country_Mark[1] = _User_Country_Mark[1];
        }

        //내 캐릭터의 정보
        Link_Script.i.User_Team = Link_Script.i.User_Data[Link_Script.i.User_ID].User_Team;
        Link_Script.i.Start_Pos_Index = Link_Script.i.User_Data[Link_Script.i.User_ID].Start_Pos_Index;

        if (Network_State == NETWORK_STATE.QUICKJOINING)
        {
            Network_State = NETWORK_STATE.QUICKJOIN_OK;

            //퀵조인 성공시 호출
            SendManager.Instance.Complete_QuickJoin(Link_Script.i.MapIndex);
        }
    }

    //팀플 조인 답변
    void Recv_TeamPlay_Join(ByteData _Receive_data)
    {
        //플레이어 기본 정보 셋팅
        Link_Script.i.User_Data_Init();

        Game_Script = null;

        Recv_TimerOver_State = RECV_GAMEOVER_SATAE.IDLE;
        Recv_RewardEnd_State = RECV_GAMEOVER_SATAE.IDLE;
        Recv_TimerOver_Data = null;

        Network_State = NETWORK_STATE.QUICKJOINING;

        //1인 퀵조인 답변
        Recv_Quick_Join(Receive_Buffer);
    }

    //킬링 메시지
    void Recv_Killing_Message(ByteData _Receive_data)
    {
        // byte : 유저수
        //		uint : 방 유저 WebUserID 
        //		string : KILL 문구

        byte Total_User_Count = 0;
        uint _User_ID = 0;
        String _User_Killing_Message = "";
              
        //전체 플레이어 숫자
        _Receive_data.OutPutVariable(ref Total_User_Count);

        for (int i = 0; i < Total_User_Count; i++)
        {
            _Receive_data.OutPutVariable(ref _User_ID);
            _Receive_data.OutPutVariable(ref _User_Killing_Message);

            if (Link_Script.i.User_Data.ContainsKey(_User_ID) == false) continue;

            //Debug.Log("_User_ID = " + _User_ID + ", _User_Killing_Message = " + _User_Killing_Message);

            Link_Script.i.User_Data[_User_ID].Killing_Message = _User_Killing_Message;           
        }
    }

    //================================================================================================================================================================================

    //게임 시작 알려서 연결유지 시간 짧게 해준다.
    void Send_GamePlay_Start()
    {
        Data_Send(NETKIND.CTS_GAME_START, null);        
    }

    void Recv_GamePlay_Start()
    {
        
    }

    //================================================================================================================================================================================

    //어택성공 보내기
    public void Send_AtkDmg_Data(ByteData _unitData)
    {
        Data_Send(NETKIND.CTS_SET_USER_ATKDMG, _unitData.GetTrimByteData());
    }

    //HP 데이터 갱신
    void Recv_Set_HP(ByteData _Receive_data)
    {
        if (Game_Script == null) return;

        uint ATK_User_ID = 0;
        uint DEF_User_ID = 0;
        float DEF_HP = 0;
        Vector3 ATK_Start_Vec = new Vector3();
        Vector3 ATK_Target_Vec = new Vector3();
        byte Gun_Type = 0;
        bool Critical_Check = false;
        float Damage_Num = 0.0f;
        
        _Receive_data.OutPutVariable(ref ATK_User_ID);
        _Receive_data.OutPutVariable(ref DEF_User_ID);
        _Receive_data.OutPutVariable(ref DEF_HP);
        _Receive_data.OutPutVariable(ref ATK_Start_Vec);
        _Receive_data.OutPutVariable(ref ATK_Target_Vec);
        _Receive_data.OutPutVariable(ref Gun_Type);
        _Receive_data.OutPutVariable(ref Critical_Check);
        _Receive_data.OutPutVariable(ref Damage_Num);

        
        if (ATK_User_ID != DEF_User_ID)//자살이 아닐때
        {
            //공격한 유저
            if (Game_Script.Char_Script.ContainsKey(ATK_User_ID))
            {
                Game_Script.Char_Script[ATK_User_ID].Receive_ATK_UI_Damage_Init(DEF_User_ID, Critical_Check, Damage_Num);
            }

            //공격 당한 유저
            if (Game_Script.Char_Script.ContainsKey(DEF_User_ID))
            {
                Game_Script.Char_Script[DEF_User_ID].Receive_ATK_Data(DEF_HP, ATK_Start_Vec, ATK_Target_Vec, Gun_Type);
            }
        }
        else//자살로 판단한다.
        {
            //공격 당한 유저
            if (Game_Script.Char_Script.ContainsKey(DEF_User_ID))
            {
                Game_Script.Char_Script[DEF_User_ID].Receive_ATK_Data(DEF_HP, ATK_Start_Vec, ATK_Target_Vec, (byte)GUN_TYPE.SUICIDE);
            }
        }
    }

    //================================================================================================================================================================================

    //서버 현재 시간 요청 보내기
    public void Send_GetServerTime()
    {
        Data_Send(NETKIND.CTS_GAME_ROOM_TIME, null);
    }

    void Recv_GetServerTime(ByteData _Receive_data)
    {
        if (Game_Script == null) return;

        // long : 서버 현재 시간
        // long : 종료 예정 시간

        long Server_Now_Time = 0;
        long Server_End_Time = 0;

        _Receive_data.OutPutVariable(ref Server_Now_Time);
        _Receive_data.OutPutVariable(ref Server_End_Time);

        Game_Script.Play_GameStart_Time = new DateTime(Server_Now_Time) - DateTime.Now;
        Game_Script.Play_GameEnd_Time = new DateTime(Server_End_Time); 
    }

    //================================================================================================================================================================================

    //리스폰시 캐릭터 셋팅 바뀐값 서버에 넘겨주기
    public void Send_Respawn_Char_Init(ByteData _unitData)
    {
        Data_Send(NETKIND.CTS_UNIT_CHANGE, _unitData.GetTrimByteData());
    }

    void Recv_Respawn_Data(ByteData _Receive_data)
    {
        if (Game_Script == null) return;

        uint User_ID = 0;        

        _Receive_data.OutPutVariable(ref User_ID);        

        if (Game_Script.Char_Script.ContainsKey(User_ID))
        {
            Game_Script.Char_Script[User_ID].Recv_Respawn_Data(_Receive_data);
        }
    }

    //================================================================================================================================================================================

    //즉시 리스폰 데이터 보내기
    public void Send_Now_Respawn_Data(ByteData _unitData)
    {
        Data_Send(NETKIND.CTS_INSTANT_REVIVAL, _unitData.GetTrimByteData());
    }

    void Recv_Now_Respawn_Data(ByteData _Receive_data)
    {
        if (Game_Script == null) return;

        //int : 골드 재화 보유량
        //int : 보석 재화 보유량
        //byte : 즉시 부활 종류

        byte _Respawn_Kind = 0;

        _Receive_data.OutPutVariable(ref Link_Script.i.Player_Gold);
        _Receive_data.OutPutVariable(ref Link_Script.i.Player_Diamond);
        _Receive_data.OutPutVariable(ref _Respawn_Kind);

        SendManager.Instance.Set_userGoods(ITEMTYPE.GOLD, Link_Script.i.Player_Gold);
        SendManager.Instance.Set_userGoods(ITEMTYPE.GEM, Link_Script.i.Player_Diamond);
        Game_Script.Respawn_Kind = (RESPAWN_KIND)_Respawn_Kind;
    }

    //================================================================================================================================================================================

    void Recv_UserKill(ByteData _Receive_data)
    {
        if (Game_Script == null) return;

        //캐릭터 죽었을때 데이터 받기
        Game_Script.Recv_Kill_Info(_Receive_data);
    }

    //================================================================================================================================================================================

    //플레이중에 들어온 유저 체크
    void Recv_RoomIn_User(ByteData _Receive_data)
    {
        //uint : 접속 유저 WebUserID

        if (Game_Script == null) return;

        uint In_User_ID = 0;

        Receive_Buffer.OutPutVariable(ref In_User_ID);

        Game_Script.User_InGame_List.Add(In_User_ID);

        //Debug.Log("In_User_ID = " + In_User_ID);
    }

    //================================================================================================================================================================================

    //플레이중에 나간 유저 체크
    void Recv_RoomOut_User(ByteData _Receive_data)
    {
        if (Game_Script == null) return;

        uint Out_User_ID = 0;

        Receive_Buffer.OutPutVariable(ref Out_User_ID);

        Game_Script.User_OutGame_List.Add(Out_User_ID);
    }

    //================================================================================================================================================================================

    //자살 데이터 보내기
    public void Send_Suicide_Data()
    {
        Data_Send(NETKIND.CTS_SET_SUICIDE, null);
    }

    //================================================================================================================================================================================

    //봇 죽인 데이터 보내기
    public void Send_Bot_Kill_Data(ByteData _unitData)
    {
        Data_Send(NETKIND.CTS_ALONE_PLAY_KILL, _unitData.GetTrimByteData());
    }

    //================================================================================================================================================================================

    //P to P : 데이터 보내기
    public void Send_PTP_Data(ByteData _unitData)
    {
        Data_Send(NETKIND.CTS_GAMEDATA_RELAY, _unitData.GetTrimByteData());
    }

    byte Relay_Protocol = 0;
    uint Relay_User_ID = 0;

    //P to P  : 데이터 받기
    void Recv_PTP_Data(ByteData _Receive_data)
    {
        if (Game_Script == null) return;

        Relay_Protocol = 0;
        Relay_User_ID = 0;

        _Receive_data.OutPutVariable(ref Relay_Protocol);

        switch ((RELAY_PROTOCOL)Relay_Protocol)
        {
            case RELAY_PROTOCOL.MOVE://유닛 이동 데이터 받기
                                
                Game_Script.Recv_Char_Move_Data(_Receive_data);

                break;           
            case RELAY_PROTOCOL.SHOT://유닛 총쏘는 데이터 받기

                _Receive_data.OutPutVariable(ref Relay_User_ID);

                if (Game_Script.Char_Script.ContainsKey(Relay_User_ID) == false) break;

                Game_Script.Char_Script[Relay_User_ID].Network_Char_Shot_Pos(_Receive_data);

                break;
            case RELAY_PROTOCOL.SHOTGUN_SHOT://유닛 샷건 총쏘는 데이터 받기

                _Receive_data.OutPutVariable(ref Relay_User_ID);

                if (Game_Script.Char_Script.ContainsKey(Relay_User_ID) == false) break;

                Game_Script.Char_Script[Relay_User_ID].Network_Char_ShotGun_Pos(_Receive_data);

                break;
            case RELAY_PROTOCOL.GRENADE_MOVE://수류탄 움직임 데이터

                Game_Script.Recv_Grenade_Move_Data(_Receive_data);

                break;
            case RELAY_PROTOCOL.GRENADE_EXPLOSION://수류탄 터지는 데이터

                Game_Script.Recv_Explosion_Data(_Receive_data);

                break;
            case RELAY_PROTOCOL.SKILL://유닛 스킬 데이터 받기

                Game_Script.Recv_Char_Skill_Data(_Receive_data);

                break;
            case RELAY_PROTOCOL.REVENGE_OK://복수 성공 데이터 받기

                _Receive_data.OutPutVariable(ref Relay_User_ID);

                if (Game_Script.Char_Script.ContainsKey(Relay_User_ID) == false) break;

                Game_Script.Char_Script[Relay_User_ID].Revenge_OK_Recv_Data();

                break;
            case RELAY_PROTOCOL.BARRIER_CHECK://방어막 데이터 받기

                _Receive_data.OutPutVariable(ref Relay_User_ID);

                if (Game_Script.Char_Script.ContainsKey(Relay_User_ID) == false) break;

                Game_Script.Char_Script[Relay_User_ID].Barrier_Recv_Data(_Receive_data);

                break;
            case RELAY_PROTOCOL.VOICE_CHAT://음성챗팅 데이터 받기

                Game_Script.Recv_Voice_Chat_Data(_Receive_data);

                break; 
            default:

                Disconnect(" P to P 선언 되지 않은 프로토콜 : " + Relay_Protocol);

                break;
        }        
    }

    //================================================================================================================================================================================

    //현재 전체 스코어 요청
    public void Send_Now_Total_Score()
    {
        Data_Send(NETKIND.CTS_GAME_SCORE, null);
    }

    //현재 전체 스코어 데이터 받기
    void Recv_Now_Total_Score(ByteData _Receive_data)
    {
        if (Game_Script == null) return;

        Game_Script.Recv_Now_Total_Score(_Receive_data);
    }

    //================================================================================================================================================================================

    public void Send_Base_Atk(float Damage_Num)
    {
        // float : 데미지량 

        Send_Buffer = new ByteData(128, 0);

        Send_Buffer.InPutByte(Damage_Num);        

        Data_Send(NETKIND.CTS_SET_ATK_MAINBASE, Send_Buffer.GetTrimByteData());
    }

    void Recv_Base_HP(ByteData _Receive_data)
    {
        // float : 최대 체력
        // float : Red팀 진지 체력
        // float : Blue팀 진지 체력

        _Receive_data.OutPutVariable(ref Link_Script.i.Base_OJ_Max_HP);
        _Receive_data.OutPutVariable(ref Link_Script.i.Base_OJ_Now_HP[0]);
        _Receive_data.OutPutVariable(ref Link_Script.i.Base_OJ_Now_HP[1]);
    }

    //================================================================================================================================================================================

    //게임 나가기 보내기
    public void Send_Game_Out()
    {
        Data_Send(NETKIND.CTS_BATTLEROOM_OUT, null);
    }

    //================================================================================================================================================================================
    
    //게임 시간이 끝났다, 보상 결과 올때까지 대기
    void Recv_TimerOver(ByteData _Receive_data)
    {
        Recv_TimerOver_State = RECV_GAMEOVER_SATAE.GET_PROTOCOL;
        Recv_TimerOver_Data = _Receive_data;
    }

    //서버에서 게임 보상 셋팅 완료했다 게임 완전히 끝낸다.
    void Recv_RewardEnd()
    {
        Recv_RewardEnd_State = RECV_GAMEOVER_SATAE.GET_PROTOCOL;
    }

    //게임오버 프로토콜 체크 연산(게임씬으로 이동전에 게임오버 프로토콜을 받을수 있기때문에, 게임오버 프로토콜은 버퍼에 담아 놓고 게임플레이 상태에서 처리 해준다.)
    void Recv_GameOver_Operation()
    {
        if (Recv_TimerOver_State == RECV_GAMEOVER_SATAE.GET_PROTOCOL)
        {
            Game_Script.Recv_TimerOver(Recv_TimerOver_Data);

            Recv_TimerOver_State = RECV_GAMEOVER_SATAE.SET_INIT_END;
        }

        if (Recv_RewardEnd_State == RECV_GAMEOVER_SATAE.GET_PROTOCOL && Recv_TimerOver_State == RECV_GAMEOVER_SATAE.SET_INIT_END)
        {
            Game_Script.Recv_RewardEnd();

            Recv_RewardEnd_State = RECV_GAMEOVER_SATAE.SET_INIT_END;
        }
    }

    //================================================================================================================================================================================

    //데이터 보내기
    void Data_Send(NETKIND _Protocol, byte[] Send_Data)
    {
        SendManager.Instance.Send_NetData(_Protocol, Send_Data);
    }

    //================================================================================================================================================================================

    ByteData Receive_Buffer;
    NETKIND Receive_Protocol;

    public void Receive_Delegate(byte[] Packet_Data)
    {
        try
        {            
            Receive_Buffer = new ByteData(Packet_Data);
            Receive_Protocol = (NETKIND)Receive_Buffer.Getbyte();
                       
            switch (Receive_Protocol)
            {                
                case NETKIND.STC_BATTLEROOM_INFO:

                    Recv_Quick_Join(Receive_Buffer);

                    break;
                case NETKIND.STC_BATTLEROOM_START_INFO:

                    Recv_TeamPlay_Join(Receive_Buffer);

                    break;
                case NETKIND.STC_KILL_WORDS:

                    Recv_Killing_Message(Receive_Buffer);

                    break;
                case NETKIND.STC_CONNECTION_RETENTION:

                    Recv_HeartBeat();

                    break;
                case NETKIND.STC_CONNECT_TIMEOVER:

                    Recv_Connect_TimeOver();

                    break;
                case NETKIND.STC_GAME_START:

                    Recv_GamePlay_Start();

                    break;
                case NETKIND.STC_GAMEDATA_RELAY:
                                        
                    Recv_PTP_Data(Receive_Buffer);

                    break;
                case NETKIND.STC_SET_HP:

                    Recv_Set_HP(Receive_Buffer);

                    break;
                case NETKIND.STC_GAME_ROOM_TIME:

                    Recv_GetServerTime(Receive_Buffer);

                    break;
                case NETKIND.STC_REVIVAL:

                    Recv_Respawn_Data(Receive_Buffer);

                    break;

                case NETKIND.STC_INSTANT_REVIVAL:

                    Recv_Now_Respawn_Data(Receive_Buffer);

                    break;

                case NETKIND.STC_SET_USERKILL:

                    Recv_UserKill(Receive_Buffer);

                    break;
                case NETKIND.STC_ROOM_JOIN_NOTICE:

                    Recv_RoomIn_User(Receive_Buffer);

                    break;
                case NETKIND.STC_ROOM_OUT_USER://플레이중에 나간 유저 체크

                    Recv_RoomOut_User(Receive_Buffer);
                    
                    break;
                case NETKIND.STC_GAME_SCORE:

                    Recv_Now_Total_Score(Receive_Buffer);

                    break;                
                case NETKIND.STC_MAINBASE_HP:

                    Recv_Base_HP(Receive_Buffer);

                    break;
                case NETKIND.STC_GAME_TIMEOVER:

                    Recv_TimerOver(Receive_Buffer);

                    break;
                case NETKIND.STC_GAME_REWRDEND:

                    Recv_RewardEnd();

                    break;                
                //case NETKIND.STC_CHAT_MESSAGE:

                //    //인게임중 채팅 메세지를 받습니다 (서버에서 받은 byteData )
                //    SendManager.Instance.RecieveChatMessage_BattleGame(Receive_Buffer);

                //    break;
                //case NETKIND.STC_ERROR_CODE:

                //    byte Error_Code = 0;

                //    Receive_Buffer.OutPutVariable(ref Error_Code);

                //    Debug.LogError("서버에서 정의한 에러값 넘어왔다 : " + (ErrorCode)Error_Code);

                //    break;  
                default:

                    Disconnect("선언 되지 않은 프로토콜 : " + Receive_Protocol);

                    break;
            }
        }
        catch (Exception ex)
        {
            Disconnect(ex.ToString());
        }
    }

    //================================================================================================================================================================================

    void Disconnect(String Error_Note)
    {
        Debug.LogError("Error_Note : " + Error_Note);
    }

    //================================================================================================================================================================================
}
