using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct User_Users
{
	public struct ConnectAdress
	{
		public string continent_Code;
		public string continent_Name;
		public string continent_iso_Code;
	}

	public uint UserID;									// 유저아이디
	public string NkNm;									// 닉네임
    public ushort UserLv;                               //유저레벨
	public ushort ClanMark;								//클랜마크
	public string ClanName;								//클랜명
	public int Ver;										// 버젼
	public string CtrCd;									//국가코드
	public string Lgnkey ;								// 로그인 키값 = 구글/페이스북 에서 받은 키값
	public string LgnToken;							// 로그인 토큰값
	public byte PlfID;								//플랫폼 아이디
	public DateTime mtime;								//변경된시간
	public DateTime ctime;								//생성된 시간
	public ConnectAdress adress;						// 지역



	public void Init()
	{
		UserID = 0;
		ClanMark = 0;
		Ver = 0;
        UserLv = 0;
        NkNm = "";
		ClanName = "";
		CtrCd = "";
		Lgnkey = "";
		LgnToken = "";
		mtime = DateTime.MinValue;
		ctime = DateTime.MinValue;
		adress = new ConnectAdress();
	}

	
}
