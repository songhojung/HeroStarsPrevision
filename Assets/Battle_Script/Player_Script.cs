using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;

public enum CHAR_USER_KIND { PLAYER, NETWORK }

public enum CHAR_TEAM_SATAE { PLAYER_TEAM, ENEMY_TEAM }

public enum JUMP_STATE { IDEL, JUMP_UP, SUPER_JUMP_UP, JUMP_DOWN }

public enum SKILL_KIND { NONE, SOLDIER, JUMPER, VISION, ROLLER, GUARD, BOOMER, FLAMER, ROCKET, FLYING, RAGE, HIGHJUMP, INCREASE_HP, INCREASE_MAGAZINE, INCREASE_POWER, THROUGH_SHOT};

public enum RELOAD_STATE { IDEL, RELOAD_INIT, RELOAD, RELOAD_END }

public enum CHAR_SHOT_STATE { IDEL, ANI_START, SHOT_LAUNCH, SHOT_END, SHOT_SNIPER_END }

public enum GUN_TYPE { PISTOL, REVOLVER, DOUBLE_HANDGUN, PUMP_SHOTGUN, AUTO_SHOTGUN, RIFLE, SUB_MACHINGUN, HEAVY_MACHINGUN, SNIPER, GRENADE, LAUNCHER, ROLLER_SKILL, FLAMER_SKILL, SUICIDE, ROCKET_SKILL, THROUGH_SHOT_SKILL, BOOMER_SKILL }

public enum GUN_EQUIP_STATE { MAIN, SUB };

enum DOUBLE_SHOOTING_STATE { IDEL, FIRST_SHOT, SHOT_WAIT, SECOND_SHOT };

enum DIE_STATE { IDEL, ANI_START, RAGDOLL_START, RESPAWN_TIME, RESPAWN_INIT }

enum DISCONNECT_DIE_STATE { IDEL, DIE_ANI_INIT, DIE_ANI }

public class Player_Script : MonoBehaviour
{

    String[] Ragdoll_Collider_Name = {  "Bip001 Pelvis", "Bip001 Spine1",
                                        "Bip001 L Thigh", "Bip001 L Calf", "Bip001 R Thigh", "Bip001 R Calf", "Bip001 L UpperArm", "Bip001 L Forearm", "Bip001 R UpperArm", "Bip001 R Forearm",
                                        "Bip001 Head"};

    public class Out_Line_class
    {
        public Outline OJ;
    }
    public Dictionary<String, Out_Line_class> Out_Line_OJ = new Dictionary<String, Out_Line_class>();

    public class Ragdoll_Rigidbody_class
    {
        public GameObject GameObject_OJ;
        public Transform Transform_OJ;
        public Rigidbody Rigid_Body;
    }
    public Dictionary<String, Ragdoll_Rigidbody_class> Ragdoll_Rigidbody = new Dictionary<String, Ragdoll_Rigidbody_class>();

    public class Char_Model_class
    {
        public GameObject OJ;        
    }
    public Dictionary<int, Char_Model_class> Char_Model_OJ = new Dictionary<int, Char_Model_class>();

    public class Weapon_class
    {
        public GameObject OJ;
    }
    public Dictionary<int, Weapon_class> Weapon_OJ = new Dictionary<int, Weapon_class>();

    public class Costume_class
    {
        public GameObject OJ;
    }
    public Dictionary<int, Costume_class> Costume_OJ = new Dictionary<int, Costume_class>();

    public class Costume_Over_class
    {
        public GameObject OJ;
    }
    public Dictionary<String, Costume_Over_class> Costume_Over_OJ = new Dictionary<String, Costume_Over_class>();
    
    CharacterController Char_Controller;

    CameraControl_Script CameraControlScript;

    Network_Battle_Script Net_Script;

    GamePlay_UI_Script UI_Script;

    GamePlay_Script Game_Script;


    public GameObject[] Prefab_SkinnedMesh;//캐릭터 종류별 스킨메쉬 오브젝트
    public GameObject Prefab_Right_Weapon;//오른손 무기 오브젝트
    public GameObject Prefab_Left_Weapon;//왼손 무기 오브젝트
    public GameObject Prefab_Item_Head_Costume;//머리 코스튬 오브젝트
    public GameObject Prefab_Item_Body_Costume;//몸통 코스튬 오브젝트
    public GameObject Prefab_Over_Costume;//캐릭터와 코스튬이 겹치는 오브젝트
    public GameObject Prefab_barrier_red;//레드팀 보호막 오브젝트
    public GameObject Prefab_barrier_blue;//블루팀 보호막 오브젝트
    public GameObject Prefab_Auto_Targeting_OJ;//캐릭터 자동타켓 오브젝트
    public GameObject Prefab_HeadShot_Target_OJ;//헤드샷 체크 오브젝트
    
    //스킬관련 오브젝트
    public GameObject Prefab_Skill_Front_Defense;
    public GameObject Prefab_Skill_Effect_Rush;
    public GameObject Prefab_Skill_Hitter_FlameThrower_OJ;
    public GameObject Prefab_Skill_Hitter_FlameThrower_OJ_Local;
    public GameObject Prefab_Skill_Effect_Superjump;
    public GameObject Prefab_Skill_Effect_Flying;
    public GameObject Prefab_Skill_Effect_Rage;

    public GameObject Prefab_ARROW_3D;//나를 죽인 캐릭터 표시
    public GameObject Prefab_Revenge_OK_Icon;//복수 성공 아이콘


    int CHAR_OVER_OJ_MAX = 4;//캐릭터별 코슈튬과 오버되서 보이는 오브젝트 갯수

    Animator Char_Animator;
    AnimatorStateInfo Animator_State;
    bool Animator_Play_Check = false;
    int Animator_Play_Index = 0;
    bool Animator_Change_Check = false;

    LayerMask Ragdoll_Layer;
    LayerMask Ragdoll_Die_Layer;
    public LayerMask Char_Controller_Layer;
    //public LayerMask Auto_Targeting_Layer;
    public LayerMask RayCast_Layer;
    LayerMask HeadShot_Layer;

    Transform HeadShot_Target_OJ;

    CHAR_USER_KIND Char_User_Kind;
    
    public CHAR_TEAM_SATAE Char_Team_State;

    GameObject Auto_Targeting_OJ;

    Transform Char_Chest;
    Vector3 Char_Chest_Offset = new Vector3(0, -60, -100);

    Transform Player_Char_LookAt_Transform;
    Vector3 Network_Char_LookAt_Vector;

    bool Char_Use_Check = false;
    
    public String Object_Name = "";

    public uint User_ID = 0;
    public String User_NickName = "";
    public int Char_Index = 0;
    public byte Char_Level = 0;
    int Costume_Kind_1;
    int Costume_Kind_2;
    int Costume_Kind_3;
    public byte User_Team = 0;
    public ushort User_Clan_Mark;
    public string User_Clan_Name;
    public byte[] User_Country_Mark = new byte[2];
    public float MY_HP = 0.0f;
    float MY_MAX_HP = 0.0f;
    float MY_Move_Speed = 0.0f;
    int[] MY_MAX_ATK = new int[2];
    int[] MY_MIN_ATK = new int[2];
    float[] MY_Critical = new float[2];    
    public int[] Aim_Init = new int[2] ;//초기정확도 
    public int[] Aim_Ctrl = new int[2];//조준반동제어    
    float[] Atk_Speed = new float[2];//공격속도(분당/발수)        
    int[] Gun_Bullet_MAX = new int[2];//남은 총알
    public int[] Gun_Bullet = new int[2];//남은 총알

    bool SetBuff_Check = false;//셋트 장비 효과 체크
    SetBufKnd SetBuff_Kind;
    float SetBuff_Result = 0.0f;

    public int Grenade_MAX_ATK = 0;
    public int Grenade_MIN_ATK = 0;
    public float Grenade_ATK_Range = 0.0f;

    public int Boomer_MAX_ATK = 0;
    public int Boomer_MIN_ATK = 0;
    public float Boomer_ATK_Range = 0.0f;

    public int Launcher_MAX_ATK = 0;
    public int Launcher_MIN_ATK = 0;
    public float Launcher_ATK_Range = 0.0f;

    public int Rocket_MAX_ATK = 0;
    public int Rocket_MIN_ATK = 0;
    public float Rocket_ATK_Range = 0.0f;

    public int ThroughShot_MAX_ATK = 0;
    public int ThroughShot_MIN_ATK = 0;

    public float Main_Skill_Reload_Time = 0.0f;
    public float Main_Skill_Reload_MAX_Time = 0.0f;
    public float Main_Skill_Running_Time = 0.0f;
    public float Sub_Skill_Reload_Time = 0.0f;
    public float Sub_Skill_Reload_MAX_Time = 0.0f;
    public float Sub_Skill_Running_Time = 0.0f;
    public float Grenade_Reload_Time = 0.0f;
    public float Grenade_Reload_MAX_Time = 0.0f;
            
    public RELOAD_STATE[] Gun_Reload_State = new RELOAD_STATE[2];
    float[] Gun_Reload_Time = new float[2];
    float[] Gun_Reload_MAX_Time = new float[2];    
    
    public CHAR_SHOT_STATE Char_Shot_State;
    float Shot_Make_Time = 0.0f;
    bool Shot_Network_Check = false;
    bool ShotGun_Network_Check = false;    
    public Vector3 Shot_Aim_Pos = new Vector3();    
    RaycastHit Shot_RaycastHit;

    bool BulletMark_Check = false;
    public Vector3 BulletMark_Dir = new Vector3();

    public GUN_TYPE Gun_Type;
    public GUN_EQUIP_STATE Gun_Equip_State;
    public int[] Gun_Index = new int[2];
    byte[] Gun_Level = new byte[2];

    public byte MainGun_Scope_Magni = 0;
    public byte SubGun_Scope_Magni = 0;

    //샷건 한번에 나가는 총알의 갯수
    Vector3[] ShotGun_Aim_Pos = new Vector3[6];
    Vector3[] ShotGun_BulletMark_Dir = new Vector3[6];
    bool[] ShotGun_BulletMark_Check = new bool[6];

    //쌍권총 총알 나가는 체크
    DOUBLE_SHOOTING_STATE Double_Shot_State;

    //수류탄 쏜 카운트
    int Grenade_Count = 0;

    //네트웍 캐릭터 모델 바꿨는지 체크
    public bool Net_Char_Model_Change_Check = false;

    //네트웍 캐릭터 코스튬 바꿨는지 체크
    bool Net_Char_Costume_Change_Check = false;

    //네트웍 캐릭터 총 바꿨는지 체크
    public bool Net_Char_Gun_Change_Check = false;
           
    public JUMP_STATE Jump_State;        
    float Char_Gravity = 0.0f;
    float Sub_Skill_Jump = 0.0f;
    
    //캐릭터가 가지고 있는 스킬 종류
    public SKILL_KIND Char_Main_Skill_Kind;
    public SKILL_KIND Char_Sub_Skill_Kind;    

    //러쉬 스킬 오브젝트
    GameObject Roller_OJ;

    //보호막 스킬 오브젝트
    GameObject Guard_OJ;
    BoxCollider Guard_BoxCollider;

    //화영방사기 스킬 오브젝트
    GameObject Flamer_OJ;
    GameObject Flamer_Local_OJ;
    ParticleSystem Flamer_PA;
    ParticleSystem Flamer_Local_PA;

    //슈퍼점프 스킬 오브젝트
    GameObject SuperJump_OJ;
    bool SuperJump_Net_Check = false;

    //날아다니기 스킬 오브젝트
    GameObject Flying_Skill_OJ;

    //크리티컬 100% 스킬 오브젝트
    GameObject Rage_Skill_OJ;

    //날 죽인 캐릭터 표시 오브젝트
    GameObject Revenge_OJ;

    //복수 성공 아이콘 오브젝트
    GameObject Revenge_OK_Icon_OJ;
    public float Revenge_OK_Icon_Time = 0.0f;
    public float Revenge_OK_Icon_MAX_Time = 10.0f;

    //기본점프
    bool NormalJump_Net_Check = false;

    public float Vision_Time = 0.0f;
    public float Roller_Time = 0.0f;
    float Roller_Hit_Check_Time = 0.0f;
    public float Guard_Time = 0.0f;
    public float Flamer_Time = 0.0f;
    float Flamer_Hitting_Time = 0.0f;
    public float Boomer_Start_Time = 0.0f;
    public float Flying_Start_Time = 0.0f;
    public float ThroughShot_Start_Time = 0.0f;
    int Flying_UD_State = 0;
    public float Rage_Time = 0.0f;

    DIE_STATE Die_State;
    Vector3 Die_Atk_Start_Vec = new Vector3();
    Vector3 Die_Atk_Target_Vec = new Vector3();
    byte Die_Atk_GunType = 0;
    float Respawn_HP = 0.0f;
    byte Respawn_Pos_Index = 0;
    public uint Player_Killer_UserID = 0;
    float Camera_Dead_Move_Speed = 0.0f;
    public uint Revenge_UserID = 0;

    bool Char_Hit_Effect_Check = false;
    GUN_TYPE Char_Hit_Effect_GunType;
    Vector3 Char_Hit_Effect_Vec = new Vector3();

    GameObject[] Barrier_OJ = new GameObject[2];
    public bool Barrier_Check = false;    
    float Barrier_Time = 0.0f;
    float Barrier_Net_Send_Time = 0.0f;
    

    Transform Head_Info_Transform;
    Text Head_Info_Text;
    String Head_OJ_Name;

    public bool Damage_UI_Check = false;

    DISCONNECT_DIE_STATE Disconnect_Die_State;

    AudioSource Audio_Source;

    void Awake()
    {
        //----------------------------------------------------------------------------------------------------------------------------------
        
        Transform[] GetTransforms = transform.GetComponentsInChildren<Transform>();

        Transform[] Prefab_GetTransforms;

        //----------------------------------------------------------------------------------------------------------------------------------


        //아웃라인 효과 오브젝트 가져오기
        foreach (Transform child in GetTransforms)
        {
            if (child.GetComponent<Outline>() != null)
            {
                if (Out_Line_OJ.ContainsKey(child.name) == false)
                {
                    Out_Line_OJ.Add(child.name, new Out_Line_class());
                    Out_Line_OJ[child.name].OJ = child.GetComponent<Outline>();
                    Out_Line_OJ[child.name].OJ.enabled = false;
                }
            }
        }

        //----------------------------------------------------------------------------------------------------------------------------------

        //랙돌 셋팅
        Ragdoll_Layer = LayerMask.NameToLayer("Ragdoll");
        Ragdoll_Die_Layer = LayerMask.NameToLayer("Ragdoll_Die");

        Ragdoll_Rigidbody.Clear();
        
        foreach (Transform child in GetTransforms)
        {
            for (int i = 0; i < Ragdoll_Collider_Name.Length; i++)
            {
                if (child.name.Equals(Ragdoll_Collider_Name[i]))
                {
                    if (Ragdoll_Rigidbody.ContainsKey(Ragdoll_Collider_Name[i]) == false)
                    {
                        Ragdoll_Rigidbody.Add(Ragdoll_Collider_Name[i], new Ragdoll_Rigidbody_class());

                        Ragdoll_Rigidbody[Ragdoll_Collider_Name[i]].GameObject_OJ = child.gameObject;
                        Ragdoll_Rigidbody[Ragdoll_Collider_Name[i]].Transform_OJ = child;
                        Ragdoll_Rigidbody[Ragdoll_Collider_Name[i]].Rigid_Body = child.GetComponent<Rigidbody>();
                        //Ragdoll_Rigidbody[Ragdoll_Collider_Name[i]].Rigid_Body.collisionDetectionMode = CollisionDetectionMode.Continuous;                        
                        Ragdoll_Rigidbody[Ragdoll_Collider_Name[i]].Rigid_Body.isKinematic = true;
                        Ragdoll_Rigidbody[Ragdoll_Collider_Name[i]].GameObject_OJ.layer = Ragdoll_Layer;                        
                    }
                    break;
                }
            }                
        }

        //----------------------------------------------------------------------------------------------------------------------------------

        ////캐릭터 스킨메쉬 가져오기
        //foreach (Transform child in GetTransforms)
        //{
        //    for (int i = 0; i < CHAR_COUNT_MAX; i++)
        //    {
        //        if (child.name.Equals("ch_" + (10001 + i)))
        //        {
        //            Char_Model_OJ.Add((10001 + i), new Char_Model_class());

        //            Char_Model_OJ[(10001 + i)].OJ = child.gameObject;
        //            Char_Model_OJ[(10001 + i)].OJ.SetActive(false);
        //            break;
        //        }
        //    }
        //}
        
        //캐릭터 스킨메쉬 가져오기
        for (int i = 0; i < Prefab_SkinnedMesh.Length; i++)
        {
            Char_Model_OJ.Add((10001 + i), new Char_Model_class());

            Char_Model_OJ[(10001 + i)].OJ = Prefab_SkinnedMesh[i];
            Char_Model_OJ[(10001 + i)].OJ.SetActive(false);
        }

        //----------------------------------------------------------------------------------------------------------------------------------

        ////캐릭터에 붙어있는 총 무기 오브젝트 가져오기
        //foreach (Transform child in GetTransforms)
        //{
        //    for (int i = 0; i < Link_Script.i.W_Index.Length; i++)
        //    {
        //        if (child.name.Equals("Weapon_" + Link_Script.i.W_Index[i]))
        //        {
        //            Weapon_OJ.Add(Link_Script.i.W_Index[i], new Weapon_class());

        //            Weapon_OJ[Link_Script.i.W_Index[i]].OJ = child.gameObject;
        //            Weapon_OJ[Link_Script.i.W_Index[i]].OJ.SetActive(false);
        //            break;
        //        }
        //    }

        //    for (int i = 0; i < Link_Script.i.W_Left_Index.Length; i++)
        //    {
        //        if (child.name.Equals("Weapon_" + Link_Script.i.W_Left_Index[i] + "_1"))
        //        {
        //            Weapon_OJ.Add(Link_Script.i.W_Left_Index[i] + 100000, new Weapon_class());

        //            Weapon_OJ[Link_Script.i.W_Left_Index[i] + 100000].OJ = child.gameObject;
        //            Weapon_OJ[Link_Script.i.W_Left_Index[i] + 100000].OJ.SetActive(false);
        //            break;
        //        }
        //    }
        //}

        
        //캐릭터 오른손에 붙어있는 총 무기 오브젝트 가져오기
        Prefab_GetTransforms = Prefab_Right_Weapon.transform.GetComponentsInChildren<Transform>();
        foreach (Transform Prefab_child in Prefab_GetTransforms)
        {
            for (int i = 0; i < Link_Script.i.W_Index.Length; i++)
            {
                if (Prefab_child.name.Equals("Weapon_" + Link_Script.i.W_Index[i]))
                {
                    Weapon_OJ.Add(Link_Script.i.W_Index[i], new Weapon_class());

                    Weapon_OJ[Link_Script.i.W_Index[i]].OJ = Prefab_child.gameObject;
                    Weapon_OJ[Link_Script.i.W_Index[i]].OJ.SetActive(false);
                    break;
                }
            }
        }
        
        //캐릭터 왼손에 붙어있는 총 무기 오브젝트 가져오기
        Prefab_GetTransforms = Prefab_Left_Weapon.transform.GetComponentsInChildren<Transform>();
        foreach (Transform Prefab_child in Prefab_GetTransforms)
        {
            for (int i = 0; i < Link_Script.i.W_Left_Index.Length; i++)
            {
                if (Prefab_child.name.Equals("Weapon_" + Link_Script.i.W_Left_Index[i] + "_1"))
                {
                    Weapon_OJ.Add(Link_Script.i.W_Left_Index[i] + 100000, new Weapon_class());

                    Weapon_OJ[Link_Script.i.W_Left_Index[i] + 100000].OJ = Prefab_child.gameObject;
                    Weapon_OJ[Link_Script.i.W_Left_Index[i] + 100000].OJ.SetActive(false);
                    break;
                }
            }
        }
        
        //----------------------------------------------------------------------------------------------------------------------------------

        ////캐릭터에 붙어있는 총 코스튬 오브젝트 가져오기
        //foreach (Transform child in GetTransforms)
        //{
        //    for (int i = 0; i < Link_Script.i.I_Index.Length; i++)
        //    {
        //        if (child.name.Equals("Item_" + Link_Script.i.I_Index[i]))
        //        {
        //            Costume_OJ.Add(Link_Script.i.I_Index[i], new Costume_class());

        //            Costume_OJ[Link_Script.i.I_Index[i]].OJ = child.gameObject;
        //            Costume_OJ[Link_Script.i.I_Index[i]].OJ.SetActive(false);
        //            break;
        //        }
        //    }
        //}

        //캐릭터 머리에 붙어있는 총 코스튬 오브젝트 가져오기
        Prefab_GetTransforms = Prefab_Item_Head_Costume.transform.GetComponentsInChildren<Transform>();
        foreach (Transform Prefab_child in Prefab_GetTransforms)
        {
            for (int i = 0; i < Link_Script.i.I_Index.Length; i++)
            {
                if (Prefab_child.name.Equals("Item_" + Link_Script.i.I_Index[i]))
                {
                    Costume_OJ.Add(Link_Script.i.I_Index[i], new Costume_class());

                    Costume_OJ[Link_Script.i.I_Index[i]].OJ = Prefab_child.gameObject;
                    Costume_OJ[Link_Script.i.I_Index[i]].OJ.SetActive(false);
                    break;
                }
            }
        }

        //캐릭터 몸통에 붙어있는 총 코스튬 오브젝트 가져오기
        Prefab_GetTransforms = Prefab_Item_Body_Costume.transform.GetComponentsInChildren<Transform>();
        foreach (Transform Prefab_child in Prefab_GetTransforms)
        {
            for (int i = 0; i < Link_Script.i.I_Index.Length; i++)
            {
                if (Prefab_child.name.Equals("Item_" + Link_Script.i.I_Index[i]))
                {
                    Costume_OJ.Add(Link_Script.i.I_Index[i], new Costume_class());

                    Costume_OJ[Link_Script.i.I_Index[i]].OJ = Prefab_child.gameObject;
                    Costume_OJ[Link_Script.i.I_Index[i]].OJ.SetActive(false);
                    break;
                }
            }
        }
        
        //----------------------------------------------------------------------------------------------------------------------------------

        ////코슈튬과 겹쳐서 오버되는 오브젝트 가져오기
        //foreach (Transform child in GetTransforms)
        //{
        //    for (int i = 0; i < CHAR_COUNT_MAX; i++)
        //    {
        //        int Temp_Char_index = 10001 + i;

        //        for (int k = 0; k < CHAR_OVER_OJ_MAX; k++)
        //        {
        //            String Temp_Over_OJ = "ch_" + Temp_Char_index + "_OverOJ_" + k;

        //            if (child.name.Equals(Temp_Over_OJ))
        //            {
        //                Costume_Over_OJ.Add(Temp_Over_OJ, new Costume_Over_class());
        //                Costume_Over_OJ[Temp_Over_OJ].OJ = child.gameObject;
        //                Costume_Over_OJ[Temp_Over_OJ].OJ.SetActive(false);
        //            }
        //        }
        //    }
        //}

        //코슈튬과 겹쳐서 오버되는 오브젝트 가져오기
        Prefab_GetTransforms = Prefab_Over_Costume.transform.GetComponentsInChildren<Transform>();
        foreach (Transform Prefab_child in Prefab_GetTransforms)
        {
            for (int i = 0; i < Prefab_SkinnedMesh.Length; i++)
            {
                int Temp_Char_index = 10001 + i;

                for (int k = 0; k < CHAR_OVER_OJ_MAX; k++)
                {
                    String Temp_Over_OJ = "ch_" + Temp_Char_index + "_OverOJ_" + k;

                    if (Prefab_child.name.Equals(Temp_Over_OJ))
                    {
                        Costume_Over_OJ.Add(Temp_Over_OJ, new Costume_Over_class());
                        Costume_Over_OJ[Temp_Over_OJ].OJ = Prefab_child.gameObject;
                        Costume_Over_OJ[Temp_Over_OJ].OJ.SetActive(false);
                    }
                }
            }
        }



        //----------------------------------------------------------------------------------------------------------------------------------

        ////보호막 셋팅
        //foreach (Transform child in GetTransforms)
        //{
        //    if (child.name.Equals("barrier_red"))
        //    {
        //        Barrier_OJ[0] = child.gameObject;
        //        Barrier_OJ[0].SetActive(false);
        //    }
        //    else if (child.name.Equals("barrier_blue"))
        //    {
        //        Barrier_OJ[1] = child.gameObject;
        //        Barrier_OJ[1].SetActive(false);
        //    }
        //    else if (child.name.Equals("Auto_Targeting_OJ"))
        //    {
        //        Auto_Targeting_OJ = child.gameObject;
        //        Auto_Targeting_OJ.SetActive(false);
        //    }           
        //}


        //보호막 셋팅
        Barrier_OJ[0] = Prefab_barrier_red;
        Barrier_OJ[0].SetActive(false);
        Barrier_OJ[1] = Prefab_barrier_blue;
        Barrier_OJ[1].SetActive(false);

        //자동 타켓팅 오브젝트
        Auto_Targeting_OJ = Prefab_Auto_Targeting_OJ;
        Auto_Targeting_OJ.SetActive(false);

        //----------------------------------------------------------------------------------------------------------------------------------

        ////스킬 오브젝트 가져오기
        //foreach (Transform child in GetTransforms)
        //{
        //    if (child.name.Equals("Front_Defense"))
        //    {
        //        Guard_OJ = child.gameObject;
        //        Guard_OJ.tag = "WALL";
        //        Guard_BoxCollider = Guard_OJ.GetComponent<BoxCollider>();
        //        Guard_BoxCollider.enabled = false;
        //        Guard_OJ.SetActive(false);
        //    }
        //    else if (child.name.Equals("Effect_Rush"))
        //    {
        //        Roller_OJ = child.gameObject;
        //        Roller_OJ.SetActive(false);
        //    }
        //    else if (child.name.Equals("Hitter_FlameThrower_OJ"))
        //    {
        //        Flamer_OJ = child.gameObject;
        //        Flamer_PA = Flamer_OJ.GetComponent<ParticleSystem>();
        //        Flamer_OJ.SetActive(false);
        //    }
        //    else if (child.name.Equals("Hitter_FlameThrower_OJ_Local"))
        //    {
        //        Flamer_Local_OJ = child.gameObject;
        //        Flamer_Local_PA = Flamer_Local_OJ.GetComponent<ParticleSystem>();
        //        Flamer_Local_OJ.SetActive(false);

        //    }
        //    else if (child.name.Equals("Effect_Superjump"))
        //    {
        //        SuperJump_OJ = child.gameObject;
        //        SuperJump_OJ.SetActive(false);
        //    }
        //    else if (child.name.Equals("Effect_Flying"))
        //    {
        //        Flying_Skill_OJ = child.gameObject;
        //        Flying_Skill_OJ.SetActive(false);
        //    }
        //    else if (child.name.Equals("Effect_Rage"))
        //    {
        //        Rage_Skill_OJ = child.gameObject;
        //        Rage_Skill_OJ.SetActive(false);
        //    }
        //}



        //스킬 오브젝트 가져오기        
        Guard_OJ = Prefab_Skill_Front_Defense;
        Guard_OJ.tag = "WALL";
        Guard_BoxCollider = Guard_OJ.GetComponent<BoxCollider>();
        Guard_BoxCollider.enabled = false;
        Guard_OJ.SetActive(false);
        
        Roller_OJ = Prefab_Skill_Effect_Rush;
        Roller_OJ.SetActive(false);
        
        Flamer_OJ = Prefab_Skill_Hitter_FlameThrower_OJ;
        Flamer_PA = Flamer_OJ.GetComponent<ParticleSystem>();
        Flamer_OJ.SetActive(false);
        
        Flamer_Local_OJ = Prefab_Skill_Hitter_FlameThrower_OJ_Local;
        Flamer_Local_PA = Flamer_Local_OJ.GetComponent<ParticleSystem>();
        Flamer_Local_OJ.SetActive(false);
        
        SuperJump_OJ = Prefab_Skill_Effect_Superjump;
        SuperJump_OJ.SetActive(false);
        
        Flying_Skill_OJ = Prefab_Skill_Effect_Flying;
        Flying_Skill_OJ.SetActive(false);
        
        Rage_Skill_OJ = Prefab_Skill_Effect_Rage;
        Rage_Skill_OJ.SetActive(false);

        //----------------------------------------------------------------------------------------------------------------------------------

        ////날 죽인 캐릭터 표시 오브젝트
        //foreach (Transform child in GetTransforms)
        //{
        //    if (child.name.Equals("ARROW_3D"))
        //    {
        //        Revenge_OJ = child.gameObject;
        //        Revenge_OJ.SetActive(false);
        //    }
        //    else if (child.name.Equals("Revenge_OK_Icon"))
        //    {
        //        Revenge_OK_Icon_OJ = child.gameObject;
        //        Revenge_OK_Icon_OJ.SetActive(false);
        //    }
        //}
                
        //나를 죽인 캐릭터 표시
        Revenge_OJ = Prefab_ARROW_3D;
        Revenge_OJ.SetActive(false);

        //복수 성공 아이콘
        Revenge_OK_Icon_OJ = Prefab_Revenge_OK_Icon;
        Revenge_OK_Icon_OJ.SetActive(false);

        //----------------------------------------------------------------------------------------------------------------------------------

        //foreach (Transform child in GetTransforms)
        //{
        //    if (child.name.Equals("HeadShot_Target_OJ"))
        //    {
        //        HeadShot_Target_OJ = child;
        //        break;
        //    }
        //}

        HeadShot_Target_OJ = Prefab_HeadShot_Target_OJ.GetComponent<Transform>();        
        
        //----------------------------------------------------------------------------------------------------------------------------------
        
        Char_Controller = GetComponent<CharacterController>();

        Char_Animator = transform.GetComponent<Animator>();

        Char_Chest = Char_Animator.GetBoneTransform(HumanBodyBones.Chest);

        Char_Controller_Layer = LayerMask.NameToLayer("Char_Controller");

        //Auto_Targeting_Layer = 1 << Char_Controller_Layer;//"Char_Controler" 레이어만 체크

        //RayCast_Layer = ~(1 << Char_Controller_Layer | 1 << Ragdoll_Die_Layer);//"Char_Controller"레이어와 "Ragdoll_Die"레이어만 체크하지 말아라
        //RayCast_Layer = ~(1 << LayerMask.NameToLayer("Char_Controler"));//"Char_Controler" 레이어를 제외한 나머지 레이어 체크
        RayCast_Layer = ~(1 << LayerMask.NameToLayer("Ragdoll_Die"));//"Ragdoll_Die" 레이어를 제외한 나머지 레이어 체크

        HeadShot_Layer = 1 << LayerMask.NameToLayer("HeadShot_OJ");//"HeadShot_OJ" 레이어만 체크
                        
        //----------------------------------------------------------------------------------------------------------------------------------

        Char_Use_Check = false;

        //수류탄 쏜 카운트
        Grenade_Count = 0;

        Disconnect_Die_State = DISCONNECT_DIE_STATE.IDEL;

        Audio_Source = transform.GetComponent<AudioSource>();
    }

    //================================================================================================================================================================

    void Update()
    {
        if (!Link_Script.i.IsConnected()) return;

        if (!Char_Use_Check || Game_Script.GameOver_Check) return;

        if (Disconnect_Die_State != DISCONNECT_DIE_STATE.IDEL)
        {
            Network_Disconnect_Operation();
            return;
        }

        //애니메이션 상태값 셋팅
        Animator_State_Init();        

        //캐릭터 중력 연산
        Char_Gravity_Oeration();

        //무적 상태 체크 연산
        Barrier_Operation();

        //스킬, 수류탄 버튼 리로드 연산
        Skill_Grenade_Reload_Operation();

        //지속형 스킬 시간 체크
        Skill_Time_Operation();

        //네트웍 데이터 받아서 움직임 처리
        Network_Move_Operation();

        //총 쏘는 연산
        Shot_Operation();

        //리로드 애니메이션 끝나는 체크
        Reload_End_Check_Operation();

        //캐릭터에 총 맞은 임펙트
        Bullethit_Operation();

        //발사 종류 스킬 애니 끝났을때 무기 이미지 그려주는 체크
        Skill_Shot_End_Check_Operation();

        //캐릭터 죽는 연산
        Char_Die_Operation();

        //네트웍 캐릭터 모델, 무기 바꾸기
        Net_Char_Model_Gun_Change();

        //스킬 데이터 바로 보내기
        Char_Skill_Data_Now_Send();

        //플레이어가 준 데미지 그리기
        ATK_UI_Damage_Operation();

        //복수 성공 아이콘 띄우기
        Revenge_OK_Operation();
                                
        //애니메이션 플레이 시키기
        Animator_Play_Operation();
    }

    void LateUpdate()
    {
        if (!Char_Use_Check || Game_Script.GameOver_Check) return;

        if (Disconnect_Die_State != DISCONNECT_DIE_STATE.IDEL) return;
                
        if (Die_State != DIE_STATE.IDEL)
        {
            //플레이어 캐릭터 죽었을때 카메라 움직임 연산
            Camera_Dead_Move_Operation();

            return;
        }

        //캐릭터 상반신 쳐다보기 연산
        Char_Chest_LookAt_Operation();

        //카메라 움직인 관련 연산
        CameraMove_Operation();

        //캐릭터 머리위에 정보 텍스트 연산
        Head_Info_Text_Operation();
    }

    //================================================================================================================================================================

    void Script_Init()
    {
        if(CameraControlScript == null) CameraControlScript = GameObject.Find("Camera_Set_Object").GetComponent<CameraControl_Script>();

        if (Net_Script == null) Net_Script = GameObject.Find("Network_Script").GetComponent<Network_Battle_Script>();

        if (Game_Script == null) Game_Script = GameObject.Find("Script_Object").GetComponent<GamePlay_Script>();

        if (UI_Script == null) UI_Script = GameObject.Find("GamePlay_UI_Set").GetComponent<GamePlay_UI_Script>();
    }

    //캐릭터 최초 생성 셋팅
    public void Char_Init(CHAR_USER_KIND _Char_User_Kind, CHAR_TEAM_SATAE _Char_Team_State)
    {
        Script_Init();

        Char_Use_Check = true;

        Char_User_Kind = _Char_User_Kind;
        Char_Team_State = _Char_Team_State;

        if (Char_User_Kind == CHAR_USER_KIND.PLAYER)
        {
            User_ID = Link_Script.i.User_ID;
            User_NickName = Link_Script.i.User_NickName;            
            User_Team = Link_Script.i.User_Team;

            User_Clan_Mark = Link_Script.i.User_Clan_Mark;
            User_Clan_Name = Link_Script.i.User_Clan_Name;
            User_Country_Mark[0] = Link_Script.i.User_Country_Mark[0];
            User_Country_Mark[1] = Link_Script.i.User_Country_Mark[1];

            Char_Index = Link_Script.i.Char_Index;
            Char_Level = Link_Script.i.Char_Level;

            Costume_Kind_1 = Link_Script.i.Costume_Kind_1;
            Costume_Kind_2 = Link_Script.i.Costume_Kind_2;
            Costume_Kind_3 = Link_Script.i.Costume_Kind_3;

            //캐릭터 이미지 바꾸기
            Char_Model_Change();

            //캐릭터 코스튬 바꾸기
            Char_Costume_Change();
                                                
            //최초 캐릭터 생성일때만 셋팅되고, 이후 리스폰 될때마다 서버에서 받아서 HP 값 셋팅해준다.
            MY_HP = Link_Script.i.Char_Now_HP;
            MY_MAX_HP = MY_HP;
            MY_Move_Speed = Link_Script.i.Char_Move_Speed;            
            //MY_Move_Speed = 10.0f;
                                    
            //주,보조 무기 인덱스 셋팅
            Gun_Index[0] = Link_Script.i.Main_Gun_Index;
            Gun_Index[1] = Link_Script.i.Sub_Gun_Index;
            Gun_Level[0] = Link_Script.i.Main_Gun_Level;
            Gun_Level[1] = Link_Script.i.Sub_Gun_Level;

            //주,보조 무기 타입 셋팅
            Link_Script.i.Main_Gun_Type = (byte)SendManager.Instance.Get_WeaponType(Link_Script.i.Main_Gun_Index);
            Link_Script.i.Sub_Gun_Type = (byte)SendManager.Instance.Get_WeaponType(Link_Script.i.Sub_Gun_Index);
            Gun_Type = (GUN_TYPE)Link_Script.i.Main_Gun_Type;
                        
            //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            //주무기 데이터 셋팅
            User_weapon Main_Gun_Data = Link_Script.i.Get_Char_ALL_Gun_Data[(uint)Link_Script.i.Main_Gun_Index];

            Aim_Init[(int)GUN_EQUIP_STATE.MAIN] = SendManager.Instance.Get_TableData().Infos_weapons[Main_Gun_Data.WpnIdx].AimInit;//초기정확도
            Aim_Ctrl[(int)GUN_EQUIP_STATE.MAIN] = SendManager.Instance.Get_TableData().Infos_weapons[Main_Gun_Data.WpnIdx].AimCtrl;//조준반동제어
            Atk_Speed[(int)GUN_EQUIP_STATE.MAIN] = 60.0f / (float)SendManager.Instance.Get_TableData().Infos_weapons[Main_Gun_Data.WpnIdx].AtkSpeed;//공격속도(분당/발수)
            Gun_Bullet_MAX[(int)GUN_EQUIP_STATE.MAIN] = SendManager.Instance.Get_TableData().Infos_weapons[Main_Gun_Data.WpnIdx].Magazine;//남은 총알
            Gun_Bullet[(int)GUN_EQUIP_STATE.MAIN] = Gun_Bullet_MAX[(int)GUN_EQUIP_STATE.MAIN];//남은 총알

            MY_MAX_ATK[(int)GUN_EQUIP_STATE.MAIN] = SendManager.Instance.Get_ReinfWeapon(Main_Gun_Data.WpnIdx, Main_Gun_Data.RefLv).AtkMax;
            MY_MIN_ATK[(int)GUN_EQUIP_STATE.MAIN] = SendManager.Instance.Get_ReinfWeapon(Main_Gun_Data.WpnIdx, Main_Gun_Data.RefLv).AtkMin;
            MY_Critical[(int)GUN_EQUIP_STATE.MAIN] = SendManager.Instance.Get_ReinfWeapon(Main_Gun_Data.WpnIdx, Main_Gun_Data.RefLv).Critical;

            Gun_Reload_State[(int)GUN_EQUIP_STATE.MAIN] = RELOAD_STATE.IDEL;
            Gun_Reload_Time[(int)GUN_EQUIP_STATE.MAIN] = 0.0f;
            Gun_Reload_MAX_Time[(int)GUN_EQUIP_STATE.MAIN] = SendManager.Instance.Get_TableData().Infos_weapons[Main_Gun_Data.WpnIdx].GunReload * 0.01f;

            MainGun_Scope_Magni = SendManager.Instance.Get_TableData().Infos_weapons[Main_Gun_Data.WpnIdx].ZoomScale;

            //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            if (Link_Script.i.Get_Char_ALL_Gun_Data.ContainsKey((uint)Link_Script.i.Sub_Gun_Index))
            {
                //보조무기 데이터 셋팅
                User_weapon Sub_Gun_Data = Link_Script.i.Get_Char_ALL_Gun_Data[(uint)Link_Script.i.Sub_Gun_Index];

                Aim_Init[(int)GUN_EQUIP_STATE.SUB] = SendManager.Instance.Get_TableData().Infos_weapons[Sub_Gun_Data.WpnIdx].AimInit;//초기정확도
                Aim_Ctrl[(int)GUN_EQUIP_STATE.SUB] = SendManager.Instance.Get_TableData().Infos_weapons[Sub_Gun_Data.WpnIdx].AimCtrl;//조준반동제어
                Atk_Speed[(int)GUN_EQUIP_STATE.SUB] = 60.0f / (float)SendManager.Instance.Get_TableData().Infos_weapons[Sub_Gun_Data.WpnIdx].AtkSpeed;//공격속도(분당/발수)
                Gun_Bullet_MAX[(int)GUN_EQUIP_STATE.SUB] = SendManager.Instance.Get_TableData().Infos_weapons[Sub_Gun_Data.WpnIdx].Magazine;//남은 총알
                Gun_Bullet[(int)GUN_EQUIP_STATE.SUB] = Gun_Bullet_MAX[(int)GUN_EQUIP_STATE.SUB];//남은 총알

                MY_MAX_ATK[(int)GUN_EQUIP_STATE.SUB] = SendManager.Instance.Get_ReinfWeapon(Sub_Gun_Data.WpnIdx, Sub_Gun_Data.RefLv).AtkMax;
                MY_MIN_ATK[(int)GUN_EQUIP_STATE.SUB] = SendManager.Instance.Get_ReinfWeapon(Sub_Gun_Data.WpnIdx, Sub_Gun_Data.RefLv).AtkMin;
                MY_Critical[(int)GUN_EQUIP_STATE.SUB] = SendManager.Instance.Get_ReinfWeapon(Sub_Gun_Data.WpnIdx, Sub_Gun_Data.RefLv).Critical;

                Gun_Reload_State[(int)GUN_EQUIP_STATE.SUB] = RELOAD_STATE.IDEL;
                Gun_Reload_Time[(int)GUN_EQUIP_STATE.SUB] = 0.0f;
                Gun_Reload_MAX_Time[(int)GUN_EQUIP_STATE.SUB] = SendManager.Instance.Get_TableData().Infos_weapons[Sub_Gun_Data.WpnIdx].GunReload * 0.01f;

                SubGun_Scope_Magni = SendManager.Instance.Get_TableData().Infos_weapons[Sub_Gun_Data.WpnIdx].ZoomScale;
            }            

            //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            //주무기 사용으로 셋팅
            Gun_Equip_State = GUN_EQUIP_STATE.MAIN;
            Gun_Change(Gun_Equip_State);//총 바꾸기

            //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            //메인 스킬 인덱스
            byte Main_Skill_Index = SendManager.Instance.Get_TableData().Infos_units[(uint)Char_Index].SkillKind;

            //메인 스킬 종류
            Char_Main_Skill_Kind = (SKILL_KIND)SendManager.Instance.Get_TableData().Infos_Skills[Main_Skill_Index].SkillKnd;

            //메인 스킬 이미지로 셋팅
            UI_Script.Main_Skill_Icon_Init((int)Main_Skill_Index);

            Main_Skill_Reload_Time = 0.0f;
            Main_Skill_Reload_MAX_Time = SendManager.Instance.Get_TableData().Infos_Skills[Main_Skill_Index].CoolTime;
            Main_Skill_Running_Time = SendManager.Instance.Get_TableData().Infos_Skills[Main_Skill_Index].RunningTime;

            //서브 스킬 버튼 없애기
            UI_Script.Sub_Skill_Button_OJ.SetActive(false);
            Sub_Skill_Jump = 0.0f;
            Sub_Skill_Reload_Time = 0.0f;
            Sub_Skill_Reload_MAX_Time = 0.0f;                       

            byte Char_Sub_Skill_Index = Link_Script.i.Get_ALL_Char_Data[(uint)Char_Index].SubSkill;
                                    
            if (Char_Sub_Skill_Index != 0)
            {
                //서브 스킬 종류
                Char_Sub_Skill_Kind = (SKILL_KIND)SendManager.Instance.Get_TableData().Infos_Skills[Char_Sub_Skill_Index].SkillKnd;
                                
                //서브 스킬 수치
                ushort Sub_Skill_Value = SendManager.Instance.Get_TableData().Infos_Skills[Char_Sub_Skill_Index].SkillVal;

                Sub_Skill_Reload_MAX_Time = SendManager.Instance.Get_TableData().Infos_Skills[Char_Sub_Skill_Index].CoolTime;
                Sub_Skill_Running_Time = SendManager.Instance.Get_TableData().Infos_Skills[Char_Sub_Skill_Index].RunningTime;
                                
                switch (Char_Sub_Skill_Kind)
                {
                    case SKILL_KIND.SOLDIER:
                    case SKILL_KIND.JUMPER:
                    case SKILL_KIND.VISION:
                    case SKILL_KIND.ROLLER:
                    case SKILL_KIND.GUARD:
                    case SKILL_KIND.BOOMER:
                    case SKILL_KIND.FLAMER:
                    case SKILL_KIND.ROCKET:
                    case SKILL_KIND.FLYING:
                    case SKILL_KIND.RAGE:
                    case SKILL_KIND.THROUGH_SHOT:

                        //서브 스킬 버튼 생성
                        UI_Script.Sub_Skill_Button_OJ.SetActive(true);

                        //서브 스킬 이미지로 셋팅
                        UI_Script.Sub_Skill_Icon_Init((int)Char_Sub_Skill_Index);

                        break;
                    case SKILL_KIND.HIGHJUMP://일반 점프 조금도 높게 뛰기

                        Sub_Skill_Jump = 0.1f;

                        break;
                    case SKILL_KIND.INCREASE_HP://전체 HP 늘리기

                        //최초 캐릭터 생성일때만 셋팅되고, 이후 리스폰 될때마다 서버에서 받아서 HP 값 셋팅해준다.
                        MY_HP += Sub_Skill_Value;
                        MY_MAX_HP = MY_HP;

                        break;
                    case SKILL_KIND.INCREASE_MAGAZINE://전체 탄환수 늘리기

                        for (int i = 0; i < 2; i++)
                        {
                            float Temp = Gun_Bullet_MAX[i] * (Sub_Skill_Value * 0.01f);
                            if (Temp - (int)Temp >= 0.5f) Temp += 1;//반올림

                            Gun_Bullet_MAX[i] += (int)Temp;
                            Gun_Bullet[i] = Gun_Bullet_MAX[i];//남은 총알
                        }

                        break;
                    case SKILL_KIND.INCREASE_POWER://전체 파워 늘리기

                        for (int i = 0; i < 2; i++)
                        {
                            MY_MAX_ATK[i] += Sub_Skill_Value;
                            MY_MIN_ATK[i] += Sub_Skill_Value;
                        }

                        break;
                }
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            //BufKnd : 1: 공격력 200% 증가
            //BufKnd : 2: 체력 30% 증가(서버 체크)
            //BufKnd : 3: 공격형 스킬 위력 200% 증가 (*무기 위력은 증가 안함)
            //BufKnd : 4  데미지 20% 감소로 가겠습니다 (서버 체크)
                        
            //셋트 장비 효과 체크
            SetBuff_Check = false;
            SetBuff_Kind = SetBufKnd.ATK_UP;
            SetBuff_Result = 0.0f;

            foreach (var _Infos_SetBuffs in SendManager.Instance.Get_TableData().Infos_SetBuffs)
            {
                if (Gun_Index[0] == _Infos_SetBuffs.Value.MainWpnIdx &&
                    Gun_Index[1] == _Infos_SetBuffs.Value.SubWpnIdx &&
                    Costume_Kind_1 == _Infos_SetBuffs.Value.DecoIdx1 &&
                    Costume_Kind_2 == _Infos_SetBuffs.Value.DecoIdx2 &&
                    Costume_Kind_3 == _Infos_SetBuffs.Value.DecoIdx3)
                {

                    SetBuff_Check = true;//세트 버프 체크
                    SetBuff_Kind = _Infos_SetBuffs.Value.BufKind;//세트 버프 종류
                    SetBuff_Result = _Infos_SetBuffs.Value.BufVal;//세트 버프 적용될 수치

                    break;
                }
            }

            //최초 캐릭터 생성시 화면에 보여지는 HP는 클라이언트가 처리해준다. 이후엔 리스폰값으로 받은 데이터로 처리
            if (SetBuff_Check && SetBuff_Kind == SetBufKnd.HP_UP)
            {
                MY_HP = (int)(MY_HP * SetBuff_Result);
                MY_MAX_HP = MY_HP;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            
            //코스튬 능력치 반환
            float Costume_Power = ( SendManager.Instance.Get_DecoAbillity(Costume_Kind_1, DECOABL_TYPE.Skill_Tm) +
                                    SendManager.Instance.Get_DecoAbillity(Costume_Kind_2, DECOABL_TYPE.Skill_Tm) +
                                    SendManager.Instance.Get_DecoAbillity(Costume_Kind_3, DECOABL_TYPE.Skill_Tm)
                                    ) * 0.01f;
            Main_Skill_Reload_MAX_Time -= Main_Skill_Reload_MAX_Time * Costume_Power;
            Sub_Skill_Reload_MAX_Time -= Sub_Skill_Reload_MAX_Time * Costume_Power;

            Grenade_Reload_Time = 0.0f;
            Grenade_Reload_MAX_Time = 3.0f;

            //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            //수류탄 데이터 셋팅
            Grenade_MAX_ATK = SendManager.Instance.Get_TableData().Infos_weapons[(uint)Link_Script.i.W_Grenade_Index].AtkMax;
            Grenade_MIN_ATK = SendManager.Instance.Get_TableData().Infos_weapons[(uint)Link_Script.i.W_Grenade_Index].AtkMin;           
            Grenade_ATK_Range = 2.5f;

            //코스튬 능력치 반환
            Costume_Power = (   SendManager.Instance.Get_DecoAbillity(Costume_Kind_1, DECOABL_TYPE.Granade_Pw) +
                                SendManager.Instance.Get_DecoAbillity(Costume_Kind_2, DECOABL_TYPE.Granade_Pw) +
                                SendManager.Instance.Get_DecoAbillity(Costume_Kind_3, DECOABL_TYPE.Granade_Pw)
                                ) * 0.01f;
            Grenade_MAX_ATK += (int)(Grenade_MAX_ATK * Costume_Power);
            Grenade_MIN_ATK += (int)(Grenade_MIN_ATK * Costume_Power);

            //붐머 데이터 셋팅
            Boomer_MAX_ATK = Level_Atk_Init(50, Char_Level);
            Boomer_MIN_ATK = Level_Atk_Init(40, Char_Level);
            Boomer_ATK_Range = 2.5f;

            //유탄 데이터 셋팅
            Launcher_MAX_ATK = Level_Atk_Init(SendManager.Instance.Get_TableData().Infos_weapons[(uint)Link_Script.i.W_Launchar_Index].AtkMax, Char_Level);
            Launcher_MIN_ATK = Level_Atk_Init(SendManager.Instance.Get_TableData().Infos_weapons[(uint)Link_Script.i.W_Launchar_Index].AtkMin, Char_Level);
            Launcher_ATK_Range = 2.5f;

            //직사포 데이터 셋팅
            Rocket_MAX_ATK = Level_Atk_Init(SendManager.Instance.Get_TableData().Infos_weapons[(uint)Link_Script.i.W_Rocket_Index].AtkMax, Char_Level);
            Rocket_MIN_ATK = Level_Atk_Init(SendManager.Instance.Get_TableData().Infos_weapons[(uint)Link_Script.i.W_Rocket_Index].AtkMin, Char_Level);
            Rocket_ATK_Range = 2.5f;
            
            //광통샷 데이터 셋팅
            ThroughShot_MAX_ATK = Level_Atk_Init(SendManager.Instance.Get_TableData().Infos_weapons[(uint)Link_Script.i.W_ThroughShot_Index].AtkMax, Char_Level);
            ThroughShot_MIN_ATK = Level_Atk_Init(SendManager.Instance.Get_TableData().Infos_weapons[(uint)Link_Script.i.W_ThroughShot_Index].AtkMin, Char_Level);
                                    
            //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            Char_Shot_State = CHAR_SHOT_STATE.IDEL;
            Shot_Make_Time = 0.0f;
            Shot_Network_Check = false;
            ShotGun_Network_Check = false;            
            Shot_Aim_Pos = new Vector3(0, 0, 0);
            BulletMark_Dir = new Vector3(0, 0, 0);

            //플레이어 총쏘는 컨트럴 리셋
            Game_Script.Player_Shot_State = PLAYER_SHOT_STATE.IDEL;
                                    
            Auto_Targeting_OJ.SetActive(false);

            //현재 장착된 무기 이미지로 셋팅
            UI_Script.Gun_Icon_Init(Gun_Index[(int)GUN_EQUIP_STATE.MAIN], Gun_Index[(int)GUN_EQUIP_STATE.SUB]);
                        
            //서버 현재 시간 요청 보내기
            Net_Script.Send_GetServerTime();            
        }
        else if (Char_User_Kind == CHAR_USER_KIND.NETWORK)
        {
            Auto_Targeting_OJ.SetActive(true);

            uint Max_Play_Count = 70;
            uint Now_Play_Count = SendManager.Instance.Get_UserGames().PlayCnt;
            float Max_Size = 0.6f;
            
            ////헤드샷 범위 셋팅
            //if (Now_Play_Count <= Max_Play_Count)//유저 플레이횟수 반환
            //{
            //    float Temp_Size = Max_Size - (Max_Size * (float)((float)Now_Play_Count / (float)Max_Play_Count));
            //    HeadShot_Target_OJ.localScale = new Vector3(0.6f + Temp_Size, 0.6f + Temp_Size, 0.6f + Temp_Size);
            //}
            //else
            //{
            //    HeadShot_Target_OJ.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            //}

            //헤드샷 범위 셋팅
            HeadShot_Target_OJ.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        }

        //죽고 난다음에 캐릭터 컨트럴러에 맞아서 수류탄이 튕기기때문에 죽으면 컬리더 셋팅해준다.
        Char_Controller.center = new Vector3(0.0f, 1.0f, 0.0f);
        //Auto_Targeting_OJ.SetActive(true);

        //쌍권총 총알 나가는 체크
        Double_Shot_State = DOUBLE_SHOOTING_STATE.IDEL;
            
        //네트웍 캐릭터 모델 바꿨는지 체크
        Net_Char_Model_Change_Check = false;

        //네트웍 캐릭터 코스튬 바꿨는지 체크
        Net_Char_Costume_Change_Check = false;

        //네트웍 캐릭터 총 바꿨는지 체크
        Net_Char_Gun_Change_Check = false;

        Jump_State = JUMP_STATE.IDEL;        
        Char_Gravity = 0.0f;
        
        Die_State = DIE_STATE.IDEL;
        Die_Atk_Start_Vec = new Vector3();
        Die_Atk_Target_Vec = new Vector3();
        Die_Atk_GunType = 0;
        
        Char_Hit_Effect_Check = false;
        Char_Hit_Effect_GunType = GUN_TYPE.RIFLE;
        Char_Hit_Effect_Vec = new Vector3();

        Vision_Time = 0.0f;
        Roller_Time = 0.0f;
        Roller_Hit_Check_Time = 0.0f;
        Guard_Time = 0.0f;
        Flamer_Time = 0.0f;
        Flamer_Hitting_Time = 0.0f;
        Boomer_Start_Time = 0.0f;
        Flying_Start_Time = 0.0f;
        ThroughShot_Start_Time = 0.0f;
        Rage_Time = 0.0f;
        SuperJump_Net_Check = false;
        NormalJump_Net_Check = false;

        Roller_OJ.SetActive(false);

        Guard_OJ.SetActive(false);
        Guard_BoxCollider.enabled = false;

        Flying_Skill_OJ.SetActive(false);
        Rage_Skill_OJ.SetActive(false);

        Damage_UI_Check = false;

        Char_Animator.enabled = true;
        

        for (int i = 0; i < Ragdoll_Collider_Name.Length; i++)
        {
            Ragdoll_Rigidbody[Ragdoll_Collider_Name[i]].GameObject_OJ.layer = Ragdoll_Layer;
            Ragdoll_Rigidbody[Ragdoll_Collider_Name[i]].Rigid_Body.isKinematic = true;
        }

        if (Head_Info_Transform == null)
        {
            Head_OJ_Name = User_ID + "_Head_Info_Text";

            Instantiate(Resources.Load("Battle_Prefab/1_Game_OJ_Folder/Head_Info_Text") as GameObject).name = Head_OJ_Name;
            Head_Info_Transform = GameObject.Find(Head_OJ_Name).GetComponent<Transform>();
            Head_Info_Transform.SetParent(GameObject.Find("Canvas").transform);
            Head_Info_Transform.localScale = new Vector3(1, 1, 1);
            Head_Info_Transform.localPosition = new Vector3(0, 0, 0);
            Head_Info_Transform.SetAsFirstSibling();//가장 첫번째 레이어로 맞춰서 켄버스 안에 있는 플레이 UI 밑으로 가려지게 만든다.

            Head_Info_Text = Head_Info_Transform.GetComponent<Text>();

            //팀에 따른 텍스트 컬러 조정
            if (Char_Team_State == CHAR_TEAM_SATAE.PLAYER_TEAM)
            {
                Head_Info_Text.color = new Color(ColorResult(3), ColorResult(52), ColorResult(172), 1.0f);
            }
            else
            {
                Head_Info_Text.color = new Color(ColorResult(210), ColorResult(28), ColorResult(28), 1.0f);
            }
        }        
        
        //유닛 움직임 네트웍으로 전송하기
        if (Char_User_Kind == CHAR_USER_KIND.PLAYER)
        {
            CameraControlScript.MotionBlur_Script.enabled = false;
            CameraControlScript.MotionBlur_Effect_State = 0; 

            Player_Char_LookAt_Transform = CameraControlScript.Camera_Center_Aim_OJ_Transform();

            if (Char_Coroutine != null) StopCoroutine(Char_Coroutine);
            Char_Coroutine = StartCoroutine(Char_Pos_Sending());

            if (Char_Skill_Coroutine != null) StopCoroutine(Char_Skill_Coroutine);
            Char_Skill_Coroutine = StartCoroutine(Char_Skill_Sending());

            Char_Skill_Now_Send_Check = true;
        }
        else if (Char_User_Kind == CHAR_USER_KIND.NETWORK)
        {
            Network_Char_LookAt_Vector = new Vector3();
        }

        //방어막 셋팅
        if (Char_User_Kind == CHAR_USER_KIND.PLAYER)
        {
            //무적 상태 오브젝트 
            if (User_Team < 50) Barrier_OJ[User_Team].SetActive(true);
            Barrier_Check = true;
            Barrier_Time = 3.0f;
            Barrier_Net_Send_Time = 0.0f;

            //헬리콥터 즉시리스폰 효과주기
            if (Game_Script.Respawn_Kind == RESPAWN_KIND.HELICOPTER)
            {
                Barrier_Time = 5.0f;
                Flying_Start(5.0f);
            }

            Game_Script.Respawn_Kind = RESPAWN_KIND.NORMAL;
        }
        else if (Char_User_Kind == CHAR_USER_KIND.NETWORK)
        {
            //무적 상태 오브젝트 
            if (User_Team < 50) Barrier_OJ[User_Team].SetActive(false);
            Barrier_Check = false;
            Barrier_Time = 0.0f;
            Barrier_Net_Send_Time = 0.0f;
        }
                
        Animator_Play_Init(ANISTATE.UPPER_MOVE[(int)Gun_Type]);

        //구르는 애니메이션중에 죽었다 살아났을때 애니메이션 리셋이 필요하다
        Char_Animator.Play(ANISTATE.UPPER_MOVE[(int)Gun_Type], 0);
        Char_Animator.Play(ANISTATE.LOW_MOVE, 1);
    }

    //캐릭터 리스폰 셋팅
    void Char_Respawn_Init()
    {
        Char_Respawn_Pos_Init(Respawn_Pos_Index);
        Char_Init(Char_User_Kind, Char_Team_State);
        Char_Controller.Move(Vector3.zero);

        //네트웍 캐릭터도 리스폰 데이터 받아서 좌표값 강제 셋팅해주고, 
        //다음 움직임 데이터 받을 텀에 이전 좌표값이 영향을 주지 못하도록 바로 네트웍 좌표값도 셋팅해준다.
        if (Char_User_Kind == CHAR_USER_KIND.NETWORK)
        {
            Net_Target_Pos = transform.position;

            //네트웍 캐릭터가 리스폰 되자 마자 캐릭터 이미지 셋팅하기 위해 리스폰 셋팅될때 캐릭터 변경체크를 해준다.(추가)
            Net_Char_Model_Change_Check = true;
        }

        MY_HP = Respawn_HP;
        MY_MAX_HP = MY_HP;

        Respawn_HP = 0.0f;
        Respawn_Pos_Index = 0;
    }

    //캐릭터 리스폰 위치 셋팅
    public void Char_Respawn_Pos_Init(byte Start_Index)
    {
        if (User_Team < 50)
        {
            transform.position = Game_Script.MapScript.Map_Start_Pos[((Start_Index % 12) % 6) + (User_Team * 6)];
            transform.rotation = Game_Script.MapScript.Map_Start_Dir[((Start_Index % 12) % 6) + (User_Team * 6)];
        }
        else
        {
            transform.position = Game_Script.MapScript.Map_Start_Pos[((Start_Index % 12) % 6) + (0 * 6)];
            transform.rotation = Game_Script.MapScript.Map_Start_Dir[((Start_Index % 12) % 6) + (0 * 6)];
        }
        

        if (Char_User_Kind == CHAR_USER_KIND.PLAYER)
        {
            CameraControlScript.Camera_Center_OJ_Transform().rotation = transform.rotation;
        }
    }

    //현재 캐릭터 레벨기준 공격력 증가 수치 셋팅
    int Level_Atk_Init(int _Atk, int _Level)
    {
        int Result = (int)(_Atk + ((_Atk * 0.1f) * (_Level - 1)));

        //셋트 버프 효과, 공격형 스킬 위력 증가
        if (SetBuff_Check && SetBuff_Kind == SetBufKnd.ATK_SKILL_DMG_UP)
        {
            Result = (int)(Result * SetBuff_Result);
        }

        return Result;
    }
        
    //================================================================================================================================================================

    //캐릭터 이미지 바꾸기
    void Char_Model_Change()
    {
        foreach (var _Char_Model_OJ in Char_Model_OJ)
        {
            _Char_Model_OJ.Value.OJ.SetActive(false);
        }

        Char_Model_OJ[Char_Index].OJ.SetActive(true);

        //현재 캐릭터의 코슈튬과 겹치는 오브젝트 켜기
        Costume_Over_OJ_Init(true);
    }

    //캐릭터 코스튬 바꾸기
    void Char_Costume_Change()
    {
        foreach (var _Costume_OJ in Costume_OJ)
        {
            _Costume_OJ.Value.OJ.SetActive(false);
        }

        if (Costume_Kind_1 != 0)
        {
            Costume_OJ[Costume_Kind_1].OJ.SetActive(true);

            //특정오브젝트 활성,비활성 (0이면 활성, 1이면 비활성)
            if (SendManager.Instance.Get_DecoInfo(Costume_Kind_1).AttmntActive == 1)
            {
                //현재 캐릭터의 코슈튬과 겹치는 오브젝트 끄기
                Costume_Over_OJ_Init(false);
            }
        }
        
        if (Costume_Kind_2 != 0) Costume_OJ[Costume_Kind_2].OJ.SetActive(true);
        if (Costume_Kind_3 != 0) Costume_OJ[Costume_Kind_3].OJ.SetActive(true);       
    }

    //현재 캐릭터의 코슈튬과 겹치는 오브젝트 켜기
    void Costume_Over_OJ_Init(bool View_Check)
    {
        foreach (var _Costume_Over_OJ in Costume_Over_OJ)
        {
            _Costume_Over_OJ.Value.OJ.SetActive(false);
        }

        if (View_Check)
        {           
            for (int k = 0; k < CHAR_OVER_OJ_MAX; k++)
            {
                String Temp_Over_OJ = "ch_" + Char_Index + "_OverOJ_" + k;

                if (Costume_Over_OJ.ContainsKey(Temp_Over_OJ))
                {
                    Costume_Over_OJ[Temp_Over_OJ].OJ.SetActive(true);
                }
            }
        }            
    }

    //네트웍 캐릭터 모델, 무기 바꾸기
    void Net_Char_Model_Gun_Change()
    {
        //네트웍 캐릭터 모델 바꿨는지 체크
        if (Net_Char_Model_Change_Check)
        {
            Net_Char_Model_Change_Check = false;

            //캐릭터 이미지 바꾸기
            Char_Model_Change();
        }

        //네트웍 캐릭터 코스튬 바꿨는지 체크
        if (Net_Char_Costume_Change_Check)
        {
            Net_Char_Costume_Change_Check = false;

            //캐릭터 코스튬 바꾸기
            Char_Costume_Change();
        }
            
        //네트웍 캐릭터 총 바꿨는지 체크
        if (Net_Char_Gun_Change_Check)
        {
            Net_Char_Gun_Change_Check = false;

            //무기이미지 보이기 셋팅
            Weapon_OJ_Init(false, -1);

            //무기이미지 보이기 셋팅
            Weapon_OJ_Init(true, Gun_Index[0]);
        }
    }

    //캐릭터 화면에 보이는 셋팅
    public void Char_View_Check(bool View_Check)
    {
        if (View_Check == false)
        {
            foreach (var _Char_Model_OJ in Char_Model_OJ)
            {
                _Char_Model_OJ.Value.OJ.SetActive(false);
            }

            foreach (var _Costume_OJ in Costume_OJ)
            {
                _Costume_OJ.Value.OJ.SetActive(false);
            }

            //현재 캐릭터의 코슈튬과 겹치는 오브젝트 끄기
            Costume_Over_OJ_Init(false);

            //주무기 스킨만 셋팅하기
            Transform[] GetTransforms = Weapon_OJ[Gun_Index[(int)Gun_Equip_State]].OJ.transform.GetComponentsInChildren<Transform>();
            foreach (Transform child in GetTransforms)
            {
                if (child.GetComponent<MeshRenderer>() != null) child.GetComponent<MeshRenderer>().enabled = false;
            }

            //무적 상태 오브젝트 
            if (User_Team < 50) Barrier_OJ[User_Team].SetActive(false);            
        }
        else
        {
            Char_Model_OJ[Char_Index].OJ.SetActive(true);

            //현재 캐릭터의 코슈튬과 겹치는 오브젝트 켜기
            Costume_Over_OJ_Init(true);

            if (Costume_Kind_1 != 0)
            {
                Costume_OJ[Costume_Kind_1].OJ.SetActive(true);

                //특정오브젝트 활성,비활성 (0이면 활성, 1이면 비활성)
                if (SendManager.Instance.Get_DecoInfo(Costume_Kind_1).AttmntActive == 1)
                {
                    //현재 캐릭터의 코슈튬과 겹치는 오브젝트 끄기
                    Costume_Over_OJ_Init(false);
                }
            }
            if (Costume_Kind_2 != 0) Costume_OJ[Costume_Kind_2].OJ.SetActive(true);
            if (Costume_Kind_3 != 0) Costume_OJ[Costume_Kind_3].OJ.SetActive(true);

            //주무기 스킨만 셋팅하기
            Transform[] GetTransforms = Weapon_OJ[Gun_Index[(int)Gun_Equip_State]].OJ.transform.GetComponentsInChildren<Transform>();
            foreach (Transform child in GetTransforms)
            {
                if (child.GetComponent<MeshRenderer>() != null) child.GetComponent<MeshRenderer>().enabled = true;
            }

            if (Barrier_Check)
            {
                //무적 상태 오브젝트 
                if (User_Team < 50) Barrier_OJ[User_Team].SetActive(true);
            }
            
            Animator_Play_Init(ANISTATE.UPPER_MOVE[(int)Gun_Type]);
        }
    }

    public void Char_Sight_Init()
    {
        if (Animator_State.shortNameHash == ANISTATE.UPPER_MOVE[(int)Gun_Type])
        {
            //조준 하는 애니메이션
            Animator_Play_Init(ANISTATE.SIGHT);
        }
    }

    //================================================================================================================================================================

    //플레이어 캐릭터 죽었을때 카메라 움직임 연산
    void Camera_Dead_Move_Operation()
    {
        if (Char_User_Kind != CHAR_USER_KIND.PLAYER) return;
        
        if (Game_Script.Char_Script.ContainsKey(Player_Killer_UserID))
        {
            Camera_Dead_Move_Speed += Time.deltaTime;
            if (Camera_Dead_Move_Speed <= 3.0f)
            {
                //플레이어 캐릭터 쳐다보기
                CameraControlScript.Camera_Transform.LookAt(Ragdoll_Rigidbody[Ragdoll_Collider_Name[0]].Transform_OJ);
            }
            else
            {
                Vector3 Target_Pos = Game_Script.Char_Script[Player_Killer_UserID].transform.position + (Vector3.up * 5.0f) - (Vector3.forward * 6.0f);
                CameraControlScript.Camera_Pos = Vector3.Lerp(CameraControlScript.Camera_Pos, Target_Pos, Time.deltaTime * 1.0f);

                //플레이어를 죽인 캐릭터 쳐다보기
                CameraControlScript.Camera_Transform.LookAt(Game_Script.Char_Script[Player_Killer_UserID].transform);
            }
        }
        else//자살로 인한 죽음일때 카메라 처리
        {
            //CameraControlScript.Camera_Transform.position += Vector3.up * (Time.deltaTime * 1.0f);
        }
    }

    //캐릭터 상반신 쳐다보기 연산
    void Char_Chest_LookAt_Operation()
    {
        if (Roller_Time > 0.0f) return;

        if (Char_User_Kind == CHAR_USER_KIND.PLAYER)
        {
            Char_Chest.LookAt(Player_Char_LookAt_Transform);
            if (Flamer_OJ.activeSelf) Flamer_OJ.transform.LookAt(Player_Char_LookAt_Transform);
        }
        else if (Char_User_Kind == CHAR_USER_KIND.NETWORK)
        {
            Char_Chest.LookAt(Network_Char_LookAt_Vector);
            if (Flamer_OJ.activeSelf) Flamer_OJ.transform.LookAt(Network_Char_LookAt_Vector);
        }

        Char_Chest.rotation = Char_Chest.rotation * Quaternion.Euler(Char_Chest_Offset);
    }

    //카메라 움직인 관련 연산
    void CameraMove_Operation()
    {
        if (Char_User_Kind != CHAR_USER_KIND.PLAYER) return;

        //카메라 좌표 캐릭터와 맞추기
        CameraControlScript.TransformPos(transform.position);

        //카메라 이동 큐브 레이케스트 연산
        CameraControlScript.Camera_Move_Raycast_Operation();

        ////타켓 큐브 움직임 연산
        //CameraControlScript.Aim_Cube_Move_Operation();

        //카메라 움직임 연산
        CameraControlScript.Camera_Move_Operation();

        //카메라 각도 움직임에 따른 캐릭터 좌우 각도 움직임
        transform.rotation = Quaternion.Euler(new Vector3(0, CameraControlScript.Camera_Center_OJ_EulerAngles().y, 0));
    }

    //================================================================================================================================================================
        
    //캐릭터 중력 연산
    void Char_Gravity_Oeration()
    {
        if (Char_User_Kind != CHAR_USER_KIND.PLAYER || Die_State != DIE_STATE.IDEL) return;

        if (Flying_Start_Time == 0.0f)
        {

            if (Jump_State == JUMP_STATE.IDEL)
            {
                CameraControlScript.Direct_Move = false;

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
            else
            {
                CameraControlScript.Direct_Move = true;

                if (SuperJump_OJ.activeSelf)
                {
                    //슈퍼 점프중 올가는 정점 근처까지 왔다면 천천히 올라가게 해서 체공시간을 높여준다.
                    if (Char_Gravity > -1.0f) Char_Gravity += (Time.deltaTime * 0.15f);
                    else Char_Gravity += (Time.deltaTime * 0.5f);

                    //떨어질때 슈퍼점프 효과 없애준다.
                    if (Char_Gravity > 0.0f)
                    {
                        SuperJump_Net_Check = false;
                        SuperJump_OJ.SetActive(false);

                        Char_Skill_Now_Send_Check = true;
                    }
                }
                else
                {
                    Char_Gravity += (Time.deltaTime * 0.5f);
                }

                Char_Controller.Move(Vector3.down.normalized * (Char_Gravity * (Time.deltaTime * 50.0f)));

                if (Char_Controller.isGrounded == true)
                {
                    Jump_State = JUMP_STATE.IDEL;
                }
            }

        }
        else//하늘을 날고 있다
        {
            if (Physics.Raycast(this.transform.position, Vector3.down, out Shot_RaycastHit, Mathf.Infinity))
            {
                float Temp_Distance = (Shot_RaycastHit.point - this.transform.position).magnitude;

                if (Temp_Distance < 10.0f - 2.0f) Flying_UD_State = 0;
                else if (Temp_Distance > 10.0f + 2.0f) Flying_UD_State = 1;

                if (Flying_UD_State == 0)//빠르게 올라가는중
                {
                    if (Temp_Distance < 10.0f) Char_Controller.Move(Vector3.up * (4.0f * Time.deltaTime));
                    else Flying_UD_State = 3;
                }
                else if (Flying_UD_State == 1)//빠르게 내려 가는중
                {
                    if (Temp_Distance > 10.0f - 1.0f) Char_Controller.Move(Vector3.down * (4.0f * Time.deltaTime));
                    else Flying_UD_State = 2;
                }
                else if (Flying_UD_State == 2)//느리게 올라가는중
                {
                    if (Temp_Distance < 10.0f) Char_Controller.Move(Vector3.up * (0.5f * Time.deltaTime));
                    else Flying_UD_State = 3;
                }
                else if (Flying_UD_State == 3)//느리게 내려 가는중
                {
                    if (Temp_Distance > 10.0f - 1.0f) Char_Controller.Move(Vector3.down * (0.5f * Time.deltaTime));
                    else Flying_UD_State = 2;
                }
            }
        }        
    }
        
    float Char_Speed = 0.0f;

    //플레이어가 컨트럴하는 캐릭터 이동 시키기
    public void Char_Move(float Temp_X, float Temp_Z)
    {
        if (Roller_Time == 0.0f)
        {
            Char_Speed = Time.deltaTime * MY_Move_Speed;

            if (Mathf.Abs(Temp_X) <= CameraControlScript.Camera_PixelWidth * 0.1f && Mathf.Abs(Temp_Z) <= CameraControlScript.Camera_PixelHeight * 0.1f)
            {
                Char_Speed = Time.deltaTime * (MY_Move_Speed * 0.5f);
            }
            else
            {
                if (Temp_Z <= 0.0f)
                {
                    Char_Speed = Time.deltaTime * (MY_Move_Speed * 0.5f);
                }
                else
                {
                    if (Mathf.Abs(Temp_X) > Mathf.Abs(Temp_Z)) Char_Speed = Time.deltaTime * (MY_Move_Speed * 0.5f);
                }
            }      
        }
        else
        {
            Char_Speed = Time.deltaTime * (MY_Move_Speed * 1.5f);            
        }

        if (Flying_Start_Time > 0.0f)
        {
            //이동속도가 너무 느린 캐릭터는 좀 빨리 날아다닐수 있게 한다.
            if (MY_Move_Speed <= 10.0f)
            {
                Char_Speed = Time.deltaTime * 10.0f;
            }
            else
            {
                Char_Speed = Time.deltaTime * MY_Move_Speed;
            }            
        }

        if (Flamer_Time > 0.0f)
        {
            if (Temp_Z > 0.3f)//앞으로 빨리 가고 있다면 파티클 공간을 월드에서 로컬로
            {
                Flamer_PA.Stop();
                Flamer_Local_PA.Play();
            }
            else//천천히 움직이고 있다고 판단되면 파티클 공간을 로컬에서 월드로
            {
                Flamer_PA.Play();
                Flamer_Local_PA.Stop();
            }
        }

        Char_Controller.Move(transform.TransformDirection(new Vector3(Temp_X, 0, Temp_Z).normalized * Char_Speed));
    }
    
    //================================================================================================================================================================

    //일반 점프
    public void Jump_Start()
    {        
        Jump_State = JUMP_STATE.JUMP_UP;
                
        Char_Gravity = -0.2f - Sub_Skill_Jump;
        Char_Controller.Move(Vector3.down.normalized * Char_Gravity);

        NormalJump_Net_Check = true;

        Char_Skill_Now_Send_Check = true;

        //사운드 재생
        SendManager.Instance.PlayGameSound(AUDIOSOURCE_TYPE.AUDIO_3D, 23, Audio_Source);
    }

    //슈퍼 점프 스킬
    public void Super_Jump_Start()
    {        
        Jump_State = JUMP_STATE.SUPER_JUMP_UP;

        Char_Gravity = -0.33f;
        Char_Controller.Move(Vector3.down.normalized * Char_Gravity);

        SuperJump_Net_Check = true;

        Char_Skill_Now_Send_Check = true;

        SuperJump_OJ.SetActive(true);
    }

    //상대편 투시 스킬
    public void Vision_Start(float Running_Time)
    {
        Vision_Time = Running_Time;

        //상대편 투시 효과 셋팅
        Vision_Init(true);
    }

    //상대편 투시 효과 셋팅
    void Vision_Init(bool Check)
    {
        if (Link_Script.i.Play_Mode == BattleKind.ALONE_PLAY_BATTLE)//싱글모드
        {
            foreach (var _Bot_Char_Script in Game_Script.Bot_Script)
            {
                if (_Bot_Char_Script.Value.Char_Team_State == CHAR_TEAM_SATAE.ENEMY_TEAM)
                {
                    foreach (var _Bot_Out_Line_OJ in _Bot_Char_Script.Value.Bot_Out_Line_OJ) _Bot_Out_Line_OJ.Value.OJ.enabled = Check;
                }
            }
        }
        else
        {
            foreach (var _Char_Script in Game_Script.Char_Script)
            {
                if (_Char_Script.Value.Char_Team_State == CHAR_TEAM_SATAE.ENEMY_TEAM)
                {
                    foreach (var _Out_Line_OJ in _Char_Script.Value.Out_Line_OJ) _Out_Line_OJ.Value.OJ.enabled = Check;
                }
            }
        }        

        //투시 스킬 UI 효과
        UI_Script.XRay_View(Check);
    }

    //돌진 스킬
    public void Roller_Start(float Running_Time)
    {
        Roller_Time = Running_Time;
        Roller_Hit_Check_Time = 0.0f;
        Roller_OJ.SetActive(true);

        Char_Skill_Now_Send_Check = true;
    }

    //CharacterController 에서 호출해주는 충동체크 함수
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!Char_Use_Check || Game_Script.GameOver_Check) return;

        if (Char_User_Kind == CHAR_USER_KIND.NETWORK || MY_HP == 0.0f) return;

        //죽는 바닥까지 떨어졌다면 죽인다
        if (hit.collider.CompareTag("DeadZone"))
        {
            MY_HP = 0.0f;

            //자살 데이터 보내기
            Net_Script.Send_Suicide_Data();
            return;
        }

        //러쉬 스킬중이 아니라면 리턴
        if (Roller_Time == 0.0f) return;

        if (Roller_Hit_Check_Time < 0.1f) return;

        if (hit.collider.CompareTag("WALL")) return;

        if (Link_Script.i.Play_Mode == BattleKind.ALONE_PLAY_BATTLE)//싱글모드
        {
            BOT_Script Bot_Char_Hitting_Script = hit.collider.transform.root.GetComponent<BOT_Script>();

            if (Bot_Char_Hitting_Script != null && Bot_Char_Hitting_Script.Char_Team_State == CHAR_TEAM_SATAE.ENEMY_TEAM)
            {
                if (Bot_Char_Hitting_Script.MY_HP > 0.0f)
                {
                    Roller_Hit_Check_Time = 0.0f;

                    bool MY_Critical_Check = false;

                    //데미지 연산
                    float MY_Total_Damage = Random(Level_Atk_Init(10, Char_Level), Level_Atk_Init(15, Char_Level));

                    if (Rage_Time > 0.0f)
                    {
                        MY_Total_Damage += MY_Total_Damage;
                        MY_Critical_Check = true;
                    }

                    //어택 데미지 데이터 보내기
                    Bot_Char_Hitting_Script.Bot_Damage_Operation(MY_Total_Damage, hit.point, hit.point, (byte)GUN_TYPE.ROLLER_SKILL, false, MY_Critical_Check);
                }
            }
        }
        else
        {
            Player_Script Char_Hitting_Script = hit.collider.transform.root.GetComponent<Player_Script>();

            if (Char_Hitting_Script != null && Char_Hitting_Script.Char_Team_State == CHAR_TEAM_SATAE.ENEMY_TEAM)
            {
                if (Char_Hitting_Script.MY_HP > 0.0f && Char_Hitting_Script.Barrier_Check == false)
                {
                    Roller_Hit_Check_Time = 0.0f;

                    bool MY_Critical_Check = false;

                    //데미지 연산
                    float MY_Total_Damage = Random(Level_Atk_Init(10, Char_Level), Level_Atk_Init(15, Char_Level));

                    if (Rage_Time > 0.0f)
                    {
                        MY_Total_Damage += MY_Total_Damage;
                        MY_Critical_Check = true;
                    }

                    //어택 데미지 데이터 보내기
                    Send_ATK_Data(Char_Hitting_Script.User_ID, MY_Critical_Check, false, MY_Total_Damage, hit.point, hit.point, (byte)GUN_TYPE.ROLLER_SKILL);
                }
            }
        }
    }

    //전면 방어 스킬
    public void Guard_Start(float Running_Time)
    {
        Guard_Time = Running_Time;

        Guard_OJ.SetActive(true);
        Guard_BoxCollider.enabled = false;

        Char_Skill_Now_Send_Check = true;
    }

    //화염방사기 스킬
    public void Flamer_Start(float Running_Time)
    {
        Flamer_Time = Running_Time;
        Flamer_Hitting_Time = 0.0f;

        Flamer_OJ.SetActive(true);
        Flamer_Local_OJ.SetActive(true);

        Flamer_PA.Play();
        Flamer_Local_PA.Stop();

        Dir_Four[0] = Vector3.up * 0.3f;
        Dir_Four[1] = Vector3.down * 0.3f;
        Dir_Four[2] = Vector3.left * 0.3f;
        Dir_Four[3] = Vector3.right * 0.3f;

        //공격 애니메이션
        Animator_Play_Init(ANISTATE.ATTACK_FLAME_THROWER);

        Char_Skill_Now_Send_Check = true;
    }

    Vector3[] Dir_Four = new Vector3[4];

    //화염방사기 충돌 체크
    void FlameThrower_Hit_Operation()
    {
        Flamer_Hitting_Time -= Time.deltaTime;
        if (Flamer_Hitting_Time <= 0.0f)
        {
            Flamer_Hitting_Time = 0.1f;

            for (int i = 0; i < 4; i++)
            {
                Vector3 Start_Vec = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z) + Dir_Four[i];
                Vector3 Target_Vec = Player_Char_LookAt_Transform.position + Dir_Four[i];

                //Debug.DrawRay(Start_Vec, (Target_Vec - Start_Vec).normalized * 6.0f, new Color(ColorResult(255), 0, 0));
                if (Physics.Raycast(Start_Vec, (Target_Vec - Start_Vec).normalized, out Shot_RaycastHit, 6.0f))
                {
                    //뭔게 맞았다면 맞은 위치를 목표지점으로 셋팅해준다.
                    Shot_Aim_Pos = Shot_RaycastHit.point;
                    BulletMark_Dir = Shot_RaycastHit.normal;

                    if (!Shot_RaycastHit.collider.CompareTag("OutSide_Cube") && !Shot_RaycastHit.collider.CompareTag("WALL"))
                    {

                        if (Link_Script.i.Play_Mode == BattleKind.ALONE_PLAY_BATTLE)//싱글모드
                        {
                            //캐릭터 맞춘 체크
                            BOT_Script Bot_Char_Hitting_Script = Shot_RaycastHit.transform.root.GetComponent<BOT_Script>();

                            if (Bot_Char_Hitting_Script != null && Bot_Char_Hitting_Script.Char_Team_State == CHAR_TEAM_SATAE.ENEMY_TEAM)
                            {
                                if (Bot_Char_Hitting_Script.MY_HP > 0.0f)
                                {
                                    bool MY_Critical_Check = false;

                                    //데미지 연산
                                    float MY_Total_Damage = Random(Level_Atk_Init(10, Char_Level), Level_Atk_Init(15, Char_Level));

                                    if (Rage_Time > 0.0f)
                                    {
                                        MY_Total_Damage += MY_Total_Damage;
                                        MY_Critical_Check = true;
                                    }

                                    //어택 데미지 데이터 보내기
                                    Bot_Char_Hitting_Script.Bot_Damage_Operation(MY_Total_Damage, new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), Shot_Aim_Pos, (byte)GUN_TYPE.FLAMER_SKILL, false, MY_Critical_Check);

                                    break;
                                }
                            }
                        }
                        else
                        {
                            //캐릭터 맞춘 체크
                            Player_Script Char_Hitting_Script = Shot_RaycastHit.transform.root.GetComponent<Player_Script>();

                            if (Char_Hitting_Script != null && Char_Hitting_Script.Char_Team_State == CHAR_TEAM_SATAE.ENEMY_TEAM)
                            {
                                if (Char_Hitting_Script.MY_HP > 0.0f && Char_Hitting_Script.Barrier_Check == false)
                                {
                                    bool MY_Critical_Check = false;

                                    //데미지 연산
                                    float MY_Total_Damage = Random(Level_Atk_Init(10, Char_Level), Level_Atk_Init(15, Char_Level));

                                    if (Rage_Time > 0.0f)
                                    {
                                        MY_Total_Damage += MY_Total_Damage;
                                        MY_Critical_Check = true;
                                    }

                                    //어택 데미지 데이터 보내기
                                    Send_ATK_Data(Char_Hitting_Script.User_ID, MY_Critical_Check, false, MY_Total_Damage, new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), Shot_Aim_Pos, (byte)GUN_TYPE.FLAMER_SKILL);

                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }


    //사방 수류탄 던지기 스킬
    public void Boomer_Start()
    {
        //일정 시간 뒤에 폭탄이 실제로 나간다
        Boomer_Start_Time = 0.5f;

        //공격 애니메이션
        Animator_Play_Init(ANISTATE.ATTACK_BOOM);                
    }

    //날아다니기
    public void Flying_Start(float Flying_Time)
    {
        Flying_Start_Time = Flying_Time;
        Flying_UD_State = 0;

        Flying_Skill_OJ.SetActive(true);

        Char_Skill_Now_Send_Check = true;
    }

    //크리티컬 100% 스킬
    public void Rage_Start()
    {
        Rage_Time = 5.0f;

        Rage_Skill_OJ.SetActive(true);

        Char_Skill_Now_Send_Check = true;
    }


    //관통샷 스킬
    public void ThroughShot_Start()
    {
        //일정 시간 뒤에 폭탄이 실제로 나간다
        ThroughShot_Start_Time = 0.5f;

        //공격 애니메이션
        Animator_Play_Init(ANISTATE.ATTACK_THROWSHOT);
    }
    
    //================================================================================================================================================================

    bool Char_Skill_Now_Send_Check = false;

    //스킬 데이터 바로 보내기
    void Char_Skill_Data_Now_Send()
    {
        if (Char_Skill_Now_Send_Check == false) return;

        Char_Skill_Now_Send_Check = false;

        Char_Skill_Send_Data();
    }

    Coroutine Char_Skill_Coroutine = null;

    //유닛 스킬관련 데이터 네트웍으로 전송하기
    IEnumerator Char_Skill_Sending()
    {
        yield return new WaitForSeconds(Link_Script.i.Char_Skill_Data_BPS);

        Char_Skill_Send_Data();

        Char_Skill_Coroutine = StartCoroutine(Char_Skill_Sending());
    }

    //스킬 관련 데이터 보내기
    public void Char_Skill_Send_Data()
    {
        if (!Char_Use_Check || Game_Script.GameOver_Check) return;

        if (Die_State != DIE_STATE.IDEL) return;

        ByteData Send_Buffer = new ByteData(512, 0);

        Send_Buffer.InPutByte((byte)RELAY_PROTOCOL.SKILL);
        Send_Buffer.InPutByte(User_ID);
        Send_Buffer.InPutByte(Costume_Kind_1);
        Send_Buffer.InPutByte(Costume_Kind_2);
        Send_Buffer.InPutByte(Costume_Kind_3);
        Send_Buffer.InPutByte(Animator_Change_Check);
        Send_Buffer.InPutByte(Animator_Play_Index);
        Send_Buffer.InPutByte(NormalJump_Net_Check);
        Send_Buffer.InPutByte(SuperJump_Net_Check);        
        Send_Buffer.InPutByte(Roller_Time);
        Send_Buffer.InPutByte(Guard_Time);
        Send_Buffer.InPutByte(Flamer_Time);
        Send_Buffer.InPutByte(Rage_Time);
        Send_Buffer.InPutByte(Flying_Start_Time);
        
        Net_Script.Send_PTP_Data(Send_Buffer);

        Animator_Change_Check = false;
        NormalJump_Net_Check = false;
    }
        
    int Net_Costume_Kind_1 = 0;
    int Net_Costume_Kind_2 = 0;
    int Net_Costume_Kind_3 = 0;
    int Net_Animator_Play_Index = 0;

    //네트웍으로 받은 스킬데이터 좌표
    public void Network_Skill_Pos(ByteData _Receive_data)
    {
        _Receive_data.OutPutVariable(ref Net_Costume_Kind_1);
        _Receive_data.OutPutVariable(ref Net_Costume_Kind_2);
        _Receive_data.OutPutVariable(ref Net_Costume_Kind_3);
        _Receive_data.OutPutVariable(ref Animator_Change_Check);
        _Receive_data.OutPutVariable(ref Net_Animator_Play_Index);
        _Receive_data.OutPutVariable(ref NormalJump_Net_Check);
        _Receive_data.OutPutVariable(ref SuperJump_Net_Check);
        _Receive_data.OutPutVariable(ref Roller_Time);
        _Receive_data.OutPutVariable(ref Guard_Time);
        _Receive_data.OutPutVariable(ref Flamer_Time);
        _Receive_data.OutPutVariable(ref Rage_Time);
        _Receive_data.OutPutVariable(ref Flying_Start_Time);
        
        if (Animator_Change_Check) Animator_Play_Init(Net_Animator_Play_Index);
                
        //현재 캐릭터의 코스튬 이덱스 이미지에 맞춰서 바꿔준다.
        if (Costume_Kind_1 != Net_Costume_Kind_1 || Costume_Kind_2 != Net_Costume_Kind_2 || Costume_Kind_3 != Net_Costume_Kind_3)
        {
            Costume_Kind_1 = Net_Costume_Kind_1;
            Costume_Kind_2 = Net_Costume_Kind_2;
            Costume_Kind_3 = Net_Costume_Kind_3;
            Net_Char_Costume_Change_Check = true;//네트웍 캐릭터 코스튬 바꾸기
        }
    }

    //================================================================================================================================================================

    Coroutine Char_Coroutine = null;        

    //유닛 움직임 네트웍으로 전송하기
    IEnumerator Char_Pos_Sending()
    {
        yield return new WaitForSeconds(Link_Script.i.Char_Data_BPS);

        Char_Pos_Send_Data();

        Char_Coroutine = StartCoroutine(Char_Pos_Sending());
    }

    public void Char_Pos_Send_Data()
    {
        if (!Char_Use_Check || Game_Script.GameOver_Check) return;

        if (Die_State != DIE_STATE.IDEL) return;

        ByteData Send_Buffer = new ByteData(512, 0);

        Send_Buffer.InPutByte((byte)RELAY_PROTOCOL.MOVE);
        Send_Buffer.InPutByte(User_ID);
        Send_Buffer.InPutByte(Char_Index);
        Send_Buffer.InPutByte(Char_Level);
        Send_Buffer.InPutByte((byte)Gun_Type);
        Send_Buffer.InPutByte(Gun_Index[(int)Gun_Equip_State]);
        Send_Buffer.InPutByte(transform.position);
        Send_Buffer.InPutByte(Char_Animator.GetFloat("Move_X"));
        Send_Buffer.InPutByte(Char_Animator.GetFloat("Move_Z"));
        Send_Buffer.InPutByte(transform.rotation.eulerAngles.y);
        Send_Buffer.InPutByte(Player_Char_LookAt_Transform.position);

        Net_Script.Send_PTP_Data(Send_Buffer);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------

    Vector3 Net_Target_Pos;
    float Net_Low_Ani_X = 0.0f;
    float Net_Low_Ani_Z = 0.0f;
    float Net_EulerAngles_Y = 0.0f;
    Vector3 Net_Chest_LookAt_Vecter;
    Vector3 Net_Jump_SendData_Vec;
               
    //네트웍으로 받은 움직일 좌표
    public void Network_Move_Pos(ByteData _Receive_data)
    {
        _Receive_data.OutPutVariable(ref Net_Target_Pos);
        _Receive_data.OutPutVariable(ref Net_Low_Ani_X);
        _Receive_data.OutPutVariable(ref Net_Low_Ani_Z);
        _Receive_data.OutPutVariable(ref Net_EulerAngles_Y);
        _Receive_data.OutPutVariable(ref Net_Chest_LookAt_Vecter);

        NC_Move_Distance = 0.0f;        
    }

    //최초 캐릭터 위치는 바로 셋팅해준다.
    public void Direct_Pos(ByteData _Receive_data)
    {
        Network_Move_Pos(_Receive_data);

        transform.position = Net_Target_Pos;
        transform.rotation = Quaternion.Euler(new Vector3(0, Net_EulerAngles_Y, 0));
        Network_Char_LookAt_Vector = Net_Chest_LookAt_Vecter;
    }

    Vector3 NC_Before_Pos = new Vector3();
    float NC_Before_Low_Ani_X = 0.0f;
    float NC_Before_Low_Ani_Z = 0.0f;
    float NC_Move_Distance = 0.0f;   
    Vector3 NC_Target_Start_Pos = new Vector3();


    //네트웍 데이터 받아서 움직임 처리
    void Network_Move_Operation()
    {
        if (Char_User_Kind == CHAR_USER_KIND.PLAYER || Die_State != DIE_STATE.IDEL) return;

        if (NC_Move_Distance == 0.0f) NC_Target_Start_Pos = transform.position;

        NC_Move_Distance += Time.deltaTime * (4.0f + (0.1f / Link_Script.i.Char_Data_BPS));

        //캐릭터컨트럴을 멈췄어도 움직이는 경우 하반신 애니메이션을 움직여준다.
        if (Net_Low_Ani_X == 0.0f && Net_Low_Ani_Z == 0.0f)
        {
            NC_Before_Pos = transform.position;

            //캐릭터 이동 좌표 연산
            transform.position = Vector3.Lerp(NC_Target_Start_Pos, Net_Target_Pos, NC_Move_Distance);

            //상대방은 이미 멈췄지만 아직 내쪽에서 보여지는 캐릭터는 움직일때 하반신 애니메이션 이전 움직임값으로 셋팅해준다.
            if ((NC_Before_Pos - transform.position).magnitude > 0.01f)
            {
                //캐릭터 하반신 애니메이션
                Char_Low_Move(NC_Before_Low_Ani_X, NC_Before_Low_Ani_Z);
            }
            else
            {
                //캐릭터 하반신 애니메이션
                Char_Low_Move(Net_Low_Ani_X, Net_Low_Ani_Z);

                NC_Before_Low_Ani_X = 0.0f;
                NC_Before_Low_Ani_Z = 0.0f;
            }
        }
        else
        {
            //캐릭터 이동 좌표 연산
            transform.position = Vector3.Lerp(NC_Target_Start_Pos, Net_Target_Pos, NC_Move_Distance);

            //캐릭터 하반신 애니메이션
            Char_Low_Move(Net_Low_Ani_X, Net_Low_Ani_Z);

            NC_Before_Low_Ani_X = Net_Low_Ani_X;
            NC_Before_Low_Ani_Z = Net_Low_Ani_Z;
        }

        //캐릭터 각도 연산
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0, Net_EulerAngles_Y, 0)), (Time.deltaTime * 10.0f));

        //캐릭터 흉상 각도 연산
        Network_Char_LookAt_Vector = Vector3.Lerp(Network_Char_LookAt_Vector, Net_Chest_LookAt_Vecter, (Time.deltaTime * 10.0f));
    }
               
    //================================================================================================================================================================

    //캐릭터 총쏘는 데이터 보내기
    public void Char_Shot_Data_Send()
    {
        ByteData Send_Buffer = new ByteData(128, 0);

        Send_Buffer.InPutByte((byte)RELAY_PROTOCOL.SHOT);
        Send_Buffer.InPutByte(User_ID);
        Send_Buffer.InPutByte(Shot_Aim_Pos);
        Send_Buffer.InPutByte(BulletMark_Dir);
        Send_Buffer.InPutByte(BulletMark_Check);

        Net_Script.Send_PTP_Data(Send_Buffer);
    }

    //네트웍으로 받은 캐릭터 총쏘는 데이터
    public void Network_Char_Shot_Pos(ByteData _Receive_data)
    {
        _Receive_data.OutPutVariable(ref Shot_Aim_Pos);
        _Receive_data.OutPutVariable(ref BulletMark_Dir);
        _Receive_data.OutPutVariable(ref BulletMark_Check);
        
        Shot_Network_Check = true;
    }

    //-------------------------------------------------------------------------------------------------------------------

    //캐릭터 샷건 총쏘는 데이터 보내기
    void Char_ShouGun_Data_Send()
    {
        ByteData Send_Buffer = new ByteData(512, 0);

        Send_Buffer.InPutByte((byte)RELAY_PROTOCOL.SHOTGUN_SHOT);
        Send_Buffer.InPutByte(User_ID);

        for (int i = 0; i < ShotGun_Aim_Pos.Length; i++)
        {
            Send_Buffer.InPutByte(ShotGun_Aim_Pos[i]);
            Send_Buffer.InPutByte(ShotGun_BulletMark_Dir[i]);
            Send_Buffer.InPutByte(ShotGun_BulletMark_Check[i]);
        }
        
        Net_Script.Send_PTP_Data(Send_Buffer);
    }

    //네트웍으로 받은 캐릭터 샷건 총쏘는 데이터
    public void Network_Char_ShotGun_Pos(ByteData _Receive_data)
    {
        for (int i = 0; i < ShotGun_Aim_Pos.Length; i++)
        {
            _Receive_data.OutPutVariable(ref ShotGun_Aim_Pos[i]);
            _Receive_data.OutPutVariable(ref ShotGun_BulletMark_Dir[i]);
            _Receive_data.OutPutVariable(ref ShotGun_BulletMark_Check[i]);
        }
        
        ShotGun_Network_Check = true;
    }

    //================================================================================================================================================================

    //발사 버튼 눌렸다
    public void Shot_On()
    {
        Char_Shot_State = CHAR_SHOT_STATE.ANI_START;
    }

    //발사 버튼 떨어졌다
    public void Shot_Off()
    {
        Char_Shot_State = CHAR_SHOT_STATE.SHOT_END;
    }

    //총 쏘는 연산
    void Shot_Operation()
    {
        if (Die_State != DIE_STATE.IDEL) return;

        if (Char_User_Kind == CHAR_USER_KIND.NETWORK)
        {
            //네트웍 캐릭터 일반 총 연산
            if (Shot_Network_Check)
            {
                Shot_Network_Check = false;

                //총알 날아가는 레이저 만들기
                StartCoroutine(ShowLaserBeam());
            }
            
            //네트웍 캐릭터 샷건 연산
            if (ShotGun_Network_Check)
            {
                ShotGun_Network_Check = false;

                for (int i = 0; i < ShotGun_Aim_Pos.Length; i++)
                {
                    Shot_Aim_Pos = ShotGun_Aim_Pos[i];
                    BulletMark_Dir = ShotGun_BulletMark_Dir[i];
                    BulletMark_Check = ShotGun_BulletMark_Check[i];

                    //총알 날아가는 레이저 만들기
                    StartCoroutine(ShowLaserBeam());
                }                
            }

            return;
        }

        if (Gun_Equip_State == GUN_EQUIP_STATE.MAIN && Gun_Reload_State[0] != RELOAD_STATE.IDEL) return;
        if (Gun_Equip_State == GUN_EQUIP_STATE.SUB && Gun_Reload_State[1] != RELOAD_STATE.IDEL) return;

        
        if (Gun_Type == GUN_TYPE.PUMP_SHOTGUN || Gun_Type == GUN_TYPE.AUTO_SHOTGUN)
        {
            //샷건 쏘기
            ShotGun_Shooting_Operation();
        }
        else if (Gun_Type == GUN_TYPE.DOUBLE_HANDGUN)
        {
            //쌍권총 쏘기
            Double_Shooting_Operation();
        }
        //else if (Gun_Type == GUN_TYPE.SNIPER)
        //{
        //    //스나이퍼 총쏘기
        //    Sniper_Shooting_Operation();
        //}
        else
        {
            //버튼 누르고 있으면 자동 발사
            Auto_Shooting_Operation();
        }        
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    //버튼 누르고 있으면 자동 발사
    void Auto_Shooting_Operation()
    {        
        switch (Char_Shot_State)
        {
            case CHAR_SHOT_STATE.IDEL:

                //샷버튼 연타시 바로바로 나갈수 있기때문에 여기서도 시간 체크 해준다.
                if (Shot_Make_Time != 0.0f)
                {
                    //다음 총알 쏠수 있는 시간 계산
                    TimeCount(ref Shot_Make_Time, Atk_Speed[(int)Gun_Equip_State]);
                }
                else
                {
                    //리로드 체크
                    Reload_Check();
                }

                break;
            case CHAR_SHOT_STATE.ANI_START:

                Char_Shot_State = CHAR_SHOT_STATE.SHOT_LAUNCH;

                break;
            case CHAR_SHOT_STATE.SHOT_LAUNCH:

                if (Shot_Make_Time == 0.0f)
                {
                    //리로드 체크
                    if (Reload_Check()) break;

                    //벽에 총알 튀는 임펙트 체크
                    BulletMark_Check = false;

                    //UI 화면의 조준점 이미지 좌표 기준으로 총 나갈 랜덤 범위 정해준다.
                    float Rand_Angle = Random(0.0f, 360.0f);
                    float Rand_Aim_Radius = Random(0.0f, (UI_Script.Aim_Radius_OJ.position.y - UI_Script.Spot_Image.position.y));
                    float Rand_X = UI_Script.Spot_Image.position.x + (Rand_Aim_Radius * Mathf.Cos(Rand_Angle * Mathf.Deg2Rad));
                    float Rand_Y = UI_Script.Spot_Image.position.y + (Rand_Aim_Radius * Mathf.Sin(Rand_Angle * Mathf.Deg2Rad));
                                        
                    //폰화면 기준으로 레이 셋팅
                    Ray Camera_Ray = CameraControlScript.Main_Camera.ScreenPointToRay(new Vector3(Rand_X, Rand_Y, 0.0f));

                    //흔들림 연산후 충돌체크
                    if (Physics.Raycast(Camera_Ray, out Shot_RaycastHit, Mathf.Infinity, RayCast_Layer))
                    {
                        //뭔게 맞았다면 맞은 위치를 목표지점으로 셋팅해준다.
                        Shot_Aim_Pos = Shot_RaycastHit.point;
                        BulletMark_Dir = Shot_RaycastHit.normal;

                        if (Shot_RaycastHit.collider.CompareTag("WALL"))
                        {
                            //벽에 총알 튀는 임펙트 체크
                            BulletMark_Check = true;                            
                        }
                        else if (!Shot_RaycastHit.collider.CompareTag("OutSide_Cube"))
                        {
                            //캐릭터 맞춘 체크
                            Char_Hitting_Check(Shot_RaycastHit, Camera_Ray);
                        }
                    }
                    
                    //총 종류별 카메라 흔들림 연산
                    CameraControlScript.Gun_Camera_Shake(Gun_Type);

                    //조준반동 연산
                    Aim_Move_Init();

                    //탄환감소
                    if (--Gun_Bullet[(int)Gun_Equip_State] <= 0) Gun_Bullet[(int)Gun_Equip_State] = 0;

                    //공격 애니메이션
                    Animator_Play_Init(ANISTATE.ATTACK[(int)Gun_Type]);

                    //총알 날아가는 레이저 만들기
                    StartCoroutine(ShowLaserBeam());

                    //캐릭터 총쏘는 데이터 보내기
                    Char_Shot_Data_Send();
                }

                //다음 총알 쏠수 있는 시간 계산
                TimeCount(ref Shot_Make_Time, Atk_Speed[(int)Gun_Equip_State]);
                
                break;
            case CHAR_SHOT_STATE.SHOT_END:

                //리로드 체크
                Reload_Check();

                Char_Shot_State = CHAR_SHOT_STATE.IDEL;

                break;
        }
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    ////스나이퍼 총쏘기
    //void Sniper_Shooting_Operation()
    //{
    //    switch (Char_Shot_State)
    //    {
    //        case CHAR_SHOT_STATE.IDEL:

    //            //리로드 체크
    //            if (Reload_Check()) break;

    //            break;
    //        case CHAR_SHOT_STATE.ANI_START:

    //            //조준하는 연산
    //            Char_Shot_State = CHAR_SHOT_STATE.SHOT_LAUNCH;

    //            //조준 하는 애니메이션
    //            Animator_Play_Init(ANISTATE.SIGHT);

    //            break;
    //        case CHAR_SHOT_STATE.SHOT_LAUNCH:

    //            break;
    //        case CHAR_SHOT_STATE.SHOT_END:

    //            //벽에 총알 튀는 임펙트 체크
    //            BulletMark_Check = false;

    //            //UI 화면의 조준점 이미지 좌표 기준으로 총 나갈 랜덤 범위 정해준다.
    //            float Rand_Angle = Random(0.0f, 360.0f);
    //            float Rand_Aim_Radius = Random(0.0f, (UI_Script.Aim_Radius_OJ.position.y - UI_Script.Spot_Image.position.y));
    //            float Rand_X = UI_Script.Spot_Image.position.x + (Rand_Aim_Radius * Mathf.Cos(Rand_Angle * Mathf.Deg2Rad));
    //            float Rand_Y = UI_Script.Spot_Image.position.y + (Rand_Aim_Radius * Mathf.Sin(Rand_Angle * Mathf.Deg2Rad));

    //            //폰화면 기준으로 레이 셋팅
    //            Ray Camera_Ray = CameraControlScript.Main_Camera.ScreenPointToRay(new Vector3(Rand_X, Rand_Y, 0.0f));

    //            //흔들림 연산후 충돌체크
    //            if (Physics.Raycast(Camera_Ray, out Shot_RaycastHit, Mathf.Infinity, RayCast_Layer))
    //            {
    //                //뭔게 맞았다면 맞은 위치를 목표지점으로 셋팅해준다.
    //                Shot_Aim_Pos = Shot_RaycastHit.point;
    //                BulletMark_Dir = Shot_RaycastHit.normal;

    //                if (Shot_RaycastHit.collider.CompareTag("WALL"))
    //                {
    //                    //벽에 총알 튀는 임펙트 체크
    //                    BulletMark_Check = true;
    //                }
    //                else if (!Shot_RaycastHit.collider.CompareTag("OutSide_Cube"))
    //                {
    //                    //캐릭터 맞춘 체크
    //                    Char_Hitting_Check(Shot_RaycastHit, Camera_Ray);
    //                }
    //            }

    //            //총 종류별 카메라 흔들림 연산
    //            CameraControlScript.Gun_Camera_Shake(Gun_Type);

    //            //조준반동 연산
    //            Aim_Move_Init();

    //            //탄환감소
    //            if (--Gun_Bullet[(int)Gun_Equip_State] <= 0) Gun_Bullet[(int)Gun_Equip_State] = 0;

    //            //공격 애니메이션
    //            Animator_Play_Init(ANISTATE.ATTACK[(int)Gun_Type]);

    //            //총알 날아가는 레이저 만들기
    //            StartCoroutine(ShowLaserBeam());

    //            //캐릭터 총쏘는 데이터 보내기
    //            Char_Shot_Data_Send();

    //            Char_Shot_State = CHAR_SHOT_STATE.SHOT_SNIPER_END;

    //            break;
    //        case CHAR_SHOT_STATE.SHOT_SNIPER_END:

    //            if (TimeCount(ref Shot_Make_Time, Atk_Speed[(int)Gun_Equip_State]))
    //            {
    //                //리로드 체크
    //                Reload_Check();

    //                Char_Shot_State = CHAR_SHOT_STATE.IDEL;
    //            }

    //            break;
    //    }
    //}

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    //샷건 쏘기
    void ShotGun_Shooting_Operation()
    {
        switch (Char_Shot_State)
        {
            case CHAR_SHOT_STATE.IDEL:

                //샷버튼 연타시 바로바로 나갈수 있기때문에 여기서도 시간 체크 해준다.
                if (Shot_Make_Time != 0.0f)
                {
                    //다음 총알 쏠수 있는 시간 계산
                    TimeCount(ref Shot_Make_Time, Atk_Speed[(int)Gun_Equip_State]);
                }
                else
                {
                    //리로드 체크
                    Reload_Check();
                }

                break;
            case CHAR_SHOT_STATE.ANI_START:

                Char_Shot_State = CHAR_SHOT_STATE.SHOT_LAUNCH;

                break;
            case CHAR_SHOT_STATE.SHOT_LAUNCH:

                if (Shot_Make_Time == 0.0f)
                {
                    //리로드 체크
                    if (Reload_Check()) break;

                    for (int i = 0; i < ShotGun_Aim_Pos.Length; i++)
                    {
                        //벽에 총알 튀는 임펙트 체크
                        BulletMark_Check = false;
                        ShotGun_BulletMark_Check[i] = false;

                        //UI 화면의 조준점 이미지 좌표 기준으로 총 나갈 랜덤 범위 정해준다.
                        float Rand_Angle = Random(0.0f, 360.0f);
                        float Rand_Aim_Radius = Random(0.0f, (UI_Script.Aim_Radius_OJ.position.y - UI_Script.Spot_Image.position.y));
                        float Rand_X = UI_Script.Spot_Image.position.x + (Rand_Aim_Radius * Mathf.Cos(Rand_Angle * Mathf.Deg2Rad));
                        float Rand_Y = UI_Script.Spot_Image.position.y + (Rand_Aim_Radius * Mathf.Sin(Rand_Angle * Mathf.Deg2Rad));

                        //폰화면 기준으로 레이 셋팅
                        Ray Camera_Ray = CameraControlScript.Main_Camera.ScreenPointToRay(new Vector3(Rand_X, Rand_Y, 0.0f));

                        //흔들림 연산후 충돌체크
                        if (Physics.Raycast(Camera_Ray, out Shot_RaycastHit, Mathf.Infinity, RayCast_Layer))
                        {
                            //뭔게 맞았다면 맞은 위치를 목표지점으로 셋팅해준다.
                            Shot_Aim_Pos = Shot_RaycastHit.point;
                            BulletMark_Dir = Shot_RaycastHit.normal;

                            if (Shot_RaycastHit.collider.CompareTag("WALL"))
                            {
                                //벽에 총알 튀는 임펙트 체크
                                BulletMark_Check = true;
                                ShotGun_BulletMark_Check[i] = true;
                            }
                            else if (!Shot_RaycastHit.collider.CompareTag("OutSide_Cube"))
                            {
                                //캐릭터 맞춘 체크
                                Char_Hitting_Check(Shot_RaycastHit, Camera_Ray);
                            }
                        }
                        
                        //샷건 총알 좌표 저장하기
                        ShotGun_Aim_Pos[i] = Shot_Aim_Pos;
                        ShotGun_BulletMark_Dir[i] = BulletMark_Dir;
                                                
                        //총알 날아가는 레이저 만들기
                        StartCoroutine(ShowLaserBeam());                        
                    }

                    //총 종류별 카메라 흔들림 연산
                    CameraControlScript.Gun_Camera_Shake(Gun_Type);

                    //조준반동 연산
                    Aim_Move_Init();

                    //탄환감소
                    if (--Gun_Bullet[(int)Gun_Equip_State] <= 0) Gun_Bullet[(int)Gun_Equip_State] = 0;

                    //공격 애니메이션
                    Animator_Play_Init(ANISTATE.ATTACK[(int)Gun_Type]);

                    //캐릭터 샷건 총쏘는 데이터 보내기
                    Char_ShouGun_Data_Send();
                }

                //다음 총알 쏠수 있는 시간 계산
                TimeCount(ref Shot_Make_Time, Atk_Speed[(int)Gun_Equip_State]);

                break;
            case CHAR_SHOT_STATE.SHOT_END:

                //리로드 체크
                Reload_Check();

                Char_Shot_State = CHAR_SHOT_STATE.IDEL;

                break;
        }
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    //쌍권총 쏘기
    void Double_Shooting_Operation()
    {
        switch (Char_Shot_State)
        {
            case CHAR_SHOT_STATE.IDEL:

                //샷버튼 연타시 바로바로 나갈수 있기때문에 여기서도 시간 체크 해준다.
                if (Shot_Make_Time != 0.0f)
                {
                    //다음 총알 쏠수 있는 시간 계산
                    TimeCount(ref Shot_Make_Time, Atk_Speed[(int)Gun_Equip_State]);
                }
                else
                {
                    //리로드 체크
                    Reload_Check();
                }

                break;
            case CHAR_SHOT_STATE.ANI_START:

                Char_Shot_State = CHAR_SHOT_STATE.SHOT_LAUNCH;

                break;
            case CHAR_SHOT_STATE.SHOT_LAUNCH:
                                
                if (Shot_Make_Time == 0.0f && Double_Shot_State == DOUBLE_SHOOTING_STATE.IDEL)
                {
                    //리로드 체크
                    if (Reload_Check()) break;

                    Double_Shot_State = DOUBLE_SHOOTING_STATE.FIRST_SHOT;
                }

                //다음 총알 쏠수 있는 시간 계산
                TimeCount(ref Shot_Make_Time, Atk_Speed[(int)Gun_Equip_State]);

                break;
            case CHAR_SHOT_STATE.SHOT_END:

                //리로드 체크
                Reload_Check();

                Char_Shot_State = CHAR_SHOT_STATE.IDEL;

                break;
        }

        switch (Double_Shot_State)
        {
            case DOUBLE_SHOOTING_STATE.IDEL:

                break;
            case DOUBLE_SHOOTING_STATE.FIRST_SHOT:
            case DOUBLE_SHOOTING_STATE.SECOND_SHOT:

                //벽에 총알 튀는 임펙트 체크
                BulletMark_Check = false;

                //UI 화면의 조준점 이미지 좌표 기준으로 총 나갈 랜덤 범위 정해준다.
                float Rand_Angle = Random(0.0f, 360.0f);
                float Rand_Aim_Radius = Random(0.0f, (UI_Script.Aim_Radius_OJ.position.y - UI_Script.Spot_Image.position.y));
                float Rand_X = UI_Script.Spot_Image.position.x + (Rand_Aim_Radius * Mathf.Cos(Rand_Angle * Mathf.Deg2Rad));
                float Rand_Y = UI_Script.Spot_Image.position.y + (Rand_Aim_Radius * Mathf.Sin(Rand_Angle * Mathf.Deg2Rad));

                //폰화면 기준으로 레이 셋팅
                Ray Camera_Ray = CameraControlScript.Main_Camera.ScreenPointToRay(new Vector3(Rand_X, Rand_Y, 0.0f));

                //흔들림 연산후 충돌체크
                if (Physics.Raycast(Camera_Ray, out Shot_RaycastHit, Mathf.Infinity, RayCast_Layer))
                {
                    //뭔게 맞았다면 맞은 위치를 목표지점으로 셋팅해준다.
                    Shot_Aim_Pos = Shot_RaycastHit.point;
                    BulletMark_Dir = Shot_RaycastHit.normal;

                    if (Shot_RaycastHit.collider.CompareTag("WALL"))
                    {
                        //벽에 총알 튀는 임펙트 체크
                        BulletMark_Check = true;
                    }
                    else if (!Shot_RaycastHit.collider.CompareTag("OutSide_Cube"))
                    {
                        //캐릭터 맞춘 체크
                        Char_Hitting_Check(Shot_RaycastHit, Camera_Ray);
                    }
                }
                
                //총 종류별 카메라 흔들림 연산
                CameraControlScript.Gun_Camera_Shake(Gun_Type);

                //조준반동 연산
                Aim_Move_Init();

                //탄환감소
                if (--Gun_Bullet[(int)Gun_Equip_State] <= 0) Gun_Bullet[(int)Gun_Equip_State] = 0;

                //총알 날아가는 레이저 만들기
                StartCoroutine(ShowLaserBeam());
                                
                if (Double_Shot_State == DOUBLE_SHOOTING_STATE.FIRST_SHOT)
                {
                    //공격 애니메이션
                    Animator_Play_Init(ANISTATE.ATTACK[(int)Gun_Type]);

                    //캐릭터 총쏘는 데이터 보내기
                    Char_Shot_Data_Send();

                    Double_Shot_State = DOUBLE_SHOOTING_STATE.SHOT_WAIT;
                }
                else if (Double_Shot_State == DOUBLE_SHOOTING_STATE.SECOND_SHOT)
                {
                    //캐릭터 총쏘는 데이터 보내기
                    Char_Shot_Data_Send();

                    Double_Shot_State = DOUBLE_SHOOTING_STATE.IDEL;
                }
                
                break;
            case DOUBLE_SHOOTING_STATE.SHOT_WAIT:

                if (Animator_Play_Index == ANISTATE.ATTACK[(int)Gun_Type])
                {
                    //더블핸드는 어택공격 애니메이션 하나에 두번의 샷 동작이 들어 있어 애니메이션 길이를 절반으로 체크해준다.
                    if (Animator_State.length * 0.5f <= Animator_State.normalizedTime)
                    {
                        Double_Shot_State = DOUBLE_SHOOTING_STATE.SECOND_SHOT;
                    }
                }

                break;            
        }
    }

    //==========================================================================================================================================================================================================

    //조준반동 연산
    void Aim_Move_Init()
    {
        Game_Script.Aim_Move_Size += (100 - Aim_Ctrl[(int)Gun_Equip_State]) * 0.8f;
    }
        
    //==========================================================================================================================================================================================================

    //어택 데미지 데이터 보내기
    public void Send_ATK_Data(uint Enemy_User_ID, bool _Critical_Check, bool _HeadShot_Check, float _Total_Damage, Vector3 _Start_Pos, Vector3 _Target_Pos, byte _Gun_Type)
    {
        //나의 네트웍 상태가 좋지 않은 경우에는 상대방이 멈춰있기때문에 어택 프로토콜 보내지 않는다.
        if (Net_Script.Network_Delay_Check) return;

        //킬,데스 카운트별 버프 셋팅 
        if (Game_Script.Death_Buff_Size > 0.0f) _Total_Damage += (int)(_Total_Damage * Game_Script.Death_Buff_Size);

        ByteData Send_Buffer = new ByteData(128, 0);

        Send_Buffer.InPutByte(User_ID);
        Send_Buffer.InPutByte(Enemy_User_ID);
        Send_Buffer.InPutByte(_Critical_Check);
        Send_Buffer.InPutByte(_HeadShot_Check);
        Send_Buffer.InPutByte(_Total_Damage);
        Send_Buffer.InPutByte(_Start_Pos);
        Send_Buffer.InPutByte(_Target_Pos);
        Send_Buffer.InPutByte(_Gun_Type);

        Net_Script.Send_AtkDmg_Data(Send_Buffer);
    }

    //어택 데미지 데이터 받기
    public void Receive_ATK_Data(float _Now_HP, Vector3 _ATK_Start_Vec, Vector3 _ATK_Target_Vec, byte _Gun_Type)
    {
        MY_HP = _Now_HP;

        //자살일때
        if ((GUN_TYPE)_Gun_Type == GUN_TYPE.SUICIDE) MY_HP = 0.0f;

        if (MY_HP <= 0.0f && Die_State == DIE_STATE.IDEL)
        {
            Die_State = DIE_STATE.ANI_START;
            Die_Atk_Start_Vec = _ATK_Start_Vec;
            Die_Atk_Target_Vec = _ATK_Target_Vec;
            Die_Atk_GunType = _Gun_Type;
        }

        //수류탄, 유탄, 자살이 아닐때 캐릭터 맞았을때 터지는 임펙트 안그려준다.
        if ((GUN_TYPE)_Gun_Type != GUN_TYPE.GRENADE && (GUN_TYPE)_Gun_Type != GUN_TYPE.BOOMER_SKILL && (GUN_TYPE)_Gun_Type != GUN_TYPE.LAUNCHER && (GUN_TYPE)_Gun_Type != GUN_TYPE.SUICIDE)
        {
            Char_Hit_Effect_Check = true;
            Char_Hit_Effect_GunType = (GUN_TYPE)_Gun_Type;
            Char_Hit_Effect_Vec = _ATK_Target_Vec;
        }

        if (Char_User_Kind == CHAR_USER_KIND.PLAYER && (GUN_TYPE)_Gun_Type != GUN_TYPE.SUICIDE)
        {
            //맞은 캐릭터가 플레이어라면 데미지 효과 그려준다.
            Damage_UI_Check = true;
        }        
    }
        
    //캐릭터 맞춘 체크
    void Char_Hitting_Check(RaycastHit _Shot_RaycastHit, Ray Camera_Ray)
    {
        if (Link_Script.i.Play_Mode == BattleKind.WAR_OF_POSITION && _Shot_RaycastHit.collider.CompareTag("Destroy_OJ"))//진지부수기 오브젝트
        {
            //벽에 총알 튀는 임펙트 체크
            BulletMark_Check = true;

            DestroyMode_Script Destroy_Hitting_Script = _Shot_RaycastHit.transform.root.GetComponent<DestroyMode_Script>();

            if (_Shot_RaycastHit.collider.name.Equals(Destroy_Hitting_Script.Enemy_Base_Name))
            {
                //데미지 연산
                float MY_Total_Damage = Random(MY_MIN_ATK[(int)Gun_Equip_State], MY_MAX_ATK[(int)Gun_Equip_State]);

                Net_Script.Send_Base_Atk(MY_Total_Damage);
            }
        }
        else if (Link_Script.i.Play_Mode == BattleKind.ALONE_PLAY_BATTLE)//싱글모드
        {
            BOT_Script Bot_Hitting_Script = _Shot_RaycastHit.transform.root.GetComponent<BOT_Script>();

            if (Bot_Hitting_Script != null && Bot_Hitting_Script.Char_Team_State == CHAR_TEAM_SATAE.ENEMY_TEAM)
            {
                if (Bot_Hitting_Script.MY_HP > 0.0f)
                {
                    //데미지 연산
                    Damage_Operation(Camera_Ray, Bot_Hitting_Script.User_ID, Bot_Hitting_Script);
                }
            }
        }
        else
        {
            Player_Script Char_Hitting_Script = _Shot_RaycastHit.transform.root.GetComponent<Player_Script>();

            if (Char_Hitting_Script != null && Char_Hitting_Script.Char_Team_State == CHAR_TEAM_SATAE.ENEMY_TEAM)
            {
                if (Char_Hitting_Script.MY_HP > 0.0f && Char_Hitting_Script.Barrier_Check == false && Char_Hitting_Script.Roller_Time == 0.0f && Char_Hitting_Script.Flamer_Time == 0.0f)
                {
                    //데미지 연산
                    Damage_Operation(Camera_Ray, Char_Hitting_Script.User_ID, null);
                }
            }

        }
    }

    //데미지 연산
    void Damage_Operation(Ray Camera_Ray, uint User_ID, BOT_Script Bot_Hitting_Script)
    {
        bool MY_Critical_Check = false;

        //데미지 연산
        float MY_Total_Damage = Random(MY_MIN_ATK[(int)Gun_Equip_State], MY_MAX_ATK[(int)Gun_Equip_State]);

        //크리티컬 연산(현재 데미지의 2배)
        if (Probability(MY_Critical[(int)Gun_Equip_State]) || Rage_Time > 0.0f)
        {
            MY_Total_Damage += MY_Total_Damage;
            MY_Critical_Check = true;
        }

        bool _HeadShot_Check = false;

        //샷건은 헤드샷 빼준다.
        if (Gun_Type != GUN_TYPE.PUMP_SHOTGUN && Gun_Type != GUN_TYPE.AUTO_SHOTGUN)
        {
            //헤드샷 연산(현재 데미지의 5배)
            _HeadShot_Check = HeadShot_Check(Camera_Ray, User_ID);
            if (_HeadShot_Check)
            {
                MY_Total_Damage += MY_Total_Damage * 5.0f;
                MY_Critical_Check = true;
            }
        }

        //셋트 버프 효과 2배 공격력
        if (SetBuff_Check && SetBuff_Kind == SetBufKnd.ATK_UP)
        {
            MY_Total_Damage = (int)(MY_Total_Damage * SetBuff_Result);
        }

        if (Link_Script.i.Play_Mode == BattleKind.ALONE_PLAY_BATTLE)//싱글모드
        {
            Bot_Hitting_Script.Bot_Damage_Operation(MY_Total_Damage, new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), Shot_Aim_Pos, (byte)Gun_Type, _HeadShot_Check, MY_Critical_Check);
        }
        else
        {
            //어택 데미지 데이터 보내기
            Send_ATK_Data(User_ID, MY_Critical_Check, _HeadShot_Check, MY_Total_Damage, new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), Shot_Aim_Pos, (byte)Gun_Type);        
        }                
    }

    RaycastHit HeadShot_RaycastHit;

    //헤드샷 체크
    bool HeadShot_Check(Ray HeadShot_Check_Ray, uint User_ID)
    {
        if (Physics.Raycast(HeadShot_Check_Ray, out HeadShot_RaycastHit, Mathf.Infinity, HeadShot_Layer))
        {
            if (Link_Script.i.Play_Mode == BattleKind.ALONE_PLAY_BATTLE)//싱글모드
            {
                BOT_Script Char_Hitting_Script = HeadShot_RaycastHit.transform.root.GetComponent<BOT_Script>();

                if (Char_Hitting_Script != null && Char_Hitting_Script.User_ID == User_ID) return true;
            }
            else
            {
                Player_Script Char_Hitting_Script = HeadShot_RaycastHit.transform.root.GetComponent<Player_Script>();

                if (Char_Hitting_Script != null && Char_Hitting_Script.User_ID == User_ID) return true;
            }            
        }

        return false;
    }

    //==========================================================================================================================================================================================================

    public struct ATK_DAMAGE_INFO
    {
        public uint DEF_User_ID;
        public bool Critical_Check;
        public float Damage_Num;        
    }
    ATK_DAMAGE_INFO Atk_Damage_Info;

    ArrayList Atk_Damage_Info_List = new ArrayList();

    //플레이어가 데미지를 준 캐릭터의 데이터가 왔다면 화면에 데미지 연산 그려준다.
    public void Receive_ATK_UI_Damage_Init(uint DEF_User_ID, bool Critical_Check, float Damage_Num)
    {
        if (Char_User_Kind == CHAR_USER_KIND.NETWORK) return;

        if (Game_Script.Char_Script.ContainsKey(DEF_User_ID) == false) return;

        Atk_Damage_Info.DEF_User_ID = DEF_User_ID;
        Atk_Damage_Info.Critical_Check = Critical_Check;
        Atk_Damage_Info.Damage_Num = (int)Damage_Num;
        Atk_Damage_Info_List.Add(Atk_Damage_Info);
    }

    int Damage_Num_Effect_Dir = 0;

    //플레이어가 준 데미지 그리기
    void ATK_UI_Damage_Operation()
    {
        if (Char_User_Kind == CHAR_USER_KIND.NETWORK) return;

        for (int i = 0; i < Atk_Damage_Info_List.Count; i++)
        {
            ATK_DAMAGE_INFO _Atk_Damage_Info = (ATK_DAMAGE_INFO)Atk_Damage_Info_List[0];
            Atk_Damage_Info_List.RemoveAt(0);

            if (Game_Script.Char_Script.ContainsKey(_Atk_Damage_Info.DEF_User_ID) == false) continue;

            //데미지 숫자 만들어주기
            Transform GamePlay_Damage_Transform = Instantiate(Resources.Load("Battle_Prefab/2_UI_Folder/GamePlay_Damage") as GameObject).GetComponent<Transform>();
            GamePlay_Damage_Transform.SetParent(GameObject.Find("Canvas").transform);
            GamePlay_Damage_Transform.localScale = new Vector3(1, 1, 1);
            GamePlay_Damage_Transform.localPosition = new Vector3(0, 0, 0);
            GamePlay_Damage_Transform.SetAsLastSibling();//가장 나중에 그려진 레이어로 맞추기

            //텍스트 스크립트 초기화        
            GamePlay_Damage_Transform.GetComponent<Damage_Num_Script>().Make_Init(_Atk_Damage_Info.DEF_User_ID, _Atk_Damage_Info.Critical_Check, _Atk_Damage_Info.Damage_Num, Damage_Num_Effect_Dir);

            if (++Damage_Num_Effect_Dir >= 2) Damage_Num_Effect_Dir = 0;
        }
    }

    //==========================================================================================================================================================================================================
        
    //복수 성공 아이콘 띄우기
    void Revenge_OK_Operation()
    {
        if (Revenge_OK_Icon_Time == 0.0f) return;

        if (Revenge_OK_Icon_Time == Revenge_OK_Icon_MAX_Time)
        {
            Revenge_OK_Icon_OJ.SetActive(true);

            if (Char_User_Kind == CHAR_USER_KIND.PLAYER)
            {
                //복수 성공 표시 패킷 보내기
                Revenge_OK_Send_Data();

                if (Game_Script.Char_Script.ContainsKey(Revenge_UserID))
                {
                    Game_Script.Char_Script[Revenge_UserID].Revenge_OJ.SetActive(false);
                }                
                Revenge_UserID = 0;
            }
        }

        Revenge_OK_Icon_Time -= Time.deltaTime;
        if (Revenge_OK_Icon_Time <= 0.0f)
        {
            Revenge_OK_Icon_Time = 0.0f;
            Revenge_OK_Icon_OJ.SetActive(false);
        }
    }

    //복수 성공 표시 패킷 보내기
    void Revenge_OK_Send_Data()
    {
        ByteData Send_Buffer = new ByteData(128, 0);

        Send_Buffer.InPutByte((byte)RELAY_PROTOCOL.REVENGE_OK);
        Send_Buffer.InPutByte(User_ID);      

        Net_Script.Send_PTP_Data(Send_Buffer);
    }

    //복수 성공 표시 패킷 받기
    public void Revenge_OK_Recv_Data()
    {
        Revenge_OK_Icon_Time = Revenge_OK_Icon_MAX_Time;
    }

    //==========================================================================================================================================================================================================

    //캐릭터 죽을때 현재 스킬등 변수값 초기화
    void Char_Die_Skill_Init()
    {
        //죽고 난다음에 캐릭터 컨트럴러에 맞아서 수류탄이 튕기기때문에 죽으면 컬리더 셋팅해준다.
        Char_Controller.center = new Vector3(0.0f, -100.0f, 0.0f);
        Auto_Targeting_OJ.SetActive(false);

        //죽을때 헤드샷 오브젝트 크기 정상으로 돌려놓는다. 너무 커서 캐릭터가 눕질 않는다.
        HeadShot_Target_OJ.localScale = new Vector3(0.6f, 0.6f, 0.6f);

        Shot_Off();

        Gun_Reload_State[0] = RELOAD_STATE.IDEL;
        Gun_Reload_State[1] = RELOAD_STATE.IDEL;

        Head_Info_Text.enabled = false;

        Vision_Time = 0.0f;
        Roller_Time = 0.0f;
        Roller_Hit_Check_Time = 0.0f;
        Guard_Time = 0.0f;
        Flamer_Time = 0.0f;
        Flamer_Hitting_Time = 0.0f;
        Boomer_Start_Time = 0.0f;
        Rage_Time = 0.0f;
        Flying_Start_Time = 0.0f;
        ThroughShot_Start_Time = 0.0f;

        Roller_OJ.SetActive(false);

        Guard_OJ.SetActive(false);
        Guard_BoxCollider.enabled = false;

        Flamer_PA.Stop();
        Flamer_Local_PA.Stop();
        Flamer_OJ.SetActive(false);
        Flamer_Local_OJ.SetActive(false);

        SuperJump_Net_Check = false;
        SuperJump_OJ.SetActive(false);

        Rage_Skill_OJ.SetActive(false);
        Flying_Skill_OJ.SetActive(false);

        if (Char_User_Kind == CHAR_USER_KIND.PLAYER)
        {
            //상대편 투시 효과 셋팅
            Vision_Init(false);
        }

        //복수 성공 아이콘 없애준다.
        Revenge_OK_Icon_OJ.SetActive(false);
    }

    //캐릭터 죽는 연산
    void Char_Die_Operation()
    {
        switch (Die_State)
        {
            case DIE_STATE.IDEL:

                break;
            case DIE_STATE.ANI_START:

                if (Char_User_Kind == CHAR_USER_KIND.PLAYER)
                {
                    Animator_Play_Init(ANISTATE.UPPER_MOVE[(int)Gun_Type]);
                    Char_Animator.Play(ANISTATE.UPPER_MOVE[(int)Gun_Type], 0);
                    Char_Animator.Play(ANISTATE.LOW_MOVE, 1);

                    //작동 키 리셋
                    Game_Script.Left_Cursor_Init();
                    Game_Script.Right_Cursor_Init();

                    Game_Script.Shot_Button(BUTTON_DIR_STATE.RIGHT, PLAYER_SHOT_STATE.SHOT_END);

                    UI_Script.Zoom_UI_Init();

                    Char_View_Check(true);

                    Game_Script.Progress_State = PROGRESS_STATE.PANEL_CLOSE;
                }
                                
                //캐릭터 죽을때 현재 스킬등 변수값 초기화
                Char_Die_Skill_Init();

                Camera_Dead_Move_Speed = 0.0f;

                NC_Move_Distance = 0.0f;

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

                break;
            case DIE_STATE.RESPAWN_INIT:

                
                if (Char_User_Kind == CHAR_USER_KIND.PLAYER)
                {
                    //리스폰 UI 끄기
                    Game_Script.Disable_Respawn_UI_OJ();

                    //자살이 아닐때, 날 죽인 캐릭터 복수 아이콘 띄우기
                    if (Player_Killer_UserID != 0)
                    {                       
                        foreach (var _Char_Script in Game_Script.Char_Script)
                        {
                            if (_Char_Script.Value.Revenge_OJ.activeSelf) _Char_Script.Value.Revenge_OJ.SetActive(false);
                        }

                        if (Game_Script.Char_Script.ContainsKey(Revenge_UserID))
                        {
                            Game_Script.Char_Script[Revenge_UserID].Revenge_OJ.SetActive(true);
                        }                        
                    }
                }

                //캐릭터 리스폰 셋팅
                Char_Respawn_Init();
                                                                                                
                Die_State = DIE_STATE.IDEL;

                break;
        }
    }

    //리스폰할 데이터값 셋팅
    public void Recv_Respawn_Data(ByteData _Receive_data)
    {
        //int : 유닛 아이디
        //byte : 유닛 강화도
        //int : 메인무기
        //int : 보조무기
        // float : 오브젝트 체력
        // byte : 시작(부활 + 입장) 포인트
        
        int _Char_Index = 0;
        byte _Char_Level = 0;
        int _Main_Gun_Index = 0;
        int _Sub_Gun_Index = 0;
        float _Respawn_HP = 0.0f;
        byte _Respawn_Pos_Index = 0;

        _Receive_data.OutPutVariable(ref _Char_Index);
        _Receive_data.OutPutVariable(ref _Char_Level);
        _Receive_data.OutPutVariable(ref _Main_Gun_Index);
        _Receive_data.OutPutVariable(ref _Sub_Gun_Index);
        _Receive_data.OutPutVariable(ref _Respawn_HP);
        _Receive_data.OutPutVariable(ref _Respawn_Pos_Index);

        if (Char_User_Kind == CHAR_USER_KIND.PLAYER)
        {
            Link_Script.i.Char_Index = _Char_Index;
            Link_Script.i.Char_Level = _Char_Level;
            Link_Script.i.Main_Gun_Index = _Main_Gun_Index;
            Link_Script.i.Sub_Gun_Index = _Sub_Gun_Index;
            Link_Script.i.Char_Move_Speed = SendManager.Instance.Get_ReinfCharacter((uint)Link_Script.i.Char_Index, Link_Script.i.Char_Level).MvSpeed;

            //지정된 캐릭터가 소유한 총의 모든 정보
            //Link_Script.i.Get_Char_ALL_Gun_Data = SendManager.Instance.Get_userWeapons(Link_Script.i.Char_Index);
            Link_Script.i.Get_Char_ALL_Gun_Data = SendManager.Instance.Get_userWeapons();

            //지정된 캐릭터가 선택하고 있는 주,보조 무기 인덱스 셋팅
            Link_Script.i.Get_ALL_Char_Data[(uint)Link_Script.i.Char_Index].MainWpnIdx = (uint)Link_Script.i.Main_Gun_Index;
            Link_Script.i.Get_ALL_Char_Data[(uint)Link_Script.i.Char_Index].SubWpnIdx = (uint)Link_Script.i.Sub_Gun_Index;

            //지정된 캐릭터의 커스텀 정보 수정
            Link_Script.i.Costume_Kind_1 = Link_Script.i.Get_ALL_Char_Data[(uint)Link_Script.i.Char_Index].DecoIdx1;
            Link_Script.i.Costume_Kind_2 = Link_Script.i.Get_ALL_Char_Data[(uint)Link_Script.i.Char_Index].DecoIdx2;
            Link_Script.i.Costume_Kind_3 = Link_Script.i.Get_ALL_Char_Data[(uint)Link_Script.i.Char_Index].DecoIdx3;

            Char_Skill_Now_Send_Check = true;
        }
        else
        {
            //네트웍 캐릭터가 리스폰 되자 마자 캐릭터 이미지 셋팅하기 위해 패킷받으면 인덱스 부터 바로 셋팅해준다.(추가)
            Char_Index = _Char_Index;
        }

        Respawn_HP = _Respawn_HP;
        Respawn_Pos_Index = _Respawn_Pos_Index;

        Die_State = DIE_STATE.RESPAWN_INIT;
    }

    //==========================================================================================================================================================================================================
        
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
            Ragdoll_Rigidbody[Ragdoll_Collider_Name[i]].Rigid_Body.isKinematic = false;
            Ragdoll_Rigidbody[Ragdoll_Collider_Name[i]].GameObject_OJ.layer = Ragdoll_Die_Layer;
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
                Ragdoll_Rigidbody[Ragdoll_Collider_Name[i]].Rigid_Body.AddExplosionForce(Explosion_Force, Die_Atk_Start_Vec, Explosion_Radius, Explosion_Up, ForceMode.Impulse);
            }
        }
        else//일반 총 렉돌
        {
            for (int i = 0; i < Ragdoll_Collider_Name.Length; i++)
            {
                Ragdoll_Rigidbody[Ragdoll_Collider_Name[i]].Rigid_Body.AddForceAtPosition((Die_Atk_Target_Vec - Die_Atk_Start_Vec).normalized * Force, Die_Atk_Target_Vec);
            }
        }
    }

    //==========================================================================================================================================================================================================

    //리로드 체크
    bool Reload_Check()
    {
        if (Die_State != DIE_STATE.IDEL) return false;

        if (Gun_Bullet[(int)Gun_Equip_State] == 0)
        {
            //리로드 시작
            Reload_Start();           

            return true;
        }

        return false;
    }

    //리로드 시작
    public void Reload_Start()
    {
        if (Gun_Reload_State[(int)Gun_Equip_State] == RELOAD_STATE.IDEL && Gun_Bullet[(int)Gun_Equip_State] < Gun_Bullet_MAX[(int)Gun_Equip_State])
        {
            Gun_Reload_State[(int)Gun_Equip_State] = RELOAD_STATE.RELOAD_INIT;
        }
    }
        
    //리로드 애니메이션 끝나는 체크
    void Reload_End_Check_Operation()
    {
        if (Char_User_Kind == CHAR_USER_KIND.NETWORK || Die_State != DIE_STATE.IDEL) return;

        for (int i = 0; i < Gun_Reload_State.Length; i++)
        {
            switch (Gun_Reload_State[i])
            {
                case RELOAD_STATE.IDEL:

                    //리로드시 카메라 위치 이동시키기
                    if (i == (int)Gun_Equip_State) CameraControlScript.Camera_Move_Pos_Init(0);

                    //리로드 버튼 아이콘 애니메이션
                    UI_Script.Reload_UI_Update((GUN_EQUIP_STATE)i, ((Gun_Bullet[i] * 100.0f) / Gun_Bullet_MAX[i]) * 0.01f);
                    
                    break;
                case RELOAD_STATE.RELOAD_INIT:

                    Gun_Reload_Time[i] = Gun_Reload_MAX_Time[i];//리로드 시간 셋팅
                    Gun_Bullet[i] = 0;//남은 총알

                    if (UI_Script.Zoom_Ani_State == ZOOM_ANI_STATE.ZOOM_END)
                    {
                        UI_Script.Zoom_Ani_Init(ZOOM_ANI_STATE.ZOOM_OUT);

                        Char_View_Check(true);
                    }

                    Animator_Play_Init(ANISTATE.RELOAD[(int)Gun_Type]);

                    Gun_Reload_State[i] = RELOAD_STATE.RELOAD;

                    break;
                case RELOAD_STATE.RELOAD:

                    //리로드시 카메라 위치 이동시키기
                    if (i == (int)Gun_Equip_State) CameraControlScript.Camera_Move_Pos_Init(1);

                    //리로드 애니메이션중에 다른 애니메이션이 오면 그 애니메이션 처리해준다. 
                    if (
                        Animator_State.shortNameHash != ANISTATE.ATTACK[(int)GUN_TYPE.GRENADE] &&                        
                        Animator_State.shortNameHash != ANISTATE.ATTACK[(int)GUN_TYPE.LAUNCHER] &&
                        Animator_State.shortNameHash != ANISTATE.ATTACK[(int)GUN_TYPE.ROCKET_SKILL] && 
                        Animator_State.shortNameHash != ANISTATE.ATTACK_BOOM &&
                        Animator_State.shortNameHash != ANISTATE.ATTACK_RUSH &&
                        Animator_State.shortNameHash != ANISTATE.ATTACK_THROWSHOT &&
                        
                        
                        Animator_Play_Index != ANISTATE.ATTACK[(int)GUN_TYPE.GRENADE] &&                        
                        Animator_Play_Index != ANISTATE.ATTACK[(int)GUN_TYPE.LAUNCHER] &&
                        Animator_Play_Index != ANISTATE.ATTACK[(int)GUN_TYPE.ROCKET_SKILL] && 
                        Animator_Play_Index != ANISTATE.ATTACK_BOOM &&
                        Animator_Play_Index != ANISTATE.ATTACK_RUSH &&
                        Animator_Play_Index != ANISTATE.ATTACK_THROWSHOT
                        )
                    {
                        if ((Gun_Equip_State == GUN_EQUIP_STATE.MAIN && i == 0) || (Gun_Equip_State == GUN_EQUIP_STATE.SUB && i == 1))
                        {
                            Animator_Play_Init(ANISTATE.RELOAD[(int)Gun_Type]);
                        }                                              
                    }
                                        
                    //리로드 버튼 아이콘 애니메이션
                    UI_Script.Reload_UI_Update((GUN_EQUIP_STATE)i, 1 - (Gun_Reload_Time[i] / Gun_Reload_MAX_Time[i]));

                    Gun_Reload_Time[i] -= Time.deltaTime;
                    if (Gun_Reload_Time[i] <= 0.0f)
                    {
                        Gun_Reload_Time[i] = 0.0f;
                        Gun_Reload_State[i] = RELOAD_STATE.RELOAD_END;
                    }

                    break;
                case RELOAD_STATE.RELOAD_END:

                    //리로드 버튼 아이콘 애니메이션
                    UI_Script.Reload_UI_Update((GUN_EQUIP_STATE)i, 1);

                    Gun_Bullet[i] = Gun_Bullet_MAX[i];//남은 총알            

                    //리로드가 끝났는데도 아직 플레이어가 총을 쏘고 있는 상태라면 총쏘는 첨 상태로 돌려준다.(총쏘는 애니메이션 플레이 하기 위해)
                    if (Char_Shot_State == CHAR_SHOT_STATE.SHOT_LAUNCH)
                    {
                        Char_Shot_State = CHAR_SHOT_STATE.ANI_START;
                    }
                    else
                    {
                        //현재 장착중인 무기의 리로드가 끝났다면 대기 모션으로 바꾼다.
                        if (i == (int)Gun_Equip_State) Animator_Play_Init(ANISTATE.UPPER_MOVE[(int)Gun_Type]);
                    }
                    
                    Gun_Reload_State[i] = RELOAD_STATE.IDEL;

                    break;
            }
        }
                        
        //탄환감소 UI 정보 갱신
        UI_Script.Gun_Bullet_Update(Gun_Bullet[0], Gun_Bullet[1]);
    }

    //==========================================================================================================================================================================================================

    //주,보조 무기 바꾸기
    public void Gun_Change(GUN_EQUIP_STATE _Gun_Equip_State)
    {        
        //현재 장착된 주,보조 무기 셋팅
        Gun_Equip_State = _Gun_Equip_State;

        //무기이미지 보이기 셋팅
        Weapon_OJ_Init(false, -1);

        //무기이미지 보이기 셋팅
        Weapon_OJ_Init(true, Gun_Index[(int)Gun_Equip_State]);

        //현재 장착된 무기의 타입 셋팅
        if (Gun_Equip_State == GUN_EQUIP_STATE.MAIN)
        {
            Gun_Type = (GUN_TYPE)Link_Script.i.Main_Gun_Type;

            UI_Script.Zoom_Button_Init(MainGun_Scope_Magni);            
        }
        else if (Gun_Equip_State == GUN_EQUIP_STATE.SUB)
        {
            Gun_Type = (GUN_TYPE)Link_Script.i.Sub_Gun_Type;

            UI_Script.Zoom_Button_Init(SubGun_Scope_Magni);
        }

        //if (Gun_Type == GUN_TYPE.SNIPER)
        //{
        //    //자동, 수동일때에 따른 샷버튼 이미지 그리기 셋팅
        //    UI_Script.Shot_Right_Button_View(true);

        //    UI_Script.Aim_Color_Change(AIM_COLOR_STATE.WHITE);
        //}
        //else
        //{
        //    //자동, 수동일때에 따른 샷버튼 이미지 그리기 셋팅
        //    UI_Script.Shot_Right_Button_View(!Game_Script.Option_Auto_Shot);
        //}

        //자동, 수동일때에 따른 샷버튼 이미지 그리기 셋팅
        UI_Script.Shot_Right_Button_View(!Game_Script.Option_Auto_Shot);

        if (UI_Script.Zoom_Ani_State == ZOOM_ANI_STATE.ZOOM_END)
        {
            UI_Script.Zoom_Ani_Init(ZOOM_ANI_STATE.ZOOM_OUT);

            Char_View_Check(true);
        }

        //장착된 무기가 리로드 중이라면 리로드 애니메이션 돌린다
        if (Gun_Reload_State[(int)Gun_Equip_State] == RELOAD_STATE.RELOAD)
        {
            Animator_Play_Init(ANISTATE.RELOAD[(int)Gun_Type]);
        }
        else
        {
            Animator_Play_Init(ANISTATE.UPPER_MOVE[(int)Gun_Type]);
        }
        
        //이전 총쏜 상태 리셋
        Char_Shot_State = CHAR_SHOT_STATE.IDEL;

        //더블 핸드 상태 리셋
        Double_Shot_State = DOUBLE_SHOOTING_STATE.IDEL;
    }
       
    //무기이미지 보이기 셋팅
    void Weapon_OJ_Init(bool View_Check, int Image_Index)
    {
        if (View_Check)
        {
            Weapon_OJ[Image_Index].OJ.SetActive(true);//오른손 무기 체크
            if (Weapon_OJ.ContainsKey(Image_Index + 100000)) Weapon_OJ[Image_Index + 100000].OJ.SetActive(true);//왼손 무기 체크
        }
        else
        {
            //오른손 무기 체크 
            for (int i = 0; i < Link_Script.i.W_Index.Length; i++)
            {
                Weapon_OJ[Link_Script.i.W_Index[i]].OJ.SetActive(false);               
            }

            //왼손 무기 체크
            for (int i = 0; i < Link_Script.i.W_Left_Index.Length; i++)
            {
                Weapon_OJ[Link_Script.i.W_Left_Index[i] + 100000].OJ.SetActive(false);
            }
        }
    }

    //==========================================================================================================================================================================================================

    //수류탄 생성하기
    public void Grenade_Make(GUN_TYPE _Gun_Type, bool Skill_Check)
    {
        int Make_Count = 0;
        float ForcePower = 0.0f;
        Vector3[] Start_Pos = new Vector3[4];
        Vector3[] Target_Pos = new Vector3[4];
        

        if (Skill_Check == false)//일반 수류탄, 유탄발사
        {
            //공격 애니메이션
            Animator_Play_Init(ANISTATE.ATTACK[(int)_Gun_Type]);

            Make_Count = 1;
            ForcePower = 1000.0f;
            
            //수류탄 날아갈 타겟 좌표 구하기
            if (_Gun_Type == GUN_TYPE.GRENADE)
            {
                Start_Pos[0] = transform.position + transform.TransformDirection(new Vector3(0.4f, 0.7f, 0.3f));                
            }
            else if (_Gun_Type == GUN_TYPE.LAUNCHER)
            {
                Start_Pos[0] = CameraControlScript.Camera_Launcher_Start_World_Pos;
            }
            else if (_Gun_Type == GUN_TYPE.ROCKET_SKILL)
            {
                Start_Pos[0] = CameraControlScript.Camera_Launcher_Start_World_Pos + transform.TransformDirection(new Vector3(0.3f, 0.3f, 0.0f));
            }
            
            if (_Gun_Type == GUN_TYPE.ROCKET_SKILL)
            {
                if (Physics.Raycast(CameraControlScript.Main_Camera.ScreenPointToRay(UI_Script.Spot_Image.position), out Shot_RaycastHit, Mathf.Infinity, RayCast_Layer))
                {
                    Target_Pos[0] = Shot_RaycastHit.point;
                }
            }            
            else
            {
                if (Physics.Raycast(Start_Pos[0], (CameraControlScript.Camera_Grenade_Aim_World_Pos - Start_Pos[0]).normalized, out Shot_RaycastHit, Mathf.Infinity, RayCast_Layer))
                {
                    Target_Pos[0] = Shot_RaycastHit.point;
                }
            }              
        }
        else//스킬 발사
        {
            if (_Gun_Type == GUN_TYPE.THROUGH_SHOT_SKILL)//관통샷
            {
                Make_Count = 1;
                ForcePower = 1000.0f;

                //시작 포인트
                Start_Pos[0] = CameraControlScript.Camera_Launcher_Start_World_Pos + transform.TransformDirection(new Vector3(0.0f, 0.5f, 0.2f));

                //도착 포인트
                Ray Camera_Ray = CameraControlScript.Main_Camera.ScreenPointToRay(UI_Script.Spot_Image.position);
                if (Physics.Raycast(Camera_Ray, out Shot_RaycastHit, Mathf.Infinity, RayCast_Layer))
                {
                    Target_Pos[0] = Shot_RaycastHit.point;

                    //캐릭터가 장애물에 붙어 있으면 시작위치보다 뒤쪽으로 있기때문에 거리체크후 강제로 앞으로 쏴준다.
                    if ((Shot_RaycastHit.point - Start_Pos[0]).magnitude < 10.0f)
                    {
                        //Target_Pos[0] = Camera_Ray.GetPoint(10.0f);

                        Start_Pos[0] = CameraControlScript.Camera_Launcher_Start_World_Pos;
                        Target_Pos[0] = CameraControlScript.Camera_Launcher_Start_Transform.TransformDirection(new Vector3(0.0f, 0.0f, 1000.0f));
                    }
                }
            }
            else if (_Gun_Type == GUN_TYPE.BOOMER_SKILL)//수류탄
            {
                Make_Count = 4;
                ForcePower = Random(300.0f, 500.0f);

                Start_Pos[0] = transform.position + transform.TransformDirection(new Vector3(0.1f, 1.0f, 0.0f));
                Start_Pos[1] = transform.position + transform.TransformDirection(new Vector3(-0.1f, 1.0f, 0.0f));
                Start_Pos[2] = transform.position + transform.TransformDirection(new Vector3(0.0f, 1.0f, 0.1f));
                Start_Pos[3] = transform.position + transform.TransformDirection(new Vector3(-0.1f, 1.0f, -0.1f));

                Target_Pos[0] = transform.position + transform.TransformDirection(new Vector3(1.0f, Random(1.0f, 2.0f), Random(1.0f, 2.0f)));
                Target_Pos[1] = transform.position + transform.TransformDirection(new Vector3(-1.0f, Random(1.0f, 2.0f), Random(1.0f, 2.0f)));
                Target_Pos[2] = transform.position + transform.TransformDirection(new Vector3(0.0f, Random(1.0f, 2.0f), Random(1.0f, 2.0f)));
                Target_Pos[3] = transform.position + transform.TransformDirection(new Vector3(-0.1f, Random(1.0f, 2.0f), Random(1.0f, 2.0f)));
            }            
        }

        for (int i = 0; i < Make_Count; i++)
        {
            //수류탄 객체 생성
            String OJ_Name = User_ID + "_" + Grenade_Count;
            Grenade_Count++;

            if (_Gun_Type == GUN_TYPE.GRENADE)
            {
                Instantiate(Resources.Load("Battle_Prefab/4_Grenade_Folder/Grenade") as GameObject).name = OJ_Name;
            }
            else if (_Gun_Type == GUN_TYPE.BOOMER_SKILL)
            {
                Instantiate(Resources.Load("Battle_Prefab/4_Grenade_Folder/Grenade_Boomer") as GameObject).name = OJ_Name;
            }
            else if (_Gun_Type == GUN_TYPE.LAUNCHER)
            {
                Instantiate(Resources.Load("Battle_Prefab/4_Grenade_Folder/Launchershell") as GameObject).name = OJ_Name;
            }
            else if (_Gun_Type == GUN_TYPE.ROCKET_SKILL)
            {
                Instantiate(Resources.Load("Battle_Prefab/4_Grenade_Folder/Rocketshell") as GameObject).name = OJ_Name;
            }
            else if (_Gun_Type == GUN_TYPE.THROUGH_SHOT_SKILL)
            {
                Instantiate(Resources.Load("Battle_Prefab/4_Grenade_Folder/Laser_Ball") as GameObject).name = OJ_Name;
            }

            Transform Grenade_Transform = GameObject.Find(OJ_Name).transform.GetComponent<Transform>();
            Grenade_Transform.rotation = CameraControlScript.Camera_Center_OJ_Transform().rotation;
            Grenade_Transform.SetParent(Link_Script.i.Effect_OJ_Set);

            Game_Script.Grenade_Data.Add(OJ_Name, Grenade_Transform.GetComponent<Grenade_Script>());

            Game_Script.Grenade_Data[OJ_Name].Grenade_User_Kind = CHAR_USER_KIND.PLAYER;
            Game_Script.Grenade_Data[OJ_Name].User_ID = User_ID;
            Game_Script.Grenade_Data[OJ_Name].User_Team = User_Team;
            Game_Script.Grenade_Data[OJ_Name].OJ_Name = OJ_Name;
            Game_Script.Grenade_Data[OJ_Name].Gun_Type = _Gun_Type;
            Game_Script.Grenade_Data[OJ_Name].Start_Pos = Start_Pos[i];
            Game_Script.Grenade_Data[OJ_Name].Target_Pos = Target_Pos[i];

            if (_Gun_Type == GUN_TYPE.THROUGH_SHOT_SKILL)
            {
                Game_Script.Grenade_Data[OJ_Name].Explosion_Time_MAX = 10.0f;
            }
            else
            {
                if (Skill_Check == false)
                {
                    Game_Script.Grenade_Data[OJ_Name].Explosion_Time_MAX = 3.0f;
                }
                else
                {
                    Game_Script.Grenade_Data[OJ_Name].Explosion_Time_MAX = Random(2.5f, 3.5f);
                }  
            }                      

            Game_Script.Grenade_Data[OJ_Name].Grenade_Shot(ForcePower);


            if (_Gun_Type == GUN_TYPE.LAUNCHER || _Gun_Type == GUN_TYPE.ROCKET_SKILL)
            {
                //총 종류별 카메라 흔들림 연산
                CameraControlScript.Gun_Camera_Shake(_Gun_Type);
            }
        }        
    }

    //발사 종류 스킬 애니 끝났을때 무기 이미지 그려주는 체크
    void Skill_Shot_End_Check_Operation()
    {
        if (Animator_State.shortNameHash == ANISTATE.ATTACK[(int)GUN_TYPE.GRENADE] ||            
            Animator_State.shortNameHash == ANISTATE.ATTACK[(int)GUN_TYPE.LAUNCHER] ||
            Animator_State.shortNameHash == ANISTATE.ATTACK[(int)GUN_TYPE.ROCKET_SKILL] || 
            Animator_State.shortNameHash == ANISTATE.ATTACK_FLAME_THROWER ||
            Animator_State.shortNameHash == ANISTATE.ATTACK_THROWSHOT)
        {
            if (Animator_State.length > Animator_State.normalizedTime)
            {
                //무기이미지가 이미 꺼져있다면 리턴
                if (!Weapon_OJ[Gun_Index[(int)Gun_Equip_State]].OJ.activeSelf) return;

                //무기이미지 보이기 셋팅
                Weapon_OJ_Init(false, -1);

                //유탄발사기 무기 이미지 그리기
                if (Animator_Play_Index == ANISTATE.ATTACK[(int)GUN_TYPE.LAUNCHER]) Weapon_OJ[Link_Script.i.W_Launchar_Index].OJ.SetActive(true);

                //직사포 무기 이미지 그리기
                if (Animator_Play_Index == ANISTATE.ATTACK[(int)GUN_TYPE.ROCKET_SKILL]) Weapon_OJ[Link_Script.i.W_Rocket_Index].OJ.SetActive(true);

                //화염방사기 무기 이미지 그리기
                if (Animator_Play_Index == ANISTATE.ATTACK_FLAME_THROWER) Weapon_OJ[Link_Script.i.W_FlameThrower_Index].OJ.SetActive(true);

                //관통샷 무기 이미지 그리기
                if (Animator_Play_Index == ANISTATE.ATTACK_THROWSHOT) Weapon_OJ[Link_Script.i.W_ThroughShot_Index].OJ.SetActive(true);
            }
            else
            {
                if (Flamer_Time > 0.0f)
                {
                    Animator_Play_Init(ANISTATE.ATTACK_FLAME_THROWER);
                }
                else
                {
                    if (Char_User_Kind == CHAR_USER_KIND.PLAYER)
                    {
                        //장착된 무기 상태에 따른 애니메이션 셋팅
                        if (Gun_Reload_State[(int)Gun_Equip_State] == RELOAD_STATE.RELOAD)
                        {
                            Animator_Play_Init(ANISTATE.RELOAD[(int)Gun_Type]);
                        }
                        else
                        {
                            Animator_Play_Init(ANISTATE.UPPER_MOVE[(int)Gun_Type]);
                        }
                    }
                }
            }
        }
        else
        {
            //무기 이미지가 이미 켜져있다면 리턴
            if (Weapon_OJ[Gun_Index[(int)Gun_Equip_State]].OJ.activeSelf) return;

            if (Char_User_Kind == CHAR_USER_KIND.PLAYER)
            {
                if (UI_Script.Zoom_Ani_State == ZOOM_ANI_STATE.IDEL)
                {
                    //무기이미지 보이기 셋팅
                    Weapon_OJ_Init(true, Gun_Index[(int)Gun_Equip_State]);
                }
            }
            else
            {
                //무기이미지 보이기 셋팅
                Weapon_OJ_Init(true, Gun_Index[(int)Gun_Equip_State]);
            }            

            //유탄 무기 없애기
            Weapon_OJ[Link_Script.i.W_Launchar_Index].OJ.SetActive(false);

            //직사포 무기 이미지 그리기
            Weapon_OJ[Link_Script.i.W_Rocket_Index].OJ.SetActive(false);

            //화염 방사기 없애기
            Weapon_OJ[Link_Script.i.W_FlameThrower_Index].OJ.SetActive(false);

            //관통샷 무기 없애기
            Weapon_OJ[Link_Script.i.W_ThroughShot_Index].OJ.SetActive(false); 
        }
    }

    //==========================================================================================================================================================================================================
            
    //무적 상태 체크 연산
    void Barrier_Operation()
    {
        if (Barrier_Check == false)
        {
            if (User_Team < 50)
            {
                if (Barrier_OJ[User_Team].activeSelf) Barrier_OJ[User_Team].SetActive(Barrier_Check);
            }
            return;
        }

        if (Char_User_Kind == CHAR_USER_KIND.PLAYER)
        {
            Barrier_Time -= Time.deltaTime;
            if (Barrier_Time <= 0.0f)
            {
                Barrier_Check = false;
                Barrier_Time = 0.0f;

                //무적 상태 오브젝트 
                if (User_Team < 50) Barrier_OJ[User_Team].SetActive(Barrier_Check);

                Barrier_Net_Send_Time = 100.0f;
            }

            //초당 플레이어 캐릭터 방어막 쳐있는지 체크해주는 패킷
            Barrier_Net_Send_Time += Time.deltaTime;
            if (Barrier_Net_Send_Time >= 0.1f)
            {
                Barrier_Net_Send_Time = 0.0f;

                ByteData Send_Buffer = new ByteData(512, 0);
                Send_Buffer.InPutByte((byte)RELAY_PROTOCOL.BARRIER_CHECK);
                Send_Buffer.InPutByte(User_ID);
                Send_Buffer.InPutByte(Barrier_Check);
                Net_Script.Send_PTP_Data(Send_Buffer);
            }
        }
        else if (Char_User_Kind == CHAR_USER_KIND.NETWORK)
        {
            //무적 상태 오브젝트 
            if (User_Team < 50) Barrier_OJ[User_Team].SetActive(Barrier_Check);
        }
    }

    //무적 상태 체크
    public void Barrier_Recv_Data(ByteData _Receive_data)
    {
        _Receive_data.OutPutVariable(ref Barrier_Check);        
    }

    //==========================================================================================================================================================================================================

    //스킬, 수류탄 버튼 리로드, 플레이어 HP UI 연산
    void Skill_Grenade_Reload_Operation()
    {
        if (Char_User_Kind != CHAR_USER_KIND.PLAYER) return;

        //메인 스킬 버튼 아이콘 애니메이션
        UI_Script.Main_Skill_Button_UI_Update(1 - (Main_Skill_Reload_Time / Main_Skill_Reload_MAX_Time));

        //서브 스킬 버튼 아이콘 애니메이션
        UI_Script.Sub_Skill_Button_UI_Update(1 - (Sub_Skill_Reload_Time / Sub_Skill_Reload_MAX_Time));

        //수류탄 버튼 아이콘 애니메이션
        UI_Script.Grenade_Button_UI_Update(1 - (Grenade_Reload_Time / Grenade_Reload_MAX_Time));

        if (Main_Skill_Reload_Time > 0.0f)
        {
            Main_Skill_Reload_Time -= Time.deltaTime;
            if (Main_Skill_Reload_Time <= 0.0f) Main_Skill_Reload_Time = 0.0f;
        }

        if (Sub_Skill_Reload_Time > 0.0f)
        {
            Sub_Skill_Reload_Time -= Time.deltaTime;
            if (Sub_Skill_Reload_Time <= 0.0f) Sub_Skill_Reload_Time = 0.0f;
        }

        if (Grenade_Reload_Time > 0.0f)
        {
            Grenade_Reload_Time -= Time.deltaTime;
            if (Grenade_Reload_Time <= 0.0f) Grenade_Reload_Time = 0.0f;
        }

        UI_Script.MY_HP_UI_Init(MY_HP, ((MY_HP * 100.0f) / MY_MAX_HP) * 0.01f);
    }

    //지속형 스킬 시간 체크
    void Skill_Time_Operation()
    {
        if (Char_User_Kind == CHAR_USER_KIND.PLAYER)
        {
            //투시 스킬 타임
            if (Vision_Time > 0.0f)
            {
                Vision_Time -= Time.deltaTime;
                if (Vision_Time <= 0.0f)
                {
                    Vision_Time = 0.0f;

                    //상대편 투시 효과 셋팅
                    Vision_Init(false);
                }
            }

            //러쉬스킬 타임
            if (Roller_Time > 0.0f)
            {
                Roller_Time -= Time.deltaTime;

                Roller_Hit_Check_Time += Time.deltaTime;

                if (Roller_Time <= 0.0f)
                {
                    Roller_Time = 0.0f;
                    Roller_Hit_Check_Time = 0.0f;

                    Roller_OJ.SetActive(false);
                    
                    Animator_Play_Init(ANISTATE.UPPER_MOVE[(int)Gun_Type]);
                    
                    Char_Animator.Play(ANISTATE.UPPER_MOVE[(int)Gun_Type], 0);
                    Char_Animator.Play(ANISTATE.LOW_MOVE, 1);

                    Char_Skill_Now_Send_Check = true;
                }
                else
                {
                    Animator_Play_Init(ANISTATE.ATTACK_RUSH);

                    Char_Animator.Play(ANISTATE.ATTACK_RUSH, 0);
                    Char_Animator.Play(ANISTATE.ATTACK_RUSH, 1);
                }
            }

            //전면 방어 타임
            if (Guard_Time > 0.0f)
            {
                Guard_Time -= Time.deltaTime;
                if (Guard_Time <= 0.0f)
                {
                    Guard_Time = 0.0f;

                    Guard_OJ.SetActive(false);
                    Guard_BoxCollider.enabled = false;

                    Char_Skill_Now_Send_Check = true;
                }
            }

            //화염방사기 타임
            if (Flamer_Time > 0.0f)
            {
                Flamer_Time -= Time.deltaTime;
                if (Flamer_Time <= 0.0f)
                {
                    Flamer_Time = 0.0f;
                    Flamer_Hitting_Time = 0.0f;

                    Flamer_OJ.SetActive(false);
                    Flamer_Local_OJ.SetActive(false);

                    Char_Skill_Now_Send_Check = true;
                }
                else
                {
                    //화염방사기 충돌 체크
                    FlameThrower_Hit_Operation();
                }
            }

            //4방향 폭탄 던지는 시작 시작 체크
            if (Boomer_Start_Time > 0.0f)
            {
                Boomer_Start_Time -= Time.deltaTime;
                if (Boomer_Start_Time <= 0.0f)
                {
                    Boomer_Start_Time = 0.0f;
                    Grenade_Make(GUN_TYPE.BOOMER_SKILL, true);
                }
            }
            else
            {
                //4방향 폭탄 던지는 애니메이션 끝났다면 대기 자세로 변환
                if (Animator_State.shortNameHash == ANISTATE.ATTACK_BOOM && Animator_State.length <= Animator_State.normalizedTime)
                {
                    Animator_Play_Init(ANISTATE.UPPER_MOVE[(int)Gun_Type]);
                }
            }

            //날아다니기
            if (Flying_Start_Time > 0.0f)
            {
                Flying_Start_Time -= Time.deltaTime;
                if (Flying_Start_Time <= 0.0f)
                {
                    Flying_Start_Time = 0.0f;

                    Flying_Skill_OJ.SetActive(false);

                    Char_Skill_Now_Send_Check = true;
                }
            }

            //크리티컬 100%
            if (Rage_Time > 0.0f)
            {
                Rage_Time -= Time.deltaTime;
                if (Rage_Time <= 0.0f)
                {
                    Rage_Time = 0.0f;

                    Rage_Skill_OJ.SetActive(false);

                    Char_Skill_Now_Send_Check = true;
                }
            }

            //관통샷 시작 시작 체크
            if (ThroughShot_Start_Time > 0.0f)
            {
                ThroughShot_Start_Time -= Time.deltaTime;
                if (ThroughShot_Start_Time <= 0.0f)
                {
                    ThroughShot_Start_Time = 0.0f;
                    Grenade_Make(GUN_TYPE.THROUGH_SHOT_SKILL, true);

                    //총 종류별 카메라 흔들림 연산
                    CameraControlScript.Gun_Camera_Shake(GUN_TYPE.THROUGH_SHOT_SKILL);
                }
            }
            else
            {
                //4방향 폭탄 던지는 애니메이션 끝났다면 대기 자세로 변환
                if (Animator_State.shortNameHash == ANISTATE.ATTACK_THROWSHOT && Animator_State.length <= Animator_State.normalizedTime)
                {
                    Animator_Play_Init(ANISTATE.UPPER_MOVE[(int)Gun_Type]);
                }
            }
        }
        else if (Char_User_Kind == CHAR_USER_KIND.NETWORK && Die_State == DIE_STATE.IDEL)
        {
            if (NormalJump_Net_Check)
            {
                NormalJump_Net_Check = false;

                //사운드 재생
                SendManager.Instance.PlayGameSound(AUDIOSOURCE_TYPE.AUDIO_3D, 23, Audio_Source);
            }

            //러쉬스킬 타임
            if (Roller_Time > 0.0f)
            {
                if (!Roller_OJ.activeSelf)
                {
                    Roller_OJ.SetActive(true);

                    Char_Animator.Play(ANISTATE.ATTACK_RUSH, 0);
                    Char_Animator.Play(ANISTATE.ATTACK_RUSH, 1);
                }
            }
            else
            {
                if (Roller_OJ.activeSelf)
                {
                    Roller_OJ.SetActive(false);

                    Char_Animator.Play(ANISTATE.UPPER_MOVE[(int)Gun_Type], 0);
                    Char_Animator.Play(ANISTATE.LOW_MOVE, 1);
                }
            }

            //전면 방어 타임
            if (Guard_Time > 0.0f)
            {
                if (!Guard_OJ.activeSelf)
                {
                    Guard_OJ.SetActive(true);
                    Guard_BoxCollider.enabled = true;
                }
            }
            else
            {
                if (Guard_OJ.activeSelf)
                {
                    Guard_OJ.SetActive(false);
                    Guard_BoxCollider.enabled = false;
                }
            }

            //화염방사기 타임
            if (Flamer_Time > 0.0f)
            {
                if (!Flamer_OJ.activeSelf)
                {
                   Flamer_OJ.SetActive(true);
                   Flamer_Local_OJ.SetActive(true);

                   Flamer_PA.Play();
                   Flamer_Local_PA.Stop();
                }
            }
            else
            {
                if (Flamer_OJ.activeSelf)
                {
                    Flamer_OJ.SetActive(false);
                    Flamer_Local_OJ.SetActive(false);

                    Flamer_PA.Stop();
                    Flamer_Local_PA.Stop();
                }
            }

            //슈퍼 점프 임펙트
            if (SuperJump_Net_Check)
            {
                SuperJump_OJ.SetActive(true);
            }
            else
            {
                SuperJump_OJ.SetActive(false);
            }
            
            //날아다니기
            if (Flying_Start_Time > 0.0f)
            {
                if (!Flying_Skill_OJ.activeSelf) Flying_Skill_OJ.SetActive(true);
            }
            else
            {
                if (Flying_Skill_OJ.activeSelf) Flying_Skill_OJ.SetActive(false);
            }

            //크리티컬 100%
            if (Rage_Time > 0.0f)
            {
                if (!Rage_Skill_OJ.activeSelf) Rage_Skill_OJ.SetActive(true);
            }
            else
            {
                if (Rage_Skill_OJ.activeSelf) Rage_Skill_OJ.SetActive(false);
            }
        }
    }

    //==========================================================================================================================================================================================================

    //벽에 총알 튀는 임펙트
    void BulletMark_Init(Vector3 _Target_Pos, Vector3 _Target_Dir)
    {
        //연기효과
        Transform BulletMark_OJ = Instantiate(Resources.Load("Battle_Prefab/3_Effect_Folder/Effect_Bulletmark") as GameObject).GetComponent<Transform>();
        BulletMark_OJ.transform.SetParent(Link_Script.i.Effect_OJ_Set);
        BulletMark_OJ.position = _Target_Pos;
        BulletMark_OJ.LookAt(transform.position);

        //총알자국
        GameObject BulletHole_OJ = Instantiate(Resources.Load("Battle_Prefab/3_Effect_Folder/BulletHole") as GameObject);
        BulletHole_OJ.transform.SetParent(Link_Script.i.Effect_OJ_Set);
        BulletHole_OJ.transform.position = _Target_Pos;
        BulletHole_OJ.transform.rotation = Quaternion.FromToRotation(Vector3.forward, _Target_Dir);
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

    //총알 날아가는 레이저 만들기
    IEnumerator ShowLaserBeam()
    {
        if (BulletMark_Check)
        {
            //벽에 총알 튀는 임펙트
            BulletMark_Init(Shot_Aim_Pos, BulletMark_Dir);
        }

        GameObject LaserBeam_OJ = Instantiate(Resources.Load("Battle_Prefab/3_Effect_Folder/LaserBeam") as GameObject);
        LineRenderer LineRenderer_OJ = LaserBeam_OJ.GetComponent<LineRenderer>();
        LaserBeam_OJ.transform.SetParent(Link_Script.i.Effect_OJ_Set);

        Vector3[] LineVec = new Vector3[2];

        if (Char_User_Kind == CHAR_USER_KIND.PLAYER)
        {
            if (UI_Script.Zoom_Ani_State == ZOOM_ANI_STATE.IDEL)
            {
                LineVec[0] = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
            }
            else//줌인 상태에서는 카메라에서 총알 쏴준다.
            {
                LineVec[0] = CameraControlScript.Camera_Pos;
            }
        }
        else
        {
            LineVec[0] = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        }        
        
        LineVec[1] = Shot_Aim_Pos;
        LineRenderer_OJ.SetPositions(LineVec);

        bool _Check = true;
        while (_Check)
        {
            yield return new WaitForSeconds(Time.deltaTime);

            LineVec[0] = Vector3.Lerp(LineVec[0], LineVec[1], Time.deltaTime * 20.0f);

            LineRenderer_OJ.SetPositions(LineVec);

            if ((LineVec[0] - LineVec[1]).magnitude < 0.1f)
            {
                Destroy(LineRenderer_OJ.gameObject);

                _Check = false;

                yield return null;
            }
        }
    }

    //==========================================================================================================================================================================================================

    float Scale_Result = 0.0f;

    //캐릭터 머리위에 정보 텍스트 연산
    void Head_Info_Text_Operation()
    {
        if (Char_User_Kind == CHAR_USER_KIND.PLAYER)
        {
            Head_Info_Text.enabled = false;
            return;
        }

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
            if (Game_Script.Char_Script[Link_Script.i.User_ID].Vision_Time == 0.0f)
            {
                if (!IsInView(Head_Info_Transform.position, Text_Pos, Object_Name, true)) Head_Info_Text.enabled = false;
            }
            else
            {                
                if (!IsInView(Head_Info_Transform.position, Text_Pos, Object_Name, false)) Head_Info_Text.enabled = false;                
            }            
        }

        //Head_Info_Text.text = "[Lv." + Char_Level + "] " + User_NickName + " [HP : " + MY_HP + "]";
        Head_Info_Text.text = "[Lv." + Char_Level + "] " + User_NickName;
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

    //게임 오버시 캐릭터별 셋팅
    public void GameOver_Char_Init()
    {
        Head_Info_Text.enabled = false;
    }

    //==========================================================================================================================================================================================================

    public void Char_Now_Destroy()
    {
        try
        {
            if (Char_Coroutine != null) StopCoroutine(Char_Coroutine);
            if (Char_Skill_Coroutine != null) StopCoroutine(Char_Skill_Coroutine);

            Destroy(Head_Info_Transform.gameObject);

            Destroy(gameObject);
        }
        catch (Exception ex)
        {
            Debug.LogError("ex : " + ex);
        }
    }

    public void Network_Disconnect_Die()
    {                 
        Disconnect_Die_State = DISCONNECT_DIE_STATE.DIE_ANI_INIT;
    }
    
    void Network_Disconnect_Operation()
    {
        if (Disconnect_Die_State == DISCONNECT_DIE_STATE.DIE_ANI_INIT)
        {
            Char_Animator.enabled = false;

            foreach (var _Grenade_Data in Game_Script.Grenade_Data.ToList())
            {
                if (_Grenade_Data.Value.User_ID == User_ID)
                {
                    _Grenade_Data.Value.OJ_Destroy();
                }
            }

            //캐릭터 죽을때 현재 스킬등 변수값 초기화
            Char_Die_Skill_Init();

            for (int i = 0; i < Ragdoll_Collider_Name.Length; i++)
            {
                Ragdoll_Rigidbody[Ragdoll_Collider_Name[i]].Rigid_Body.isKinematic = false;
                Ragdoll_Rigidbody[Ragdoll_Collider_Name[i]].GameObject_OJ.layer = Ragdoll_Die_Layer;
            }

            //무적 상태 오브젝트 
            if (User_Team < 50) Barrier_OJ[User_Team].SetActive(false);

            StartCoroutine(Network_Disconnect_Die_Start());
        }
    }

    IEnumerator Network_Disconnect_Die_Start()
    {
        yield return new WaitForSeconds(3.0f);

        Char_Now_Destroy();
    }

    //==========================================================================================================================================================================================================
    
    //애니메이션 상태값 셋팅
    void Animator_State_Init()
    {
        if (!Char_Animator.enabled) return;

        Animator_State = Char_Animator.GetCurrentAnimatorStateInfo(0);

        //LateUpdate()함수에서 애니플레이 시키고 Animator_Play_Check = false를 시켜도
        //터치이벤트발생시 불리는 함수는 LateUpdate() 뒤에 호출 되므로
        //Animator_Play_Check 의 값이 true로 바뀔수가 있기때문에
        //false 일때는 터치이벤트 함수호출로 인한 애니메이션 발생이 없다고 판단한다.
        if (Animator_Play_Check == false) Animator_Play_Index = Animator_State.shortNameHash;        
    }

    //애니메이션 플레이 시키기
    void Animator_Play_Operation()
    {
        if (!Char_Animator.enabled || !Animator_Play_Check) return;

        Char_Animator.Play(Animator_Play_Index, 0);

        Animator_Play_Check = false;

        if (Char_User_Kind == CHAR_USER_KIND.PLAYER)
        {
            Animator_Change_Check = true;
            Char_Skill_Now_Send_Check = true;
        }
    }

    void Animator_Play_Init(int Ani_Kind)
    {
        Animator_Play_Check = true;
        Animator_Play_Index = Ani_Kind;
    }

    //캐릭터 하반신 움직임
    public void Char_Low_Move(float Xpos, float Zpos)
    {       
        Char_Animator.SetFloat(ANISTATE.XPOS, Xpos);
        Char_Animator.SetFloat(ANISTATE.ZPOS, Zpos);
    }

    //캐릭터 하반신 움직임 속도
    void Char_Low_Speed(float Speed)
    {
        Char_Animator.SetFloat(ANISTATE.LOW_SPEED, Speed);
    }

    //================================================================================================================================================================

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

    //a값부터 (b - 1)까지
    int Random(int a, int b)
    {
        return UnityEngine.Random.Range(a, b);
    }

    float Random(float a, float b)
    {
        return UnityEngine.Random.Range(a, b);
    }

    bool Probability(float _probability)
    {
        if (Random(0.0f, 100.0f) < _probability) return true;

        return false;
    }

    float ColorResult(int _Color)
    {
        return (float)(_Color / 255.0f);
    }

    //==========================================================================================================================================================================================================

}


public class ANISTATE
{
    //애니메이션 변수
    public static int XPOS = Animator.StringToHash("Move_X");
    public static int ZPOS = Animator.StringToHash("Move_Z");
    public static int LOW_SPEED = Animator.StringToHash("Low_Speed");
    public static int UPPER_SPEED = Animator.StringToHash("Upper_Speed");


    //상반신 대기 애니메이션
    public static int[] UPPER_MOVE = { 
                                         Animator.StringToHash("Upper_Move_Pistols"), Animator.StringToHash("Upper_Move_Revolver"), Animator.StringToHash("Upper_Move_DualPistols"), 
                                         Animator.StringToHash("Upper_Move_PumpShotgun") ,Animator.StringToHash("Upper_Move_PumpShotgun") ,Animator.StringToHash("Upper_Move_Rifle"), 
                                         Animator.StringToHash("Upper_Move_SubMachinGun"), Animator.StringToHash("Upper_Move_Heavy_MachinGun"), Animator.StringToHash("Upper_Move_Sniper") ,
                                         Animator.StringToHash("Upper_Move_Grenade"), Animator.StringToHash("Upper_Move_Launcher")
                                     };

    //공격 대기 애니메이션
    public static int[] ATTACK = { 
                                         Animator.StringToHash("ATTACK_Pistols"), Animator.StringToHash("ATTACK_Revolver"), Animator.StringToHash("ATTACK_DualPistols"), 
                                         Animator.StringToHash("ATTACK_PumpShotgun") ,Animator.StringToHash("ATTACK_PumpShotgun") ,Animator.StringToHash("ATTACK_Rifle"), 
                                         Animator.StringToHash("ATTACK_SubMachinGun"),  Animator.StringToHash("ATTACK_Heavy_MachinGun"), Animator.StringToHash("ATTACK_Sniper") ,
                                         Animator.StringToHash("ATTACK_Grenade"), Animator.StringToHash("ATTACK_Launcher"), 
                                         Animator.StringToHash(""), Animator.StringToHash(""), Animator.StringToHash(""), 
                                         Animator.StringToHash("ATTACK_Rocket")
                                     };

    //리로드 대기 애니메이션
    public static int[] RELOAD = { 
                                         Animator.StringToHash("RELOAD_Pistols"), Animator.StringToHash("RELOAD_Revolver"), Animator.StringToHash("RELOAD_DualPistols"), 
                                         Animator.StringToHash("RELOAD_PumpShotgun") ,Animator.StringToHash("RELOAD_PumpShotgun") ,Animator.StringToHash("RELOAD_Rifle"), 
                                         Animator.StringToHash("RELOAD_SubMachinGun"),Animator.StringToHash("RELOAD_Heavy_MachinGun"), Animator.StringToHash("RELOAD_Sniper") ,
                                         Animator.StringToHash("RELOAD_Grenade"),  Animator.StringToHash("RELOAD_Launcher")
                                     };
    
    //하반신 대기 애니메이션
    public static int LOW_MOVE = Animator.StringToHash("Low_Move");

    public static int SIGHT = Animator.StringToHash("SIGHT");
    public static int ATTACK_FLAME_THROWER = Animator.StringToHash("ATTACK_FlameThrower");
    public static int ATTACK_BOOM = Animator.StringToHash("ATTACK_Boom");
    public static int ATTACK_RUSH = Animator.StringToHash("ATTACK_Rush");
    public static int ATTACK_THROWSHOT = Animator.StringToHash("ATTACK_Throwshot");    
    
    public static int DIE = Animator.StringToHash("DIE");    
}