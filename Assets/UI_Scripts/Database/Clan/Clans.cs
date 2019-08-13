using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Clans
{
	public uint ClanID;				//클랜아이디
	public ushort ClanMark;			//클랜마크
	public string ClanName;			//클랜명
	public string MasterNm;			//마스터닉네임
	public string ClanInfoTxt;			//클랜 소개글
	public byte MemberCnt;				//가입자수
	public byte PersonCnt;				//수용인원
	public byte JoinRule;				//클랜가입방식
	public DateTime GoldBufTm;			//골드버프종료시간
	public DateTime ExpBufTm;			//경험치버프종료시간
	public DateTime mtime;				//변경된시간
	public DateTime ctime;				//생성된 시간


	public void Init()
	{
		ClanID = 0;
		ClanMark = 0;
		ClanName = "";
		MasterNm = "";
		ClanInfoTxt = "";
		MemberCnt = 0;
		PersonCnt = 0;
		JoinRule = 0;
		GoldBufTm = DateTime.MinValue;
		ExpBufTm = DateTime.MinValue;
		mtime = DateTime.MinValue;
		ctime = DateTime.MinValue;
	}

}