using System;
using UnityEngine;
using kazgame.utils;

namespace kazgame.ste
{
	public class E9AttackThing : Actor
	{
		private Rigidbody2D _rigid;

		protected override void DoInit ()
		{
			base.DoInit ();
			_rigid = GetComponent<Rigidbody2D> ();
			_rigid.isKinematic = true;
		}

		public void Attack(Vector3 from,Vector3 target,float speed){
			transform.position = from;
			gameObject.SetActive (true);
			Vector2 dir = (target - from).normalized * speed;
			_rigid.velocity = dir;
			Log.Debug ("atacking......{0}",speed);
		}

		void Update(){
			if (!Vector2DUtils.IsInScreen(transform.position)){
				StopAct ();
				Log.Debug ("go to pooll...");
			}
		}

		public override void StopAct ()
		{
			Collide2DUtils.StopRigidVelocity (_rigid);
			base.StopAct ();
		}

		void OnTriggerEnter2D (Collider2D parCollider)
		{
			bool disegg = CheckCollideWithEgg (parCollider,GameOverReason.enemy,"E9_AttackThing");
			if (disegg){
				return;
			}
			bool desline = CheckCollideWithLine (parCollider);
			if (desline){
				StopAct ();
			}
		}
	}
}

