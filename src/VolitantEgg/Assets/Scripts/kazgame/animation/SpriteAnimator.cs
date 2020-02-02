using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using kazgame.utils;

namespace kazgame.animation
{
	public class SpriteAnimator : MonoBase, PauseAble
	{
		public const string EVENT_LOOPEND = "EVENT_LOOPEND";
		public const string EVENT_LOOP_INDEX = "EVENT_LOOP_INDEX";

		public List<Sprite> sprites;

		/// <summary>
		/// seconds,0.125 is 1sec/8frames
		/// </summary>
		public float interval = .125f;

		/// <summary>
		/// less than 1 mean loop forever
		/// </summary>
		public int loopCount = 0;

		public bool playOnStart = false;

		public SpriteRenderer spriteRender;

		/// <summary>
		/// the sprite index in the one animate loop
		/// </summary>
		/// <value>The index of the loop.</value>
		public int loopIndex {
			get;
			private set;	
		}

		public bool running {
			get;
			private set;	
		}

		private Coroutine _cor;

		private bool _pausing;

		private List<Sprite> _sprites;


		public delegate void OnAnimateEventDelegate (string eventName, SpriteAnimator animator);

		public OnAnimateEventDelegate OnAnimateEvent;

		protected void FireEvent (string eventName)
		{
			if (null == OnAnimateEvent) {
				return;
			}

			OnAnimateEvent (eventName, this);
		}

		protected override void DoInit ()
		{
			base.DoInit ();

			if (null == sprites[0]){
				throw new SystemException ("sprite 0 can not be null.");
			}
		}

		void Start(){
			if (playOnStart){
				StartPlay ();
			}
		}

		private void prepare(){
			Sprite last = null;
			_sprites = new List<Sprite> (sprites.Count);
			for (int i = 0; i < sprites.Count; i++) {
				Sprite item = sprites [i];
				if (null == item) {
					_sprites.Add(last);
				} else {
					last = item;
					_sprites.Add(item);
					//_sprites [i] = item;
				}
			}
			if (null == spriteRender){
				spriteRender = GetComponent<SpriteRenderer> ();
			}
		}

		void Reset(){
			_pausing = false;
			loopIndex = 0;
			running = false;
			if (null != _cor){
				StopCoroutine (_cor);
			}
		}

		public void StartPlay(){
			Reset ();
			running = true;
			prepare ();
			if (null==spriteRender){
				throw new SystemException ("sprite render is null.");
			}
			_cor = StartCoroutine (DoAnimate());
		}

		public void StopPlay(){
			Reset ();
		}

		IEnumerator DoAnimate ()
		{
//			Log.Debug ("started ,,,,");
			int index = 0;
			int loopRendered = 0;
			while(true){
				if (!_pausing){
					spriteRender.sprite = _sprites [index];
					loopIndex = index;
					FireEvent (EVENT_LOOP_INDEX);
					index++;
					index = index % _sprites.Count;	

					if (0 == index){
						loopRendered++;

						if (loopCount > 0 && loopRendered >= loopCount){
//							Log.Debug ("loop compltete.");
							FireEvent (EVENT_LOOPEND);
							yield break;
						}
					}

					//				Log.Debug ("index {0} - {1}",index,loopRendered);
				}

				yield return new WaitForSeconds (interval);
			}
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


		[ContextMenu ("Play")]
		public void EdPlayAnimate(){
			InitSelf ();
			StartPlay ();
		}
	}
}

