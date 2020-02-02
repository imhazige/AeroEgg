using System;
using UnityEngine;
using kazgame.animation;
using System.Collections;
using kazgame.utils;
using kazgame.character;
using System.Collections.Generic;

namespace kazgame.ste
{
	public class E7 : Actor
	{
		public float maxAngle = 45f;
		public float speed = 100f;

		public Transform rotateCenter;
		public Transform board;
		public Material lineMaterial;

		private float extraLineSize = 0.01f;
		private Color extraLineColor = Color.green;

		private Coroutine _cor = null;

		protected override void DoInit ()
		{
			base.DoInit ();

			if (null == rotateCenter){
				rotateCenter = GameObjectUtils.FindAnyOne (this.gameObject, "rotateCenter").transform;
			}
			if (null == board){
				board = GameObjectUtils.FindAnyOne (this.gameObject, "board").transform;
			}
			Vector2DUtils.ChangePositionZ (transform,(float)ZIndexs.enemy);
		}

		protected override void Start ()
		{
			base.Start ();
			_cor = StartCoroutine (DoSwing());
		}

		LineRenderer DrawExtraLine(Vector3 from,Vector3 to){
			Vector3[] points = new Vector3[]{from,to};
			GameObject go = new GameObject ();
			go.transform.SetParent (transform);
			go.name = "vine";

			LineRenderer line = go.AddComponent<LineRenderer> ();
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


		IEnumerator DoSwing ()
		{
			Vector3 _rotateCenterPos = rotateCenter.position;
			_rotateCenterPos.z = transform.position.z;
			float angle = 0;
			while(true){
				if (!pausing){
					float vangle = speed * Time.deltaTime;
					transform.RotateAround (_rotateCenterPos,Vector3.forward,vangle);
					angle += vangle;
					if (angle > maxAngle && speed > 0){
						speed = -speed;
					}else if (angle < -maxAngle && speed < 0){
						speed = -speed;
					}
				}

				yield return null;
			}
				
		}
	}
}

