using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using kazgame.utils;

namespace kazgame.ui
{
	public class UIBasedLayerMask : MonoBase
	{
		public Button maskLayer;
		private float _originalAlpha;

		private readonly Dictionary<Transform, MaskedObject> masked = new Dictionary<Transform, MaskedObject> ();

		protected override void DoInit ()
		{
			base.DoInit ();

			_originalAlpha = maskLayer.image.color.a;
//			Log.Debug ("original alpha is " + _originalAlpha);
		}

		public void Show (Transform target,float alpha = -1f)
		{
			InitSelf ();
			if (null != target) {
				MaskedObject mo = new MaskedObject ();
				mo.parent = target.transform.parent;
				mo.slibingIndex = target.GetSiblingIndex ();
				masked.Add (target, mo);

				target.SetParent (maskLayer.transform.parent);

				maskLayer.transform.SetAsLastSibling ();
				target.SetAsLastSibling ();
			}
			maskLayer.gameObject.SetActive (true);
			Color c = maskLayer.image.color;
			if (0 > alpha) {
				c.a = _originalAlpha;
			} else {
				c.a = alpha;
			}
//			Log.Debug ("alpha set to" + c.a);
			maskLayer.image.color = c;
		}

		public void Unmask (bool all = false)
		{
			if (all) {
				foreach (KeyValuePair<Transform, MaskedObject> kp in masked) {
					kp.Key.SetParent (kp.Value.parent);
					kp.Key.SetSiblingIndex (kp.Value.slibingIndex);
				}
				masked.Clear ();
			} else if (maskLayer.transform.parent.childCount > 1) {
				Transform target = maskLayer.transform.parent.GetChild (maskLayer.transform.parent.childCount - 1);
				if (target != maskLayer) {
					MaskedObject mo = masked [target];

					target.SetParent (mo.parent);
					target.SetSiblingIndex (mo.slibingIndex);

					masked.Remove (target);

					//pop next
					if (maskLayer.transform.parent.childCount >= 1) {
						maskLayer.transform.SetSiblingIndex (maskLayer.transform.parent.childCount - 2);						
					}
				}
			}
			if (0 == masked.Count) {
				maskLayer.gameObject.SetActive (false);
			}
		}
	}

	public class MaskedObject
	{
		public int slibingIndex;
		public Transform parent;
	}
}

