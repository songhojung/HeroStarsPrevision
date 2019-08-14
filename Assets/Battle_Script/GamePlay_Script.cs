
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public enum GAMEPLAY_STATE { IDEL, INIT, GAME_READY, GAME_PLAY, GAME_OVER_INIT, GAME_OVER_WAIT, GAME_OVER_WEB_SEND, GAME_OVER_RESULT, GAME_OUT_INIT, GAME_OUT, NETWORK_DISCONNECT, ERROR, ERROR_WAIT }

public enum PLAYER_SHOT_STATE { IDEL, SHOT_START, SHOT_LAUNCH, SHOT_END, SHOT_SNIPER_END }

public enum GAMEOVER_WIN_TEAM { RED, BLUE, DRAW }

public enum RESPAWN_KIND { NORMAL, HELICOPTER }

public class GamePlay_Script : MonoBehaviour
{
    #region sigletone
    private static GamePlay_Script _instance;
    public static GamePlay_Script Getsingleton
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(GamePlay_Script)) as GamePlay_Script;


            }

            return _instance;
        }
    }

    #endregion

    string deviceUniqueIdentifier;

    public GAMEPLAY_STATE GamePlay_State;
    public PLAYER_SHOT_STATE Player_Shot_State;    

    CameraControl_Script CameraControlScript = null;

    Network_Battle_Script Net_Script = null;

    GamePlay_UI_Script GamePlay_UI = null;

    Respawn_UI_Script Respawn_UI = null;

    Option_Script Option_UI = null;

    Progress_Script Progress_UI = null;

    Single_GameOver_Script Single_GameOver_UI = null;

    Exit_UI_Script Exit_UI = null;

    public Map_Script MapScript = null;

    DestroyMode_Script Destroy_Mode_Script = null;

    Joystick_Script Move_Joystick = null;
    Joystick_Script Rotation_Joystick = null;

    public Dictionary<uint, Player_Script> Char_Script = new Dictionary<uint, Player_Script>();

    public Dictionary<String, Grenade_Script> Grenade_Data = new Dictionary<String, Grenade_Script>();

    public Dictionary<uint, BOT_Script> Bot_Script = new Dictionary<uint, BOT_Script>();

    Canvas Canvas_OJ = null;
    float Canvas_OJ_Width = 0.0f;

    private const int LEFT = 0;
    private const int RIGHT = 1;
    int[] Cursor_State = new int[2];
    int[] Touch_FingerId = new int[2];

    int Already_Char_Count = 0;

    uint MY_INDEX = 0;

    public bool GameOver_Check = false;
    GAMEOVER_WIN_TEAM GameOver_Win_Team;    
    int GameOver_Reward_Check = 0;
    float GameOver_Net_Check_Time = 0.0f;

    //float Sniper_ShotAni_Delay_Time = 0.0f;
    public float Aim_Move_Size = 0.0f;    

    public float Option_Sensitive = 0.0f;
    public float Option_Sensitive_Zoom = 0.0f;
    public bool Option_Auto_Shot = false;

    int Sniper_Auto_Target_State = 0;
    float Sniper_Auto_Target_Time = 0.0f;

    float Skill_Delay_Time = 0.0f;
    float Grenade_Delay_Time = 0.0f;

    public TimeSpan Play_GameStart_Time;
    public DateTime Play_GameEnd_Time;

    bool HeadShot_UI_Check = false;
    bool Respawn_UI_Check = false;
    public long Respawn_Server_Time = 0;
    String Respawn_Player_Killer_NickName = "";
    String Respawn_Killing_Message = "";
    ushort Respawn_Player_Kill_Count = 0;
    ushort Respawn_Player_Death_Count = 0;

    bool Combo_Check = false;
    int Combo_Count = 0;

    public float Death_Buff_Size = 0.0f;

    public int Bot_Kill_Count = 0;

    bool Keyboard_Check = false;
    TouchScreenKeyboard Keyboard;
    String Keyboard_String = "";

    Ray Targeting_Ray;
    RaycastHit Targeting_RaycastHit;        

    EXIT_UI_STATE Exit_UI_State;

    public OPTION_STATE Option_State;

    public PROGRESS_STATE Progress_State;

    public RESPAWN_KIND Respawn_Kind;

    public int Bot_Make_Count = 0;

    private void Awake()
    {
        _instance = this; 
    }

    void Start()
    {

#if UNITY_EDITOR
        deviceUniqueIdentifier = "Emulator";//예뮬레이터 유니크 ID 
#else
        deviceUniqueIdentifier = "Device";//디바이스 유니크 ID
#endif
        CameraControlScript = GameObject.Find("Camera_Set_Object").GetComponent<CameraControl_Script>();

        Net_Script = GameObject.Find("Network_Script").GetComponent<Network_Battle_Script>();

        MapScript = transform.GetComponent<Map_Script>();
        MapScript.Map_Init(Link_Script.i.MapIndex);

        if (Link_Script.i.Play_Mode == BattleKind.ALONE_PLAY_BATTLE)//싱글모드
        {
            MapScript.Map_Bot_Init(Link_Script.i.MapIndex);
        }
                        
        Canvas_OJ = GameObject.Find("Canvas").GetComponent<Canvas>();
        Canvas_OJ.worldCamera = CameraControlScript.Main_Camera;
        Canvas_OJ_Width = Canvas_OJ.worldCamera.pixelWidth;

        GamePlay_State = GAMEPLAY_STATE.IDEL;
        Player_Shot_State = PLAYER_SHOT_STATE.IDEL;        
    }


    void Update()
    {
        if (!Link_Script.i.IsConnected()) return;

        ////프레임 보기
        //FrameView();

        Main_Operation();

        //test
        if(Input.GetKeyDown(KeyCode.F5))
        {
            ForceCreate_ObtainItem();
        }
    }


    void Main_Operation()
    {
        switch (GamePlay_State)
        {
            case GAMEPLAY_STATE.IDEL:
            case GAMEPLAY_STATE.INIT:

                Already_Char_Count = 0;

                //------------------------------------------------------------------------------------------------------------------------------------------------------

                ////음성채팅 만들기
                //Voice_Make_OJ();

                //플레이 UI 띄우기
                Make_GamePlay_UI_OJ();

                //이동, 각도 조절 이미지 만들기
                Make_Joystick_OJ();

                //리스폰 UI 만들기
                Make_Respawn_UI_OJ();

                //채팅 UI 생성
                SendManager.Instance.Create_ChatUI();

                //------------------------------------------------------------------------------------------------------------------------------------------------------

                MY_INDEX = Link_Script.i.User_ID;
                String Object_Name = MY_INDEX + "";

                //캐릭터 만들기
                Transform Char_OJ = Make_Char_OJ(Object_Name);

                Char_Script.Add(MY_INDEX, Char_OJ.GetComponent<Player_Script>());

                Char_Script[MY_INDEX].Object_Name = Object_Name;

                Char_Script[MY_INDEX].Char_Init(CHAR_USER_KIND.PLAYER, CHAR_TEAM_SATAE.PLAYER_TEAM);

                Char_Script[MY_INDEX].Char_Respawn_Pos_Init(Link_Script.i.User_Data[MY_INDEX].Start_Pos_Index);

                //카메라 움직임 관련 큐브 초기화
                CameraControlScript.Control_Cube_Init(Char_Script[MY_INDEX].transform);

                //------------------------------------------------------------------------------------------------------------------------------------------------------

                if (Link_Script.i.Play_Mode == BattleKind.WAR_OF_POSITION)
                {
                    Object_Name = "Destroy_Mode_Point_Set_" + Link_Script.i.MapIndex;

                    Transform Destroy_Transform = Instantiate(Resources.Load("Battle_Prefab/1_Game_OJ_Folder/" + Object_Name) as GameObject).GetComponent<Transform>();
                    Destroy_Transform.position = new Vector3(0, 0, 0);
                    Destroy_Transform.localScale = new Vector3(1, 1, 1);

                    Destroy_Mode_Script = Destroy_Transform.GetComponent<DestroyMode_Script>();

                    Destroy_Mode_Script.HP_Init(Char_Script[MY_INDEX].User_Team);
                }
                else if (Link_Script.i.Play_Mode == BattleKind.ALONE_PLAY_BATTLE)//싱글모드
                {
                    Bot_Make_Count = 3;                    
                }
                
                //------------------------------------------------------------------------------------------------------------------------------------------------------

                GameOver_Check = false;
                GameOver_Win_Team = GAMEOVER_WIN_TEAM.DRAW;                
                GameOver_Reward_Check = 0;
                GameOver_Net_Check_Time = 0.0f;

                //Sniper_ShotAni_Delay_Time = 0.0f;
                Aim_Move_Size = 0.0f;

                Sniper_Auto_Target_State = 0;
                Sniper_Auto_Target_Time = 0.0f;

                //스나이퍼 자동 타켓팅됐을때 나타나는 원형 게이지
                GamePlay_UI.ShotDelay_Aim_View(0.0f, 0.0f);

                Skill_Delay_Time = 0.0f;
                Grenade_Delay_Time = 0.0f;

                HeadShot_UI_Check = false;

                Combo_Check = false;
                Combo_Count = 0;
                                
                Death_Buff_Size = 0.0f;
                
                Progress_State = PROGRESS_STATE.IDEL;
                
                Progress_Info_Init();

                Left_Cursor_Init();
                Right_Cursor_Init();

                Option_State = OPTION_STATE.IDEL;

                //환경설정값 셋팅
                Option_Init();

                Exit_UI_State = EXIT_UI_STATE.IDEL;

                Respawn_Kind = RESPAWN_KIND.NORMAL;

                //현재 전체 스코어 요청(난입했을때 바로 UI 스코어 연동될수 있도록)
                Net_Script.Send_Now_Total_Score();

                Bot_Kill_Count = 0;
                GamePlay_UI.Kill_Num_Training_Text.text = "0";

                //------------------------------------------------------------------------------------------------------------------------------------------------------

                GamePlay_State = GAMEPLAY_STATE.GAME_READY;

                break;
            case GAMEPLAY_STATE.GAME_READY:

                //탄환감소 UI 정보 갱신
                GamePlay_UI.Gun_Bullet_Update(Char_Script[MY_INDEX].Gun_Bullet[(int)GUN_EQUIP_STATE.MAIN], Char_Script[MY_INDEX].Gun_Bullet[(int)GUN_EQUIP_STATE.SUB]);

                GamePlay_State = GAMEPLAY_STATE.GAME_PLAY;

                break;
            case GAMEPLAY_STATE.GAME_PLAY:

                //봇 만들기
                Bot_Make_Init();

                //플레이어 움직임 연산 
                PlayerMove_Operation();
                                
                //타겟팅 체크 연산
                Auto_Targeting_Operation();

                //스나이퍼 오토 타겟팅 되는 시간 연산
                Sniper_Auto_Target_Time_Operation();

                //키보드 입력 체크
                Keyboard_Operation();

                //유저 들어 오고, 나가는 표시
                Network_In_Out_Operation();

                //네트웍 캐릭터 움직임 연산
                Char_Move_Operation();

                //수류탄 연산
                Char_Grenade_Operation();

                //플레이어 총쏘는 연산
                MY_Shot_Operation();

                //조준점 벌어짐 연산
                Aim_Operation();

                //플레이어 UI 효과 연산
                Play_UI_Operation();

                //중간 집계 상황판 보기
                Progress_Operation();

                //안드로이드 취소키 처리
                ESC_Key_Operation();

                //게임 오버 받으면 일정시간 딜레이후 넘어간다. 게임오버 바로 넘어가면 캐릭터 죽는게 스킵 되버린다.
                GameOver_Net_Check();

                //Hj: 아이템 작동관련
                Operation_ObtainItem();

                break;
            case GAMEPLAY_STATE.GAME_OVER_INIT:

                GameOver_Check = true;

                Left_Cursor_Init();
                Right_Cursor_Init();

                //게임 오버시 캐릭터별 셋팅
                foreach (var _Char_Script in Char_Script)
                {
                    _Char_Script.Value.GameOver_Char_Init();
                }

                //게임 UI 끄기
                GamePlay_UI.Panel_View(false);

                //화면의 키보드 닫기
                Keyboard_Close();

                //채팅 UI 삭제하기
                SendManager.Instance.Clear_ChatUI();

                //리스폰 UI 끄기
                if (Respawn_UI != null) Respawn_UI.gameObject.SetActive(false);

                //게임 나가기 UI 끄기
                if (Exit_UI != null) Exit_UI.Exit_UI_View(false);

                //환경설정 UI 끄기
                if (Option_UI != null) Option_UI.Option_View(false);

                if (Link_Script.i.Play_Mode == BattleKind.ALONE_PLAY_BATTLE)//싱글모드
                {
                    //싱글모드 게임 오버 UI 만들기
                    Make_Bot_Mode_GameOver_UI_OJ();

                    Single_GameOver_UI.Panel_Play_Init(Bot_Kill_Count);
                }
                else
                {
                    ////현재 전체 스코어 요청
                    //Net_Script.Send_Now_Total_Score();

                    //현재 스코어 정보창 만들기
                    Make_Battleprogress_OJ();

                    Progress_UI.Panel_Play_Init();

                    if (GameOver_Win_Team == GAMEOVER_WIN_TEAM.DRAW)
                    {
                        Progress_UI.Panel_GameOver_Text_View(DefineKey.text_Draw);
                    }
                    else if (GameOver_Win_Team == GAMEOVER_WIN_TEAM.RED)
                    {
                        if ((GAMEOVER_WIN_TEAM)Char_Script[MY_INDEX].User_Team == GAMEOVER_WIN_TEAM.RED)
                        {
                            Progress_UI.Panel_GameOver_Text_View(DefineKey.text_Win);
                        }
                        else
                        {
                            Progress_UI.Panel_GameOver_Text_View(DefineKey.text_Lose);
                        }
                    }
                    else if (GameOver_Win_Team == GAMEOVER_WIN_TEAM.BLUE)
                    {
                        if ((GAMEOVER_WIN_TEAM)Char_Script[MY_INDEX].User_Team == GAMEOVER_WIN_TEAM.BLUE)
                        {
                            Progress_UI.Panel_GameOver_Text_View(DefineKey.text_Win);
                        }
                        else
                        {
                            Progress_UI.Panel_GameOver_Text_View(DefineKey.text_Lose);
                        }
                    }

                    Progress_UI.Panel_View(true);

                    Progress_UI.BG_Img_OJ_View(false);
                }                

                GamePlay_State = GAMEPLAY_STATE.GAME_OVER_WAIT;

                break;
            case GAMEPLAY_STATE.GAME_OVER_WAIT:

                if (GameOver_Reward_Check == 1)
                {
                    GameOver_Reward_Check = 2;

                    //유저 정보 데이터 갱신시키기
                    SendManager.Instance.Protocol_GameEndRfsData(User_Data_Updata_OK);
                }
                else if (GameOver_Reward_Check == 2)
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        Progress_Exit_Button();
                    }
                }

                break;
            case GAMEPLAY_STATE.GAME_OVER_WEB_SEND:

                byte Win_Team = 1;

                if (Link_Script.i.Play_Mode == BattleKind.ALONE_PLAY_BATTLE)//싱글모드
                {
                    Single_GameOver_UI.Panel_View(false);

                    Win_Team = 0;
                }
                else
                {
                    Progress_UI.Panel_View(false);

                    if (GameOver_Win_Team == GAMEOVER_WIN_TEAM.DRAW)
                    {
                        Win_Team = 2;
                    }
                    else if (GameOver_Win_Team == GAMEOVER_WIN_TEAM.RED)
                    {
                        if ((GAMEOVER_WIN_TEAM)Char_Script[MY_INDEX].User_Team == GAMEOVER_WIN_TEAM.RED) Win_Team = 0;
                    }
                    else if (GameOver_Win_Team == GAMEOVER_WIN_TEAM.BLUE)
                    {
                        if ((GAMEOVER_WIN_TEAM)Char_Script[MY_INDEX].User_Team == GAMEOVER_WIN_TEAM.BLUE) Win_Team = 0;
                    }
                }

                SendManager.Instance.Create_ResultUI(Win_Team);

                GamePlay_State = GAMEPLAY_STATE.GAME_OVER_RESULT;

                break;
            case GAMEPLAY_STATE.GAME_OVER_RESULT:
                break;
            case GAMEPLAY_STATE.GAME_OUT_INIT:

                //게임 나가기 셋팅
                GameOver_Init(true);

                GamePlay_State = GAMEPLAY_STATE.GAME_OUT;

                break;
            case GAMEPLAY_STATE.GAME_OUT:
                break;
            case GAMEPLAY_STATE.NETWORK_DISCONNECT:
                break;
            case GAMEPLAY_STATE.ERROR:

                break;
            case GAMEPLAY_STATE.ERROR_WAIT:

                break;
        }
    }

    //====================================================================================================================================================================================================

    //캐릭터 만들기
    Transform Make_Char_OJ(String Object_Name)
    {
        //Destroy(gameObject.GetComponent<Player_Script>());//컨포넌트 삭제
        //Char_OJ.gameObject.AddComponent<Player_Script>();//컨포넌트 삽입        

        //if (Already_Char_Count < 12)
        //{
        //    GameObject.Find("CH_SET_" + Already_Char_Count).name = Object_Name;
        //    Already_Char_Count++;
        //}
        //else
        //{
        //    Instantiate(Resources.Load("Battle_Prefab/0_Unit_Folder/CH_SET") as GameObject).name = Object_Name;
        //}

        
        Instantiate(Resources.Load("Battle_Prefab/0_Unit_Folder/CH_SET") as GameObject).name = Object_Name;
        Transform Char_OJ = GameObject.Find(Object_Name).GetComponent<Transform>();

        Char_OJ.parent = null;

        return Char_OJ;
    }

    //====================================================================================================================================================================================================

    //봇 만들기
    public void Bot_Make_Init()
    {
        if (Link_Script.i.Play_Mode != BattleKind.ALONE_PLAY_BATTLE) return;

        if (Bot_Make_Count > 0)
        {
            while (true)
            {
                uint Bot_ID = (uint)UnityEngine.Random.Range(0, MapScript.Map_Bot_Point_Pos.Length);
                                
                if (Bot_Script.ContainsKey(Bot_ID)) continue;

                String Object_Name = "BOT_" + Bot_ID;

                Instantiate(Resources.Load("Battle_Prefab/0_Unit_Folder/BOT") as GameObject).name = Object_Name;
                Transform Bot_OJ = GameObject.Find(Object_Name).GetComponent<Transform>();
                Bot_OJ.position = MapScript.Map_Bot_Point_Pos[Bot_ID];

                Bot_Script.Add(Bot_ID, Bot_OJ.GetComponent<BOT_Script>());
                Bot_Script[Bot_ID].Bot_Init();
                Bot_Script[Bot_ID].Object_Name = Object_Name;
                Bot_Script[Bot_ID].User_ID = Bot_ID;
                Bot_Script[Bot_ID].Start_Rotation_Pos = Char_Script[MY_INDEX].transform.position;
                Bot_Script[Bot_ID].User_Team = 0;
                Bot_Script[Bot_ID].MY_HP = 100;
                Bot_Script[Bot_ID].MY_MAX_HP = 100;
                Bot_Script[Bot_ID].Head_Text_Make();
                Bot_Script[Bot_ID].User_NickName = "FRANKEN";

                Transform BOT_Respawn_OJ = Instantiate(Resources.Load("Battle_Prefab/3_Effect_Folder/BOT_Respawn") as GameObject).GetComponent<Transform>();
                BOT_Respawn_OJ.position = MapScript.Map_Bot_Point_Pos[Bot_ID];

                Bot_Make_Count--;

                break;
            } 
        }        
    }

    //====================================================================================================================================================================================================

    bool PC_Control_Check = false;

    float Key_X = 0;
    float Key_Y = 0;
    int Key_X_State = 0;
    int Key_Y_State = 0;
    
    //플레이어 움직임 연산 
    void PlayerMove_Operation()
    {
        //Progress_State = PROGRESS_STATE.PANEL_VIEW;

        if (Char_Script[MY_INDEX].MY_HP == 0.0f || Progress_State != PROGRESS_STATE.IDEL || Option_State != OPTION_STATE.IDEL || Exit_UI_State != EXIT_UI_STATE.IDEL) return;

        if (deviceUniqueIdentifier.Equals("Emulator"))//예뮬용 연산
        {
            //-------------------------------------------------------------------------------------------------

            if (Input.GetKeyDown(KeyCode.F1))
            {
                if (PC_Control_Check)
                {
                    PC_Control_Check = false;

                    Cursor.lockState = CursorLockMode.None;
                }
                else
                {
                    PC_Control_Check = true;

                    Cursor.lockState = CursorLockMode.Locked;
                    

                    Camera_Rotation_State = 0;

                    //카메라 각도 연산
                    CameraRotation(Canvas_OJ_Width / 2, Canvas_OJ.worldCamera.pixelHeight / 2);

                    GamePlay_UI.Zoom_Button_OJ.SetActive(false);

                    //GamePlay_UI.Shot_Button_Left_Img.enabled = false;

                    //자동, 수동일때에 따른 샷버튼 이미지 그리기 셋팅
                    GamePlay_UI.Shot_Right_Button_View(false);
                }
            }

            //-------------------------------------------------------------------------------------------------


            if (PC_Control_Check == false)
            {

                if (Input.GetKeyDown(KeyCode.A)) Key_X_State = -1;
                if (Input.GetKeyDown(KeyCode.D)) Key_X_State = 1;
                if (Input.GetKeyDown(KeyCode.W)) Key_Y_State = 1;
                if (Input.GetKeyDown(KeyCode.S)) Key_Y_State = -1;

                if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
                {
                    Key_X = 0;
                    Key_X_State = 0;
                }
                if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
                {
                    Key_Y = 0;
                    Key_Y_State = 0;
                }

                Key_X += Time.deltaTime * (CameraControlScript.Camera_PixelWidth * 1.0f) * Key_X_State;
                Key_Y += Time.deltaTime * (CameraControlScript.Camera_PixelHeight * 1.0f) * Key_Y_State;

                if (Key_X_State != 0 || Key_Y_State != 0)
                {
                    Cursor_State[LEFT] = 1;

                    //캐릭터 움직임 연산
                    CharControlMove(Key_X, Key_Y);
                }

                if (Input.GetMouseButton(0))//마우스 입력중
                {
                    float X_Point = Input.mousePosition.x;
                    float Y_Point = Input.mousePosition.y;


                    //if (Cursor_State[LEFT] == 0 && Canvas_OJ_Width / 2 > X_Point)//화면 왼쪽
                    //{
                    //    Cursor_State[LEFT] = 1;
                    //}

                    //if (Cursor_State[LEFT] == 1 && Key_X_State == 0 && Key_Y_State == 0)
                    //{
                    //    //커서 이미지 움직임 연산
                    //    Move_Joystick.CursorImage_Move(X_Point, Y_Point);

                    //    //캐릭터 움직임 연산
                    //    CharControlMove(X_Point, Y_Point);
                    //}



                    if (Cursor_State[RIGHT] == 0 && Canvas_OJ_Width / 2 <= X_Point)//화면 오른쪽
                    {
                        Cursor_State[RIGHT] = 1;
                    }

                    if (Cursor_State[RIGHT] == 1)
                    {
                        //Rotation_Joystick.CursorImage_Move(X_Point, Y_Point);//커서 이미지 움직임 연산

                        //카메라 각도 연산
                        CameraRotation(X_Point, Y_Point);
                    }
                }

                //if (Input.GetMouseButton(0) == false && Key_X_State == 0 && Key_Y_State == 0 && Cursor_State[LEFT] == 1) Left_Cursor_Init();
                if (Key_X_State == 0 && Key_Y_State == 0 && Cursor_State[LEFT] == 1) Left_Cursor_Init();

                if (Input.GetMouseButton(0) == false && Cursor_State[RIGHT] == 1) Right_Cursor_Init();

            }
            else//PC용 컨트럴 연산
            {
                //------------------------------------------------------------------------------------------

                if (Input.GetKeyDown(KeyCode.A))
                {
                    Key_X = 0;
                    Key_X_State = -1;
                }
                else if (Input.GetKeyUp(KeyCode.A) && Key_X_State == -1)
                {
                    Key_X = 0;
                    Key_X_State = 0;
                }

                if (Input.GetKeyDown(KeyCode.D))
                {
                    Key_X = 0;
                    Key_X_State = 1;
                }
                else if (Input.GetKeyUp(KeyCode.D) && Key_X_State == 1)
                {
                    Key_X = 0;
                    Key_X_State = 0;
                }
                
                if (Input.GetKeyDown(KeyCode.S))
                {
                    Key_Y = 0;
                    Key_Y_State = -1;
                }
                else if (Input.GetKeyUp(KeyCode.S) && Key_Y_State == -1)
                {
                    Key_Y = 0;
                    Key_Y_State = 0;
                }

                if (Input.GetKeyDown(KeyCode.W))
                {
                    Key_Y = 0;
                    Key_Y_State = 1;
                }
                else if (Input.GetKeyUp(KeyCode.W) && Key_Y_State == 1)
                {
                    Key_Y = 0;
                    Key_Y_State = 0;
                }


                Key_X += Time.deltaTime * (CameraControlScript.Camera_PixelWidth * 1.0f) * Key_X_State;
                Key_Y += Time.deltaTime * (CameraControlScript.Camera_PixelHeight * 1.0f) * Key_Y_State;
                                
                //캐릭터 움직임 연산
                CharControlMove(Key_X, Key_Y);

                //------------------------------------------------------------------------------------------
                
                float UD_Speed = 500.0f;
                float LF_Speed = 500.0f;

                float Mouse_X = Input.GetAxis("Mouse X");
                float Mouse_Y = Input.GetAxis("Mouse Y");
                
                float Temp_X = CameraControlScript.Camera_Center_OJ_EulerAngles().x - (Mouse_Y * Camera_Rotation_Speed * (Time.deltaTime * UD_Speed));
                float Temp_Y = CameraControlScript.Camera_Center_OJ_EulerAngles().y + (Mouse_X * Camera_Rotation_Speed * (Time.deltaTime * LF_Speed));

                if (GamePlay_UI.Zoom_Ani_State != ZOOM_ANI_STATE.IDEL)//줌인 상태
                {
                    Temp_X = CameraControlScript.Camera_Center_OJ_EulerAngles().x - (Mouse_Y * Camera_Rotation_Zoom_Speed * (Time.deltaTime * UD_Speed));
                    Temp_Y = CameraControlScript.Camera_Center_OJ_EulerAngles().y + (Mouse_X * Camera_Rotation_Zoom_Speed * (Time.deltaTime * LF_Speed));
                }

                if (Temp_X >= 45 + Camera_Max_Angle && Temp_X <= 180) Temp_X = 45 + Camera_Max_Angle;
                else if (Temp_X >= 180 && Temp_X <= 315 - Camera_Max_Angle) Temp_X = 315 - Camera_Max_Angle;

                CameraControlScript.Camera_Center_OJ_Transform().rotation = Quaternion.Euler(Temp_X, Temp_Y, 0);

                //------------------------------------------------------------------------------------------

                if (Input.GetMouseButtonDown(0))
                {
                    Shot_Button(BUTTON_DIR_STATE.RIGHT, PLAYER_SHOT_STATE.SHOT_START);
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    Shot_Button(BUTTON_DIR_STATE.RIGHT, PLAYER_SHOT_STATE.SHOT_END);
                }

                //------------------------------------------------------------------------------------------

                if (Input.GetMouseButtonDown(1))
                {
                    if (Char_Script[MY_INDEX].Gun_Equip_State == GUN_EQUIP_STATE.MAIN)
                    {
                        if (Char_Script[MY_INDEX].MainGun_Scope_Magni > 0)
                        {
                            Zoom_Button();
                        }
                    }
                    else if (Char_Script[MY_INDEX].Gun_Equip_State == GUN_EQUIP_STATE.SUB)
                    {
                        if (Char_Script[MY_INDEX].SubGun_Scope_Magni > 0)
                        {
                            Zoom_Button();
                        }
                    }                    
                }
                else if (Input.GetMouseButtonUp(1))
                {

                }

                //------------------------------------------------------------------------------------------

                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    Main_Gun_Change_Button();
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    Sub_Gun_Change_Button();
                }

                //------------------------------------------------------------------------------------------

                if (Input.GetKeyDown(KeyCode.F))
                {
                    Grenade_Button();
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    Main_Skill_Button();
                }

                if (Input.GetKeyDown(KeyCode.R))
                {
                    Sub_Skill_Button();
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Jump_Button();
                }

                //------------------------------------------------------------------------------------------
            }                       
            
        }
        else//디바이스 연산
        {
            if (Input.touchCount > 0)//터치 입력중
            {
                float X_Point = 0;
                float Y_Point = 0;

                for (int i = 0; i < Input.touchCount; i++)
                {
                    if (Input.GetTouch(i).phase != TouchPhase.Ended)
                    {
                        X_Point = Input.GetTouch(i).position.x;
                        Y_Point = Input.GetTouch(i).position.y;

                        if (Input.GetTouch(i).phase == TouchPhase.Began)
                        {
                            if (Canvas_OJ_Width / 2 > X_Point && Cursor_State[LEFT] == 0)//화면 왼쪽
                            {
                                Cursor_State[LEFT] = 1;
                                Touch_FingerId[LEFT] = Input.GetTouch(i).fingerId;
                            }
                            else if (Canvas_OJ_Width / 2 <= X_Point && Cursor_State[RIGHT] == 0)//오른쪽 화면
                            {
                                Cursor_State[RIGHT] = 1;
                                Touch_FingerId[RIGHT] = Input.GetTouch(i).fingerId;
                            }
                        }

                        if (Cursor_State[LEFT] == 1 && Touch_FingerId[LEFT] == Input.GetTouch(i).fingerId)
                        {
                            ////커서 이미지 움직임 연산
                            //Move_Joystick.CursorImage_Move(X_Point, Y_Point);

                            //캐릭터 움직임 연산
                            CharControlMove(X_Point, Y_Point);
                        }
                        else if (Cursor_State[RIGHT] == 1 && Touch_FingerId[RIGHT] == Input.GetTouch(i).fingerId)
                        {
                            //Rotation_Joystick.CursorImage_Move(X_Point, Y_Point);//커서 이미지 움직임 연산

                            //카메라 각도 연산
                            CameraRotation(X_Point, Y_Point);
                        }
                    }
                    else if (Input.GetTouch(i).phase == TouchPhase.Ended)
                    {
                        if (Cursor_State[LEFT] == 1 && Touch_FingerId[LEFT] == Input.GetTouch(i).fingerId)
                        {
                            Left_Cursor_Init();
                        }
                        else if (Cursor_State[RIGHT] == 1 && Touch_FingerId[RIGHT] == Input.GetTouch(i).fingerId)
                        {
                            Right_Cursor_Init();
                        }
                    }
                }
            }
        }
    }

    public void Left_Cursor_Init()
    {
        Cursor_State[LEFT] = 0;
        Touch_FingerId[LEFT] = -1;
        Move_Joystick.CursorImage_Enable(false);
        CharControl_State = 0;

        //캐릭터 하반신 움직임
        Char_Script[MY_INDEX].Char_Low_Move(0, 0);

        //플레이어가 컨트럴 하는 캐릭터 이동값 네트웍으로 보낼 셋팅
        Char_Script[MY_INDEX].Char_Move(0, 0);


        Key_X = 0;
        Key_X_State = 0;
        Key_Y = 0;
        Key_Y_State = 0;
    }

    public void Right_Cursor_Init()
    {
        Cursor_State[RIGHT] = 0;
        Touch_FingerId[RIGHT] = -1;
        Rotation_Joystick.CursorImage_Enable(false);
        Camera_Rotation_State = 0;
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------------

    int CharControl_State = 0;
    float CharStartPoint_X = 0.0f;
    float CharStartPoint_Y = 0.0f;

    //캐릭터 움직임 연산
    void CharControlMove(float Move_X, float Move_Y)
    {
        if (Move_Shot_Lock) return;

        if (CharControl_State == 0)
        {
            CharControl_State = 1;

            //화면상 터치 시작 좌표값 셋팅
            CharStartPoint_X = Move_X;
            CharStartPoint_Y = Move_Y;
        }

        switch (CharControl_State)
        {
            case 1:

                float Temp_X = Move_X - CharStartPoint_X;
                float Temp_Z = Move_Y - CharStartPoint_Y;

                if (Mathf.Abs(Temp_X) < CameraControlScript.Camera_PixelWidth * 0.01f && Mathf.Abs(Temp_Z) < CameraControlScript.Camera_PixelHeight * 0.01f)
                {
                    Temp_X = 0;
                    Temp_Z = 0;
                }
                               
                //캐릭터 하반신 움직임
                if (Char_Script[MY_INDEX].Flying_Start_Time == 0.0f)
                {
                    Char_Script[MY_INDEX].Char_Low_Move(new Vector3(Temp_X, 0, Temp_Z).normalized.x, new Vector3(Temp_X, 0, Temp_Z).normalized.z);
                }
                else//날고 있다
                {
                    Char_Script[MY_INDEX].Char_Low_Move(0, 0);
                }                

                //캐릭터 이동 시키기
                Char_Script[MY_INDEX].Char_Move(Temp_X, Temp_Z);

                Aim_Move_Size += (Time.deltaTime * 50.0f);

                break;
        }
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------------

    int Camera_Rotation_State = 0;
    float Camera_Before_X = 0.0f;
    float Camera_Before_Y = 0.0f;
    float Camera_NowRotation_X = 0.0f;
    float Camera_NowRotation_Y = 0.0f;
    float Camera_Rotation_Speed = 0.0f;
    float Camera_Rotation_Zoom_Speed = 0.0f;

    float Camera_Move_Angle_X = 0.0f;

    float Camera_Max_Angle = 15;

    //기준큐브 움직이는 각도의 폭
    void Camera_Rotation_Speed_Init()
    {
        Camera_Rotation_Speed = Mathf.Max(Option_Sensitive, 0.05f);
        Camera_Rotation_Zoom_Speed = Mathf.Max(Option_Sensitive_Zoom * 0.3f, 0.02f);        
    }

    //카메라 각도 연산
    void CameraRotation(float Move_X, float Move_Y)
    {
        if (Camera_Rotation_State == 0)
        {
            Camera_Rotation_State = 1;

            Camera_Move_Angle_X = 0.0f;

            //화면상 터치 시작 좌표값 셋팅
            Camera_Before_X = Move_X;
            Camera_Before_Y = Move_Y;

            //현재 카메라 기준 큐브 각도값 셋팅
            Camera_NowRotation_X = CameraControlScript.Camera_Center_OJ_EulerAngles().x;
            Camera_NowRotation_Y = CameraControlScript.Camera_Center_OJ_EulerAngles().y;
        }

        if (Camera_Rotation_State == 1)
        {
            float UD_Speed = (Mathf.Abs(Camera_Before_Y - Move_Y) / CameraControlScript.Camera_PixelHeight) * 2.0f;
            float LF_Speed = (Mathf.Abs(Camera_Before_X - Move_X) / CameraControlScript.Camera_PixelWidth) * 2.0f;

            UD_Speed = Mathf.Min(Mathf.Max(UD_Speed, 0.3f), 0.5f);
            LF_Speed = Mathf.Min(Mathf.Max(LF_Speed, 0.3f), 0.5f);

            float Temp_X = Camera_NowRotation_X + ((Camera_Before_Y - Move_Y) * (Camera_Rotation_Speed * UD_Speed));
            float Temp_Y = Camera_NowRotation_Y - ((Camera_Before_X - Move_X) * (Camera_Rotation_Speed * LF_Speed));

            if (GamePlay_UI.Zoom_Ani_State != ZOOM_ANI_STATE.IDEL)//줌인 상태
            {
                Temp_X = Camera_NowRotation_X + ((Camera_Before_Y - Move_Y) * (Camera_Rotation_Zoom_Speed * UD_Speed));
                Temp_Y = Camera_NowRotation_Y - ((Camera_Before_X - Move_X) * (Camera_Rotation_Zoom_Speed * LF_Speed));
            }

            if (Temp_X < 0) Temp_X += 360;
            else if (Temp_X > 360) Temp_X -= 360;

            if (Camera_Move_Angle_X == 315 - Camera_Max_Angle)//위로 올라가는거 막는 체크
            {
                if (Temp_X < Camera_Move_Angle_X) Temp_X = 315 - Camera_Max_Angle;
            }
            if (Camera_Move_Angle_X == 45 + Camera_Max_Angle)//밑으로 내려가는거 막는 체크
            {
                if (Temp_X > Camera_Move_Angle_X) Temp_X = 45 + Camera_Max_Angle;
            }

            if (Temp_X >= 45 + Camera_Max_Angle && Temp_X <= 180) Temp_X = 45 + Camera_Max_Angle;
            else if (Temp_X >= 180 && Temp_X <= 315 - Camera_Max_Angle) Temp_X = 315 - Camera_Max_Angle;

            //이전 각도 기억하기
            Camera_Move_Angle_X = Temp_X;

            //기준큐브 각도 움직임 연산
            CameraControlScript.Camera_Center_OJ_Rotation(Quaternion.Euler(Temp_X, Temp_Y, 0));
        }
    }

    ////카메라 각도 연산
    //void CameraRotation(float Move_X, float Move_Y)
    //{                
    //    if (Camera_Rotation_State == 0)
    //    {
    //        Camera_Rotation_State = 1;

    //        Camera_Move_Angle_X = 0.0f;

    //        //화면상 터치 시작 좌표값 셋팅
    //        Camera_Before_X = Move_X;
    //        Camera_Before_Y = Move_Y;

    //        //현재 카메라 기준 큐브 각도값 셋팅
    //        Camera_NowRotation_X = CameraControlScript.Camera_Center_OJ_EulerAngles().x;
    //        Camera_NowRotation_Y = CameraControlScript.Camera_Center_OJ_EulerAngles().y;
    //    }

    //    if (Camera_Rotation_State == 1)
    //    {            
    //        //float UD_Speed = 1.0f;
    //        //float LF_Speed = 1.0f;
            
    //        //if (Mathf.Abs(Camera_Before_Y - Move_Y) < CameraControlScript.Camera_PixelHeight * 0.01f) UD_Speed = 0.3f;
    //        //else if (Mathf.Abs(Camera_Before_Y - Move_Y) < CameraControlScript.Camera_PixelHeight * 0.02f) UD_Speed = 0.5f;
    //        //else UD_Speed = 1.0f;

    //        //if (Mathf.Abs(Camera_Before_X - Move_X) < CameraControlScript.Camera_PixelWidth * 0.01f) LF_Speed = 0.3f;
    //        //else if (Mathf.Abs(Camera_Before_X - Move_X) < CameraControlScript.Camera_PixelWidth * 0.02f) LF_Speed = 0.5f;
    //        //else LF_Speed = 1.0f;


    //        float UD_Speed = Mathf.Max((Mathf.Abs(Camera_Before_Y - Move_Y) / CameraControlScript.Camera_PixelHeight) * 10.0f, 0.3f);
    //        float LF_Speed = Mathf.Max((Mathf.Abs(Camera_Before_X - Move_X) / CameraControlScript.Camera_PixelWidth) * 10.0f, 0.3f);
            
    //        float Temp_X = Camera_NowRotation_X + ((Camera_Before_Y - Move_Y) * (Camera_Rotation_Speed * UD_Speed));
    //        float Temp_Y = Camera_NowRotation_Y - ((Camera_Before_X - Move_X) * (Camera_Rotation_Speed * LF_Speed));

    //        if (GamePlay_UI.Zoom_Ani_State != ZOOM_ANI_STATE.IDEL)//줌인 상태
    //        {
    //            Temp_X = Camera_NowRotation_X + ((Camera_Before_Y - Move_Y) * (Camera_Rotation_Zoom_Speed * UD_Speed));
    //            Temp_Y = Camera_NowRotation_Y - ((Camera_Before_X - Move_X) * (Camera_Rotation_Zoom_Speed * LF_Speed));
    //        }   

    //        if (Temp_X < 0) Temp_X += 360;
    //        else if (Temp_X > 360) Temp_X -= 360;
                        
    //        if (Camera_Move_Angle_X == 315 - Camera_Max_Angle)//위로 올라가는거 막는 체크
    //        {
    //            if (Temp_X < Camera_Move_Angle_X) Temp_X = 315 - Camera_Max_Angle;
    //        }
    //        if (Camera_Move_Angle_X == 45 + Camera_Max_Angle)//밑으로 내려가는거 막는 체크
    //        {
    //            if (Temp_X > Camera_Move_Angle_X) Temp_X = 45 + Camera_Max_Angle;
    //        }

    //        if (Temp_X >= 45 + Camera_Max_Angle && Temp_X <= 180) Temp_X = 45 + Camera_Max_Angle;
    //        else if (Temp_X >= 180 && Temp_X <= 315 - Camera_Max_Angle) Temp_X = 315 - Camera_Max_Angle;


    //        //기준큐브 각도 움직임 연산
    //        CameraControlScript.Camera_Center_OJ_Rotation(Quaternion.Euler(Temp_X, Temp_Y, 0));

    //        //--------------------------------------------------------------------------------------------------------------------
    //        //이전 각도 기억하기
    //        Camera_Move_Angle_X = Temp_X;

    //        //화면상 터치 시작 좌표값 셋팅
    //        Camera_Before_X = Move_X;
    //        Camera_Before_Y = Move_Y;

    //        //현재 카메라 기준 큐브 각도값 셋팅
    //        Camera_NowRotation_X = Temp_X;
    //        Camera_NowRotation_Y = Temp_Y;
    //        //--------------------------------------------------------------------------------------------------------------------
    //    }
    //}

    //====================================================================================================================================================================================================

    public ArrayList User_InGame_List = new ArrayList();
    public ArrayList User_OutGame_List = new ArrayList();

    //유저 들어 오고, 나가는 표시
    void Network_In_Out_Operation()
    {
        //유저 들어오는 체크
        for (int i = 0; i < User_InGame_List.Count; i++)
        {
            uint In_User_ID = (uint)User_InGame_List[0];
            User_InGame_List.RemoveAt(0);

            if (Link_Script.i.User_Data.ContainsKey(In_User_ID))
            {
                //채팅공지(입장)
                SendManager.Instance.Send_NoticeMessage("<color=#FFFF00FF>" + Link_Script.i.User_Data[In_User_ID].User_NickName + "  " + TextDataManager.Dic_TranslateText[503] + "</color>");                
            }
        }

        //유저 나가는 체크
        for (int i = 0; i < User_OutGame_List.Count; i++)
        {
            uint Out_User_ID = (uint)User_OutGame_List[0];
            User_OutGame_List.RemoveAt(0);

            if (Char_Script.ContainsKey(Out_User_ID))
            {
                //채팅공지(퇴장)
                SendManager.Instance.Send_NoticeMessage("<color=#FFFF00FF>" + Char_Script[Out_User_ID].User_NickName + "  " + TextDataManager.Dic_TranslateText[504] + "</color>");                

                Char_Script[Out_User_ID].Network_Disconnect_Die();
                Char_Script.Remove(Out_User_ID);
            }

            if (Link_Script.i.User_Data.ContainsKey(Out_User_ID))
            {
                Link_Script.i.User_Data.Remove(Out_User_ID);
            }

            //Debug.LogWarning("[" + Out_User_ID + "] 아이디 유저 네트웍 끊겨서 죽는 처리 한다.");
        }
    }

    //====================================================================================================================================================================================================

    public struct CHAR_MOVE_INFO
    {
        public uint User_ID;
        public int Char_Index;
        public byte Char_Level;        
        public byte Gun_Type;
        public int Gun_Index;
        public ByteData Receive_data;
    }
    CHAR_MOVE_INFO Char_Move_Info;

    ArrayList Char_Move_Info_List = new ArrayList();

    //상대방 캐릭터 움직임 네트웍 데이터 받기
    public void Recv_Char_Move_Data(ByteData _Receive_data)
    {        
        _Receive_data.OutPutVariable(ref Char_Move_Info.User_ID);
        _Receive_data.OutPutVariable(ref Char_Move_Info.Char_Index);
        _Receive_data.OutPutVariable(ref Char_Move_Info.Char_Level);        
        _Receive_data.OutPutVariable(ref Char_Move_Info.Gun_Type);
        _Receive_data.OutPutVariable(ref Char_Move_Info.Gun_Index);
        
        //나머지 데이터 담기
        Char_Move_Info.Receive_data = _Receive_data;

        //캐릭터가 생성 되지 않았다면
        if (Char_Script.ContainsKey(Char_Move_Info.User_ID) == false)
        {
            Char_Move_Info_List.Add(Char_Move_Info);
            return;
        }
        else
        {
            //캐릭터 움직임 데이터 연산
            Char_Script[Char_Move_Info.User_ID].Network_Move_Pos(Char_Move_Info.Receive_data);
        }

        //현재 캐릭터의 인덱스 이미지에 맞춰서 바꾼다.
        if (Char_Script[Char_Move_Info.User_ID].Char_Index != Char_Move_Info.Char_Index)
        {
            Char_Script[Char_Move_Info.User_ID].Char_Index = Char_Move_Info.Char_Index;
            Char_Script[Char_Move_Info.User_ID].Net_Char_Model_Change_Check = true;//네트웍 캐릭터 바꾸기
        }

        //현재 총 상태 맞춰서 바꿔준다.
        if (Char_Script[Char_Move_Info.User_ID].Gun_Index[0] != Char_Move_Info.Gun_Index)
        {
            Char_Script[Char_Move_Info.User_ID].Gun_Type = (GUN_TYPE)Char_Move_Info.Gun_Type;
            Char_Script[Char_Move_Info.User_ID].Gun_Index[0] = Char_Move_Info.Gun_Index;
            Char_Script[Char_Move_Info.User_ID].Net_Char_Gun_Change_Check = true;//네트웍 캐릭터 무기 바꾸기
        }                
    }

    //네트웍 캐릭터 움직임 연산
    void Char_Move_Operation()
    {
        for (int i = 0; i < Char_Move_Info_List.Count; i++)
        {
            CHAR_MOVE_INFO _Char_Move_Info = (CHAR_MOVE_INFO)Char_Move_Info_List[0];
            Char_Move_Info_List.RemoveAt(0);
                        
            //캐릭터가 이미 있다면 넘긴다
            if (Char_Script.ContainsKey(_Char_Move_Info.User_ID)) continue;

            //캐릭터가 없는 정보가 넘어왔지만, 방정보 데이터에 현재캐릭터의 정보가 없다면 넘긴다.
            if (Link_Script.i.User_Data.ContainsKey(_Char_Move_Info.User_ID) == false) continue;

            String Object_Name = _Char_Move_Info.User_ID + "";
                        
            //캐릭터 만들기
            Transform Char_OJ = Make_Char_OJ(Object_Name);
                        
            Char_Script.Add(_Char_Move_Info.User_ID, Char_OJ.GetComponent<Player_Script>());

            //캐릭터의 중요데이터 먼저 셋팅
            Char_Script[_Char_Move_Info.User_ID].Object_Name = Object_Name;
            Char_Script[_Char_Move_Info.User_ID].User_ID = _Char_Move_Info.User_ID;
            Char_Script[_Char_Move_Info.User_ID].User_Team = Link_Script.i.User_Data[_Char_Move_Info.User_ID].User_Team;            
            Char_Script[_Char_Move_Info.User_ID].User_NickName = Link_Script.i.User_Data[_Char_Move_Info.User_ID].User_NickName;
            Char_Script[_Char_Move_Info.User_ID].User_Clan_Mark = Link_Script.i.User_Data[_Char_Move_Info.User_ID].User_Clan_Mark;
            Char_Script[_Char_Move_Info.User_ID].User_Country_Mark[0] = Link_Script.i.User_Data[_Char_Move_Info.User_ID].User_Country_Mark[0];
            Char_Script[_Char_Move_Info.User_ID].User_Country_Mark[1] = Link_Script.i.User_Data[_Char_Move_Info.User_ID].User_Country_Mark[1];
            Char_Script[_Char_Move_Info.User_ID].Char_Index = _Char_Move_Info.Char_Index;
            Char_Script[_Char_Move_Info.User_ID].Char_Level = _Char_Move_Info.Char_Level;
            Char_Script[_Char_Move_Info.User_ID].Gun_Type = (GUN_TYPE)_Char_Move_Info.Gun_Type;
            Char_Script[_Char_Move_Info.User_ID].Gun_Index[0] = _Char_Move_Info.Gun_Index;            

            //캐릭터 셋팅
            if (Char_Script[_Char_Move_Info.User_ID].User_Team == Char_Script[MY_INDEX].User_Team)
            {
                Char_Script[_Char_Move_Info.User_ID].Char_Init(CHAR_USER_KIND.NETWORK, CHAR_TEAM_SATAE.PLAYER_TEAM);
            }
            else
            {
                Char_Script[_Char_Move_Info.User_ID].Char_Init(CHAR_USER_KIND.NETWORK, CHAR_TEAM_SATAE.ENEMY_TEAM);
            }

            Char_Script[_Char_Move_Info.User_ID].Net_Char_Model_Change_Check = true;//네트웍 캐릭터 바꾸기            
            Char_Script[_Char_Move_Info.User_ID].Net_Char_Gun_Change_Check = true;//네트웍 캐릭터 무기 바꾸기

            //캐릭터 레벨대비 체력 수치 가져와서 세팅해준다.
            Char_Script[_Char_Move_Info.User_ID].MY_HP = SendManager.Instance.Get_ReinfCharacter((uint)Char_Script[_Char_Move_Info.User_ID].Char_Index, Char_Script[_Char_Move_Info.User_ID].Char_Level).Hp;
            
            //최초 캐릭터 위치는 바로 셋팅해준다.
            Char_Script[_Char_Move_Info.User_ID].Direct_Pos(_Char_Move_Info.Receive_data);
        }
    }

    //====================================================================================================================================================================================================

    //--------------------------------------------------------------------------------------------------------------------------------------------

    public struct GRENADE_MOVE_INFO
    {
        public String OJ_Name;
        public uint User_ID;
        public byte Gun_Type;
        public ByteData Receive_data;
    }
    GRENADE_MOVE_INFO Grenade_Move_Info;

    ArrayList Grenade_Move_Info_List = new ArrayList();

    //수류탄 움직임 데이터
    public void Recv_Grenade_Move_Data(ByteData _Receive_data)
    {
        _Receive_data.OutPutVariable(ref Grenade_Move_Info.OJ_Name);
        _Receive_data.OutPutVariable(ref Grenade_Move_Info.User_ID);
        _Receive_data.OutPutVariable(ref Grenade_Move_Info.Gun_Type);

        //나머지 데이터 담기
        Grenade_Move_Info.Receive_data = _Receive_data;

        //수류탄이 생성 되지 않았다면 만들어 준다.
        if (Grenade_Data.ContainsKey(Grenade_Move_Info.OJ_Name) == false)
        {
            Grenade_Move_Info_List.Add(Grenade_Move_Info);
            return;
        }
        else
        {
            //수류탄 이동시키기
            Grenade_Data[Grenade_Move_Info.OJ_Name].Grenade_Net_Move(Grenade_Move_Info.Receive_data);
        }
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------

    ArrayList Grenade_Explosion_List = new ArrayList();

    //수류탄 터지는 데이터
    public void Recv_Explosion_Data(ByteData _Receive_data)
    {
        Grenade_Explosion_List.Add(_Receive_data);
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------

    //수류탄 연산
    void Char_Grenade_Operation()
    {
        //수류탄 움직임 데이터 처리
        for (int i = 0; i < Grenade_Move_Info_List.Count; i++)
        {
            GRENADE_MOVE_INFO _Grenade_Move_Info = (GRENADE_MOVE_INFO)Grenade_Move_Info_List[0];
            Grenade_Move_Info_List.RemoveAt(0);

            //수류탄이 이미 있다면 넘긴다
            if (Grenade_Data.ContainsKey(_Grenade_Move_Info.OJ_Name)) continue;

            //월드에 없는 수류탄 데이터지만, 실제 수류탄을 날린 캐릭터 정보가 없다면 수류탄 생성 하지 않는다.
            if (Link_Script.i.User_Data.ContainsKey(_Grenade_Move_Info.User_ID) == false) continue;

            if ((GUN_TYPE)_Grenade_Move_Info.Gun_Type == GUN_TYPE.GRENADE)
            {
                Instantiate(Resources.Load("Battle_Prefab/4_Grenade_Folder/Grenade") as GameObject).name = _Grenade_Move_Info.OJ_Name;
            }
            else if ((GUN_TYPE)_Grenade_Move_Info.Gun_Type == GUN_TYPE.BOOMER_SKILL)
            {
                Instantiate(Resources.Load("Battle_Prefab/4_Grenade_Folder/Grenade_Boomer") as GameObject).name = _Grenade_Move_Info.OJ_Name;
            }
            else if ((GUN_TYPE)_Grenade_Move_Info.Gun_Type == GUN_TYPE.LAUNCHER)
            {
                Instantiate(Resources.Load("Battle_Prefab/4_Grenade_Folder/Launchershell") as GameObject).name = _Grenade_Move_Info.OJ_Name;
            }
            else if ((GUN_TYPE)_Grenade_Move_Info.Gun_Type == GUN_TYPE.ROCKET_SKILL)
            {
                Instantiate(Resources.Load("Battle_Prefab/4_Grenade_Folder/Rocketshell") as GameObject).name = _Grenade_Move_Info.OJ_Name;
            }
            else if ((GUN_TYPE)_Grenade_Move_Info.Gun_Type == GUN_TYPE.THROUGH_SHOT_SKILL)
            {
                Instantiate(Resources.Load("Battle_Prefab/4_Grenade_Folder/Laser_Ball") as GameObject).name = _Grenade_Move_Info.OJ_Name;
            }

            Transform Grenade_Transform = GameObject.Find(_Grenade_Move_Info.OJ_Name).transform.GetComponent<Transform>();
            Grenade_Transform.SetParent(Link_Script.i.Effect_OJ_Set);

            Grenade_Data.Add(_Grenade_Move_Info.OJ_Name, Grenade_Transform.GetComponent<Grenade_Script>());

            Grenade_Data[_Grenade_Move_Info.OJ_Name].Grenade_User_Kind = CHAR_USER_KIND.NETWORK;
            Grenade_Data[_Grenade_Move_Info.OJ_Name].User_ID = _Grenade_Move_Info.User_ID;
            Grenade_Data[_Grenade_Move_Info.OJ_Name].OJ_Name = _Grenade_Move_Info.OJ_Name;
            Grenade_Data[_Grenade_Move_Info.OJ_Name].Gun_Type = (GUN_TYPE)_Grenade_Move_Info.Gun_Type;

            //최초 네트웍 위치는 바로 셋팅해준다.
            Grenade_Data[_Grenade_Move_Info.OJ_Name].Direct_Pos(_Grenade_Move_Info.Receive_data);            
        }

        //수류탄 터지는 데이터 처리
        for (int i = 0; i < Grenade_Explosion_List.Count; i++)
        {
            ByteData _Receive_data = (ByteData)Grenade_Explosion_List[0];
            Grenade_Explosion_List.RemoveAt(0);

            String OJ_Name = "";

            _Receive_data.OutPutVariable(ref OJ_Name);

            //터질 수류탄이 없다면 넘긴다
            if (!Grenade_Data.ContainsKey(OJ_Name)) continue;

            Grenade_Data[OJ_Name].Grenade_Explosion_Receive_Data(_Receive_data);   
        }
    }

    //====================================================================================================================================================================================================
       
    //상대방 캐릭터 스킬데이터 네트웍 데이터 받기
    public void Recv_Char_Skill_Data(ByteData _Receive_data)
    {
        uint _User_ID = 0;
        
        _Receive_data.OutPutVariable(ref _User_ID);
        
        //나머지 데이터 받기
        ByteData Receive_data = _Receive_data;

        //캐릭터가 생성 되었을때만
        if (Char_Script.ContainsKey(_User_ID))
        {            
            //캐릭터 스킬데이터 데이터 연산
            Char_Script[_User_ID].Network_Skill_Pos(Receive_data);
        }       
    }

    //====================================================================================================================================================================================================
        
    //타겟팅 체크 연산
    void Auto_Targeting_Operation()
    {        
        if (Char_Script[MY_INDEX].MY_HP == 0.0f) return;
        
        //if (Char_Script[MY_INDEX].Gun_Type == GUN_TYPE.SNIPER) return;//스나

        Targeting_Ray = CameraControlScript.Main_Camera.ScreenPointToRay(GamePlay_UI.Spot_Image.position);

        if (Physics.Raycast(Targeting_Ray, out Targeting_RaycastHit, Mathf.Infinity))
        {
            bool _Null_Check = false;
            byte _User_Team = 0;
            float _MY_HP = 0.0f;

            if (Link_Script.i.Play_Mode == BattleKind.ALONE_PLAY_BATTLE)//싱글모드
            {
                BOT_Script Targeting_Script = Targeting_RaycastHit.transform.root.GetComponent<BOT_Script>();

                if (Targeting_Script != null)
                {
                    _Null_Check = true;
                    _User_Team = Targeting_Script.User_Team;
                    _MY_HP = Targeting_Script.MY_HP;
                }
            }
            else
            {
                Player_Script Targeting_Script = Targeting_RaycastHit.transform.root.GetComponent<Player_Script>();

                if (Targeting_Script != null)
                {
                    _Null_Check = true;
                    _User_Team = Targeting_Script.User_Team;
                    _MY_HP = Targeting_Script.MY_HP;
                }
            }

            if (_Null_Check && _User_Team != Char_Script[MY_INDEX].User_Team && _MY_HP > 0.0f)
            {
                GamePlay_UI.Aim_Color_Change(AIM_COLOR_STATE.RED);

                if (Char_Script[MY_INDEX].Gun_Type == GUN_TYPE.SNIPER)//스나이퍼
                {
                    if (Option_Auto_Shot && Player_Shot_State == PLAYER_SHOT_STATE.IDEL && Sniper_Auto_Target_State == 0)
                    {
                        if (Char_Script[MY_INDEX].Gun_Reload_State[(int)GUN_EQUIP_STATE.MAIN] == RELOAD_STATE.IDEL)
                        {
                            Sniper_Auto_Target_State = 1;
                            Sniper_Auto_Target_Time = 1.0f;
                        }                       
                    }
                }
                else
                {
                    if (Option_Auto_Shot && Player_Shot_State == PLAYER_SHOT_STATE.IDEL) Player_Shot_State = PLAYER_SHOT_STATE.SHOT_START;
                }                
            }
            else
            {
                GamePlay_UI.Aim_Color_Change(AIM_COLOR_STATE.WHITE);

                if (Char_Script[MY_INDEX].Gun_Type == GUN_TYPE.SNIPER)//스나이퍼
                {
                    if (Option_Auto_Shot && Player_Shot_State != PLAYER_SHOT_STATE.IDEL && Move_Shot_Lock == false && Sniper_Auto_Target_State == 0)
                    {
                        Player_Shot_State = PLAYER_SHOT_STATE.SHOT_END;
                    }
                }
                else
                {
                    if (Option_Auto_Shot && Player_Shot_State != PLAYER_SHOT_STATE.IDEL && Move_Shot_Lock == false) Player_Shot_State = PLAYER_SHOT_STATE.SHOT_END;
                }
            }
        }
    }

    //====================================================================================================================================================================================================

    //스나이퍼 오토 타겟팅 되는 시간 연산
    void Sniper_Auto_Target_Time_Operation()
    {
        if (Char_Script[MY_INDEX].Gun_Type != GUN_TYPE.SNIPER) return;

        switch (Sniper_Auto_Target_State)
        {
            case 0:
                break;
            case 1://일정시간 대기후 스나이퍼 쏜다.

                //스나이퍼 자동 타켓팅됐을때 나타나는 원형 게이지
                GamePlay_UI.ShotDelay_Aim_View(Sniper_Auto_Target_Time, 1.0f);

                Sniper_Auto_Target_Time -= Time.deltaTime;
                if (Sniper_Auto_Target_Time <= 0.0f)
                {
                    Sniper_Auto_Target_State = 2;
                    Sniper_Auto_Target_Time = 0.1f;

                    if (Option_Auto_Shot && Player_Shot_State == PLAYER_SHOT_STATE.IDEL) Player_Shot_State = PLAYER_SHOT_STATE.SHOT_START;
                }

                break;
            case 2://쏘고난후 일정시간 대기후 쏘기대기상태로 넘어간다(바로 넘어가면 버튼띄었다는 판단으로 총을 안쏠때가 있다)

                //스나이퍼 자동 타켓팅됐을때 나타나는 원형 게이지
                GamePlay_UI.ShotDelay_Aim_View(0.0f, 0.0f);

                Sniper_Auto_Target_Time -= Time.deltaTime;
                if (Sniper_Auto_Target_Time <= 0.0f)
                {
                    Sniper_Auto_Target_State = 0;
                    Sniper_Auto_Target_Time = 0.0f;

                    if (Option_Auto_Shot && Player_Shot_State != PLAYER_SHOT_STATE.IDEL) Player_Shot_State = PLAYER_SHOT_STATE.SHOT_END;
                }

                break;
        }
    }
    
    //====================================================================================================================================================================================================

    //줌인,아웃 버튼
    public void Zoom_Button()
    {
        if (Char_Script[MY_INDEX].MY_HP == 0.0f) return;        

        if (Grenade_Delay_Time != 0.0f || Skill_Delay_Time != 0.0f) return;

        if (Char_Script[MY_INDEX].Gun_Equip_State == GUN_EQUIP_STATE.MAIN)
        {
            if (Char_Script[MY_INDEX].Gun_Reload_State[(int)GUN_EQUIP_STATE.MAIN] != RELOAD_STATE.IDEL) return;
        }
        else if (Char_Script[MY_INDEX].Gun_Equip_State == GUN_EQUIP_STATE.SUB)
        {
            if (Char_Script[MY_INDEX].Gun_Reload_State[(int)GUN_EQUIP_STATE.SUB] != RELOAD_STATE.IDEL) return;
        }
        
        //줌인,아웃 상태로 들어갈때는 각도움직임 값 달라지기때문에 리셋해준다.
        Camera_Rotation_State = 0;

        if (GamePlay_UI.Zoom_Ani_State == ZOOM_ANI_STATE.IDEL)
        {
            if (Char_Script[MY_INDEX].Gun_Type == GUN_TYPE.SNIPER)
            {
                GamePlay_UI.Zoom_Ani_Init(ZOOM_ANI_STATE.ZOOM_IN, 0);
            }
            else
            {
                GamePlay_UI.Zoom_Ani_Init(ZOOM_ANI_STATE.ZOOM_IN, 1);
            }            

            Char_Script[MY_INDEX].Char_View_Check(false);         
        }
        else if (GamePlay_UI.Zoom_Ani_State == ZOOM_ANI_STATE.ZOOM_END)
        {
            GamePlay_UI.Zoom_Ani_Init(ZOOM_ANI_STATE.ZOOM_OUT);

            Char_Script[MY_INDEX].Char_View_Check(true);
        }        
    }

    //====================================================================================================================================================================================================

    bool Move_Shot_Lock = false;

    //발사 버튼
    public void Shot_Button(BUTTON_DIR_STATE _Dir, PLAYER_SHOT_STATE _Player_Shot_State)
    {
        Player_Shot_State = _Player_Shot_State;

        if (Player_Shot_State == PLAYER_SHOT_STATE.SHOT_END)
        {
            GamePlay_UI.Shot_Button_RayTarget_Init(BUTTON_DIR_STATE.ALL_CHECK);

            Move_Shot_Lock = false;
        }
        else
        {
            GamePlay_UI.Shot_Button_RayTarget_Init(_Dir);

            if (_Dir == BUTTON_DIR_STATE.LEFT) Move_Shot_Lock = true;
        }
    }

    //플레이어 총쏘는 연산
    void MY_Shot_Operation()
    {       
        if (GamePlay_UI.Zoom_Ani_State == ZOOM_ANI_STATE.ZOOM_END)
        {
            Char_Script[MY_INDEX].Char_Sight_Init();
        }        

        if (Grenade_Delay_Time != 0.0f)
        {
            Grenade_Delay_Time -= Time.deltaTime;
            if (Grenade_Delay_Time <= 0.0f)Grenade_Delay_Time = 0.0f;
        }

        if (Skill_Delay_Time != 0.0f)
        {
            Skill_Delay_Time -= Time.deltaTime;
            if (Skill_Delay_Time <= 0.0f) Skill_Delay_Time = 0.0f;
        }

        if (Grenade_Delay_Time != 0.0f || Skill_Delay_Time != 0.0f) return;        

        //-----------------------------------------------------------------------

        if (Char_Script[MY_INDEX].MY_HP == 0.0f) return;

        switch (Player_Shot_State)
        {
            case PLAYER_SHOT_STATE.IDEL:

                break;
            case PLAYER_SHOT_STATE.SHOT_START:

                if (Char_Script[MY_INDEX].Gun_Reload_State[(int)Char_Script[MY_INDEX].Gun_Equip_State] != RELOAD_STATE.IDEL) break;

                Char_Script[MY_INDEX].Shot_On();

                Player_Shot_State = PLAYER_SHOT_STATE.SHOT_LAUNCH;

                break;
            case PLAYER_SHOT_STATE.SHOT_LAUNCH:

                break;
            case PLAYER_SHOT_STATE.SHOT_END:

                Char_Script[MY_INDEX].Shot_Off();

                Player_Shot_State = PLAYER_SHOT_STATE.IDEL;

                break;
        }
    }

    //====================================================================================================================================================================================================

    //캐릭터 죽었을때 데이터 받기
    public void Recv_Kill_Info(ByteData _Receive_data)
    {
        if (Char_Script.ContainsKey(MY_INDEX) == false) return;

        // uint : 공격 유저 WebUserID
        // uint : 죽은 유저 WebUserID
        // bool  : 헤드샷 
        // ushort : Red팀 킬 카운트 (Red:0)
        // ushort : Blue팀 킬 카운트 (Blue:1)
        // ushort : Red팀 데스 카운트 (Red:0)
        // ushort : Blue팀 데스 카운트 (Blue:1)
        // long : 유저 부활 시간
        // ushort : 죽은 유저 킬
        // ushort : 죽은 유저 데스

        uint Kill_User_ID = 0;
        uint Dead_User_ID = 0;
        bool HeadShot_Check = false;
        ushort Red_Kill_Count = 0;
        ushort Blue_Kill_Count = 0;
        ushort Red_Dead_Count = 0;
        ushort Blue_Dead_Count = 0;

        _Receive_data.OutPutVariable(ref Kill_User_ID);
        _Receive_data.OutPutVariable(ref Dead_User_ID);
        _Receive_data.OutPutVariable(ref HeadShot_Check);
        _Receive_data.OutPutVariable(ref Red_Kill_Count);
        _Receive_data.OutPutVariable(ref Blue_Kill_Count);
        _Receive_data.OutPutVariable(ref Red_Dead_Count);
        _Receive_data.OutPutVariable(ref Blue_Dead_Count);
        _Receive_data.OutPutVariable(ref Respawn_Server_Time);
        _Receive_data.OutPutVariable(ref Respawn_Player_Kill_Count);
        _Receive_data.OutPutVariable(ref Respawn_Player_Death_Count);
        
        //화면 상단의 팀 킬,데스 카운트 UI
        GamePlay_UI.Team_Kill_Dead_UI_Init(Red_Kill_Count, Red_Dead_Count, Blue_Kill_Count, Blue_Dead_Count);

        if (Char_Script.ContainsKey(Kill_User_ID) && Char_Script.ContainsKey(Dead_User_ID))
        {
            //죽인유저, 죽은유저 텍스트 처리 셋팅
            GamePlay_UI.Killer_Message(Char_Script[Kill_User_ID].User_NickName, Char_Script[Dead_User_ID].User_NickName);
        }        


        if (Char_Script[MY_INDEX].User_ID == Dead_User_ID)//내가 Dead
        {
            //리스폰 UI 띄워준다.
            Combo_Check = false;
            Combo_Count = 0;
            Respawn_UI_Check = true;
            Respawn_Player_Killer_NickName = "";
            Respawn_Killing_Message = "";

            if (Kill_User_ID == Dead_User_ID)//내가 자살일때
            {
                Char_Script[MY_INDEX].Player_Killer_UserID = 0;//나를 죽인 캐릭터 카메라로 비추기위한 변수
                Respawn_Player_Killer_NickName = Char_Script[MY_INDEX].User_NickName;
            }
            else
            {
                Char_Script[MY_INDEX].Player_Killer_UserID = Kill_User_ID;//나를 죽인 캐릭터 카메라로 비추기위한 변수
                Char_Script[MY_INDEX].Revenge_UserID = Kill_User_ID;//나를 죽인 캐릭터 복수표시를 위한 변수

                if (Char_Script.ContainsKey(Kill_User_ID))
                {
                    Respawn_Player_Killer_NickName = Char_Script[Kill_User_ID].User_NickName;

                    if (Link_Script.i.User_Data.ContainsKey(Kill_User_ID))
                    {
                        Respawn_Killing_Message = Link_Script.i.User_Data[Kill_User_ID].Killing_Message;
                    }
                }
            }

            //킬,데스 카운트별 버프 셋팅                
            int Kill_Death_Count = Respawn_Player_Death_Count - Respawn_Player_Kill_Count;

            if (Kill_Death_Count == 1) Death_Buff_Size = 0.1f;
            else if (Kill_Death_Count == 2) Death_Buff_Size = 0.2f;
            else if (Kill_Death_Count == 3) Death_Buff_Size = 0.3f;
            else if (Kill_Death_Count == 4) Death_Buff_Size = 0.4f;
            else if (Kill_Death_Count >= 5) Death_Buff_Size = 0.5f;
            else Death_Buff_Size = 0.0f;

        }
        else if (Char_Script[MY_INDEX].User_ID == Kill_User_ID)//내가 Killer
        {
            Combo_Check = true;
            if (++Combo_Count >= 10) Combo_Count = 10;

            //헤드샷 효과
            HeadShot_UI_Check = HeadShot_Check;

            //복수 성공
            if (Char_Script[MY_INDEX].Revenge_UserID == Dead_User_ID)
            {
                Char_Script[MY_INDEX].Revenge_OK_Icon_Time = Char_Script[MY_INDEX].Revenge_OK_Icon_MAX_Time;
            }

            //킬,데스 카운트별 버프 셋팅 리셋
            Death_Buff_Size = 0.0f;
        }
        else//다른 사람끼리 죽고,죽였을때
        {
        }
    }

    //봇 죽였을때 연산
    public void Bot_Kill_Operation(uint Bot_User_ID, bool HeadShot_Check)
    {       
        //화면 상단의 팀 킬,데스 카운트 UI
        GamePlay_UI.Kill_Num_Training_Text.text = "" + Bot_Kill_Count;

        //죽인유저, 죽은유저 텍스트 처리 셋팅
        GamePlay_UI.Killer_Message(Char_Script[MY_INDEX].User_NickName, Bot_Script[Bot_User_ID].User_NickName);
        
        Combo_Check = true;
        if (++Combo_Count >= 10) Combo_Count = 10;

        //헤드샷 효과
        HeadShot_UI_Check = HeadShot_Check;
                
        //킬,데스 카운트별 버프 셋팅 리셋
        Death_Buff_Size = 0.0f;
    }

    //====================================================================================================================================================================================================

    //주무기 교체 버튼
    public void Main_Gun_Change_Button()
    {
        if (Char_Script[MY_INDEX].MY_HP == 0.0f) return;

        if (Char_Script[MY_INDEX].Flamer_Time != 0.0f) return;
                
        if (Player_Shot_State != PLAYER_SHOT_STATE.IDEL)
        {
            Char_Script[MY_INDEX].Shot_Off();

            Player_Shot_State = PLAYER_SHOT_STATE.IDEL;
        }

        //리로드 시킨다
        if (Char_Script[MY_INDEX].Gun_Equip_State == GUN_EQUIP_STATE.MAIN)
        {
            Char_Script[MY_INDEX].Reload_Start();
            return;
        }

        //주,보조 무기 바꾸기
        Char_Script[MY_INDEX].Gun_Change(GUN_EQUIP_STATE.MAIN);

        //주,보조 무기 선택되어 있는 UI 체크
        GamePlay_UI.Gun_Select_Init(GUN_EQUIP_STATE.MAIN);

        Sniper_Auto_Target_State = 0;
        Sniper_Auto_Target_Time = 0.0f;

        //스나이퍼 자동 타켓팅됐을때 나타나는 원형 게이지
        GamePlay_UI.ShotDelay_Aim_View(0.0f, 0.0f);
    }

    //보조무기 교체 버튼
    public void Sub_Gun_Change_Button()
    {
        if (Char_Script[MY_INDEX].MY_HP == 0.0f) return;

        if (Char_Script[MY_INDEX].Flamer_Time != 0.0f) return;
                
        if (Player_Shot_State != PLAYER_SHOT_STATE.IDEL)
        {
            Char_Script[MY_INDEX].Shot_Off();

            Player_Shot_State = PLAYER_SHOT_STATE.IDEL;
        }

        //리로드 시킨다
        if (Char_Script[MY_INDEX].Gun_Equip_State == GUN_EQUIP_STATE.SUB)
        {
            Char_Script[MY_INDEX].Reload_Start();
            return;
        }

        //주,보조 무기 바꾸기
        Char_Script[MY_INDEX].Gun_Change(GUN_EQUIP_STATE.SUB);

        //주,보조 무기 선택되어 있는 UI 체크
        GamePlay_UI.Gun_Select_Init(GUN_EQUIP_STATE.SUB);

        Sniper_Auto_Target_State = 0;
        Sniper_Auto_Target_Time = 0.0f;

        //스나이퍼 자동 타켓팅됐을때 나타나는 원형 게이지
        GamePlay_UI.ShotDelay_Aim_View(0.0f, 0.0f);
    }

    //====================================================================================================================================================================================================

    bool Skill_Return_Check(SKILL_KIND Skill_Kind)
    {
        if (Char_Script[MY_INDEX].MY_HP == 0.0f) return true;

        switch (Skill_Kind)
        {
            case SKILL_KIND.SOLDIER://유탄발사

                if (Char_Script[MY_INDEX].Weapon_OJ[Link_Script.i.W_Launchar_Index].OJ.activeSelf ||
                    Char_Script[MY_INDEX].Roller_Time != 0.0f ||
                    Char_Script[MY_INDEX].Boomer_Start_Time != 0.0f ||
                    Char_Script[MY_INDEX].Flamer_Time != 0.0f ||
                    Char_Script[MY_INDEX].Weapon_OJ[Link_Script.i.W_Rocket_Index].OJ.activeSelf ||
                    Char_Script[MY_INDEX].Weapon_OJ[Link_Script.i.W_ThroughShot_Index].OJ.activeSelf) return true;

                break;
            case SKILL_KIND.JUMPER://슈퍼점프

                if (Char_Script[MY_INDEX].Jump_State == JUMP_STATE.SUPER_JUMP_UP || Char_Script[MY_INDEX].Flying_Start_Time != 0.0f) return true;

                break;            
            case SKILL_KIND.VISION://투시

                if (Char_Script[MY_INDEX].Vision_Time != 0.0f) return true;

                break;
            case SKILL_KIND.ROLLER://구르기 돌진

                if (Char_Script[MY_INDEX].Weapon_OJ[Link_Script.i.W_Launchar_Index].OJ.activeSelf ||
                    Char_Script[MY_INDEX].Roller_Time != 0.0f ||
                    Char_Script[MY_INDEX].Boomer_Start_Time != 0.0f ||
                    Char_Script[MY_INDEX].Flamer_Time != 0.0f ||
                    Char_Script[MY_INDEX].Flying_Start_Time != 0.0f || 
                    Char_Script[MY_INDEX].Weapon_OJ[Link_Script.i.W_Rocket_Index].OJ.activeSelf ||
                    Char_Script[MY_INDEX].Weapon_OJ[Link_Script.i.W_ThroughShot_Index].OJ.activeSelf) return true;

                break;
            case SKILL_KIND.GUARD://방어막

                if (Char_Script[MY_INDEX].Guard_Time != 0.0f) return true;

                break;            
            case SKILL_KIND.BOOMER://수류탄 다수 투척

                if (Char_Script[MY_INDEX].Weapon_OJ[Link_Script.i.W_Launchar_Index].OJ.activeSelf ||
                    Char_Script[MY_INDEX].Roller_Time != 0.0f ||
                    Char_Script[MY_INDEX].Boomer_Start_Time != 0.0f ||
                    Char_Script[MY_INDEX].Flamer_Time != 0.0f ||
                    Char_Script[MY_INDEX].Weapon_OJ[Link_Script.i.W_Rocket_Index].OJ.activeSelf ||
                    Char_Script[MY_INDEX].Weapon_OJ[Link_Script.i.W_ThroughShot_Index].OJ.activeSelf) return true;

                break;
            case SKILL_KIND.FLAMER://화염방사기

                if (Char_Script[MY_INDEX].Weapon_OJ[Link_Script.i.W_Launchar_Index].OJ.activeSelf ||
                    Char_Script[MY_INDEX].Roller_Time != 0.0f ||
                    Char_Script[MY_INDEX].Boomer_Start_Time != 0.0f ||
                    Char_Script[MY_INDEX].Flamer_Time != 0.0f ||
                    Char_Script[MY_INDEX].Weapon_OJ[Link_Script.i.W_Rocket_Index].OJ.activeSelf ||
                    Char_Script[MY_INDEX].Weapon_OJ[Link_Script.i.W_ThroughShot_Index].OJ.activeSelf) return true;

                break;
            case SKILL_KIND.ROCKET://직사포

                if (Char_Script[MY_INDEX].Weapon_OJ[Link_Script.i.W_Launchar_Index].OJ.activeSelf ||
                     Char_Script[MY_INDEX].Roller_Time != 0.0f ||
                     Char_Script[MY_INDEX].Boomer_Start_Time != 0.0f ||
                     Char_Script[MY_INDEX].Flamer_Time != 0.0f ||
                     Char_Script[MY_INDEX].Weapon_OJ[Link_Script.i.W_Rocket_Index].OJ.activeSelf ||
                    Char_Script[MY_INDEX].Weapon_OJ[Link_Script.i.W_ThroughShot_Index].OJ.activeSelf) return true;

                break;
            case SKILL_KIND.FLYING://날아다니기

                if (Char_Script[MY_INDEX].Jump_State == JUMP_STATE.SUPER_JUMP_UP || Char_Script[MY_INDEX].Flying_Start_Time != 0.0f) return true;

                break;
            case SKILL_KIND.RAGE://크리티컬 100%                

                break;
            case SKILL_KIND.THROUGH_SHOT://관통샷             

                if (Char_Script[MY_INDEX].Weapon_OJ[Link_Script.i.W_Launchar_Index].OJ.activeSelf ||
                    Char_Script[MY_INDEX].Roller_Time != 0.0f ||
                    Char_Script[MY_INDEX].Boomer_Start_Time != 0.0f ||
                    Char_Script[MY_INDEX].Flamer_Time != 0.0f ||
                    Char_Script[MY_INDEX].Weapon_OJ[Link_Script.i.W_Rocket_Index].OJ.activeSelf ||
                    Char_Script[MY_INDEX].Weapon_OJ[Link_Script.i.W_ThroughShot_Index].OJ.activeSelf) return true;

                break;
        }

        return false;
    }



    //메인 스킬 버튼
    public void Main_Skill_Button()
    {
        if (Char_Script[MY_INDEX].Main_Skill_Reload_Time != 0.0f) return;

        if (Skill_Return_Check(Char_Script[MY_INDEX].Char_Main_Skill_Kind)) return;
                
        if (Char_Script[MY_INDEX].Char_Main_Skill_Kind == SKILL_KIND.SOLDIER || Char_Script[MY_INDEX].Char_Main_Skill_Kind == SKILL_KIND.JUMPER ||
            Char_Script[MY_INDEX].Char_Main_Skill_Kind == SKILL_KIND.ROLLER || Char_Script[MY_INDEX].Char_Main_Skill_Kind == SKILL_KIND.BOOMER ||
            Char_Script[MY_INDEX].Char_Main_Skill_Kind == SKILL_KIND.FLAMER || Char_Script[MY_INDEX].Char_Main_Skill_Kind == SKILL_KIND.ROCKET ||
            Char_Script[MY_INDEX].Char_Main_Skill_Kind == SKILL_KIND.THROUGH_SHOT)
        {
            //줌인된 상태 스킬 사용했을때 풀어주기
            if (GamePlay_UI.Zoom_Ani_State == ZOOM_ANI_STATE.ZOOM_END)
            {
                GamePlay_UI.Zoom_Ani_Init(ZOOM_ANI_STATE.ZOOM_OUT);

                Char_Script[MY_INDEX].Char_View_Check(true);
            }
        }

        switch (Char_Script[MY_INDEX].Char_Main_Skill_Kind)
        {
            case SKILL_KIND.SOLDIER://유탄발사

                //스킬 쓰고 난후 일정시간동안은 총 못나가게 막는다.
                Skill_Delay_Time = 1.0f;

                if (Player_Shot_State != PLAYER_SHOT_STATE.IDEL)
                {
                    Char_Script[MY_INDEX].Shot_Off();

                    Player_Shot_State = PLAYER_SHOT_STATE.IDEL;
                }

                Char_Script[MY_INDEX].Grenade_Make(GUN_TYPE.LAUNCHER, false);

                break;
            case SKILL_KIND.JUMPER://슈퍼점프

                Char_Script[MY_INDEX].Super_Jump_Start();
                                
                break;           
            case SKILL_KIND.VISION://투시

                Char_Script[MY_INDEX].Vision_Start(Char_Script[MY_INDEX].Main_Skill_Running_Time);

                break;
            case SKILL_KIND.ROLLER://구르기 돌진

                //스킬 쓰고 난후 일정시간동안은 총 못나가게 막는다.
                Skill_Delay_Time = 5.0f + 0.2f;

                if (Player_Shot_State != PLAYER_SHOT_STATE.IDEL)
                {
                    Char_Script[MY_INDEX].Shot_Off();

                    Player_Shot_State = PLAYER_SHOT_STATE.IDEL;
                }

                Char_Script[MY_INDEX].Roller_Start(Char_Script[MY_INDEX].Main_Skill_Running_Time);

                break;
            case SKILL_KIND.GUARD://방어막

                Char_Script[MY_INDEX].Guard_Start(Char_Script[MY_INDEX].Main_Skill_Running_Time);

                break;            
            case SKILL_KIND.BOOMER://수류탄 다수 투척

                //스킬 쓰고 난후 일정시간동안은 총 못나가게 막는다.
                Skill_Delay_Time = 1.5f;

                if (Player_Shot_State != PLAYER_SHOT_STATE.IDEL)
                {
                    Char_Script[MY_INDEX].Shot_Off();

                    Player_Shot_State = PLAYER_SHOT_STATE.IDEL;
                }

                Char_Script[MY_INDEX].Boomer_Start();

                break;
            case SKILL_KIND.FLAMER://화염방사기

                //스킬 쓰고 난후 일정시간동안은 총 못나가게 막는다.
                Skill_Delay_Time = 5.0f;

                if (Player_Shot_State != PLAYER_SHOT_STATE.IDEL)
                {
                    Char_Script[MY_INDEX].Shot_Off();

                    Player_Shot_State = PLAYER_SHOT_STATE.IDEL;
                }

                Char_Script[MY_INDEX].Flamer_Start(Char_Script[MY_INDEX].Main_Skill_Running_Time);

                break;
            case SKILL_KIND.ROCKET://직사포

                //스킬 쓰고 난후 일정시간동안은 총 못나가게 막는다.
                Skill_Delay_Time = 1.0f;

                if (Player_Shot_State != PLAYER_SHOT_STATE.IDEL)
                {
                    Char_Script[MY_INDEX].Shot_Off();

                    Player_Shot_State = PLAYER_SHOT_STATE.IDEL;
                }

                Char_Script[MY_INDEX].Grenade_Make(GUN_TYPE.ROCKET_SKILL, false);

                break;
            case SKILL_KIND.THROUGH_SHOT://관통샷

                //스킬 쓰고 난후 일정시간동안은 총 못나가게 막는다.
                Skill_Delay_Time = 1.5f;

                if (Player_Shot_State != PLAYER_SHOT_STATE.IDEL)
                {
                    Char_Script[MY_INDEX].Shot_Off();

                    Player_Shot_State = PLAYER_SHOT_STATE.IDEL;
                }

                Char_Script[MY_INDEX].ThroughShot_Start();

                break;
        }

        //스킬 리로드 시간 셋팅
        Char_Script[MY_INDEX].Main_Skill_Reload_Time = Char_Script[MY_INDEX].Main_Skill_Reload_MAX_Time;        
    }

    //서브 스킬 버튼
    public void Sub_Skill_Button()
    {
        if (Char_Script[MY_INDEX].Sub_Skill_Reload_Time != 0.0f) return;

        if (Skill_Return_Check(Char_Script[MY_INDEX].Char_Sub_Skill_Kind)) return;

        if (Char_Script[MY_INDEX].Char_Sub_Skill_Kind == SKILL_KIND.SOLDIER || Char_Script[MY_INDEX].Char_Sub_Skill_Kind == SKILL_KIND.JUMPER ||
            Char_Script[MY_INDEX].Char_Sub_Skill_Kind == SKILL_KIND.ROLLER || Char_Script[MY_INDEX].Char_Sub_Skill_Kind == SKILL_KIND.BOOMER ||
            Char_Script[MY_INDEX].Char_Sub_Skill_Kind == SKILL_KIND.FLAMER || Char_Script[MY_INDEX].Char_Sub_Skill_Kind == SKILL_KIND.ROCKET ||
            Char_Script[MY_INDEX].Char_Sub_Skill_Kind == SKILL_KIND.THROUGH_SHOT)
        {
            //줌인된 상태 스킬 사용했을때 풀어주기
            if (GamePlay_UI.Zoom_Ani_State == ZOOM_ANI_STATE.ZOOM_END)
            {
                GamePlay_UI.Zoom_Ani_Init(ZOOM_ANI_STATE.ZOOM_OUT);

                Char_Script[MY_INDEX].Char_View_Check(true);
            }
        }
        
        switch (Char_Script[MY_INDEX].Char_Sub_Skill_Kind)
        {
            case SKILL_KIND.SOLDIER://유탄발사

                //스킬 쓰고 난후 일정시간동안은 총 못나가게 막는다.
                Skill_Delay_Time = 1.0f;

                if (Player_Shot_State != PLAYER_SHOT_STATE.IDEL)
                {
                    Char_Script[MY_INDEX].Shot_Off();

                    Player_Shot_State = PLAYER_SHOT_STATE.IDEL;
                }

                Char_Script[MY_INDEX].Grenade_Make(GUN_TYPE.LAUNCHER, false);

                break;
            case SKILL_KIND.JUMPER://슈퍼점프

                Char_Script[MY_INDEX].Super_Jump_Start();

                break;            
            case SKILL_KIND.VISION://투시

                Char_Script[MY_INDEX].Vision_Start(Char_Script[MY_INDEX].Sub_Skill_Running_Time);

                break;
            case SKILL_KIND.ROLLER://구르기 돌진

                //스킬 쓰고 난후 일정시간동안은 총 못나가게 막는다.
                Skill_Delay_Time = 5.0f + 0.2f;

                if (Player_Shot_State != PLAYER_SHOT_STATE.IDEL)
                {
                    Char_Script[MY_INDEX].Shot_Off();

                    Player_Shot_State = PLAYER_SHOT_STATE.IDEL;
                }

                Char_Script[MY_INDEX].Roller_Start(Char_Script[MY_INDEX].Sub_Skill_Running_Time);

                break;
            case SKILL_KIND.GUARD://방어막

                Char_Script[MY_INDEX].Guard_Start(Char_Script[MY_INDEX].Sub_Skill_Running_Time);

                break;
            case SKILL_KIND.BOOMER://수류탄 다수 투척

                //스킬 쓰고 난후 일정시간동안은 총 못나가게 막는다.
                Skill_Delay_Time = 1.5f;

                if (Player_Shot_State != PLAYER_SHOT_STATE.IDEL)
                {
                    Char_Script[MY_INDEX].Shot_Off();

                    Player_Shot_State = PLAYER_SHOT_STATE.IDEL;
                }

                Char_Script[MY_INDEX].Boomer_Start();

                break;
            case SKILL_KIND.FLAMER://화염방사기

                //스킬 쓰고 난후 일정시간동안은 총 못나가게 막는다.
                Skill_Delay_Time = 5.0f;

                if (Player_Shot_State != PLAYER_SHOT_STATE.IDEL)
                {
                    Char_Script[MY_INDEX].Shot_Off();

                    Player_Shot_State = PLAYER_SHOT_STATE.IDEL;
                }

                Char_Script[MY_INDEX].Flamer_Start(Char_Script[MY_INDEX].Sub_Skill_Running_Time);

                break;            
            case SKILL_KIND.ROCKET://직사포

                //스킬 쓰고 난후 일정시간동안은 총 못나가게 막는다.
                Skill_Delay_Time = 1.0f;

                if (Player_Shot_State != PLAYER_SHOT_STATE.IDEL)
                {
                    Char_Script[MY_INDEX].Shot_Off();

                    Player_Shot_State = PLAYER_SHOT_STATE.IDEL;
                }

                Char_Script[MY_INDEX].Grenade_Make(GUN_TYPE.ROCKET_SKILL, false);

                break;
            case SKILL_KIND.FLYING://날아다니기

                Char_Script[MY_INDEX].Flying_Start(20.0f);

                break;
            case SKILL_KIND.RAGE://크리티컬 100%

                Char_Script[MY_INDEX].Rage_Start();

                break;
            case SKILL_KIND.THROUGH_SHOT://관통샷

                //스킬 쓰고 난후 일정시간동안은 총 못나가게 막는다.
                Skill_Delay_Time = 1.5f;

                if (Player_Shot_State != PLAYER_SHOT_STATE.IDEL)
                {
                    Char_Script[MY_INDEX].Shot_Off();

                    Player_Shot_State = PLAYER_SHOT_STATE.IDEL;
                }

                Char_Script[MY_INDEX].ThroughShot_Start();

                break;
        }

        //스킬 리로드 시간 셋팅
        Char_Script[MY_INDEX].Sub_Skill_Reload_Time = Char_Script[MY_INDEX].Sub_Skill_Reload_MAX_Time;        
    }

    //====================================================================================================================================================================================================
            
    //수류탄 버튼
    public void Grenade_Button()
    {
        if (Char_Script[MY_INDEX].MY_HP == 0.0f) return;

        if (Char_Script[MY_INDEX].Flamer_Time != 0.0f) return;        

        if (Char_Script[MY_INDEX].Grenade_Reload_Time != 0.0f) return;

        if (Char_Script[MY_INDEX].Roller_Time != 0.0f) return;

        //수류탄 리로드 시간 셋팅
        Char_Script[MY_INDEX].Grenade_Reload_Time = Char_Script[MY_INDEX].Grenade_Reload_MAX_Time;

        //수류탄 쓰고 난후 일정시간동안은 총 못나가게 막는다.
        Grenade_Delay_Time = 0.5f;
        
        if (Player_Shot_State != PLAYER_SHOT_STATE.IDEL)
        {
            Char_Script[MY_INDEX].Shot_Off();

            Player_Shot_State = PLAYER_SHOT_STATE.IDEL;
        }

        if (GamePlay_UI.Zoom_Ani_State == ZOOM_ANI_STATE.ZOOM_END)
        {
            GamePlay_UI.Zoom_Ani_Init(ZOOM_ANI_STATE.ZOOM_OUT);

            Char_Script[MY_INDEX].Char_View_Check(true);
        }

        Char_Script[MY_INDEX].Grenade_Make(GUN_TYPE.GRENADE, false);
    }

    //====================================================================================================================================================================================================

    //점프 버튼 연산
    public void Jump_Button()
    {
        if (Char_Script[MY_INDEX].MY_HP == 0.0f) return;

        if (Char_Script[MY_INDEX].Jump_State == JUMP_STATE.JUMP_UP || Char_Script[MY_INDEX].Jump_State == JUMP_STATE.SUPER_JUMP_UP || Char_Script[MY_INDEX].Flying_Start_Time > 0.0f) return;

        Char_Script[MY_INDEX].Jump_Start();
    }

    //====================================================================================================================================================================================================

    //환경설정 버튼 연산
    public void Option_Button()
    {
        Option_State = OPTION_STATE.PANEL_OPEN;

        Left_Cursor_Init();
        Right_Cursor_Init();

        //환경설정 패널 만들기
        Make_Option_OJ();
    }

    //환경설정 나가기 버튼 연산
    public void Option_Exit_Button()
    {
        Option_State = OPTION_STATE.IDEL;

        Left_Cursor_Init();
        Right_Cursor_Init();

        Option_UI.Option_View(false);

        //환경설정값 셋팅
        Option_Init();
    }

    //환경설정값 셋팅
    void Option_Init()
    {
        OptionSetting Setting = SendManager.Instance.Get_OptionSettingInfo();
        Option_Auto_Shot = Setting.AttackType;
        Option_Sensitive = Setting.Sensitive * 0.01f;
        Option_Sensitive_Zoom = Setting.SensitiveZoomIn * 0.01f;

        //if (Char_Script[MY_INDEX].Gun_Type == GUN_TYPE.SNIPER)
        //{
        //    //자동, 수동일때에 따른 샷버튼 이미지 그리기 셋팅
        //    GamePlay_UI.Shot_Right_Button_View(true);
        //}
        //else
        //{
        //    //자동, 수동일때에 따른 샷버튼 이미지 그리기 셋팅
        //    GamePlay_UI.Shot_Right_Button_View(!Option_Auto_Shot);
        //}

        //자동, 수동일때에 따른 샷버튼 이미지 그리기 셋팅
        GamePlay_UI.Shot_Right_Button_View(!Option_Auto_Shot);//스나

        //자동,수동 컨트럴 버튼이미지 셋팅
        GamePlay_UI.Shot_Control_Button_View(Option_Auto_Shot);

        //기준큐브 움직이는 각도의 폭
        Camera_Rotation_Speed_Init();

        if (Option_Auto_Shot == false)
        {
            if (Player_Shot_State != PLAYER_SHOT_STATE.IDEL) Player_Shot_State = PLAYER_SHOT_STATE.SHOT_END;

            Sniper_Auto_Target_State = 0;
            Sniper_Auto_Target_Time = 0.0f;

            //스나이퍼 자동 타켓팅됐을때 나타나는 원형 게이지
            GamePlay_UI.ShotDelay_Aim_View(0.0f, 0.0f);
        }

        //줌인 상태에서 자동,수동 컨트럴 했을때 셋팅
        if (GamePlay_UI.Zoom_Ani_State != ZOOM_ANI_STATE.IDEL)
        {
            if (Option_Auto_Shot) GamePlay_UI.Shot_Left_Button_View(false);
            else GamePlay_UI.Shot_Left_Button_View(true);
        }
    }

    //게임중 자동,수동 컨트럴 버튼 연산
    public void Shot_Control_Button()
    {
        OptionSetting Setting = SendManager.Instance.Get_OptionSettingInfo();

        if (Setting.AttackType) Setting.AttackType = false;
        else Setting.AttackType = true;

        SendManager.Instance.Save_OptionSettingInfo(Setting);
                                        
        //환경설정값 셋팅
        Option_Init();
    }

    //====================================================================================================================================================================================================

    //채팅 버튼 연산
    public void Chatting_Button()
    {
        Left_Cursor_Init();
        Right_Cursor_Init();

        Keyboard_Open();
    }

    //화면에 키보드 입력창 띄우기
    void Keyboard_Open()
    {
        if (Keyboard_Check == false)
        {
            Keyboard_Check = true;
            Keyboard = TouchScreenKeyboard.Open("");//키보드 화면에 보이기
            Keyboard_String = ""; 
        }
    }

    //화면의 키보드 닫기
    void Keyboard_Close()
    {
        Keyboard_Check = false;
        if (Keyboard != null) Keyboard.active = false;
        Keyboard_String = "";
    }

    //키보드 입력 체크
    void Keyboard_Operation()
    {
        //if (Keyboard_Check && Keyboard.done)
        //{
        //    Keyboard_Check = false;

        //    Keyboard_String = Keyboard.text.Trim();//아무 글자도 안넣었다면 보내기 없다

        //    if (!Keyboard_String.Equals("")) SendManager.Instance.Send_ChatMessage(Keyboard_String);//채팅 내용 보내기
        //}
    }

    //====================================================================================================================================================================================================

    //게임 나가기 패널 오픈 버튼
    public void Exit_UI_Button()
    {
        if (Exit_UI_State == EXIT_UI_STATE.IDEL)
        {
            Exit_UI_State = EXIT_UI_STATE.PANEL_OPEN;

            Left_Cursor_Init();
            Right_Cursor_Init();

            //게임 나기기 패널 만들기
            Make_Exit_OJ();
        }        
    }

    //나가기 패널 Yes 버튼
    public void Exit_UI_Yes_Button()
    {
        if (GamePlay_State != GAMEPLAY_STATE.GAME_PLAY) return;

        GamePlay_State = GAMEPLAY_STATE.GAME_OUT_INIT;
    }

    //나가기 패널 No 버튼
    public void Exit_UI_No_Button()
    {
        Exit_UI_State = EXIT_UI_STATE.IDEL;

        Left_Cursor_Init();
        Right_Cursor_Init();

        Exit_UI.Exit_UI_View(false);
    }

    //====================================================================================================================================================================================================

    //안드로이드 취소키 처리
    void ESC_Key_Operation()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Exit_UI_State != EXIT_UI_STATE.IDEL)
            {
                Exit_UI_No_Button();
            }
            else if (Option_State != OPTION_STATE.IDEL)
            {
                Option_Exit_Button();
            }
            else
            {
                Exit_UI_Button();
            }
        }
    }

    //====================================================================================================================================================================================================

    //조준점 벌어짐 연산
    void Aim_Operation()
    {
        if (Char_Script[MY_INDEX].Gun_Type == GUN_TYPE.SNIPER && GamePlay_UI.Zoom_Ani_State == ZOOM_ANI_STATE.IDEL)
        {
            Aim_Move_Size = Mathf.Lerp(Aim_Move_Size, 100.0f - 10, Time.deltaTime * 2.0f);
        }
        else
        {
            Aim_Move_Size = Mathf.Lerp(Aim_Move_Size, 100.0f - Char_Script[MY_INDEX].Aim_Init[(int)Char_Script[MY_INDEX].Gun_Equip_State], Time.deltaTime * 2.0f);
        }

        GamePlay_UI.Aim_Move_Init(Aim_Move_Size);
    }

    //====================================================================================================================================================================================================

    //플레이어 UI 효과 연산
    void Play_UI_Operation()
    {
        if (Char_Script[MY_INDEX].Damage_UI_Check)
        {
            Char_Script[MY_INDEX].Damage_UI_Check = false;

            //맞았다는 UI 효과 나타내기
            GamePlay_UI.Damage_UI_View();
        }

        if (HeadShot_UI_Check)
        {
            HeadShot_UI_Check = false;

            //헤드샷 효과
            GamePlay_UI.Headshot_UI_View();
        }

        if (Combo_Check)
        {
            Combo_Check = false;
            GamePlay_UI.Combo_Effect_View(Combo_Count);
        }

        if (Respawn_UI_Check)
        {
            Respawn_UI_Check = false;

            //리스폰 UI 켜기
            Enable_Respawn_UI_OJ();
        }
    }


    //====================================================================================================================================================================================================
        
    //게임 오버 받으면 일정시간 딜레이후 넘어간다. 게임오버 바로 넘어가면 캐릭터 죽는게 스킵 되버린다.
    void GameOver_Net_Check()
    {
        if (GameOver_Net_Check_Time == 0.0f) return;

        GameOver_Net_Check_Time -= Time.deltaTime;
        if (GameOver_Net_Check_Time <= 0.0f)
        {
            GamePlay_State = GAMEPLAY_STATE.GAME_OVER_INIT;
        }
    }

    //게임 시간이 끝났다, 보상 결과 올때까지 대기
    public void Recv_TimerOver(ByteData _Receive_data)
    {
        //	byte : 승리팀 (Red:0, Blue:1, Draw:2)

        byte Win_Result = 0;

        _Receive_data.OutPutVariable(ref Win_Result);
        
        GameOver_Win_Team = (GAMEOVER_WIN_TEAM)Win_Result;

        GameOver_Reward_Check = 0;

        GameOver_Net_Check_Time = 1.0f;
    }

    //서버에서 게임 보상 셋팅 완료했다 게임 완전히 끝낸다.
    public void Recv_RewardEnd()
    {
        GameOver_Reward_Check = 1;
    }

    public void User_Data_Updata_OK()
    {
        if (Link_Script.i.Play_Mode == BattleKind.ALONE_PLAY_BATTLE)//싱글모드
        {
            Single_GameOver_UI.Panel_GameOver_Button_View();
        }
        else
        {
            Progress_UI.Panel_GameOver_Button_View();
        }        
    }

    //게임 나가기 셋팅
    public void GameOver_Init(bool Exit_Check)
    {
        //게임플레이중 오브젝트 삭제 및 정리
        GamePlay_Out_Setting();
                
        //게임 나가기 보내기
        Net_Script.Send_Game_Out();
        Net_Script.Network_State = NETWORK_STATE.IDEL;

        //게임 오버가 아닌 중간에 강제로 나가기 체크
        if (Exit_Check)
        {
            // 전투씬 끝나고 로비로 이동 함수
            SendManager.Instance.Set_GoToLobby();
        }        
    }

    //게임플레이중 오브젝트 삭제 및 정리
    public void GamePlay_Out_Setting()
    {
        Char_Move_Info_List.Clear();
        Grenade_Move_Info_List.Clear();

        //캐릭터 삭제하기
        foreach (var _Char_Script in Char_Script)
        {
            _Char_Script.Value.Char_Now_Destroy();
        }

        //캐릭터 데이터 삭제하기
        Char_Script.Clear();
        Link_Script.i.User_Data.Clear();

        //월드에 있는 수류탄, 임펙트 오브젝트 전부 삭제
        Transform[] GetTransforms = Link_Script.i.Effect_OJ_Set.GetComponentsInChildren<Transform>();
        foreach (Transform child in GetTransforms)
        {
            if (child != Link_Script.i.Effect_OJ_Set) Destroy(child.gameObject);
        }
    }

    //====================================================================================================================================================================================================

    //VoiceChat_Script Voice_Chat_Script;

    //음성채팅 만들기
    void Voice_Make_OJ()
    {
		//Transform Voice_Chat_OJ_Transform = Instantiate(Resources.Load("Battle_Prefab/1_Game_OJ_Folder/Voice_Chat_OJ") as GameObject).GetComponent<Transform>();
		//Voice_Chat_OJ_Transform.position = new Vector3(0, 0, 0);
		//Voice_Chat_OJ_Transform.localScale = new Vector3(1, 1, 1);

		//Voice_Chat_Script = Voice_Chat_OJ_Transform.GetComponent<VoiceChat_Script>();
    }

    //상대방 캐릭터 스킬데이터 네트웍 데이터 받기
    public void Recv_Voice_Chat_Data(ByteData _Receive_data)
    {
        //Voice_Chat_Script.Recive_Data(_Receive_data);
    }

    //====================================================================================================================================================================================================

    //플레이 UI 띄우기
    void Make_GamePlay_UI_OJ()
    {
        if (GamePlay_UI == null)
        {
            Instantiate(Resources.Load("Battle_Prefab/2_UI_Folder/GamePlay_UI_Set") as GameObject).name = "GamePlay_UI_Set";
            GamePlay_UI = GameObject.Find("GamePlay_UI_Set").transform.GetComponent<GamePlay_UI_Script>();
        }
        
        GamePlay_UI.Panel_Init();
    }

    //====================================================================================================================================================================================================

    //이동, 각도 조절 이미지 만들기
    void Make_Joystick_OJ()
    {
        if (Move_Joystick == null)
        {
            //이동 커서 이미지 만들기 
            Instantiate(Resources.Load("Battle_Prefab/2_UI_Folder/Move_Image_Set") as GameObject).name = "Move_Image_Set";
            GameObject.Find("Move_Image_Set").GetComponent<Transform>().transform.SetParent(GameObject.Find("Canvas").transform);
            GameObject.Find("Move_Image_Set").GetComponent<Transform>().transform.localScale = new Vector3(1, 1, 1);

            Move_Joystick = GameObject.Find("Move_Image_Set").GetComponent<Joystick_Script>();
        }

        if (Rotation_Joystick == null)
        {
            //각도 조절 이미지 만들기
            Instantiate(Resources.Load("Battle_Prefab/2_UI_Folder/Move_Image_Set") as GameObject).name = "Rotation_Image_Set";
            GameObject.Find("Rotation_Image_Set").GetComponent<Transform>().transform.SetParent(GameObject.Find("Canvas").transform);
            GameObject.Find("Rotation_Image_Set").GetComponent<Transform>().transform.localScale = new Vector3(1, 1, 1);

            Rotation_Joystick = GameObject.Find("Rotation_Image_Set").GetComponent<Joystick_Script>();
        }

        for (int i = 0; i < Touch_FingerId.Length; i++)
        {
            Touch_FingerId[i] = -1;
        }
    }

    //====================================================================================================================================================================================================

    //환경설정 패널 만들기
    void Make_Option_OJ()
    {
        if (Option_UI == null)
        {
            Instantiate(Resources.Load("Battle_Prefab/2_UI_Folder/Gameplay_UI_PopupSetting") as GameObject).name = "Gameplay_UI_PopupSetting";
            Option_UI = GameObject.Find("Gameplay_UI_PopupSetting").transform.GetComponent<Option_Script>();
        }

        Option_UI.Option_View(true);
    }

    //====================================================================================================================================================================================================

    //게임 나기기 패널 만들기
    void Make_Exit_OJ()
    {
        if (Exit_UI == null)
        {
            Instantiate(Resources.Load("Battle_Prefab/2_UI_Folder/Gameplay_UI_PopupSelective") as GameObject).name = "Gameplay_UI_PopupSelective";
            Exit_UI = GameObject.Find("Gameplay_UI_PopupSelective").transform.GetComponent<Exit_UI_Script>();
        }

        Exit_UI.Exit_UI_View(true);
    }

    //====================================================================================================================================================================================================

    //현재 스코어 정보창 만들기
    void Make_Battleprogress_OJ()
    {
        if (Progress_UI == null)
        {
            Instantiate(Resources.Load("Battle_Prefab/2_UI_Folder/Gameplay_UI_Battleprogress") as GameObject).name = "Gameplay_UI_Battleprogress";
            Progress_UI = GameObject.Find("Gameplay_UI_Battleprogress").transform.GetComponent<Progress_Script>();
        }        
    }

    //중간 집계 상황판 버튼 연산
    public void Progress_Button_Operation(PROGRESS_STATE _Progress_State)
    {
        Progress_State = _Progress_State;
    }

    //스코어 정보창 나가기 버튼
    public void Progress_Exit_Button()
    {
        GamePlay_State = GAMEPLAY_STATE.GAME_OVER_WEB_SEND;
    }

    //중간 집계 상황판 보기
    void Progress_Operation()
    {
        switch (Progress_State)
        {
            case PROGRESS_STATE.IDEL:

                break;
            case PROGRESS_STATE.PANEL_OPEN:
            case PROGRESS_STATE.RESPAWN_PANEL_OPEN:

                //현재 전체 스코어 요청
                Net_Script.Send_Now_Total_Score();

                Make_Battleprogress_OJ();//현재 스코어 정보창 만들기

                Progress_UI.Panel_Play_Init();

                Progress_UI.Panel_View(true);

                if (Progress_State == PROGRESS_STATE.RESPAWN_PANEL_OPEN)
                {
                    Progress_UI.BG_Img_OJ_View(true);
                }
                else
                {
                    Progress_UI.BG_Img_OJ_View(false);
                }                

                Progress_State = PROGRESS_STATE.PANEL_VIEW;

                break;
            case PROGRESS_STATE.PANEL_VIEW:

                break;
            case PROGRESS_STATE.PANEL_CLOSE:

                Left_Cursor_Init();
                Right_Cursor_Init();

                if (Progress_UI != null) Progress_UI.Panel_View(false);

                Progress_State = PROGRESS_STATE.IDEL;

                break;
        }
    }

    public ushort R_Total_Kill;
    public ushort R_Total_Death;
    public ushort B_Total_Kill;
    public ushort B_Total_Death;

    public struct PROGRESS_PANEL_INFO
    {
        public uint User_ID;
        public ushort ClanMark_Index;
        public byte[] Country_Index;
        public String NickName;        
        public ushort Kill_Num;
        public ushort Death_Num;
    }
    public PROGRESS_PANEL_INFO[] R_Progress_Info;
    public PROGRESS_PANEL_INFO[] B_Progress_Info;

    void Progress_Info_Init()
    {
        R_Total_Kill = 0;
        R_Total_Death = 0;
        B_Total_Kill = 0;
        B_Total_Death = 0;

        R_Progress_Info = new PROGRESS_PANEL_INFO[6];
        B_Progress_Info = new PROGRESS_PANEL_INFO[6];
    }

    //현재 전체 스코어 데이터 받기
    public void Recv_Now_Total_Score(ByteData _Receive_data)
    {
        // ushort : Red팀 킬 카운트 (Red:0)
        // ushort : Blue팀 킬 카운트 (Blue:1)
        // ushort : Red팀 데스 카운트 (Red:0)
        // ushort : Blue팀 데스 카운트 (Blue:1)
        // byte : 유저수
        //		uint : 방 유저 WebUserID 
        //		ushort : 킬수
        //		ushort : 데스수
                
        byte User_Count = 0;
        
        _Receive_data.OutPutVariable(ref R_Total_Kill);
        _Receive_data.OutPutVariable(ref B_Total_Kill);
        _Receive_data.OutPutVariable(ref R_Total_Death);
        _Receive_data.OutPutVariable(ref B_Total_Death);
        _Receive_data.OutPutVariable(ref User_Count);
        
        uint[] User_ID = new uint[User_Count];
        ushort[] User_Kill = new ushort[User_Count];
        ushort[] User_Death = new ushort[User_Count];

        for (int i = 0; i < User_Count; i++)
        {
            _Receive_data.OutPutVariable(ref User_ID[i]);
            _Receive_data.OutPutVariable(ref User_Kill[i]);
            _Receive_data.OutPutVariable(ref User_Death[i]);
        }

        for (int i = 0; i < R_Progress_Info.Length; i++)
        {
            R_Progress_Info[i].User_ID = 0;
            R_Progress_Info[i].ClanMark_Index = 0;
            R_Progress_Info[i].Country_Index = new byte[2];
            R_Progress_Info[i].Country_Index[0] = 0;
            R_Progress_Info[i].Country_Index[1] = 0;
            R_Progress_Info[i].NickName = "";
            R_Progress_Info[i].Kill_Num = 0;
            R_Progress_Info[i].Death_Num = 0;

            B_Progress_Info[i].User_ID = 0;
            B_Progress_Info[i].ClanMark_Index = 0;
            B_Progress_Info[i].Country_Index = new byte[2];
            B_Progress_Info[i].Country_Index[0] = 0;
            B_Progress_Info[i].Country_Index[1] = 0;
            B_Progress_Info[i].NickName = "";
            B_Progress_Info[i].Kill_Num = 0;
            B_Progress_Info[i].Death_Num = 0;
        }

        int R_Team_Count = 0;
        int B_Team_Count = 0;

        for (int i = 0; i < User_Count; i++)
        {
            if (Char_Script.ContainsKey(User_ID[i]) == false) continue;

            if (Link_Script.i.User_Data.ContainsKey(User_ID[i]) == false) continue;
                        
            if (Char_Script[User_ID[i]].User_Team == 0)//Red
            {                
                R_Progress_Info[R_Team_Count].User_ID = User_ID[i];
                R_Progress_Info[R_Team_Count].ClanMark_Index = Char_Script[User_ID[i]].User_Clan_Mark;
                R_Progress_Info[R_Team_Count].Country_Index[0] = Char_Script[User_ID[i]].User_Country_Mark[0];
                R_Progress_Info[R_Team_Count].Country_Index[1] = Char_Script[User_ID[i]].User_Country_Mark[1];
                R_Progress_Info[R_Team_Count].NickName = Char_Script[User_ID[i]].User_NickName;
                R_Progress_Info[R_Team_Count].Kill_Num = User_Kill[i];
                R_Progress_Info[R_Team_Count].Death_Num = User_Death[i];
                R_Team_Count++;
            }
            else//Blue
            {
                B_Progress_Info[B_Team_Count].User_ID = User_ID[i];
                B_Progress_Info[B_Team_Count].ClanMark_Index = Char_Script[User_ID[i]].User_Clan_Mark;
                B_Progress_Info[B_Team_Count].Country_Index[0] = Char_Script[User_ID[i]].User_Country_Mark[0];
                B_Progress_Info[B_Team_Count].Country_Index[1] = Char_Script[User_ID[i]].User_Country_Mark[1];
                B_Progress_Info[B_Team_Count].NickName = Char_Script[User_ID[i]].User_NickName;
                B_Progress_Info[B_Team_Count].Kill_Num = User_Kill[i];
                B_Progress_Info[B_Team_Count].Death_Num = User_Death[i];
                B_Team_Count++;
            }
        }
        
        //화면 상단의 팀 킬,데스 카운트 UI
        GamePlay_UI.Team_Kill_Dead_UI_Init(R_Total_Kill, R_Total_Death, B_Total_Kill, B_Total_Death);

        //프로그레스 패널이 화면에 떠있다면 내용 갱신시켜준다.
        if (Progress_UI != null)
        {
            Progress_UI.Data_Refresh_Check = true;
        }
    }

    //====================================================================================================================================================================================================

    //싱글모드 게임 오버 UI 만들기
    void Make_Bot_Mode_GameOver_UI_OJ()
    {
        if (Single_GameOver_UI == null)
        {
            Instantiate(Resources.Load("Battle_Prefab/2_UI_Folder/Gameplay_UI_Singleprogress") as GameObject).name = "Gameplay_UI_Singleprogress";
            Single_GameOver_UI = GameObject.Find("Gameplay_UI_Singleprogress").transform.GetComponent<Single_GameOver_Script>();
        }
    }

    //====================================================================================================================================================================================================

    //리스폰 UI 만들기
    void Make_Respawn_UI_OJ()
    {
        if (Respawn_UI == null)
        {
            Instantiate(Resources.Load("Battle_Prefab/2_UI_Folder/GamePlay_UI_Respawn") as GameObject).name = "GamePlay_UI_Respawn";
            Respawn_UI = GameObject.Find("GamePlay_UI_Respawn").transform.GetComponent<Respawn_UI_Script>();
        }
    }

    //리스폰 UI 켜기
    void Enable_Respawn_UI_OJ()
    {        
        Respawn_UI.Panel_Init(Respawn_Player_Killer_NickName, Respawn_Killing_Message, Respawn_Player_Kill_Count, Respawn_Player_Death_Count);

        GamePlay_UI.Panel_View(false);
        Respawn_UI.gameObject.SetActive(true);
    }

    //리스폰 UI 끄기
    public void Disable_Respawn_UI_OJ()
    {
        if (GamePlay_UI != null) GamePlay_UI.Panel_View(true);
        if (Respawn_UI != null) Respawn_UI.gameObject.SetActive(false);

        if (Progress_State == PROGRESS_STATE.PANEL_VIEW) Progress_Button_Operation(PROGRESS_STATE.PANEL_CLOSE);
    }

    //즉시 리스폰 버튼 호출
    public void Now_Respawn_Button(RESPAWN_KIND _Respawn_Kind)
    {
        if (Respawn_UI.Now_Respawn_Button_Check)
        {
            Respawn_UI.Now_Respawn_Button_Check = false;

            ByteData Send_Buffer = new ByteData(128, 0);

            Send_Buffer.InPutByte((byte)_Respawn_Kind);
            
            //즉시 리스폰 데이터 보내기
            Net_Script.Send_Now_Respawn_Data(Send_Buffer);
        }
    }
    
    //====================================================================================================================================================================================================
        
    bool TimeCount(ref float _Time, float _Max_Time)
    {
        _Time += Time.deltaTime;
        if (_Time >= _Max_Time)
        {
            _Time = 0.0f;

            return true;
        }

        return false;
    }

    //====================================================================================================================================================================================================

    Transform Frame_Text_OJ = null;
    Text FrameText;
    float FrameTime = 0;
    int FrameCount = 0;

    void FrameView()
    {
        if (Frame_Text_OJ == null)
        {
            Instantiate(Resources.Load("Battle_Prefab/2_UI_Folder/Frame_Text_Set") as GameObject).name = "Frame_Text_Set";
            Frame_Text_OJ = GameObject.Find("Frame_Text_Set").transform;
            Frame_Text_OJ.SetParent(GameObject.Find("Canvas").transform);
            Frame_Text_OJ.localScale = new Vector3(1, 1, 1);
            Frame_Text_OJ.localPosition = new Vector3(0, 0, 0);

            FrameText = GameObject.Find("Frame_Text").GetComponent<Text>();
        }
        else
        {
            FrameTime += Time.deltaTime;
            if (FrameTime >= 1.0f)
            {
                Frame_Text_OJ.SetAsLastSibling();//현재 자식 오브젝트들중에서 제일 상위 레이어로 이동 시키기
                FrameText.text = "V Frame : " + FrameCount;
                FrameTime = 0;
                FrameCount = 0;
            }
            else
            {
                FrameCount++;
            }
        }
    }

    //====================================================================================================================================================================================================




    // =============================인게임 게임 아이템 처리관련================================
    class ObtainItem_Info
    {
        public byte createdIndex = 0;
        public ObtainGameItemKind ObtainItemUserKnd;
        public ByteData Receive_data;
    }


    public Dictionary<byte, ObtainGameItem> Dic_OtainItems = new Dictionary<byte, ObtainGameItem>();
    private List<ObtainItem_Info> Lst_willCreationObtainItem = new List<ObtainItem_Info>();
    private GameObject Object_OrinObtainGameItem;


    //hj : 아이템 작동
    void Operation_ObtainItem()
    {
        //생성 아이템잇는지 체크 잇으면생성,후 초기화
        for (int i = 0; i < Lst_willCreationObtainItem.Count; i++)
        {
            byte createIdx = Lst_willCreationObtainItem[i].createdIndex;

            if (!Dic_OtainItems.ContainsKey(createIdx))
            {
                //생성
                Make_ObtainItem(createIdx);
                //초기화
                Dic_OtainItems[createIdx].Init_Item(CHAR_USER_KIND.NETWORK, createIdx);
                //초기위치설정
                Dic_OtainItems[createIdx].Init_LocationPos(Lst_willCreationObtainItem[i].Receive_data);
            }
        }

        Lst_willCreationObtainItem.Clear();
    }


    //hj : 아이템 움직임관련 데이터 받기
    public void Recv_OtainGameItem_Move_Data(ByteData _Receive_data)
    {
        byte createdIndex = 0;
       // byte ObtainItemUserKnd = 0;
         
        _Receive_data.OutPutVariable(ref createdIndex);
       // _Receive_data.OutPutVariable(ref ObtainItemUserKnd);


        //생성 되지 않았다면 만들어 준다.
        if (Dic_OtainItems.ContainsKey(createdIndex) == false)
        {

            //ObtainItem_Info obItem = new ObtainItem_Info();
            //obItem.createdIndex = createdIndex;
            //obItem.Receive_data = _Receive_data; // 남은데이터 할당

            //Lst_willCreationObtainItem.Add(obItem);
            Make_ObtainItem(createdIndex);

            Dic_OtainItems[createdIndex].Init_Item(CHAR_USER_KIND.NETWORK, createdIndex);
            //초기위치설정
            Dic_OtainItems[createdIndex].Init_LocationPos(_Receive_data);
        }
        else
        {
            //아이템 이동데이터 전달
            Dic_OtainItems[createdIndex].Recv_ObtainGameItem_Data(_Receive_data);
        }
    }

   
    //hj : 아이템 생성
    void Make_ObtainItem(byte _createdIndex)
    {
        if(Object_OrinObtainGameItem == null)
        {
            Object_OrinObtainGameItem = Resources.Load("Battle_Prefab/1_Game_OJ_Folder/ObtainGameItem_Object") as GameObject;
        }

        ObtainGameItem obtainItem = Instantiate(Object_OrinObtainGameItem).GetComponent<ObtainGameItem>();
        obtainItem.name = "ObtainItem_" + _createdIndex;
        obtainItem.transform.SetParent(Link_Script.i.Effect_OJ_Set);
        Dic_OtainItems[_createdIndex] = obtainItem;
    }


    int TotalObtainItemCreatedCntForTest = 0;
    void ForceCreate_ObtainItem()
    {
        Make_ObtainItem((byte)TotalObtainItemCreatedCntForTest);

        if(Dic_OtainItems.ContainsKey((byte)TotalObtainItemCreatedCntForTest))
        {
            Vector3 pos = Char_Script[MY_INDEX].transform.position;

            Dic_OtainItems[(byte)TotalObtainItemCreatedCntForTest].Init_Item(CHAR_USER_KIND.PLAYER, (byte)TotalObtainItemCreatedCntForTest);

            Dic_OtainItems[(byte)TotalObtainItemCreatedCntForTest].Init_LocationPos(pos, Vector3.zero);

            TotalObtainItemCreatedCntForTest++;
        }
    }





}
