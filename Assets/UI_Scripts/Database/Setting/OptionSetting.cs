using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OptionSetting 
{
	public static OptionSetting instance = new OptionSetting();

	public OptionSetting()
	{
		//Debug.Log("Init => vSyncCount : " + QualitySettings.vSyncCount + " targetFrameRate : " + Application.targetFrameRate);

		Get_SettingValues();
	}

	public bool Notice_pushMsg;						//공지관련푸쉬알림 (true : 켜기 , false : 끄기)
	public bool Clan_pushMsg;							//클랜관련알림 (true : 켜기 , false : 끄기)
	public bool UnlockBx_pushMsg;						//상자해체관련알림 (true : 켜기 , false : 끄기)
	public bool GraphicQuality;					//그래픽표현 (true : 저사양 , false : 고야상)
	public bool AttackType;						//공격방식 (true : 자동 , false : 수동)
	public bool FramePerSecond;					//fps (true : 30fps , false : 60fps)
	public bool AutoBattle;						//자동전투 (true : 자동 , false : 수동)
	public int Sensitive;							//시선감도 (최대치 100)
	public int SensitiveZoomIn;					//줌인시선감도(최대치 100)
	public int VolumBGM							//배경음량 (최대치100)
	{
		get
		{
			return volumeBgm;
		}
		set
		{
			volumeBgm = value;
			SoundExecuter.Getsingleton.ChangeMusicVolume(volumeBgm / 100f);
		}
	}
	public int volumeBgm;				
	public int VolumVoice;							//보이스음량(최대치100)
	public int VolumEffect;							//효과음량(최대치100)
	public int usingLangueage;						// 사용중인 언어




	public void Save_SettingValues()
	{

		PlayerPrefs.SetInt(DefineKey.setting_NoticeMsg, Convert.ToInt16(Notice_pushMsg));
		PlayerPrefs.SetInt(DefineKey.setting_ClanMsg, Convert.ToInt16(Clan_pushMsg));
		PlayerPrefs.SetInt(DefineKey.setting_UnlockBxMsg, Convert.ToInt16(UnlockBx_pushMsg));
		PlayerPrefs.SetInt(DefineKey.setting_Quality, Convert.ToInt16(GraphicQuality));
		PlayerPrefs.SetInt(DefineKey.setting_AttackType, Convert.ToInt16(AttackType));
		PlayerPrefs.SetInt(DefineKey.setting_FPS, Convert.ToInt16(FramePerSecond));
		PlayerPrefs.SetInt(DefineKey.setting_AutoBattle, Convert.ToInt16(AutoBattle));
		PlayerPrefs.SetInt(DefineKey.setting_Sensitive, Convert.ToInt16(Sensitive));
		PlayerPrefs.SetInt(DefineKey.setting_SensitiveZoomIn, Convert.ToInt16(SensitiveZoomIn));
		PlayerPrefs.SetInt(DefineKey.setting_VolumBGM, Convert.ToInt16(VolumBGM));
		PlayerPrefs.SetInt(DefineKey.setting_VolumVoice, Convert.ToInt16(VolumVoice));
		PlayerPrefs.SetInt(DefineKey.setting_VolumEffect, Convert.ToInt16(VolumEffect));
		PlayerPrefs.SetInt(DefineKey.setting_Language, Convert.ToInt16(usingLangueage));
		PlayerPrefs.Save();


		//FramePerSecond 값 따른 fps 셋팅
		Setting_FPS();
	}

	

	public void Get_SettingValues()
	{

		if (PlayerPrefs.HasKey(DefineKey.setting_NoticeMsg))
			Notice_pushMsg = Convert.ToBoolean(PlayerPrefs.GetInt(DefineKey.setting_NoticeMsg));
		else
			Notice_pushMsg = true;

		if (PlayerPrefs.HasKey(DefineKey.setting_ClanMsg))
			Clan_pushMsg = Convert.ToBoolean(PlayerPrefs.GetInt(DefineKey.setting_ClanMsg));
		else
			Clan_pushMsg = true;

		if (PlayerPrefs.HasKey(DefineKey.setting_UnlockBxMsg))
			UnlockBx_pushMsg = Convert.ToBoolean(PlayerPrefs.GetInt(DefineKey.setting_UnlockBxMsg));
		else
			UnlockBx_pushMsg = true;

		if (PlayerPrefs.HasKey(DefineKey.setting_Quality))
			GraphicQuality = Convert.ToBoolean(PlayerPrefs.GetInt(DefineKey.setting_Quality));
		else
			GraphicQuality = true;

		if (PlayerPrefs.HasKey(DefineKey.setting_AttackType))
			AttackType = Convert.ToBoolean(PlayerPrefs.GetInt(DefineKey.setting_AttackType));
		else
			AttackType = false;

		if (PlayerPrefs.HasKey(DefineKey.setting_AutoBattle))
			AutoBattle = Convert.ToBoolean(PlayerPrefs.GetInt(DefineKey.setting_AutoBattle));
		else
			AutoBattle = false;

		if (PlayerPrefs.HasKey(DefineKey.setting_FPS))
			FramePerSecond = Convert.ToBoolean(PlayerPrefs.GetInt(DefineKey.setting_FPS));
		else
			FramePerSecond = false;

		//FramePerSecond 값 따른 fps 셋팅
		Setting_FPS();


		if (PlayerPrefs.HasKey(DefineKey.setting_Sensitive))
			Sensitive = Convert.ToUInt16(PlayerPrefs.GetInt(DefineKey.setting_Sensitive));
		else
			Sensitive = 50;

		if (PlayerPrefs.HasKey(DefineKey.setting_SensitiveZoomIn))
			SensitiveZoomIn = Convert.ToUInt16(PlayerPrefs.GetInt(DefineKey.setting_SensitiveZoomIn));
		else
			SensitiveZoomIn = 50;


		if (PlayerPrefs.HasKey(DefineKey.setting_VolumBGM))
			VolumBGM = Convert.ToUInt16(PlayerPrefs.GetInt(DefineKey.setting_VolumBGM));
		else
			VolumBGM = 50;

		if (PlayerPrefs.HasKey(DefineKey.setting_VolumVoice))
			VolumVoice = Convert.ToUInt16(PlayerPrefs.GetInt(DefineKey.setting_VolumVoice));
		else
			VolumVoice = 100;

		if (PlayerPrefs.HasKey(DefineKey.setting_VolumEffect))
			VolumEffect = Convert.ToUInt16(PlayerPrefs.GetInt(DefineKey.setting_VolumEffect));
		else
			VolumEffect = 100;

		if (PlayerPrefs.HasKey(DefineKey.setting_Language))
		{
			
			usingLangueage = Convert.ToUInt16(PlayerPrefs.GetInt(DefineKey.setting_Language));
			UserEditor.Getsingleton.EditLog("saved usingLangueage : " + usingLangueage);
		}
		else
		{
			SystemLanguage _deviceLang = Application.systemLanguage;
			UserEditor.Getsingleton.EditLog("_deviceLang : " + _deviceLang);
			usingLangueage = GetLanguage_FromDeviceLang(_deviceLang);

			UserEditor.Getsingleton.EditLog("GetLanguage_FromDeviceLang : " + usingLangueage);
		}
		//manager에 언어값 설정하자 
		LanguageManager.Getsingleton.language = (LanguageCode)usingLangueage;

		
	}

	void Setting_FPS()
	{
		if (!FramePerSecond)
		{
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = 60;
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
		}
		else
		{
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = 30;
			Screen.sleepTimeout = SleepTimeout.SystemSetting;
		}


	}


	int GetLanguage_FromDeviceLang(SystemLanguage deviceLang)
	{
		int langIdx = 1;
		switch (deviceLang)
		{
			case SystemLanguage.Afrikaans:
				break;
			case SystemLanguage.Arabic:
				break;
			case SystemLanguage.Basque:
				break;
			case SystemLanguage.Belarusian:
				break;
			case SystemLanguage.Bulgarian:
				break;
			case SystemLanguage.Catalan:
				break;
			case SystemLanguage.Chinese:
			case SystemLanguage.ChineseSimplified:
			case SystemLanguage.ChineseTraditional:
				langIdx = 3;
				break;
			case SystemLanguage.Czech:
				break;
			case SystemLanguage.Danish:
				break;
			case SystemLanguage.Dutch:
				break;
			case SystemLanguage.English:
				langIdx = 1;
				break;
			case SystemLanguage.Estonian:
				break;
			case SystemLanguage.Faroese:
				break;
			case SystemLanguage.Finnish:
				break;
			case SystemLanguage.French:
				break;
			case SystemLanguage.German:
				break;
			case SystemLanguage.Greek:
				break;
			case SystemLanguage.Hebrew:
				break;
			case SystemLanguage.Hungarian:
				break;
			case SystemLanguage.Icelandic:
				break;
			case SystemLanguage.Indonesian:
				break;
			case SystemLanguage.Italian:
				break;
			case SystemLanguage.Japanese:
				langIdx = 2;
				break;
			case SystemLanguage.Korean:
				langIdx = 0;
				break;
			case SystemLanguage.Latvian:
				break;
			case SystemLanguage.Lithuanian:
				break;
			case SystemLanguage.Norwegian:
				break;
			case SystemLanguage.Polish:
				break;
			case SystemLanguage.Portuguese:
				break;
			case SystemLanguage.Romanian:
				break;
			case SystemLanguage.Russian:
				break;
			case SystemLanguage.SerboCroatian:
				break;
			case SystemLanguage.Slovak:
				break;
			case SystemLanguage.Slovenian:
				break;
			case SystemLanguage.Spanish:
				break;
			case SystemLanguage.Swedish:
				break;
			case SystemLanguage.Thai:
				break;
			case SystemLanguage.Turkish:
				break;
			case SystemLanguage.Ukrainian:
				break;
			case SystemLanguage.Unknown:
				break;
			case SystemLanguage.Vietnamese:
				break;
			default:
				langIdx = 1;
				break;
		}

		return langIdx;
	}
}
