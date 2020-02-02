using System;
using UnityEngine;

namespace kazgame
{
	public  class MonoBase : MonoBehaviour
	{
		private bool ___inited;

		protected virtual void DoInit(){
			
		} 

		protected void InitSelf(){
			if (___inited){
				return;
			}

			DoInit ();

			___inited = true;
		}
	}
}

