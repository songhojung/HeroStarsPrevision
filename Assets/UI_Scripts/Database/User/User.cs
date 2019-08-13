using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class User
{
	//서버
	public Server_infos server_Info = new Server_infos();
	public Dictionary<byte, GameServer_Info> User_GameServerInfos = new Dictionary<byte, GameServer_Info>();		// 게임서버리스트들
	public GameServer_Info User_userGameServer = new GameServer_Info();										// 현재 사용하는 게임서버
	public byte DefaultConnectServerIdx = 0;																//기본 서버 인덱스

	

	//클라 버전
	public Dictionary<string, string> Market_versions = new Dictionary<string, string>();

	//로그인
	public Login_State LogInState = Login_State.LogNormal;		// 현재로그인 상태값
	public string AuthenticationKEY;						// 앱가드 인증을위한 키 ->로그인시 발급 받음

	//출첵
	public bool IsChkAttd = false;							//앱실행후 처음으로 출첵햇냐

	//광고
	public byte RltIdx = 0;				 //광고본후 얻은 룰렛인덱스
	public User_Ad User_Ads = new User_Ad(); //유저 광고정보

     //유저
	public User_Logins user_logins = new User_Logins();
	public User_Users user_Users = new User_Users();
    public Dictionary<ITEMTYPE, User_Good> User_Goods = new Dictionary<ITEMTYPE, User_Good>();
	public Dictionary<int, User_Times> User_Times = new Dictionary<int, User_Times>();
	public Dictionary<uint, User_Units> User_Units = new Dictionary<uint, User_Units>();
	public Dictionary<uint, User_drawSkill> User_DrawSkills = new Dictionary<uint, User_drawSkill>();					//유닛별 뽑은 보조스킬정보		
	public Dictionary<uint, User_Posts> User_Posts = new Dictionary<uint, User_Posts>();
	public Dictionary<byte, User_Block> User_Blocks = new Dictionary<byte, User_Block>();
	public Dictionary<uint, User_unit_useInfo> User_unit_useInfos = new Dictionary<uint, User_unit_useInfo>();
	public Dictionary<uint, User_weapon> User_Weapons = new Dictionary<uint, User_weapon>(); // weapong[무기인덱스]
	public Dictionary<uint, Dictionary<uint, user_Deco>> User_Decos = new Dictionary<uint, Dictionary<uint, user_Deco>>(); //deco[유닛인덱스][치장인덱스]
	public Dictionary<USERWORD_TYPE, User_Word> User_Words = new Dictionary<USERWORD_TYPE, User_Word>(); // 유저 게임문구
	public User_DeckUnit[] User_DeckUnits;
	public User_Games user_Games = new User_Games();
	public User_Clans user_Clans = new User_Clans();
	public User_Address user_Address = new User_Address();					//로그인시 그유저의 지역정보
	public User_unit_shop user_Unitshops = new User_unit_shop();				// 유저 유닛상점구매시 데이터

	//로비캐릭선택
	public User_useUnit User_useUnit = new User_useUnit();
	public static bool isSelectedCharacter = false;						//캐릭터 회전
	
	//클랜
	public Clan_members MyUserClan_member = new Clan_members();		//본유저의 클랜의 멤버 정보
	public Clan_members OtherUserClan_member = new Clan_members();	// 다른유저의 클랜의 멤버정보
	public Clans clan_Clans = new Clans();		//본유저가 속한 클랜 정보
	public Dictionary<uint, Clan_Lists> Clan_clanLists = new Dictionary<uint, Clan_Lists>();   // 추천클랜 리스트 들
	public Dictionary<byte, Clan_members> Clan_members = new Dictionary<byte, Clan_members>(); // 본유저가 속한 클랜의 멤버들정보
	public Dictionary<ushort, Clan_Boards> Clan_boards = new Dictionary<ushort, Clan_Boards>(); // 본유저가 속한 클랜의 게시물정보

	//채팅
	public List<User_Chat> Lst_user_ChatMessages = new List<User_Chat>();		//모든 채팅 메세지들
	public Dictionary<ChatMessageType, bool> user_RecieveMsgType = new Dictionary<ChatMessageType, bool>(); // 수신할 메세지 타입정보들
	public User_Chat user_SendChat = new User_Chat();			//본유저가 보낼 채팅정보
	public User_Chat user_RecieveChat = new User_Chat();			// 본유저가 받을 채팅정보
	public User_Chat user_ChatChannel = new User_Chat();

	//대기방
	public User_RoomInfo User_readyRoomInfo = new User_RoomInfo();						//대기방 정보
	public User_RoomInfo User_RcvRoomInvite = new User_RoomInfo();						//초대리시브 정보
	public List<User_RoomInfo> User_LstRcvRoomInvites = new List<User_RoomInfo>();			// 초대 리스브 정보들
	public User_RoomInfo User_RcvJoinTogether = new User_RoomInfo();						//같이하기 정보
	public Dictionary<uint, User_RoomInfo> User_RoomUserInfos = new Dictionary<uint, User_RoomInfo>(); //생성된 방정보
	


	//친구
	public Dictionary<uint, User_Friends> User_Friends = new Dictionary<uint, User_Friends>();
	public string InviteURL;

	//랭킹
	public Dictionary<uint, Rank_UnitExp> User_UnitRanking = new Dictionary<uint, Rank_UnitExp>(); // 본유저의 현재 가진 유닛들의 랭킹 정보 (키값 = 유닛인덱스)
	public Dictionary<uint, Rank_UnitExp> Rank_UnitExps = new Dictionary<uint, Rank_UnitExp>(); // 현재유닛별 랭킹정보
	public Dictionary<uint, Dictionary<uint, Rank_UnitExp>> Rank_DicUnitExps = new Dictionary<uint, Dictionary<uint, Rank_UnitExp>>(); // 각유닛별 랭킹정보


	//표시
	public Dictionary<MarkTitleType, bool> MarkChanges = new Dictionary<MarkTitleType, bool>();
	

	//연출데이터
	public Dictionary<ITEMTYPE, ShowingInfo> Dic_Showing = new Dictionary<ITEMTYPE, ShowingInfo>(); //레벨 관련 연출하기 위해 연출관련정보 담을 컬렉션변수

	//우편함시간데이터
	public DateTime Post_lastTimes;		//최근 우편함을 전달 받은 시간
	public DateTime Post_prevTimes;		//최근의이전 우편함을 전달 받은 시간
	public long User_Post_Count = 0;   // 우편함 전체 갯수 (클라 + 서버 포함)

	//아이템박스 오픈로그
	public Dictionary<int,List<GainItem>> User_BoxOpenLogs = new Dictionary<int,List<GainItem>>();


	public User()
	{
		Init();

	
	}

	public void Init()
	{
		LogInState = Login_State.LogNormal;
		IsChkAttd = false;
		AuthenticationKEY = string.Empty;
		server_Info.Init();
		User_userGameServer.Init();
		user_Users.Init();
		user_Games.Init();
		user_Clans.Init();
		clan_Clans.Init();
		Clan_clanLists.Clear();
		User_Goods.Clear();
		User_Times.Clear();
		User_Units.Clear();
		User_DrawSkills.Clear();
		User_Posts.Clear();
		User_Blocks.Clear();
		User_useUnit.Init();
		User_unit_useInfos.Clear();
		User_Weapons.Clear();
		User_Decos.Clear();
		user_Unitshops.Init();
		Clan_members.Clear();
		User_UnitRanking.Clear();
		User_Friends.Clear();
		Rank_UnitExps.Clear();
		OtherUserClan_member.Init();
		Init_MarkChanges();
		user_SendChat.Init();
		user_RecieveChat.Init();
		user_ChatChannel.Init();
		User_LstRcvRoomInvites.Clear();
		user_Address.Init();
		Init_RecieveMsgType();
		Lst_user_ChatMessages.Clear();
		Post_lastTimes = DateTime.MinValue;
		Post_prevTimes = DateTime.MinValue;
		isSelectedCharacter = false;
		
	}

	void Init_RecieveMsgType()
	{
		user_RecieveMsgType[ChatMessageType.Room] = true;
		user_RecieveMsgType[ChatMessageType.Clan] = true;
		user_RecieveMsgType[ChatMessageType.Whisper] = true;
		user_RecieveMsgType[ChatMessageType.FriendlyMatch] = true;
	}

	void Init_MarkChanges()
	{
		MarkChanges[MarkTitleType.NewsCount] = false;

	}


	public void check_Init_goods(ITEMTYPE _ItTp)
	{
		if (User_Goods.ContainsKey(_ItTp) == true) return;

		User_Goods[_ItTp] = new User_Good(_ItTp);

		switch (_ItTp)
		{
			case ITEMTYPE.LV:
				User_Goods[_ItTp].ItNum = 1;
				break;

			case ITEMTYPE.AD_REMOVE:
				User_Goods[_ItTp].ItNum = 0;
				break;

		}
	}


    public int Get_user_goods(ITEMTYPE _ItTp)
    {
		check_Init_goods(_ItTp);

        return User_Goods[_ItTp].ItNum;
    }

	public bool chk_tuto_bit(byte _bitIdx)
	{
		// 1 << 2  0001 을 왼쪽으로 쉬프트 연산 -> 0100
		if ((Get_user_goods(ITEMTYPE.TUTO_BIT) & (1 << (_bitIdx - 1))) != 0)
			return true;
		else
			return false;
	}


	public bool chk_PurchaseLimit(byte _bitIdx)
	{
		// 1 << 2  0001 을 왼쪽으로 쉬프트 연산 -> 0100
		if ((Get_user_goods(ITEMTYPE.PUR_LIMIT) & (1 << (_bitIdx-1))) != 0)
			return true;
		else
			return false;
	}


	public int Get_ChkPlayCount()
	{
		return (Get_user_goods(ITEMTYPE.WIN) + Get_user_goods(ITEMTYPE.LOSE) + Get_user_goods(ITEMTYPE.DRAW)) % 1000000000;
	}

	public int Get_allWeaponCount()
	{
	
		return User_Weapons.Count;
	}

	public int Get_allDecoCount()
	{
		int count = 0;
		foreach (var allDeco in User_Decos)
		{
			Dictionary<uint, user_Deco> decos = allDeco.Value;

			foreach (var dc in decos)
			{
				count++;
			}
		}

		return count;
	}



    public List<uint> GetEquipMainWpnIdxsOfHaveUnits(uint ownUnitIdx)
    {
        List<uint> _lstEquipedWpnIdx = new List<uint>();

        for (int i = 0; i < User_useUnit.UnitIdxs.Length; i++)
        {
            if (User_useUnit.UnitIdxs[i] == ownUnitIdx) continue; //매개변수로 들어온 유닛은 제외

            if(User_Units.ContainsKey(User_useUnit.UnitIdxs[i]))
            {
                _lstEquipedWpnIdx.Add(User_Units[User_useUnit.UnitIdxs[i]].MainWpnIdx);
            }
        }

        return _lstEquipedWpnIdx;
    }


    public uint GetUnitidxsWithEquipedThisWeapon(uint wpnIdx)
    {
        uint equipedThisWeaponUnitidx = 0;
        for (int i = 0; i < User_useUnit.UnitIdxs.Length; i++)
        {
            if (wpnIdx == 20100) break;

            if (User_Units.ContainsKey(User_useUnit.UnitIdxs[i]))
            {
                if (User_Units[User_useUnit.UnitIdxs[i]].MainWpnIdx == wpnIdx)
                    equipedThisWeaponUnitidx = User_useUnit.UnitIdxs[i];
            }
        }


        return equipedThisWeaponUnitidx;
    }


	//사용할 게임서버 설정하기
	public void Set_useGameServer(byte subIdx)
	{
		if (User_GameServerInfos.ContainsKey(subIdx))
			User_userGameServer = User_GameServerInfos[subIdx];
		else
		{
			foreach (var sv in User_GameServerInfos)
			{
				User_userGameServer = sv.Value;
				break;
			}
		}

		UserEditor.Getsingleton.EditLog ("서버접속 name : " + User_userGameServer.ServerName + " svidx : " + User_userGameServer.SubIdx);
	}


    public string GetBatchTxt(BatchType batch)
    {
        string txt = string.Empty;
        switch (batch)
        {
            case BatchType.First:
                txt = "1st";
                break;
            case BatchType.Second:
                txt = "2nd";

                break;
            case BatchType.Third:
                txt = "3rd";

                break;
        }

        return txt;
    }

	

}
