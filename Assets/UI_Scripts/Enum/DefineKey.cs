using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//======================배포시 확인 사항 ====================================
/*
 1. 서버체크 (개발, 라이브)
 2. 버젼체크 (개발 버전, 라이브버전) ->ReleaseVersion , isAlphaversion = false 로 설정
 3. 서버정보체크 (웹서버 주소) ,  ServerKind ConnectServer 체크
 4. defineKey 의 ConnectServer -> 디버그로그 위한 것 live로 변경
 5. 앱가드 적용 햇는지 확인(에디터 is Use Appguard login) 체크하기 =>AOS 만
 * 
*/

//======================배포시 확인 사항 ====================================

public enum ServerKind
{
	Developer = 1,		//개발 서버
	//Test,		//테스트 서버 (아직 서버 셋팅 다 안됨)
	Live,		//라이브 서버
}

public static class DefineKey
{
	//디버그 로그 할 종류
	public static ServerKind LogKind = ServerKind.Developer;

	//버젼값
	public const int Release_AOS_Version = 1013;		//안드로이드 배포빌드버전
	public const int Release_IOS_Version = 1012;		//IOS 배포빌드버전
	public const int QA_Version = 1000;				//QA 빌드버전
	public const bool isAlphaversion = false;		//알파버전상태이냐


	//프로토콜
	public static string CheckServerURL
	{
		get
		{
#if UNITY_EDITOR
			switch (UserEditor.Getsingleton.ConnectServer)
#else
			switch(UserEditor.Getsingleton.ConnectServer)
#endif
			{
				case ServerKind.Live: return "http://slb-889787.ncloudslb.com/GetServerList3.php";
#if UNITY_EDITOR
				default: return "http://dygames.iptime.org:40003/GetServerList_SG.php";
#else
				default: return "http://dygames.iptime.org:40003/GetServerList_SG.php";
#endif
            }
        }
	}

	public static int ReleaseVersion()
	{
		int ver = 0;

		

#if UNITY_EDITOR
		ver = Release_AOS_Version;

#elif UNITY_ANDROID
		ver = Release_AOS_Version;
#elif UNITY_IOS
		ver = Release_IOS_Version;
#endif

		if (isAlphaversion)
			ver = 1000;

		return ver;
	}

	//플랫폼 로그인키 및 이메일
	public const string UserPlatformID = "UserPlatformID";
	public const string UserPlatformEmail = "UserPlatformEmail";

	//플랫폼 타입
	public const string BeforeUserPlatformAccount = "BeforeUserPlatformAccount";
	
		
	public const string UserID = "UserID";
	public const string LgnToken = "LgnToken";


	public const string Notice = "공지";
	public const string Inform = "알림";
	public const string Gift = "선물";
	public const string Invite = "초대";
	public const string Complete = "완료";

	public const string combat = "전투";
	public const string store = "상점";

	public const string master = "마스터";
	public const string staff = "스텝";
	public const string normal = "일반";

    public const string LEAGUESUPPLY_MESSAGE = "You get supply Item";
    public const string CLANRANKINGREWARD_MASSGE = "You get clan ranking reward";


	//sprite 이름
	public const string icon_loading = "icon_loading";
	public const string back_slot_blank = "back_slot1_blank";
	public const string back_slot_ready = "back_slot1_ready";
	public const string back_slot_select = "back_slot1_select";
	public const string leagueNum = "text_leaguenumber_";
	public const string icon_grade = "icon_grade_";
	public const string character = "character_";
	public const string characterGem = "character_gem";
	public const string characterGold = "character_gold";
	public const string Exp = "icon_lv_1";
	public const string characterMedal = "character_medal";
	public const string characterLeague = "character_league";
	public const string characterClan = "character_clan";
	public const string clanmark = "clanmark_";
	public const string boxIcon = "boxicon_";
	public const string shopIcon = "shopicon_";
	public const string Gold = "icon_gold";
	public const string Gem = "icon_jam";
	public const string Cash = "icon_cash";
	public const string Medal = "icon_medal";
	public const string BattleCombo = "battle_text_";
	public const string Icon_master = "icon_master";
	public const string Icon_staff = "icon_staff";
	public const string ChatIcon_all = "chaticon_all";
	public const string ChatIcon_clan = "chaticon_clan";
	public const string ChatIcon_friend = "chaticon_friend";
	public const string ChatIcon_vs = "chaticon_vs";
	public const string Icon_Trophy = "icon_trophy";
	public const string Icon_Crown = "icon_crown";
	public const string text_Win = "text_win";
	public const string text_Lose = "text_lose";
	public const string text_Draw = "text_draw";
	public const string loginicon_guest = "loginicon_guest";
	public const string loginicon_google = "loginicon_google";
	public const string loginicon_facebook = "loginicon_facebook";
	public const string loginicon_gamecenter = "loginicon_gamecenter";
	public const string unitrank = "unitrank_";
	public const string chicon = "chicon_";
	public const string mapicon = "mapicon_";
	public const string Battleskill = "battle_skill_";
	public const string lobbySkill = "lobby_skill_";
	public const string WpnType = "wpntype_";
	public const string Remove = "icon_remove";
	public const string Set = "set_";

	//animation 키값
	public const string Ani_Boxshowing = "boxshowing_";
	public const string Ani_RouletteSpeed = "aniSpeed";


// playerpref 키값
	//로비 알림표시관련
	public const string UnitLeaderIdx = "UnitLeaderIdx";
	public const string IsCheckNewUnit = "isCheckNewUnit";
	public const string IsAvailableReinf = "isAvailableReinf";
	public const string UnitCount = "UnitCount";
	public const string BoardCount = "BoardCount";
	//setting 관련
	public const string setting_NoticeMsg = "setting_NoticeMsg";
	public const string setting_ClanMsg = "setting_ClanMsg";
	public const string setting_UnlockBxMsg = "setting_UnlockBxMsg";
	public const string setting_Quality = "setting_Quality";
	public const string setting_AttackType = "setting_AttackType";
	public const string setting_AutoBattle = "setting_AutoBattle";
	public const string setting_FPS = "setting_FPS";
	public const string setting_Sensitive = "setting_Sensitive";
	public const string setting_SensitiveZoomIn = "setting_SensitiveZoomIn";
	public const string setting_VolumBGM = "setting_VolumBGM ";
	public const string setting_VolumVoice = "setting_VolumVoice";
	public const string setting_VolumEffect = "setting_VolumEffect";
	public const string setting_Language = "setting_Language";

	//우편한관련
	public const string PostCount = "PostCount";
	public const string PostNoticeCount = "PostNoticeCount";

	//로비 튜토리얼
	public const string LobbyTuto_1 = "LobbyTuto_1";
	public const string LobbyTuto_2 = "LobbyTuto_2";

	//국가코드
	public const string UserCountryCode = "UserCountryCode";


	//색
	public static Color Green = new Color(0.52f,0.86f, 0.086f);
	public  static Color Blue = new Color(0, 0.5f, 1);
	public static Color LightBlack = new Color(60.0f / 255.0f, 60.0f / 255.0f, 60.0f / 255.0f);
	public static Color Yellow = new Color(1, 1, 0);
	public static Color YellowGreen = new Color(0.5f, 1f, 0);
	public static Color Cyan = Color.cyan;
	public static Color White = Color.white;
	public static Color SoldYellow = new Color(255.0f / 255.0f, 209.0f / 255.0f, 69.0f / 255.0f);



	//푸쉬 번호
	public const int PUSH_ID_FREEGEM = 1;		//무료보석 푸쉬ID
	public const int PUSH_ID_ATTDAY = 100;		//출석 푸쉬ID




	//Scene
	public const string Main = "Main";
	public const string GameScene = "GameScene";


	//Ver
	public const string Ver = "Ver";
	public const string MktUrl = "MktUrl";

	//약관동의
	public const string AgreeProvision = "AgreeProvision";

	//나중에 앱리뷰 
	public const string ReviewLater = "ReviewLater";

	//클라이언트 푸쉬
	public const string ClientPush = "ClientPush";

	//전면광고 시청
	public const string WatchedAds = "WatchedAds";

//분석용
	public const string PlayCount = "PlayCount";
	public const string Game = "Game";
	public const string Reinforce = "강화";
	public const string ReinforceWpn = "무기강화";
	public const string ReinforceUnit = "캐릭터강화";
	public const string RfLV = "강화Lv.";
	public const string User = "User";
	public const string Unit = "Unit";
	public const string Character = "캐릭터";
	public const string Weapon = "무기";
	public const string subWeapon = "서브무기";
	public const string Head = "머리";
	public const string Face = "얼굴";
	public const string Body = "몸";
	public const string Buy = "구매";
	public const string Deco = "치장";
	public const string HaveUnitCount = "유닛보유";
	public const string Over = "개 이상";
	public const string Over1 = "이상";
	public const string LessThan = "이하";
	public const string UnitKD = "유닛별킬데스";
	public const string UsedInGameplay = "인게임사용";
	public const string UsedCount = "사용횟수:";
	public const string UsedUnit = "UsedUnit";
	public const string UsedMainWp = "UsedMainWp";
	public const string UsedSubWp = "UsedSubWp";

}