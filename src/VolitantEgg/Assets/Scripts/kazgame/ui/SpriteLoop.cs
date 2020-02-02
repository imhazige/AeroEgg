using System;
using UnityEngine;
using System.Collections;
using kazgame.utils;

namespace kazgame.ui
{
	public abstract class SpriteLoop : MonoBase
	{
		public Sprite[] sprites;
		public float interval;
		public bool loop;
		public bool autoStart;

		private Coroutine _cor;
		private int _curIndex;

		protected override void DoInit ()
		{
			base.DoInit ();
		}

		void Awake(){
			InitSelf ();
		}

		void Start(){
			if (autoStart){
				StartLoop ();
			}
		}

		public void StartLoop(){
			_curIndex = 0;
			if (null == sprites || 0 >= sprites.Length){
				return;
			}
			if (null != _cor){
				StopCoroutine (_cor);
			}
			_cor = StartCoroutine (DoLoop());
		}

		public void StopLoop(){
			_curIndex = 0;
			if (null != _cor){
				StopCoroutine (_cor);
			}
		}

		abstract protected void LoopToSprite(Sprite sprite);

		IEnumerator DoLoop ()
		{
			while (true) {
				LoopToSprite(sprites[_curIndex]);

				_curIndex++;

				if (_curIndex >= sprites.Length){
					if (!loop){
						yield break;
					}
				}

				_curIndex = _curIndex % sprites.Length;

				yield return new WaitForSeconds (interval);
			}
		}
	}
}

