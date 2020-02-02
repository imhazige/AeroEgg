using System;
using UnityEngine;
using kazgame.animation;
using System.Collections;
using kazgame.utils;
using kazgame.character;
using System.Collections.Generic;

namespace kazgame.ste
{
	public class E9 : Actor
	{
		public Transform localAtackThingStartPos;
		public EdgeCollider2D moveGuid;
		public SpriteAnimator animIdle;
		public SpriteAnimator animActive;
		public SpriteAnimator animJump;
		public SpriteAnimator animClimb;
		public SpriteAnimator animAttack;

		public List<int> adjustPolygonColliderIndexs;

		public float moveSpeed = 2f;
		public float sensibleDistance = 5f;
		public List<int> jumpIndexs;

		public Collider2D collid2d;

		private const float attackSpeed = 8f;
		private const int startThrowAttackAnimateIndex = 6;

		private Vector3 moveGuideStartPoint;
		private const int idleStartSensibleIndex = 16;
		private const float sensibleAngle = 60f;
		private const float activeJumpSpeed = 8f;

		private Coroutine _cor;
		private XYTracerGuidMoveController tracer;
		private bool attacking {
			get;
			set;
		}

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
			tracer.xAxis = false;
			tracer.transform = transform;
			tracer.guid = moveGuid;
			tracer.moveSpeed = moveSpeed;
			tracer.ReachPointEvent += Tracer_ReachPointEvent;
			tracer.FlipingEvent += Tracer_FlipingEvent;
			tracer.ReachTargetEvent += Tracer_ReachTargetEvent;
			tracer.excludesIndex = jumpIndexs;

			animAttack.loopCount = 1;
			animAttack.OnAnimateEvent += OnAnimateEventDelegate;
			Vector2DUtils.ChangePositionZ (transform.parent,(float)ZIndexs.enemy9Group);

			animIdle.loopCount = 0;
			animIdle.playOnStart = true;
			animIdle.OnAnimateEvent += OnAnimateEventDelegate;

			animActive.loopCount = 1;
			animActive.playOnStart = false;
			animActive.OnAnimateEvent += OnAnimateEventDelegate;

			animClimb.loopCount = 0;
			animClimb.playOnStart = false;
		}

		void OnAnimateEventDelegate (string eventName, SpriteAnimator animator)
		{
			if (animator == animAttack){
				if (SpriteAnimator.EVENT_LOOPEND == eventName) {
					tracer.OnResume ();
					animClimb.StartPlay ();
				} else if (SpriteAnimator.EVENT_LOOP_INDEX == eventName) {
					if (startThrowAttackAnimateIndex == animAttack.loopIndex){
						LevelsController lc = GameController.GetSingleton().levelsController;
						E9AttackThing at = lc.objectPoolController.InitiateFromPool<E9AttackThing>(LevelsController.PK_E9AttackThing);
						Vector3 locpos = localAtackThingStartPos.localPosition;
						locpos = transform.TransformPoint (locpos);
						at.Attack (locpos,lc.eggController.transform.position,attackSpeed);
					}
				}
			}else if (animator == animActive){
				if (SpriteAnimator.EVENT_LOOPEND == eventName){
					animActive.StopPlay ();
					animJump.StartPlay ();
					Vector3 pos0 = moveGuideStartPoint;
					_cor = StartCoroutine (DoJump(pos0));
				}
			}else if (animator == animIdle){
				if (SpriteAnimator.EVENT_LOOP_INDEX == eventName) {
					if (animIdle.loopIndex >= idleStartSensibleIndex) {
						Transform egg = GameController.GetSingleton ().levelsController.eggController.transform;
						float angle = Vector2DUtils.XAngleOfLineSegment (transform.position, egg.position);
						bool startTrace = false;
						if (transform.position.x < egg.position.x) {
							//left to egg
							if (angle < sensibleAngle && angle > -sensibleAngle) {
								startTrace = true;
								Log.Debug ("E9 left start trace... {0}",angle);
							} else {
//								Log.Debug ("1 {0}", angle);
							}
						} else {
							if (angle > 180 - sensibleAngle || angle < - (180 - sensibleAngle)) {
								startTrace = true;
								Log.Debug ("E9 right start trace... {0}",angle);
							} else {
//								Log.Debug ("2 {0}", angle);
							}
						}
						if (startTrace) {
							animIdle.StopPlay ();
							animActive.StartPlay ();
						}
					} else {
//						Log.Debug ("3 {0}", animIdle.loopIndex);
					}
				} else {
//					Log.Debug ("4 {0}", animIdle.loopIndex);
				}
			}
		}

		void Tracer_ReachTargetEvent (XYTracerGuidMoveController param)
		{
			tracer.OnPause ();
			//			Log.Debug ("reach event {0}",param.transform.position);
			animClimb.StopPlay ();
			animAttack.StartPlay ();
		}

		void Tracer_FlipingEvent (GuidMoveController param)
		{
//			Vector2DUtils.Flip(transform);
		}

		void Tracer_ReachPointEvent (GuidMoveController param)
		{
			//			Log.Debug ("moving to {0}",tracer.movingIndex);
			//			if (param.movingIndex == 1 && tracer.fliping){
			//				tracer.Flip ();	
			//			}
			//			if (param.movingIndex == 2 && !tracer.fliping){
			//				tracer.Flip ();	
			//			}
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
					animClimb.StopPlay ();
				}
			} else {
				//				Log.Debug ("go to normal {0}",tracer.movingIndex);
				animJump.StopPlay ();
				animClimb.StartPlay ();
			}
		}

		protected override void Start ()
		{
			base.Start ();
			moveGuideStartPoint = moveGuid.transform.TransformPoint (moveGuid.points[0]);
			moveGuideStartPoint.z = transform.position.z;
		}

		void Update(){
			tracer.Update ();
		}

		public override void OnPause ()
		{
			base.OnPause ();
			animIdle.OnPause ();
			animActive.OnPause ();
			animClimb.OnPause ();
			animAttack.OnPause ();
			animJump.OnPause ();

			tracer.OnPause ();
		}

		public override void OnResume ()
		{
			base.OnResume ();
			animIdle.OnResume ();
			animActive.OnResume ();
			animClimb.OnResume ();
			animAttack.OnResume ();
			animJump.OnResume ();

			tracer.OnResume ();
		}

		void OnTriggerEnter2D (Collider2D parCollider)
		{
			CheckCollideWithEgg (parCollider,GameOverReason.enemy,"E9");
			CheckCollideWithLine (parCollider);
		}


		IEnumerator DoJump (Vector3 pos)
		{
			while(transform.position != pos){
				if (!pausing){
					Vector2DUtils.MoveToSmoothly (transform,pos,activeJumpSpeed);
				}	

				yield return null;
			}
			animJump.StopPlay ();
			animClimb.StartPlay ();
			Transform egg = GameController.GetSingleton ().levelsController.eggController.transform;
			tracer.target = egg;
			tracer.Start ();
			Log.Debug ("E9 start trace...");
		}
	}
}

