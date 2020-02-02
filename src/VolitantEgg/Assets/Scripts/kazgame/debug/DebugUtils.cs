using UnityEngine;
#if UNITY_EDITOR
using System.Collections;
using UnityEditor;

using System;

#endif

namespace kazgame.debug
{
	public static class DebugUtils
	{
		public static void Pause(){
			#if UNITY_EDITOR
			EditorApplication.isPaused = true;
			#endif
		}

		public static void DrawPoint(Vector3 p, float r = .1f){
			DrawPoint(p,Color.white,r);
		}

		public static void DrawPoint(Vector3 p,Color color, float r = .1f){
			#if UNITY_EDITOR
			Vector3 p1 = new Vector3 (p.x - r,p.y + r,p.z);
			Vector3 p2 = new Vector3 (p.x + r,p.y + r,p.z);
			Vector3 p3 = new Vector3 (p.x + r,p.y - r,p.z);
			Vector3 p4 = new Vector3 (p.x - r,p.y - r,p.z);
			Debug.DrawLine (p1,p2,color);
			Debug.DrawLine (p2,p3,color);
			Debug.DrawLine (p3,p4,color);
			Debug.DrawLine (p4,p1,color);
			#endif
		}
	}
}

