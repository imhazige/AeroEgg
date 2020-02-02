using System;
using UnityEngine;
using System.Collections;
using kazgame.utils;

namespace kazgame.ui
{
	public abstract class ColorFadeLoopEffect: MonoBase
	{
		public float fadeSpeed = .2f;

		private Coroutine _cor;
		private float _originColorAlpha;

		private bool up;

		private const float lowAlpha = .5f;

		abstract protected Color targetColor{ set; get;}

		void Awake(){
			InitSelf ();
			_originColorAlpha = targetColor.a;
		}

		public virtual void Start(){
			StartEffect ();
		}

		public void StartEffect(){
			StopEffect ();
			_cor = StartCoroutine (DoFlash());
		}

		public void StopEffect(){
			SetColor (_originColorAlpha);
			if (null != _cor){
				StopCoroutine (_cor);
			}
		}

		protected IEnumerator DoFlash (){
			while (true) {
				float c = targetColor.a;
				if (Mathf.Approximately(1,c)) {
					up = false;
				} else if (Mathf.Approximately(lowAlpha,c)) {
					up = true;
				}
				c = Mathf.MoveTowards (c,up?1:lowAlpha,fadeSpeed*Time.deltaTime);
				SetColor (c);
				yield return null;
			}
		}

		void SetColor(float a){
			Color color = targetColor;
			color.a = a;
			targetColor = color;
		}
	}
}

