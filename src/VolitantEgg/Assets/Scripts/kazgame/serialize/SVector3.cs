using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace kazgame.serialize
{
	[System.Serializable]
	public struct SVector3 {
		float x;
		float y;
		float z;

		public SVector3(Vector3 v3){
			x = v3.x;
			y = v3.y;
			z = v3.z;
		}

		public Vector3 ToVector3 ()
		{
			return new Vector3 (x,y,z);
		}

		public static List<SVector3> List (List<Vector3> list)
		{
			if (null == list){
				return null;
			}
			List<SVector3> ls = new List<SVector3> ();

			foreach(Vector3 v in list){
				ls.Add (new SVector3(v));
			}

			return ls;
		}
	}
}
