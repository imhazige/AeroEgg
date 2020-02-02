using System;
using UnityEngine;
using kazgame.utils;

namespace kazgame.ste
{
	public class O2 : SharpObstable
	{
		protected override void DoInit ()
		{
			const int total = 3;
			sprites = new System.Collections.Generic.List<UnityEngine.Sprite> (total);
			for (int i = 0; i < total; i++){
				Sprite sp = Resources.Load<Sprite> (string.Format("Img/O2-{0}",i+1));

				sprites.Add (sp);
			}

			base.DoInit ();
			Vector2DUtils.ChangePositionZ (transform,(float)ZIndexs.o2);
		}
	}
}

