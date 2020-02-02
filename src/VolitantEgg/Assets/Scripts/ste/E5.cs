using System;
using UnityEngine;
using kazgame.animation;
using System.Collections;
using kazgame.utils;
using kazgame.character;
using System.Collections.Generic;

namespace kazgame.ste
{
    public class E5 : Actor
    {
        public EdgeCollider2D moveGuid;
        public SpriteAnimator animRun;
        public SpriteAnimator animJump;
        public SpriteAnimator animStare;
        public SpriteAnimator animAttack;

        public List<int> adjustPolygonColliderIndexs;

        public float moveSpeed = 2f;
        public float sensibleDistance = 5f;
        public List<int> jumpIndexs;

        public Collider2D collid2d;
        public Rigidbody2D rigid;
        public float attackDistance = 2f;
        public float attackSpeed = 2f;

        private Coroutine _cor;
        private XYTracerGuidMoveController tracer;
        private bool attacking
        {
            get;
            set;
        }

        protected override void DoInit()
        {
            base.DoInit();
            if (null == moveGuid)
            {
                moveGuid = GetComponentInChildren<EdgeCollider2D>();
            }
            if (null == rigid)
            {
                rigid = GetComponent<Rigidbody2D>();
            }
            if (null == collid2d)
            {
                collid2d = GetComponent<Collider2D>();
            }

            rigid.isKinematic = true;
            collid2d.isTrigger = true;
            tracer = new XYTracerGuidMoveController();
            tracer.transform = transform;
            tracer.guid = moveGuid;
            tracer.moveSpeed = moveSpeed;
            tracer.ReachPointEvent += Tracer_ReachPointEvent;
            tracer.FlipingEvent += Tracer_FlipingEvent;
            tracer.ReachTargetEvent += Tracer_ReachTargetEvent;
            tracer.excludesIndex = jumpIndexs;

            animStare.loopCount = 1;
            animStare.OnAnimateEvent += OnAnimateEventDelegate;
            animAttack.loopCount = 1;
            animAttack.OnAnimateEvent += OnAnimateEventDelegate;
            Vector2DUtils.ChangePositionZ(transform, (float)ZIndexs.enemy);
        }

        void OnAnimateEventDelegate(string eventName, SpriteAnimator animator)
        {
            if (animator == animStare)
            {
                if (SpriteAnimator.EVENT_LOOPEND == eventName)
                {
                    animAttack.StartPlay();
                    _cor = StartCoroutine(DoAttack());
                }
                else if (SpriteAnimator.EVENT_LOOP_INDEX == eventName)
                {
                    if (adjustPolygonColliderIndexs.Contains(animStare.loopIndex))
                    {
                        ResetTriggerPolygionCollider(null);
                    }
                }
            }
            else if (animator == animAttack)
            {
                if (SpriteAnimator.EVENT_LOOPEND == eventName)
                {
                    //					tracer.OnResume ();
                    //					animRun.StartPlay ();
                }
            }
        }

        IEnumerator DoAttack()
        {
            attacking = true;
            Vector3 startPos = transform.position;
            Vector3 endPos = transform.position + new Vector3(0, attackDistance, 0);

            while (transform.position.y - startPos.y < attackDistance)
            {
                if (!pausing)
                {
                    Vector2DUtils.MoveToSmoothly(transform, endPos, attackSpeed);
                }

                yield return null;
            }

            while (transform.position.y - startPos.y > Mathf.Epsilon)
            {
                if (!pausing)
                {
                    Vector2DUtils.MoveToSmoothly(transform, startPos, attackSpeed);
                }

                yield return null;
            }

            attacking = false;
            animRun.StartPlay();
            tracer.OnResume();
        }

        void Tracer_ReachTargetEvent(XYTracerGuidMoveController param)
        {
            tracer.OnPause();
            //			Log.Debug ("reach event {0}",param.transform.position);
            animRun.StopPlay();
            animStare.StartPlay();
            //when animate done, need resume
        }

        void Tracer_FlipingEvent(GuidMoveController param)
        {
            Vector2DUtils.Flip(transform);
        }

        void Tracer_ReachPointEvent(GuidMoveController param)
        {
            //			Log.Debug ("moving to {0}",tracer.movingIndex);
            //			if (param.movingIndex == 1 && tracer.fliping){
            //				tracer.Flip ();	
            //			}
            //			if (param.movingIndex == 2 && !tracer.fliping){
            //				tracer.Flip ();	
            //			}
            bool inJump = false;
            if (tracer.fliping)
            {
                if (jumpIndexs.Contains(param.movingIndex - 1))
                {
                    inJump = true;
                }
            }
            else
            {
                if (jumpIndexs.Contains(param.movingIndex))
                {
                    inJump = true;
                }
            }
            if (inJump)
            {
                //				Log.Debug ("need jump {0}",tracer.movingIndex);
                if (!animJump.running)
                {
                    //					Log.Debug ("start jump {0}",tracer.movingIndex);
                    animJump.StartPlay();
                    animRun.StopPlay();
                }
            }
            else
            {
                //				Log.Debug ("go to normal {0}",tracer.movingIndex);
                animJump.StopPlay();
                animRun.StartPlay();
            }
        }

        void Awake()
        {
            InitSelf();
        }

        void Start()
        {
            tracer.Start();
            //_cor = StartCoroutine (DoMyUpdate());
        }

        void Update()
        {
            if (attacking)
            {

            }
            Transform egg = GameController.GetSingleton().levelsController.eggController.transform;
            float dis = Mathf.Abs(egg.position.y - transform.position.y);
            //			Log.Debug ("1 {0},2 {1}",egg.position.y > transform.position.y - 0.05f,dis);
            if (egg.position.y > transform.position.y - 1f && sensibleDistance >= dis)
            {
                tracer.target = egg;
            }
            else
            {
                tracer.target = null;
            }

            tracer.Update();
        }

        public override void OnPause()
        {
            base.OnPause();
            animRun.OnPause();
            animJump.OnPause();
            animStare.OnPause();
            animAttack.OnPause();
            tracer.OnPause();
        }

        public override void OnResume()
        {
            base.OnResume();
            animRun.OnResume();
            animJump.OnResume();
            animStare.OnResume();
            animAttack.OnResume();
            tracer.OnResume();
        }

        void OnTriggerEnter2D(Collider2D parCollider)
        {
            CheckCollideWithEgg(parCollider, GameOverReason.enemy, "E5");
            CheckCollideWithLine(parCollider);
        }

    }
}

