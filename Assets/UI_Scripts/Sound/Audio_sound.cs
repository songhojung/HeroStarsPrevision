using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class Audio_sound : MonoBehaviour, IPointerDownHandler ,IPointerClickHandler
{
	public AUDIOSOUND_TYPE Audio_Type = AUDIOSOUND_TYPE.NONE;
	public int Audio_index = 0;



	void Start()
	{

		
		
		
	}

	public void OnPointerDown(PointerEventData eventdata)
	{
		
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		switch (Audio_Type)
		{
			case AUDIOSOUND_TYPE.VOICE:

				Set_PlaySound(Audio_index);

				break;
			case AUDIOSOUND_TYPE.EFFECT:

				Set_PlayEffect(Audio_index);

				break;
			case AUDIOSOUND_TYPE.MUSIC:
				//button.onClick.AddListener(() => Set_PlayMusic(Audio_index));
				//Set_PlayMusic(Audio_index);
				break;
		}
	}


	 void Set_PlaySound(int _audioIdx)
	{
		SoundExecuter.Getsingleton.PlaySound((int)_audioIdx);
	}

	 void Set_PlaySound(int _audioIdx , bool ison)
	{
		if(ison)
		SoundExecuter.Getsingleton.PlaySound((int)_audioIdx);
	}

	 void Set_PlayEffect(int _audioIdx)
	{
		SoundExecuter.Getsingleton.PlayEffect((int)_audioIdx);
	}

	 void Set_PlayEffect(int _audioIdx, bool ison)
	{
		if (ison)
		SoundExecuter.Getsingleton.PlayEffect((int)_audioIdx);
	}

	

}
