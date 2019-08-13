using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class Respawn_UI_Script : MonoBehaviour
{
    public class ToggleGroup_class
    {
        public GameObject OJ;
        public int OJ_Index;
        public String OJ_Name;        
        public Toggle Toggle_Item;        
    }

    public Dictionary<String, ToggleGroup_class> Char_Toggle_List = new Dictionary<String, ToggleGroup_class>();
    public Dictionary<String, ToggleGroup_class> MainGun_Toggle_List = new Dictionary<String, ToggleGroup_class>();
    public Dictionary<String, ToggleGroup_class> SubGun_Toggle_List = new Dictionary<String, ToggleGroup_class>();

    Dictionary<uint, User_weapon> Select_Char_ALL_Gun_Data;

    GamePlay_Script Play_Script;
    Network_Battle_Script Net_Script;

    public Text Killer_NickName;
    public Text Killer_Message;

    public Text Player_Kill_Count;
    public Text Player_Death_Count;

    public Text Respawn_Time_Text;
    public Image Respawn_Circle_Img;

    public GameObject Char_List_OJ;

    public ToggleGroup Char_ToggleGroup;
    public ToggleGroup MainGun_ToggleGroup;
    public ToggleGroup SubGun_ToggleGroup;

    public GameObject Time_Sound_OJ;

    int Toggle_Char_Index = 0;
    byte Toggle_Char_Level = 0;
    int Toggle_Main_Gun_Index = 0;
    int Toggle_Sub_Gun_Index = 0;

    public GameObject Now_Respawn_Button;
    public Image Now_Respawn_Icon;
    public Text Now_Respawn_Price;
    public GameObject Now_Helicopter_Respawn_Button;
    public Image Now_Helicopter_Respawn_Icon;
    public Text Now_Helicopter_Respawn_Price;
    public bool Now_Respawn_Button_Check = false;

    bool Respawn_Time_Check = false;
    TimeSpan Respawn_Ramain_Time;
    DateTime Respawn_Time;

    bool Init_Check = false;
    
    bool Char_Setting_Check = false;

    public GameObject Progress_Button;

    void Awake()
    {
        Vector2 Size_Delta = transform.GetComponent<RectTransform>().sizeDelta;
        transform.SetParent(GameObject.Find("Canvas").transform);
        transform.localPosition = new Vector3(0, 10000, 0);
        transform.localScale = new Vector3(1, 1, 1);
        transform.GetComponent<RectTransform>().sizeDelta = Size_Delta;

        Play_Script = GameObject.Find("Script_Object").GetComponent<GamePlay_Script>();
        Net_Script = GameObject.Find("Network_Script").GetComponent<Network_Battle_Script>();

        //------------------------------------------------------------------------------------------------------------------------------------------------

        Time_Sound_OJ.SetActive(false);

    }

    void Start()
    {
        //------------------------------------------------------------------------------------------------------------------------------

        //IEnumerator<Toggle> Char_IEnumerator = Char_ToggleGroup.ActiveToggles().GetEnumerator();
        //IEnumerator<Toggle> MainGun_IEnumerator = MainGun_ToggleGroup.ActiveToggles().GetEnumerator();
        //IEnumerator<Toggle> SubGun_IEnumerator = SubGun_ToggleGroup.ActiveToggles().GetEnumerator();

        //String OJ_Name = "";
        //while (Char_IEnumerator.MoveNext())
        //{
        //    OJ_Name = Char_IEnumerator.Current.transform.name;
        //    if (Char_Toggle_List.ContainsKey(OJ_Name) == false)
        //    {
        //        Char_Toggle_List.Add(OJ_Name, new ToggleGroup_class());
        //        Char_Toggle_List[OJ_Name].OJ = Char_IEnumerator.Current.gameObject;
        //        int.TryParse(OJ_Name, out Char_Toggle_List[OJ_Name].OJ_Index);

        //        Char_Toggle_List[OJ_Name].OJ_Name = OJ_Name;                
        //        Char_Toggle_List[OJ_Name].Toggle_Item = Char_IEnumerator.Current;
        //        Char_Toggle_List[OJ_Name].Toggle_Item.isOn = false;
        //    }            
        //}

        //while (MainGun_IEnumerator.MoveNext())
        //{
        //    OJ_Name = MainGun_IEnumerator.Current.transform.name;
        //    if (MainGun_Toggle_List.ContainsKey(OJ_Name) == false)
        //    {
        //        MainGun_Toggle_List.Add(OJ_Name, new ToggleGroup_class());
        //        MainGun_Toggle_List[OJ_Name].OJ = MainGun_IEnumerator.Current.gameObject;
        //        int.TryParse(OJ_Name, out MainGun_Toggle_List[OJ_Name].OJ_Index);

        //        MainGun_Toggle_List[OJ_Name].OJ_Name = OJ_Name;
        //        MainGun_Toggle_List[OJ_Name].Toggle_Item = MainGun_IEnumerator.Current;
        //        MainGun_Toggle_List[OJ_Name].Toggle_Item.isOn = false;
        //    }
        //}

        //while (SubGun_IEnumerator.MoveNext())
        //{
        //    OJ_Name = SubGun_IEnumerator.Current.transform.name;
        //    if (SubGun_Toggle_List.ContainsKey(OJ_Name) == false)
        //    {
        //        SubGun_Toggle_List.Add(OJ_Name, new ToggleGroup_class());
        //        SubGun_Toggle_List[OJ_Name].OJ = SubGun_IEnumerator.Current.gameObject;
        //        int.TryParse(OJ_Name, out SubGun_Toggle_List[OJ_Name].OJ_Index);

        //        SubGun_Toggle_List[OJ_Name].OJ_Name = OJ_Name;
        //        SubGun_Toggle_List[OJ_Name].Toggle_Item = SubGun_IEnumerator.Current;
        //        SubGun_Toggle_List[OJ_Name].Toggle_Item.isOn = false;
        //    }
        //}

        ////캐릭터 선택 리스트 오브젝트
        //Char_List_OJ.SetActive(true);

    }

    void Update()
    {
        if (!Link_Script.i.IsConnected()) return;

        //리스폰 시간 연산
        Respawn_Time_Operation();
    }

    public void Panel_Init(String _Respawn_Player_Killer_NickName, String _Respawn_Killing_Message, ushort _Respawn_Player_Kill_Count, ushort _Respawn_Player_Death_Count)
    {
        transform.localPosition = new Vector3(0, 0, 0);

        //-----------------------------------------------------------------------------------------------------------------------------------------------

        Killer_NickName.text = _Respawn_Player_Killer_NickName;

        //자살일때
        if (_Respawn_Player_Killer_NickName.Equals(Link_Script.i.User_NickName))
        {
            Killer_Message.text = Link_Script.i.Suicide_Message;
        }
        else//자살이 아닐때
        {
            Killer_Message.text = _Respawn_Killing_Message;
        }

        Player_Kill_Count.text = "" + _Respawn_Player_Kill_Count;
        Player_Death_Count.text = "" + _Respawn_Player_Death_Count;

        //-----------------------------------------------------------------------------------------------------------------------------------------------

        Respawn_Time_Check = true;
        Respawn_Time = new DateTime(Play_Script.Respawn_Server_Time);

        //-----------------------------------------------------------------------------------------------------------------------------------------------

        //Char_Setting_Check = false;

        //Toggle_Char_Index = Link_Script.i.Char_Index;
        //Toggle_Char_Level = Link_Script.i.Get_ALL_Char_Data[(uint)Toggle_Char_Index].RefLv;
        //Toggle_Main_Gun_Index = (int)Link_Script.i.Get_ALL_Char_Data[(uint)Toggle_Char_Index].MainWpnIdx;
        //Toggle_Sub_Gun_Index = (int)Link_Script.i.Get_ALL_Char_Data[(uint)Toggle_Char_Index].SubWpnIdx;

        //Init_Check = true;

        ////선택한 캐릭터가 소유한 총기 스크롤 리스트 보여주기 셋팅
        //Gun_List_Item_Init();

        //Init_Check = false;

        //-----------------------------------------------------------------------------------------------------------------------------------------------

        ////캐릭터 선택 리스트 오브젝트
        //if (Link_Script.i.Play_Mode == BattleKind.BEGINNER_TEAM_BATTLE)//초보모드일때는 캐릭터선택 리스트 없애버린다.
        //{
        //    Char_List_OJ.SetActive(false);
        //}
        //else
        //{
        //    Char_List_OJ.SetActive(true);
        //}        
        
        //-----------------------------------------------------------------------------------------------------------------------------------------------

        Now_Respawn_Button_Check = false;        
        Now_Respawn_Button.SetActive(false);
        Now_Helicopter_Respawn_Button.SetActive(false);

        //Link_Script.i.Player_Diamond = 0;
        //Link_Script.i.Player_Gold = 500;

        //즉시 부활 재화 타입, 소유하고 있는 재화가 즉시리스폰을 할수 있는 상태일때만 버튼 보인다.
        if (TableDataManager.instance.Infos_ConstValues[(int)ConstValue_TYPE.Const_InstantItTp].ConsVal == 1 &&
            Link_Script.i.Player_Diamond >= TableDataManager.instance.Infos_ConstValues[(byte)ConstValue_TYPE.Const_InstantValue].ConsVal)//다이아
        {
            Now_Respawn_Button_Check = true;
            Now_Respawn_Button.SetActive(true); 

            Now_Respawn_Icon.sprite = ImageManager.instance.Get_Sprite(DefineKey.Gem);
            Now_Respawn_Icon.SetNativeSize();

            //즉시 부활 가격
            Now_Respawn_Price.text = "" + TableDataManager.instance.Infos_ConstValues[(byte)ConstValue_TYPE.Const_InstantValue].ConsVal;
        }
        else if (TableDataManager.instance.Infos_ConstValues[(int)ConstValue_TYPE.Const_InstantItTp].ConsVal == 2 &&
            Link_Script.i.Player_Gold >= TableDataManager.instance.Infos_ConstValues[(byte)ConstValue_TYPE.Const_InstantValue].ConsVal)//골드
        {
            Now_Respawn_Button_Check = true;
            Now_Respawn_Button.SetActive(true); 

            Now_Respawn_Icon.sprite = ImageManager.instance.Get_Sprite(DefineKey.Gold);
            Now_Respawn_Icon.SetNativeSize();

            //즉시 부활 가격
            Now_Respawn_Price.text = "" + TableDataManager.instance.Infos_ConstValues[(byte)ConstValue_TYPE.Const_InstantValue].ConsVal;
        }
        
        //즉시 헬리콥터 부활 재화 타입, 소유하고 있는 재화가 즉시리스폰을 할수 있는 상태일때만 버튼 보인다.
        if (TableDataManager.instance.Infos_ConstValues[(int)ConstValue_TYPE.Const_FlyingItTp].ConsVal == 1 &&
            Link_Script.i.Player_Diamond >= TableDataManager.instance.Infos_ConstValues[(byte)ConstValue_TYPE.Const_FlyingValue].ConsVal)//다이아
        {
            Now_Respawn_Button_Check = true;
            Now_Helicopter_Respawn_Button.SetActive(true);

            Now_Helicopter_Respawn_Icon.sprite = ImageManager.instance.Get_Sprite(DefineKey.Gem);
            Now_Helicopter_Respawn_Icon.SetNativeSize();

            //즉시헬리콥터 부활 가격
            Now_Helicopter_Respawn_Price.text = "" + TableDataManager.instance.Infos_ConstValues[(byte)ConstValue_TYPE.Const_FlyingValue].ConsVal;
        }
        else if (TableDataManager.instance.Infos_ConstValues[(int)ConstValue_TYPE.Const_FlyingItTp].ConsVal == 2 &&
            Link_Script.i.Player_Gold >= TableDataManager.instance.Infos_ConstValues[(byte)ConstValue_TYPE.Const_FlyingValue].ConsVal)//골드
        {
            Now_Respawn_Button_Check = true;
            Now_Helicopter_Respawn_Button.SetActive(true);

            Now_Helicopter_Respawn_Icon.sprite = ImageManager.instance.Get_Sprite(DefineKey.Gold);
            Now_Helicopter_Respawn_Icon.SetNativeSize();

            //즉시헬리콥터 부활 가격
            Now_Helicopter_Respawn_Price.text = "" + TableDataManager.instance.Infos_ConstValues[(byte)ConstValue_TYPE.Const_FlyingValue].ConsVal;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------

        //다음 슬롯에 셋팅된 캐릭터로 바꾼다고 서버에 날려준다.
        for (int i = 0; i < Link_Script.i.Get_UseCharacterIdx.Length; i++)
        {
            if (Link_Script.i.Char_Index == (int)Link_Script.i.Get_UseCharacterIdx[i])
            {
                if (i == Link_Script.i.Get_UseCharacterIdx.Length - 1)
                {
                    Toggle_Char_Index = (int)Link_Script.i.Get_UseCharacterIdx[0];
                }
                else
                {
                    Toggle_Char_Index = (int)Link_Script.i.Get_UseCharacterIdx[i + 1];
                }

                Toggle_Char_Level = Link_Script.i.Get_ALL_Char_Data[(uint)Toggle_Char_Index].RefLv;
                Toggle_Main_Gun_Index = (int)Link_Script.i.Get_ALL_Char_Data[(uint)Toggle_Char_Index].MainWpnIdx;
                Toggle_Sub_Gun_Index = (int)Link_Script.i.Get_ALL_Char_Data[(uint)Toggle_Char_Index].SubWpnIdx;

                //리스폰시 캐릭터 셋팅 바뀐값 서버에 넘겨주기
                Char_Setting_Send();

                break;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------

        Time_Sound_OJ.SetActive(true);

        if (Link_Script.i.Play_Mode == BattleKind.ALONE_PLAY_BATTLE)//싱글모드
        {
            Progress_Button.SetActive(false);
        }
        else
        {
            Progress_Button.SetActive(true);
        }        
    }

    

    //선택한 캐릭터가 소유한 총기 스크롤 리스트 보여주기 셋팅
    void Gun_List_Item_Init()
    {
        ////현재 리스트상에 선택 되어 있는 캐릭터의 쓸수 있는 무기들의 정보를 가져온다.
        //Select_Char_ALL_Gun_Data = SendManager.Instance.Get_userWeapons(Toggle_Char_Index);

        //foreach (var _Char_Toggle_List in Char_Toggle_List)
        //{
        //    _Char_Toggle_List.Value.Toggle_Item.isOn = false;
        //    _Char_Toggle_List.Value.OJ.SetActive(false);

        //    foreach (var _Get_ALL_Char_Data in Link_Script.i.Get_ALL_Char_Data)
        //    {
        //        if (_Get_ALL_Char_Data.Value.Unitidx == _Char_Toggle_List.Value.OJ_Index)
        //        {
        //            _Char_Toggle_List.Value.OJ.SetActive(true);
        //            break;
        //        }
        //    }
        //}

        //foreach (var _MainGun_Toggle_List in MainGun_Toggle_List)
        //{
        //    _MainGun_Toggle_List.Value.Toggle_Item.isOn = false;
        //    _MainGun_Toggle_List.Value.OJ.SetActive(false);

        //    foreach (var _Select_Char_ALL_Gun_Data in Select_Char_ALL_Gun_Data)
        //    {
        //        if (_Select_Char_ALL_Gun_Data.Value.WpnIdx == _MainGun_Toggle_List.Value.OJ_Index)
        //        {
        //            _MainGun_Toggle_List.Value.OJ.SetActive(true);
        //            break;
        //        }
        //    }
        //}

        //foreach (var _SubGun_Toggle_List in SubGun_Toggle_List)
        //{
        //    _SubGun_Toggle_List.Value.Toggle_Item.isOn = false;
        //    _SubGun_Toggle_List.Value.OJ.SetActive(false);

        //    foreach (var _Select_Char_ALL_Gun_Data in Select_Char_ALL_Gun_Data)
        //    {
        //        if (_Select_Char_ALL_Gun_Data.Value.WpnIdx == _SubGun_Toggle_List.Value.OJ_Index)
        //        {
        //            _SubGun_Toggle_List.Value.OJ.SetActive(true);
        //            break;
        //        }
        //    }
        //}

        //Char_Toggle_List[Toggle_Char_Index.ToString()].Toggle_Item.isOn = true;
        //MainGun_Toggle_List[Toggle_Main_Gun_Index.ToString()].Toggle_Item.isOn = true;
        //SubGun_Toggle_List[Toggle_Sub_Gun_Index.ToString()].Toggle_Item.isOn = true;
    }

    //스크롤 리스트의 토글에서 터치 이벤트가 발생했다
    public void Item_Touch_Event()
    {
        //if (transform.localPosition != Vector3.zero || Init_Check) return;

        //Char_Setting_Check = true;

        //foreach (var _Char_Toggle_List in Char_Toggle_List)
        //{
        //    if (_Char_Toggle_List.Value.Toggle_Item.isOn)
        //    {
        //        if (Link_Script.i.Get_ALL_Char_Data.ContainsKey((uint)_Char_Toggle_List.Value.OJ_Index) == false) continue;

        //        if (Toggle_Char_Index != _Char_Toggle_List.Value.OJ_Index)
        //        {
        //            Toggle_Char_Index = _Char_Toggle_List.Value.OJ_Index;
        //            Toggle_Char_Level = Link_Script.i.Get_ALL_Char_Data[(uint)Toggle_Char_Index].RefLv;
        //            Toggle_Main_Gun_Index = (int)Link_Script.i.Get_ALL_Char_Data[(uint)Toggle_Char_Index].MainWpnIdx;
        //            Toggle_Sub_Gun_Index = (int)Link_Script.i.Get_ALL_Char_Data[(uint)Toggle_Char_Index].SubWpnIdx;

        //            //선택한 캐릭터가 소유한 총기 스크롤 리스트 보여주기 셋팅
        //            Gun_List_Item_Init();
        //        }
        //        break;
        //    }
        //}

        //foreach (var _MainGun_Toggle_List in MainGun_Toggle_List)
        //{
        //    if (_MainGun_Toggle_List.Value.Toggle_Item.isOn)
        //    {
        //        if (Select_Char_ALL_Gun_Data.ContainsKey((uint)_MainGun_Toggle_List.Value.OJ_Index) == false) continue;

        //        Toggle_Main_Gun_Index = _MainGun_Toggle_List.Value.OJ_Index;
        //        break;
        //    }
        //}

        //foreach (var _SubGun_Toggle_List in SubGun_Toggle_List)
        //{
        //    if (_SubGun_Toggle_List.Value.Toggle_Item.isOn)
        //    {
        //        if (Select_Char_ALL_Gun_Data.ContainsKey((uint)_SubGun_Toggle_List.Value.OJ_Index) == false) continue;

        //        Toggle_Sub_Gun_Index = _SubGun_Toggle_List.Value.OJ_Index;
        //        break;
        //    }
        //}
    }

    //리스폰시 캐릭터 셋팅 바뀐값 서버에 넘겨주기
    void Char_Setting_Send()
    {
        //int : 유닛 아이디
        //byte : 유닛 강화도
        //int : 메인무기
        //int : 보조무기
        //int : 치장1 인덱스
        //int : 치장2 인덱스
        //int : 치장3 인덱스
        //byte : 서브 스킬

        ByteData Send_Buffer = new ByteData(128, 0);

        Send_Buffer.InPutByte(Toggle_Char_Index);
        Send_Buffer.InPutByte(Toggle_Char_Level);
        Send_Buffer.InPutByte(Toggle_Main_Gun_Index);
        Send_Buffer.InPutByte(Toggle_Sub_Gun_Index);
        Send_Buffer.InPutByte(Link_Script.i.Get_ALL_Char_Data[(uint)Toggle_Char_Index].DecoIdx1);
        Send_Buffer.InPutByte(Link_Script.i.Get_ALL_Char_Data[(uint)Toggle_Char_Index].DecoIdx2);
        Send_Buffer.InPutByte(Link_Script.i.Get_ALL_Char_Data[(uint)Toggle_Char_Index].DecoIdx3);
        Send_Buffer.InPutByte(Link_Script.i.Get_ALL_Char_Data[(uint)Toggle_Char_Index].SubSkill);
                       
        Net_Script.Send_Respawn_Char_Init(Send_Buffer);
    }

    //리스폰 시간 연산
    void Respawn_Time_Operation()
    {
        if (!Respawn_Time_Check) return;

        //남은 시간 텍스트 연산
        Respawn_Ramain_Time = Respawn_Time - (DateTime.Now + Play_Script.Play_GameStart_Time);
        Respawn_Time_Text.text = Respawn_Ramain_Time.Seconds + "." + String.Format("{0,2:D2}", (int)(Respawn_Ramain_Time.Milliseconds * 0.1f));

        //원 이미지 채워지는 연산
        Respawn_Circle_Img.fillAmount = 1.0f - ((Respawn_Ramain_Time.Seconds + (Respawn_Ramain_Time.Milliseconds * 0.001f)) * 0.1f);

        if (Respawn_Ramain_Time.Seconds <= 0 && Respawn_Ramain_Time.Milliseconds <= 0)
        {
            Respawn_Time_Text.text = "0.0";
            Respawn_Circle_Img.fillAmount = 1.0f;
        }

        //if (Char_Setting_Check)
        //{
        //    Char_Setting_Check = false;

        //    //리스폰시 캐릭터 셋팅 바뀐값 서버에 넘겨주기
        //    Char_Setting_Send();
        //}
    }
}
