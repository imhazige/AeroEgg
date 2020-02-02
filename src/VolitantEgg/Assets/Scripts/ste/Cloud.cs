using System;
using System.Collections;
using UnityEngine;
using kazgame.utils;

namespace kazgame.ste
{
	public class Cloud : MonoBase,PauseAble
	{
		public bool running {
			get;
			private set;
		}

		private float height;

		private float speed;

		private bool pausing;

		private float scale;

		private Coroutine _cor;

		private SpriteRenderer render;

		protected override void DoInit ()
		{
			base.DoInit ();
			render = GetComponent<SpriteRenderer> ();
		}

		void Awake(){
			InitSelf ();
		}

		public void StartMove(float height, float speed, float scale){
			InitSelf ();

			pausing = false;
			this.height = height;
			this.speed = speed;
			this.scale = scale;
			if (null != _cor){
				StopCoroutine (_cor);
				_cor = null;
			}
			running = true;
			transform.localScale = new Vector3(scale,scale,1);
			Vector2[] scope = Vector2DUtils.GetScreenScope();
			float xbound = render.bounds.size.x * .5f;
			float rightX = scope [1].x + xbound;
			float leftX = scope [0].x - xbound;
			float startX = speed < 0?rightX:leftX;
			float targetX = speed < 0?leftX:rightX;
			Vector2DUtils.ChangePositionX (transform,startX);
			gameObject.SetActive (true);
			_cor = StartCoroutine (DoUpdate(targetX));
		}

		public void StopShow(){
			InitSelf ();
			if (null != _cor){
				StopCoroutine (_cor);
				_cor = null;
			}
			running = false;
			gameObject.SetActive (false);
		}

		IEnumerator DoUpdate (float targetX)
		{
			while (true) {
				if (!pausing){
					Vector2[] scope = Vector2DUtils.GetScreenScope();
					float y = scope [0].y + height;
					Vector3 targetPos = Vector2.MoveTowards (transform.position,new Vector2(targetX,transform.position.y),Mathf.Abs(speed)*Time.deltaTime);
					targetPos.y = y;
					targetPos.z = transform.position.z;
					transform.position = targetPos;

					if (Math.Abs (transform.position.x - targetX) < Mathf.Epsilon) {
						running = false;
						gameObject.SetActive (false);
						break;
					}
				}

				yield return null;
			}
		}

		#region PauseAble implementation

		public void OnPause ()
		{
			pausing = true;
		}

		public void OnResume ()
		{
			pausing = false;
		}

		#endregion
	}
}

