using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class characterInfo : MonoBehaviour
{

	public enum EquipItemType
	{
		Head = 0,
		Body = 1,
		RWeapon = 2,
		LWeapon = 3,
		Over = 4,
		FACE = 5,

	}

	public enum CharacterEffect
	{
		NONE	 = 99,
		BuyEffect = 0,
		SetEffect = 1,

	}


	public int unitIdx = 0;
	public GameObject Obj_unlock;
	public GameObject buyCharacterEffect;
	public List<GameObject> Lst_CharacterEffects;
	private bool isHaveUnit = false;
	public bool isHave
	{
		set
		{

			isHaveUnit = value;
			Set_DoCharacterEffect();
		}
		get { return isHaveUnit; }
	}
	private bool isDoBuyEffect = false;

	private string TriggerAniName = "StartGesture_";
	private int aniRandIdx = 0;
	private Animator Ani;
	private AnimatorStateInfo currentState;				//현재구동중인 애니메이션
	private SkinnedMeshRenderer characterSkinMr;			//현재 캐릭터 스킨드메쉬

	public infos_unit unitInfos;
	private User_Units gainUserUnit;
	public User_Units userUnit
	{
		set
		{
			gainUserUnit = value;
		
			Apply_CharacterEquip();

            Active_SkinnedMeshMeterial();

        }
		get { return gainUserUnit;	}
	}


	//캐릭터메터리얼 관련
	public Transform BodyBipOBJ;
	private Dictionary<uint, GameObject> Dic_chrctSkinnedMesh = new Dictionary<uint, GameObject>();


	//아이템관련
	public List<Transform> Tr_Items;
	private Dictionary<uint, GameObject> Dic_ItemHead = new Dictionary<uint, GameObject>();
	private Dictionary<uint, GameObject> Dic_ItemFace = new Dictionary<uint, GameObject>();
	private Dictionary<uint, GameObject> Dic_ItemBody = new Dictionary<uint, GameObject>();
	private Dictionary<uint, GameObject> Dic_ItemRightWpn = new Dictionary<uint, GameObject>();
	private Dictionary<uint, GameObject> Dic_ItemLeftWpn = new Dictionary<uint, GameObject>();
	private Dictionary<uint, GameObject> Dic_OverOBJ = new Dictionary<uint, GameObject>();
	private const int OVER_OBJ_MAX = 10;				//캐릭터 부착물 최대 수


	void Start () 
	{

		Ani = GetComponent<Animator>();


			//Get_Items();

			//비소유시 메터리얼컬러변경
			//Change_meterialMainColor();

	}


	//캐릭 기초 세팅
	public void Init_chrctBasicInfo()
	{
		//오브젝트이름 바꾸기
		gameObject.name = string.Format("{0}{1}", gameObject.name, unitIdx);


		//캐릭터 스킨메터릴얼 체크
		Chk_ChrctSkinMeterial();

		//오브젝트에 붙어있는 아이템 정보 가져오기
		Chk_getItemInfo(Dic_ItemHead, EquipItemType.Head);
		Chk_getItemInfo(Dic_ItemFace, EquipItemType.FACE);
		Chk_getItemInfo(Dic_ItemBody, EquipItemType.Body);
		Chk_getItemInfo(Dic_ItemRightWpn, EquipItemType.RWeapon);
		Chk_getItemInfo(Dic_ItemLeftWpn, EquipItemType.LWeapon);
		Chk_getItemInfo(Dic_OverOBJ, EquipItemType.Over);


		if (gainUserUnit != null)
		{
			Apply_CharacterEquip();
		}
	}



    public void Init_chrctBasicInfo(User_Units _userUnit , infos_unit _infoUnit)
    {
        //오브젝트이름 바꾸기
        //gameObject.name = string.Format("{0}{1}", gameObject.name, unitIdx);

        gainUserUnit = _userUnit;
        unitInfos = _infoUnit;
        unitIdx = (int)gainUserUnit.Unitidx;


        //캐릭터 스킨메터릴얼 체크
        Chk_ChrctSkinMeterial();

        //오브젝트에 붙어있는 아이템 정보 가져오기
        Chk_getItemInfo(Dic_ItemHead, EquipItemType.Head);
        Chk_getItemInfo(Dic_ItemFace, EquipItemType.FACE);
        Chk_getItemInfo(Dic_ItemBody, EquipItemType.Body);
        Chk_getItemInfo(Dic_ItemRightWpn, EquipItemType.RWeapon);
        Chk_getItemInfo(Dic_ItemLeftWpn, EquipItemType.LWeapon);
        Chk_getItemInfo(Dic_OverOBJ, EquipItemType.Over);


        if (gainUserUnit != null)
        {
            Apply_CharacterEquip();
        }
    }


    public void Refresh_ChrctInfo(User_Units _userUnit, infos_unit _infoUnit)
    {

        gainUserUnit = _userUnit;
        unitInfos = _infoUnit;
        unitIdx = gainUserUnit != null ?(int)gainUserUnit.Unitidx : (int)unitInfos.UnitIdx;

        Apply_CharacterEquip();

        Active_SkinnedMeshMeterial();
    }


    //캐릭터 스킨메터릴얼 체크
    void Chk_ChrctSkinMeterial()
	{
		//모든 스킨Renderer 찾기
		SkinnedMeshRenderer[] skinnedMeshes = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();

		for (int i = 0; i < skinnedMeshes.Length; i++ )
		{
			string _name = skinnedMeshes[i].gameObject.name;
			string[] spName = _name.Split('_');

			uint _unitIdx = Convert.ToUInt32(spName[1]);

			Dic_chrctSkinnedMesh[_unitIdx] = skinnedMeshes[i].gameObject;
		}


		//해당된 스킨 켜기
		Active_SkinnedMeshMeterial();
	}


	void Active_SkinnedMeshMeterial()
	{
		foreach (var skin in Dic_chrctSkinnedMesh)
		{
			if (unitIdx == skin.Key)
				skin.Value.SetActive(true);
			else
				skin.Value.SetActive(false);
		}
	}



	//오브젝트에 붙어있는 아이템 정보 가져오기
	void Chk_getItemInfo(Dictionary<uint,GameObject> dicObj, EquipItemType type)
	{
		Transform[] trLst = null;
		Dictionary<uint, Infos_Weapon> dic_infosweapon = null;
		Dictionary<uint, Infos_Deco> dic_infosDeco = null;

		if (type == EquipItemType.FACE) 
			trLst = Tr_Items[(int)EquipItemType.Head].GetComponentsInChildren<Transform>();
		else
			trLst = Tr_Items[(int)type].GetComponentsInChildren<Transform>();


		//우선 아이템 다 false
		for (int i = 0; i < trLst.Length; i++)
		{
			if (type == EquipItemType.Head) break; // face 부분에서 확인후 setAtvie(false) 할테니 나가자 
			if (i == 0) continue;

			if (trLst[i].name.Contains("Item_") || trLst[i].name.Contains("Weapon_"))
				trLst[i].gameObject.SetActive(false);
		}

		if (type == EquipItemType.Head)
		{
			dic_infosDeco = TableDataManager.instance.Infos_Decos;
			foreach (var deco in dic_infosDeco)
			{
				for (int i = 0; i < trLst.Length; i++)
				{
					if (i == 0) continue;
					
					if(string.Equals(trLst[i].name,"Item_"+deco.Value.DecoIdx))
					{
						if (deco.Value.DecoPart == DECOPART_TYPE.HEAD)
						{
							trLst[i].gameObject.SetActive(false);
							dicObj[deco.Value.DecoIdx] = trLst[i].gameObject;
						}

					}
					
				}
			}
		}
		else if ( type == EquipItemType.FACE)
		{
			dic_infosDeco = TableDataManager.instance.Infos_Decos;
			foreach (var deco in dic_infosDeco)
			{
				for (int i = 0; i < trLst.Length; i++)
				{
					if (i == 0) continue;

					if (string.Equals(trLst[i].name, "Item_" + deco.Value.DecoIdx))
					{
						if (deco.Value.DecoPart == DECOPART_TYPE.FACE)
						{
							trLst[i].gameObject.SetActive(false);
							dicObj[deco.Value.DecoIdx] = trLst[i].gameObject;
						}
					}

				}
			}
		}
		else if (type == EquipItemType.Body)
		{
			dic_infosDeco = TableDataManager.instance.Infos_Decos;
			foreach (var deco in dic_infosDeco)
			{
				for (int i = 0; i < trLst.Length; i++)
				{
					if (i == 0) continue;

					if (string.Equals(trLst[i].name, "Item_" + deco.Value.DecoIdx))
					{
						trLst[i].gameObject.SetActive(false);
						dicObj[deco.Value.DecoIdx] = trLst[i].gameObject;
					}

				}
			}
		}
		else if (type == EquipItemType.RWeapon)
		{
			dic_infosweapon = TableDataManager.instance.Infos_weapons;
			foreach (var wpn in dic_infosweapon)
			{
				for (int i = 0; i < trLst.Length; i++)
				{
					if (i == 0) continue;

					
					if (string.Equals(trLst[i].name, "Weapon_" + wpn.Value.WpnIdx))
					{
						trLst[i].gameObject.SetActive(false);
						dicObj[wpn.Value.WpnIdx] = trLst[i].gameObject;
					}
					

				}
			}
		}
		else if (type == EquipItemType.LWeapon)
		{
			dic_infosweapon = TableDataManager.instance.Infos_weapons;
			foreach (var wpn in dic_infosweapon)
			{
				for (int i = 0; i < trLst.Length; i++)
				{
					if (i == 0) continue;
					

					if (string.Equals(trLst[i].name, "Weapon_" + wpn.Value.WpnIdx +"_1"))
					{
						trLst[i].gameObject.SetActive(false);
						dicObj[wpn.Value.WpnIdx] = trLst[i].gameObject;
					}

				}
			}
		}
		else if (type == EquipItemType.Over)
		{
			Dictionary<uint, infos_unit> dicInfoUnit = TableDataManager.instance.Infos_units;
			for (int i = 0; i < trLst.Length; i++)
			{
				if (i == 0) continue;

				foreach (var infoUnit in dicInfoUnit)
				{
					for (int h = 0; h < OVER_OBJ_MAX; h++ )
					{
						if (string.Equals(trLst[i].name, string.Format("ch_{0}_OverOJ_{1}", infoUnit.Value.UnitIdx, h)))
						{
							//dic에 담기
							dicObj[infoUnit.Value.UnitIdx] = trLst[i].gameObject;
						}

					}

				}
				//비활성 해놓기
				trLst[i].gameObject.SetActive(false);

			}
		}


		
	}



	public void Set_lockObj(bool isActive)
	{
		Obj_unlock.SetActive(isActive);
	}



	//이유닛이 현재 적용된 아이템들 적용하기
	public void Apply_CharacterEquip()
	{
		

		foreach (var itemHead in Dic_ItemHead)
		{
            if(gainUserUnit == null)
                itemHead.Value.SetActive(false);

            else if (itemHead.Key == gainUserUnit.DecoIdx1)
				itemHead.Value.SetActive(true);
			else
				itemHead.Value.SetActive(false);
		}

		foreach (var itemBody in Dic_ItemBody)
		{
            if (gainUserUnit == null)
                itemBody.Value.SetActive(false);
           else if (itemBody.Key == gainUserUnit.DecoIdx2)
				itemBody.Value.SetActive(true);
			else
				itemBody.Value.SetActive(false);
		}

		foreach (var itemFace in Dic_ItemFace)
		{
            if (gainUserUnit == null)
                itemFace.Value.SetActive(false);
           else if (itemFace.Key == gainUserUnit.DecoIdx3)
				itemFace.Value.SetActive(true);
			else
				itemFace.Value.SetActive(false);
		}

		foreach (var itemWpn in Dic_ItemRightWpn)
		{
            if (gainUserUnit == null)
                itemWpn.Value.SetActive(false);
           else if (itemWpn.Key == gainUserUnit.MainWpnIdx)
			{
				itemWpn.Value.SetActive(true);

				// 20700 ,20701 쌍권총이므로 왼쪽 총 활성화
				if (itemWpn.Key == 20700 || itemWpn.Key == 20701 || itemWpn.Key == 20702)
					Dic_ItemLeftWpn[itemWpn.Key].SetActive(true);

			}
			else
			{
				itemWpn.Value.SetActive(false);

				// 20700 ,20701 쌍권총이므로 왼쪽 총 비활성화
				if (itemWpn.Key == 20700 || itemWpn.Key == 20701 || itemWpn.Key == 20702)
					Dic_ItemLeftWpn[itemWpn.Key].SetActive(false);

			}
		}



		//머리 부착물 예외처리
        if(gainUserUnit!=  null)
		Apply_HeadAttechment((uint)gainUserUnit.DecoIdx1);
	

	}



	// 현재 유닛의 장비아이템 착용
	public void Apply_SelectItemCharacterEquip(EquipType equipType, uint ItemIdx)
	{
		Dictionary<uint, GameObject> dicObj = null;

		if (equipType == EquipType.Dress_HEAD)
		{
			dicObj = Dic_ItemHead;

			//머리 부착물 예외처리
			Apply_HeadAttechment(ItemIdx);
		}
		else if (equipType == EquipType.Dress_FACE) dicObj = Dic_ItemFace;
		else if (equipType == EquipType.Dress_BODY) dicObj = Dic_ItemBody;
		else if (equipType == EquipType.Main) dicObj = Dic_ItemRightWpn;
		else if (equipType == EquipType.Sub) dicObj = Dic_ItemRightWpn;

		if (dicObj != null)
		{


			foreach (var item in dicObj)
			{
				if (item.Key == ItemIdx)
				{
					item.Value.SetActive(true);
					if (equipType == EquipType.Main) //sub -> main으로 변경
					{
						// 20700 ,20701 쌍권총이므로 왼쪽 총 활성화
						if (item.Key == 20700 || item.Key == 20701 || item.Key == 20702)
							Dic_ItemLeftWpn[item.Key].SetActive(true);
					}
				}
				else
				{
					item.Value.SetActive(false);

					//if (equipType == EquipType.Sub)
					{
						// 20700 ,20701 쌍권총이므로 왼쪽 총 비활성화
						if (item.Key == 20700 || item.Key == 20701 || item.Key == 20702)
							Dic_ItemLeftWpn[item.Key].SetActive(false);
					}
				}

			

			}
		}
	}



	//머리 부착물 예외처리
	void Apply_HeadAttechment(uint _ItemIdx)
	{

		if (Dic_OverOBJ.ContainsKey((uint)unitIdx))
		{
			Dictionary<uint, Infos_Deco> _dicDeco = TableDataManager.instance.Infos_Decos;
			if (_dicDeco.ContainsKey(_ItemIdx))
			{
				//머리 부착물 데이터가 0이면 활성 1이면 비활성
				Dic_OverOBJ[(uint)unitIdx].SetActive(_dicDeco[_ItemIdx].AttmntActive == 0);
			}
			else // head 치장이 없다면 부착물활성 
			{
				Dic_OverOBJ[(uint)unitIdx].SetActive(true);
			}
		}
	}



	//착용된 아이템중 셋트장비인지 체크
	public SetBufKnd Chk_EquipSetBuf(bool isEffectActive)
	{
		Dictionary<ushort, Infos_SetBuf> infosSetbuf = TableDataManager.instance.Infos_SetBuffs;
		bool isSet = false;
		SetBufKnd setKind = SetBufKnd.NONE;
		if (isEffectActive == true)
		{

			if (gainUserUnit != null)
			{
				foreach (var sb in infosSetbuf)
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
		}


		//셋트이펙트 활성/비활성
		Activate_ChrcEffect(CharacterEffect.SetEffect, isSet);

		return setKind;
	}



	//터치시행동 애니메이션 state 이름
	 int[] TOUCHSTATE = { Animator.StringToHash("state1"),Animator.StringToHash("state2"), Animator.StringToHash("state3")};
	
	// 캐릭터 터치시 할 행동
	public void Do_toughBehavior()
	{
		currentState = Ani.GetCurrentAnimatorStateInfo(0);
		if (currentState.shortNameHash == TOUCHSTATE[0] || currentState.shortNameHash == TOUCHSTATE[1] || currentState.shortNameHash == TOUCHSTATE[2])
			return;
		else
		{
			aniRandIdx = UnityEngine.Random.Range(0, 3);
			Ani.SetTrigger(string.Format("{0}{1}", TriggerAniName, aniRandIdx));
		}
		
		
	}




	//비소유시 메터리얼컬러변경
	void Change_meterialMainColor()
	{
		if(characterSkinMr == null)
			characterSkinMr = GetComponentInChildren<SkinnedMeshRenderer>();

		if (isHaveUnit)
			characterSkinMr.materials[0].color = new Color(1, 1, 1);
		else
			characterSkinMr.materials[0].color = DefineKey.LightBlack;
	}


	//캐릭터에 붙은 이펙트이미지 설정
	void Set_DoCharacterEffect()
	{

		//if (isHaveUnit)
		//{
		//	//자물쇠 오브젝트 비활성
		//	Obj_unlock.SetActive(false);
		//}
		//else
		//{
		//	//자물쇠 오브젝트 비활성
		//	Obj_unlock.SetActive(true);
		//}

		if (isDoBuyEffect)
			Activate_ChrcEffect(CharacterEffect.BuyEffect,false);
	}


	//구매시 캐릭터이펙트 플레이
	public void playBuyEffect(float PlayWaitsecond)
	{
		//구입시 이펙트 플레이
		StartCoroutine(play_buyEffect(PlayWaitsecond));
	}

	IEnumerator play_buyEffect(float _PlayWaitsecond)
	{
		isDoBuyEffect = true;
		yield return new WaitForSeconds(_PlayWaitsecond);
		Activate_ChrcEffect(CharacterEffect.BuyEffect, true);
		yield return new WaitForSeconds(5f);
		Activate_ChrcEffect(CharacterEffect.BuyEffect, false);

	}



	//캐릭터 이펙트 활성/비활성
	void Activate_ChrcEffect(CharacterEffect effect, bool isActive)
	{
		Lst_CharacterEffects[(int)effect].SetActive(isActive);
	}
	
}
