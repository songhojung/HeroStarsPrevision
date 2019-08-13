using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class User_Clans
{
	public uint UserID;			//유저아이디
	public uint ClanID;			//클랜아이디
	public byte MemIdx;			//멤버인덱스
	public byte RwdIdx;			//보상인덱스 (clanbattle_switch 테이블 RwdIdx 와 다를경우 보상 받을수 있음)
	public DateTime JoinTmLm;		//가입제한시간
	public DateTime mtime;			//변경된시간
	public DateTime ctime;			//생성된시간


	public void Init()
	{
		UserID = 0;
		ClanID = 0;
		MemIdx = 0;
		RwdIdx = 0;
		JoinTmLm = DateTime.MinValue;
		mtime = DateTime.MinValue;
		ctime = DateTime.MinValue;
	}
}
