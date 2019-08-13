using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Popup_buySetItem : UI_PopupBase
{
    public List<GameObject> Lst_Items;
    public List<Image> Lst_ImageBuyItem;
    public Text Text_Price;

    private List<uint> buyItemLst;
    private int setIdx;
    
    public void Set_buySetItem(List<uint> _buyItemLst, int _setIdx)
    {
        setIdx = _setIdx;
        buyItemLst = _buyItemLst;

        TableDataManager table = TableDataManager.instance;
        int totalPrice = 0;
        for (int i = 0; i < buyItemLst.Count; i++)
        {
            //아이템 활성
            Lst_Items[i].SetActive(true);
            //아이템이미지
            Lst_ImageBuyItem[i].sprite = ImageManager.instance.Get_Sprite(buyItemLst[i].ToString());

            if (table.Infos_weapons.ContainsKey(buyItemLst[i]))
                totalPrice += (int)table.Infos_weapons[buyItemLst[i]].SellItNum;
            if(table.Infos_Decos.ContainsKey(buyItemLst[i]))
                totalPrice += (int)table.Infos_Decos[buyItemLst[i]].SellItNum;

        }


        //가격
        Text_Price.text = totalPrice.ToString();
    }


    public void Set_addEventYESButton(del_ResPopup _Action)
    {
        delegate_ResponseOk = _Action;
    }


    public void ReponseButton_BuySet()
    {
        UI_Equipment.Getsingleton.popupSetItemInfo.BuyItem(buyItemLst, setIdx);
        UI_Manager.Getsingleton.ClearUI(this);
    }



    public void ResponseButton_Close()
    {
        UI_Manager.Getsingleton.ClearUI(this);
    }
}
