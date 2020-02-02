using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace kazgame.objectpool
{
	public class ObjectPool<T> where T : Component,Poolable
	{
		private T example;
		public int pooledAmount = 20;
		public bool allowGrow = true;
		public Transform cloneObjectsParent;

		private List<T> pooledObjects;

		private bool _inited;

		public ObjectPool(T aexample){
			example = aexample;
		}

		public void InitPool(){
			if (_inited){
				return;
			}
			pooledObjects = new List<T>();
			for(int i = 0; i < pooledAmount; i++)
			{
				T obj = NewClone ();
				pooledObjects.Add(obj);
			}
			_inited = true;
		}

		public T GetPooledObject()
		{
			if (!_inited){
				return null;
			}
			for(int i = 0; i< pooledObjects.Count; i++)
			{
				//if pool size have not reach max, new one
				if(pooledObjects[i] == default(T))
				{
					T obj = NewClone ();
					obj.OnActiveFromPool ();
					pooledObjects[i] = obj;
					return pooledObjects[i];
				}
				//as pool have all inited, try find idle object in the pool
				if(pooledObjects[i].able2Pool)
				{
					pooledObjects[i].OnActiveFromPool ();
					return pooledObjects[i];
				}    
			}

			//if allow growth, new one
			if (allowGrow)
			{
				T obj = NewClone ();
				pooledObjects.Add(obj);
				obj.OnActiveFromPool ();
				return obj;
			}

			return null;
		}

		T NewClone(){
			T t = Object.Instantiate<GameObject>(example.gameObject).GetComponent<T>();

			if (null != cloneObjectsParent){
				t.transform.SetParent (cloneObjectsParent);
			}

			t.gameObject.SetActive (false);

			return t;
		}

		void Destroy(){
			if (!_inited){
				return;
			}
			foreach (T obj in pooledObjects){
				if (null != obj){
					Object.Destroy (obj.gameObject);
				}
			}
			_inited = false;
		}

		public List<T> allObjects{
			get{ 
				return new List<T>(pooledObjects);
			}
		}
	}
}