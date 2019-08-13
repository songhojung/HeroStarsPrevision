using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class User_RoomInfo
{
	//방정보쪽
	public uint RoomMsterUserID;				//방장 ID
	public byte MapIndex;						// 맵인덱스
	public byte PersonCnt;						//유저수
	public bool isCloseRoom;					// 공개방? 비공개방?
	public uint roomUserID;					//방 유저 ID
	public string roomUserNkNm;				//방 유저 닉네임
	public byte roomUserTeam;					//팀(red : 0 , blue :1 , draw : 2)
	public byte roomUserSlot;					//방 슬롯 위치 1~12
	public ushort roomUserClanMark;				//방유저 클랜마크
	public string roomUserFlag;				//방유저 나라코드


	//초대쪽
	public uint InviterUserID;					//초대자 ID
	public string InviterUserNkNm;				//초대자 닉네임


	//같이하기 쪽
	public byte JoinResult;					//참여결과 (0 :성공 , 1:친구가접속아님, 2:친구가 방에들어가지않음, 3: 방 입장유저초과)
	public byte JoinSuccessState;				//성공시 방상태값

	public void Init()
	{
		RoomMsterUserID= 0;
		MapIndex =0;
		PersonCnt =0;
		isCloseRoom = true;
		roomUserID = 0;
		roomUserNkNm = string.Empty;
		roomUserTeam = 0;
		roomUserClanMark = 0;
		InviterUserID = 0;
		InviterUserNkNm = string.Empty;
		roomUserFlag = string.Empty;
		JoinResult = 0;
		JoinSuccessState = 0;
	}

}
