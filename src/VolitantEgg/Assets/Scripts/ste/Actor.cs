using System;
using UnityEngine;
using kazgame.objectpool;
using kazgame.ui;
using kazgame.utils;

namespace kazgame.ste
{
    public abstract class Actor : MonoBase, Poolable, PauseAble
    {
        private bool _active;
        private bool _pausing = false;

        protected virtual void Awake()
        {
            InitSelf();
        }

        protected virtual void Start()
        {
        }

        protected override void DoInit()
        {
            base.DoInit();
        }

        protected virtual void Reset()
        {
            _pausing = false;
        }

        virtual public void StopAct()
        {
            gameObject.SetActive(false);
            _active = false;
        }

        #region PauseAble implementation

        public virtual void OnPause()
        {
            _pausing = true;
        }

        public virtual void OnResume()
        {
            _pausing = false;
        }

        #endregion

        public bool pausing
        {
            get
            {
                return _pausing;
            }
        }

        public void ResetTriggerPolygionCollider(PolygonCollider2D collider)
        {
            if (null == collider)
            {
                collider = GetComponent<PolygonCollider2D>();
            }
            GameObject attached = collider.gameObject;
            Destroy(collider);
            collider = attached.AddComponent<PolygonCollider2D>();
            collider.isTrigger = true;
        }

        #region Poolable implementation
        public void OnActiveFromPool()
        {
            Reset();
            _active = true;
        }
        public bool able2Pool
        {
            get
            {
                return !_active;
            }
        }
        #endregion

        protected bool CheckCollideWithEgg(Collider2D collider, GameOverReason reason, string detail)
        {
            if (null != collider && collider.gameObject == GameController.GetSingleton().levelsController.eggController.gameObject)
            {
                ColliderDistance2D dis = collider.Distance(gameObject.GetComponent<Collider2D>());
                Log.Info("xxx {0} {1} {2}", dis.distance, dis.isOverlapped, detail);
                if (dis.distance < 0.1 || dis.isOverlapped)
                {
                    GameController.GetSingleton().levelsController.levelController.GameOver(transform, reason, detail);

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// will destroy collided line
        /// </summary>
        /// <param name="parCollider">Par collider.</param>
        protected bool CheckCollideWithLine(Collider2D parCollider)
        {
            if (null != parCollider)
            {
                Line line = parCollider.GetComponent<Line>();
                if (null != line)
                {
                    line.ForceDestroyLine();
                    return true;
                }
            }

            return false;
        }
    }
}

