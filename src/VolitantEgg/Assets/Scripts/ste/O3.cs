using System;
using UnityEngine;
using kazgame.utils;
using kazgame.animation;

namespace kazgame.ste
{
	public class O3 : Actor
	{

		public float extraLineSize = 0.01f;
		public Color extraLineColor = Color.white;
		public Material lineMaterial;

		public float crashSpeed {
			get;
			set;
		}

		private EdgeCollider2D[] extraLines;
		private CircleCollider2D _circleCollider;

		protected override void DoInit ()
		{
			base.DoInit ();

			if (crashSpeed <= 0f){
				crashSpeed = Constants.o3_crash_speed;
			}

			extraLines = GetComponentsInChildren<EdgeCollider2D> ();
			_circleCollider = GetComponent<CircleCollider2D> ();
			_circleCollider.isTrigger = true;
			Vector2DUtils.ChangePositionZ (transform,(float)ZIndexs.o3);
		}

		void Awake(){
			InitSelf ();
		}

		void Start(){
			foreach (EdgeCollider2D item in extraLines) {
				item.isTrigger = true;

				DrawExtraLine (item);
			}
		}

		LineRenderer DrawExtraLine(EdgeCollider2D guid){
			Vector3[] points  = new Vector3[guid.points.Length];
			for (int i = 0; i < points.Length; i++) {
				Vector3 p = points [i];
				Vector3 pp = guid.transform.TransformPoint (guid.points [i]);
				p.x = pp.x;
				p.y = pp.y;
				p.z = transform.position.z;
				points [i] = p;
			}
			LineRenderer line = guid.gameObject.AddComponent<LineRenderer> ();
			line.startWidth = line.endWidth = extraLineSize;
			line.startColor = line.endColor = extraLineColor;
			line.useWorldSpace = true;
			line.receiveShadows = false;
			line.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
			line.positionCount = points.Length;
			line.SetPositions (points);
			line.material = lineMaterial;

			return line;
		}

		void OnTriggerEnter2D (Collider2D parCollider)
		{
			if (null != parCollider && parCollider.gameObject == GameController.GetSingleton().levelsController.eggController.gameObject){
				float speed = GameController.GetSingleton ().levelsController.eggController.speed;
				if (speed < crashSpeed) {
					CheckCollideWithEgg (parCollider, GameOverReason.obstacle, "O3");
				} else {
					Log.Debug ("o3 destroyed by egg with speed {0}",speed);
					gameObject.SetActive (false);
				}
			}else if (null != parCollider ){
				O4 o4 = parCollider.GetComponent<O4> ();
				if (null != o4){
					Log.Debug ("o3 destroyed by o4");
					gameObject.SetActive (false);
				}
			}
		}
	}
}

