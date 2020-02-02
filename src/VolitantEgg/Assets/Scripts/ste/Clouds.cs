using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using kazgame.utils;

namespace kazgame.ste
{
	public class Clouds : MonoBase, PauseAble
	{
		private Cloud[] clouds;

		private bool pausing;

		private Coroutine _cor;

		protected override void DoInit ()
		{
			base.DoInit ();
			clouds = GetComponentsInChildren<Cloud> ();
			foreach (Cloud item in clouds) {
				item.gameObject.SetActive (false);
			}
		}

		public void StartShow(){
			InitSelf ();
			pausing = false;
			_cor = StartCoroutine (DoUpdate());
		}

		public void StopShow(){
			InitSelf ();
			if (null != _cor){
				StopCoroutine (_cor);
			}
			foreach (Cloud item in clouds) {
				item.StopShow ();
			}
		}

		IEnumerator DoUpdate ()
		{
			Vector2[] scope = Vector2DUtils.GetScreenScope ();
			float maxHeight = scope [1].y - scope [0].y;
			while (true) {
				if (!pausing){
					int index = UnityEngine.Random.Range (0,clouds.Length);
					Cloud c = clouds [index];
					if (!c.running){
						float height = UnityEngine.Random.Range (0,maxHeight);
						float speed = UnityEngine.Random.Range (0.05f,Constants.speed_cloud_move_max);
						if (UnityEngine.Random.Range (0,2) == 0){
							speed *= -1;
						}
						float scale = UnityEngine.Random.Range (0.2f,1.5f);
						c.StartMove (height,speed,scale);
					}
				}
				yield return new WaitForSeconds (5f);
			}
		}

		#region PauseAble implementation

		public void OnPause ()
		{
			pausing = true;
			foreach (Cloud item in clouds) {
				item.OnPause ();
			}
		}

		public void OnResume ()
		{
			pausing = false;
			foreach (Cloud item in clouds) {
				item.OnResume ();
			}
		}

		#endregion
	}
}

