using System;
using UnityEngine;

namespace kazgame.utils
{
	public static class DateTimeUtils
	{
		public static string formatSecondsWithinHour (float seconds, string seperator = "'")
		{
			System.TimeSpan t = System.TimeSpan.FromSeconds (seconds);
			return string.Format ("{0}{2}{1:D2}", t.Minutes, t.Seconds, seperator); 
		}

		public static string toHMSF(DateTime dt){
			return dt.ToString ("HH:mm:ss:fff");
		}
	}

}