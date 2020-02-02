using System;
using UnityEngine;
using System.Collections;
using kazgame.utils;
using System.Collections.Generic;

namespace kazgame.animation
{
	[System.Serializable]
	public struct ShakeConfig
	{
		public float scope;
		public float time;
		public float interval;
	}

	public class ShakeEffect : MonoBase,PauseAble
	{
		[SerializeField]
		public List<ShakeConfig> configs = new List<ShakeConfig>();
		public float interval;
		public bool autoStart = true;

		private Coroutine _cor;
		private int _curIndex;
//		private Vector3 _position;
		private bool _pausing;

		protected override void DoInit ()
		{
			base.DoInit ();
		}

		void Awake(){
			InitSelf ();
		}

		void Start(){
//			_position = transform.position;
			if (autoStart){
				StartAct ();
			}
		}

		public void StartAct(){
			_curIndex = 0;
			_pausing = false;
			if (null == configs){
				return;
			}
			if (null != _cor){
				StopCoroutine (_cor);
			}
			_cor = StartCoroutine (DoLoop());
		}

		public void StopAct(){
			_curIndex = 0;
			_pausing = false;
			Log.Debug ("StopAct");
			if (null != _cor){
				StopCoroutine (_cor);
			}

		}

		#region PauseAble implementation

		public void OnPause ()
		{
			_pausing = true;
		}

		public void OnResume ()
		{
			_pausing = false;
		}

		#endregion

		IEnumerator DoLoop ()
		{
			while (true) {
				ShakeConfig config = configs [_curIndex];
				float shakedTime = 0;
				while (true && shakedTime < config.time) {
					Vector3 _position = transform.position;
					if (!_pausing){
						float m = config.scope * UnityEngine.Random.Range (-1f,1f);
						Vector2DUtils.ChangePosition (transform,_position.x + m,_position.y + m,_position.z);

						shakedTime += Time.deltaTime;	
					}
//					Log.Debug (" 1111 ... {0}",gameObject.name);

					yield return new WaitForSeconds (config.interval); 
				}
//				Log.Debug (" 2222 ... {0}",gameObject.name);
				_curIndex = (_curIndex + 1) % configs.Count;

				yield return new WaitForSeconds (interval); 
			}
		}
	}
}

