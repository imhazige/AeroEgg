using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using kazgame.utils;
using kazgame.animation;

namespace kazgame.ste
{
	public class EggController : Actor
	{

		public delegate void AnimateDoneCallback();

		private Rigidbody2D _rigid;

		private Vector3 _startPos;
		private Vector3 _startAngleVector;

		private float _flyingLength;

		private float _flyingTime;

		private float _speed;

		private Vector2 _pausedVector;
		private float _pausedAngleVector;
		private AnimateDoneCallback curGameOverAnimateCallback;
		private AnimateDoneCallback curGameCompleteAnimateCallback;
		private SpriteRenderer render;
		private Sprite startSprite;

		public SpriteAnimator gameoverAnimate;
		public SpriteAnimator gameCompleteAnimate;
		public SpriteAnimator gameCongrasAnimate;

		public bool running {
			get;
			private set;
		}

		public float flyingTime {
			get{ 
				return _flyingTime;
			}
		}

		public float speed {
			get { 
				return _speed;
			}
		}

		public float flyingLength{
			get{ 
				return _flyingLength;
			}
			set{ 
				Vector3 pos = transform.position;
				pos.y = _startPos.y - value / Constants.egg_fly_length_unit_time;
				transform.position = pos;
				_flyingLength = value;
			}
		}

		protected override void DoInit ()
		{
			base.DoInit ();

			_rigid = GetComponent<Rigidbody2D> ();
			_startPos = transform.position;
			_startAngleVector = transform.eulerAngles;

			gameoverAnimate.playOnStart = false;
			gameoverAnimate.loopCount = 1;
			gameoverAnimate.OnAnimateEvent += OnGameOverAnimateEvent; 

			gameCompleteAnimate.playOnStart = false;
			gameCompleteAnimate.loopCount = 1;
			gameCompleteAnimate.OnAnimateEvent += OnGameOverAnimateEvent; 

			gameCongrasAnimate.playOnStart = false;
			gameCongrasAnimate.loopCount = 0;
			gameCongrasAnimate.OnAnimateEvent += OnGameOverAnimateEvent;

			render = GetComponent<SpriteRenderer> ();
			startSprite = render.sprite;
		}

		void OnGameOverAnimateEvent (string eventName, SpriteAnimator animator)
		{
			if (SpriteAnimator.EVENT_LOOPEND.Equals(eventName)){
				if (animator == gameoverAnimate){
					if (null != curGameOverAnimateCallback){
						curGameOverAnimateCallback ();		
					}
				}else if (animator == gameCompleteAnimate){
					if (null != curGameCompleteAnimateCallback){
						curGameCompleteAnimateCallback ();	
						gameCompleteAnimate.StopPlay ();
					}
				}

			}
		}

		protected override void Reset ()
		{
			base.Reset ();

			running = false;
			render.sprite = startSprite;
			transform.position = _startPos;
			transform.eulerAngles = _startAngleVector;
			_rigid.isKinematic = true;
			_rigid.drag = 1;
			_flyingLength = 0;
			_flyingTime = 0f;
			_speed = 0f;
			_pausedVector = Vector2.zero;
			_pausedAngleVector = 0f;
			curGameOverAnimateCallback = null;
			gameoverAnimate.StopPlay ();
		}

		void Update(){
			if (!pausing){
				_flyingLength = Mathf.Abs(transform.position.y - _startPos.y) * Constants.egg_fly_length_unit_time;
				_flyingTime += Time.deltaTime; 
				_speed = Mathf.Max(Mathf.Abs(_rigid.velocity.y),Mathf.Abs(_rigid.velocity.x)) * Constants.egg_fly_speed_unit_time;
//				Log.Debug ("------speed:{0}",_rigid.velocity);
			}
		}

		public void StartRun (Vector3 startPos = default(Vector3))
		{
			InitSelf ();
			Reset ();
			if (Vector3.zero != startPos) {
				Log.Info ("start run at position {0}",startPos);
				Vector2DUtils.ChangePositionY (transform,startPos.y);
			} else {
				transform.position = _startPos;
			}
//			gameObject.SetActive (true);
			_rigid.isKinematic = false;
			running = true;
		}

		public void StopRun(){
			InitSelf ();
			Reset ();
//			gameObject.SetActive (false);
		}

		public void ShowGameOverAnimate (AnimateDoneCallback animationDoneCallBack)
		{
			//stop
			transform.eulerAngles = _startAngleVector;
			_rigid.velocity = Vector2.zero;
			_rigid.angularVelocity = 0f;
			_rigid.isKinematic = true;
			curGameOverAnimateCallback = animationDoneCallBack;
			gameoverAnimate.StartPlay ();
		}

		public void ShowGameComplateAnimate (AnimateDoneCallback animationDoneCallBack)
		{
			//stop
			transform.eulerAngles = _startAngleVector;
			_rigid.velocity = Vector2.zero;
			_rigid.angularVelocity = 0f;
			_rigid.isKinematic = true;
			curGameCompleteAnimateCallback = animationDoneCallBack;
			gameCompleteAnimate.StartPlay ();
		}

		public void ShowGameCongrasAnimate ()
		{
			gameCongrasAnimate.StartPlay ();
		}

		public void EndRun(){
			_rigid.isKinematic = true;
			gameObject.SetActive (false);
		}

		public override void OnPause ()
		{
			base.OnPause ();
//			if (able2Pool){
//				Log.Debug ("ggggg");
//				return;
//			}
			_pausedVector = _rigid.velocity;
			_pausedAngleVector = _rigid.angularVelocity;
			_rigid.velocity = Vector2.zero;
			_rigid.angularVelocity = 0f;
//			Log.Debug ("PAUSe");
			_rigid.isKinematic = true;
			gameCompleteAnimate.OnPause ();
			gameoverAnimate.OnPause ();
			gameCongrasAnimate.OnPause ();
		}

		public override void OnResume ()
		{
			base.OnResume ();
//			if (able2Pool){
//				return;
//			}
			_rigid.velocity = _pausedVector;
			_rigid.angularVelocity = _pausedAngleVector;
//			Log.Debug ("Resume");
			_rigid.isKinematic = false;
			gameCompleteAnimate.OnResume ();
			gameoverAnimate.OnResume ();
			gameCongrasAnimate.OnResume ();
		}

		void OnCollisionEnter2D (Collision2D other)
		{
			if (null != other.collider){
				SideBoder sb = other.collider.gameObject.GetComponent<SideBoder> ();
				if (null != sb && sb.gameObject.name.Equals("BottomBorder")){
					GameController.singleton.levelsController.GameComplete ();
					return;
				}
				if (null == other.collider.gameObject.GetComponent<Line>()){
					//Log.Debug ("Line OnCollisionEnter2D,{0} {1}",other.gameObject,speed);
					if (speed >= Constants.crack_speed){
						GameController.GetSingleton ().levelsController.levelController.GameOver (transform,GameOverReason.collideWithSpeed,"collide with speed " + speed);
						return;
					}
				}
			}
//			Log.Debug ("Line OnCollisionEnter2D,{0}",other.gameObject);
		}

		void OnCollisionExit2D (Collision2D other)
		{
		}

		void OnTriggerEnter2D (Collider2D other)
		{
			if (null != other){
				
			}
		}

		void OnTriggerExit2D (Collider2D other)
		{
			//            Debug.LogFormat ("Line OnTriggerExit2D,{0}",other);
		}

	}
}
