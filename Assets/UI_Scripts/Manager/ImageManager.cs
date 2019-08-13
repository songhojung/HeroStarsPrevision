using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;

public class ImageManager 
{

	public static ImageManager instance = new ImageManager();

	//== 이미지 스프라이트
	public Sprite[] lst_sprites_UI;
	public Sprite[] lst_sprites_CharacterUI;
	public Sprite[] lst_sprites_ShopUI;
	public Sprite[] lst_sptires_ClanMarkUI;
	public Sprite[] lst_sptires_RankUI;
	public Sprite[] lst_sptires_EquipItemUI;

	public Dictionary<string, Sprite> Dic_AllSprites = new Dictionary<string, Sprite>();

	//== 캐릭터모델 프리팹
	private Dictionary<int, GameObject> Lst_LobbyCharacter = new Dictionary<int, GameObject>();

	public  ImageManager()
	{
		//LoadAll_sprites();
	}

	void LoadAll_sprites()
	{
		lst_sprites_UI = Resources.LoadAll<Sprite>("Image/SpritePacker_UI");
		lst_sprites_CharacterUI = Resources.LoadAll<Sprite>("Image/SpritePacker_character");
		lst_sprites_ShopUI = Resources.LoadAll<Sprite>("Image/SpritePacker_shop");
		lst_sptires_ClanMarkUI = Resources.LoadAll<Sprite>("Image/SpritePacker_clanmark");
	}


	public IEnumerator routine_spriteLoad()
	{


		if (Dic_AllSprites.Count <= 0)
		{
			UserEditor.Getsingleton.EditLog("이미지로드 시작@@");


			lst_sprites_UI = Resources.LoadAll<Sprite>("Image/SpritePacker_DynamicUI");

			for (int i = 0; i < lst_sprites_UI.Length; i++)
			{
				Dic_AllSprites[lst_sprites_UI[i].name] = lst_sprites_UI[i];
			}

			yield return null;

			//lst_sprites_CharacterUI = Resources.LoadAll<Sprite>("Image/SpritePacker_character");
			//for (int i = 0; i < lst_sprites_CharacterUI.Length; i++)
			//{
			//    Dic_AllSprites[lst_sprites_CharacterUI[i].name] = lst_sprites_CharacterUI[i];
			//}
			//yield return null;

			lst_sprites_ShopUI = Resources.LoadAll<Sprite>("Image/SpritePacker_shop");
			for (int i = 0; i < lst_sprites_ShopUI.Length; i++)
			{
				Dic_AllSprites[lst_sprites_ShopUI[i].name] = lst_sprites_ShopUI[i];
			}
			yield return null;

			lst_sptires_ClanMarkUI = Resources.LoadAll<Sprite>("Image/SpritePacker_clanmark");
			for (int i = 0; i < lst_sptires_ClanMarkUI.Length; i++)
			{
				Dic_AllSprites[lst_sptires_ClanMarkUI[i].name] = lst_sptires_ClanMarkUI[i];
			}
			yield return null;

			lst_sptires_RankUI = Resources.LoadAll<Sprite>("Image/SpritePacker_Rank");
			for (int i = 0; i < lst_sptires_RankUI.Length; i++)
			{
				Dic_AllSprites[lst_sptires_RankUI[i].name] = lst_sptires_RankUI[i];
			}
			yield return null;

			lst_sptires_EquipItemUI = Resources.LoadAll<Sprite>("Image/SpritePacker_EquipItem");
			for (int i = 0; i < lst_sptires_EquipItemUI.Length; i++)
			{
				Dic_AllSprites[lst_sptires_EquipItemUI[i].name] = lst_sptires_EquipItemUI[i];
			}
			yield return null;

		}
		else
		{
			UserEditor.Getsingleton.EditLog("이미 이미지 로드 함~~~~~@@");
			yield return null;
		}
		UserEditor.Getsingleton.EditLog("이미지로드 완료@@");
		//Loadmanager.instance.LoadingData(false);

	}

	/// <summary>
	/// 모든 스프라이트 팩커를 검색하여 해당 스프라이트를 반환합니다 (이미지이름)
	/// </summary>
	public Sprite Get_Sprite(string spriteName)
	{
        //Sprite _sprite = new Sprite();
        Sprite _sprite;

        if (Dic_AllSprites.ContainsKey(spriteName))
		{
			_sprite = Dic_AllSprites[spriteName];
		}
		else
		{
			_sprite = Resources.Load<Sprite>("Image/FLAG/" + spriteName);

			if (_sprite == null)
				Debug.LogError("Sprite is null, Please check in your resource file or Check sprite name");
			else
				Dic_AllSprites[spriteName] = _sprite;
		}

//        IEnumerable<Sprite> sp = lst_sprites_UI.Where(n => n.name == spriteName);

//        if (sp.Count() <= 0)
//        {
//            sp = lst_sprites_ShopUI.Where(n => n.name == spriteName);

//            if (sp.Count() <= 0)
//            {
//                sp = lst_sprites_CharacterUI.Where(n => n.name == spriteName);

//                if (sp.Count() <= 0)
//                {
//                    sp = lst_sptires_ClanMarkUI.Where(n => n.name == spriteName);

//                }
//            }

//        }

//        if (sp.Count() > 0)
//        {
//            _sprite = findSprite(sp);
//        }
//        else
//        {
//#if UNITY_EDITOR
//            Debug.LogError("Sprite is null, Please check in your resource file or Check sprite name");
//#endif
//        }

		return _sprite;
	}

	//국가코드 이미지 반환합니다.
	public Sprite Get_FlagSprite(byte[] code)
	{
		Sprite _sprite = null;
		string spName = Encoding.Default.GetString(code);
		if (Dic_AllSprites.ContainsKey(spName))
		{
			_sprite = Dic_AllSprites[spName];
		}
		else
		{
			_sprite = Resources.Load<Sprite>("Image/FLAG/" + spName);

			if (_sprite == null)
				Debug.LogError("Sprite is null, Please check in your resource file or Check sprite name");
			else
				Dic_AllSprites[spName] = _sprite;
		}
		return _sprite;
	}


	//국가코드 이미지 반환합니다.
	public Sprite Get_FlagSprite(string spName)
	{
		Sprite _sprite = null;

		if (string.IsNullOrEmpty(spName))
			spName = "kr";

		if (Dic_AllSprites.ContainsKey(spName))
		{
			_sprite = Dic_AllSprites[spName];
		}
		else
		{
			_sprite = Resources.Load<Sprite>("Image/FLAG/" + spName);

			if (_sprite == null)
				Debug.LogError("Sprite is null, Please check in your resource file or Check sprite name");
			else
				Dic_AllSprites[spName] = _sprite;
		}
		return _sprite;
	}

	Sprite findSprite(IEnumerable<Sprite> _sp)
	{
		Sprite _sprite = null;
		foreach (var item in _sp)
		{
			_sprite = item;
		}
		return _sprite;


	}


	public GameObject Get_CharacterModel(int Index)
	{
		GameObject newObj;
		if (!Lst_LobbyCharacter.ContainsKey(Index))
		{
			newObj = Resources.Load(string.Format("Prefebs/Lobbych/Lobbych_{0}", Index.ToString())) as GameObject;
		}
		else
		{
			newObj = Lst_LobbyCharacter[Index];
		}

		return newObj;	
	}
}
