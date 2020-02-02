using System;
using UnityEngine;
using UnityEngine.UI;
using kazgame.utils;

namespace kazgame.ui
{
	public class UITextIntEffect : MonoBase
	{
		/// <summary>
		/// The speed of increasing, it should depend on the unit you want to increase every time,
		/// if you set speed to 1 and you want change value to 10, it will cost 10 seconds, if you set speed to
		/// 10, it will spend 1 seconds.
		/// normally it should be 10 times than the value you normally will increase
		/// </summary>
		private int _speed = 100;

		private Text _text;
		private int _value;
		private int _targetValue;

		protected override void DoInit ()
		{
			base.DoInit ();

			_text = GetComponent<Text> ();
		}

		void Awake(){
			InitSelf ();
		}

		/// <summary>
		/// Changes the value.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <param name="speed">if 0 (default), will 10 times of the scope, if -1, will not have any effect, set to the value directly</param>
		public void ChangeValue(int value,int speed = 0){
			_targetValue = value;
			_speed = speed;
			if (0 == _speed){
				_speed = (_targetValue - _value) * 10;
			}else if (-1 == _speed){
				_value = _targetValue;
				_text.text = _value + "";
			}
		}

		public void Reset(){
			InitSelf ();
			_value = 0;
			_targetValue = 0;
			_text.text = _value + "";
		}

		void Update(){
			if (_targetValue == _value){
				_text.text = _value + "";
				return;
			}

			float v = Mathf.MoveTowards (_value,_targetValue, Mathf.Abs(_speed) * Time.deltaTime);
//			Log.Debug ("target is {0} > {2} > {1}" , _value,_targetValue,v);
			if (0.001f > Mathf.Abs (v - _targetValue)) {
				_value = _targetValue;
			} else {
				int v1 = (int)v;
				if (v1 == _value){
					//the round will keep 0
					v1 += 1;
				}
				_value = v1;
			}


			_text.text = _value + "";
		}
	}
}

