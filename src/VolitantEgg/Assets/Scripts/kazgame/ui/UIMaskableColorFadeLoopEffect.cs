using System;
using UnityEngine.UI;
using UnityEngine;

namespace kazgame.ui
{
	public class UIMaskableColorFadeLoopEffect : ColorFadeLoopEffect
	{
		private MaskableGraphic _colorable; 

		protected override void DoInit ()
		{
			base.DoInit ();

			_colorable = GetComponent<MaskableGraphic> ();
		}

		protected override Color targetColor {
			get {
				return _colorable.color;
			}
			set {
				_colorable.color = value;
			}
		}
	}
}

