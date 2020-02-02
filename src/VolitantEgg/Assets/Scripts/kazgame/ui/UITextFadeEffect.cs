using System;
using UnityEngine;
using UnityEngine.UI;
using kazgame.utils;

namespace kazgame.ui
{
	public class UITextFadeEffect : MonoBase
	{
		private float _speed = 100;

		private Text _text;
		private bool seted = false;
		private float _fadeTarget;
		private float _fadeStart = 1;

		protected override void DoInit ()
		{
			base.DoInit ();
			_text = GetComponent<Text> ();
		}

		void Awake ()
		{
			InitSelf ();
		}

		/// <summary>
		/// Fade the specified value, fadeTime, fadeStart and fadeTaget.
		/// </summary>
		/// <param name="value">text value</param>
		/// <param name="fadeTime">the total time fade will take in seconds</param>
		/// <param name="fadeStart">Fade start.</param>
		/// <param name="fadeTaget">Fade taget.</param>
		public void Fade (string value, float fadeTime = 3, float fadeStart = 1, float fadeTaget = 0)
		{
			gameObject.SetActive (true);
			seted = false;
			_text.text = value;
			_fadeStart = fadeStart;
			_fadeTarget = fadeTaget;
			if (Math.Abs (fadeTime) < Mathf.Epsilon) {
				seted = true;
				UIUtils.SetTextAlpha(_text, fadeTaget);
			} else {
				UIUtils.SetTextAlpha (_text, _fadeStart);
				_speed = Mathf.Abs(_fadeTarget - _fadeStart) / fadeTime;
			}
		}

		void Update ()
		{
			if (seted) {
				return;
			}

			float a = Mathf.MoveTowards (_text.color.a, _fadeTarget, _speed * Time.deltaTime);
			if (Math.Abs (_fadeTarget - a) < Mathf.Epsilon) {
				a = _fadeTarget;
				seted = true;
			}
//			Log.Debug ("fadeing " + a);

			UIUtils.SetTextAlpha (_text, a);
		}
	}
}

