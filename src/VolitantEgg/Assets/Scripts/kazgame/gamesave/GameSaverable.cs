using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace kazgame.gamesave
{
	public interface GameSaverable
	{
		Hashtable SaveGame();
		void LoadGame (Hashtable data);
	}

}
