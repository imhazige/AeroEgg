using System;
using UnityEngine;
using UnityEngine.UI;
using kazgame.utils;

namespace kazgame.ste
{
	public class DebugUI : MonoBase
	{
		public Button btnUp;
		public Button btnDown;
		public Button btnLeft;
		public Button btnRight;
		public Button btnGo;
		public InputField inputHeight;
		public Toggle togglePause;
		public Toggle toggleNoFail;
		public Toggle toggleShow;

		protected override void DoInit ()
		{
			base.DoInit ();
			btnGo.onClick.AddListener (EggGo);
			btnDown.onClick.AddListener (EggDown);
			btnLeft.onClick.AddListener (EggLeft);
			btnRight.onClick.AddListener (EggRight);
			btnUp.onClick.AddListener (EggUp);
			togglePause.onValueChanged.AddListener (ToggleClick);
			toggleNoFail.onValueChanged.AddListener (ToggleNoFailClick);
			toggleShow.onValueChanged.AddListener (ToggleShowClick);
			gameObject.SetActive (Constants.debug_show_debug_tool);
		}

		void Awake(){
			InitSelf ();
		}

		void EggUp(){
			float len = GameController.GetSingleton ().levelsController.eggController.flyingLength;
			len -= 1;
			len = Math.Max (0,len);
			GameController.GetSingleton ().levelsController.eggController.flyingLength = len;
		}

		void EggDown(){
			float len = GameController.GetSingleton ().levelsController.eggController.flyingLength;
			len += 1;
			len = Math.Max (0,len);
			GameController.GetSingleton ().levelsController.eggController.flyingLength = len;
		}

		void EggLeft(){
			Transform eggTrans = GameController.GetSingleton ().levelsController.eggController.transform;
			Vector2DUtils.ChangePositionX (eggTrans,eggTrans.position.x - 1);
		}

		void EggRight(){
			Transform eggTrans = GameController.GetSingleton ().levelsController.eggController.transform;
			Vector2DUtils.ChangePositionX (eggTrans,eggTrans.position.x + 1);
		}

		void EggGo(){
			GameController.GetSingleton ().levelsController.eggController.flyingLength = float.Parse(inputHeight.text);
		}

		void ToggleClick(bool isOn){
			if (isOn) {
				GameController.singleton.levelsController.levelController.OnPause ();
				GameController.singleton.levelsController.eggController.OnPause ();
			} else {
				GameController.singleton.levelsController.levelController.OnResume ();
				GameController.singleton.levelsController.eggController.OnResume ();
			}
		}

		void ToggleNoFailClick(bool isOn){
			GameController.singleton.levelsController.debugNoFail = Constants.debug_model && isOn;
		}

		void ToggleShowClick(bool isOn){
			toggleNoFail.transform.parent.gameObject.SetActive (isOn);
		}
	}
}

