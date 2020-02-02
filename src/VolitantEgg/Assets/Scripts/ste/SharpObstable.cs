using System;
using UnityEngine;
using System.Collections.Generic;
using kazgame.ui;
using kazgame.utils;

namespace kazgame.ste
{
    public class SharpObstable : Actor
    {
        public EdgeCollider2D guid;
        public List<Sprite> sprites;
        public float angle = 0f;

        private readonly List<SpriteRenderer> _renders = new List<SpriteRenderer>();

        protected override void DoInit()
        {
            base.DoInit();
            if (null == guid)
            {
                guid = GetComponent<EdgeCollider2D>();
                if (null == guid)
                {
                    guid = GetComponentInChildren<EdgeCollider2D>();
                }
                if (null == guid)
                {
                    throw new Exception("guid not set");
                }
            }

            Vector2[] ps = guid.points;
            int indexSp = 0;
            Transform dp = guid.transform;
            foreach (Vector2 item in ps)
            {
                GameObject go = new GameObject();
                go.transform.SetParent(dp);
                Vector3 pos = transform.TransformPoint(item);
                pos.z = transform.position.z;
                go.transform.position = pos;
                SpriteRenderer render = go.AddComponent<SpriteRenderer>();
                _renders.Add(render);
                render.sprite = sprites[indexSp];
                render.transform.Rotate(new Vector3(0, 0, angle));
                go.name = render.sprite.name;
                SpriteRenderFlashEffect effect = go.AddComponent<SpriteRenderFlashEffect>();
                effect.flashColor = Color.black;
                //for event
                PolygonCollider2D collider2d = go.AddComponent<PolygonCollider2D>();
                indexSp = (indexSp + 1) % sprites.Count;
                EventForwarder eventForwarder = go.AddComponent<EventForwarder>();
                eventForwarder.OnCollisionEnter2DEvent += HandleCollider;
            }
        }

        void Awake()
        {
            InitSelf();
        }

        void HandleCollider(Collision2D collision)
        {
            // TODO:解决抖动碰撞问题，相隔很远也碰撞了，这时似乎向量为负数
            Log.Info("HandleCollider >>>> {0} {1}", collision.contactCount, collision.relativeVelocity);
            // if (collision.relativeVelocity.x < 0)
            // {
            //     return;
            // }

            CheckCollideWithEgg(collision.collider, GameOverReason.obstacle, gameObject.name);
        }

        //		public void OnCollisionEnter2D(Collision2D collision)
        //		{
        //			//for the edge line
        //			HandleCollider (collision);
        //		}
    }
}

