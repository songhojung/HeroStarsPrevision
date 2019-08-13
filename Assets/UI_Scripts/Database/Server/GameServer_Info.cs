using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameServer_Info 
{
	public byte SubIdx;			// 서버인덱스
	public string ServerName;		//서버 이름
	public string PubIp;			// IP주소
	public ushort CnPort;			// 접속 Port
	public byte ConPer;			// 접속량 게이지 

	

	public void Init()
	{
		SubIdx = 0;
		ServerName = string .Empty;
		PubIp = string.Empty;
		CnPort = 0;
		ConPer = 0;		
	}
}
