using System;
using UnityEngine;
using kazgame.objectpool;
using System.Collections;
using System.Collections.Generic;

namespace kazgame.objectpool
{
	public class ObjectPoolController
	{
		private readonly Transform cloneObjectParent; 
		private readonly Hashtable _actorPools;

		public ObjectPoolController(Transform cloneObjectParent){
			_actorPools = new Hashtable();
			this.cloneObjectParent = cloneObjectParent;
		}

		public T InitiateFromPool<T> (string name)where T : MonoBehaviour,Poolable
		{
			ObjectPool<T> pool = (ObjectPool<T>)_actorPools [name];

			return pool.GetPooledObject ();
		}

		public ObjectPool<T> InitPool<T> (string name,T copy, int poolSize, bool allowGrowth = true) where T : MonoBehaviour,Poolable
		{
			ObjectPool<T> pool = new ObjectPool<T> (copy);
			pool.pooledAmount = poolSize;
			pool.cloneObjectsParent = cloneObjectParent;
			pool.allowGrow = allowGrowth;
			pool.InitPool ();

			_actorPools.Add (name,pool);

			return pool;
		}
	}
}

