#if (UNITY_EDITOR)
using System;
using UnityEditor;
using UnityEngine;
using kazgame.utils;

namespace kazgame.kingkong
{
	public class DebugDrawHorizontalLineMatchViewport:MonoBehaviour
	{
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

			Vector2[] scope = Vector2DUtils.GetScreenScope ();

			Vector3 _p1 = Vector3.zero;
			Vector3 _p2 = Vector3.zero;

			_p1.y = transform.position.y;
			_p1.x = scope[0].x;

			_p2.y = transform.position.y;
			_p2.x = scope[1].x;

			Gizmos.color = color;

			Gizmos.DrawLine (_p1,_p2);
		}
	}
}
#endif

