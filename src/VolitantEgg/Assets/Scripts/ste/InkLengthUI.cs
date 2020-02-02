using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace kazgame.ste
{
	public class InkLengthUI : MonoBase {
		private Image _image;
		private Vector2 mOriginSize;

		protected override void DoInit ()
		{
			base.DoInit ();

			_image = GetComponent<Image> ();
			RectTransform rt = (RectTransform)_image.transform;
			mOriginSize = rt.sizeDelta;
		}

		void Awake () {
			InitSelf ();
		}
		
		public void UpdateInk(float rate){
			RectTransform rt = (RectTransform)_image.transform;

			rt.sizeDelta = new Vector2 (mOriginSize.x*rate,rt.sizeDelta.y);
		}
	}
}
