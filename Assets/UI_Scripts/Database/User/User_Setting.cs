using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User_Setting
{
	public bool Notice_msg;						//공지관련알림 (true : 켜기 , false : 끄기)
	public bool Clan_msg;							//클랜관련알림 (true : 켜기 , false : 끄기)
	public bool UnlockBx_msg;						//상자해체관련알림 (true : 켜기 , false : 끄기)
	public bool GraphicQuality;					//그래픽표현 (true : 저사양 , false : 고야상)
	public bool AttackType;						//공격방식 (true : 자동 , false : 수동)
	public int Sensitive;							//시선감도 (최대치 100)
	public int VolumBGM;							//배경음량 (최대치100)
	public int VolumVoice;							//보이스음량(최대치100)
	public int VolumEffect;						//효과음량(최대치100)
	public int usingLangueage;						// 사용중인 언어


	public User_Setting()
	{	

		Notice_msg = true;
		Clan_msg = true;
		UnlockBx_msg = true;
		GraphicQuality = true;
		AttackType = true;
		Sensitive = 100;
		VolumBGM = 100;
		VolumVoice = 100;
		VolumEffect =100;
		usingLangueage =0; 
	}
}
