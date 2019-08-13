using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItem_GainItem : UI_Base 
{

	[HideInInspector]
	public GainItem gainItem;

	public Image Image_Icon;
	public Text Text_ItemName;
	public Text Text_ItemNum;



	public override void set_Open()
	{
		base.set_Open();
	}

	public override void set_Close()
	{
		base.set_Close();
	}


	public void Set_GainItemInfo(GainItem item)
	{
		gainItem = item;


		// 이미지적용
		Apply_GainItem();

	}
	private void Apply_GainItem()
	{

		switch (gainItem.ItTp)
		{
			case ITEMTYPE.GEM:
				Image_Icon.sprite = ImageManager.instance.Get_Sprite(DefineKey.Gem);
				Text_ItemName.text = TextDataManager.Dic_TranslateText[37]; // 보석 
				break;
			case ITEMTYPE.GOLD:
				//골드 이미지
				Image_Icon.sprite = ImageManager.instance.Get_Sprite(DefineKey.Gold);
				Text_ItemName.text =TextDataManager.Dic_TranslateText[36]; // 골드
				break;

			default:
				Image_Icon.sprite = ImageManager.instance.Get_Sprite("btn_lobby_ciclebutton");
				Text_ItemName.text = "없음";
				break;
			
		}

		//갯수
		if (gainItem.ItTp == ITEMTYPE.ITEMBOX)
			Text_ItemNum.text = string.Format("X {0}",1);
		else
			Text_ItemNum.text = string.Format("X {0}", gainItem.num);

		Image_Icon.SetNativeSize();
	}
}

