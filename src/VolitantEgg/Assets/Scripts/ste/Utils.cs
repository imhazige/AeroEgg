using System;
using UnityEngine;

namespace kazgame.ste
{
	public static class Utils
	{
		public static Sprite LoadSprite(string spname){
			return Resources.Load<Sprite> ("Img/" + spname);
		}
	}
}

