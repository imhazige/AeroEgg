using UnityEngine;
using System.Collections;

namespace kazgame
{
	/// <summary>
	/// use edge collider to edit the points in editor when design
	/// </summary>
	[RequireComponent(typeof(EdgeCollider2D))]
	public class PointScope : MonoBehaviour,IPointScope {
		public bool autoHidden;

		private EdgeCollider2D _box;
		private Vector2[] _scope;

		public Vector2[] scope{
			get{ 
				#if !UNITY_EDITOR
				if (null == _scope){
				#endif	
					//in editor update the scope everytime as it is editing
					_box = GetComponent<EdgeCollider2D> ();

					_scope = new Vector2[_box.points.Length];

					for (int i = 0 ; i < _box.points.Length ; i++){
						Vector2 v = _box.offset + _box.points[i];

						v = transform.TransformPoint (v);
						_scope [i] = v;
					}

					if (autoHidden){
						gameObject.SetActive (false);	
					}
					_box.enabled = false;
				#if !UNITY_EDITOR
				}
				#endif

				return _scope;
			}
		}
	}
}
