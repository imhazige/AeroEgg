using System;
using UnityEngine;
using System.Collections.Generic;
using kazgame.utils;

namespace kazgame
{
	public class TraceCamerArticulateTile : MonoBase
	{
		
		#if (UNITY_EDITOR)

		/// <summary>
		/// use this to show tiles group 2x times, design the scene
		/// </summary>
		public int editorShowTime = 1;

		[ContextMenu ("show based on editorShowTime")]
		public void EdShowInEditor(){
			InitSelf ();
			GameObjectUtils.CleanChild (gameObject);
			_renders = new List<SpriteRenderer> ();
			_startPos = transform.position;
			int i = 0;
			do{
				ReverseBuild();
				i++;
			}while(i < editorShowTime);
		}

		#endif

		public List<SpriteRenderer> sprites;

		public Camera traceCamera;

		public Direction2D direction2D = Direction2D.down;

		/// <summary>
		/// when camera move, some tile will be invisible, this check when the pos is more far than 
		/// this value X screen size, it will move to comming screen
		/// this is need for some time you control the camera move, you need avoid move the object 
		/// too earlier
		/// </summary>
		public float retainPastScreenSize = 1;

		private List<SpriteRenderer> _renders;

		private Vector2 _unitSize;
		private Vector3 _startPos;
		private Vector3 _lastCameraPos;

		private Vector2 _preCaculateSize;


		protected override void DoInit ()
		{
			base.DoInit ();
			if (null == traceCamera) {
				traceCamera = Camera.main;
			}
			_unitSize = sprites [0].bounds.size;
			InitFit ();
		}

		void Awake ()
		{
			InitSelf ();
		}

		void Update(){
			Adjust ();
		}

		private void Adjust(){
			Vector3 curCameraPos = traceCamera.transform.position;
			Vector3 mpoint = Vector3.zero;

			//move whole, for support backforward
			int moveTime = 0;
			switch (direction2D) {
			case Direction2D.down:
				{
					moveTime = (int)((curCameraPos.y - _lastCameraPos.y) / _preCaculateSize.y);
					if (curCameraPos.y > _lastCameraPos.y){
						//back, move one more time, let it adjust at later
						moveTime += 1;
					}

					break;
				}
			default:
				throw new Exception(String.Format("have not implement for direction {0}",direction2D));
			}

			if (0 != moveTime){
				foreach (SpriteRenderer item in _renders) {
					Vector3 pos = item.transform.position;
					switch (direction2D) {
					case Direction2D.down:
						{
							pos.y += _preCaculateSize.y * moveTime;

							break;
						}
					default:
						throw new Exception(String.Format("have not implement for direction {0}",direction2D));
					}
					item.transform.position = pos;
				}
			}

			foreach (SpriteRenderer item in _renders) {
				Vector3 pos = item.transform.position;
				switch (direction2D) {
				case Direction2D.down:
					{
						if (Vector3.zero == mpoint){
							mpoint = traceCamera.ScreenToWorldPoint(new Vector3(0,(1 + retainPastScreenSize) * Screen.height,0));
						}
						if (pos.y - _unitSize.y > mpoint.y) {
//							Log.Debug ("adjust :  {0}--{1}--{2}--{3}",pos.y , _unitSize.y , mpoint.y,pos.y - _unitSize.y);
							pos.y -= _preCaculateSize.y;
						}

						break;
					}
				default:
					throw new Exception(String.Format("have not implement for direction {0}",direction2D));
				}
				item.transform.position = pos;
			}

			_lastCameraPos = curCameraPos;
		}

		private void InitFit ()
		{
			_lastCameraPos = traceCamera.transform.position;
			GameObjectUtils.CleanChild (gameObject);
			_renders = new List<SpriteRenderer> ();
			_startPos = transform.position;
//			Log.Debug ("start pos {0}",_startPos);

			bool needdo = true;
			int i = 0; 
			do{
				ReverseBuild();
				switch (direction2D) {
				case Direction2D.down:
					{
						Vector3 mpoint = traceCamera.ScreenToWorldPoint(new Vector3(0,0 - Screen.height,0));
						needdo = _startPos.y > mpoint.y;
//						Log.Debug ("break ok....");
						break;
					}
				default:
					throw new Exception(String.Format("have not implement for direction {0}",direction2D));
				}
				i++;
			}while(needdo && i < 5);
		}

		private void ReverseBuild ()
		{
			Vector3 pos = Vector2.zero;
			Vector3 pos1 = Vector2.zero;
			int rCount = _renders.Count / (sprites.Count * 2);
			for (int i = 0; i < sprites.Count; i++) {
				SpriteRenderer sp = sprites [i];

				SpriteRenderer sr = Instantiate<SpriteRenderer>(sp);
				GameObject ga = sr.gameObject;
				ga.name = gameObject.name + "-" + rCount + "-" + i;
				ga.transform.SetParent (transform);
				_renders.Add (sr);
				//reverse
				SpriteRenderer sr1 = Instantiate<SpriteRenderer>(sp);
				GameObject ga1 = sr1.gameObject;
				ga1.name = gameObject.name + "-" + rCount + "-" + i + "_r";
				ga1.transform.SetParent (transform);
				_renders.Add (sr1);
				switch (direction2D) {
				case Direction2D.down:
					{
						pos = new Vector2 (0, -i * _unitSize.y);
						pos1 = new Vector2 (0, - (sprites.Count + sprites.Count - i -1) * _unitSize.y);
						//reverse
						Vector2DUtils.UpsideDown(sr1.transform);
						break;
					}
				default:
					break;
				}
				pos = _startPos + pos;
				pos.z = transform.position.z;
				sr.transform.position = pos;
				pos1 = _startPos + pos1;
				pos1.z = transform.position.z;
				sr1.transform.position = pos1;
			}
			_preCaculateSize = _renders.Count * _unitSize;
			switch (direction2D) {
			case Direction2D.down:
				{
					_startPos.y = transform.position.y - _preCaculateSize.y;
					break;
				}
			default:
				break;
			}

		}
	}
}

