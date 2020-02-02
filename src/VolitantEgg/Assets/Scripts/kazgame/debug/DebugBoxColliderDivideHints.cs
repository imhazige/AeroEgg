#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using kazgame.utils;

namespace kazgame.debug
{
	/// <summary>
	/// will draw debug divide lines of the boxcollider
	/// </summary>
	public class DebugBoxColliderDivideHints : MonoBehaviour
	{
		public int divideX = 0;
		public int divideY = 0;

		public Color color = Color.yellow;
		public Color evenColor = Color.blue;
		public bool debugDraw = true;

		void OnDrawGizmos () {
			if (debugDraw){
				DrawLines ();	
			}
		}

		void DrawLines () {
			if(EditorApplication.isCompiling || EditorApplication.isUpdating){
				return;
			}

			BoxCollider2D[] boxpos = gameObject.GetComponents<BoxCollider2D>();
			for (int i = 0 ; null != boxpos && i < boxpos.Length;i++){
				BoxCollider2D p2d = boxpos [i];
				Vector2[] v2 = Vector2DUtils.GetBoxCollider2DScope (p2d);

				//horizontal divide
				if (divideX > 0){
					float d = (v2 [1].x - v2 [0].x)/divideX;
					for (int j = 0 ; j < divideX ; j++){
						float vx = v2 [0].x + j * d;

						if (j % 2 != 0) {
							Gizmos.color = evenColor;
						} else {
							Gizmos.color = color;
						}
						Gizmos.DrawLine (new Vector2(vx,v2 [0].y),new Vector2(vx,v2 [1].y));
					}
				}

				//vertical divide
				if (divideY > 0){
					float d = (v2 [1].y - v2 [0].y)/divideY;
					for (int j = 0 ; j < divideY ; j++){
						float vy = v2 [0].y + j * d;

						if (j % 2 == 0) {
							Gizmos.color = evenColor;
						} else {
							Gizmos.color = color;
						}
						Gizmos.DrawLine (new Vector2(v2 [0].x,vy),new Vector2(v2 [1].x,vy));
					}
				}
			}
		}
	}
}
#endif