#if (UNITY_EDITOR)
using System;
using UnityEditor;
using UnityEngine;
using kazgame.utils;

namespace kazgame.debug
{
	public class DebugCircleScope : MonoBase
	{
		public Color color = Color.green;
		public bool debugDraw = true;
		public float radius;
		public float opasity = 0.2f;

		void OnDrawGizmos () {
			if (!debugDraw){
				return;
			}


			color.a = opasity;
			Gizmos.color = color;
			Gizmos.DrawSphere (transform.position,radius);
		}
	}
}

#endif

