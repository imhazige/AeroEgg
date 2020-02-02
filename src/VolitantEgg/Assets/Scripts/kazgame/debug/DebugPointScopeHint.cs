#if (UNITY_EDITOR)
using System;
using UnityEditor;
using UnityEngine;
using kazgame.utils;


namespace kazgame.debug
{
	public class DebugPointScopeHint:MonoBehaviour
	{
		public Color color = Color.yellow;
		public bool debugDraw = true;
		public float radius;
		public float width;
		public float height;

		public IPointScope pointScope;

		public virtual void OnDrawGizmos () {
			if (debugDraw){
				if (null == pointScope){
					pointScope = GetComponent<IPointScope> ();
				}
				if (null == pointScope){
					Log.Warn ("point scope did not specified.");

					return;
				}
				Gizmos.color = color;
				Vector2[] vs = pointScope.scope;
				if (null == vs) {
					return;
				}
				if (radius > 0) {
					foreach (Vector2 v in vs) {
						Gizmos.DrawSphere (v,radius);
					}
				} else {
					foreach (Vector2 v in vs) {
						Gizmos.DrawCube (v,new Vector3(width,height,0));
					}
				}
			}
		}
	}
}
#endif
