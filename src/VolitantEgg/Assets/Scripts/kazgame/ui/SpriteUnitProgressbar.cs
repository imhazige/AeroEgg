using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using kazgame.utils;

namespace kazgame.ui
{
	public class SpriteUnitProgressbar : MonoBase
	{
		public Sprite spriteDoneUnit;
		public SpriteRenderer unitExample;

		public int unitLength = 3;

		public bool loop;

		public float gapOfUnit = 0;

		private Sprite _spriteOrigin;
		private Vector3 _spriteSize;
		private Coroutine _cor;

		private List<SpriteRenderer> _units;

		private int _loopIndex;

		protected override void DoInit ()
		{
			base.DoInit ();

			_spriteOrigin = unitExample.sprite;
			_spriteSize = _spriteOrigin.bounds.size;
			_units = new List<SpriteRenderer> (unitLength);

			Vector3 pos0 = unitExample.transform.position;
			_units.Add (unitExample);
			float startX = pos0.x - (unitLength - 1) / 2f * (_spriteSize.x + gapOfUnit);
			pos0.x = startX;
			unitExample.transform.position = pos0;
			for (int i = 0 ; i < unitLength - 1; i++){
				GameObject go = new GameObject ();
				go.name = unitExample.transform.parent.name + "-u-" + i;
				go.transform.parent = unitExample.transform.parent;
				Vector3 pos = pos0;
				pos.x = startX + (i + 1) * (_spriteSize.x + gapOfUnit); 
				go.transform.position = pos;
				SpriteRenderer isp = go.AddComponent<SpriteRenderer> ();
				isp.sprite = _spriteOrigin;

				_units.Add (isp);
			}
		}

		void Awake(){
			InitSelf();
		}

		void Start(){
			
		}

		public void StartProgress(){
			InitSelf ();
			if (loop){
				_cor = StartCoroutine (DoLoop());
			}
		}

		public void UpdateProgress(float rate){
			if (loop){
				throw new SystemException ("the mode is loop.");
			}

			int index = (int)(_units.Count * rate) - 1;

			if (index < 0){
				return;
			}

			index = Mathf.Clamp (index,0,_units.Count-1);
//			Log.Info ("indx ===" + index);
			for (int i = 0 ; i <= index; i++){
				SpriteRenderer sp = _units [i];
				sp.sprite = spriteDoneUnit;
			}
		}

		public void FinishProgress(){
			if (null != _cor){
				StopCoroutine (_cor);
			}

			foreach (SpriteRenderer item in _units) {
				item.sprite = spriteDoneUnit;
			}
		}

		protected IEnumerator DoLoop (){
			while (true) {
				SpriteRenderer sp = _units [_loopIndex];
				sp.sprite = _spriteOrigin == sp.sprite ? spriteDoneUnit : _spriteOrigin;
				_loopIndex = (_loopIndex + 1) % _units.Count;
				yield return new WaitForSeconds(1);
			}
		}
	}
}

