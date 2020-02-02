using UnityEngine;
using System.Collections;
using UnityEngine.UI;


namespace kazgame.ste
{
	public delegate void AfterShown ();

	public class MenuAnimator : MonoBase
	{
		private const float fallDownSpeed = 50;

		private Coroutine _cor;
		private Vector3 _origin;
		private AfterShown fnAfterShown;

		protected override void DoInit ()
		{
			base.DoInit ();
			_origin = transform.position;
		}

		void Awake(){
			InitSelf ();
		}

		public void StartShow(AfterShown fn = null){
			gameObject.SetActive (true);
			UseCodeWay();
			fnAfterShown = fn;
		}

		private IEnumerator DoFallDown ()
		{
			while(!Mathf.Approximately (transform.position.y,_origin.y )){
				transform.position = Vector3.MoveTowards (transform.position,_origin,fallDownSpeed*Time.deltaTime);

				yield return null;
			}

			if (null != fnAfterShown){
				fnAfterShown ();
			}
		}

		private void UseCodeWay(){
			transform.position = _origin + new Vector3(0,10,0);
			_cor = StartCoroutine(DoFallDown());
		}

		public void StartHide(){
			if (null != _cor){
				StopCoroutine (_cor);
			}
			gameObject.SetActive (false);
		}

		public void EnabledMenu (bool b)
		{
			Button[] mbs = GetComponentsInChildren<Button> ();

			if (null != mbs){
				foreach (Button item in mbs) {
					item.enabled = b;
				}
			}
		}

	}
}
