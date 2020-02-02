using System;
using UnityEngine;
using kazgame.utils;
using kazgame.character;
using kazgame.debug;

namespace kazgame.ste
{
	public static class UnitTest
	{
		public static void test(){
//			PrintScreenSize ();
//			GetUniqNameByHierarchy();
//			LineLineIntersection();
			Angle();
		}

		public static void PrintScreenSize(){
			Vector3 ru = Camera.main.ScreenToWorldPoint (new Vector2(Screen.width,Screen.height));
			Vector3 lb = Camera.main.ScreenToWorldPoint (new Vector2(0,0));
			Log.Debug ("screen size {0}",ru-lb);
		}

		public static void GetUniqNameByHierarchy(){
			string name = GameObjectUtils.GetUniqNameByHierarchy (Camera.main.gameObject);
			Log.Debug ("name is {0}",name);
		}

		public static void LineLineIntersection(){
			Vector3 pInter = Vector3.zero;
			Vector3 p1 = new Vector3 (0,0);
			Vector3 p2 = new Vector3 (10,10);
			Vector3 vx = new Vector3 (p1.x,10);
			Vector3 vx1 = new Vector3 (p1.x, -10);
			DebugUtils.DrawPoint (p1); 
			DebugUtils.DrawPoint (p2); 
			Debug.DrawLine (p1,p2);	
			Debug.DrawLine (vx,vx1);	
//			DebugUtils.DrawPoint (vx); 
//			DebugUtils.DrawPoint (vx1); 
			bool cross = Vector2DUtils.LineLineIntersection (out pInter,p1, p2, vx, vx1);
			if (cross) {
				Log.Debug ("has {0}",pInter);
				Debug.DrawLine (Vector3.zero, pInter);	
			} else {
				Log.Debug ("null.....");
			}
		}

		public static void Angle(){
			Vector2 v1 = new Vector2 (0,0);
			Vector2 v0 = new Vector2(0,0);
			float an = 0;

			v1 = new Vector2 (1,1);
			an = Vector2DUtils.XAngleOfLineSegment (v0 , v1);
			Log.Debug ("{0} angle is {1}",v1,an);

			v1 = new Vector2 (-1,1);
			an = Vector2DUtils.XAngleOfLineSegment (v0 , v1);
			Log.Debug ("{0} angle is {1}",v1,an);

			v1 = new Vector2 (-1,-1);
			an = Vector2DUtils.XAngleOfLineSegment (v0 , v1);
			Log.Debug ("{0} angle is {1}",v1,an);

			v1 = new Vector2 (1,-1);
			an = Vector2DUtils.XAngleOfLineSegment (v0 , v1);
			Log.Debug ("{0} angle is {1}",v1,an);
		}
	}
}

