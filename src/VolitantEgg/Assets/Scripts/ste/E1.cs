using System;
using UnityEngine;
using kazgame.animation;
using System.Collections;
using kazgame.utils;

namespace kazgame.ste
{
	public class E1 : Actor
	{
		public EdgeCollider2D flyGuid;
		public SpriteAnimator animIdle;
		public SpriteAnimator animFly;
		public float flySpeed = 4f;
		public float sensibleDistance = 2f;

		private Coroutine _cor;
		private int _flyIndex;
		private Vector3[] flyPoints;

		protected override void DoInit ()
		{
			base.DoInit ();
			if (null == flyGuid){
				flyGuid = GetComponentInChildren<EdgeCollider2D> ();
			}
			flyGuid.isTrigger = true;
			_cor = StartCoroutine (DoMyUpdate());
			Vector2DUtils.ChangePositionZ (transform,(float)ZIndexs.enemy);
		}

		void Awake(){
			InitSelf ();
		}

		void Start(){
			flyPoints = new Vector3[flyGuid.points.Length];
			for (int i = 0; i < flyGuid.points.Length; i++) {
				Vector3 fp =  flyGuid.points[i];
				flyPoints[i] = flyGuid.transform.TransformPoint(fp);
				flyPoints [i].z = transform.position.z;
			}
//			Log.Debug ("all fly points {0}",LogUtils.logList(flyPoints));
			animIdle.StartPlay ();
		}

		public override void OnPause ()
		{
			base.OnPause ();
			animIdle.OnPause ();
			animFly.OnPause ();
		}

		public override void OnResume ()
		{
			base.OnResume ();
			animIdle.OnResume ();
			animFly.OnResume ();
		}

		void OnTriggerEnter2D (Collider2D parCollider)
		{
			CheckCollideWithEgg (parCollider,GameOverReason.enemy,"E1");
			CheckCollideWithLine (parCollider);
		}

		IEnumerator DoMyUpdate()
		{
			while (true) {
				if (null == GameController.GetSingleton () || null == GameController.GetSingleton ().levelsController || null == GameController.GetSingleton ().levelsController.eggController){
					//for design
					yield return null;				
				}
				Vector3 pos = GameController.GetSingleton ().levelsController.eggController.transform.position;
				float dis = Mathf.Abs ((pos - transform.position).magnitude);
				//Log.Debug ("E1 see egg {0}",dis);
				if (sensibleDistance > dis){
					Log.Debug ("E1 see egg {0}",dis);
					//start fly
					animIdle.StopPlay();
					animFly.StartPlay ();
					GameController.singleton.audioController.PlayBirdFly ();
					break;
				}

				yield return null;
			}

			while (true) {
				if (!Vector2DUtils.IsInScreen (transform.position)) {
					Log.Debug ("E1 out of screen");
					gameObject.SetActive (false);

					yield break;
				}
				if (_flyIndex > flyPoints.Length - 1){
					gameObject.SetActive (false);
					break;
				}
				Vector3 fp = flyPoints [_flyIndex];
				Vector2DUtils.MoveToSmoothly (transform,fp,flySpeed);
				if (fp == transform.position){
					_flyIndex++;
				}

				yield return null;
			}
		}
	}
}

