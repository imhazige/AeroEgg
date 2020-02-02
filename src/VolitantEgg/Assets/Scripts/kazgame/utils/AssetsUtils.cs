#if (UNITY_EDITOR)
using UnityEngine;
using System.Collections;
using UnityEditor;

namespace kazgame.utils
{
	public static class AssetsUtils
	{
		/// <summary>
		/// save the game object to its prefabs, this require the game object is already
		/// a prefabs
		/// </summary>
		/// <param name="gameObject">Game object.</param>
		public static void _ApplyPrefabs (GameObject gameObject)
		{
			GameObject instanceRoot = gameObject;
			Object targetPrefab = UnityEditor.PrefabUtility.GetPrefabParent (instanceRoot);
			if (null == targetPrefab) {
				//this is a preabs or not a pref at all
				Log.Warn ("ApplyPrefabs to {0} have no effect. make sure it is already a prefabs at first.", gameObject.name);
				return;
			}
			PrefabUtility.ReplacePrefab (instanceRoot, targetPrefab, ReplacePrefabOptions.ConnectToPrefab);
		}

		public static GameObject SavePrefabsResourceRelativePathWithoutExtention (string path, GameObject gameObject)
		{
			const string RESOURCE_ROOT = "Assets/Resources/";
			Object prefab = PrefabUtility.CreateEmptyPrefab (RESOURCE_ROOT + path + ".prefab");
			return PrefabUtility.ReplacePrefab (gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
		}

		public static GameObject SaveAsUnlinkPrefabsResourceRelativePathWithoutExtention(GameObject tmpCopyParent,string path, GameObject go){
			bool needDeleteParent = false;
			if (null == tmpCopyParent){
				tmpCopyParent = new GameObject ();
				needDeleteParent = true;
			}
			GameObject copy = Object.Instantiate (go);
			copy.transform.SetParent (tmpCopyParent.transform);
			GameObject saved = SavePrefabsResourceRelativePathWithoutExtention (path,copy);
			//this only work in editor
			Object.DestroyImmediate (copy);
			if (needDeleteParent){
				Object.DestroyImmediate (tmpCopyParent);
			}

			return saved;
		}

		public static string GetPrefabsPath (GameObject gameObject)
		{
			Object parentObject = PrefabUtility.GetPrefabParent (gameObject);
			string path = AssetDatabase.GetAssetPath (parentObject);

			return path;
		}

		public static string GetPrefabsResourceRelativePathWithoutExtention (GameObject gameObject)
		{
			string path = GetPrefabsPath (gameObject);

			const string RESOURCE_ROOT = "Assets/Resources/";

			int index = path.IndexOf (RESOURCE_ROOT);

			if (0 != index) {
				return null;
			}

			path = path.Substring (RESOURCE_ROOT.Length);

			index = path.LastIndexOf (".");

			if (-1 == index) {
				return path;
			}

			path = path.Substring (0, index);

			return path;
		}

	}
}
#endif