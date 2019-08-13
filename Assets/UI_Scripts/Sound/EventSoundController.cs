using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSoundController : MonoBehaviour 
{
	//에니메이션 이벤트 에 쓰일 사운드컨트롤러 => 각 오브젝트에 play_Sound 스크립트에 인덱스를 배열로 넣고 불러와 재생시킨다

	public List<Play_sound> Lst_PlaySound;
	public AUDIOSOURCE_TYPE Audio_Type = AUDIOSOURCE_TYPE.AUDIO_2D;

	private AudioSource AudioSource3d;		//3d 사운드소스

	void Start()
	{
		if (Audio_Type == AUDIOSOURCE_TYPE.AUDIO_3D)
		{
			AudioSource3d = GetComponent<AudioSource>(); // 캐릭터에 붙어있는는 audiosource 가져오기
			if (AudioSource3d != null)
			AudioSource3d.spatialBlend = 1;		//3D 사운드 환경만들기 위한 변수 설정 (0 이면 2D사운드, 1이면 3D 사운드)
		}
	}

	
	public void Play_EventSound2D(int idx)
	{

		if (Lst_PlaySound.Count > 0)
		{
			for (int i = 0; i < Lst_PlaySound.Count; i++)
			{
				if (Lst_PlaySound[i].gameObject.activeSelf)
					SoundExecuter.Getsingleton.PlayEffect(Lst_PlaySound[i].Lst_AudioIndex[idx]);

			}


		}
	}


	public void Play_EventSound3D(int idx)
	{
		if (AudioSource3d != null)
		{
			if (Lst_PlaySound.Count > 0)
			{
				for (int i = 0; i < Lst_PlaySound.Count; i++)
				{
					if (Lst_PlaySound[i].gameObject.activeSelf)
						SoundExecuter.Getsingleton.PlaySound_3D(Lst_PlaySound[i].Lst_AudioIndex[idx], AudioSource3d);

				}


			}
		}
		else
			Debug.LogError("Check AudioSource in this object");
	}
}
