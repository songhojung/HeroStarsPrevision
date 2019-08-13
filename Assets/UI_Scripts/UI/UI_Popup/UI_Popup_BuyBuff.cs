using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Popup_BuyBuff : UI_PopupBase
{
    public Text Text_buyBuffInfo;
    public Image Image_BuyBuffType;
    public Text Text_Price;
    public Text Text_AddTime;

    private BUFF_TYPE buyBuffType;
    private uint price;
    public void Set_BuyBuff(BUFF_TYPE buffType)
    {
        buyBuffType = buffType;

        string buffMainTxt = string.Empty;
        string buffSubTxt = string.Empty;

        price = TableDataManager.instance.Infos_ConstValues[(int)ConstValue_TYPE.Const_ClanBuffPrice].ConsVal;

        int addDurationTime = (int)TableDataManager.instance.Infos_ConstValues[(int)ConstValue_TYPE.Const_ClanBuffDurationTime].ConsVal / 60;
        
        switch (buyBuffType)
        {
            case BUFF_TYPE.CLANEXP:
                string persantExp = string.Format("{0}%", TableDataManager.instance.Infos_ConstValues[(int)ConstValue_TYPE.Const_ClanExpBuffRateValue].ConsVal);

                buffMainTxt = string.Format("경험치{0} 의 유지시간을 {1}시간 추가시키겠습니까?",persantExp, addDurationTime);
                buffSubTxt = "<color=#ff0000>*적용중에는 전투시 획득하는 경험치양이 증가합니다\n*클래멤버 모두에게 적용됩니다.</color>";
                break;
            case BUFF_TYPE.CLANGOLD:
                string persantGold = string.Format("{0}%", TableDataManager.instance.Infos_ConstValues[(int)ConstValue_TYPE.Const_ClanGoldBuffRateValue].ConsVal);
                buffMainTxt = string.Format("골드{0} 의 유지시간을 {1}시간 추가시키겠습니까?",persantGold, addDurationTime);
                buffSubTxt = "<color=#ff0000>*적용중에는 전투시 획득하는 골드양이 증가합니다\n*클래멤버 모두에게 적용됩니다.</color>";
                break;
        }

        Text_buyBuffInfo.text = string.Format("{0}\n\n{1}", buffMainTxt, buffSubTxt);
        Text_Price.text = price.ToString();
        Text_AddTime.text = string.Format("+{0}시간", addDurationTime);
    }


    public void ResponseButton_Buff()
    {
        int buffType = buyBuffType == BUFF_TYPE.CLANEXP ? 2 : 1; //clanexp = 2 , clangold = 1
        uint price = TableDataManager.instance.Infos_ConstValues[(int)ConstValue_TYPE.Const_ClanBuffPrice].ConsVal;

        webRequest.ClanSetBuf((byte)buffType, price, callback_complete_setBuff);
        UI_Manager.Getsingleton.ClearUI(this);

    }

    void callback_complete_setBuff()
    {

        //clan_board 갱신을 하기위해 clanInfo 프로토콜 한번호출하자 
        webRequest.ClanInfo(UI_Clan.Getinstance.callback_complte_RequestClaninfo);


    }






    public void ResponseButton_Close()
    {
        UI_Manager.Getsingleton.ClearUI(this);
    }

}
