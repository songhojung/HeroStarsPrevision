using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DigitalRuby.SoundManagerNamespace;
using UnityEngine.SceneManagement;

public class SoundExecuter : MonoBehaviour 
{
	public SoundControllManager ScMgr;

    public AudioSource audioSource_2D;
	public AudioSource audioSource_2D_BGM;
    public AudioClip[] VoiceAudioClips;
    public AudioClip[] EffectAudioClips;
	public AudioClip[] MusicAudioClips;


    private static SoundExecuter _instance;
    public static SoundExecuter Getsingleton
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType(typeof(SoundExecuter)) as SoundExecuter;
                if (_instance == null)
                {
					GameObject instanceObj = Instantiate(Resources.Load("Prefebs/SoundManager")) as GameObject;
					instanceObj.name = "SoundManager";
					_instance = instanceObj.GetComponent<SoundExecuter>();
					DontDestroyOnLoad(instanceObj);
                }
            }

            return _instance;
            
        }
    }

    void Awake()
    {
        //_instance = this;
		

		//SceneManager.activeSceneChanged += ChageMusicforScene;
    }

	public bool IsPlay(AudioSource source)
	{
		return ScMgr.IsPlay(source);
	}

    public void PlaySound(int index)
    {

		OptionSetting setting = OptionSetting.instance;
		float voiceVolum = setting.VolumVoice / 100f;
       // audioSource.PlayOneShot(audioClips[index]);
		audioSource_2D.PlayOneShotSoundManaged(VoiceAudioClips[index], voiceVolum);
		
    }

	public void PlayEffect(int index)
	{
		// audioSource.PlayOneShot(audioClips[index]);
		OptionSetting setting = OptionSetting.instance;
	
		float EffectVolum = setting.VolumEffect / 100f;
		//audioSource_2D.PlayOneShotSoundManaged(EffectAudioClips[index],EffectVolum);
		ScMgr.PlayOneShotSound(audioSource_2D, EffectAudioClips[index], EffectVolum);
	}

    public void PlayMusic(int index)
	{
		OptionSetting setting = OptionSetting.instance;
		float MusicVolum = setting.VolumBGM / 100f;
        //audioSource.PlayLoopingMusicManaged(1.0f, 1.0f, true);
		//audioSource_2D_BGM.PlayLoopingMusicManaged(MusicAudioClips[index], MusicVolum, 0.0f, true);
		ScMgr.PlayLoopingMusic(audioSource_2D_BGM, MusicAudioClips[index], MusicVolum);
    }

	public void PlayEffect_3D(int index, AudioSource _audioSource)
	{
		// audioSource.PlayOneShot(audioClips[index]);
		OptionSetting setting = OptionSetting.instance;

		float EffectVolum = setting.VolumEffect / 100f;
		//_audioSource.PlayOneShotSoundManaged(EffectAudioClips[index], EffectVolum);
		ScMgr.PlayOneShotSound(_audioSource, EffectAudioClips[index], EffectVolum);
	}
	public void PlaySound_3D(int index, AudioSource _audioSource)
	{

		OptionSetting setting = OptionSetting.instance;
		float voiceVolum = setting.VolumEffect / 100f;
		// audioSource.PlayOneShot(audioClips[index]);
		//_audioSource.PlayOneShotSoundManaged(VoiceAudioClips[index], voiceVolum);
		ScMgr.PlayOneShotSound(_audioSource, VoiceAudioClips[index], voiceVolum);

	}

	public void ChangeMusicVolume(float volume)
	{
		//audioSource_2D_BGM.MusicVolumeMultiplier(volume);

		ScMgr.MusicVolumeMultiplier(volume);
	}

	public void StopMusic(int index)
	{
		OptionSetting setting = OptionSetting.instance;
		float MusicVolum = setting.VolumBGM / 100f;
		ScMgr.StopLoopingMusic(audioSource_2D_BGM, MusicAudioClips[index], MusicVolum);
	}

	public void PauseMusic()
	{
		//audioSource_2D_BGM.MusicAllPauseMultiplier();
		ScMgr.PauseAllMusic();
	}

	public void ResumeMusic()
	{
		//audioSource_2D_BGM.MusicAllresumeMultiplier();
		ScMgr.ResumeAllMusic();
	}


	//배경음악 체크하고 시작하기
	public void ChkStartMusic(MUSIC_TYPE musictype, bool isStart)
	{
		if (isStart)
		{
			PlayMusic((int)musictype);
		}
		else
		{
			if (IsPlay(audioSource_2D_BGM))
			{
				StopMusic((int)musictype);
			}
		}
	}


	public void ChageMusicforScene(Scene s1, Scene s2)
	{


		if (s2.name == DefineKey.Main)
		{
			PlayMusic((int)MUSIC_TYPE.LOBBY);
		}
		else
		{
			if (IsPlay(audioSource_2D_BGM))
			{
				StopMusic((int)MUSIC_TYPE.LOBBY);
			}
			
		}
	}

	//홈키 누르고 돌아올떄 
	public void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			ScMgr.ResumeAllMusic();
		}
		else
		{
			ScMgr.PauseAllMusic();

		}
	}

}
