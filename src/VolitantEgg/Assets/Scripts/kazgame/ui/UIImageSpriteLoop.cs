using System;
using UnityEngine.UI;


namespace kazgame.ui
{
	public class UIImageSpriteLoop : SpriteLoop
	{
		private Image _image; 

		protected override void DoInit ()
		{
			base.DoInit ();

			_image = GetComponent<Image> ();
		}

		protected override void LoopToSprite (UnityEngine.Sprite sprite)
		{
			_image.sprite = sprite;
		}
	}
}

