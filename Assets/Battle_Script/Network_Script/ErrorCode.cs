using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum ErrorCode
{
	OK = 0,
	LOGIN_OVERLAP_MATCH,		//매치 서버 중복 로그인
	LOGIN_OVERLAP_BATTLE,		//배틀 서버 중복 로그인
	LOGIN_OVERLAP_CHATTING,		//채팅 서버 중복 로그인
	WHISPER_USER_IS_NULL,		//귓속말 대상이 접속이지 않습니다.
	USER_IS_NULL,				//대상이 접속중이지 않습니다.
	MATCH_ROOM_IS_NULL,			//존재 하지 않는 매칭 방입니다.
	CLAN_IS_ALONE,				//클랜에 혼자 있는데 클랜 매칭 들어올경우
	ROOM_IS_FULL,				//풀방일 경우
	ROOM_IS_NULL,				//방이 존재하지 않습니다.
}
