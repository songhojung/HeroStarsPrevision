using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Clan_members
{
	public uint ClanID;			//클랜아이디
	public byte MemIdx;			//멤버인덱스
	public byte CnRank;			//클랜직급 (0:일반 , 1:스텝,2:마스터 )
	public uint UserID;         //유저아이디
    public ushort UserLv;         //유저레벨
    public string NkNm;			//닉네임
	public byte SvIdx;			//접속 서버 (0: 접속안함, 1~n: 접속서버인덱스)
	public string CtrCd;			//국가 정보

	public DateTime JoinTime;		//클랜 가입 시간
	public DateTime mtime;			//변경된시간
	public DateTime ctime;			//생성된시간


	public void Init()
	{
		
		ClanID = 0;
		MemIdx = 0;
		CnRank= 0;
		UserID = 0;
		NkNm= "";
		CtrCd = "";
		JoinTime = DateTime.MinValue;	
		mtime = DateTime.MinValue;
		ctime = DateTime.MinValue;
	}

}