using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Server_infos 
{
	public byte SvIdx;				//서버인덱스
	public byte MktIdx;				//마켓인덱스
	public byte SvStatus;				//서버상태
	public string SvChkMsg;			//서버점검메세지
	public DateTime SvChkTm;			//서버점검시간
	public string WebSvIp;				//게임 웹 서버 IP주소
	public DateTime ServerTime;		// 서버 현재 시간
	public DateTime mtime;				//변경된 시간
	public DateTime ctime;				//생성된 시간



	public void Init()
	{
		SvIdx = 0;
		MktIdx = 0;
		SvStatus = 0;
		SvChkMsg = "";
		SvChkTm = DateTime.MinValue;
		WebSvIp = "";
		ServerTime = DateTime.MinValue;
		mtime = DateTime.MinValue;
		ctime = DateTime.MinValue;
	
	}
}
