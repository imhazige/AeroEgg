using UnityEngine;
using System.Collections;
using System;

namespace kazgame.level
{
	public class LevelSegment : MonoBase {
		public float segmentLength = 5f;

		public LevelSegment(){
			#if (UNITY_EDITOR)
			direction2D = Direction2D.up;
			#endif
		}

		#if (UNITY_EDITOR)
		public string description;
		public bool debugDrawLength = true;
		public Color debugDrawColor = Color.yellow;
		/// <summary>
		/// for debug line draw, not used in the logic
		/// </summary>
		/// <value>The direction2 d.</value>
		public Direction2D direction2D {
			get;
			set;
		}

		void OnDrawGizmos() {
			if (debugDrawLength){
				Vector3 to = transform.position;
				switch (direction2D) {
				case Direction2D.down:
					{
						to.y -= segmentLength;
						break;
					}
				case Direction2D.right:
					{
						to.x += segmentLength;
						break;
					}
				default:
					throw new Exception(String.Format("have not implement for direction {0}",direction2D));
				}
				Gizmos.color = debugDrawColor;
				Gizmos.DrawLine (transform.position,to);
			}
		}
		#endif
	}
}
