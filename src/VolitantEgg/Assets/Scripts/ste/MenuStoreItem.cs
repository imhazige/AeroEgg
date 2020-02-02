using System;
using UnityEngine;

namespace kazgame.ste
{
	public class MenuStoreItem : MonoBase
	{
		public string title;

		public void Show(bool show){
			gameObject.SetActive (show);
		}
	}
}

