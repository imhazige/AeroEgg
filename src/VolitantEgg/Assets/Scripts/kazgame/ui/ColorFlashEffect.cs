using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using kazgame.utils;

namespace kazgame.ui
{
	public abstract class ColorFlashEffect : MonoBase,PauseAble
	{
		public Color flashColor = Color.yellow;
		public float interval = .5f;

		private Coroutine _cor;
		private Color _originColor;
		private bool _pausing;

		abstract protected Color targetColor{ set; get;}

		public virtual void Awake(){
			InitSelf ();
			_originColor = targetColor;
		}

		public virtual void Start(){
			StartFlash ();
		}

		public void StartFlash(){
			InitSelf ();
			StopFlash ();
			_cor = StartCoroutine (DoFlash());
		}

		public void StopFlash(){
			InitSelf ();
			targetColor = _originColor;
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

		protected IEnumerator DoFlash (){
			while (true) {
				if (!_pausing){
					Color c = targetColor;
					if (c == flashColor) {
						c = _originColor;
					} else if (c == _originColor) {
						c = flashColor;
					}
					targetColor = c;	
				}
				yield return new WaitForSeconds (interval);
			}
		}
	}
}

