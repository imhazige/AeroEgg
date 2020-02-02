using UnityEngine;
using System.Collections;
using kazgame.utils;

namespace kazgame
{
	[RequireComponent(typeof(BoxCollider2D))]
	public class BoxScope : MonoBehaviour {
		private BoxCollider2D _box;
		private Vector2[] _scope;

		public Vector2[] scope{
			get{ 
				if (null == _scope){
					_box = GetComponent<BoxCollider2D> ();

					_scope = Vector2DUtils.GetBoxCollider2DScope (_box);

					gameObject.SetActive (false);
					_box.enabled = false;
				}

				return _scope;
			}
		}
	}
}
