using System;
using UnityEngine;
using kazgame.utils;
using System.Collections.Generic;

namespace kazgame.character
{
	public class XYTracerGuidMoveController : GuidMoveController
	{
		public EdgeCollider2D guid;
		public Transform target;
		public List<int> excludesIndex;
		/// <summary>
		/// should be large enough for to avoid flip frequently
		/// </summary>
		public float error = .05f;
		/// <summary>
		/// note, this value will make it more random to trace the target
		/// if you found sometimes it did not trace the target immediatly, 
		/// set it to 0 will resolve the problem.
		/// </summary>
		public float traceInterval = 1f;
		public bool xAxis = true;

		public bool onlyFlipWhenReachPoint {
			get;
			set;
		}

		public delegate void EventHandler1<TParam>(TParam param);
		public event EventHandler1<XYTracerGuidMoveController> ReachTargetEvent = (XYTracerGuidMoveController c) => {};

		private bool _reached = false;
		private float _traceIntervalPast = 0f;


		public override void Start ()
		{
			guid.isTrigger = true;
			movePoints = Vector2DUtils.GetEdgeColliderPoints (guid);
			base.Start ();
		}

		public override void Stop ()
		{
			base.Stop ();
			_traceIntervalPast = 0f;
			_reached = false;
		}

		protected override void PostUpdate (int reachedPoint)
		{
			base.PostUpdate (reachedPoint);

			if (null == target){
				return;
			}

			if (_reached){
				_traceIntervalPast += Time.deltaTime;
				if (_traceIntervalPast < traceInterval) {
					return;	
				} else {
					_traceIntervalPast = 0f;
					_reached = false;
				}
			}
			PointInfo pi = FindSamePointOnWholePath ();
			if (null == pi){
//								Log.Debug ("FindSameXPointOnWholePath is null");
				return;
			}
			//			Log.Debug ("FindSameXPointOnWholePath is {0}",pi);
			Debug.DrawLine (pi.position,target.position,Color.magenta);
			//			Log.Debug ("x dis {0}",transform.position.x - pi.position.x);
			if (CheckReached(pi)){
				_reached = true;
				ReachTargetEvent (this);
//				Log.Debug ("reached target .... ");
				return;
			}
			int lastIndex = fliping ? movingIndex + 1 : movingIndex - 1;
			if (!(onlyFlipWhenReachPoint && -1 == reachedPoint)) {
				if (lastIndex >= 0 && lastIndex <= movePoints.Length - 1) {
					Vector3 lastPoint = movePoints [lastIndex];
					if (!IsSameDirection (lastPoint, pi)) {
						//different direction, need flip
//						Log.Debug ("Flip.... {0},{1}",lastIndex, pi);
						Flip ();
					} else {
//						Log.Debug ("2 ----- {0},{1}",lastIndex, pi);
					}
				} else {
//					Log.Debug("1---- {0},{1}",onlyFlipWhenReachPoint,reachedPoint);
				}
			} else {
//				Log.Debug("not flip {0},{1}",onlyFlipWhenReachPoint,reachedPoint);
			}
		}

		public PointInfo FindSamePointOnWholePath (){
			PointInfo pi = null;
			if (xAxis) {
				pi = FindSameXPointOnWholePath (target.position.x, excludesIndex);
			} else {
				pi = FindSameYPointOnWholePath (target.position.y, excludesIndex);
			}

			return pi;
		}

		bool IsSameDirection(Vector3 lastPoint,PointInfo pi){
			Vector3 movingPoint = movePoints[movingIndex];
			float dir = 0f;
			if (xAxis) {
				dir = (movingPoint.x - lastPoint.x) * (pi.position.x - transform.position.x);
			} else {
				dir = (movingPoint.y - lastPoint.y) * (pi.position.y - transform.position.y);
			}
			return dir >= 0;
		}

		bool CheckReached(PointInfo pi){
			float dis = 0f;
			if (xAxis) {
				dis = Mathf.Abs (transform.position.x - pi.position.x);
			} else {
				dis = Mathf.Abs (transform.position.y - pi.position.y);
			}

			return error >  dis;
		}
	}
}


