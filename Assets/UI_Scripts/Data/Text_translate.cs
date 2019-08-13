using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Text_translate : MonoBehaviour 
{
	private Text text;
	public int Text_Index ;
	private LanguageManager _langManager ;

	void Awake()
	{
		text = GetComponent<Text>();
		_langManager = LanguageManager.Getsingleton;
		_langManager.list_Tranlatelst.Add(this);

		if (TextDataManager.Dic_TranslateText.ContainsKey(Text_Index))
			text.text = TextDataManager.Dic_TranslateText[Text_Index];
	}
	void Start()
	{

	}




	public void OnDestroy()
	{
		_langManager.list_Tranlatelst.Remove(this);
	}




	public void Refresh()
	{


		text.text = TextDataManager.Dic_TranslateText[Text_Index];
	}

	
	
	
	
	
	
}
