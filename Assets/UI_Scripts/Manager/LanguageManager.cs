using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour 
{
	public LanguageCode language = LanguageCode.EN;

	[HideInInspector]
	public List<Text_translate> list_Tranlatelst = new List<Text_translate>();

	private static LanguageManager _instance;

	public static LanguageManager Getsingleton
	{
		get
		{
			if (_instance == null)
			{
				_instance = GameObject.FindObjectOfType<LanguageManager>();
				if (_instance == null)
				{
					_instance = new GameObject("LanguageManager").AddComponent<LanguageManager>();
					DontDestroyOnLoad(_instance);
				}
			}
			return _instance;

		}
	}

	void Awake()
	{
		//_instance = this;
		//DontDestroyOnLoad(this);
	}

	 

	public void ResponseButton_Language(LanguageCode _lang)
	{
		

	}

	public void SetLanguage(LanguageCode _lang)
	{
		TextDataManager.GetTextData(_lang);

		//for (int i = 0; i < list_Tranlatelst.Count; i++)
		//	list_Tranlatelst[i].Refresh();
	}

	public void SetLanguageRefresh()
	{
		for (int i = 0; i < list_Tranlatelst.Count; i++)
			list_Tranlatelst[i].Refresh();
	}

}
