using System;
using UnityEngine;
using kazgame.utils;

namespace kazgame
{
	public class TracerCameraController : PauseAble
	{
		/// <summary>
		/// Distance in the x axis the player can move before the camera follows.
		/// 0 mean will trace the player x directly
		/// </summary>
		public float xMargin = 1f;

		/// <summary>
		/// Distance in the y axis the player can move before the camera follows.
		/// 0 mean will trace the player y directly
		/// </summary>
		public float yMargin = 1f;

		/// <summary>
		/// How smoothly the camera catches up with it's target movement in the x axis.
		/// the larger the value is, the faster the camera will move
		/// but 0 mean will trace the player x directly
		/// </summary>
		public float xSmooth = 8f;
		/// <summary>
		/// How smoothly the camera catches up with it's target movement in the y axis.
		/// the larger the value is, the faster the camera will move
		/// 0 mean will trace the player y directly
		/// </summary>
		public float ySmooth = 8f;

		public Vector2 scopeX;
		public Vector2 scopeY;

		public bool traceX = true;
		public bool traceY = true;

		private Transform player;

		bool _pausing;

		private Camera camera;
		// Reference to the player's camera.transform.

		bool CheckXMargin ()
		{
			// Returns true if the distance between the camera and the player in the x axis is greater than the x margin.
			return Mathf.Abs (camera.transform.position.x - player.position.x) > xMargin;
		}


		bool CheckYMargin ()
		{
			// Returns true if the distance between the camera and the player in the y axis is greater than the y margin.
			return Mathf.Abs (camera.transform.position.y - player.position.y) > yMargin;
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

		public void StartTrace (Camera camera, Transform player)
		{
			this.player = player;
			this.camera = camera;
			_pausing = false;
		}

		public void StopTrace ()
		{
			this.player = null;
			this.camera = null;
			_pausing = false;
		}

		public void Update ()
		{
			if (_pausing) {
				return;
			}
			if (null == player) {
				return;
			}
			TrackPlayer ();
		}


		void TrackPlayer ()
		{
			// By default the target x and y coordinates of the camera are it's current x and y coordinates.
			float targetX = camera.transform.position.x;
			float targetY = camera.transform.position.y;

			//			Vector2 maxXAndY = scope.getScopeMaxPoint ();
			//			Vector2 minXAndY = scope.getScopeMinPoint();

			float halfHeight = camera.orthographicSize;
			float halfWidth = halfHeight * camera.aspect;


			if (scopeX != default(Vector2)){
				scopeX = new Vector2 (scopeX.x + halfWidth,scopeX.y-halfWidth);
			}

			if (scopeY != default(Vector2)){
				scopeY = new Vector2 (scopeY.x + halfHeight,scopeY.y-halfHeight);
			}

			// The target x and y coordinates should not be larger than the maximum or smaller than the minimum.
			if (traceX){
				// If the player has moved beyond the x margin...
				if (xSmooth > 0) {
					if (CheckXMargin ()) {
						// ... the target x coordinate should be a Lerp between the camera's current x position and the player's current x position.
						targetX = Mathf.Lerp (camera.transform.position.x, player.position.x, xSmooth * Time.deltaTime);
					}
				} else {
					//if need not smooth,trace the player directly
					targetX = player.position.x;
				}
				if (scopeX != default(Vector2)){
					targetX = Mathf.Clamp (targetX, scopeX.x, scopeX.y);	
				}
			}

			if (traceY){
				// If the player has moved beyond the y margin...
				if (ySmooth > 0) {
					if (CheckYMargin ()) {
						// ... the target y coordinate should be a Lerp between the camera's current y position and the player's current y position.
						targetY = Mathf.Lerp (camera.transform.position.y, player.position.y, ySmooth * Time.deltaTime);
					}
				} else {
					//if need not smooth,trace the player directly
					targetY = player.position.y;
				}
				if (scopeY != default(Vector2)){
					targetY = Mathf.Clamp (targetY, scopeY.x, scopeY.y);
				}
			}

			// Set the camera's position to the target position with the same z component.
			camera.transform.position = new Vector3 (targetX, targetY, camera.transform.position.z);
		}
	}

}