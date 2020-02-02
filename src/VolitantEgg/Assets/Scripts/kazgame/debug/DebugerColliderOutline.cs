#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using kazgame.utils;

namespace kazgame.debug
{
	public class DebugerColliderOutline : MonoBehaviour {
		public Color color = Color.yellow;
		public bool debugDraw = true;

		void OnDrawGizmos () {
			if (debugDraw){
				DrawOutLine ();	
			}
		}

		void DrawOutLine () {
			if(EditorApplication.isCompiling || EditorApplication.isUpdating){
				return;
			}

			Gizmos.color = color;

			PolygonCollider2D[] polygions = gameObject.GetComponents<PolygonCollider2D>();
			for (int i = 0 ; null != polygions && i < polygions.Length;i++){
				PolygonCollider2D p2d = polygions [i];
				DrawPolygion (p2d.points);
			}

			EdgeCollider2D[] edpos = gameObject.GetComponents<EdgeCollider2D>();
			for (int i = 0 ; null != edpos && i < edpos.Length;i++){
				EdgeCollider2D p2d = edpos [i];
				DrawPolygion (p2d.points);
			}

			BoxCollider2D[] boxpos = gameObject.GetComponents<BoxCollider2D>();
			for (int i = 0 ; null != boxpos && i < boxpos.Length;i++){
				BoxCollider2D p2d = boxpos [i];
				Vector2[] v2 = Vector2DUtils.GetBoxCollider2DScope (p2d);
				DrawBoxOnWorld (Vector2DUtils.BoxScopeToPoints(v2));
			}
		}

		void DrawPolygion(Vector2[] points){
			for (int i = 0 ;null != points && i < points.Length - 1 ; i++){
				Gizmos.DrawLine (transform.TransformPoint(points[i]),transform.TransformPoint(points[i+1]));
			}
		}

		void DrawBoxOnWorld(Vector2[] points){
			for (int i = 0 ;null != points && i < points.Length - 1 ; i++){
				Gizmos.DrawLine (points[i],points[i+1]);
			}
			Gizmos.DrawLine (points[points.Length-1],points[0]);
		}
	}
}

#endif
