using System;
using UnityEngine;
using kazgame.utils;

namespace kazgame.ui
{
	public class CheckBox : MonoBehaviour
	{
		public delegate void OnValueChangeDelegate(bool value);
		public OnValueChangeDelegate OnValueChange;

		public Sprite spriteUnChecked;

		private Sprite _originCheckedSprite;

		private SpriteRenderer _render;

		private BoxCollider2D _collider;

		private TouchDetector _detector;

		protected virtual void Awake(){
			_render = GetComponent<SpriteRenderer>();

			_originCheckedSprite = _render.sprite;

			_detector = new TouchDetector (checkCollider);
			_detector.OnTouchEvent += OnTouchEvent ;
		}

		protected virtual void Update(){
			_detector.Update ();

		}

		void OnTouchEvent (int state, Vector3 position)
		{
//			Log.Debug ("state{0}",state);
			if (2 == state){
				value = !value;

				if (null != OnValueChange){
					OnValueChange (value);
				}
			}
		}

		public virtual bool value{
			get{ 
				return _render.sprite == _originCheckedSprite;
			}

			set{ 
				if (value) {
					_render.sprite = _originCheckedSprite;
				} else {
					_render.sprite = spriteUnChecked ;
				}
			}
		}

		public BoxCollider2D checkCollider{
			get{ 
				if (null == _collider){
					_collider = GetComponent<BoxCollider2D> ();
				}
				return _collider;
			}
		}
	}
}
