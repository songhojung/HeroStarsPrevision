using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundControllManager : MonoBehaviour 
{


	private List<AudioSource> LoopingMusices = new List<AudioSource>();
	private List<AudioSource> effectOneShot = new List<AudioSource>();
	//private  float soundVolume = 1.0f;
	//private  float musicVolume = 1.0f;


	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public bool IsPlay(AudioSource source)
	{
		return source.isPlaying;
	}

	public void PlayOneShotSound(AudioSource source, AudioClip clip, float volumeScale)
	{

	
		source.PlayOneShot(clip, volumeScale);

	}

	public void PlayLoopingMusic(AudioSource source, AudioClip clip, float volumeScale)
	{
		for (int i = 0; i < LoopingMusices.Count; i++)
		{
			if (LoopingMusices[i].clip != clip)
			{
				LoopingMusices.Remove(LoopingMusices[i]);
			}
		}

		
		source.loop = true;
		source.clip = clip;
		source.volume = volumeScale;
		source.Play();

		LoopingMusices.Add(source);
	}

	public void StopLoopingMusic(AudioSource source, AudioClip clip, float volumeScale)
	{

		StartCoroutine(reduce_musicVolume(source,volumeScale));
		
	}

	IEnumerator reduce_musicVolume(AudioSource source, float volumeScale)
	{
		float timestamp = 0;

		while (true)
		{
			if (source.volume == 0f)
			{
				source.Stop();
				LoopingMusices.Remove(source);
				break;
			}
			source.volume = Mathf.Lerp(volumeScale, 0, (timestamp += Time.deltaTime) / 1f);
			yield return null;
		}
	}

	public void MusicVolumeMultiplier(float volume)
	{

		foreach (AudioSource s in LoopingMusices)
		{
			if (s.isPlaying)
			{
				s.volume = volume;
			}
		}

	}

	public  void PauseAllMusic()
	{
		foreach (AudioSource s in LoopingMusices)
		{
			s.Pause();
		}
	}

	public  void ResumeAllMusic()
	{
		foreach (AudioSource s in LoopingMusices)
		{
			s.UnPause();
		}
	}
}
