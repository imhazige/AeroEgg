using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace kazgame.utils
{
	public static class GameObjectUtils
	{
		public static void CleanChildInEditorMode (GameObject go)
		{
			#if (UNITY_EDITOR)
			CleanChild(go);
			#endif
		}

		public static void CleanChild (GameObject go)
		{
			List<GameObject> children = new List<GameObject> ();
			foreach (Transform child in go.transform) {
				children.Add (child.gameObject);
			}
			#if (UNITY_EDITOR)
			children.ForEach (Object.DestroyImmediate);
			#else
			children.ForEach (Object.Destroy);
			#endif
		}

		public static void CleanChildType<T> (GameObject go) where T : Component
		{
			T [] ts = go.GetComponentsInChildren<T>();
			List<GameObject> children = new List<GameObject> ();
			foreach (T child in ts) {
				children.Add (child.gameObject);
			}
			#if (UNITY_EDITOR)
			children.ForEach (Object.DestroyImmediate);
			#else
			children.ForEach (Object.Destroy);
			#endif
		}

		public static string GetUniqNameByHierarchy(GameObject go){
			Transform parent = go.transform.parent;
			string name = go.name;
			while(null != parent){
				name = parent.name + "-" + name;
				parent = parent.parent;
			}

			return name;
		}

		/// <summary>
		/// Find the specified go, nameToFind and findInActive.
		/// </summary>
		/// <param name="go">Go.</param>
		/// <param name="nameToFind">Name to find.</param>
		/// <param name="findInActive">If set to <c>true</c> find in active.</param>
		[System.Obsolete("obscure funtion, the parameter findInActive is strange, use FindAnyOne instead")]
		public static GameObject Find (GameObject go, string nameToFind, bool findInActive = true)
		{
			if (null == go) {
				return null;
			}
			Transform transform = go.transform;
			int childCount = transform.childCount;
			for (int i = 0; i < childCount; ++i) {
				Transform child = transform.GetChild (i);
				if (child.gameObject.name == nameToFind && child.gameObject.activeSelf == findInActive)
					return child.gameObject;
				GameObject result = Find (child.gameObject, nameToFind, findInActive);
				if (result != null)
					return result;
			}
			return null;
		}

		public static GameObject FindAnyOne (this GameObject go, string nameToFind)
		{
			if (null == go) {
				return null;
			}
			Transform transform = go.transform;
			int childCount = transform.childCount;
			for (int i = 0; i < childCount; ++i) {
				Transform child = transform.GetChild (i);
				if (child.gameObject.name == nameToFind)
					return child.gameObject;
				GameObject result = FindAnyOne (child.gameObject, nameToFind);
				if (result != null)
					return result;
			}
			return null;
		}

		public static GameObject FindGameObjectWithTag (string tag, bool findInActive = true)
		{
			return FindGameObjectWithTag (GetRootObject (), tag, findInActive);
		}

		public static GameObject FindGameObjectWithTag (GameObject go, string tag, bool findInActive = true)
		{
			if (findInActive) {
				if (null == go) {
					return null;
				}
				Transform transform = go.transform;
				int childCount = transform.childCount;
				for (int i = 0; i < childCount; ++i) {
					Transform child = transform.GetChild (i);
					if (child.gameObject.tag == tag)
						return child.gameObject;
					GameObject result = FindGameObjectWithTag (child.gameObject, tag, findInActive);
					if (result != null)
						return result;
				}
				return null;
			} else {
				return GameObject.FindGameObjectWithTag (tag);
			}
		}

		public static Canvas GetCanvas ()
		{
			return GameObject.Find ("Canvas").GetComponent<Canvas> ();
		}

		[System.Obsolete()]
		public static GameObject FindUI (string nameToFind, bool findInActive = true)
		{
			return Find (GetCanvas ().gameObject, nameToFind, findInActive);
		}

		[System.Obsolete()]
		public static GameObject Find (string nameToFind, bool findInActive = true)
		{
			GameObject root = GetRootObject ();

			return Find (root, nameToFind, findInActive);
		}

		public static GameObject GetRootObject ()
		{
			GameObject root = GameObject.Find ("GameRoot");

			return root;
		}

		/// <summary>
		/// depth first search.
		/// </summary>
		/// <returns>The component in chidren.</returns>
		/// <param name="go">Go.</param>
		/// <param name="findInActive">If set to <c>true</c> search result will be not active.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T GetComponentInChidren<T> (GameObject go, bool findInActive = true) where T : Component
		{
			if (null == go) {
				return default(T);
			}
			T result = go.GetComponentInChildren<T> ();
			if (null != result) {
				return result;
			}
			if (findInActive) {
				Transform transform = go.transform;
				int childCount = transform.childCount;
				for (int i = 0; i < childCount; ++i) {
					Transform child = transform.GetChild (i);
					if (child.gameObject.activeInHierarchy) {
						continue;
					}
					result = child.gameObject.GetComponent<T> ();
					if (null != result) {
						return result;
					}
				}
				for (int i = 0; i < childCount; ++i) {
					Transform child = transform.GetChild (i);
					if (child.gameObject.activeInHierarchy) {
						continue;
					}

					result = GetComponentInChidren<T> (child.gameObject, true);
					if (null != result) {
						return result;
					}	

				}
				return default(T);
			} else {
				return result;
			}
		}


		public static T[] GetComponentsInChildren<T> (GameObject go, bool findInActive = true) where T : Component
		{
			if (null == go) {
				return default(T[]);
			}
			T[] result = go.GetComponentsInChildren<T> ();
			if (null != result) {
				return result;
			}
			if (findInActive) {
				Transform transform = go.transform;
				int childCount = transform.childCount;
				for (int i = 0; i < childCount; ++i) {
					Transform child = transform.GetChild (i);
					if (child.gameObject.activeInHierarchy) {
						continue;
					}
					result = child.gameObject.GetComponentsInChildren<T> ();
					if (null != result) {
						return result;
					}
				}
				for (int i = 0; i < childCount; ++i) {
					Transform child = transform.GetChild (i);
					if (child.gameObject.activeInHierarchy) {
						continue;
					}

					result = GetComponentsInChildren<T> (child.gameObject, true);
					if (null != result) {
						return result;
					}	

				}
				return default(T[]);
			} else {
				return result;
			}
		}

		public static T GetComponentInChidren<T> (bool findInActive = true) where T : Component
		{
			return GetComponentInChidren<T> (GetRootObject (), findInActive);
		}

		public static T GetUIComponentInChidren<T> (bool findInActive = true) where T : Component
		{
			return GetComponentInChidren<T> (GetCanvas ().gameObject, findInActive);
			;
		}

		public static T GetComponent<T> () where T : Component
		{
			return GetRootObject ().GetComponent<T> ();
		}

		[System.Obsolete()]
		public static T GetUIComponent<T> (string objectName) where T : Component
		{
			GameObject obj = FindUI (objectName);
			if (null == obj) {
				return default(T);
			}
			
			return obj.GetComponent<T> ();
		}

		[System.Obsolete()]
		public static T GetComponent<T> (string objectName) where T : Component
		{
			GameObject obj = Find (objectName);
			if (null == obj) {
				return default(T);
			}

			return obj.GetComponent<T> ();
		}

		public static bool IsNull (System.Object aObj)
		{
			return aObj == null || aObj.Equals (null);
		}
	}
}
