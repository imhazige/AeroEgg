using System;
using UnityEngine;
using kazgame.utils;

namespace kazgame.ui
{
	/// <summary>
	/// this is for non-ui object, you should use ui sliderbar
	/// </summary>
	[Obsolete("this is for non-ui object, you should use ui sliderbar.")]
	public class TouchableSlider : MonoBehaviour
	{
		private BoxCollider2D _collider;
		private Vector2[] _boxScope;
		private float _value;
		private bool _vertical;

		public Transform tip;

		private TouchDetector _detector;

		public delegate void OnValueChangeDelegate(float value,float minPos,float maxPos);
		public OnValueChangeDelegate OnValueChange;

		void Awake(){
			_collider = GetComponent<BoxCollider2D>();
			_detector = new TouchDetector (_collider);
			_boxScope = Vector2DUtils.GetBoxCollider2DScope (_collider);
			_vertical = Mathf.Abs (_boxScope [1].y - _boxScope [0].y) > Mathf.Abs (_boxScope [1].x - _boxScope [0].x);

			_detector.OnTouchEvent += OnTouchEvent ;
		}

		void Start(){
		}

		void Update(){
			_detector.Update ();
		}

		public BoxCollider2D checkCollider{
			get{ 
				if (null == _collider){
					_collider = GetComponent<BoxCollider2D> ();
				}
				return _collider;
			}
		}

		private void UpdatePoint(Vector3 pos){
			float minpos = _vertical ? _boxScope [0].y : _boxScope [0].x;
			float maxpos = _vertical ? _boxScope [1].y : _boxScope [1].x;

			_value = _vertical ? pos.y - _boxScope [0].y:pos.x - _boxScope [0].x;
			_value = _value / (maxpos - minpos);
			_value = Mathf.Clamp (_value,0,1);

			if (null != OnValueChange){
				OnValueChange (_value,minpos,maxpos);
			}

			UpdateTip ();
		}

		void OnTouchEvent (int state, Vector3 position)
		{
			if (1 == state){
				UpdatePoint (position);
			}
		}

		public float value{
			get{ 
				return _value;
			}
			set { 
//				Log.Debug ("set value {0}",value);
				_value = value;
				UpdateTip ();
			}
		}

		private void UpdateTip(){
			float minpos = _vertical ? _boxScope [0].y : _boxScope [0].x;
			float maxpos = _vertical ? _boxScope [1].y : _boxScope [1].x;

			float valuepos = minpos + value * (maxpos - minpos);
			Vector3 tippos = tip.position;
			if (null != tip){
				if (_vertical) {
					tippos.y = valuepos;
				} else {
					tippos.x = valuepos;
				}
				tip.position = tippos;
			}
		}

	}
}

