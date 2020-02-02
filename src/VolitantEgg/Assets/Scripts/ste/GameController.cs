using System;
using UnityEngine;
using UnityEngine.UI;
using kazgame.utils;
using kazgame.ui;

namespace kazgame.ste
{
	[ExecuteInEditMode]
	public class GameController : MonoBase
	{
		public LevelsController levelsController;
		public MainMenuController mainMenuController;
		public AudioController audioController;
		public StoreManager storeManager;

		public UIBasedLayerMask maskLayer;

		public static GameController GetSingleton(){
			return singleton;
		}

		public static GameController singleton {
			get ;
			private set;
		}

		void Awake(){
			InitSelf ();
		}

		protected override void DoInit ()
		{
			base.DoInit ();
			singleton = this;
			storeManager = new StoreManager ();
			maskLayer.Unmask ();
			Log.Info ("init gamecontroller");
		}
	}
}

