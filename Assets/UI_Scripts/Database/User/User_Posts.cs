using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class User_Posts 
{
	public uint UserID;			//유저아이디
	public uint PostIdx;			//우편함인덱스
	public uint SndUserID;			//보낸유저아이디
	public uint SndClanID;			//보낸클랜아이디
	public string SndNkNm;			//보낸유저닉네임
	public byte GetFlg;			//우편함받은유무 (1:받음 , 2 : 읽음)
	public byte PstPasIdx;			//우편함 문구 종류 (1:클랜유닛지원, 2: 클랜가입초대메세지 , 3:운영자선물발송)
	public ITEMTYPE ItTp;			//아이템타입
	public uint ItIdx;			//아아팀인덱스
	public string PostTxt;			//우편함 문구 (클랜가입 초대시 클랜명이 들어간다)
	public uint ItNum;			//아이템갯수
	public byte ExpiryDay;			//수령가능시간 
	public DateTime mtime;			//변경된시간
	public DateTime ctime;			//생성된시간

	
}
