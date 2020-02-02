using System;
using UnityEngine;
using System.Collections;
using kazgame.utils;

namespace kazgame
{
	/// <summary>
	/// use 4 sprite render shape a mask, work on world position
	/// but it have problem, because there are overlap regions, so it not good
	/// </summary>
	public class SpriteAlphaRectMask2D : MonoBase,PauseAble
	{
		public delegate void AnimationDoneCallBack (Vector3 center, Vector2 size);

		public Sprite sprite;

		private SpriteRenderer lr;
		private SpriteRenderer tr;
		private SpriteRenderer rr;
		private SpriteRenderer br;

		private Coroutine cor;



		public bool pausing {
			get;
			private set;
		}

		protected override void DoInit ()
		{
			base.DoInit ();
			lr = createRender (); 
			lr.gameObject.name = "SARMask-left";
			tr = createRender (); 
			tr.gameObject.name = "SARMask-top";
			rr = createRender (); 
			rr.gameObject.name = "SARMask-right";
			br = createRender (); 
			br.gameObject.name = "SARMask-bottom";
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

		private void InitRenders(float alpha){
			
		}

		public void StartMask (Vector3 center, Vector2 size, AnimationDoneCallBack callback,float animateTime = 0f,float alpha = 0.2f)
		{
			InitSelf ();

			if (null != cor){
				StopCoroutine (cor);	
			}

			Color c = Color.black;
			c.a = alpha;
			lr.color = c;
			tr.color = c;
			rr.color = c;
			br.color = c;

			cor = StartCoroutine (DoUpdate(center,size,callback,animateTime));
		}

		IEnumerator DoUpdate (Vector3 center, Vector2 size, AnimationDoneCallBack callback, float animateTime)
		{
			Vector2[] scope = Vector2DUtils.GetScreenScope ();
			//to radius
			size = size * 0.5f;
			Vector3 lStart = new Vector3 (scope[0].x - lr.bounds.size.x * 0.5f,center.y,center.z);
			Vector3 lEnd = lStart;
			lEnd.x = center.x - size.x - lr.bounds.size.x * 0.5f;

			Vector3 tStart = new Vector3 (center.x,scope[1].y + tr.bounds.size.y * 0.5f,center.z);
			Vector3 tEnd = tStart;
			tEnd.y = center.y + size.y + tr.bounds.size.y * 0.5f;

			Vector3 rStart = new Vector3 (scope[1].x + rr.bounds.size.x * 0.5f,center.y,center.z);
			Vector3 rEnd = rStart;
			rEnd.x = center.x + size.x + rr.bounds.size.x * 0.5f;

			Vector3 bStart = new Vector3 (center.x,scope[0].y - br.bounds.size.y * 0.5f,center.z);
			Vector3 bEnd = bStart;
			bEnd.y = center.y - size.y - br.bounds.size.y * 0.5f;

			lr.transform.position = lStart;
			tr.transform.position = tStart;
			rr.transform.position = rStart;
			br.transform.position = bStart;

			if (Math.Abs (animateTime) > Mathf.Epsilon) {
				float lSpeed = ((lEnd - lStart)/animateTime).magnitude;
				float tSpeed = ((tEnd - tStart)/animateTime).magnitude;
				float rSpeed = ((rEnd - rStart)/animateTime).magnitude;
				float bSpeed = ((bEnd - bStart)/animateTime).magnitude;
				float timeUsed = 0;
				while (timeUsed < animateTime){
					if (!pausing){
						timeUsed += Time.deltaTime;
						Vector2DUtils.MoveToSmoothly (lr.transform,lEnd,lSpeed);
						Vector2DUtils.MoveToSmoothly (tr.transform,tEnd,tSpeed);
						Vector2DUtils.MoveToSmoothly (rr.transform,rEnd,rSpeed);
						Vector2DUtils.MoveToSmoothly (br.transform,bEnd,bSpeed);
					}
					yield return null;
				}
			}

			lr.transform.position = lEnd;
			tr.transform.position = tEnd;
			rr.transform.position = rEnd;
			br.transform.position = bEnd;

			callback (center, size);
		}

		public void StopMask ()
		{
			InitSelf ();

			lr.gameObject.SetActive (false);
			tr.gameObject.SetActive (false);
			rr.gameObject.SetActive (false);
			br.gameObject.SetActive (false);

			if (null != cor){
				StopCoroutine (cor);	
			}
		}

		private SpriteRenderer createRender ()
		{
			GameObject go = new GameObject ();
			go.transform.SetParent (transform);
			SpriteRenderer r = go.AddComponent<SpriteRenderer> ();
			r.sprite = sprite;
			go.transform.localScale = new Vector3(1000,1000,1);

			return r;
		}
	}
}

