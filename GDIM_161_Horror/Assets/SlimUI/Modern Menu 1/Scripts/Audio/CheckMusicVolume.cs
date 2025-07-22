using UnityEngine;
using System.Collections;

namespace SlimUI.ModernMenu{
	public class CheckMusicVolume : MonoBehaviour {
		public void  Start ()
		{
			// remember volume level from last time
			if (GetComponent<AudioSource>() == null) return;
			GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MusicVolume");
		}

		public void UpdateVolume ()
		{
            if (GetComponent<AudioSource>() == null) return;
            GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MusicVolume");
		}
	}
}