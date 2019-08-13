using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Popup_SetItemInfo : MonoBehaviour
{
    public List<GameObject> DisableButtons;

    private uint unitIdx;
    private Dictionary<ushort, Infos_SetBuf> infosSetbuff;
    public void Open_SetInfo(uint targetUnitidx)
    {
        gameObject.SetActive(true);

        unitIdx = targetUnitidx;

        if (infosSetbuff == null)
            infosSetbuff = TableDataManager.instance.Infos_SetBuffs;

        //장착중 버튼 활성
        Activation_DisableButton();
    }


    public void Close_SetInfo()
    {
        gameObject.SetActive(false);

    }





    void Activation_DisableButton()
    {
        //현재 세트장비 타입반환
        SetBufKnd bufKnd = Chk_EquipSet(unitIdx);

        for (int i = 0; i < DisableButtons.Count; i++)
        {
            DisableButtons[i].SetActive(i == (int)bufKnd - 1);
        }
    }












    //현재 세트장비 타입 반환
    public SetBufKnd Chk_EquipSet(uint _unitidx)
    {
        User user = UserDataManager.instance.user;

        bool isSet = false;
        SetBufKnd setKind = SetBufKnd.NONE;

        User_Units gainUserUnit = null;
        if (user.User_Units.ContainsKey(_unitidx))
            gainUserUnit = user.User_Units[_unitidx];

        if (gainUserUnit != null)
        {
            foreach (var sb in infosSetbuff)
            {
                bool correct = true;

                if (isSet == true) break;

                if (sb.Value.MainWpnIdx != gainUserUnit.MainWpnIdx) correct = false;
                else if (sb.Value.MainWpnIdx == 0) correct = true;
                if (sb.Value.SubWpnIdx != gainUserUnit.SubWpnIdx) correct = false;
                else if (sb.Value.SubWpnIdx == 0) correct = true;
                if (sb.Value.DecoIdx1 != gainUserUnit.DecoIdx1) correct = false;
                if (sb.Value.DecoIdx2 != gainUserUnit.DecoIdx2) correct = false;
                if (sb.Value.DecoIdx3 != gainUserUnit.DecoIdx3) correct = false;

                if (correct == true)
                {
                    isSet = correct;
                    setKind = (SetBufKnd)sb.Value.BufKind;
                }
            }
        }

        return setKind;
    }



    // 세트하기위해서 부족한 아이템들 반환
    public List<uint> Get_insufficientSetItems(int setIdx, uint unitidx)
    {
        User user = UserDataManager.instance.user;
        Infos_SetBuf infoset = null;
        if (TableDataManager.instance.Infos_SetBuffs.ContainsKey((ushort)setIdx))
            infoset = TableDataManager.instance.Infos_SetBuffs[(ushort)setIdx];

        List<uint> insufficientItemIndexList = new List<uint>();


        if (infoset != null)
        {
            Dictionary<uint, user_Deco> dicUserDecos = null;
            if (user.User_Decos.ContainsKey(unitidx))
                dicUserDecos = user.User_Decos[unitIdx];

            
            if (dicUserDecos != null)
            {
                if (!dicUserDecos.ContainsKey(infoset.DecoIdx1))
                    insufficientItemIndexList.Add(infoset.DecoIdx1);
                if (!dicUserDecos.ContainsKey(infoset.DecoIdx2))
                    insufficientItemIndexList.Add(infoset.DecoIdx2);
                if (!dicUserDecos.ContainsKey(infoset.DecoIdx3))
                    insufficientItemIndexList.Add(infoset.DecoIdx3);
            }
            else //한번구매안해서 데이터 키값이없다
            {
                insufficientItemIndexList.Add(infoset.DecoIdx1);
                insufficientItemIndexList.Add(infoset.DecoIdx2);
                insufficientItemIndexList.Add(infoset.DecoIdx3);
            }

        }

        return insufficientItemIndexList;
    }






    public void ResponseBtn_Equip(int setidx)
    {
        List<uint> insufficientItems = Get_insufficientSetItems(setidx, unitIdx);

        if (insufficientItems.Count > 0)
        {
            // 부족한 아이템 팝업
            UI_Popup_buySetItem popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_buySetItem>(UIPOPUP.POPUPBUYSETITEM);
            popup.Set_buySetItem(insufficientItems,setidx);
        }
        else
        {
            //장착 프로토콜
            EquipItem( setidx);




        }
    }


    //구매하기
    public void BuyItem(List<uint> _insufficientItems,int setidx)
    {
        uint itemIdx = _insufficientItems[0]; //차례대로 구매하자

        User _user = UserDataManager.instance.user;
        uint price = 0;

       
        if(TableDataManager.instance.Infos_Decos.ContainsKey(itemIdx))
            price = TableDataManager.instance.Infos_Decos[itemIdx].SellItNum;
        if (TableDataManager.instance.Infos_weapons.ContainsKey(itemIdx))
            price = TableDataManager.instance.Infos_weapons[itemIdx].SellItNum;

        if(price > 0)
            webRequest.ShopBuyDeco(unitIdx, itemIdx, price, ()=>completeBuySetItem(_insufficientItems, itemIdx, setidx));

        
    }

    void completeBuySetItem(List<uint> _insufficientItems,uint _itemidx ,int setidx)
    {
        _insufficientItems.RemoveAt(0); //구매햇던거 리스트 삭제

        if(_insufficientItems.Count > 0)
        {
            //다시구매
            BuyItem(_insufficientItems,  setidx);
        }
        else
        {

            Infos_SetBuf infoset = null;
            if (TableDataManager.instance.Infos_SetBuffs.ContainsKey((ushort)setidx))
                infoset = TableDataManager.instance.Infos_SetBuffs[(ushort)setidx];

            //UI_EquipItem 의 Select_Item 데이터 재할당 
            Asign_SelectItemData(infoset);
            //갱신
            UI_Equipment.Getsingleton.callback_complete_BuyItem();

            //장착
            EquipItem(setidx);
        }
    }


    void EquipItem(int setidx)
    {
        Infos_SetBuf infoset = null;
        if (TableDataManager.instance.Infos_SetBuffs.ContainsKey((ushort)setidx))
            infoset = TableDataManager.instance.Infos_SetBuffs[(ushort)setidx];

        User_Units nowUseUnit = UserDataManager.instance.user.User_Units[unitIdx];
        webRequest.SetEquipItem(unitIdx, nowUseUnit.MainWpnIdx, nowUseUnit.SubWpnIdx
            , infoset.DecoIdx1, infoset.DecoIdx2, infoset.DecoIdx3, () => CompleteEquip(infoset));

    }


    void CompleteEquip(Infos_SetBuf _infoset)
    {

        //UI_EquipItem 의 Select_Item 데이터 재할당
        Asign_SelectItemData(_infoset);

        //갱신
        UI_Equipment.Getsingleton.callback_complete_Equip();

        //토스트팝업
        UI_Popup_Toast popupToast = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Toast>(UIPOPUP.POPUPTOAST);
        string txt = string.Format("{0}를 장착했습니다.", TableDataManager.instance.GetSetName(_infoset.BufKind));
        popupToast.SetPopupMessage(txt);

        //세트팝업창닫기
        gameObject.SetActive(false);
    }


    void Asign_SelectItemData(Infos_SetBuf _infoset)
    {
        //현재 탭에 대해 선택아이템 변경
        EquipType _nowEquipType = UI_Equipment.Getsingleton.nowEquipInven;
        uint targetItemIdx = 0;
        if (_nowEquipType == EquipType.Main) targetItemIdx = _infoset.MainWpnIdx;
        if (_nowEquipType == EquipType.Sub) targetItemIdx = _infoset.SubWpnIdx;
        if (_nowEquipType == EquipType.Dress_HEAD) targetItemIdx = _infoset.DecoIdx1;
        if (_nowEquipType == EquipType.Dress_BODY) targetItemIdx = _infoset.DecoIdx2;
        if (_nowEquipType == EquipType.Dress_FACE) targetItemIdx = _infoset.DecoIdx3;

        List<UIItem_EquipItem> _lst_Items = UI_Equipment.Getsingleton.Get_nowEquipItemList(UI_Equipment.Getsingleton.nowEquipInven);

        for (int i = 0; i < _lst_Items.Count; i++)
            if (_lst_Items[i].ItemIdx == targetItemIdx)
                UI_Equipment.Getsingleton.Selected_Item = _lst_Items[i]; //선택아이템 변경
    }



    
}
