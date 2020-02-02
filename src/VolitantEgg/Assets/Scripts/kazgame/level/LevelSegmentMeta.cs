using UnityEngine;
using System.Collections;

namespace kazgame.level
{
	[System.Serializable]
	public class LevelSegmentMeta {
		public enum LoadState
		{
			not,loading,loaded,unloaded
		}

		public Vector3 position;
		public string prefPath;
		public float length;
		public LoadState state;

		public override string ToString(){

			return string.Format ("{0};{1};{2};{3}",position,prefPath,length,state);
		}
	}
}
