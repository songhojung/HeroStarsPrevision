using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Clan_Lists 
{
	public uint ClanID;				//클랜아이디
	public ushort ClanMark;			//클랜마크
	public string ClanName;			//클랜명
	public string NkNm;				//마스터닉네임
	public string CtrCd;				//국가코드
	public byte MemberCnt;				//현재인원
	public byte PersonCnt;				//수용인원
	public ushort CnPointSum0;			//클랜합산점수 0
	public ushort CnPointSum1;			//클랜합산점수 1



	public void Init()
	{
		ClanID = 0;
		ClanMark = 0;
		ClanName = "";
		NkNm = "";
		CtrCd = "";
		MemberCnt = 0;
		PersonCnt = 0;
		CnPointSum0 = 0;
		CnPointSum1 = 0;
	}

}
