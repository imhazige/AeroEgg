using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using kazgame.utils;

namespace kazgame
{
	[ExecuteInEditMode]
	public class StraightLineCollider2D : MonoBase {

		public Direction2D direction;
		public float length;
		/// <summary>
		/// collider ply, too thin may cause fast move object pass through
		/// </summary>
		public float ply = .03f;

		//use assign, do not use code to create, otherwise will create many
		public BoxCollider2D box;

		#if UNITY_EDITOR
		void OnDrawGizmos () {
			ResetEdge ();
		}
		#endif

		protected override void DoInit ()
		{
			base.DoInit ();

			if (null == box){
				box = GetComponent<BoxCollider2D> ();
			}
			ResetEdge ();
		}

		void Awake(){
			InitSelf ();
		}

		private void ResetEdge(){
			Vector2 offset = Vector2.zero;
			Vector2 colliderOffset = new Vector2 (length*.5f,ply*.5f);
			Vector2 colliderSize = Vector2.zero;

			switch (direction) {
			case Direction2D.right:
				{
					colliderOffset = new Vector2 (colliderOffset.x,0);
					colliderSize = new Vector2 (length,ply);
					break;
				}
			case Direction2D.up:
				{
					colliderOffset = new Vector2 (0,colliderOffset.x);
					colliderSize = new Vector2 (ply,length);
					break;
				}
			case Direction2D.left:
				{
					colliderOffset = new Vector2 (-colliderOffset.x,0);
					colliderSize = new Vector2 (length,ply);
					break;
				}
			case Direction2D.down:
				{
					colliderOffset = new Vector2 (0,-colliderOffset.x);
					colliderSize = new Vector2 (ply,length);
					break;
				}
			case Direction2D.center:
				{
					colliderOffset = new Vector2 (0,0);
					colliderSize = new Vector2 (ply,length);
					break;
				}
			default:
				break;
			}

			box.offset = colliderOffset;
			box.size = colliderSize;
		}
	}
}
