using System;
using kazgame.animation;
using UnityEngine;
using System.Collections;
using kazgame.utils;

namespace kazgame.ste
{
	public class E10 : Actor
	{
		public SpriteAnimator animWaggle;
		public float angle = 5f;
		public float speed = 20f;

		private Coroutine _cor;
		private Direction2D _direction2D;
		private Transform _branch;
		private Vector3 _startAngulars;
		private Vector3 _branchSpriteSize;


		protected override void DoInit ()
		{
			base.DoInit ();
			if (null == animWaggle){
				animWaggle = GetComponent<SpriteAnimator> ();
			}
			Vector2DUtils.ChangePositionZ (transform,(float)ZIndexs.enemy);
		}

		void Awake(){
			InitSelf ();
		}

		void Start(){
			animWaggle.StartPlay ();
			_branch = transform.parent;
			_branchSpriteSize = _branch.GetComponent<SpriteRenderer> ().bounds.size;
			_direction2D = _branch.gameObject.name.StartsWith ("left")?Direction2D.right:Direction2D.left;
			_startAngulars = transform.eulerAngles;
			_cor = StartCoroutine (DoMyUpdate());
		}

		public override void OnPause ()
		{
			base.OnPause ();
			animWaggle.OnPause ();
		}

		public override void OnResume ()
		{
			base.OnResume ();
			animWaggle.OnResume ();
		}

		void OnTriggerEnter2D (Collider2D parCollider)
		{
			CheckCollideWithEgg (parCollider,GameOverReason.enemy,"E10");
			CheckCollideWithLine (parCollider);
		}

		IEnumerator DoMyUpdate()
		{
			Vector3 center = Vector3.zero;
			if (Direction2D.right == _direction2D) {
				center = _branch.transform.position;
				center.x = center.x - _branchSpriteSize.x * 0.5f;
			} else {
				center = _branch.transform.position;
				center.x = center.x + _branchSpriteSize.x * 0.5f;
			}
			int up = 1;
			while (true) {
				if (Vector2DUtils.IsOutsideScreen (transform.position,Direction2D.up)) {
					Log.Debug ("E10 out of screen");
					gameObject.SetActive (false);
					yield break;
				}
				if (!pausing){
					_branch.transform.RotateAround (center, new Vector3(0,0,1), up * speed * Time.deltaTime);
//					Log.Debug ("erule {0}", transform.eulerAngles);
					float z = transform.eulerAngles.z;
					if (up == 1 && z > angle && z < 180) {
						up = -1;
					} else if (up == -1 && z < 360 - angle && z > 180) {
						up = 1;
					}
				}

				yield return null;
			}
		}
	}
}

