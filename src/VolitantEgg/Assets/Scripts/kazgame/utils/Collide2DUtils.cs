using System;
using UnityEngine;

namespace kazgame.utils
{
	public static class Collide2DUtils
	{
		public struct VelocityData{
			public Vector2 velocity;
			public float angularVelocity;
			public bool isKinematic;
		}
		
		public static VelocityData StopRigidVelocity(Rigidbody2D rigid){
			VelocityData data = new VelocityData ();
			data.velocity = rigid.velocity;
			data.angularVelocity = rigid.angularVelocity;
			data.isKinematic = rigid.isKinematic;
			rigid.velocity = Vector2.zero;
			rigid.angularVelocity = 0f;
			rigid.isKinematic = true;


			return data;
		}

		public static void RestoreRigidVelocity(Rigidbody2D rigid,VelocityData data){
			rigid.velocity = data.velocity;
			rigid.angularVelocity = data.angularVelocity;
			rigid.isKinematic = data.isKinematic;
		}
	}
}

