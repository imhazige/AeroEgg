using System;
using UnityEngine;
using kazgame.utils;

namespace kazgame.ui
{
	public class TouchDetector
	{
		public delegate void OnMouseEventDelegate (int state, Vector3 position);

		public OnMouseEventDelegate OnTouchEvent;
		public bool enable = true;

		private bool _touching;

		private Collider2D checkCollider;

		public TouchDetector(Collider2D collider){
			checkCollider = collider;
		}

		public void Update ()
		{
			if (!enable){
				return;
			}
			Vector3 point = new Vector3 ();
			bool isMouseUp = false;
			bool isMouseDown = false;
			#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR
			// If mouse button down, remove old line and set its color to green
			if (Input.GetMouseButtonDown (0)) {
				isMouseDown = true;
			} else if (Input.GetMouseButtonUp (0)) {
				isMouseUp = true;
			}
			point = Input.mousePosition;
			#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

			//Check if Input has registered more than zero touches
			if (Input.touchCount > 0)
			{
			//Store the first touch detected.
			Touch myTouch = Input.touches[0];

			//Check if the phase of that touch equals Began
			if (myTouch.phase == TouchPhase.Began)
			{
			isMouseDown = true;
			}
			//If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
			else if (myTouch.phase == TouchPhase.Ended)
			{
			isMouseUp = true;
			}
			point =  (Vector3)myTouch.position;
			}

			#endif

			//			Log.Debug ("mouse up{0},down{1},pos{2}",isMouseUp,isMouseDown,point);

			point = Camera.main.ScreenToWorldPoint (point);
			if (isMouseUp) {
				if (_touching) {
					_touching = false;	

					FireEvent (2,point);
				}
			} else if (isMouseDown) {
				if (null == checkCollider || checkCollider == Physics2D.OverlapPoint (point)) {
					_touching = true;
					FireEvent (0,point);
				}
			}

			if (_touching) {
				FireEvent (1,point);
			}
		}

		private void FireEvent (int state, Vector3 position)
		{
			if (!enable){
				return;
			}
			if (null == OnTouchEvent) {
				return;
			}

			OnTouchEvent (state, position);
		}
	}
}

