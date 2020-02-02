using System;
using UnityEngine;
using kazgame.animation;
using System.Collections;
using kazgame.utils;
using System.Collections.Generic;
using kazgame.character;

namespace kazgame.ste
{
	public class E4 : Actor
	{
		public EdgeCollider2D moveGuid;
		public SpriteAnimator animRun;
		public SpriteAnimator animJump;

		public List<int> adjustPolygonColliderIndexs;

		public float moveSpeed = 2f;
		public List<int> jumpIndexs;

		public Collider2D collid2d;

		private XYTracerGuidMoveController tracer;

		protected override void DoInit ()
		{
			base.DoInit ();
			if (null == moveGuid){
				moveGuid = GetComponentInChildren<EdgeCollider2D> ();
			}
			if (null == collid2d){
				collid2d = GetComponent<Collider2D> ();
			}

			collid2d.isTrigger = true;
			tracer = new XYTracerGuidMoveController ();
			tracer.transform = transform;
			tracer.guid = moveGuid;
			tracer.moveSpeed = moveSpeed;
			tracer.ReachPointEvent += Tracer_ReachPointEvent;
			tracer.FlipingEvent += Tracer_FlipingEvent;
			tracer.excludesIndex = jumpIndexs;

			Vector2DUtils.ChangePositionZ (transform,(float)ZIndexs.enemy);
		}

		void Tracer_FlipingEvent (GuidMoveController param)
		{
			Vector2DUtils.Flip(transform);
		}

		void Tracer_ReachPointEvent (GuidMoveController param)
		{
			bool inJump = false;
			if (tracer.fliping) {
				if (jumpIndexs.Contains (param.movingIndex-1)) {
					inJump = true;
				}
			} else {
				if (jumpIndexs.Contains (param.movingIndex)) {
					inJump = true;
				}
			}
			if (inJump) {
				//				Log.Debug ("need jump {0}",tracer.movingIndex);
				if (!animJump.running) {
					//					Log.Debug ("start jump {0}",tracer.movingIndex);
					animJump.StartPlay ();	
					animRun.StopPlay ();
				}
			} else {
				//				Log.Debug ("go to normal {0}",tracer.movingIndex);
				animJump.StopPlay ();
				animRun.StartPlay ();
			}
		}

		void Awake(){
			InitSelf ();
		}

		void Start(){
			tracer.Start ();
			//_cor = StartCoroutine (DoMyUpdate());
		}

		void Update(){
			tracer.Update ();
		}

		public override void OnPause ()
		{
			base.OnPause ();
			animRun.OnPause ();
			animJump.OnPause ();
			tracer.OnPause ();
		}

		public override void OnResume ()
		{
			base.OnResume ();
			animRun.OnResume ();
			animJump.OnResume ();
			tracer.OnResume ();
		}

		void OnTriggerEnter2D (Collider2D parCollider)
		{
			CheckCollideWithEgg (parCollider,GameOverReason.enemy,"E4");
			CheckCollideWithLine (parCollider);
		}
	}
}

