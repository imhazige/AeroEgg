using System;
using UnityEngine;
using kazgame.utils;
using System.Collections.Generic;

namespace kazgame.character
{
	public class PointInfo{
		public int startIndex;
		public Vector3 position;

		public override string ToString ()
		{
			return JsonUtility.ToJson(this);
		}
	}

	public class GuidMoveController:PauseAble
	{
		public Transform transform;
		public float moveSpeed = 1f;

		public int movingIndex {
			private set;
			get;
		}

		public bool fliping {
			private set;
			get;
		}

		public delegate void EventHandler1<TParam>(TParam param);
		public event EventHandler1<GuidMoveController> ReachPointEvent = (GuidMoveController c) => {};
		public event EventHandler1<GuidMoveController> FlipingEvent = (GuidMoveController c) => {};

		public bool started {
			get;
			private set;
		}

		public Vector3[] movePoints {
			get;
			set;
		}

		private bool _pausing;

		public bool pausing {
			get { 
				return _pausing;
			}
		}

		public virtual void Start(){
			started = true;
			_pausing = false;
		}

		public virtual void Stop(){
			started = false;
			_pausing = false;
		}

		protected virtual void PostUpdate (int reachedPoint)
		{
		}

		public void Update ()
		{
			if (!started){
				return;
			}
			if (_pausing) {
				return;
			}
			Vector3 fp = movePoints [movingIndex];
			Vector3 v = Vector2DUtils.MoveToSmoothly (transform,fp,moveSpeed);
//			Log.Debug ("moving... {0},{1},{2},{3},{4}",movingIndex,fp,v,transform.position,moveSpeed);
			int reachedPoint = -1;
			if ((Vector2)fp == (Vector2)transform.position){
//				Log.Debug ("OnReachPosition {0},{1}",fp , transform.position);
				OnReachPosition (movingIndex,fliping);
				reachedPoint = movingIndex;
				if (!fliping) {
					if (movingIndex == movePoints.Length - 1) {
						HandleFlip ();
					} else {
						movingIndex++;	
					}
				} else {
					if (movingIndex == 0) {
						HandleFlip ();
					} else {
						movingIndex--;	
					}
				}
			}
			PostUpdate (reachedPoint);
		}

		public PointInfo FindClosestPointOnWholePath(Vector2 target,List<int> excludesIndex = null){
			PointInfo info = null;
			const int div = 10;
			float dis = float.MaxValue;
			for (int i = 0; i < movePoints.Length - 1; i++) {
				if (null != excludesIndex && excludesIndex.Contains(i)){
					continue;
				}
				Vector3 p1 = movePoints[i];
				Vector3 p2 = movePoints [i + 1];
				Vector3 dir = p2 - p1;
				Vector3 ds = dir/div;
				Vector3 p = Vector3.zero;
				for (int s = 0; s <= div; s++) {
					p = p1 + ds * s;
				}
				float tmpDis = Mathf.Abs (Vector2.Distance (p, target));
				if (tmpDis < dis){
					info = new PointInfo ();
					dis = tmpDis;
					info.startIndex = i;
					info.position = p;
				}
			}

			return info;
		}

		public PointInfo FindSameXPointOnWholePath(float x,List<int> excludesIndex = null){
//			Log.Debug ("movinf points {0}",LogUtils.logList(movePoints));
			PointInfo info = null;
			Vector3 vx = new Vector3 (x,0);
			Vector3 vx1 = new Vector3 (x,1);
			Vector3 vx2 = new Vector3 (x,-1);

			return FindCrossPointOnWholePath (vx, vx1, vx2, excludesIndex);
		}

		public PointInfo FindSameYPointOnWholePath(float y,List<int> excludesIndex = null){
			//			Log.Debug ("movinf points {0}",LogUtils.logList(movePoints));
			PointInfo info = null;
			Vector3 vx = new Vector3 (0,y);
			Vector3 vx1 = new Vector3 (1,y);
			Vector3 vx2 = new Vector3 (-1,y);

			return FindCrossPointOnWholePath (vx, vx1, vx2, excludesIndex);
		}

		private PointInfo FindCrossPointOnWholePath(Vector3 vx,Vector3 vx1,Vector3 vx2,List<int> excludesIndex = null){
			PointInfo info = null;
			for (int i = 0; i < movePoints.Length - 1; i++) {
				if (null != excludesIndex && excludesIndex.Contains(i)){
					continue;
				}
				Vector3 p1 = movePoints[i];
				Vector3 p2 = movePoints [i + 1];
				Vector3 pInter = Vector3.zero;
				bool cross = Vector2DUtils.LineLineIntersection (out pInter, p1, p2 - p1, vx, vx1 - vx);
				if (!cross){
					cross = Vector2DUtils.LineLineIntersection (out pInter, p1, p2 - p1, vx, vx2 - vx);
				}
				if (!cross){
					continue;
				}
				if ( 0 != Vector2DUtils.PointOnWhichSideOfLineSegment(p1,p2,pInter)){
					continue;
				}
				info = new PointInfo ();
				info.position = pInter;
				info.startIndex = i;
				break;
			}

			return info;
		}

		public Vector2 FindClosestPointOnMovingLine(Vector2 target){
			Vector2 p3 = target;
			Vector2 p2 = movePoints[movingIndex];
			Vector2 p1 = movePoints[GetNearByIndex()];

			Vector2 np = Vector2DUtils.ClosestOnLine (p1,p2,p3);

			return np;
		}

		public int GetNearByIndex(){
			int index =  fliping ? movingIndex + 1 : movingIndex - 1;

			if (index == -1){
				index = 1;
			}else if (index  == movePoints.Length){
				index = movePoints.Length - 2;
			}

			return index;
		}

		public void Flip(){
			fliping = !fliping;
			movingIndex = fliping ? movingIndex - 1 : movingIndex + 1;
			movingIndex = Mathf.Clamp (movingIndex,0,movePoints.Length - 1);
//			Log.Debug ("after flip {0},{1}",fliping,movingIndex);
			FlipingEvent (this);
		}



		virtual protected void HandleFlip(){
			fliping = !fliping;
			FlipingEvent (this);
		}

		virtual protected void OnReachPosition ( int pmovingIndex, bool pfliping)
		{
			ReachPointEvent (this);
		}

		#region PauseAble implementation

		public void OnPause ()
		{
			_pausing = true;
		}

		public void OnResume ()
		{
			_pausing = false;
		}

		#endregion


	}
}

