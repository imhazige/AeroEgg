using System;
using UnityEngine;
using kazgame.animation;
using kazgame.utils;
using System.Collections;

namespace kazgame.ste
{
	public class ButterFly : Actor
	{
		private SpriteAnimator animator;
		private SpriteRenderer render;
		private Vector3 startAngulars;
		private Color[] colors;

		private Coroutine _cor;

		protected override void DoInit ()
		{
			base.DoInit ();

			animator = GetComponent<SpriteAnimator> ();
			animator.loopCount = 0;
			animator.playOnStart = false;
			render = GetComponent<SpriteRenderer> ();
			startAngulars = transform.eulerAngles;
			colors = new Color[]{ Color.yellow,ColorUtils.HexToColor("E18624FF"),ColorUtils.HexToColor("9A9EE8FF") };
		}

		public override void OnPause ()
		{
			base.OnPause ();
			animator.OnPause ();
		}

		public override void OnResume ()
		{
			base.OnResume ();
			animator.OnResume ();
		}

		protected override void Reset ()
		{
			base.Reset ();
			animator.StopPlay ();
			if (null != _cor){
				StopCoroutine (_cor);
			}
			int colorIndex = UnityEngine.Random.Range (0, colors.Length);
			render.color = colors[colorIndex];
			Vector2[] scope = Vector2DUtils.GetScreenScope();
			float xbound = render.bounds.size.x * .5f;
			float rightX = scope [1].x + xbound;
			float leftX = scope [0].x - xbound;
			float speed = UnityEngine.Random.Range (0.8f,Constants.speed_butterfly_move_max);
			float startX = speed < Mathf.Lerp(0.5f,Constants.speed_butterfly_move_max,0.5f)?rightX:leftX;
			float startY = UnityEngine.Random.Range (scope[0].y,scope[1].y);
			transform.position = new Vector3 (startX,startY,transform.position.z);
			Vector3 target1 = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width * 0.5f, Screen.height * 0.5f, 0));
			target1.z = transform.position.z;
			gameObject.SetActive (true);
			_cor = StartCoroutine (DoUpdate(speed,target1));
		}

		IEnumerator DoUpdate (float speed,Vector3 target)
		{
			animator.StartPlay ();
			while (true) {
				transform.eulerAngles = startAngulars;
				if (target.x < transform.position.x){
					Vector2DUtils.Flip (transform);
				}
				while (transform.position != target) {
//					Log.Debug ("gooooo : {0},{1}",transform.position , target);
					if (Vector2DUtils.IsOutsideScreen(transform.position,Direction2D.up)){
//						Log.Debug ("{0} outof screen.",gameObject.name);
						StopAct ();
						yield break;
					}
					if (!pausing) {
						Vector2DUtils.MoveToSmoothly (transform, target, speed);
					}

					yield return null;
				}

				Vector2[] scope = Vector2DUtils.GetScreenScope();
				float xbound = render.bounds.size.x * .5f;
				float rightX = scope [1].x + xbound + 10;
				float leftX = scope [0].x - xbound - 10;
				target.x = UnityEngine.Random.Range (leftX,rightX);
				target.y = UnityEngine.Random.Range (scope[0].y,scope[1].y);

//				Log.Debug ("move to new pos : {0}",target);
			}
		}
	}
}

