using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Infos_Weapon
{
	public uint WpnIdx;				//무기인덱스
	public string WpnName;				//무기이름
	public WEAPONE_TYPE WpnType;		//무기종류
	public EQUIPPART_TYPE WpnPart;		//무기장착부위
	public int AtkMin;				//최소공격력
	public int AtkMax;				//최대공격력
	public int AtkRange;				//공격거리
	public int AimInit;				//초기정확도
	public int AtkSpeed;				//공격속도(초당)
	public int AimCtrl;				//조준반동제어
	public int Magazine;				//탄창용량
	public float Critical;				//크리티컬(확률)
	public ushort GunReload;			//재장전
	public byte ZoomScale;				//확대배율
	public ITEMTYPE SellItTp;			//판매아이템 타입
	public uint SellItNum;				//판매아이템갯수
	public byte SortIdx;				//정렬순서
	public byte NewFlg;				//새로운 총기아이콘표시
	public DateTime mtime;				//변경된시간
	public DateTime ctime;				//생성된시간

	public Infos_Weapon()
	{}

	public Infos_Weapon(Infos_Weapon _Infos_Weapon)
	{

		WpnIdx = _Infos_Weapon.WpnIdx;
		WpnName = _Infos_Weapon.WpnName;
		WpnType = _Infos_Weapon.WpnType;
		WpnPart = _Infos_Weapon.WpnPart;
		AtkMin = _Infos_Weapon.AtkMin;
		AtkMax = _Infos_Weapon.AtkMax;
		AtkRange = _Infos_Weapon.AtkRange;
		AimInit = _Infos_Weapon.AimInit;
		AtkSpeed = _Infos_Weapon.AtkSpeed;
		AimCtrl = _Infos_Weapon.AimCtrl;
		Magazine = _Infos_Weapon.Magazine;
		Critical = _Infos_Weapon.Critical;
		GunReload = _Infos_Weapon.GunReload;
		ZoomScale = _Infos_Weapon.ZoomScale;
		mtime = _Infos_Weapon.mtime;
		ctime = _Infos_Weapon.ctime;
		SellItTp = _Infos_Weapon.SellItTp;
		SellItNum = _Infos_Weapon.SellItNum;
	}
}