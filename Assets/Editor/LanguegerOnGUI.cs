using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CustomEditor(typeof(LanguageManager))]
public class LanguegerOnGUI : Editor 
{
    public LanguageManager Languege_Manager;

    private LanguageCode Game_Languege = LanguageCode.KR;
	private LanguageCode bf_language = LanguageCode.NONE;


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
		Languege_Manager = LanguageManager.Getsingleton;
		
		Game_Languege = Languege_Manager.language;
		 if (bf_language != Game_Languege)
		 {

		     Languege_Manager.SetLanguage(Game_Languege);
		     bf_language = Game_Languege;
		 }

    }

	
}
