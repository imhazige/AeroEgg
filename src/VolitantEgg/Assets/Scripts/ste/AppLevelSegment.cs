using System;
using kazgame.level;

namespace kazgame.ste
{
	public class AppLevelSegment : LevelSegment
	{
		public AppLevelSegment(){
			#if (UNITY_EDITOR)
			direction2D = Direction2D.down;
			#endif
		}
	}
}

