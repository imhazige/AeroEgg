using System;
using UnityEngine;
using kazgame.animation;
using System.Collections;
using kazgame.utils;
using kazgame.character;
using System.Collections.Generic;

namespace kazgame.ste
{

	public class E6 : Actor
	{
		public SpriteAnimator animIdle;
		public SpriteAnimator animAttack;

		public float extraLineSize = 0.01f;
		public Color extraLineColor = Color.white;
		public Material lineMaterial;

		private EdgeCollider2D tongueGuid;
		private Coroutine _cor;
		private const float angleScope = 30f;
		private const float attackSpeed = 10f;
		private const float attackInterval = 0.5f;
		private EdgeCollider2D edgeCollider;

		private SwitchTimer _timer;
		
		protected override void DoInit ()
		{
			base.DoInit ();

			_timer = new SwitchTimer ();
			tongueGuid = GetComponentInChildren<EdgeCollider2D> ();
			tongueGuid.enabled = false;
			Vector2DUtils.ChangePositionZ (transform.parent,(float)ZIndexs.enemy);
			edgeCollider = gameObject.AddComponent<EdgeCollider2D> ();
			edgeCollider.isTrigger = true;
		}


		protected override void Start ()
		{
			base.Start ();
			_cor = StartCoroutine (DoUpdate());
		}	

		IEnumerator DoUpdate(){
			Vector3[] vs = Vector2DUtils.GetEdgeColliderPoints (tongueGuid,transform.position.z);
			Vector3 p0 = vs [0];
			Vector3 p1 = vs [vs.Length-1];
			LineRenderer line = null;
			float length = Mathf.Abs(Vector2.Distance (p1,p0));
			Vector3 tongnePos = p0;
			bool attacking = false;

			_timer.maxMoveLength = length;
			_timer.moveSpeed = attackSpeed;
			_timer.switchDuration = attackInterval;
			_timer.Start ();
			animIdle.StartPlay ();

			while (true) {
				if (!pausing) {
					float mv = _timer.Update ();

					Vector3 eggPos = GameController.GetSingleton ().levelsController.eggController.transform.position;

					eggPos.z = p0.z;

					float angle = Vector2DUtils.Angle3 (p0,p1,eggPos);

					Vector3 postarget = p0 + (eggPos - p0).normalized * length;

					if (_timer.phase == SwitchTimer.Phase.MovingOut) {
					} else if (_timer.phase == SwitchTimer.Phase.MoveBackPrepare) {
						postarget = p0;
					} else if (_timer.phase == SwitchTimer.Phase.MoveingBack) {
						postarget = p0;
					} else if (_timer.phase == SwitchTimer.Phase.MoveOutPrepare) {
						attacking = false;
						animAttack.StopPlay ();
						animIdle.StartPlay ();
					}

					Vector3 tmppos = Vector3.MoveTowards (tongnePos, postarget, mv);

					if (attacking) {
						Vector3[] points = new Vector3[]{ p0, tongnePos };
						tongnePos = tmppos;
						line.SetPositions (points);
						UpdateLineCollider (points);
					} else if (angle <= angleScope) {
						Vector3[] points = new Vector3[]{ p0, tongnePos };
						UpdateLineCollider (points);
						tongnePos = tmppos;
						attacking = true;
						animIdle.StopPlay ();
						animAttack.StartPlay ();
						if (null == line) {
							line = DrawExtraLine (p0, tongnePos);	
						} else {
							line.SetPositions (points);
						}
						UpdateLineCollider (points);
					} else {
//						Log.Debug ("3333----");
					}

				}

				yield return null;
			}

		}

		LineRenderer DrawExtraLine(Vector3 from, Vector3 to){
			Vector3[] points  = new Vector3[]{from,to};
			LineRenderer line = gameObject.AddComponent<LineRenderer> ();
			line.startWidth = line.endWidth = extraLineSize;
			line.startColor = line.endColor = extraLineColor;
			line.useWorldSpace = true;
			line.receiveShadows = false;
			line.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
			line.positionCount = points.Length;
			line.SetPositions (points);
			line.material = lineMaterial;

			return line;
		}

		void UpdateLineCollider (Vector3[] poins)
		{
			List<Vector3> pointsList = new List<Vector3> (poins);
			if (!edgeCollider) {
				return;	
			}
			int dc = pointsList.Count * 2;
			Vector2[] points = new Vector2[dc + 1];	

			float boxWidth = extraLineSize / 2;

			for (int i = 0; i < pointsList.Count - 1; i++) {
				Vector3 vs = pointsList [i];
				Vector3 ve = pointsList [i + 1];

				float angle = Mathf.Atan ((ve.y - vs.y) / (ve.x - vs.x));

				float xdis = boxWidth * Mathf.Sin (angle);
				float ydis = boxWidth * Mathf.Cos (angle);

				Vector2 v0 = new Vector2 (vs.x - xdis, vs.y + ydis);
				Vector2 v1 = new Vector2 (vs.x + xdis, vs.y - ydis);

				if (0 == i) {
					points [dc] = v0;	
				}

				points [i] = v0;
				points [dc - 1 - i] = v1;

				if (i + 1 == pointsList.Count - 1) {
					v0 = new Vector2 (ve.x - xdis, ve.y + ydis);
					v1 = new Vector2 (ve.x + xdis, ve.y - ydis);

					points [i + 1] = v0;
					points [i + 2] = v1;
				}
			}

			//the collider origin point is the center of the game object
			edgeCollider.offset = transform.position * -1;
			edgeCollider.points = points;
		}

		void OnTriggerEnter2D (Collider2D parCollider)
		{
			CheckCollideWithEgg (parCollider,GameOverReason.enemy,"E6");
			//will not work -- edge to edge will not trigger
			CheckCollideWithLine (parCollider);
		}
	}
}
