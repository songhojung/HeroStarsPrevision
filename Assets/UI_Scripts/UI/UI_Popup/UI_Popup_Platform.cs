using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Popup_Platform : UI_PopupBase 
{
	public List<GameObject> Lst_Platform;

	public override void Set_Open()
	{
		base.Set_Open();
	}

	public override void Set_Close()
	{
		base.Set_Close();
	}

	public void Set_Platform()
	{
		for (int i = 0; i < Lst_Platform.Count; i++)
		{
            Lst_Platform[i].SetActive(i==3);
//#if UNITY_EDITOR
//            Lst_Platform[i].SetActive(true);

//#elif UNITY_ANDROID
//			if(i == 1)
//				Lst_Platform[i].SetActive(false);
//			else
//				Lst_Platform[i].SetActive(true);
//#elif UNITY_IOS
//			if (i == 0)
//			{
//				Lst_Platform[i].SetActive(false);
//			}
//			else
//			{
//				Lst_Platform[i].SetActive(true);
//			}
//#endif
		}
	}

	public void ResponseButton_GuestLogin()
	{

		UI_Title.Getsingleton.Try_Login_GuestID();

		UI_Manager.Getsingleton.ClearUI(this);
		
		
	}

	public void ResponseButton_GoogleLogin()
	{
		//기존 연동된 계정 로그아웃
		PlatformManager.Getsingleton.Platform_LogOut();

		UI_Title.Getsingleton.Try_Login_Google();
		UI_Manager.Getsingleton.ClearUI(this);
	}

	public void ResponseButton_FacebookLogin()
	{
		//기존 연동된 계정 로그아웃
		PlatformManager.Getsingleton.Platform_LogOut();

		UI_Title.Getsingleton.Try_Login_Facebook();
		UI_Manager.Getsingleton.ClearUI(this);
	}

	public void ResponseButton_GamecenterLogin()
	{
		//기존 연동된 계정 로그아웃
		PlatformManager.Getsingleton.Platform_LogOut();

		UI_Title.Getsingleton.Try_Login_GameCenter();
		UI_Manager.Getsingleton.ClearUI(this);
	}



	
	
}
