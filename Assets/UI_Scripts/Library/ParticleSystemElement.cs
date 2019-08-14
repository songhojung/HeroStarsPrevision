using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ParticleSystemElement : MonoBehaviour
{

	public List<ParticleSystem> Particles;

	public int PlayParticle_Index = 0;

	public bool includeChildren = true;

	public bool IsPlayParticleControll = false;



	//public void Particle_play1()
	//{
	//    if (Particles.Count > 0)
	//    {

	//        if (IsPlayParticleControll)
	//        {
	//            if (includeChildren)
	//            {
	//                if (PlayParticle_Index >= Particles.Count)
	//                    Debug.LogError("Prticle_Index value is large than particleSystem count. Please set again Prticle_index value");
	//                else
	//                    Particles[PlayParticle_Index].Play(includeChildren);
	//            }
	//            else
	//            {
	//                if (PlayParticle_Index >= Particles.Count)
	//                    Debug.LogError("Prticle_Index value is large than particleSystem count. Please set again Prticle_index value");
	//                else
	//                    Particles[PlayParticle_Index].Play();
	//            }
	//        }
	//    }
	//}

	//public void Particle_play2(int _PlayParticle_Index)
	//{
	//    if (Particles.Count > 0)
	//    {

	//        if (IsPlayParticleControll)
	//        {
	//            if (includeChildren)
	//            {
	//                if (_PlayParticle_Index >= Particles.Count)
	//                    Debug.LogError("Prticle_Index value is large than particleSystem count. Please set again Prticle_index value");
	//                else
	//                    Particles[_PlayParticle_Index].Play(includeChildren);
	//            }
	//            else
	//            {
	//                if (_PlayParticle_Index >= Particles.Count)
	//                    Debug.LogError("Prticle_Index value is large than particleSystem count. Please set again Prticle_index value");
	//                else
	//                    Particles[_PlayParticle_Index].Play();
	//            }
	//        }
	//    }
	//}


	
}

