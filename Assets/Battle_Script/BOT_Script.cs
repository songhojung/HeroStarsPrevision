using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;

public class BOT_Script : MonoBehaviour
{
    String[] Ragdoll_Collider_Name = {  "Bip001 Pelvis", "Bip001 Spine1",
                                        "Bip001 L Thigh", "Bip001 L Calf", "Bip001 R Thigh", "Bip001 R Calf", "Bip001 L UpperArm", "Bip001 L Forearm", "Bip001 R UpperArm", "Bip001 R Forearm",
                                        "Bip001 Head"};

    public class Bot_Ragdoll_Rigidbody_class
    {
        public GameObject GameObject_OJ;
        public Transform Transform_OJ;
        public Rigidbody Rigid_Body;
    }
    public Dictionary<String, Bot_Ragdoll_Rigidbody_class> Bot_Ragdoll_Rigidbody = new Dictionary<String, Bot_Ragdoll_Rigidbody_class>();

    public class Bot_Out_Line_class
    {
        public Outline OJ;
    }
    public Dictionary<String, Bot_Out_Line_class> Bot_Out_Line_OJ = new Dictionary<String, Bot_Out_Line_class>();

    GamePlay_Script Game_Script;

    CameraControl_Script CameraControlScript;

    Network_Battle_Script Net_Script;

    CharacterController Char_Controller;

    Animator Char_Animator;

    LayerMask Ragdoll_Layer;
    LayerMask Ragdoll_Die_Layer;
    LayerMask Char_Controller_Layer;

    public GameObject Auto_Targeting_OJ;
    public Transform HeadShot_Target_OJ;

    public GameObject ARROW_3D_OJ;
            
    DIE_STATE Die_State;
    Vector3 Die_Atk_Start_Vec = new Vector3();
    Vector3 Die_Atk_Target_Vec = new Vector3();
    byte Die_Atk_GunType = 0;

    float Char_Gravity = 0.0f;

    public Vector3 Start_Rotation_Pos = new Vector3(0, 0, 0);

    public String Object_Name = "";

    public uint User_ID = 0;

    public byte User_Team = 0;

    public CHAR_TEAM_SATAE Char_Team_State;

    public float MY_HP = 0.0f;
    public float MY_MAX_HP = 0.0f;

    public String User_NickName = "";

    bool Char_Hit_Effect_Check = false;
    GUN_TYPE Char_Hit_Effect_GunType;
    Vector3 Char_Hit_Effect_Vec = new Vector3();

    float Destroy_Time = 0.0f;

    Transform Head_Info_Transform;
    Text Head_Info_Text;
    String Head_OJ_Name;

    AudioSource Audio_Source;

    void Awake()
    {
        Game_Script = GameObject.Find("Script_Object").GetComponent<GamePlay_Script>();

        CameraControlScript = GameObject.Find("Camera_Set_Object").GetComponent<CameraControl_Script>();

        Net_Script = GameObject.Find("Network_Script").GetComponent<Network_Battle_Script>();

        Char_Controller = GetComponent<CharacterController>();

        Char_Animator = transform.GetComponent<Animator>();

        Audio_Source = transform.GetComponent<AudioSource>();

        //----------------------------------------------------------------------------------------------------------------------------------

        Transform[] GetTransforms = transform.GetComponentsInChildren<Transform>();

        //랙돌 셋팅
        Ragdoll_Layer = LayerMask.NameToLayer("Ragdoll");
        Ragdoll_Die_Layer = LayerMask.NameToLayer("Ragdoll_Die");

        Bot_Ragdoll_Rigidbody.Clear();

        foreach (Transform child in GetTransforms)
        {
            for (int i = 0; i < Ragdoll_Collider_Name.Length; i++)
            {
                if (child.name.Equals(Ragdoll_Collider_Name[i]))
                {
                    if (Bot_Ragdoll_Rigidbody.ContainsKey(Ragdoll_Collider_Name[i]) == false)
                    {
                        Bot_Ragdoll_Rigidbody.Add(Ragdoll_Collider_Name[i], new Bot_Ragdoll_Rigidbody_class());

                        Bot_Ragdoll_Rigidbody[Ragdoll_Collider_Name[i]].GameObject_OJ = child.gameObject;
                        Bot_Ragdoll_Rigidbody[Ragdoll_Collider_Name[i]].Transform_OJ = child;
                        Bot_Ragdoll_Rigidbody[Ragdoll_Collider_Name[i]].Rigid_Body = child.GetComponent<Rigidbody>();
                        //Bot_Ragdoll_Rigidbody[Ragdoll_Collider_Name[i]].Rigid_Body.collisionDetectionMode = CollisionDetectionMode.Continuous;
                        Bot_Ragdoll_Rigidbody[Ragdoll_Collider_Name[i]].Rigid_Body.isKinematic = true;
                        Bot_Ragdoll_Rigidbody[Ragdoll_Collider_Name[i]].GameObject_OJ.layer = Ragdoll_Layer;
                    }
                    break;
                }
            }
        }

        //----------------------------------------------------------------------------------------------------------------------------------

        //아웃라인 효과 오브젝트 가져오기
        foreach (Transform child in GetTransforms)
        {
            if (child.GetComponent<Outline>() != null)
            {
                if (Bot_Out_Line_OJ.ContainsKey(child.name) == false)
                {
                    Bot_Out_Line_OJ.Add(child.name, new Bot_Out_Line_class());
                    Bot_Out_Line_OJ[child.name].OJ = child.GetComponent<Outline>();
                    Bot_Out_Line_OJ[child.name].OJ.enabled = false;
                }
            }
        }

        //----------------------------------------------------------------------------------------------------------------------------------
                
        Char_Controller_Layer = LayerMask.NameToLayer("Char_Controller");
    }

    void Start()
    {
        
    }
        
    void Update()
    {
        if (!Link_Script.i.IsConnected()) return;

        if (Game_Script.GameOver_Check) return;
                        
        //캐릭터 중력 연산
        Char_Gravity_Oeration();

        //캐릭터에 총 맞은 임펙트
        Bullethit_Operation();

        //캐릭터 죽는 연산
        Char_Die_Operation();
    }

    void LateUpdate()
    {
        if (Game_Script.GameOver_Check) return;

        this.transform.LookAt(Start_Rotation_Pos);
        this.transform.rotation = Quaternion.Euler(0, this.transform.rotation.eulerAngles.y, 0);

        //캐릭터 머리위에 정보 텍스트 연산
        Head_Info_Text_Operation();
    }

    //==========================================================================================================================================================

    public void Bot_Init()
    {
        Char_Team_State = CHAR_TEAM_SATAE.ENEMY_TEAM;

        Char_Hit_Effect_Check = false;
        Char_Hit_Effect_GunType = GUN_TYPE.RIFLE;
        Char_Hit_Effect_Vec = new Vector3();

        Auto_Targeting_OJ.SetActive(true);
    }

    //==========================================================================================================================================================

    //캐릭터 중력 연산
    void Char_Gravity_Oeration()
    {
        if (Die_State != DIE_STATE.IDEL) return;

        if (Char_Controller.isGrounded == false)
        {
            Char_Gravity += (Time.deltaTime * 0.5f);

            Char_Controller.Move(Vector3.down.normalized * Char_Gravity);
        }
        else
        {
            Char_Gravity = 0.0f;
        }      
    }

    //==========================================================================================================================================================

    public void Head_Text_Make()
    {
        Head_OJ_Name = User_ID + "_Head_Info_Text";

        Instantiate(Resources.Load("Battle_Prefab/1_Game_OJ_Folder/Head_Info_Text") as GameObject).name = Head_OJ_Name;
        Head_Info_Transform = GameObject.Find(Head_OJ_Name).GetComponent<Transform>();
        Head_Info_Transform.SetParent(GameObject.Find("Canvas").transform);
        Head_Info_Transform.localScale = new Vector3(1, 1, 1);
        Head_Info_Transform.localPosition = new Vector3(0, 0, 0);
        Head_Info_Transform.SetAsFirstSibling();//가장 첫번째 레이어로 맞춰서 켄버스 안에 있는 플레이 UI 밑으로 가려지게 만든다.

        Head_Info_Text = Head_Info_Transform.GetComponent<Text>();

        Head_Info_Text.color = new Color(ColorResult(210), ColorResult(28), ColorResult(28), 1.0f);
    }

    float Scale_Result = 0.0f;

    //캐릭터 머리위에 정보 텍스트 연산
    void Head_Info_Text_Operation()
    {
        if (Head_Info_Transform == null) return;

        Vector3 Text_Pos = transform.position + (Vector3.up * 1.15f);

        Head_Info_Transform.position = CameraControlScript.Main_Camera.WorldToScreenPoint(Text_Pos);

        //플레이어 카메라와 캐릭터의 거리에 따라 닉네임 폰트 크기 셋팅해준다.
        Scale_Result = 1.0f - ((CameraControlScript.Camera_Pos - this.transform.position).magnitude * 0.015f);
        Scale_Result = Mathf.Max(Mathf.Min(Scale_Result, 1.0f), 0.4f);
        Head_Info_Transform.transform.localScale = new Vector3(Scale_Result, Scale_Result, 1.0f);

        Head_Info_Text.enabled = true;

        //카메라 앞에 오브젝트가 있는지 체크하기
        if (Char_Team_State == CHAR_TEAM_SATAE.PLAYER_TEAM)
        {
            if (!IsInView(Head_Info_Transform.position, Text_Pos, Object_Name, false)) Head_Info_Text.enabled = false;
        }
        else
        {
            if (Game_Script.Char_Script.ContainsKey(Link_Script.i.User_ID) && Game_Script.Char_Script[Link_Script.i.User_ID].Vision_Time == 0.0f)
            {
                if (!IsInView(Head_Info_Transform.position, Text_Pos, Object_Name, true)) Head_Info_Text.enabled = false;
            }
            else
            {
                if (!IsInView(Head_Info_Transform.position, Text_Pos, Object_Name, false)) Head_Info_Text.enabled = false;
            }
        }

        if (Die_State == DIE_STATE.IDEL && Game_Script.GamePlay_State == GAMEPLAY_STATE.GAME_PLAY)
        {
            Head_Info_Text.text = "" + User_NickName;
        }
        else
        {
            Head_Info_Text.text = "";
        }
    }

    RaycastHit FrontCheck_Hit;

    //카메라 앞에 오브젝트가 있는지 체크하기
    private bool IsInView(Vector3 FrontCheck_PointOnScreen, Vector3 Target_Vec, String Target_Name, bool Raycast_Check)
    {
        //타겟이 앞에 없다면
        if (FrontCheck_PointOnScreen.z < 0) return false;

        //타겟이 화면상에 없다면
        if ((FrontCheck_PointOnScreen.x < 0) || (FrontCheck_PointOnScreen.x > Screen.width) || (FrontCheck_PointOnScreen.y < 0) || (FrontCheck_PointOnScreen.y > Screen.height)) return false;

        //타켓이 장애물에 가려졌다면
        //if (Raycast_Check && Physics.Linecast(CameraControlScript.Camera_Pos, Target_Vec, out FrontCheck_Hit, ~(1 << Char_Controller_Layer)))
        if (Raycast_Check && Physics.Linecast(CameraControlScript.Camera_Pos, Target_Vec, out FrontCheck_Hit, ~(1 << Char_Controller_Layer | 1 << LayerMask.NameToLayer("HeadShot_OJ"))))
        {
            if (FrontCheck_Hit.transform.name.Equals(Target_Name) == false) return false;
        }

        return true;
    }

    //==========================================================================================================================================================================================================

    int Damage_Num_Effect_Dir = 0;

    //어택 데미지 데이터 받기
    public void Bot_Damage_Operation(float MY_Total_Damage, Vector3 _ATK_Start_Vec, Vector3 _ATK_Target_Vec, byte _Gun_Type, bool _HeadShot_Check, bool Critical_Check)
    {
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        MY_HP -= MY_Total_Damage;
        if (MY_HP <= 0.0f) MY_HP = 0.0f;

        if (MY_HP <= 0.0f && Die_State == DIE_STATE.IDEL)
        {
            Die_State = DIE_STATE.ANI_START;
            Die_Atk_Start_Vec = _ATK_Start_Vec;
            Die_Atk_Target_Vec = _ATK_Target_Vec;
            Die_Atk_GunType = _Gun_Type;

            Game_Script.Bot_Kill_Count++;

            Game_Script.Bot_Kill_Operation(User_ID, _HeadShot_Check);

            //--------------------------------------------------------------------------------------
            ByteData Send_Buffer = new ByteData(128, 0);

            //현재 플레이어가 사용하고 있는 유닛의 인덱스
            Send_Buffer.InPutByte(Game_Script.Char_Script[Link_Script.i.User_ID].Char_Index);

            //봇 죽인 데이터 보내기
            Net_Script.Send_Bot_Kill_Data(Send_Buffer);
            //--------------------------------------------------------------------------------------
        }

        //수류탄, 유탄, 자살이 아닐때 캐릭터 맞았을때 터지는 임펙트 안그려준다.
        if ((GUN_TYPE)_Gun_Type != GUN_TYPE.GRENADE && (GUN_TYPE)_Gun_Type != GUN_TYPE.BOOMER_SKILL && (GUN_TYPE)_Gun_Type != GUN_TYPE.LAUNCHER && (GUN_TYPE)_Gun_Type != GUN_TYPE.SUICIDE)
        {
            Char_Hit_Effect_Check = true;
            Char_Hit_Effect_GunType = (GUN_TYPE)_Gun_Type;
            Char_Hit_Effect_Vec = _ATK_Target_Vec;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        //데미지 숫자 만들어주기
        Transform GamePlay_Damage_Transform = Instantiate(Resources.Load("Battle_Prefab/2_UI_Folder/GamePlay_Damage") as GameObject).GetComponent<Transform>();
        GamePlay_Damage_Transform.SetParent(GameObject.Find("Canvas").transform);
        GamePlay_Damage_Transform.localScale = new Vector3(1, 1, 1);
        GamePlay_Damage_Transform.localPosition = new Vector3(0, 0, 0);
        GamePlay_Damage_Transform.SetAsLastSibling();//가장 나중에 그려진 레이어로 맞추기

        //텍스트 스크립트 초기화        
        GamePlay_Damage_Transform.GetComponent<Damage_Num_Script>().Make_Init(User_ID, Critical_Check, MY_Total_Damage, Damage_Num_Effect_Dir);

        if (++Damage_Num_Effect_Dir >= 2) Damage_Num_Effect_Dir = 0;

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    }

    //캐릭터에 총 맞은 임펙트
    void Bullethit_Operation()
    {
        if (Char_Hit_Effect_Check == false) return;
        Char_Hit_Effect_Check = false;

        GameObject Bullethit_OJ;

        if (Char_Hit_Effect_GunType == GUN_TYPE.THROUGH_SHOT_SKILL)
        {
            Bullethit_OJ = Instantiate(Resources.Load("Battle_Prefab/3_Effect_Folder/Laserballhit") as GameObject);
        }
        else
        {
            Bullethit_OJ = Instantiate(Resources.Load("Battle_Prefab/3_Effect_Folder/Bullethit") as GameObject);
        }

        Bullethit_OJ.transform.SetParent(Link_Script.i.Effect_OJ_Set);
        Bullethit_OJ.transform.position = Char_Hit_Effect_Vec;
    }

    //캐릭터 죽을때 현재 스킬등 변수값 초기화
    void Char_Die_Skill_Init()
    {
        //죽고 난다음에 캐릭터 컨트럴러에 맞아서 수류탄이 튕기기때문에 죽으면 컬리더 셋팅해준다.
        Char_Controller.center = new Vector3(0.0f, -100.0f, 0.0f);
        Auto_Targeting_OJ.SetActive(false);

        //죽을때 헤드샷 오브젝트 크기 정상으로 돌려놓는다. 너무 커서 캐릭터가 눕질 않는다.
        HeadShot_Target_OJ.localScale = new Vector3(0.6f, 0.6f, 0.6f);

        ARROW_3D_OJ.SetActive(false);

        Destroy_Time = 0.0f;

        //Head_Info_Text.enabled = false;
    }

    //캐릭터 죽는 연산
    void Char_Die_Operation()
    {
        switch (Die_State)
        {
            case DIE_STATE.IDEL:

                break;
            case DIE_STATE.ANI_START:
                                
                //캐릭터 죽을때 현재 스킬등 변수값 초기화
                Char_Die_Skill_Init();

                //사운드 재생
                SendManager.Instance.PlayGameSound(AUDIOSOURCE_TYPE.AUDIO_3D, 30, Audio_Source);

                Die_State = DIE_STATE.RAGDOLL_START;

                break;
            case DIE_STATE.RAGDOLL_START:

                //리그돌 연산 시작
                Ragdoll_Start();

                Die_State = DIE_STATE.RESPAWN_TIME;

                break;
            case DIE_STATE.RESPAWN_TIME:

                Destroy_Time += Time.deltaTime;
                if (Destroy_Time >= 5.0f)
                {
                    Destroy_Time = 0.0f;

                    Die_State = DIE_STATE.RESPAWN_INIT;
                }

                break;
            case DIE_STATE.RESPAWN_INIT:

                Game_Script.Bot_Script.Remove(User_ID);

                Destroy(Head_Info_Transform.gameObject);

                Destroy(gameObject);

                //봇 만들기
                Game_Script.Bot_Make_Count++;

                //Die_State = DIE_STATE.IDEL;

                break;
        }
    }

    //리그돌 연산 시작
    void Ragdoll_Start()
    {
        Char_Animator.enabled = false;

        float Force = 400.0f;

        GUN_TYPE Atk_Gun_Type = (GUN_TYPE)Die_Atk_GunType;

        if (Atk_Gun_Type == GUN_TYPE.PISTOL || Atk_Gun_Type == GUN_TYPE.REVOLVER || Atk_Gun_Type == GUN_TYPE.DOUBLE_HANDGUN)
        {
            Force = 400.0f;
        }
        else if (Atk_Gun_Type == GUN_TYPE.PUMP_SHOTGUN || Atk_Gun_Type == GUN_TYPE.AUTO_SHOTGUN || Atk_Gun_Type == GUN_TYPE.SNIPER)
        {
            Force = 1000.0f;
        }
        else if (Atk_Gun_Type == GUN_TYPE.RIFLE || Atk_Gun_Type == GUN_TYPE.SUB_MACHINGUN || Atk_Gun_Type == GUN_TYPE.HEAVY_MACHINGUN)
        {
            Force = 600.0f;
        }

        for (int i = 0; i < Ragdoll_Collider_Name.Length; i++)
        {
            Bot_Ragdoll_Rigidbody[Ragdoll_Collider_Name[i]].Rigid_Body.isKinematic = false;
            Bot_Ragdoll_Rigidbody[Ragdoll_Collider_Name[i]].GameObject_OJ.layer = Ragdoll_Die_Layer;
        }

        //자살일때 포스 안날린다
        if (Atk_Gun_Type == GUN_TYPE.SUICIDE) return;

        if (Atk_Gun_Type == GUN_TYPE.GRENADE || Atk_Gun_Type == GUN_TYPE.BOOMER_SKILL || Atk_Gun_Type == GUN_TYPE.LAUNCHER)//폭발로 인한 렉돌
        {
            float Explosion_Force = 25.0f;
            float Explosion_Radius = 3.0f;
            float Explosion_Up = 1.5f;

            for (int i = 0; i < Ragdoll_Collider_Name.Length; i++)
            {
                Bot_Ragdoll_Rigidbody[Ragdoll_Collider_Name[i]].Rigid_Body.AddExplosionForce(Explosion_Force, Die_Atk_Start_Vec, Explosion_Radius, Explosion_Up, ForceMode.Impulse);
            }
        }
        else//일반 총 렉돌
        {

            for (int i = 0; i < Ragdoll_Collider_Name.Length; i++)
            {
                Bot_Ragdoll_Rigidbody[Ragdoll_Collider_Name[i]].Rigid_Body.AddForceAtPosition((Die_Atk_Target_Vec - Die_Atk_Start_Vec).normalized * Force, Die_Atk_Target_Vec);
            }
        }
    }

    //==========================================================================================================================================================

    float ColorResult(int _Color)
    {
        return (float)(_Color / 255.0f);
    }

    //==========================================================================================================================================================

}
