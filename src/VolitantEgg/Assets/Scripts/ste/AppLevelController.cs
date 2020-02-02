using System;
using UnityEngine;
using kazgame.level;
using kazgame.utils;

namespace kazgame.ste
{
	public class AppLevelController : DanymicLoadLevelController
	{
		public void GameOver (Transform source,GameOverReason reason,string detail)
		{
			//game over
			Log.Info("Game Over:{0} - {1}",reason,detail);
			GameController.singleton.levelsController.GameOver (source,reason,detail);
		}
		
	}
}

