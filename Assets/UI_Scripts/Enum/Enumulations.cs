using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum UI
{
	NONE= 99,
	TITLE = 0,
    TOP,
    LOBBY,
	CUSTOMROOM,
	MATCHING,
    STORE,
    CLAN,
    RANKING,
    FRIEND,
    NEWS,
    SETTING,
	CHAT,
	BATTLECHAT,
    POPUPNOTICE,
    POPUPSELECTIVE,
	INGAME_MAIN,
	INGAME_RESULT,
	LOADING,
	EQUIPMENT,
    CHARACTERSETTING,
}



public enum UIITEM
{
	ITEM_COMBATINFO,
	ITEM_LEAGUEUNIT,
	ITEM_UNIT,
	ITEM_CLANMEMBERELEMENT,
	ITEM_CLANBOARDELEMENT,
	ITEM_RANKINGELEMENT,
	ITEM_FRIENDELEMENT,
	ITEM_NEWSELEMENT,
	ITEM_GAINITEM,
	ITEM_STOREELEMENT,
	ITEM_STOREPACKAGEELEMENT,
	ITEM_CLANMARKLIST,
	ITEM_QUESTELEMENT,
	ITEM_CHATELEMENT,
	ITEM_EQUIPITEM,
	ITEM_CLANLISTELEMENT,
	ITEM_GAMEINVITEELEMENT,
	ITEM_RESULTUNITSLOT,
}

public enum UIPOPUP
{
	NONE,
	POPUPNOTICE,
	POPUPSELECTIVE,
	POPUPUSERINFO,
	POPUPLEAGUEINFO,
	POPUPSHOWINGBOX,
	POPUPSHOWINGLEVEL,
	POPUPTOAST,
	POPUPSUPPORTREQUEST,
	POPUPMAKENAME,
	POPUPBUYITEM,
	POPUPNETWORKERROR,
	POPUPCLANINFO,
	POPUPFIND,
	POPUPCHATMESSAGE,
	POPUPPLATFORM,
	POPUPCLANMARKLIST,
	POPUPREVIEW,
	POPUPCOUPON,
	POPUPREINFORCE,
	POPUPRFPROCESS,
	POPUPSTARTGAME,
	POPUPINVITE,
	POPUPATTENDANCE,
	POPUPSERVERSELECT,
	POPUPWRITEBOARD,
	POPUPGAINITEM,
	POPUPROULETTE,
	POPUPSHOPPURCHASE,
    POPUPBUYBUFF,
    POPUPBUYSETITEM,
}


public enum Button_STATE
{
	NONE = 99,
	STAY = 0,
	REMAIN = 1,
	READY = 2,
}

public enum ButtonState_Type
{
	NONE = 99,
	Normal = 0,
	Selected = 1,
}

public enum SUPPLYBOX_TYPE
{
	
	PublicSupply = 0,
	Tutorial = 1,
	BattleSupply_Third = 11, // 하급
	BattleSupply_Second = 12, // 중급
	BattleSupply_First = 13, // 상급
	MedalSupply = 21,
	FreeSupply = 31,
	BronzeSupply = 41, // 일반상자
	SilverSupply = 42, // 고급상자
	GoldSupply = 43,  // 최고급상자

}


public enum ITEMTYPE
{
    NONE = 0,
	GEM = 1,
    GOLD = 2,
	AD_REMOVE = 3,      //게임종료후 광고표시 0 :표시 , 0이상 광고제거됨
    EXP =4,
    GAMEPLAY = 5,           //게임플레이횟수
    LV = 6,
	ATDDAY = 7,		//연속출석일수
	TUTO_BIT = 8,
	CASH = 9,
	WIN = 10,
	LOSE = 11,
	DEFEATS = 12,
	VICTORIES = 13,
	WEEK_CASH = 14,
	MONTH_CASH = 15,
	PUR_LIMIT = 16 , //한번밖에 구매 못하는 상점 인덱스 인지 BIT 체크
	FRD_REWARD = 17, //친구초대 보상 횟수
	MAX_LEAGE = 18 , //최대 달성리그 (리그 달성 보상용도)
	DRAW = 19,
	RANK = 20,
	CLAN_SCORE = 22,
    UNIT = 65,
	WEAPON = 66,
	DECO = 67,
	ROULETTE = 68,

	ITEMBOX = 70,
	MAINUNIT = 99,
}

public enum TIMEIDX
{
	USERNAME =1,
	ADREWARD = 2,
	FREEBOX = 10,
	MEDALBOX = 11,
	UNITREQUEST = 12,
}


public enum LanguageCode
{
	NONE = 99,
	KR = 0,
	EN = 1,
	JP = 2,
	CN = 3,	
}

public enum AUDIOSOURCE_TYPE
{
	AUDIO_2D = 0,
	AUDIO_3D = 1,
}


public enum AUDIOSOUND_TYPE
{
	NONE = 99,
	VOICE = 0,
	EFFECT,
	MUSIC,


}

public enum MUSIC_TYPE
{
	NONE = 99,
	LOBBY = 0,
	BATTLE_1 = 1, 
}




public enum ProtocolName
{
	NONE = 99,
	CheckServer = 0,
	GetVersion = 1,
	GetServerTime = 2,
	GetReferenceDB = 3,
	GetServerList = 4,
	SetMemberLogin = 11,
	SetMemberJoin = 12,
	SetMemberChange = 13,
	GetAuthentication = 14,
	GetConnectAdress = 15,
	SetChoiceUnit = 16,
	SetChangeUsers = 17,
	GetUserInfos = 21,
	PostGetList = 22,
	PostRecv = 23,
	SetUseUnitIdx = 24,
	SearchUserClan = 25,
	UserNameChange = 26,
	SetOption = 27,
	SetTutoBit = 28,
	GetCooponReward = 29,
	GetAttendDayRwd = 100,
	SetEquipItem = 31,
	SetItemRef = 32,
	SetItBoxUnLock = 33,
	SetItBoxReward = 34,
	SetAdRwd = 35,
    GetAdDoubleRwd = 36,
    GameStart = 41,
	GameEnd = 42,
	GameEndRfsData = 43,
	VictoriesReStore = 44,
	GetEventGameRwd = 45,
	ClanMake = 51,
	ClanJoin = 52,
	ClanInfo = 53,
	ClanSetBuf = 54,
	ClanGiveUnit = 55,
	ClanPersonsCntUp = 56,
	ClanNameChange = 57,
	ClanList = 58,
	ClanOut = 59,
	ClanSetStep = 60,
	ClanInvite = 61,
	ClanJoinAccept = 62,
	ClanMarkChange = 63,
	ClanInfoTxtChange = 64,
	ClanBodWrite = 65,
	ShopBuyItem = 71,
	ShopBuyUnit = 72,
	ShopBuyWeapon = 73,
	ShopBuyDeco = 74,
	ShopBuySkill = 75,
	BuySkillCommit = 76,
	FriendList = 81,
	FriendGetInviteUrl = 82,
	FriendAdd = 83,
	FriendRemove = 84,
	FriendPostInvite = 85,
	UnitExpRanking = 91,
	GetClanRanking = 92,
	SetWords = 101,

}

public enum TABLEDATE_TYPE
{
	unit_ranks = 1,
	unit_infos =2,
	reinforce_prices = 3,
	weapon_infos = 4,
	notices = 5,
	const_values = 6,
	attend_rewards = 7,
	deco_infos = 8,
	reward_double_events = 9,
	shops = 10,
	skill_infos = 11,
	weapon_reinforces = 12,
	unit_reinforces = 13,
	event_shops = 14,
	roulette_rewards = 15,
	block_pkgnms= 16,
	ad_rewards = 17,
	shop_month_rewards = 18,
	shop_items = 19,
	event_shop_times = 20,
	set_bufs = 21,
    user_levels = 22,
}


public enum BattleEnteranceKind
{
    QuickJoin = 0,
    Trainning,
    MakeRoom,
}


public enum UNITSORTING_TYPE
{
	RANK = 0,
	LEAGUE = 1,
	COST = 2,
}

public enum FRIENDSORTING_TYPE
{
	ADD = 0,			//추가순
	NAME = 1,		//이름순
	LOGIN = 2,		//로그인순
	LOGINTIME = 3,	//로그인 시간순
}

public enum NEWSTYPE
{
	NONE = 99,
	WHOLE = 0, // 전체
	INFORM = 1,  // 공지, 가입성공,강퇴
	REQUEST = 2, // 요청(가입요청,초대요청)
	SUPPLY = 4,	// 첨부물
	
}

public enum NewsToggle_Type
{
	NONE = 99,
	WHOLE = 0,
	NORMAL = 1,
	ATTACH = 2,
}

public enum CLANBOARD_TYPE				//게시물타입
{
	NONE = 0,
	BOARD_CLANREQUEST = 1,			//가입신청
	BOARD_CLANREGISTERED = 2,		//가입로그
	BOARD_CLANOUT = 3,			//탈퇴로그
	BOARD_CLANBANNED=4,			//강퇴로그
	BOARD_CLANBUFFGOLD=5,			//골드버프구매
	BOARD_CLANBUFFEXP=6,			//경험치 버프구매
	BOARD_USER = 7				//유저게시글

}

public enum CLAN_BOARD_ELEMENT_TYPE // boardelement 프리팹 오브젝트 타입
{
	NONE = 99,
	Member = 0,		//멤버관련게시물
	Systems = 1,		//시스템 게시물관련
	Supply = 2,
}

public enum CLAN_MEMBER_TYPE
{
	NORMAL = 0,
	STEFF = 1,
	MASTER = 2,

}

public enum MATCHTYPE
{
	ONEVSONE,
	TWOVSTWO,

}

public enum MAKENAMEPOPUP_TYPE
{
	NONE = 0,
	REGISTERUSERNAME = 1,
	CHANGEUSERNAME = 2,
	CHANGECLANNAME = 3,
	MAKECLAN = 4,

}

public enum CloseType
{
	NONE = 0,
	QUIT = 1,
	GOLOBBY =2,
}

public enum SpeechTxtType
{
	NONE = 99,
	Start = 0,
	Kill = 1,
	Dead = 2,
	Respon = 3,
}

public enum MarkTitleType
{
	NewsCount = 0,

}

public enum ChatMessageType
{
	NONE = 99,
	Room = 1,
	Clan = 2,
	Whisper = 3,
	Notice = 4,
	FriendlyMatch = 5,
	Battle = 6,
	ClanMatch = 7,
	Invite = 8,
}

public enum MMSERVER_STATE
{
	IDEL ,
	CONNECT_FAIL,
	CONNECT_INIT , 
	CONNECT_START , 
	CONNECTING, 
	CONNECTING_OK,
	LOGIN_START, 
	LOGIN_OK,
	LOGIN_COMPLETE,
	RECONNECT_START,
	RECONNECT_FINISH,
	ROOM_MAKE,
	ROOM_OPEN,
	ROOM_INVITE,
	ROOM_OUT,
	ROOM_INFO,
	ANSWER_ROOMINVITE,
	TEAM_MOVE,
	CHAT_MESSAGE,
	COMPLETE_QUICKJOIN,
	TRY_QUICKJOIN,
	ERROR,
	ERROR_WAIT,
	ERROR_END,
}






public enum Login_State
{
	LogNormal = 0,		//앱실행후 처음 로그인시
	LogOut = 1,			//로그아웃함
	LogedIn = 2,			//로그인함
	LogSelectServer = 3,  // 로그인중 서버선택
}

public enum PLATFORM_TYPE
{
	NONE = 0,
	GUEST = 1,
	GOOGLE = 2,
	FACEBOOK =3,
	GAMECENTER = 4,
}

//상수데이터 인덱에대한 enum문
public enum ConstValue_TYPE
{
	Const_GameRwdBasicExp = 1,			//전투결과 기본경험치
	Const_GameRwdBasicGold = 2,		//전투결과 기본골드
	Const_limitChangeNm = 3,			//닉네임변경 제한일수
	Const_limitAdTime = 4,				//광고보기 제한시간
	Const_InstantValue = 5,				//즉시 부활 가격
	Const_BuySubSkillPrice = 6,		//두번쟤 스킬구매 금액
	Const_InstantItTp = 7,				//즉시부활 재화 타입
	Const_AvailableShowAdCount = 8,		//일일광고보기가능횟수
	Const_AfterGameShowAd = 9,			//게임종료후 광고보기 상수
	Const_ClanBuffPrice = 10,			//클랜 버프 구매가격
	Const_ClanBuffDurationTime = 11,	//클랜버프 지속시간 n분
	Const_ClanGoldBuffRateValue = 12,	//클랜골드버프 효과배율
	Const_ClanExpBuffRateValue = 13,	//클랜Exp버프 효과배율
	Const_GameRwdWinWhenZero = 14,			//0킬일떄 승리 보상골드
	Const_GameRwdLoseWhenZero = 15,			//0킬일떄 패배 보상골드
	Const_Const_GameRwdDrawWhenZero = 16,	//0킬일때 드로우 보상골드
	Const_FlyingItTp = 17,				//비행부활 재화타입
	Const_FlyingValue = 18,			//비행부활 비용
}

public enum PostIdx_TYPE
{
	NONE = 0,
	ClanJoinRquestMsg = 1,			//클랜가입요청메세지
	ClanJoinInviteMsg = 2,			//클랜가입초대메세지
	GiftFromOperator =3,			//운영자선물
	GiftBonusFromInAppPurchase = 4,//상점 구매보너스 물품
	FriendInvite = 5,				//친구초대메세지
	ClanJoinSucces = 6,			//클랜가입 성공
	CompleteRwdTutorial = 7,		//튜토리얼 완료보상
	Monthly = 8,					//월정액
	ClanKickMsg = 9,				//클랜강퇴메세지
	AttdReward = 10,				//출석보상
}

public enum PushAlaram_TYPE
{
	NONE = 99,
	NOTICE = 0,
	CLAN = 1,
	BOXOPEN = 2,
}

public enum RankingType
{
	None = 99,
	WHOLE = 100,
	SOLDIER = 0,
	JUMPER = 1,
	VISION = 2,
	ROLLER = 3,
	GUARDER = 4,
	BOOMER =5,
	FLAMER = 6,
	BAZOOKY = 7,
	ENERKY = 8,
	UNIT_EMPTY1 = 9,
	UNIT_EMPTY2 = 10,
	UNIT_EMPTY3 = 11,
	UNIT_EMPTY4 = 12,

}


public enum SKILL_TYPE
{
	NONE = 0,
	SOLDIER = 1,
	JUMPER = 2,
	VISION = 3,
	ROLLER = 4,
	GUARD = 5,
	BOOMER = 6,
	FLAMER = 7,
	ROCKET = 8,
	FLYING = 9,
	RAGE = 10,
	HIGHJUMP = 11,
	INCREASE_HP = 12,
	INCREASE_MAGAZINE = 13,
	INCREASE_POWER = 14,
	THROW_SHOT = 15,
	SKILL_EMPTY1 = 16,
	SKILL_EMPTY2 = 17,
	SKILL_EMPTY3 = 18,
	SKILL_EMPTY4 = 19,
	SKILL_EMPTY5 = 20,
}



public enum BatchType
{
    First = 0,
    Second = 1,
    Third = 2,
}


public enum AdDoubleRewardType
{
    None = 99,
    GameGold = 1, // 게임골드보상
    GameExp = 2, //게임경치보상
    LvUpRwd = 3, //레벨업보상
}


public enum Ads_TYPE
{
	NONE = 99,
	Reward = 0,
	Interstitial_Image = 1,
	Interstitial_Video = 2,
	Banner = 3,
}

public enum ADSTOOL_KIND
{
	TAPJOY = 0,
	ADCOLONY =1,
	ADMOB = 2,
	FACEBOOKAD = 3,
	MOPUBMD = 10,   //모펍미디에이션

}



public enum EquipType
{
	NONE = 99,
	Main = 0,
	Sub = 1,
	Dress_HEAD = 2,
	Dress_BODY = 3,
	Dress_FACE = 4,
}



public enum FindID_Type
{
	NONE = 99,
	USER = 0,
	CLAN = 1,
	FRIEND = 3,
}

public enum MemberSlot_State
{
	NONE = 99,
	Empty = 0,
	SelectOwn = 1,
	SelectOther = 2,

}

public enum BUFF_TYPE
{
	NONE = 99,
	CLANEXP = 0,
	EVENTEXP = 1,
	CLANGOLD = 2,
	EVENTGOLD = 3,
}

public enum EVENT_KIND
{
	NONE = 0,
	EXP = 1,
	GOLD = 2,
}

public enum WEAPONE_TYPE
{
	NONE = 99,
	PISTOL = 0,
	REVOLVER = 1,
	DOUBLE_HANDGUN = 2,
	PUMP_SHOTGUN = 3,
	AUTO_SHOTGUN = 4,
	RIFLE = 5,
	SUB_MACHINGUN = 6,
	HEAVY_MACHINGUN = 7,
	SNIPER = 8,
	GRENADE = 9,
	LAUNCHER = 10,
	RUSH_SKILL = 11,
	FLAME_THROWER_SKILL = 12,
	SUICIDE = 13,
	ROCKET_SKILL = 14,
	FULSEGUN = 15,
	BOOMER_SKILL = 16,			//클라에서만 쓰는 부머 스킬타입관련
}

public enum EQUIPPART_TYPE
{
	NONE = 99,
	PRIMARY = 1,
	SECONDARY = 2,
	THIRDARY = 3,
}

public enum DECOPART_TYPE
{
	NONE =99,
	HEAD = 1,
	BODY = 2,
	FACE = 3,
}

public enum DECOABL_TYPE
{
	Granade_Pw = 0,			//수류탄공격력
	Respawn_Tm = 1,			//리스폰시간
	Skill_Tm = 2,				//스킬쿨타임
}

public enum REINFORCE_TYPE
{
	NONE = 99,
	UNIT =0,
	WEAPON = 1,
	DECO = 2,
}

public enum USERWORD_TYPE
{
	NONE = 99,
	WRD_KILL = 1,		//킬문구
	WRD_SUICIDE = 2, //자살문구
}

public enum WEEK_TYPE
{
	SUN = 0,	//일
	MON = 1,	//월
	THU = 2,//화
	WHE = 3,//수
	THE = 4,//목
	FRI = 5,//금
	SAT = 6,//토
}
