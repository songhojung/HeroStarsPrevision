using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSoundController : MonoBehaviour {


	public List<Play_sound> Lst_PlaySound;

	private AudioSource AudioSource3d;		//인게임용 사운드 (총알이펙트, 장전, 캐릭터대사 등등)

	void Start()
	{
		//if (AudioSource_Type == AUDIOSOURCE_TYPE.AUDIO_3D)
		{
			AudioSource3d = GetComponent<AudioSource>(); // 캐릭터에 붙어있는는 audiosource 가져오기
			AudioSource3d.spatialBlend = 1;		//3D 사운드 환경만들기 위한 변수 설정 (0 이면 2D사운드, 1이면 3D 사운드)
		}
	}


	//등록된 사운드 중 오브젝트 활성 된것만 이벤트 발생하여 3D소리 발생한다. => 무기 총
	public void Set_PlayVoice(int idx)
	{
		if (Lst_PlaySound.Count > 0)
		{
			for (int i = 0; i < Lst_PlaySound.Count; i++ )
			{
				if(Lst_PlaySound[i].gameObject.activeSelf)
					SoundExecuter.Getsingleton.PlaySound_3D(Lst_PlaySound[i].Lst_AudioIndex[idx], AudioSource3d);

			}


		}

	}




	

	void activeObj(int idx)
	{
		for (int i = 0; i < Lst_PlaySound.Count; i++)
			Lst_PlaySound[i].gameObject.SetActive(i == idx);
	}
}
