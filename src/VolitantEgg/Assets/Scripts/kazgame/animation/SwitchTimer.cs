using UnityEngine;
using System.Collections;

namespace kazgame.animation
{
	public class SwitchTimer
	{
		public enum Phase
		{
			MoveOutPrepare,
			MovingOut,
			MoveBackPrepare,
			MoveingBack

		}

		public float switchDuration = 2f;
		public float maxMoveLength = 3f;
		public float moveSpeed;

		private float lastCloseTime = -1f;
		private float lastOpenTime = -1f;

		private float curPos = 0;

		private Phase _phase;
		private bool _started;
		private long _count;

		public void Start ()
		{
			_phase = Phase.MoveOutPrepare;
			lastCloseTime = Time.time;
			lastOpenTime = -1;
			_started = true;
			curPos = 0;
			_count = 0;
		}

		public void Stop ()
		{
			_started = false;
		}

		public bool started {
			get { 
				return _started;
			}
		}

		public long count {
			get { 
				return _count;
			}
		}

		public float Update ()
		{
			if (!_started) {
				return 0f;
			}
			if (0 <= lastOpenTime) {
				//closing
				float gapTime = Time.time - lastOpenTime;
				if (switchDuration <= gapTime) {
					_phase = Phase.MoveingBack;
					float newpos = curPos;
					float moveDis = 0;
					moveDis = newpos - moveSpeed * Time.deltaTime;
					moveDis = Mathf.Max (0, moveDis);
					newpos = moveDis;

					if (moveDis <= 0) {
						lastCloseTime = Time.time;
						lastOpenTime = -1;
						_phase = Phase.MoveOutPrepare;
						_count += 1;
					}
					curPos = newpos;
				} else {
					_phase = Phase.MoveBackPrepare;
				}
			} else {
				//opening
				float gapTime = Time.time - lastCloseTime;
				if (switchDuration <= gapTime) {
					_phase = Phase.MovingOut;
					float newpos = curPos;
					float moveDis = 0;
					moveDis = newpos + moveSpeed * Time.deltaTime;
					moveDis = Mathf.Min (maxMoveLength, moveDis);
					newpos = moveDis;

					if (maxMoveLength <= moveDis) {
						lastOpenTime = Time.time;
						lastCloseTime = -1;
						_phase = Phase.MoveBackPrepare;
					}
					curPos = newpos;
				} else {
					_phase = Phase.MoveOutPrepare;
				}
			}

			return curPos;
		}

		public Phase phase {
			get {
				return _phase;
			}
		}

	}
}
