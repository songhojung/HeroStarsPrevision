using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Popup_MakeName : UI_PopupBase
{
	private MAKENAMEPOPUP_TYPE popupType = MAKENAMEPOPUP_TYPE.NONE;

	public Text text_message;
	public Text text_needGem;
	public Text text_buttonName;
	public Text text_placeHolderName;
	public Text text_popupTitle;
	public InputField inputfield_changeName;
	public GameObject GemObj;
	public List<GameObject> Lst_NameObj = new List<GameObject>();
	public Button button_Cancle;
	public string changedName = "";

	private del_ResPopup Request_function;
	private del_webResp_0 Respons_delegate;

	public override void Set_Open()
	{
		base.Set_Open();

	}

	public override void Set_Close()
	{
		base.Set_Close();
	}


	/// <summary>
	/// 이름변경 팝업 정보 설정 (팝업타입 , 필요보석)
	/// </summary>
	public void Set_Info(MAKENAMEPOPUP_TYPE _popupType,int _needgem)
	{
		popupType = _popupType;
		text_needGem.text = _needgem.ToString();

		Set_ButtoName(_popupType);

		if (popupType == MAKENAMEPOPUP_TYPE.REGISTERUSERNAME)
		{
			//유저명을 입력해 주세요 (최대 10글자)
			//광고 및 욕설, 음란성 내용이 들어간 유저명을 사용할 경우 제재될 수 있으니 주의부탁드립니다
			text_message.text = TextDataManager.Dic_TranslateText[88];
			text_popupTitle.text = TextDataManager.Dic_TranslateText[149]; //계정생성 151
			text_placeHolderName.text = TextDataManager.Dic_TranslateText[32];//유저명을 입력하세요
			text_buttonName.text = TextDataManager.Dic_TranslateText[149];//계정생성 151
			//GemObj.SetActive(false);
			//button_Cancle.gameObject.SetActive(false);
			inputfield_changeName.characterLimit = 10;
		}
		else if (popupType == MAKENAMEPOPUP_TYPE.CHANGEUSERNAME)
		{
			//유저명을 입력해 주세요 (최대 10글자)
			//광고 및 욕설, 음란성 내용이 들어간 유저명을 사용할 경우 제재될 수 있으니 주의부탁드립니다
			text_message.text = TextDataManager.Dic_TranslateText[88];
			text_popupTitle.text = TextDataManager.Dic_TranslateText[149];//이름변경
			text_placeHolderName.text = TextDataManager.Dic_TranslateText[32];//유저명을 입력하세요
			text_buttonName.text = TextDataManager.Dic_TranslateText[149];//이름변경
			inputfield_changeName.characterLimit = 10;
		}
		else if (popupType == MAKENAMEPOPUP_TYPE.CHANGECLANNAME)
		{
			//클랜명을 입력해 주세요 (최대 6글자)
			//광고 및 욕설, 음란성 내용이 들어간 클랜명을 사용할 경우 제재될 수 있으니 주의부탁드립니다
			text_message.text = TextDataManager.Dic_TranslateText[87];
			text_popupTitle.text = TextDataManager.Dic_TranslateText[149];//이름변경
			text_placeHolderName.text = TextDataManager.Dic_TranslateText[33];//클랜명을 입력하세p요
			text_buttonName.text = TextDataManager.Dic_TranslateText[149];//이름변경

			inputfield_changeName.characterLimit = 10;

			Request_function = RequestClanNameChange;
		}
		else if (popupType == MAKENAMEPOPUP_TYPE.MAKECLAN)
		{
			//클랜명을 입력해 주세요 (최대 6글자)
			//광고 및 욕설, 음란성 내용이 들어간 클랜명을 사용할 경우 제재될 수 있으니 주의부탁드립니다
			text_message.text = TextDataManager.Dic_TranslateText[87];
			text_popupTitle.text = TextDataManager.Dic_TranslateText[86];//클랜생성
			text_placeHolderName.text = TextDataManager.Dic_TranslateText[33];//클랜명을 입력핫요
			text_buttonName.text = TextDataManager.Dic_TranslateText[86];//클랜생성

			inputfield_changeName.characterLimit = 10;

			Request_function = RequestCreatClan;
		}
	}
	void Set_ButtoName(MAKENAMEPOPUP_TYPE makeType)
	{
		if (makeType == MAKENAMEPOPUP_TYPE.REGISTERUSERNAME)
		{
			Lst_NameObj[0].SetActive(true); //일반 텍스트
			Lst_NameObj[1].SetActive(false); // 보석 텍스트
		}
		else
		{
			Lst_NameObj[0].SetActive(false); //일반 텍스트
			Lst_NameObj[1].SetActive(true); // 보석 텍스트
		}
	
	}

	public void SetPopupMessage(string msg)
	{
		text_message.text = msg;
	}

	public void ResponseInput_End()
	{
		changedName = inputfield_changeName.text;
	}


	public void Set_AddEventButton(del_webResp_0 _Action)
	{
		Respons_delegate = _Action;
	}

	public override void ResponseButton_Yes()
	{
		base.ResponseButton_Yes();

		if (changedName.Length <= 1)
		{
			UI_Popup_Toast popuptoast = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Toast>(UIPOPUP.POPUPTOAST);
			popuptoast.SetPopupMessage(TextDataManager.Dic_TranslateText[152]);
			return;
		}

		//요청할 웹프로토콜
		if (Request_function != null)
		{
			Request_function();
		}
		else
		{
			if (Respons_delegate !=null)
			Respons_delegate();
		}
		UI_Manager.Getsingleton.ClearUI(this);

	}

	public void ResponseButton_Close()
	{
		UI_Manager.Getsingleton.ClearUI(this);
	}

	void RequestCreatClan()
	{
		// 콜백은 UI_lobby.cs p78
		if (!TextDataManager.Chk_BannedLetter(ref changedName))
		{
			webRequest.ClanMake(changedName, Respons_delegate);
		}
		else
		{
			UI_Popup_Toast popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Toast>(UIPOPUP.POPUPTOAST);
			popup.SetPopupMessage(TextDataManager.Dic_ErrorCode[111]);//사용 불가능한 문자열이 포함 되어있습니다.
			inputfield_changeName.text = "";
		}

	}

	void RequestClanNameChange()
	{
		//욕설인지 체크 
		if (!TextDataManager.Chk_BannedLetter(ref changedName))
		{
			webRequest.ClanNameChange(5, changedName, Respons_delegate);
		}
		else
		{
			UI_Popup_Toast popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Toast>(UIPOPUP.POPUPTOAST);
			popup.SetPopupMessage(TextDataManager.Dic_ErrorCode[111]);//사용 불가능한 문자열이 포함 되어있습니다.
			inputfield_changeName.text = "";
		}
		
	}

}
