using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItem_TeamMemberElement : MonoBehaviour 
{
	[HideInInspector]
	public User_RoomInfo userRoomInfo;

	public int SlotIdx = 0;
	public int TeamIdx = 0;
	private bool isEnable = false;
	public bool SlotActive
	{
		set
		{
			isEnable = value;
			InfoObject.SetActive(isEnable);
		}
		get { return isEnable; }
	}
	private MemberSlot_State SlotState = MemberSlot_State.Empty;
	private bool IsMaster = false;				//방장이냐?



	public GameObject InfoObject;
	public Image image_back;
	public Image image_Master;
	public Image image_ClanMark;
	public Text text_UserName;

	public void Set_roomSlot(User_RoomInfo _userRoominfo)
	{
		userRoomInfo = _userRoominfo;



		Chk_slotStateInfo();
		Set_SlotInfo();
		

	}

	//정보들 설정한다
	public void Set_SlotInfo()
	{
		//백이미지 색
		if (SlotState == MemberSlot_State.SelectOwn)
			image_back.color = DefineKey.Yellow;
		else
			image_back.color = DefineKey.White;

		//방장표시
		if (IsMaster)
			image_Master.gameObject.SetActive(true);
		else
			image_Master.gameObject.SetActive(false);

		//유저이름
		text_UserName.text = userRoomInfo.roomUserNkNm; 

		//클랜마크
		image_ClanMark.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.clanmark, userRoomInfo.roomUserClanMark));

		

	}

	void Chk_slotStateInfo()
	{
		

		//방장이냐?
		if (UserDataManager.instance.user.User_readyRoomInfo.RoomMsterUserID == userRoomInfo.roomUserID)
			IsMaster = true;
		else
			IsMaster = false;


		//슬롯이 나자신 유저이냐?
		if(userRoomInfo.roomUserID == UserDataManager.instance.user.user_Users.UserID)
			SlotState = MemberSlot_State.SelectOwn;
		else
			SlotState = MemberSlot_State.SelectOther;
	}

	public void Clear_Slot()
	{
		userRoomInfo = null;
		SlotActive = false;

		image_back.color = DefineKey.White;

		SlotState = MemberSlot_State.Empty;

		IsMaster = false;

	}


	public void ResponseButton_Select()
	{
		if (!isEnable)
		{
			Network_MainMenuSoketManager.Getsingleton.Send_CTS_TeamMove((byte)TeamIdx, (byte)(SlotIdx + 1));
		}

	}
	

	public void ResponseButton_UserInfo()
	{
		//if (UserDataManager.instance.user.user_Users.UserID != userRoomInfo.roomUserID)
			webRequest.GetUserInfos(userRoomInfo.roomUserID, callback_Complete_TeamUser);
	}

	void callback_Complete_TeamUser()
	{
		UI_Popup_UserInfo popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_UserInfo>(UIPOPUP.POPUPUSERINFO);
		User _checkedUser;
		if (UserDataManager.instance.user.user_Users.UserID != userRoomInfo.roomUserID)
			_checkedUser = UserDataManager.instance.OtherUser;
		else
			_checkedUser = UserDataManager.instance.user;

		popup.Set_UserInfo(_checkedUser);
	}
}
