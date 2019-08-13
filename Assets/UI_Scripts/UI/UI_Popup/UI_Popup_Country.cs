using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Popup_Country : MonoBehaviour 
{
	public GameObject Obj_Content;

	private Dictionary<string, Toggle> Dic_countrys = new Dictionary<string, Toggle>();

	private del_ResPopup Nextprocess;

	private string selectCounrtryName = string.Empty;

	//content move
	private GridLayoutGroup gridLytGp;
	private float centerHeight = 210;
	private float elementHeight = 0f;


	public void Set_addEventButton(del_ResPopup _Action)
	{
		Nextprocess = _Action;
	}



	void Start()
	{
		

		
	}

	public void OnEnable()
	{

		//국가정보관련  초기화 
		Init_AllCountryInfo();

		//content 정보 설정
		Set_contentInfo();

		//자신국가로 움직임관련
		moveContent_ownCountry();
	}


	
	//국가정보관련  초기화 
	public void Init_AllCountryInfo()
	{

		if (Dic_countrys.Count <= 0)
		{
			Toggle[] textCountrys = Obj_Content.GetComponentsInChildren<Toggle>();

			for (int i = 0; i < textCountrys.Length; i++)
			{
				textCountrys[i].isOn = false;
				Dic_countrys[textCountrys[i].name] = textCountrys[i];
			}
		}
	}



	//content 정보 설정
	void Set_contentInfo()
	{
		if (gridLytGp == null)
		{
			gridLytGp = Obj_Content.GetComponent<GridLayoutGroup>();
			elementHeight = gridLytGp.cellSize.y + gridLytGp.spacing.y;
		}
		
	}


	//자신국가로 움직임관련
	void moveContent_ownCountry()
	{
		User _user = UserDataManager.instance.user;
		string userCtrCd = _user.user_Users.CtrCd;
		userCtrCd = userCtrCd.ToLower();
		float totalHeight = 0;
		float maxHeight = 16675f;
		float minHeight = 9.9f;
		bool isfind = false;
		//해당 국가 찾으면서 움직일 높이 값 저장
		foreach (var cd in Dic_countrys)
		{
			if (cd.Key == userCtrCd)
			{
				
				totalHeight +=  elementHeight - centerHeight;
				cd.Value.isOn = true;
				isfind = true;

				UserEditor.Getsingleton.EditLog("국가 찾앗다 !!!~ : " + userCtrCd + " isfind : " + isfind);
			}
			else
			{
				if (!isfind)
					totalHeight += elementHeight;

				cd.Value.isOn = false;
			}
		}

		if (isfind)
		{
			if (totalHeight < minHeight)
				totalHeight = minHeight;
			else if (totalHeight > maxHeight)
				totalHeight = maxHeight;
		}
	


		//content 움직이자
		//Obj_Content.transform.localPosition = new Vector3(0, totalHeight, 0);
		StartCoroutine(routine_moveContent(totalHeight));

	}

	//content 움직이는 코루틴
	IEnumerator routine_moveContent(float _totalHeight)
	{
		float tm = 0f ;
		Vector3 targetPos = new Vector3(0,_totalHeight,0);
		Vector3 orinPos = Obj_Content.transform.localPosition;
		while (true)
		{
			float dis = Vector3.Distance(targetPos, Obj_Content.transform.localPosition);
			if ( dis < 0.1f)
				break;

			tm += Time.deltaTime;

			Obj_Content.transform.localPosition = Vector3.Lerp(orinPos, targetPos, tm*2);

			yield return null;
		}
	}




	 string Get_CountryName()
	{
		string txt = null;

		if (Dic_countrys.Count > 0)
		{

			foreach (var tog in Dic_countrys)
			{
				if (tog.Value.isOn)
				{
					txt = tog.Value.name;
					 break;
				}
			}

		
		}

		if (string.IsNullOrEmpty(txt))
			txt= "";

		return txt;
		
	}


	public void ResponseButton_Comfirm()
	{
		selectCounrtryName =Get_CountryName();
		webRequest.SetChangeUsers(selectCounrtryName, callback_completeChangeContry);

	}

	void callback_completeChangeContry()
	{
		if (Nextprocess != null)
			Nextprocess();

		UI_Setting.Getinstance.set_refresh();

		gameObject.SetActive(false);

		if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.TOP))
			UI_Top.Getsingleton.gameObject.SetActive(true);


		//선택 국가 로컬저장
		PlayerPrefs.SetString(DefineKey.UserCountryCode, selectCounrtryName);
		PlayerPrefs.Save();

		//국가변경하엿으니 소켓로그인 다시쏘자
		Network_MainMenuSoketManager.Getsingleton.Send_CTS_Login();
	}



	public void ResponseButton_close()
	{
	
		gameObject.SetActive(false);

		if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.TOP))
			UI_Top.Getsingleton.gameObject.SetActive(true);
		
	}
	
}
