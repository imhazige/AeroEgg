using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace kazgame.ui
{
	/// <summary>
	/// how to use:
	/// the content text should be as a child of the ui object which have a rect mask 2d component,
	/// the text will show in the area of the size of the component of the rect mask 2d.
	/// </summary>
	public class MaskTextScrollEffect : MonoBase
	{
		public delegate void OnClose();

		public Text text;
		private Coroutine _cor;
		private float _textOriginY;
		private RectMask2D maskRect;

		void Awake(){
			InitSelf ();
		}

		protected override void DoInit ()
		{
			base.DoInit ();

			if (null == text){
				text = GetComponentInChildren<Text> ();
			}
			maskRect = GetComponentInChildren<RectMask2D> ();
			_textOriginY = text.rectTransform.position.y;
		}

		public void ShowText(string c){
			InitSelf ();
			Reset ();
			text.text = c;
			//adjust height, otherwise, when text rect not in the mask rect, it will not show in the mask
			Vector2 v2 = text.rectTransform.sizeDelta;
			v2.y = text.preferredHeight;
			text.rectTransform.sizeDelta = v2; 

			Vector2 pos = text.rectTransform.anchoredPosition;
			pos.y = 0 - maskRect.rectTransform.sizeDelta.y / 2 - text.preferredHeight/2;
			text.rectTransform.anchoredPosition = pos;

			gameObject.SetActive (true);
			//if you encounter error, ser the parent object MainMenu active
			_cor = StartCoroutine (SmoothMovement());
		}

		private void Reset(){
			Vector3 v = text.rectTransform.position;
			v.y = _textOriginY;
			text.rectTransform.position = v;
			if (null != _cor){
				StopCoroutine (_cor);
			}
		}

		protected IEnumerator SmoothMovement(){
			Vector2 target =  text.rectTransform.anchoredPosition;

			float h = text.preferredHeight;

			float y1 = 0 - maskRect.rectTransform.sizeDelta.y / 2;
			target.y = y1 + h /2 ;
			//			Log.Debug (">>>>>>>{0},{1},{2},{3}" , maskRect.rectTransform.position.y,maskRect.rectTransform.sizeDelta.y,h,target.y);
			while(text.rectTransform.anchoredPosition != target){
				text.rectTransform.anchoredPosition = Vector3.MoveTowards (text.rectTransform.anchoredPosition, target, 20f * Time.deltaTime);

				yield return null;
			}
		}
	}
}

