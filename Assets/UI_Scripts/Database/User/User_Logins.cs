using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class User_Logins 
{

	//=====
	public uint UserID;				// 유저아이디
	public byte MktIdx;				//마켓인덱스
	public LanguageCode LggCd;			//언어코드
	public byte ClanPush;				//클랜푸쉬 = 1 : on , 0 : off
	public byte PlfID;				//플랫폼 인덱스
	public uint IpIdx;				//IP 국가 데이터인덱스
	public UInt64 KeyTime;				// 앱가드 키 시간값


	public string NkNm;				// 닉네임
	public int Ver;					// 버젼
	public string Lgnkey;				// 로그인 키값 = 구글/페이스북 에서 받은 키값
	public string LgnToken;			// 로그인 토큰값
	public string Email= "";			// 가입플랫폼 이메일
	public DateTime mtime;				//변경된시간
	public DateTime ctime;				//생성된 시간
}
