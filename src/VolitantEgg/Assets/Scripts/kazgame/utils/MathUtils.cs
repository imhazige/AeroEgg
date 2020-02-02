using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace kazgame.utils
{
	public static class MathUtils
	{
		public static float SqrtWithSymbol(float f){
			return (f > 0 ? 1 : -1) * Mathf.Sqrt (Mathf.Abs(f));
		}

		public static void RandomizeList(IList arr)
		{
			for (int i = arr.Count - 1; i > 0; i--) {
				int r = UnityEngine.Random.Range(0,i);
				object tmp = arr[i];
				arr[i] = arr[r];
				arr[r] = tmp;
			}
		}
	}
}

