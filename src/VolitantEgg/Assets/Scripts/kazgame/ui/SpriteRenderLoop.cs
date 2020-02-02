using System;
using UnityEngine;

namespace kazgame.ui
{
	public class SpriteRenderLoop : SpriteLoop
	{
		private SpriteRenderer _image; 

		protected override void DoInit ()
		{
			base.DoInit ();

			_image = GetComponent<SpriteRenderer> ();
		}

		protected override void LoopToSprite (UnityEngine.Sprite sprite)
		{
			_image.sprite = sprite;
		}
	}
}

