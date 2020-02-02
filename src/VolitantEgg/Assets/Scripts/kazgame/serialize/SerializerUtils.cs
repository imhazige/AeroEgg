using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


namespace kazgame.serialize
{
	public static class SerializerUtils
	{
		public static void SaveAsBinary<T> (T obj, string path)
		{
			using(FileStream file = File.Create (path)){
				BinaryFormatter bf = new BinaryFormatter ();

				bf.Serialize (file, obj);
			}
		}

		public static T LoadAsBinary<T> (string path)
		{
			if (!File.Exists (path)) {
				return default(T);
			}

			using(FileStream file = File.Open (path, FileMode.Open)){
				BinaryFormatter bf = new BinaryFormatter ();

				T t = (T)bf.Deserialize (file);

				return t;
			}

		}

	}
}
