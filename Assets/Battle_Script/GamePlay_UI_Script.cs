using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public enum AIM_COLOR_STATE { RED, WHITE }

public enum BUTTON_DIR_STATE { RIGHT, LEFT, ALL_CHECK }

public enum ZOOM_ANI_STATE { IDEL, ZOOM_IN, ZOOM_END, ZOOM_OUT }

public class GamePlay_UI_Script : MonoBehaviour
{
    GamePlay_Script Play_Script;
    CameraControl_Script CameraControlScript;
    Network_Battle_Script Net_Script;

    public Transform Spot_Image;
    public Transform[] Aim_Image;
    public Transform Aim_Radius_OJ;
    public Image Spot_Color_Image;
    public Image[] Aim_Color_Image;

    public ZOOM_ANI_STATE Zoom_Ani_State;

    public Transform Sniper_Aim_Image;
    public Image[] Sniper_Img_OJ;
    public Transform Sniper_Lockon_Image;

    public Transform Dotsight_Aim_Image;
    public Image[] Dotsight_Img_OJ;

    int Zoom_Ani_Kind = 0;

    float MAX_FieldOfView = 50.0f;
    float Zoom_Ani_Speed = 4.0f;
    float Zoom_Ani_Time = 0.0f;

    public GameObject Shot_Right_Button_Set_OJ;
    public GameObject Shot_Left_Button_Set_OJ;
    public Image Shot_Button_Right_Img;
    public Image Shot_Button_Left_Img;

    public GameObject Sub_Skill_Button_OJ;

    public GameObject Zoom_Button_OJ;

    public Text Main_Gun_RemainBullet;
    public Text Sub_Gun_RemainBullet;
    public Image Main_Gun_BG_Image;
    public Image Sub_Gun_BG_Image;
    public Image Main_Gun_Icon_Image;
    public Image Sub_Gun_Icon_Image;
    public GameObject Main_Gun_Select_OJ;
    public GameObject Sub_Gun_Select_OJ;

    public GameObject Shot_Control_Img_ON_OJ;

    public Image Main_Skill_Image;
    public Image Main_Skill_BG_Image;
    public Image Sub_Skill_Image;    
    public Image Sub_Skill_BG_Image;
    public Image Grenade_Image;

    public GameObject Damage_OJ;

    public GameObject XRay_OJ;

    public GameObject ShotDelay_OJ;
    public Image ShotDelay_Img;

    public GameObject Exit_Button_OJ;
    float Exit_Button_View_Time = 0.0f;

    public Text Time_Text;
    TimeSpan Play_GameRamain_Time;

    public GameObject ComboCount_OJ;
    public Image ComboCount_Num;

    public GameObject Headshot_OJ;

    float Kill_Message_View_Time = 0.0f;
    String Kill_Message_Kill = "";
    String Death_Message_Kill = "";
    public GameObject Kill_Message_OJ;
    public Text Kill_NickName_Text;
    public Text Death_NickName_Text;

    public GameObject PowerUp_Buff_OJ;

    public Text MY_HP_Text;
    public Image MY_HP_Image;

    public GameObject Kill_Num_BG_OJ;
    public GameObject Kill_Num_Training;

    public Text Red_KillCount_Text;
    public Text Red_DeathCount_Text;
    public Text Blue_KillCount_Text;
    public Text Blue_DeathCount_Text;
    ushort Red_Kill_Count = 0;
    //ushort Red_Dead_Count = 0;
    ushort Blue_Kill_Count = 0;
    //ushort Blue_Dead_Count = 0;

    public Text Kill_Num_Training_Text;

    public GameObject Progress_Button_OJ;

    public GameObject Red_Arrow_OJ;
    public GameObject Blue_Arrow_OJ;

    public InputField InputField_OJ;
    
    public GameObject Network_State_OJ;

    float Scope_Magni = 0;
                
    //============================================================================================================================================================================

    void Awake()
    {
        Vector2 Size_Delta = transform.GetComponent<RectTransform>().sizeDelta;
        transform.SetParent(GameObject.Find("Canvas").transform);
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localScale = new Vector3(1, 1, 1);
        transform.GetComponent<RectTransform>().sizeDelta = Size_Delta;

        Play_Script = GameObject.Find("Script_Object").GetComponent<GamePlay_Script>();
        CameraControlScript = GameObject.Find("Camera_Set_Object").GetComponent<CameraControl_Script>();
        Net_Script = GameObject.Find("Network_Script").GetComponent<Network_Battle_Script>();
    }

    void Update()
    {
        if (!Link_Script.i.IsConnected()) return;

        //게임 시간 연산
        GameTime_Operation();

        //화면 상단의 팀 킬,데스 카운트 UI 연산
        Team_Kill_Dead_UI_Operation();

        //스나이퍼 UI 애니메이션
        Sniper_Ani_Operation();

        //죽인유저, 죽은유저 텍스트보여주기 연산
        Killer_Message_Operation();

        //네트웍 상태 체크
        Network_State_Operation();        
    }

    //============================================================================================================================================================================

    //게임 시간 연산
    void GameTime_Operation()
    {
        Play_GameRamain_Time = Play_Script.Play_GameEnd_Time - (DateTime.Now + Play_Script.Play_GameStart_Time);

        Time_Text.text = Play_GameRamain_Time.Minutes + ":" + String.Format("{0,2:D2}", Play_GameRamain_Time.Seconds);

        if (Play_GameRamain_Time.Minutes <= 0 && Play_GameRamain_Time.Seconds <= 0) Time_Text.text = "0:00";

        ////일정 시간 이후 나가기 버튼 보이기
        //if (Exit_Button_OJ.activeSelf == false)
        //{
        //    Exit_Button_View_Time += Time.deltaTime;
        //    if (Exit_Button_View_Time >= 15.0f)
        //    {
        //        Exit_Button_OJ.SetActive(true);
        //    }
        //}
        //Exit_Button_OJ.SetActive(true);
    }
    
    //============================================================================================================================================================================
    
    public void Panel_Init()
    {
        if (Link_Script.i.Play_Mode == BattleKind.ALONE_PLAY_BATTLE)//싱글모드
        {
            Kill_Num_BG_OJ.SetActive(false);
            Kill_Num_Training.SetActive(true);
            Progress_Button_OJ.SetActive(false);
        }
        else
        {
            Kill_Num_BG_OJ.SetActive(true);
            Kill_Num_Training.SetActive(false);
            Progress_Button_OJ.SetActive(true);
        }

        Zoom_Ani_State = ZOOM_ANI_STATE.IDEL;
        Sniper_Aim_Image.gameObject.SetActive(false);
        Sniper_Lockon_Image.gameObject.SetActive(false);
        Dotsight_Aim_Image.gameObject.SetActive(false);

        Sub_Skill_Button_OJ.SetActive(false);

        Shot_Left_Button_View(false);

        Main_Gun_RemainBullet.text = "" + 0;
        Sub_Gun_RemainBullet.text = "" + 0;

        //주,보조 무기 선택되어 있는 UI 체크
        Gun_Select_Init(GUN_EQUIP_STATE.MAIN);

        //게임 종료 목표 킬수
        Red_DeathCount_Text.text = "" + Link_Script.i.GameOver_Kill_Max;
        Blue_DeathCount_Text.text = "" + Link_Script.i.GameOver_Kill_Max;

        ComboCount_Num.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.BattleCombo, 1));
        ComboCount_OJ.SetActive(false);

        PowerUp_Buff_OJ.SetActive(false);

        Headshot_OJ.SetActive(false);
        Kill_Message_OJ.SetActive(false);
        Damage_OJ.SetActive(false);
        XRay_OJ.SetActive(false);
        Red_Arrow_OJ.SetActive(false);
        Blue_Arrow_OJ.SetActive(false);

        if (Link_Script.i.User_Team == 0)
        {
            Red_Arrow_OJ.SetActive(true);
        }
        else
        {
            Blue_Arrow_OJ.SetActive(true);
        }

        //Exit_Button_OJ.SetActive(false);
        //Exit_Button_View_Time = 0.0f;

        Kill_Message_View_Time = 0.0f;
        Kill_Message_Kill = "";
        Death_Message_Kill = "";

        Red_Kill_Count = 0;
        //Red_Dead_Count = 0;
        Blue_Kill_Count = 0;
        //Blue_Dead_Count = 0;
                
        Network_State_OJ.SetActive(false);
    }

    public void Panel_View(bool Check)
    {
        //주,보조 무기 선택되어 있는 UI 체크
        Gun_Select_Init(GUN_EQUIP_STATE.MAIN);

        Damage_OJ.SetActive(false);
        XRay_OJ.SetActive(false);
        ComboCount_OJ.SetActive(false);
        Headshot_OJ.SetActive(false);
                
        if (Check && Play_Script.Death_Buff_Size > 0.0f)
        {            
            PowerUp_Buff_OJ.SetActive(false);
            PowerUp_Buff_OJ.SetActive(true);
        }        

        //this.gameObject.SetActive(Check);

        //자동,수동 컨트럴 버튼이미지 셋팅
        Shot_Control_Button_View(Play_Script.Option_Auto_Shot);

        if (Check) transform.localPosition = new Vector3(0, 0, 0);
        else transform.localPosition = new Vector3(0, 10000, 0);
    }

    //============================================================================================================================================================================

    //자동,수동 컨트럴 버튼이미지 셋팅
    public void Shot_Control_Button_View(bool Check)
    {
        Shot_Control_Img_ON_OJ.SetActive(Check);
    }

    //============================================================================================================================================================================

    //스나이퍼 자동 타켓팅됐을때 나타나는 원형 게이지
    public void ShotDelay_Aim_View(float Remain_Time, float Max_Time)
    {
        if (Remain_Time == 0.0f)
        {
            ShotDelay_OJ.SetActive(false);
        }
        else
        {
            ShotDelay_OJ.SetActive(true);
            ShotDelay_Img.fillAmount = 1.0f - (Remain_Time / Max_Time);
        }        
    }

    //============================================================================================================================================================================

    public void Combo_Effect_View(int Count)
    {
        ComboCount_OJ.SetActive(false);
        ComboCount_OJ.SetActive(true);
        ComboCount_Num.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.BattleCombo, Count));        
    }

    //============================================================================================================================================================================

    public void Zoom_Button_Init(byte _Scope_Magni)
    {        
        if (_Scope_Magni == 0)
        {
            Zoom_Button_OJ.SetActive(false);
        }
        else
        {
            Zoom_Button_OJ.SetActive(true);

            Scope_Magni = ((MAX_FieldOfView - 10) * (_Scope_Magni * 0.1f));
        }
    }

    //============================================================================================================================================================================

    //주,보조 무기 선택되어 있는 UI 체크
    public void Gun_Select_Init(GUN_EQUIP_STATE _Gun_Equip_State)
    {
        Main_Gun_Select_OJ.SetActive(false);
        Sub_Gun_Select_OJ.SetActive(false);

        if (_Gun_Equip_State == GUN_EQUIP_STATE.MAIN)
        {
            Main_Gun_Select_OJ.SetActive(true);
        }
        else
        {
            Sub_Gun_Select_OJ.SetActive(true);
        }
    }

    //============================================================================================================================================================================

    public void Shot_Right_Button_View(bool _Check)
    {
        Shot_Right_Button_Set_OJ.SetActive(_Check);
    }

    public void Shot_Left_Button_View(bool _Check)
    {
        Shot_Left_Button_Set_OJ.SetActive(_Check);
    }

    public void Shot_Button_RayTarget_Init(BUTTON_DIR_STATE _Dir)
    {
        if (_Dir == BUTTON_DIR_STATE.ALL_CHECK)
        {
            Shot_Button_Right_Img.raycastTarget = true;
            Shot_Button_Left_Img.raycastTarget = true;
        }
        else if (_Dir == BUTTON_DIR_STATE.RIGHT)
        {
            Shot_Button_Right_Img.raycastTarget = true;
            Shot_Button_Left_Img.raycastTarget = false;
        }
        else if (_Dir == BUTTON_DIR_STATE.LEFT)
        {
            Shot_Button_Right_Img.raycastTarget = false;
            Shot_Button_Left_Img.raycastTarget = true;
        }
    }

    //============================================================================================================================================================================

    public void Aim_OJ_Active(bool _Check)
    {
        Spot_Image.gameObject.SetActive(_Check);
        Aim_UDLR_OJ_Active(_Check);
    }

    void Aim_UDLR_OJ_Active(bool _Check)
    {
        Aim_Image[0].gameObject.SetActive(_Check);
        Aim_Image[1].gameObject.SetActive(_Check);
        Aim_Image[2].gameObject.SetActive(_Check);
        Aim_Image[3].gameObject.SetActive(_Check);
    } 

    public void Aim_Move_Init(float _Move_Distance)
    {        
        Aim_Image[0].localPosition = new Vector3(0, _Move_Distance, 0);//위
        Aim_Image[1].localPosition = new Vector3(0, -_Move_Distance, 0);//아래
        Aim_Image[2].localPosition = new Vector3(-_Move_Distance, 0, 0);//왼쪽
        Aim_Image[3].localPosition = new Vector3(_Move_Distance, 0, 0);//오른쪽
    }

    public void Aim_Color_Change(AIM_COLOR_STATE Aim_Color_State)
    {
        Color Aim_Color = new Color(ColorResult(255), ColorResult(255), ColorResult(255), 1.0f);

        if (Aim_Color_State == AIM_COLOR_STATE.RED)
        {
            Aim_Color = new Color(ColorResult(255), ColorResult(0), ColorResult(0), 1.0f);
        }
        
        Spot_Color_Image.color = Aim_Color;

        for (int i = 0; i < 4; i++)
        {
            Aim_Color_Image[i].color = Aim_Color;
        }
    }

    float ColorResult(int _Color)
    {
        return (float)(_Color / 255.0f);
    }

    //============================================================================================================================================================================

    //현재 장착된 무기 이미지로 셋팅
    public void Gun_Icon_Init(int Main_Gun_Index, int Sub_Gun_Index)
    {
        Main_Gun_Icon_Image.sprite = ImageManager.instance.Get_Sprite(Main_Gun_Index + "");
        //Sub_Gun_Icon_Image.sprite = ImageManager.instance.Get_Sprite(Sub_Gun_Index + "");
    }

    //탄환감소 UI 정보 갱신
    public void Gun_Bullet_Update(int Main_Remain_Count, int Sub_Remain_Count)
    {
        Main_Gun_RemainBullet.text = "" + Main_Remain_Count;
        Sub_Gun_RemainBullet.text = "" + Sub_Remain_Count;
    }

    //리로드 버튼 아이콘 애니메이션
    public void Reload_UI_Update(GUN_EQUIP_STATE _Gun_Equip_State, float _Value)
    {
        if (_Gun_Equip_State == GUN_EQUIP_STATE.MAIN)
        {
            Main_Gun_BG_Image.fillAmount = Mathf.Lerp(Main_Gun_BG_Image.fillAmount, _Value, Time.deltaTime * 5.0f);
        }
        else if (_Gun_Equip_State == GUN_EQUIP_STATE.SUB)
        {
            Sub_Gun_BG_Image.fillAmount = Mathf.Lerp(Sub_Gun_BG_Image.fillAmount, _Value, Time.deltaTime * 5.0f);
        }
    }

    //============================================================================================================================================================================

    //메인 스킬 이미지로 셋팅
    public void Main_Skill_Icon_Init(int Skill_Index)
    {
        Main_Skill_Image.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.Battleskill, Skill_Index));
        Main_Skill_BG_Image.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.Battleskill, Skill_Index));
    }

    //서브 스킬 이미지로 셋팅
    public void Sub_Skill_Icon_Init(int Skill_Index)
    {
        Sub_Skill_Image.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.Battleskill, Skill_Index));
        Sub_Skill_BG_Image.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.Battleskill, Skill_Index));
    }

    //메인 스킬 버튼 아이콘 애니메이션
    public void Main_Skill_Button_UI_Update(float _Value)
    {
        Main_Skill_Image.fillAmount = _Value;
    }

    //서브 스킬 버튼 아이콘 애니메이션
    public void Sub_Skill_Button_UI_Update(float _Value)
    {
        Sub_Skill_Image.fillAmount = _Value;
    }

    //수류탄 버튼 아이콘 애니메이션
    public void Grenade_Button_UI_Update(float _Value)
    {
        Grenade_Image.fillAmount = _Value;
    }

    //============================================================================================================================================================================

    public void Zoom_UI_Init()
    {
        Zoom_Ani_State = ZOOM_ANI_STATE.IDEL;
        Sniper_Aim_Image.gameObject.SetActive(false);
        Sniper_Lockon_Image.gameObject.SetActive(false);
        Dotsight_Aim_Image.gameObject.SetActive(false);

        Aim_UDLR_OJ_Active(true);

        MAX_FieldOfView = 50.0f;
        Zoom_Ani_Speed = 4.0f;
        Zoom_Ani_Time = 0.0f;

        Sniper_Aim_Image.localScale = new Vector3(1.0f, 1.0f, 0.0f);
        for (int i = 0; i < Sniper_Img_OJ.Length; i++)
        {            
            Sniper_Img_OJ[i].color = new Color(Sniper_Img_OJ[i].color.r, Sniper_Img_OJ[i].color.g, Sniper_Img_OJ[i].color.b, 1);            
        }

        Dotsight_Aim_Image.localScale = new Vector3(1.0f, 1.0f, 0.0f);
        for (int i = 0; i < Dotsight_Img_OJ.Length; i++)
        {
            Dotsight_Img_OJ[i].color = new Color(Dotsight_Img_OJ[i].color.r, Dotsight_Img_OJ[i].color.g, Dotsight_Img_OJ[i].color.b, 1);            
        }

        CameraControlScript.Camera_FieldOfView = MAX_FieldOfView;
    }

    public void Zoom_Ani_Init(ZOOM_ANI_STATE _Sniper_Ani_State, int _Zoom_Ani_Kind = 0)
    {
        Zoom_Ani_State = _Sniper_Ani_State;

        switch (Zoom_Ani_State)
        {
            case ZOOM_ANI_STATE.IDEL:

                Sniper_Aim_Image.gameObject.SetActive(false);
                Sniper_Lockon_Image.gameObject.SetActive(false);

                Dotsight_Aim_Image.gameObject.SetActive(false);

                Play_Script.Shot_Button(BUTTON_DIR_STATE.LEFT, PLAYER_SHOT_STATE.SHOT_END);
                Shot_Left_Button_View(false);

                break;
            case ZOOM_ANI_STATE.ZOOM_IN:

                Zoom_Ani_Kind = _Zoom_Ani_Kind;

                if (Zoom_Ani_Kind == 0)
                {
                    Sniper_Aim_Image.gameObject.SetActive(true);
                    Sniper_Lockon_Image.gameObject.SetActive(false);
                }
                else if (Zoom_Ani_Kind == 1)
                {
                    Dotsight_Aim_Image.gameObject.SetActive(true);
                    Aim_UDLR_OJ_Active(false);
                }

                //수동일때만 왼쪽 샷버튼 보인다.
                if (Play_Script.Option_Auto_Shot == false)
                {
                    Shot_Left_Button_View(true);
                }                

                break;
            case ZOOM_ANI_STATE.ZOOM_END:

                //if (Zoom_Ani_Kind == 0)
                //{
                //    Sniper_Lockon_Image.gameObject.SetActive(true);
                //}

                break;
            case ZOOM_ANI_STATE.ZOOM_OUT:

                if (Zoom_Ani_Kind == 0)
                {
                    Sniper_Aim_Image.gameObject.SetActive(true);
                    Sniper_Lockon_Image.gameObject.SetActive(false);
                }
                else if (Zoom_Ani_Kind == 1)
                {
                    Dotsight_Aim_Image.gameObject.SetActive(true);
                    Aim_UDLR_OJ_Active(true);
                }

                break;
        }
    }

    //스나이퍼 UI 애니메이션
    void Sniper_Ani_Operation()
    {
        switch (Zoom_Ani_State)
        {
            case ZOOM_ANI_STATE.IDEL:
                break;
            case ZOOM_ANI_STATE.ZOOM_IN:
            case ZOOM_ANI_STATE.ZOOM_OUT:

                if (Zoom_Ani_State == ZOOM_ANI_STATE.ZOOM_IN)
                {
                    Zoom_Ani_Time += Time.deltaTime * Zoom_Ani_Speed;
                    if (Zoom_Ani_Time >= 1.0f)
                    {
                        Zoom_Ani_Time = 1.0f;
                        Zoom_Ani_Init(ZOOM_ANI_STATE.ZOOM_END);
                    }
                }
                else if (Zoom_Ani_State == ZOOM_ANI_STATE.ZOOM_OUT)
                {
                    Zoom_Ani_Time -= Time.deltaTime * Zoom_Ani_Speed;
                    if (Zoom_Ani_Time <= 0.0f)
                    {
                        Zoom_Ani_Time = 0.0f;
                        Zoom_Ani_Init(ZOOM_ANI_STATE.IDEL);
                    }
                }

                Sniper_Aim_Image.localScale = new Vector3(2.0f - Zoom_Ani_Time, 2.0f - Zoom_Ani_Time, 0);
                for (int i = 0; i < Sniper_Img_OJ.Length; i++)
                {
                    Sniper_Img_OJ[i].color = new Color(Sniper_Img_OJ[i].color.r, Sniper_Img_OJ[i].color.g, Sniper_Img_OJ[i].color.b, Zoom_Ani_Time);
                }

                Dotsight_Aim_Image.localScale = new Vector3(2.0f - Zoom_Ani_Time, 2.0f - Zoom_Ani_Time, 0);
                for (int i = 0; i < Dotsight_Img_OJ.Length; i++)
                {
                    Dotsight_Img_OJ[i].color = new Color(Dotsight_Img_OJ[i].color.r, Dotsight_Img_OJ[i].color.g, Dotsight_Img_OJ[i].color.b, Zoom_Ani_Time);
                }

                CameraControlScript.Camera_FieldOfView = MAX_FieldOfView - (Zoom_Ani_Time * Scope_Magni);

                break;
            case ZOOM_ANI_STATE.ZOOM_END:
                break;
        }
    }

    //============================================================================================================================================================================

    public void Damage_UI_View()
    {
        Damage_OJ.SetActive(false);
        Damage_OJ.SetActive(true);
    }

    //============================================================================================================================================================================

    //투시 스킬 UI 효과
    public void XRay_View(bool Check)
    {
        XRay_OJ.SetActive(Check);
    }

    //============================================================================================================================================================================

    public void MY_HP_UI_Init(float MY_HP, float _Value)
    {
        MY_HP_Text.text = (int)MY_HP + "";
        MY_HP_Image.fillAmount = Mathf.Lerp(MY_HP_Image.fillAmount, _Value, Time.deltaTime * 5.0f);
    }

    //============================================================================================================================================================================

    //화면 상단의 팀 킬,데스 카운트 UI
    public void Team_Kill_Dead_UI_Init(ushort _Red_Kill_Count, ushort _Red_Dead_Count, ushort _Blue_Kill_Count, ushort _Blue_Dead_Count)
    {
        Red_Kill_Count = _Red_Kill_Count;
        //Red_Dead_Count = _Red_Dead_Count;
        Blue_Kill_Count = _Blue_Kill_Count;
        //Blue_Dead_Count = _Blue_Dead_Count;
    }

    //화면 상단의 팀 킬,데스 카운트 UI 연산
    void Team_Kill_Dead_UI_Operation()
    {
        Red_KillCount_Text.text = "" + Red_Kill_Count;
        Blue_KillCount_Text.text = "" + Blue_Kill_Count;
    }

    //헤드샷 효과
    public void Headshot_UI_View()
    {
        Headshot_OJ.SetActive(false);
        Headshot_OJ.SetActive(true);
    }

    //죽인유저, 죽은유저 텍스트 처리 셋팅
    public void Killer_Message(String _Kill_NickName_Text, String _Death_NickName_Text)
    {
        Kill_Message_View_Time = 3.0f;

        Kill_Message_Kill = _Kill_NickName_Text;
        Death_Message_Kill = _Death_NickName_Text;        
    }

    //죽인유저, 죽은유저 텍스트보여주기 연산
    void Killer_Message_Operation()
    {
        if (Kill_Message_View_Time == 0.0f) return;

        if (Kill_Message_View_Time == 3.0f)
        {
            Kill_NickName_Text.text = Kill_Message_Kill;
            Death_NickName_Text.text = Death_Message_Kill;

            Kill_Message_OJ.SetActive(false);
            Kill_Message_OJ.SetActive(true);
        }

        Kill_Message_View_Time -= Time.deltaTime;
        if (Kill_Message_View_Time <= 0.0f)
        {
            Kill_Message_View_Time = 0.0f;

            Kill_Message_OJ.SetActive(false);
        }
    }

    //============================================================================================================================================================================

    //네트웍 상태 체크
    void Network_State_Operation()
    {
        if (Net_Script == null) return;

        if (Net_Script.Network_Delay_Check)
        {
            Network_State_OJ.SetActive(true);
        }
        else
        {
            Network_State_OJ.SetActive(false);
        }
    }

    //============================================================================================================================================================================
}
