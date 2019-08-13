using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_Script : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{    
    //IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IScrollHandler, IPointerDownHandler, IPointerUpHandler 
    
    GamePlay_Script Game_Script;

    void Start()
    {
        Game_Script = GameObject.Find("Script_Object").GetComponent<GamePlay_Script>();
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (transform.name.Equals("Main_Gun_Button"))//주무기교체 버튼
        {
            Game_Script.Main_Gun_Change_Button();
        }
        else if (transform.name.Equals("Sub_Gun_Button"))//보조무기교체 버튼
        {
            Game_Script.Sub_Gun_Change_Button();
        }        
        else if (transform.name.Equals("Shot_Right_Button"))//오른쪽 발사 버튼
        {
            Game_Script.Shot_Button(BUTTON_DIR_STATE.RIGHT, PLAYER_SHOT_STATE.SHOT_START);
        }
        else if (transform.name.Equals("Shot_Left_Button"))//왼쪽 발사 버튼
        {
            Game_Script.Shot_Button(BUTTON_DIR_STATE.LEFT, PLAYER_SHOT_STATE.SHOT_START);
        }
        else if (transform.name.Equals("Mani_Skill_Button"))//메인 스킬 버튼
        {
            Game_Script.Main_Skill_Button();
        }
        else if (transform.name.Equals("Sub_Skill_Button"))//서브 스킬 버튼
        {
            Game_Script.Sub_Skill_Button();
        }
        else if (transform.name.Equals("Jump_Button"))//점프 버튼
        {
            Game_Script.Jump_Button();
        }
        else if (transform.name.Equals("Grenade_Button"))//수류탄 버튼
        {
            Game_Script.Grenade_Button();
        }
        else if (transform.name.Equals("Zoom_Button"))//줌인,아웃 버튼
        {
            Game_Script.Zoom_Button();
        }
        else if (transform.name.Equals("Button_chat"))//채팅 버튼
        {
            //Game_Script.Chatting_Button();
        }
        else if (transform.name.Equals("Respawn_Chat_Button"))//리스폰 UI 채팅 버튼
        {
            Game_Script.Chatting_Button();
        }
        else if (transform.name.Equals("back_setting_Button"))//환경설정 버튼
        {
            Game_Script.Option_Button();
        }
        else if (transform.name.Equals("Button_Cancle"))//환경설정 닫기 버튼
        {
            Game_Script.Option_Exit_Button();
        }
        else if (transform.name.Equals("Shot_Control_Button"))//게임중 자동,수동 컨트럴 버튼
        {
            Game_Script.Shot_Control_Button();
        }
        else if (transform.name.Equals("Now_respawn_Button"))//즉시 보통 리스폰 버튼
        {
            Game_Script.Now_Respawn_Button(RESPAWN_KIND.NORMAL);
        }
        else if (transform.name.Equals("Now_Helicopter_respawn_Button"))//즉시 헬리콥터 리스폰 버튼
        {
            Game_Script.Now_Respawn_Button(RESPAWN_KIND.HELICOPTER);
        }
        else if (transform.name.Equals("BattleProgress"))//리스폰 화면에서 점수판 화면에 불러오기 버튼
        {
            Game_Script.Progress_Button_Operation(PROGRESS_STATE.RESPAWN_PANEL_OPEN);
        }
        else if (transform.name.Equals("Progress_Button"))//점수판 화면에 불러오기 버튼
        {
            Game_Script.Progress_Button_Operation(PROGRESS_STATE.PANEL_OPEN);
        }
        else if (transform.name.Equals("alpha_battle"))//점수판 버튼 끄기
        {
            Game_Script.Progress_Button_Operation(PROGRESS_STATE.PANEL_CLOSE);
        }
        else if (transform.name.Equals("Button_ok"))//점수판 나가기 버튼
        {
            Game_Script.Progress_Exit_Button();
        }
        else if (transform.name.Equals("Single_GameOver_Button_ok"))//싱글모드 게임 오버 나가기 버튼
        {
            Game_Script.Progress_Exit_Button();
        }
        else if (transform.name.Equals("Exit_Button"))//게임 나가기 패널오픈 버튼
        {
            Game_Script.Exit_UI_Button();
        }
        else if (transform.name.Equals("Exit_Button_Yes"))//게임 나가기 패널의 예 버튼
        {
            Game_Script.Exit_UI_Yes_Button();
        }
        else if (transform.name.Equals("Exit_Button_NO"))//게임 나가기 패널의 아니오 버튼
        {
            Game_Script.Exit_UI_No_Button();
        }        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (transform.name.Equals("Shot_Right_Button"))//오른쪽 발사 버튼
        {
            Game_Script.Shot_Button(BUTTON_DIR_STATE.RIGHT, PLAYER_SHOT_STATE.SHOT_END);
        }

        if (transform.name.Equals("Shot_Left_Button"))//왼쪽 발사 버튼
        {
            Game_Script.Shot_Button(BUTTON_DIR_STATE.LEFT, PLAYER_SHOT_STATE.SHOT_END);
        }

    }

}
