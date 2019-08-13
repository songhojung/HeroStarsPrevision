using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum NETKIND
{
	//====================================== 서버 공용 프로토콜 =========================================
	NONE = 0,

	//byte	(enum ErrorCode 참조)
	STC_ERROR_CODE,

	// 전달값 없음
	CTS_CONNECTION_RETENTION,   //네트워크 연결 유지 패킷
	STC_CONNECTION_RETENTION,

	//전달값 없음
	CTS_SERVER_TIME,        //서버 시간 요청
	//long : 현재 서버 시간 
	STC_SERVER_TIME,


	//========================================= 배틀 서버 ============================================

	///uint:웹 유저 ID
	///string:닉네임
	///ushort : 버전
	///string : KILL 문구
	///byte[2] : 국가코드
	///ushort : 클랜마크
	///byte : 언어코드
	CTS_LOGIN = 100,    //로그인 프로토콜

	// 전달값 없음
	STC_LOGIN,

	//byte : 전투 종류 (BattleKind)
	//int : 유닛 인덱스
	//byte : 유닛 강화도
	//int : 메인무기
	//int : 보조무기
	//int : 치장1 인덱스
	//int : 치장2 인덱스
	//int : 치장3 인덱스
	//byte : 서브 스킬
	CTS_QUICK_JOIN, //퀵조인

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
	STC_BATTLEROOM_INFO,    //배틀 방 정보

	//(대기방이 시작으로 변경될시 한번 전달한다)
	//전달 내용은 STC_BATTLEROOM_INFO 프로토콜과 동일 하다
	STC_BATTLEROOM_START_INFO,  //배틀 시작 방 정보 

	// byte : 유저수
	//		uint : 방 유저 WebUserID
	//		string : KILL 문구
	STC_KILL_WORDS,     //KILL 문구

	//게임 시작
	CTS_GAME_START,
	STC_GAME_START,

	//int : 유닛 아이디
	//byte : 유닛 강화도
	//int : 메인무기
	//int : 보조무기
	//int : 치장1 인덱스
	//int : 치장2 인덱스
	//int : 치장3 인덱스
	//byte : 서브 스킬
	CTS_UNIT_CHANGE,

	//게임 데이터 중계 
	//(받은 데이터 본인 제외, 방 유저들에게 받는 즉시 그대로 전달)
	//(데이터 안에 누가 쐈는지도 포함 되어있어야한다)
	//(서버가 데이터를 가공하지 않기 때문에 포함하지 않는다면 누가 쐇는지 알수가 없다)
	CTS_GAMEDATA_RELAY,         //게임 데이터 중계 
	STC_GAMEDATA_RELAY,

	// uint : 공격 유저 WebUserID
	// uint : 맞은 유저 WebUserID
	// bool  : 크리티컬 true, false
	// bool  : 헤드샷 
	// float : 데미지량
	// Vector3 : 쏜 좌표
	// Vector3 : 맞은 좌표
	// byte : 공격한 총 타입
	CTS_SET_USER_ATKDMG,    //유저 공격

	//자살
	CTS_SET_SUICIDE,

	// uint : 공격 유저 WebUserID
	// uint : 맞은 유저 WebUserID
	// float : 오브젝트 체력
	// Vector3 : 쏜 좌표
	// Vector3 : 맞은 좌표
	// byte : 공격한 총 타입
	// bool  : 크리티컬 true, false
	// float : 데미지량
	STC_SET_HP,     //체력 셋팅

	// float : 데미지량 
	CTS_SET_ATK_MAINBASE,   //진지전 적 진지 공격시

	// float : 최대 체력
	// float : Red팀 진지 체력
	// float : Blue팀 진지 체력
	STC_MAINBASE_HP,		//진지 HP 갱신

	// uint : 부활 WebUserID
	//int : 유닛 아이디
	//byte : 유닛 강화도
	//int : 메인무기
	//int : 보조무기
	// float : 오브젝트 체력
	// byte : 시작(부활 + 입장) 포인트
	STC_REVIVAL,        //오브젝트 부활

	// uint : 공격 유저 WebUserID
	// uint : 죽은 유저 WebUserID
	// bool  : 헤드샷 
	// ushort : Red팀 킬 카운트 (Red:0)
	// ushort : Blue팀 킬 카운트 (Blue:1)
	// ushort : Red팀 데스 카운트 (Red:0)
	// ushort : Blue팀 데스 카운트 (Blue:1)
	// long : 유저 부활 시간
	// ushort : 죽은 유저 킬
	// ushort : 죽은 유저 데스
	STC_SET_USERKILL,       //유저 킬

	// uint : 나간 UserID 
	STC_ROOM_OUT_USER,      //방에서 나간 유저

	//	byte : 승리팀 (Red:0, Blue:1, Draw:2)
	STC_GAME_TIMEOVER,  //소켓 게임 시간 종료 (일단 게임 정지 시킨다, 보상 처리 전) -- 현재 게임시간 3분으로 셋팅됨, 

	// 전달값 없음
	STC_GAME_REWRDEND,  //게임 시간 종료 (보상 지급 완료 된 상태)

	//연결 유지 시간 초과
	STC_CONNECT_TIMEOVER,

	//게임 시간
	// long : 서버 현재 시간
	// long : 종료 예정 시간
	CTS_GAME_ROOM_TIME,
	STC_GAME_ROOM_TIME,

	//게임중 점수
	CTS_GAME_SCORE,

	// ushort : Red팀 킬 카운트 (Red:0)
	// ushort : Blue팀 킬 카운트 (Blue:1)
	// ushort : Red팀 데스 카운트 (Red:0)
	// ushort : Blue팀 데스 카운트 (Blue:1)
	// byte : 유저수
	//		uint : 방 유저 WebUserID 
	//		ushort : 킬수
	//		ushort : 데스수
	STC_GAME_SCORE,

	//방나가기
	CTS_BATTLEROOM_OUT,

	//========================================= 방만들기 ============================================

	//비공개 방 만들기
	//int : 유닛 인덱스
	//byte : 유닛 강화도
	//int : 메인무기
	//int : 보조무기
	//int : 치장1 인덱스
	//int : 치장2 인덱스
	//int : 치장3 인덱스
	//byte : 서브 스킬
	CTS_ROOM_MAKE,
	STC_ROOM_MAKE,

	//공개,비공개방으로 변경
	//bool : (true : 비공개, false : 공개)
	CTS_OPEN_ROOM,
	//bool : (true : 비공개, false : 공개)
	STC_OPEN_ROOM,

	//초대 보내기
	//uint : 초대자 UserID
	//string : 초대자 닉네임
	CTS_INVITE_ROOM,
	//초대 받기
	//uint : 초대자 UserID
	//string : 초대자 닉네임
	STC_INVITE_ROOM,
	//초대 답장
	//uint : 초대자 UsreID
	//bool : true : 초대 수락, false : 초대 거절
	//int : 유닛 인덱스
	//byte : 유닛 강화도
	//int : 메인무기
	//int : 보조무기
	//int : 치장1 인덱스
	//int : 치장2 인덱스
	//int : 치장3 인덱스
	//byte : 서브 스킬
	CTS_INVITE_ROOM_ANSWER,

	//팀 이동
	//byte : 이동할 팀
	//byte : 슬롯 인덱스 1~12, 0으로 위의 팀에 빈슬롯 있는곳에 랜덤 배정
	CTS_TEAM_MOVE,

	//방나가기
	CTS_ROOM_OUT,

	// uint : 방장 UserID
	// byte : 맵 인덱스
	// bool : (true : 비공개, false : 공개)
	// byte : 유저수
	//		uint : 방 유저 WebUserID 
	//		byte : 방 슬롯 위치 1~12
	//		string : 닉네임
	//		byte : 팀 (Red:0, Blue:1, Draw:2)
	//		ushort : 클랜 마크
	///		byte[2] : 국가코드
	STC_READYROOM_INFO, //대기 방 정보

	//맵 변경
	//byte  : 변경 맵 인덱스 (0:랜덤, n:지정)
	CTS_MAP_CHANGE,

	//대기방 게임 시작 
	CTS_READYROOM_START,

	//함께 하기
	//uint : 방 들어갈 친구 UserID
	//int : 유닛 인덱스
	//byte : 유닛 강화도
	//int : 메인무기
	//int : 보조무기
	//int : 치장1 인덱스
	//int : 치장2 인덱스
	//int : 치장3 인덱스
	//byte : 서브 스킬
	CTS_FRIEND_ROOM_JOIN,

	//byte : 참여 실패 사유
	//  0:성공
	//	1:친구가 접속중이지 않음
	//	2:친구가 방에 들어가 있지 않음
	//	3:방 입장 유저수 초과
	//byte : 성공시 방상태값 들어감 (0:대기방, n:게임중)
	STC_FRIEND_ROOM_JOIN,

	//킬 메세지 변경 셋팅
	///string : KILL 문구
	CTS_SET_KILL_WORDS,

	//========================================= 채팅 서버 ============================================

	// <<<< 채팅 >>>> 
	//byte : 종류 (1:방 채팅)
	//string : 메세지 내용
	CTS_CHAT_MESSAGE,

	//byte : 종류 (1:방 채팅)
	//uint : WebUserID 유저아이디
	//ushort : 클랜 마크
	//string : 닉네임
	//string : 메세지 내용
	STC_CHAT_MESSAGE,

	//========================================= 추가 프로토콜 ============================================
	//byte : 즉시 부활 종류
	CTS_INSTANT_REVIVAL,        //오브젝트 즉시 부활

	//int : 골드 재화 보유량
	//int : 보석 재화 보유량
	//byte : 즉시 부활 종류
	STC_INSTANT_REVIVAL,        //오브젝트 즉시 부활

	//uint : 접속 유저 WebUserID
	STC_ROOM_JOIN_NOTICE,			//유저 접속 알림

	//string : 공지 메세지
	STC_TOAST_MSG,		//채팅 공지 사항 알림

	//string : 공지 메세지 
	//bool : true(확인 터치시 어플종료), false(팝업만 띄움)
	STC_POPUP_MSG,      //팝업 공지 사항 알림

	//int : 유닛 인덱스
	CTS_ALONE_PLAY_KILL,      //트레이닝 모드 에서 킬 했을경우

}