using System;
using System.Collections;
using System.Text;

namespace kazgame.utils
{
	public static class LogUtils
	{
		public static string logList(IList list){
			if (null == list){
				return null;
			}

			StringBuilder sb = new StringBuilder ();


			foreach (object item in list) {
				sb.Append (",");
				sb.Append (item);
			}
			sb.Remove (0,1);

			return "[" + sb.ToString() + "]";
		}

	}
}

