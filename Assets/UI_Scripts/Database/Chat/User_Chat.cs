using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class User_Chat 
{
	public uint UesrID;					//유저아이디
	public ushort ClanMark;				//클랜마크
	public string NkNm;					//유저닉네임
	public ChatMessageType msgTp;			//채팅 메세지 종류
	public uint ChennelIdx;				//채널인덱스
	public string chatMsg;					//채팅 메세지 내용
	public string WhsNkNm;					//귓속말 할 닉네임
	public string WhsMsg;					//귓속말 내용
	public byte ChkUserNum;				//체크 유저수
	public bool LoginStatus;				//접속 여부 (true : 접속 , false : 로그아웃)

	public void Init()
	{
		UesrID = 0;
		ClanMark = 0;
		NkNm = "";
		msgTp = ChatMessageType.NONE;
		ChennelIdx = 0;
		chatMsg = "";
		WhsNkNm = "";
		WhsMsg = "";
		ChkUserNum = 0;
		LoginStatus = false;
	}
	
	public void AppyChatInfo (ref User_Chat _userChat)
	{
		_userChat.UesrID = UesrID;
		_userChat.ClanMark = ClanMark;
		_userChat.NkNm = NkNm;
		_userChat.msgTp = msgTp;
		_userChat.ChennelIdx = ChennelIdx;
		_userChat.chatMsg = chatMsg;
		_userChat.WhsNkNm = WhsNkNm;
		_userChat.WhsMsg = WhsMsg;
		_userChat.ChkUserNum = ChkUserNum;
		_userChat.LoginStatus = LoginStatus;
	}

}
