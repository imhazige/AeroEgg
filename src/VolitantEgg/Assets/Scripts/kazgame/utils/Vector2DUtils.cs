using UnityEngine;
using System.Collections;
using System;

namespace kazgame.utils
{
	public static class Vector2DUtils
	{

		public static float Angle (Vector3 from, Vector3 to)
		{
			from.z = 0;
			to.z = 0;
			Vector3 localTarget = to - from;
			float targetAngle = Mathf.Atan2 (localTarget.y, localTarget.x) * Mathf.Rad2Deg;

			return targetAngle;
		}

		public static void UpsideDown (Transform from)
		{
			from.Rotate (new Vector3 (180, 0, 0));
		}

		public static void Flip (Transform from)
		{
			from.Rotate (new Vector3 (0, 180, 0));
		}

		public static Vector3 ChangePositionX (Transform transform, float x)
		{
			return ChangePosition (transform, x, transform.position.y, transform.position.z);
		}

		public static Vector3 ChangePositionY (Transform transform, float y)
		{
			return ChangePosition (transform, transform.position.x, y, transform.position.z);
		}

		public static Vector3 ChangePositionZ (Transform transform, float z)
		{
			return ChangePosition (transform, transform.position.x, transform.position.y, z);
		}

		public static Vector3 ChangePosition (Transform transform, float x, float y, float z)
		{
			Vector3 pos = new Vector3 (x, y, z);

			transform.position = pos;

			return pos;
		}

		/// <summary>
		/// Looks at from, target, baseAngle and offset.
		/// this will reset angle in x,y to 0, only apply rotate to z
		/// </summary>
		/// <param name="from">From.</param>
		/// <param name="target">Target. </param>
		/// <param name="baseAngle">Base angle.</param>
		/// <param name="offset">Offset.</param>
		public static void LocalLookAtWorld (Transform from, Vector3 worldTarget, float baseAngle = 0, float offset = 0)
		{
			if (null != from.parent) {
				worldTarget = from.parent.InverseTransformPoint (worldTarget);
			}
			float angle = Angle (from.localPosition, worldTarget);
			LocalLookAtWorld (from, angle, baseAngle, offset);
		}

		public static void LocalLookAtWorld (Transform from, float angle, float baseAngle = 0, float offset = 0)
		{
			baseAngle += angle;

			from.localRotation = Quaternion.AngleAxis (baseAngle, Vector3.forward);

			LocalMoveTo (from, angle, offset);
		}

		public static void LocalMoveTo (Transform from, float angle, float distance)
		{
			Vector3 newpos = (Vector2)from.localPosition + (AngleToVector (angle) * distance);
			newpos.z = from.localPosition.z;

			from.localPosition = newpos;
		}

		public static Vector2 AngleToVector (float angle)
		{
			Vector2 vForce = Quaternion.AngleAxis (angle, Vector3.forward) * Vector2.right;

			return vForce;
		}

		public static Vector3 MoveToSmoothly (Transform transform, Vector3 targetPos, float moveSpeed)
		{
			float step = moveSpeed * Time.deltaTime;
			targetPos.z = transform.position.z;
			Vector3 v = Vector3.MoveTowards (transform.position, targetPos, step);
			transform.position = v;

			return v;
		}

		/// <summary>
		/// Gets the box collider2D
		/// </summary>
		/// <returns>[V{x-leftbottom,y-leftbottom},V{x-righttop,y-righttop}]</returns>
		/// <param name="box2d">Box2d.</param>
		public static Vector2[] GetBoxCollider2DScope (BoxCollider2D box2d)
		{
			Vector2[] scope = new Vector2[2];

			Vector2 v = new Vector2 ();
			float hafx = box2d.size.x * 0.5f;
			float hafy = box2d.size.y * 0.5f;
			v.x = box2d.offset.x - hafx;
			v.y = box2d.offset.y - hafy;
			v = box2d.transform.TransformPoint (v);
			scope [0] = v;

			v.x = box2d.offset.x + hafx;
			v.y = box2d.offset.y + hafy;
			v = box2d.transform.TransformPoint (v);
			scope [1] = v;

			return scope;
		}

		public static Vector3[] GetEdgeColliderPoints (EdgeCollider2D edge, float z = 0)
		{
			Vector3[] points = new Vector3[edge.points.Length];
			for (int i = 0; i < edge.points.Length; i++) {
				Vector3 fp = edge.points [i];
				points [i] = edge.transform.TransformPoint (fp);
				points [i].z = z;
			}
			return points;
		}

		public static Vector2[] BoxScopeToPoints (Vector2[] scope)
		{
			Vector2[] points = new Vector2[4];
			Vector2 min = scope [0];
			Vector2 max = scope [1];
			points [0] = new Vector2 (min.x, min.y);
			points [1] = new Vector2 (max.x, min.y);
			points [2] = new Vector2 (max.x, max.y);
			points [3] = new Vector2 (min.x, max.y);

			return points;
		}

		public static Vector2[] GetScreenScope ()
		{
			Vector3 stageDimensions = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width, Screen.height, 0));

			Vector3 lbPoint = Camera.main.ScreenToWorldPoint (new Vector3 (0, 0, 0));

			return new Vector2[]{ lbPoint, stageDimensions };
		}

		/// <summary>
		/// Determines if is outside screen the specified position direction2D camera.
		/// </summary>
		/// <returns><c>true</c> if is outside screen the specified position direction2D camera; otherwise, <c>false</c>.</returns>
		/// <param name="position">Position.</param>
		/// <param name="direction2D">Direction2 d. outside direction</param>
		/// <param name="camera">Camera.</param>
		public static bool IsOutsideScreen (Vector2 position, Direction2D direction2D, Camera camera = null)
		{
			if (null == camera) {
				camera = Camera.main;
			}

			Vector3 screenPoint = camera.WorldToViewportPoint (position);
			bool result = false;
			switch (direction2D) {
			case Direction2D.down:
				{
					result = screenPoint.y <= 0;
					break;
				}
			case Direction2D.up:
				{
					result = screenPoint.y >= 1;
					break;
				}
			case Direction2D.left:
				{
					result = screenPoint.x <= 0;
					break;
				}
			case Direction2D.right:
				{
					result = screenPoint.x >= 1;
					break;
				}
			default:
				throw new System.Exception (String.Format ("have not implement for direction {0}", direction2D));
			}

			return result;
		}

		public static bool IsInScreen (Vector2 position, Camera camera = null)
		{
			if (null == camera) {
				camera = Camera.main;
			}

			Vector3 screenPoint = camera.WorldToViewportPoint (position);
			if (screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1) {
				return true;
			} else {
				return false;
			}
		}

		/// <summary>
		/// return the closest point to vPoint on line vA to vB
		/// https://forum.unity3d.com/threads/math-problem.8114/#post-59715
		/// </summary>
		public static Vector2 ClosestOnLine (Vector2 vA, Vector2 vB, Vector2 vPoint)
		{
			return ((Vector2)Vector3.Project ((vPoint - vA), (vB - vA))) + vA;
		}

		/// http://wiki.unity3d.com/index.php/3d_Math_functions
		///This function finds out on which side of a line segment the point is located.
		///The point is assumed to be on a line created by linePoint1 and linePoint2. If the point is not on
		///the line segment, project it on the line using ProjectPointOnLine() first.
		///Returns 0 if point is on the line segment.
		///Returns 1 if point is outside of the line segment and located on the side of linePoint1.
		///Returns 2 if point is outside of the line segment and located on the side of linePoint2.
		public static int PointOnWhichSideOfLineSegment (Vector3 linePoint1, Vector3 linePoint2, Vector3 point)
		{

			Vector3 lineVec = linePoint2 - linePoint1;
			Vector3 pointVec = point - linePoint1;

			float dot = Vector3.Dot (pointVec, lineVec);

			//point is on side of linePoint2, compared to linePoint1
			if (dot > 0) {

				//point is on the line segment
				if (pointVec.magnitude <= lineVec.magnitude) {

					return 0;
				}

				//point is not on the line segment and it is on the side of linePoint2
				else {

					return 2;
				}
			}

			//Point is not on side of linePoint2, compared to linePoint1.
			//Point is not on the line segment and it is on the side of linePoint1.
			else {

				return 1;
			}
		}


		/// <summary>
		/// http://wiki.unity3d.com/index.php/3d_Math_functions
		/// 
		/// Calculate the intersection point of two lines. Returns true if lines intersect, otherwise false.
		/// Note that in 3d, two lines do not intersect most of the time. So if the two lines are not in the 
		/// same plane, use ClosestPointsOnTwoLines() instead.
		/// </summary>
		/// <returns><c>true</c>, if line intersection was lined, <c>false</c> otherwise.</returns>
		/// <param name="intersection">Intersection.</param>
		/// <param name="linePoint1">Line point1.</param>
		/// <param name="lineVec1">Line vec1.</param>
		/// <param name="linePoint2">Line point2.</param>
		/// <param name="lineVec2">Line vec2.</param>
		public static bool LineLineIntersection (out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
		{
			Vector3 lineVec3 = linePoint2 - linePoint1;
			Vector3 crossVec1and2 = Vector3.Cross (lineVec1, lineVec2);
			Vector3 crossVec3and2 = Vector3.Cross (lineVec3, lineVec2);

			float planarFactor = Vector3.Dot (lineVec3, crossVec1and2);

			//is coplanar, and not parrallel
			if (Mathf.Abs (planarFactor) < Mathf.Epsilon && crossVec1and2.sqrMagnitude > Mathf.Epsilon) {
				float s = Vector3.Dot (crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
				intersection = linePoint1 + (lineVec1 * s);
				return true;
			} else {
				intersection = Vector3.zero;
				return false;
			}
		}

		public static float XAngleOfLineSegment (Vector2 vec1, Vector2 vec2)
		{
			Vector2 diference = vec2 - vec1;
			float sign = (vec2.y < vec1.y) ? -1.0f : 1.0f;
			return Vector2.Angle (Vector2.right, diference) * sign;
		}

		public static float Angle3 (Vector2 origin,Vector2 vec1, Vector2 vec2)
		{
			return Vector2.Angle (vec1 - origin, vec2 - origin);
		}

	}

}
