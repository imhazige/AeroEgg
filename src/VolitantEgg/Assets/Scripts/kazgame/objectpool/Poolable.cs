using UnityEngine;
using System.Collections;

namespace kazgame.objectpool
{
	public interface Poolable
	{
		void OnActiveFromPool ();

		bool able2Pool {
			get;
		}
	}
}
