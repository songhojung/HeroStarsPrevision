using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Infos_Skill
{
	public byte UnitSkill;				//스킬인덱스
	public SKILL_TYPE SkillKnd;		//스킬종류
	public ushort SkillVal;			//스킬값
	public ushort RndPer;				//스킬뽑기 랜덤확률
	public string SkillName;			//스킬이름
	public float CoolTime;			//쿨타임
	public float RunningTime;			//지속시간
	public DateTime mtime;				//변경된시간
	public DateTime ctime;				//생성된 시간

}
