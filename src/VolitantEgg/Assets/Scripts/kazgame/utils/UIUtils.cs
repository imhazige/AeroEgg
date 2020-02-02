using System;
using UnityEngine;
using UnityEngine.UI;

namespace kazgame.utils
{
	public static class UIUtils
	{
		public static void SetTextAlpha(Text text,float alpha){
			Color c = text.color;
			c.a = alpha;
			text.color = c;
		}
	}
}

