using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class infos_unit
{
	public uint UnitIdx;			//캐릭터인덱스
	public string UnitName;		//캐릭터이름
	public byte SkillKind;			//캐릭터스킬
	public float Hp;				//체력
	public float MvSpeed;			//이동속도
	public ushort RldSpeed;		//재장전속도
	public ITEMTYPE SellItTp;		//판매아이템 타입
	public uint SellItNum;			//판매아이템갯수
	//public int AtkMin;			//최소공격력
	//public int AtkMax;			//최대공격력
	//public int AtkRange;			//공격거리
	//public int AimInit;			//초기정확도
	//public int AtkSpeed;			//공격속도(초당)
	//public int AimCtrl;			//조준반동제어
	//public int Magazine;			//탄창용량
	//public float Critical;			//크리티컬(확률)
	//public ushort GunReload;			//재장전
	//public int League;			//등장리그
	//public DateTime mtime;			//변경된시간
	//public DateTime ctime;			//생성된시간


	public infos_unit()
	{


	}

	public infos_unit(infos_unit _infosUnit)
	{

		UnitIdx = _infosUnit.UnitIdx;
		UnitName = _infosUnit.UnitName;
		SkillKind = _infosUnit.SkillKind;
		Hp = _infosUnit.Hp;
		MvSpeed = _infosUnit.MvSpeed;
		SellItTp = _infosUnit.SellItTp;
		SellItNum = _infosUnit.SellItNum;
	}
}
