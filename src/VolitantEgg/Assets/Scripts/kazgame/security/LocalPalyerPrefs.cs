using System;
using UnityEngine;

namespace kazgame.security
{
	public class LocalPalyerPrefs
	{
		private readonly string secretKey;
		
		public LocalPalyerPrefs(string key){
			secretKey = key;
		}

		public string GetString(string name,string defaultValue = null){
			string v = PlayerPrefs.GetString (name,defaultValue);

			if (null == v){
				return v;
			}

			return StringEncryption.Decrypt (v,secretKey);
		}

		public void SetString(string name,string value){
			if (null == value) {
				return;
			}

			string v = StringEncryption.Encrypt(value,secretKey);

			PlayerPrefs.SetString (name,v);
		}

		public float GetFloat(string name,float defaultValue = default(float)){
			string v = GetString (name, null);
			if (null == v){
				return defaultValue;
			}
			float fv = defaultValue;
			float.TryParse (v, out fv);
			return fv;
		}

		public void SetFloat(string name,float value){
			SetString (name,value + "");
		}

		public int GetInt(string name,int defaultValue = default(int)){
			string v = GetString (name, null);
			if (null == v){
				return defaultValue;
			}
			int fv = defaultValue;
			int.TryParse (v, out fv);
			return fv;
		}

		public void SetInt(string name,int value){
			SetString (name,value + "");
		}
	}
}

