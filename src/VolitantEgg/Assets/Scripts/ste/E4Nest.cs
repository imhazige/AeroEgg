using System;
using UnityEngine;
using kazgame.animation;
using System.Collections;
using kazgame.utils;

namespace kazgame.ste
{
	class E4Nest : MonoBase
	{
		protected override void DoInit ()
		{
			base.DoInit ();
			Vector2DUtils.ChangePositionZ (transform,(float)ZIndexs.e4nest);
		}

		void Awake(){
			InitSelf ();
		}
	}

}

