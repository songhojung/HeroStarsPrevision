using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public  class webRequest
{
	public static ITEMTYPE nowUseGoods = ITEMTYPE.NONE; // 제화를 사용하는 프로토콜을 호출시  어떤 제화를사용하는지 저장하는변수 (제화부족으로 상점이동시 필요)
	public static uint nowSearchRankingUnitIdx = 0;		// 랭킹 검색시 현재 랭킹 검색하고 있는 유닛인덱스

	#region Protocol : CheckServer (서버상태 알기)
	public static void CheckServer(byte _MktIdx, int _ver,byte _LggCd ,del_webResp_0 delfunc)
	{
		netData _net = new netData(ProtocolName.CheckServer);

		_net.set_SendData("MktIdx", _MktIdx);
		_net.set_SendData("ver", _ver);
		_net.set_SendData("LggCd", _LggCd);

		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.CheckServer;
		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion


	



	#region Protocol : GetVersion ( 버전체크)
	public static void GetVersion(byte _MktIdx, del_webResp_0 delfunc)
	{
		netData _net = new netData();

		_net.set_SendData("MktIdx", _MktIdx);

		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.GetVersion;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion


	#region Protocol : GetServerTime (서버시간 가져오기)
	public static void GetServerTime(del_webResp_0 delfunc)
	{
		netData _net = new netData();

		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.GetServerTime;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion


	#region Protocol : GetReferenceDB (구성정보 가져오기)
	public static void GetReferenceDB(List<int> _dataIdxLst, del_webResp_0 delfunc)
	{
		netData _net = new netData();

		_net.set_SendData("DataIdxLst", _dataIdxLst);

		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.GetReferenceDB;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion


	#region Protocol : GetServerList (게임 서버 리스트 받기)
	public static void GetServerList(string _CtrCd, del_webResp_0 delfunc)
	{
		netData _net = new netData();
		_net.set_SendData("CtrCd", _CtrCd);

		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.GetServerList;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion


	#region Protocol : SetMemberLogin (로그인)
	public static void SetMemberLogin(byte _PlfID, string _lgnKey, int _ver,byte _MktIdx,byte _LggCd,string _CtrCd, string _PNID, del_webResp_0 func1)
	{
		netData _net = new netData();

		_net.set_SendData("PlfID", _PlfID);
		_net.set_SendData("LgnKey", _lgnKey);
		_net.set_SendData("Ver", _ver);
		_net.set_SendData("MktIdx", _MktIdx);
		_net.set_SendData("LggCd", _LggCd);
		_net.set_SendData("CtrCd", _CtrCd);
		_net.set_SendData("PNID", _PNID);

		_net.del_result = func1;
		_net.protocolName = ProtocolName.SetMemberLogin; // 요청할 프로토콜이름 설정

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
		
	}
	#endregion




	#region Protocol : SetMemeberJoin (회원가입)
	public static void SetMemeberJoin(byte _PlfID,string _lgnKey, string _nkNm , string _Email, del_webResp_0 func1)
	{
		netData _net = new netData();

		_net.set_SendData("PlfID", _PlfID);
		_net.set_SendData("NkNm", _nkNm);
		_net.set_SendData("LgnKey", _lgnKey);
		_net.set_SendData("Email", _Email);


		_net.del_result = func1;
		_net.protocolName = ProtocolName.SetMemberJoin; // 요청할 프로토콜이름 설정

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion

	#region Protocol : SetMemeberChange (플랫폼 계정 변경)
	public static void SetMemeberChange(byte _PlfID, string _lgnKey, string _Email, del_webResp_0 func1)
	{
		netData _net = new netData();

		_net.set_SendData("PlfID", _PlfID);
		_net.set_SendData("LgnKey", _lgnKey);
		_net.set_SendData("Email", _Email);


		_net.del_result = func1;
		_net.protocolName = ProtocolName.SetMemberChange; // 요청할 프로토콜이름 설정

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion


	#region Protocol : GetAuthentication (앱가드인증키받기)
	public static void GetAuthentication(int _Ver ,byte _PlfID, string _lgnKey, del_webResp_0 func1)
	{
		netData _net = new netData();

		_net.set_SendData("Ver", _Ver);
		_net.set_SendData("PlfID", _PlfID);
		_net.set_SendData("LgnKey", _lgnKey);


		_net.del_result = func1;
		_net.protocolName = ProtocolName.GetAuthentication; // 요청할 프로토콜이름 설정

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion


	#region Protocol : GetConnectAdress (지역 주소값받기)
	public static void GetConnectAdress(del_webResp_0 func1)
	{
		netData _net = new netData();

		_net.del_result = func1;
		_net.protocolName = ProtocolName.GetConnectAdress; // 요청할 프로토콜이름 설정

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion


	#region Protocol : SetChoiceUnit (최초 가입시 유닛 선택)
	public static void SetChoiceUnit(uint _UnitIdx, del_webResp_0 delfunc)
	{

	

		netData _net = new netData();

		_net.set_SendData("UnitIdx", _UnitIdx);

		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.SetChoiceUnit;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion




	#region Protocol : SetChangeUsers (유저데이터변경)
	public static void SetChangeUsers(string _CtrCd , del_webResp_0 delfunc)
	{



		netData _net = new netData();

		_net.set_SendData("CtrCd", _CtrCd);

		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.SetChangeUsers;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion



	#region Protocol : SetUseUnitIdx (최초 가입시 유닛 선택)
	public static void SetUseUnitIdx(uint _UnitIdx1, uint _UnitIdx2, uint _UnitIdx3, del_webResp_0 delfunc)
	{



		netData _net = new netData();

		_net.set_SendData("UnitIdx1", _UnitIdx1);
		_net.set_SendData("UnitIdx2", _UnitIdx2);
        _net.set_SendData("UnitIdx3", _UnitIdx3);

        _net.del_result = delfunc;
		_net.protocolName = ProtocolName.SetUseUnitIdx;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion


	#region Protocol : GetUserInfos (유저정보 가져오기)
	public static void GetUserInfos(uint _ChkUserID, del_webResp_0 delfunc)
	{
		
		//확인할 유저아이디 저장
		if (UserDataManager.instance.user.user_Users.UserID != _ChkUserID)
		{
			//확인할 유저아이디 순차적으로 큐에저장
			UserDataManager.instance.Que_OtherUserIDs.Enqueue(_ChkUserID);
		}

		netData _net = new netData();

		_net.set_SendData("ChkUserID", _ChkUserID);

		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.GetUserInfos;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion

	#region Protocol : SearchUserClan (유저또는클랜 찾기)
	public static void SearchUserClan(byte _SearchKind,string _SearchID, del_webResp_0 delfunc)
	{
		//확인할 유저아이디 저장

		if (_SearchKind == 1) //유저이름
		{
			if (!string.Equals(UserDataManager.instance.user.user_Users.NkNm, _SearchID))
			{
				//확인할 유저아이디 순차적으로 큐에저장
				UserDataManager.instance.Que_OtherUserIDs.Enqueue(_SearchID);
			}
		}
		else if (_SearchKind == 4) //유저아이디
		{
			if (!string.Equals(UserDataManager.instance.user.user_Users.NkNm, _SearchID))
			{
				//확인할 유저아이디 순차적으로 큐에저장
				UserDataManager.instance.Que_OtherUserIDs.Enqueue(_SearchID);
			}
		}
		else if(_SearchKind == 2) //클랜이름
		{
			
				if (!string.Equals(UserDataManager.instance.user.clan_Clans.ClanName, _SearchID))
				{
					//확인할 클랜이름 순차적으로 큐에저장
					UserDataManager.instance.Que_OtherUserIDs.Enqueue(_SearchID);
				}
		
		
		}
		else if (_SearchKind == 3) //클랜아이디
		{

				if (UserDataManager.instance.user.clan_Clans.ClanID != Convert.ToUInt32(_SearchID))
				{
					//확인할 클랜아이디 순차적으로 큐에저장
					UserDataManager.instance.Que_OtherUserIDs.Enqueue(_SearchID);
				}
			
		
		}
		

		netData _net = new netData();

		_net.set_SendData("SearchKnd", _SearchKind);
		_net.set_SendData("SearchID", _SearchID);

		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.SearchUserClan;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion

	#region Protocol : UserNameChange (유저이름바꾸기)
	public static void UserNameChange(string _NkNm, int _Price, del_webResp_0 delfunc)
	{
		//사용하는 제화타입 저장
		nowUseGoods = ITEMTYPE.GEM;

		netData _net = new netData();

		_net.set_SendData("NkNm", _NkNm);
		_net.set_SendData("Price", _Price);

		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.UserNameChange;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion

	#region Protocol : SetOption (옵션변경)
	public static void SetOption(byte _LggCd, byte _ClanPush, del_webResp_0 delfunc)
	{

		netData _net = new netData();

		_net.set_SendData("LggCd", _LggCd);
		_net.set_SendData("ClanPush", _ClanPush);

		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.SetOption;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion

	#region Protocol : SetTutoBit (튜토리얼완료)
	public static void SetTutoBit(byte _BitIdx, del_webResp_0 delfunc)
	{

		netData _net = new netData();

		_net.set_SendData("BitIdx", _BitIdx);

		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.SetTutoBit;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion
	

		
	#region Protocol : GetCooponReward (쿠폰사용하기)
	public static void GetCooponReward(string _CpNum, del_webResp_0 delfunc)
	{

		netData _net = new netData();

		_net.set_SendData("CpNum", _CpNum);

		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.GetCooponReward;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion


	#region Protocol : GetAttendDayRwd (출석체크 보상요청하기)
	public static void GetAttendDayRwd(del_webResp_0 delfunc)
	{
		netData _net = new netData();


		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.GetAttendDayRwd;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion


	#region Protocol : SetWords (문구셋팅 킬문구)
	public static void SetWords(string _Words, del_webResp_0 delfunc)
	{
		netData _net = new netData();

		_net.set_SendData("Words", _Words);

		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.SetWords;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion


	#region Protocol : PostGetList (우편물 리스트 가져오기)
	public static void PostGetList(del_webResp_0 delfunc)
	{
		netData _net = new netData();

		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.PostGetList;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion

	#region Protocol : PostRecv (우편함 받기)
	public static void PostRecv(uint _PostIdx,uint _Month, bool _Accept ,del_webResp_0 delfunc)
	{
		netData _net = new netData();

		_net.set_SendData("PostIdx", _PostIdx);
		_net.set_SendData("Month", _Month);
		_net.set_SendData("Accept", _Accept);


		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.PostRecv;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion


	#region Protocol : SetEquipItem (유닛 아이템착용)
	public static void SetEquipItem(uint _UnitIdx, uint _MainWpnIdx, uint _SubWpnIdx, uint _DecoIdx1, uint _DecoIdx2,uint _DecoIdx3, del_webResp_0 delfunc)
	{
		netData _net = new netData();

		_net.set_SendData("UnitIdx", _UnitIdx);
		_net.set_SendData("MainWpnIdx", _MainWpnIdx);
		_net.set_SendData("SubWpnIdx", _SubWpnIdx);
		_net.set_SendData("DecoIdx1", _DecoIdx1);
		_net.set_SendData("DecoIdx2", _DecoIdx2);
		_net.set_SendData("DecoIdx3", _DecoIdx3);


		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.SetEquipItem;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion

	#region Protocol : SetUnitRef (유닛 강화하기)
	public static void SetUnitRef( uint _UnitIdx, uint _ItemIdx, byte _PriceItTp, uint _PriceItNum, del_webResp_0 delfunc)
	{
		if (_PriceItTp == 1) webRequest.nowUseGoods = ITEMTYPE.GEM;
		else if (_PriceItTp == 2) webRequest.nowUseGoods = ITEMTYPE.GOLD;
		

		netData _net = new netData();

		_net.set_SendData("UnitIdx", _UnitIdx);
		_net.set_SendData("ItemIdx", _ItemIdx);
		_net.set_SendData("PriceItTp", _PriceItTp);
		_net.set_SendData("PriceItNum", _PriceItNum);


		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.SetItemRef;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion

	#region Protocol : SetItBoxUnLock (보급 아이템 박스 해제하기)
	public static void SetItBoxUnLock(int _btSlotIdx, del_webResp_0 delfunc)
	{
		//사용하는 제화타입 저장
		nowUseGoods = ITEMTYPE.GEM;

		netData _net = new netData();

		_net.set_SendData("BtSlotIdx", _btSlotIdx);

		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.SetItBoxUnLock;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion

	#region Protocol : SetItBoxReward (아이템 박스 보상받기)
	public static void SetItBoxReward(int _btSlotIdx , int _Price, del_webResp_0 delfunc)
	{

		//사용하는 제화타입 저장
		nowUseGoods = ITEMTYPE.GEM;

		netData _net = new netData();

		_net.set_SendData("BtSlotIdx", _btSlotIdx);
		_net.set_SendData("Price", _Price);

		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.SetItBoxReward;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion

	#region Protocol : SetAdRwd (광고보고 보상 받기)
	public static void SetAdRwd(del_webResp_0 delfunc)
	{

		netData _net = new netData();

		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.SetAdRwd;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
    #endregion

    #region Protocol : GetAdDoubleRwd (광고보고 배율만큼 더받기)
    public static void GetAdDoubleRwd(AdDoubleRewardType _AdKnd, del_webResp_0 delfunc)
    {

        netData _net = new netData();

        _net.set_SendData("AdKnd", (int)_AdKnd);

        _net.del_result = delfunc;
        _net.protocolName = ProtocolName.GetAdDoubleRwd;

        web_Manager _webMgr = web_Manager.Getsingleton;
        _webMgr.Que_NetData.Enqueue(_net);
        _webMgr.Send_WebProtocol();
    }
    #endregion

    #region Protocol : GameStart
    public static void GameStart(int _btIdx, del_webResp_0 delfunc)
	{
		netData _net = new netData();

		_net.set_SendData("BtIdx", _btIdx);

		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.GameStart;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion


	#region Protocol : GameEnd
	public static void GameEnd(int _endKey, bool _winFlg, del_webResp_0 delfunc)
	{
		netData _net = new netData();

		_net.set_SendData("EndKey", _endKey);
		_net.set_SendData("WinFlg", _winFlg);

		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.GameEnd;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion

	#region Protocol : GameEndRfsData
	public static void GameEndRfsData(del_webResp_0 delfunc)
	{
		netData _net = new netData();

		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.GameEndRfsData;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion

	#region Protocol : ClanMake (클랜 만들기)
	public static void ClanMake(string _ClanName ,del_webResp_0 delfunc)
	{
		//사용하는 제화타입 저장
		nowUseGoods = ITEMTYPE.GEM;

		netData _net = new netData();

		_net.set_SendData("ClanName", _ClanName);


		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.ClanMake;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion

	#region Protocol : ClanJoin(클랜 가입하기)
	public static void ClanJoin(uint _ClanID, del_webResp_0 delfunc)
	{
		//사용하는 제화타입 저장
		nowUseGoods = ITEMTYPE.GOLD;

		netData _net = new netData();

		_net.set_SendData("ClanID", _ClanID);


		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.ClanJoin;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion

	#region Protocol : ClanInfo (클랜 정보 가져오기)
	public static void ClanInfo(del_webResp_0 delfunc)
	{
		netData _net = new netData();

		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.ClanInfo;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion

	#region Protocol : ClanSetBuf (클랜 버프시간 구매)
	public static void ClanSetBuf(byte _BufType, uint _Price, del_webResp_0 delfunc)
	{
		webRequest.nowUseGoods = ITEMTYPE.GEM;

		netData _net = new netData();

		_net.set_SendData("BufType", _BufType);
		_net.set_SendData("Price", _Price);

		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.ClanSetBuf;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion


	

	#region Protocol : ClanGiveUnit (지원요청 유닛 주기)
	public static void ClanGiveUnit(uint _UnitIdx, byte _MemIdx, del_webResp_0 delfunc)
	{
		netData _net = new netData();

		_net.set_SendData("UnitIdx", _UnitIdx);
		_net.set_SendData("MemIdx", _MemIdx);

		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.ClanGiveUnit;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion

	#region Protocol : ClanPersonsCntUp (클랜 인원수 늘리기)
	public static void ClanPersonsCntUp(int _Price,del_webResp_0 delfunc)
	{
		//사용하는 제화타입 저장
		nowUseGoods = ITEMTYPE.GEM;

		netData _net = new netData();

		_net.set_SendData("Price", _Price);

		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.ClanPersonsCntUp;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion

	#region Protocol : ClanNameChange (클랜 이름바꾸기)
	public static void ClanNameChange( int _Price, string _ClanName, del_webResp_0 delfunc)
	{
		//사용하는 제화타입 저장
		nowUseGoods = ITEMTYPE.GEM;

		netData _net = new netData();

		_net.set_SendData("Price", _Price);
		_net.set_SendData("ClanName", _ClanName);

		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.ClanNameChange;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion

	#region Protocol : ClanList (클랜 추천 리스트)
	public static void ClanList(del_webResp_0 delfunc)
	{
		netData _net = new netData();

		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.ClanList;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion

	#region Protocol : ClanOut (클랜 탈퇴 / 강퇴 하기)
	public static void ClanOut(uint _OutUserID, del_webResp_0 delfunc)
	{
		netData _net = new netData();

		_net.set_SendData("OutUserID", _OutUserID);

		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.ClanOut;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion

	#region Protocol : ClanSetStep (클랜원 스텝 지정하기)
	public static void ClanSetStep(byte _MemIdx , byte _CnRank, del_webResp_0 delfunc)
	{
		netData _net = new netData();

		_net.set_SendData("MemIdx", _MemIdx);
		_net.set_SendData("CnRank", _CnRank);


		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.ClanSetStep;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion

	#region Protocol : ClanInvite (클랜 초대 하기)
	public static void ClanInvite(uint _InviteUserID, del_webResp_0 delfunc)
	{
		//사용하는 제화타입 저장
		nowUseGoods = ITEMTYPE.GOLD;

		netData _net = new netData();

		_net.set_SendData("InviteUserID", _InviteUserID);

		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.ClanInvite;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion

	#region Protocol : ClanJoinAccept (클랜 가입 수락)
	public static void ClanJoinAccept(ushort _BodIdx,uint _JoinUserID, del_webResp_0 delfunc)
	{
		netData _net = new netData();

		_net.set_SendData("BodIdx", _BodIdx);
		_net.set_SendData("JoinUserID", _JoinUserID);


		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.ClanJoinAccept;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion

	#region Protocol : ClanMarkChange (클랜 마크변경)
	public static void ClanMarkChange(int _Price , ushort _ClanMark ,del_webResp_0 delfunc)
	{
		//사용하는 제화타입 저장
		nowUseGoods = ITEMTYPE.GEM;

		netData _net = new netData();

		_net.set_SendData("Price", _Price);
		_net.set_SendData("ClanMark", _ClanMark);


		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.ClanMarkChange;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion

	#region Protocol : ClanInfoTxtChange (클랜 소개글변경)
	public static void ClanInfoTxtChange(string _ClanInfoTxt, del_webResp_0 delfunc)
	{
		netData _net = new netData();

		_net.set_SendData("ClanInfoTxt", _ClanInfoTxt);


		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.ClanInfoTxtChange;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion


	#region Protocol : ClanBodWrite (클랜 게시판에 글쓰기)
	public static void ClanBodWrite(string _BodTxt, del_webResp_0 delfunc)
	{
		netData _net = new netData();

		_net.set_SendData("BodTxt", _BodTxt);


		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.ClanBodWrite;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion





	#region Protocol : ShopBuyItem ( 상점에서 아이템 사기->에디터용)
	public static void ShopBuyItem( ushort _ShopIdx, del_webResp_0 delfunc)
	{
		//사용하는 제화타입 저장
		nowUseGoods = ITEMTYPE.GEM;

		netData _net = new netData();

		_net.set_SendData("ShopIdx", _ShopIdx);

		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.ShopBuyItem;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion

	#region Protocol : ShopBuyItem ( 상점에서 아이템 사기 -> 모바일용)
	public static void ShopBuyItem(ushort _ShopIdx, string _Payment, string _Signature, del_webResp_0 delfunc)
	{
		//사용하는 제화타입 저장
		nowUseGoods = ITEMTYPE.GEM;

		netData _net = new netData();

		_net.set_SendData("ShopIdx", _ShopIdx);
		_net.set_SendData("Payment", _Payment);
		_net.set_SendData("Signature", _Signature);


		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.ShopBuyItem;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion

	#region Protocol : ShopBuyUnit ( 유닛 구매 )
	public static void ShopBuyUnit(uint _UnitIdx, uint _Price, del_webResp_0 delfunc)
	{
		//사용하는 제화타입 저장
		nowUseGoods = ITEMTYPE.GEM;

		netData _net = new netData();

		_net.set_SendData("UnitIdx", _UnitIdx);
		_net.set_SendData("Price", _Price);


		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.ShopBuyUnit;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion

	#region Protocol : ShopBuyWeapon ( 무기 구매 )
	public static void ShopBuyweapon(uint _UnitIdx, uint _WpnIdx, uint _Price, del_webResp_0 delfunc)
	{
		//사용하는 제화타입 저장
		nowUseGoods = ITEMTYPE.GEM;

		netData _net = new netData();

		_net.set_SendData("UnitIdx", _UnitIdx);
		_net.set_SendData("WpnIdx", _WpnIdx);
		_net.set_SendData("Price", _Price);


		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.ShopBuyWeapon;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion

	#region Protocol : ShopBuyDeco ( 데코구매)
	public static void ShopBuyDeco(uint _UnitIdx, uint _DecoIdx, uint _Price, del_webResp_0 delfunc)
	{
		//사용하는 제화타입 저장
		nowUseGoods = ITEMTYPE.GEM;

		netData _net = new netData();

		_net.set_SendData("UnitIdx", _UnitIdx);
		_net.set_SendData("DecoIdx", _DecoIdx);
		_net.set_SendData("Price", _Price);


		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.ShopBuyDeco;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion

	#region Protocol : ShopBuySkill ( 스킬구입)
	public static void ShopBuySkill(uint _UnitIdx,  uint _Price, del_webResp_0 delfunc)
	{
		//사용하는 제화타입 저장
		nowUseGoods = ITEMTYPE.GEM;

		netData _net = new netData();

		_net.set_SendData("UnitIdx", _UnitIdx);
		_net.set_SendData("Price", _Price);


		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.ShopBuySkill;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion


	#region Protocol : BuySkillCommit ( 구입스킬적용)
	public static void BuySkillCommit(uint _UnitIdx, byte _SubSkill, bool _Commit, del_webResp_0 delfunc)
	{
		netData _net = new netData();

		_net.set_SendData("UnitIdx", _UnitIdx);
		_net.set_SendData("SubSkill", _SubSkill);
		_net.set_SendData("Commit", _Commit);


		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.BuySkillCommit;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion


	#region Protocol : FriendList ( 친구리스트)
	public static void FriendList(del_webResp_0 delfunc)
	{
		netData _net = new netData();


		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.FriendList;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion

	#region Protocol : FriendGetInviteUrl ( 친구 초대 url 보내기)
	public static void FriendGetInviteUrl(del_webResp_0 delfunc)
	{
		netData _net = new netData();


		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.FriendGetInviteUrl;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion

	#region Protocol : FriendAdd ( 친구추가)
	public static void FriendAdd(uint _FrUserID ,string _FrToken , del_webResp_0 delfunc)
	{
		netData _net = new netData();

		_net.set_SendData("FrUserID", _FrUserID);
		_net.set_SendData("FrToken", _FrToken);

		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.FriendAdd;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion

	#region Protocol : FriendRemove ( 친구삭제)
	public static void FriendRemove(uint _FrUserID, del_webResp_0 delfunc)
	{
		netData _net = new netData();

		_net.set_SendData("FrUserID", _FrUserID);

		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.FriendRemove;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion

	#region Protocol : FriendPostInvite ( 우편함으로 친구 요청하기)
	public static void FriendPostInvite(uint _InviteUserID, del_webResp_0 delfunc)
	{
		netData _net = new netData();

		_net.set_SendData("InviteUserID", _InviteUserID);

		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.FriendPostInvite;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion


	#region Protocol : UnitExpRanking ( 유닛랭킹보기)
	public static void UnitExpRanking(uint _UnitIdx ,del_webResp_0 delfunc)
	{
		//현재 검색한 유닛인덱스
		nowSearchRankingUnitIdx = _UnitIdx;

		netData _net = new netData();

		_net.set_SendData("UnitIdx", _UnitIdx);

		_net.del_result = delfunc;
		_net.protocolName = ProtocolName.UnitExpRanking;

		web_Manager _webMgr = web_Manager.Getsingleton;
		_webMgr.Que_NetData.Enqueue(_net);
		_webMgr.Send_WebProtocol();
	}
	#endregion





	#region method : newIdxLst (리스트 만들기)
	public static List<int> newIdxLst(int count)
	{
		List<int> _IdxLst = new List<int>();
		for (int i = 1; i < count+1; i++)
			_IdxLst.Add(i);

		return _IdxLst;
	}
	#endregion
}
