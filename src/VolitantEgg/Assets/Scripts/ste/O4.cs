using System;
using UnityEngine;
using kazgame.utils;
using kazgame.animation;

namespace kazgame.ste
{
	public class O4 : Actor
	{
		private Rigidbody2D _rigid;
		private kazgame.utils.Collide2DUtils.VelocityData _pauseVelocity;
		private Collider2D _collid;
		private bool _startedShow;
		private ShakeEffect _shakeEffect;

		protected override void DoInit ()
		{
			base.DoInit ();

			_rigid = GetComponent<Rigidbody2D> ();
			_rigid.isKinematic = true;
			_collid = GetComponent<Collider2D> ();
			_collid.isTrigger = true;
			Vector2DUtils.ChangePositionZ (transform,(float)ZIndexs.o4);
			_shakeEffect = GetComponent<ShakeEffect> ();
//			_shakeEffect.autoStart = false;
		}

		void Awake ()
		{
			InitSelf ();
		}

		void Update ()
		{
			if (_startedShow && Vector2DUtils.IsOutsideScreen(transform.position,Direction2D.down,null)){
				Log.Debug ("left screen, destroy.");
				StopAct ();
				return;
			}
			if (pausing) {
//				Log.Debug (" 01 ...");
				return;
			}		
			float tdis = Mathf.Abs(transform.position.y - GameController.GetSingleton ().levelsController.eggController.transform.position.y);

			if (!_startedShow && tdis < 0.5f) {
//				Log.Debug (" 1 ...");
				//start fall
				Fall ();
			}
		}

		void Fall ()
		{
			_shakeEffect.StopAct ();
			_rigid.isKinematic = false;
			_startedShow = true;
			_rigid.AddForce (Vector2.down * 6);
//			Log.Debug (" ssss ...");
		}

		public override void OnPause ()
		{
			base.OnPause ();
			_pauseVelocity = Collide2DUtils.StopRigidVelocity (_rigid);
		}

		public override void OnResume ()
		{
			base.OnResume ();
			Collide2DUtils.RestoreRigidVelocity (_rigid, _pauseVelocity);
		}

		void OnTriggerEnter2D (Collider2D parCollider)
		{
			CheckCollideWithEgg (parCollider,GameOverReason.obstacle,"O4");
			CheckCollideWithLine (parCollider);
		}
	}
}

