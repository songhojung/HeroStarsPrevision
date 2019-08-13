using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class User_Friends 
{
	public uint UserID;				//유저아이디
	public uint FrUserID;				// 친구유저 아이디
	public byte FrIdx;				// 친구 인덱스
	public string NkNm;				// 친구유저 닉네임
	public ushort ClanMark;			// 클랜마크
	public string ClanName;			// 클랜이름
	public byte SvIdx;				//접속 서버 (0: 접속안함, 1~n: 접속서버인덱스)
	public string CtrCd;				//국가코드
	public DateTime mtime;				// 변경된시간
	public DateTime ctime;				//생성된 시간
	

	public void Init()
	{
		UserID = 0;
		FrIdx = 0;
		FrUserID = 0;
		SvIdx = 0;
		CtrCd = "";
		mtime = DateTime.MinValue;
		ctime = DateTime.MinValue;	
	}

}
