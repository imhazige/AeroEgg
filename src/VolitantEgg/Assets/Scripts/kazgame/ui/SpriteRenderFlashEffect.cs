using System;
using UnityEngine;
using System.Collections;

namespace kazgame.ui
{
	public class SpriteRenderFlashEffect : ColorFlashEffect
	{
		private SpriteRenderer _render;

		protected override void DoInit ()
		{
			base.DoInit ();

			_render = GetComponent<SpriteRenderer> ();
		}

		protected override Color targetColor {
			get {
				return _render.color;
			}
			set {
				_render.color = value;
			}
		}
	}
}

