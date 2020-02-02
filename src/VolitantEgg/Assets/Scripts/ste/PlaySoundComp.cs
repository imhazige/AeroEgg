using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using kazgame.utils;

namespace kazgame.ste
{
	public class PlaySoundComp : MonoBase
	{
		public List<AudioClip> clips;

		private float _curMusicLastTime;

		private int _curPlayIndex;

		private Coroutine _musicCor;

		protected override void DoInit ()
		{
			base.DoInit ();
		}

		void Awake(){
			InitSelf ();
		}

		void Start(){
			PlayMusic ();
		}

		public void PlayMusic(){
			StopPlayMusic ();
			if (null == clips || 0 == clips.Count) {
				return;
			}
			_curPlayIndex = -1;
			PlayMusicNext ();
		}

		private void PlayMusicNext ()
		{
			if (null != _musicCor) {
				StopCoroutine (_musicCor);
			}

			_curPlayIndex = (_curPlayIndex + 1) % clips.Count;
			AudioClip clip = clips[_curPlayIndex];
			Log.Debug ("playing {0}",clip.name);
			_curMusicLastTime = clip.length;
			GameController.singleton.audioController.PlaySound (clip);
			_musicCor = StartCoroutine (DoPlayCheck (_curMusicLastTime));
		}

		IEnumerator DoPlayCheck (float lastTime)
		{
			float playedTime = 0;

			const float waitsec = 5;
			while (playedTime < lastTime + 5) {
				playedTime += waitsec;
				yield return new WaitForSeconds (waitsec);
			}

			int time = UnityEngine.Random.Range (0,20);
			yield return new WaitForSeconds (time);
			PlayMusicNext ();
		}

		public void StopPlayMusic ()
		{
			//			Log.Debug ("--------stop music");
			if (null != _musicCor) {
				StopCoroutine (_musicCor);
			}
		}
	}
}

