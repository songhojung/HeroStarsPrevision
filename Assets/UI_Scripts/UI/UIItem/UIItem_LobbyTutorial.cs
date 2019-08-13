using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIItem_LobbyTutorial : MonoBehaviour 
{
	public int TutoType = 0;

	public void ResponseButton_Backround(int tutoType)
	{
		//if (!UserEditor.Getsingleton.isDoTestTutorial)
		//{
		//    if (tutoType == 1)
		//    {
		//        PlayerPrefs.SetInt(DefineKey.LobbyTuto_1, tutoType);
		//        TutoType = tutoType;
		//    }
		//    else if (tutoType == 2)
		//    {
		//        PlayerPrefs.SetInt(DefineKey.LobbyTuto_2, tutoType);
		//        TutoType = tutoType;
		//    }
		//    PlayerPrefs.Save();
		//}

		////리더 다시선택할수 있게 false 하자
		//UI_Lobby.Getsingleton.isSelectedCharacter = false;

		//Destroy(this.gameObject);
	}

}
