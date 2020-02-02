using System.Collections;
using System.Collections.Generic;
using kazgame.serialize;

namespace kazgame.gamesave
{
	public static class GameSaver {

		public static void Save(Dictionary<string,GameSaverable> savers,string path){
			if (null == savers || 0 == savers.Count){
				return;
			}
			Dictionary<string,Hashtable> datas = new Dictionary<string, Hashtable> ();
			foreach(KeyValuePair<string, GameSaverable> en in savers)
			{
				GameSaverable gs = en.Value;

				Hashtable data = gs.SaveGame ();

				datas.Add (en.Key,data);
			}

			SerializerUtils.SaveAsBinary<Dictionary<string,Hashtable>> (datas,path);
		}

		public static void Load(Dictionary<string,GameSaverable> loaders,string path){
			if (null == loaders || 0 == loaders.Count){
				return;
			}
			Dictionary<string,Hashtable> datas = SerializerUtils.LoadAsBinary<Dictionary<string,Hashtable>> (path);
			foreach(KeyValuePair<string, GameSaverable> en in loaders)
			{
				GameSaverable gs = en.Value;

				gs.LoadGame (datas[en.Key]);
			}
		}
	}
}
