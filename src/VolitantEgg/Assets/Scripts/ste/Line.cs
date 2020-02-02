using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using kazgame.gamesave;
using kazgame.serialize;
using kazgame.utils;
using kazgame.objectpool;

namespace kazgame.ste
{

	public class Line : MonoBase,GameSaverable,Poolable,PauseAble
	{
		public float lineWidth = 0.5f;
		public float warnLifeEndTime = 1f;
		public Color lineColor = Color.red;
		public Color lineToEndColor = Color.yellow;

		public Material mLineMaterial;

		public float lifeTime = 3f;

		public event Action<Line> onLineLifeEndEvent;
		public event Action<Line,Collision2D> OnLineCollisionEnter2D;
		public event Action<Line,Collision2D> OnLineCollisionExit2D;

		public event Action<Line,Collider2D> OnLineTriggerEnter2D;
		public event Action<Line,Collider2D> OnLineTriggerExit2D;

		private LineRenderer line;

		private List<Vector3> pointsList;

		private EdgeCollider2D edgeCollider = null;

		private float _lifeLeftTime = -1f;
		private bool lineFinished = false;

		private float lineLength = 0f;

		private bool _active;

		private bool _pausing;

		protected override void DoInit ()
		{
			base.DoInit ();

			// Create line renderer component and set its property
			line = gameObject.AddComponent<LineRenderer> ();
			//			line.material = new Material (Shader.Find ("Particles/Additive"));
			line.material = mLineMaterial;

			Reset ();
		}

		void Awake ()
		{
			InitSelf ();
		}

		void Start ()
		{
			edgeCollider = gameObject.AddComponent<EdgeCollider2D> ();
//			edgeCollider.isTrigger = true;
		}

		#region PauseAble implementation

		public void OnPause ()
		{
			_pausing = true;
		}

		public void OnResume ()
		{
			_pausing = false;
		}

		#endregion

		void Update ()
		{
			if (able2Pool){
				return;
			}
			if (_pausing){
				return;
			}
//			Log.Debug ("sssss {0}",lifeLeftTime);
			_lifeLeftTime -= Time.deltaTime;
			if (lifeLeftTime <= 0) {
				DestroyLine ();
			} else if (lifeLeftTime < warnLifeEndTime) {
				//					line.SetColors (lineToEndColor, lineToEndColor);
				line.startColor = line.endColor = lineToEndColor;
			} 
		}

		#region Poolable implementation

		public void OnActiveFromPool ()
		{
			Reset ();
			_active = true;
			gameObject.SetActive (true);
		}

		public bool able2Pool {
			get {
				return !_active;
			}
		}

		#endregion

		#region GameSaverable implementation

		Hashtable GameSaverable.SaveGame ()
		{
			Hashtable hs = new Hashtable ();

			hs.Add ("points", SVector3.List (pointsList));
			hs.Add ("leftTime",_lifeLeftTime);

			return hs;
		}

		void GameSaverable.LoadGame (Hashtable data)
		{
			if (null == data) {
				return;
			}

			List<SVector3> ls = (List<SVector3>)data ["points"];

			if (null != ls) {
				foreach (SVector3 sv  in ls) {
					updatePoint (sv.ToVector3 ());
				}
			}

			_lifeLeftTime = (float)data["leftTime"];
		}

		#endregion

		public float lifeLeftTime {
			get { 
				return _lifeLeftTime;
			}
		}

		public void OnSpecialTriggerEnter2D (Collider2D other)
		{
			if (null != OnLineTriggerEnter2D) {
				OnLineTriggerEnter2D (this, other);
			}
		}

		void OnCollisionEnter2D (Collision2D other)
		{
//			Debug.LogFormat ("Line OnTriggerEnter2D,{0}", other);
			if (null != OnLineCollisionEnter2D) {
				OnLineCollisionEnter2D (this, other);
			}
		}

		void OnCollisionExit2D (Collision2D other)
		{
			if (null != OnLineCollisionExit2D) {
				OnLineCollisionExit2D (this, other);
			}
		}


		void OnTriggerEnter2D (Collider2D other)
		{
//			Debug.LogFormat ("Line OnTriggerEnter2D,{0}", other);
			if (null != OnLineTriggerEnter2D) {
				OnLineTriggerEnter2D (this, other);
			}
		}

		void OnTriggerExit2D (Collider2D other)
		{
//			Debug.LogFormat ("Line OnTriggerExit2D,{0}",other);
			if (null != OnLineTriggerExit2D) {
				OnLineTriggerExit2D (this, other);
			}
		}

		void Reset(){
			line.positionCount = 0;
			line.startWidth = line.endWidth = lineWidth;
			line.startColor = line.endColor = lineColor;
			line.useWorldSpace = true;
			line.receiveShadows = false;
			line.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;

			_lifeLeftTime = lifeTime;
			lineFinished = false;
			lineLength = 0f;

			pointsList = new List<Vector3> ();
			_pausing = false;
		}

		void DestroyLine ()
		{
			if (null != onLineLifeEndEvent) {
				onLineLifeEndEvent (this);	
			}
			gameObject.SetActive (false);
			_active = false;
		}

		public void ForceDestroyLine ()
		{
			if (!lineFinished) {
				finishLine ();
			}
			DestroyLine ();
		}

		public void updatePoint (Vector3 newPoint)
		{
//			Debug.Log ("line update." + Time.frameCount);
			newPoint.z = transform.position.z;
			if (!pointsList.Contains (newPoint)) {
				pointsList.Add (newPoint);
//				line.SetVertexCount (pointsList.Count);
				line.positionCount = pointsList.Count;
				line.SetPosition (pointsList.Count - 1, (Vector3)pointsList [pointsList.Count - 1]);
				//TODO match the collider

				updateCollider ();

				lineLength = caculateLineLength (pointsList);
			}
		}

		public void FastEnd ()
		{
			_lifeLeftTime = Mathf.Min (_lifeLeftTime, warnLifeEndTime);
		}

		void updateCollider ()
		{
			if (!edgeCollider) {
				return;	
			}
			int dc = pointsList.Count * 2;
			Vector2[] points = new Vector2[dc + 1];	

			float boxWidth = lineWidth / 2;

			for (int i = 0; i < pointsList.Count - 1; i++) {
				Vector3 vs = pointsList [i];
				Vector3 ve = pointsList [i + 1];

				float angle = Mathf.Atan ((ve.y - vs.y) / (ve.x - vs.x));

				float xdis = boxWidth * Mathf.Sin (angle);
				float ydis = boxWidth * Mathf.Cos (angle);

				Vector2 v0 = new Vector2 (vs.x - xdis, vs.y + ydis);
				Vector2 v1 = new Vector2 (vs.x + xdis, vs.y - ydis);

				if (0 == i) {
					points [dc] = v0;	
				}


//				Debug.LogFormat ("{0} = {1}", i, v0);
//				Debug.LogFormat ("{0} = {1}", dc - 1 - i, v1);

				points [i] = v0;
				points [dc - 1 - i] = v1;

				if (i + 1 == pointsList.Count - 1) {
					v0 = new Vector2 (ve.x - xdis, ve.y + ydis);
					v1 = new Vector2 (ve.x + xdis, ve.y - ydis);


					points [i + 1] = v0;
					points [i + 2] = v1;



				}
			}

			//the collider origin point is the center of the game object
			edgeCollider.offset = transform.position * -1;
			edgeCollider.points = points;
		}

		public void finishLine ()
		{
			lineFinished = true;
		}

		public float getLineLength ()
		{
			return lineLength;
		}

		private float caculateLineLength (List<Vector3> points)
		{
			if (null == points || 0 == points.Count) {
				return 0f;
			}
			float length = 0f;
			for (int i = 0; i < points.Count - 1; i++) {
				Vector3 p0 = points [i];
				Vector3 p1 = points [i + 1];
				length += Vector3.Distance (p0, p1);
			}

			return length;
		}

		public List<Vector3> GetLinePoints ()
		{
			return new List<Vector3> (pointsList);
		}
	}
}