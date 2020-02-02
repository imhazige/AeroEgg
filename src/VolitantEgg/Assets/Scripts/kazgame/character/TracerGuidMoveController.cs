using System;
using UnityEngine;
using kazgame.utils;
using System.Collections.Generic;

namespace kazgame.character
{
	public class TracerGuidMoveController : GuidMoveController
	{


		public EdgeCollider2D guid;
		public Transform target;

		public override void Start ()
		{
			movePoints = Vector2DUtils.GetEdgeColliderPoints (guid);
			base.Start ();
		}

		protected override void PostUpdate (int reachedPoint)
		{
			base.PostUpdate (reachedPoint);

			if (null == target){
				return;
			}

			PointInfo pi = FindClosestPointOnWholePath (target.position);
			Debug.DrawLine (pi.position,target.position);
		}

	}
}

