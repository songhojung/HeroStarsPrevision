using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//전투 종류
public enum BattleKind
{
	NONE = 0,
	NORMAL,         //일반전
	BEGINNER_TEAM_BATTLE,		//초보자 팀 배틀
	SINGLE_BATTLE,			//개인전
	WAR_OF_POSITION,  //진지전
	ALONE_PLAY_BATTLE,				//싱글 모드
}

//셋트 버프 효과
public enum SetBufKnd
{
	NONE = 0,
	ATK_UP = 1,	//공격력 증가
	HP_UP = 2,	//체력 증가
	ATK_SKILL_DMG_UP = 3,	//공격형 스킬 위력 증가
	DMG_DECLINE = 4,		//데미지 감소
}