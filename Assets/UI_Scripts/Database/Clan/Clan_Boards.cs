using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Clan_Boards
{
	public uint ClanID;			//클랜 아이디
	public ushort BodIdx;			// 게시판 인덱스
	public byte BodKind;			//게시판 종류 (1:가입신청 , 2: 가입로그 , 3:탈퇴로그, 4:추방로그 , 5: 골드버프구매, 6: 경험치버프 구매 ,7: 유저게시글)
	public uint UsetID;			//유저아이디
	public string NkNm;			//닉네임
	public string BodTxt;			//유저 게시글
	public DateTime mtime;			//변경된시간
	public DateTime ctime;			//생성된 시간
	
}
