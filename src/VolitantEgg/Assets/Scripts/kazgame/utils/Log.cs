using UnityEngine;
using System;
using System.Collections;

namespace kazgame.utils
{

	public static class Log
	{

		private static string FormatT(string format){
			return DateTimeUtils.toHMSF (System.DateTime.Now) + " " + format;
		}

		public static void Debug (string format, params object[] objs)
		{
			if (null == objs || 0 == objs.Length) {
				UnityEngine.Debug.Log (format);
			} else {
				UnityEngine.Debug.LogFormat (format, objs);
			}
		}

		public static void DebugT (string format, params object[] objs)
		{
			Debug(FormatT(format),objs);
		}

		public static void Info (string format, params object[] objs)
		{
			if (null == objs || 0 == objs.Length) {
				UnityEngine.Debug.Log (format);
			} else {
				UnityEngine.Debug.LogFormat (format, objs);
			}
		}

		public static void InfoT (string format, params object[] objs)
		{
			Info(FormatT(format),objs);
		}

		public static void Warn (string format, params object[] objs)
		{
			Warn (null, format, objs);
		}

		public static void WarnT (string format, params object[] objs)
		{
			Warn (FormatT(format),objs);
		}

		public static void Warn (Exception ex, string format, params object[] objs)
		{
			if (null == objs || 0 == objs.Length) {
				UnityEngine.Debug.LogWarning (format);
			} else {
				UnityEngine.Debug.LogWarningFormat (format, objs);
			}
			if (null != ex) {
				UnityEngine.Debug.LogException (ex);
			}
		}

		public static void WarnT (Exception ex, string format, params object[] objs)
		{
			Warn (ex,FormatT(format),objs);
		}

		public static void Error (string format, params object[] objs)
		{
			Error (null, format, objs);
		}

		public static void ErrorT (string format, params object[] objs)
		{
			Error (FormatT(format),objs);
		}

		public static void Error (Exception ex, string format, params object[] objs)
		{
			if (null == objs || 0 == objs.Length) {
				UnityEngine.Debug.LogError (format);
			} else {
				UnityEngine.Debug.LogErrorFormat (format, objs);
			}
			if (null != ex) {
				UnityEngine.Debug.LogException (ex);
			}
		}

		public static void ErrorT (Exception ex, string format, params object[] objs)
		{
			Error (ex,FormatT(format),objs);
		}
	}
}