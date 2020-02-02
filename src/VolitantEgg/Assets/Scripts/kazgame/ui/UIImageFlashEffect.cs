using System;
using UnityEngine.UI;
using UnityEngine;

namespace kazgame.ui
{
	public class UIImageFlashEffect : ColorFlashEffect
	{
		private Image _image; 

		protected override void DoInit ()
		{
			base.DoInit ();

			_image = GetComponent<Image> ();
		}

		protected override Color targetColor {
			get {
				return _image.color;
			}
			set {
				_image.color = value;
			}
		}
	}
}

