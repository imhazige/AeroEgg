using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using kazgame.gamesave;
using kazgame.ui;
using kazgame.utils;
using kazgame.objectpool;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace kazgame.ste
{
	public class LineController : MonoBase,GameSaverable,PauseAble
	{
		public float maxTotalLineLength = 10f;
		public Transform lineCopiesParent;

		public LayerMask blockMask = 0;
		public Line linePrefab;

		private bool isMousePressed;
		private List<Line> lines;
		private Line curLine = null;

		private Vector3 lastTouchPoint;

		private bool curLineHaveDrawn;
		private TouchDetector _touchDetector;

		private ObjectPoolController _objectPoolController;

		private bool _pausing;

		public enum LineUpdateEventType
		{
			LineRemoved,
			LineAdded,
			LineUpdate

		}

		public event Action<LineController,LineUpdateEventType,Line> OnLineUpdateEvent;

		protected override void DoInit ()
		{
			base.DoInit ();

			isMousePressed = false;
			lines = new List<Line> ();
			OnLineUpdateEvent += MyOnLineUpdateEvent;
//			linePrefab = Resources.Load ("Prefabs/Line", typeof(Line)) as Line;
			_touchDetector = new TouchDetector (null);
			_touchDetector.OnTouchEvent += OnTouchEventCall;
			_objectPoolController = new ObjectPoolController (lineCopiesParent);

			_objectPoolController.InitPool<Line> ("line",linePrefab,15);
		}

		void Awake ()
		{
			InitSelf ();
		}

		public void Reset ()
		{
			InitSelf ();
			isMousePressed = false;
//			foreach (Line item in lines) {
//				item.ForceDestroyLine ();
//			}
			while(lines.Count>0){
				Line l = lines[0];
				l.ForceDestroyLine ();
			}
			_touchDetector.enable = false;
			_pausing = false;
		}

		public void StartShow(){
			Reset ();
			_touchDetector.enable = true;
		}

		#region PauseAble implementation

		public void OnPause ()
		{
			_pausing = true;
			_touchDetector.enable = false;
			foreach (Line l in lines) {
				l.OnPause ();
			}
		}

		public void OnResume ()
		{
			_pausing = false;
			_touchDetector.enable = true;
			foreach (Line l in lines) {
				l.OnResume();
			}
		}

		#endregion

		void MyOnLineUpdateEvent (LineController manager, LineController.LineUpdateEventType evenType, Line line)
		{
//			Log.Debug ("update line {0}",evenType);
			GameController.GetSingleton ().levelsController.levelsMenuController.uiLineInk
			.UpdateInk (manager.getLeftLineRate ());
		}

		private Line GetLine(){
			Line line = _objectPoolController.InitiateFromPool <Line>("line");

			return line;
		}

		void OnTouchEventCall (int state, Vector3 position)
		{
			//FIXME this may make line problem, need force end line
			if (_pausing){
				return;
			}

			if (getLineTotalLength () > maxTotalLineLength) {
				if (null != curLine) {
					curLine.finishLine ();
					curLine = null;
					curLineHaveDrawn = true;
				}
			}


			bool isLineStart = 0 == state;
			bool isLineEnd = 2 == state;
//			Log.Debug ("touc state {0} : {1} : {2}",state,isLineStart,isLineEnd);
			Vector3 point = position;

			if (isLineStart) {
				curLineHaveDrawn = false;
				isMousePressed = true;
				if (null == isPointInCollider (point, blockMask)) {
					curLine = GetLine();
					AddLine (curLine);
				}
			} else if (isLineEnd) {
				isMousePressed = false;
				if (curLine) {
					curLine.finishLine ();	
				}
				curLine = null;
			}

			// Drawing line when mouse is moving(presses)
			if (isMousePressed) {
				if (null != curLine) {
					List<Vector3> ps = curLine.GetLinePoints ();
					bool needFinish = false;
					if (null != ps && 0 < ps.Count) {
						Vector2 start = (Vector2)ps [ps.Count - 1];
						Vector2 endPos = point;
						RaycastHit2D hit = Physics2D.Linecast (start, endPos, blockMask);

						if (null != hit.collider) {
							needFinish = true;
							//						Debug.LogFormat ("hited {0}",hit.collider);
							//add the point at the border
							Vector3 mousePos = hit.point;
							mousePos.z = 0f;
							curLine.updatePoint (mousePos);
							if (null != OnLineUpdateEvent) {
								OnLineUpdateEvent (this, LineUpdateEventType.LineUpdate, curLine);
							}
						}
					} else {
						Collider2D c2d = isPointInCollider (point, blockMask);

						if (null != c2d) {
							needFinish = true;
						}
					}
					if (needFinish) {
						curLine.finishLine ();
						curLineHaveDrawn = true;
						curLine = null;
					} else {
						Vector3 mousePos = point;
						mousePos.z = 0f;
						curLine.updatePoint (mousePos);
						if (null != OnLineUpdateEvent) {
							OnLineUpdateEvent (this, LineUpdateEventType.LineUpdate, curLine);
						}
					}
				} else {
					if (!isLineStart) {
						if (null == isPointInCollider (point, blockMask) && !curLineHaveDrawn) {
							curLine = GetLine();
							AddLine (curLine);

							Vector2 endPos = (Vector2)lastTouchPoint;
							Vector2 start = point;
							RaycastHit2D hit = Physics2D.Linecast (start, endPos, blockMask);

							#if UNITY_EDITOR

//							Debug.DrawLine (new Vector2 (start.x, start.y - 0.5f), new Vector2 (start.x, start.y + 0.5f), Color.yellow);
//							Debug.DrawLine (new Vector2 (endPos.x, endPos.y - 0.5f), new Vector2 (endPos.x, endPos.y + 0.5f), Color.green);
							#endif

							//this should always be true
							if (null != hit.collider) {
								//						Debug.LogFormat ("hited {0}",hit.collider);
								//add the point at the border
								Vector3 mousePos = hit.point;
								mousePos.z = 0f;
								#if UNITY_EDITOR

								Debug.DrawLine (new Vector2 (mousePos.x, mousePos.y - 0.2f), new Vector2 (mousePos.x, mousePos.y + 0.2f), Color.blue);
								//								EditorApplication.isPaused = true;
								#endif
								curLine.updatePoint (mousePos);
								if (null != OnLineUpdateEvent) {
									OnLineUpdateEvent (this, LineUpdateEventType.LineUpdate, curLine);
								}

								mousePos = point;
								mousePos.z = 0f;
								curLine.updatePoint (mousePos);
								if (null != OnLineUpdateEvent) {
									OnLineUpdateEvent (this, LineUpdateEventType.LineUpdate, curLine);
								}
							} else {
								Debug.LogErrorFormat ("wrong logic {0}", lastTouchPoint);
								Debug.DrawLine (start, endPos, Color.yellow);
								#if UNITY_EDITOR
								EditorApplication.isPaused = true;
								#endif

							}
						} 
					}
				}

				lastTouchPoint = point;

			}
		}

		#region GameSaverable implementation

		Hashtable GameSaverable.SaveGame ()
		{
			Hashtable hs = new Hashtable ();

			//TODO lines, curline need not save
			List<Hashtable> linesdata = new List<Hashtable> ();
			foreach (GameSaverable l in lines) {
				Hashtable td = l.SaveGame ();

				linesdata.Add (td);
			}

			hs.Add ("lines", linesdata);

			return hs;
		}

		void GameSaverable.LoadGame (Hashtable data)
		{
			//TODO lines, treat as have not curline
			if (null == data) {
				return;
			}
			List<Hashtable> linesdata = (List<Hashtable>)data ["lines"];

			if (null != linesdata && 0 < linesdata.Count) {
				for (int i = 0; i < linesdata.Count; i++) {
					Line line = GetLine();
//					Line line = Instantiate (linePrefab) as Line;
					((GameSaverable)line).LoadGame (linesdata [i]);
					AddLine (line);
				}
			}
		}

		#endregion

		void Update ()
		{
			_touchDetector.Update ();
		}

		Collider2D isPointInCollider (Vector2 realPoint, LayerMask mask)
		{
			return Physics2D.OverlapPoint (realPoint, blockMask);
		}

		void onLineLifeEnd (Line line)
		{
			if (!lines.Contains (line)) {
				return;
			}
			if (curLine == line) {
				//curent line have been force end(by destroyer)
				curLineHaveDrawn = true;
			}
			RemoveLine (line);
			if (null != OnLineUpdateEvent) {
				OnLineUpdateEvent (this, LineUpdateEventType.LineRemoved, line);
			}
		}


		float getLineTotalLength ()
		{
			if (null == lines || 0 == lines.Count) {
				return 0f;
			}
			float lineLength = 0f;
			for (int i = 0; i < lines.Count; i++) {
				Line line = lines [i];
				lineLength += line.getLineLength ();
			}

			return lineLength;
		}


		public float getLeftLineRate ()
		{
			return 1 - getLineTotalLength () / maxTotalLineLength;
		}


		void RemoveLine (Line line)
		{
			line.onLineLifeEndEvent -= onLineLifeEnd;
			line.OnLineCollisionEnter2D -= Line_OnLineCollisionEnter2D;
			line.OnLineTriggerEnter2D -= Line_OnLineTriggerEnter2D;
			lines.Remove (line);
		}

		void Line_OnLineCollisionEnter2D (Line line, Collision2D col)
		{
//			Log.Debug ("Line collision enter {0}", col);
		}

		void AddLine (Line line)
		{
			line.onLineLifeEndEvent += onLineLifeEnd;
			line.OnLineCollisionEnter2D += Line_OnLineCollisionEnter2D;
			line.OnLineTriggerEnter2D += Line_OnLineTriggerEnter2D;
			line.transform.parent = lineCopiesParent;
			lines.Add (line);
			if (null != OnLineUpdateEvent) {
				OnLineUpdateEvent (this, LineUpdateEventType.LineAdded, line);
			}
		}

		void Line_OnLineTriggerEnter2D (Line line, Collider2D col)
		{
		}

	}
}
