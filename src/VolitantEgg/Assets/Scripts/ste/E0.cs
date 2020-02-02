using System;
using UnityEngine;
using kazgame.animation;
using System.Collections;
using kazgame.utils;

namespace kazgame.ste 
{
	public class E0 : Actor
	{
		public Collider2D collid2d;

		protected override void DoInit ()
		{
			base.DoInit ();
			if (null == collid2d){
				collid2d = GetComponent<Collider2D> ();
			}
			collid2d.isTrigger = true;
		}

		void Awake(){
			InitSelf ();
		}

		void OnTriggerEnter2D (Collider2D parCollider)
		{
			CheckCollideWithEgg (parCollider,GameOverReason.enemy,"E0");
			CheckCollideWithLine (parCollider);
		}
	}
}

