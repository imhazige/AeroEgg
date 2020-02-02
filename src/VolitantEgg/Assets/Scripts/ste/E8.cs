using System;
using UnityEngine;
using kazgame.animation;
using System.Collections;
using kazgame.utils;
using kazgame.character;
using System.Collections.Generic;

namespace kazgame.ste
{
	public class E8 : Actor
	{
		public EdgeCollider2D moveGuid;
		public SpriteAnimator[] animIdles;
		public SpriteAnimator animJumpUp;
		public SpriteAnimator animJumpDown;

		public Collider2D collid2d;
		private Coroutine _cor;

		private const float attackSensibleDistance = 1.5f;
		private const float moveSpeed = 5f;
		private XYTracerGuidMoveController tracer;
		private SpriteAnimator activeJumpAnimate;
		private SpriteAnimator activeIdleAnimate;
		private Vector3 originalErulers;

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
			originalErulers = transform.eulerAngles;

			tracer = new XYTracerGuidMoveController ();
			tracer.xAxis = false;
//			tracer.traceInterval = 0f;
			tracer.transform = transform;
			tracer.guid = moveGuid;
			tracer.moveSpeed = moveSpeed;
			tracer.onlyFlipWhenReachPoint = true;
			tracer.ReachPointEvent += Tracer_ReachPointEvent;
			tracer.FlipingEvent += Tracer_FlipingEvent;
			tracer.ReachTargetEvent += Tracer_ReachTargetEvent;

			Vector2DUtils.ChangePositionZ (transform.parent,(float)ZIndexs.enemy);

			foreach (SpriteAnimator item in animIdles) {
				item.loopCount = 1;
				item.playOnStart = false;
				item.OnAnimateEvent += OnAnimateEventDelegate;
			}

			activeIdleAnimate = RandomIdleAnimate ();
		}

		protected override void Start ()
		{
			base.Start ();
			_cor = StartCoroutine (DoUpdate());
		}

		private SpriteAnimator RandomIdleAnimate(){
			int index = UnityEngine.Random.Range (0, animIdles.Length);

			return animIdles[index];
		}

		void Tracer_ReachTargetEvent (XYTracerGuidMoveController param)
		{
		}

		void Tracer_FlipingEvent (GuidMoveController param)
		{
			//			Vector2DUtils.Flip(transform);
		}

		private void DoIdle(){
			transform.eulerAngles = originalErulers;
			if (null != activeJumpAnimate){
				activeJumpAnimate.StopPlay ();
			}
			activeIdleAnimate = RandomIdleAnimate ();
			activeIdleAnimate.StartPlay ();
		}



		void Tracer_ReachPointEvent (GuidMoveController param)
		{
			tracer.OnPause ();
			DoIdle ();
		}

		void OnAnimateEventDelegate (string eventName, SpriteAnimator animator)
		{
			if (animator == activeIdleAnimate){
				if (SpriteAnimator.EVENT_LOOPEND == eventName){
					if (tracer.started) {
						activeIdleAnimate.StopPlay ();
						tracer.OnResume ();

						Transform egg = GameController.GetSingleton ().levelsController.eggController.transform;
						int mindex = tracer.movingIndex;
						Vector3 mpos = tracer.movePoints [mindex];
						bool up = mpos.y > transform.position.y;
						//left side or right side
						activeJumpAnimate = up ? animJumpUp : animJumpDown;
						if (mpos.x < transform.position.x){
							//left
							transform.Rotate(new Vector3(0,180,0));
						}
						activeJumpAnimate.StartPlay ();
					} else {
						DoIdle ();
					}
				}
			}

		}

		IEnumerator DoUpdate ()
		{
			DoIdle ();
			Transform egg = GameController.GetSingleton ().levelsController.eggController.transform;

			while(egg.position.y - transform.position.y > attackSensibleDistance){
				yield return null;
			}

			tracer.target = egg;
			tracer.Start ();
			while (true) {
				if (!pausing){
					

//					PointInfo pi = tracer.FindSamePointOnWholePath ();
//					if (null == pi){
//						continue;
//					}

					tracer.Update ();
				}

				yield return null;
			}
		}

		void OnTriggerEnter2D (Collider2D parCollider)
		{
			CheckCollideWithEgg (parCollider,GameOverReason.enemy,"E8");
			CheckCollideWithLine (parCollider);
		}
	}
}

