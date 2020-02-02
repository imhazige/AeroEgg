using System;
using UnityEngine;
using kazgame.utils;
using System.Collections.Generic;

#if (UNITY_EDITOR)
using UnityEditor;
#endif


namespace kazgame.ste
{
	public class EditorAssist : MonoBase
	{
		#if (UNITY_EDITOR)


		public GameController gameController;
		public GameObject levelDesignParents;
		public LoadingPage loadingPage;

		[ContextMenu ("Reset To Normal")]
		public void EdResetToNormal(){
			gameObject.SetActive (false);
			gameController.levelsController.StopShow ();
			gameController.mainMenuController.StopAndHide ();
			loadingPage.gameObject.SetActive (true);
			loadingPage.autoStart = true;
		}

		[ContextMenu ("Reset Prefs")]
		public void EdResetPrefs(){
			PlayerPrefs.SetInt (Constants.prefkeyLastScore, 0);
		}

		[ContextMenu ("Show Main Menu")]
		public void EdShowMainMenu(){
			gameController.mainMenuController.Show ();
			gameController.levelsController.StopShow ();
			loadingPage.gameObject.SetActive (false);
		}

		[ContextMenu ("Auto Design Branches")]
		public void EdAudoDesignBranches(){
			int blockLineLayer = LayerMask.NameToLayer("Can Not Draw Line");
			gameObject.SetActive (true);
			gameController.levelsController.StopShow ();
			gameController.mainMenuController.StopAndHide ();
			levelDesignParents.SetActive (true);
			loadingPage.autoStart = false;
			//add polygion collider if need
			SpriteRenderer[] branches = GameObjectUtils.GetComponentsInChildren<SpriteRenderer>(levelDesignParents);
			foreach (SpriteRenderer item in branches) {
				string iname = item.gameObject.name;
				Log.Debug ("branch {0}",item.gameObject.name);
				if (iname.StartsWith("left-branchs") || iname.StartsWith("right-branchs")){
					PolygonCollider2D pc = item.gameObject.GetComponent<PolygonCollider2D> ();
					Vector3 pos = item.transform.position;
					pos.z = (float)ZIndexs.branch;
					item.transform.position = pos;
					if (null == pc) {
						pc = item.gameObject.AddComponent<PolygonCollider2D> ();
					} else {
						DestroyImmediate (pc);
						pc = item.gameObject.AddComponent<PolygonCollider2D> ();
					}
//					item.gameObject.layer = blockLineLayer;
					item.gameObject.layer = 0;
				}
			}
		}

		[ContextMenu ("Auto Design Branches Leaf")]
		public void EdAudoDesignBranchesLeaf(){
			gameObject.SetActive (true);
			gameController.levelsController.StopShow ();
			gameController.mainMenuController.StopAndHide ();
			levelDesignParents.SetActive (true);
			loadingPage.autoStart = false;
			List<Sprite> spritesLeft = new List<Sprite>(); 
			for (int i = 1; i < 4; i++) {
				spritesLeft.Add (Resources.Load<Sprite>("Img/leaf-left-" + i) as Sprite);	
			}

			//add polygion collider if need
			SpriteRenderer[] branches = GameObjectUtils.GetComponentsInChildren<SpriteRenderer>(levelDesignParents);
			foreach (SpriteRenderer item in branches) {
				if (null == item){
					continue;
				}
				string iname = item.gameObject.name;
				Log.Debug ("branch {0}",item.gameObject.name);
				if (iname.StartsWith("leaf-left") || iname.StartsWith("leaf-right")){
					Vector3 lp = item.gameObject.transform.localPosition;
					lp.z = 0;
					item.gameObject.transform.localPosition = lp;
				}
			}
		}

		[ContextMenu ("Save Design")]
		public void EdShowLevelDesign(){
			gameObject.SetActive (true);
			gameController.levelsController.StopShow ();
			gameController.mainMenuController.StopAndHide ();
			gameController.levelsController.levelController.EdCollectEditorLevelData ();
			gameController.levelsController.levelController.EdSaveSegmentPrefabs ();
			levelDesignParents.SetActive (true);
			loadingPage.autoStart = false;
		}

		[ContextMenu ("Test")]
		public void EdUnitTest(){
			UnitTest.test ();
		}
		#endif
	}
}

