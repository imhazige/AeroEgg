using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using kazgame.utils;
using kazgame.ui;

namespace kazgame.ste
{
	public class MenuCreditsController : MonoBase
	{
		public delegate void OnClose();

		public OnClose onClose;

		public Button btnClose;

		public MaskTextScrollEffect maskTextEffect;


		void Awake(){
			InitSelf ();
		}

		protected override void DoInit ()
		{
			base.DoInit ();

			btnClose.onClick.AddListener(HideMenu);
		}

		private void Reset(){
		}

		public void HideMenu(){
			InitSelf ();
			gameObject.SetActive (false);
			if (null != onClose){
				onClose ();
			}
		}

	}
}

