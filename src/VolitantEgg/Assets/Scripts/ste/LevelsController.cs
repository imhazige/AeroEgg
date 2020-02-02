using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using kazgame.objectpool;
using kazgame.utils;
using kazgame.ui;
using UnityEngine.Advertisements;

namespace kazgame.ste
{
    public class LevelsController : MonoBase
    {
        public Transform objectPoolParents;

        public LevelsMenuController levelsMenuController;

        public LineController lineController;

        public EggController eggController;

        public AppLevelController levelController;

        public Clouds clouds;

        public Text lableCountDown;

        public UITextFadeEffect lableSavePoint;

        public GameObject continuePointsParent;

        public service.UnityAds ads;

        private ContinuePoint[] continuePoints;

        private long lastShowAds = 0;

        private ContinuePoint lastSavePoint
        {
            set;
            get;
        }

        public int runningCount
        {
            private set;
            get;
        }

        private Vector3 lastEggPos
        {
            set;
            get;
        }

        public bool pausing
        {
            private set;
            get;
        }

        public ObjectPoolController objectPoolController
        {
            get;
            private set;
        }

        public const string PK_E9AttackThing = "PK_E9AttackThing";
        public const string PK_ButterFly = "PK_ButterFly";
        public E9AttackThing copyE9AttackThing;
        public ButterFly butterfly;

        public bool debugNoFail
        {
            get;
            set;
        }

        private TracerCameraController _cameraController;

        private Coroutine _countDownCor;

        private Coroutine corRandom;

        private Vector3 cameraStartPos;

        public bool gameover
        {
            get;
            private set;
        }

        public bool gameComplete
        {
            get;
            private set;
        }

        protected override void DoInit()
        {
            base.DoInit();
            objectPoolController = new ObjectPoolController(objectPoolParents);
            scoreController = new ScoreController();
            cameraStartPos = Camera.main.transform.position;
            _cameraController = new TracerCameraController();
            _cameraController.traceX = false;
            _cameraController.ySmooth = 0;
            _cameraController.yMargin = 0;
            continuePoints = continuePointsParent.GetComponentsInChildren<ContinuePoint>();
            InitPool();
        }

        private ContinuePoint getNearContinuePositionC(float y)
        {
            ContinuePoint lastitem = null;
            foreach (ContinuePoint item in continuePoints)
            {
                //				Log.Info ("-----item {0} - {1}. >> {2}",item.name,item.transform.position,y);

                if (item.transform.position.y < y)
                {
                    continue;
                }
                float dis = Math.Abs(item.transform.position.y - y);
                if (null != lastitem)
                {
                    if (Math.Abs(lastitem.transform.position.y - y) > dis)
                    {
                        lastitem = item;
                    }
                }
                else
                {
                    lastitem = item;
                }
            }

            return lastitem;
        }

        private Vector3 getNearContinuePosition(float y)
        {
            ContinuePoint lastitem = getNearContinuePositionC(y);

            if (null == lastitem)
            {
                Log.Info("can not found last cunitnue run point. {0}", y);
                return Vector3.zero;
            }
            else
            {
                Log.Info("found last cunitnue run point {0} - {1} - {2}.", lastitem.name, lastitem.transform.position, y);
                return lastitem.transform.position;
            }
        }

        void Awake()
        {
            InitSelf();
        }

        void InitPool()
        {
            objectPoolController.InitPool<E9AttackThing>(PK_E9AttackThing, copyE9AttackThing, 5);
            objectPoolController.InitPool<ButterFly>(PK_ButterFly, butterfly, 3, false);
        }

        public ScoreController scoreController
        {
            get;
            private set;
        }

        void Update()
        {
            if (gameover)
            {
                return;
            }
            int speed = (int)eggController.speed;
            levelsMenuController.tipSpeed.text = speed + "";
            int score = (int)eggController.flyingLength;
            levelsMenuController.tipHeight.ChangeValue(score);
            scoreController.RecordScore(score);

            if (!gameComplete)
            {
                ShowSpeed(speed);

                ContinuePoint lastCp = getNearContinuePositionC(eggController.transform.position.y);
                if (null != lastCp)
                {
                    if (null == lastSavePoint || lastCp != lastSavePoint)
                    {
                        lastSavePoint = lastCp;
                        lableSavePoint.Fade("Reached savepoint!");
                    }
                }
            }

        }

        public void ShowSpeed(int speed)
        {
            if (speed >= Constants.crack_speed)
            {
                //				Log.Debug ("code go to here.......");
                levelsMenuController.tipSpeed.color = Constants.color_alert;
                levelsMenuController.speedimg.enabled = true;
                GameController.GetSingleton().audioController.PlaySpeedAlert();
                levelsMenuController.ShowStatusTip("Too Fast!");
            }
            else
            {
                levelsMenuController.HideStatusTip();
                levelsMenuController.speedimg.enabled = false;
                //不用停止，太快了
                // GameController.GetSingleton().audioController.StopSpeedAlert();
                levelsMenuController.tipSpeed.color = Color.white;
            }
        }


        void Reset()
        {
            pausing = false;
            gameover = false;
            gameComplete = false;
            scoreController.Reset();
            eggController.StopRun();
            levelsMenuController.Reset();
            lineController.Reset();
            lableCountDown.gameObject.SetActive(false);
            lableCountDown.color = Color.red;
            lableSavePoint.gameObject.SetActive(false);
            GameObjectUtils.CleanChild(objectPoolParents.gameObject);
            Camera.main.transform.position = cameraStartPos;
            GameController.singleton.audioController.StopPlayMusic();
            GameController.singleton.audioController.StopSpeedAlert();

            PlaySoundComp[] pcs = gameObject.GetComponentsInChildren<PlaySoundComp>();
            foreach (PlaySoundComp item in pcs)
            {
                item.StopPlayMusic();
            }
            if (null != corRandom)
            {
                StopCoroutine(corRandom);
            }
        }

        public void StartLevel(bool continueRun = false, Vector3 argLastEggPos = default(Vector3))
        {
            InitSelf();

            //clean up, 
            Reset();

            Vector3 startPos = Vector3.zero;
            if (continueRun)
            {
                if (argLastEggPos != default(Vector3))
                {
                    Log.Info("assigned start point... {0}", argLastEggPos);
                    lastEggPos = argLastEggPos;
                }
                //level start position
                startPos = getNearContinuePosition(lastEggPos.y);
            }
            else
            {
                runningCount = 0;
                lastEggPos = Vector3.zero;
            }

            gameObject.SetActive(true);
            levelsMenuController.StartShow();
            clouds.StopShow();
            clouds.StartShow();
            _cameraController.StartTrace(Camera.main, eggController.transform);
            //start count down
            if (null != _countDownCor)
            {
                StopCoroutine(_countDownCor);
                _countDownCor = null;
            }
            _countDownCor = StartCoroutine(DoCountDown(startPos));
        }

        IEnumerator DoRandomUpdate()
        {
            while (!gameover && !gameComplete)
            {
                //butterfly
                bool rb = UnityEngine.Random.Range(0, 2) > 0;
                if (rb)
                {
                    ButterFly bf = objectPoolController.InitiateFromPool<ButterFly>(PK_ButterFly);
                    if (null != bf)
                    {
                        bf.OnActiveFromPool();
                    }
                }
                yield return new WaitForSeconds(5);
            }

        }

        IEnumerator DoCountDown(Vector3 startPos)
        {
            //允许提前画线，让玩家有点感觉
            lineController.StartShow();
            //预加载,需要重置的应在在这里
            levelController.ResetSegments();
            levelController.ManageSegment();

            eggController.StartRun(startPos);
            eggController.OnPause();
            levelsMenuController.EnableAll(false);
            lableCountDown.gameObject.SetActive(true);
            lableCountDown.color = Color.red;
            int counted = 0;
            const int countDown = 3;
            while (counted < countDown)
            {
                if (!pausing)
                {
                    lableCountDown.text = countDown - counted + "";
                    counted++;
                    GameController.GetSingleton().audioController.PlayCountDown();
                }
                yield return new WaitForSeconds(1f);
            }
            lableCountDown.color = Constants.color_go;
            lableCountDown.text = "GO!";
            GameController.GetSingleton().audioController.PlayGo();
            yield return new WaitForSeconds(1f);

            levelController.OnResume();


            lableCountDown.gameObject.SetActive(false);
            levelsMenuController.EnableAll(true);
            GameController.singleton.audioController.PlayMainGameMusics();

            eggController.OnResume();


            PlaySoundComp[] pcs = gameObject.GetComponentsInChildren<PlaySoundComp>();
            foreach (PlaySoundComp item in pcs)
            {
                item.PlayMusic();
            }
            corRandom = StartCoroutine(DoRandomUpdate());
        }

        void FixedUpdate()
        {
            _cameraController.Update();
        }

        public void StartShow(bool continueRun = false, Vector3 startPoint = default(Vector3))
        {
            const long intervalAds = 1000 * 60 * 2;
            // const long intervalAds = 0;
            long interval = DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastShowAds;
            if (intervalAds < interval)
            {
                ads.ShowPreload((ShowResult result) =>
                            {
                                if (result != ShowResult.Failed)
                                {
                                    lastShowAds = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                                }
                                Log.Info("ads show result." + result);
                                StartLevel(continueRun, startPoint);
                            });
            }
            else
            {
                Log.Info("not show ads as a  small interval." + interval);
                StartLevel(continueRun, startPoint);
            }

        }

        public void StopShow()
        {
            InitSelf();
            Reset();
            gameObject.SetActive(false);
            levelsMenuController.StopShow();
        }

        public void GameComplete()
        {
            //game over
            gameComplete = true;
            Log.Info("Game Complete.");
            //partly pause
            _cameraController.StopTrace();
            lineController.Reset();
            levelController.OnPause();
            GameController.singleton.audioController.StopSpeedAlert();
            levelsMenuController.tipSpeed.color = Color.white;
            levelsMenuController.ShowStatusTip("Congrats!");
            GameController.singleton.audioController.PlayLevelComplete();
            eggController.ShowGameComplateAnimate(GameCompleteAnimationDoneCallBack);
            scoreController.ReportScore(scoreController.score);
            PlayerPrefs.SetInt(Constants.prefkeyLastScore, scoreController.score);
        }

        void GameCompleteAnimationDoneCallBack()
        {
            eggController.ShowGameCongrasAnimate();
        }

        public void GameOver(Transform source, GameOverReason reason, string detail)
        {
            if (Constants.debug_model && debugNoFail)
            {
                return;
            }
            if (Constants.debug_nofail)
            {
                return;
            }

            lastEggPos = eggController.transform.position;
            runningCount++;

            Log.Info("game over count {0},{1}", lastEggPos, runningCount);

            //game over
            gameover = true;
            if (GameOverReason.collideWithSpeed.Equals(reason))
            {
                ShowSpeed(Mathf.Max((int)eggController.speed, Constants.crack_speed));
            }
            Log.Info("Game Over:{0} - {1}", reason, detail);

            //:Save lastEggPos
            PlayerPrefs.SetFloat(Constants.prefkeyGameLastPointY, lastEggPos.y);
            Log.Info("SET LAST ---- {0}", lastEggPos.y);
            //partly pause
            _cameraController.StopTrace();
            lineController.Reset();
            levelController.OnPause();
            GameController.singleton.audioController.StopSpeedAlert();
            levelsMenuController.ShowStatusTip("Game Over");
            eggController.ShowGameOverAnimate(AnimationDoneCallBack);
            GameController.singleton.audioController.PlayGameOver();
        }

        void AnimationDoneCallBack()
        {
            levelsMenuController.ShowGameOverMenu();
        }

        public void PauseGame()
        {
            if (pausing)
            {
                return;
            }
            pausing = true;
            eggController.OnPause();
            _cameraController.OnPause();
            lineController.OnPause();
            levelController.OnPause();
            clouds.OnPause();
            GameController.GetSingleton().audioController.OnPause();

            PauseAble[] pps = objectPoolParents.GetComponentsInChildren<PauseAble>();
            foreach (PauseAble item in pps)
            {
                item.OnPause();
            }
        }

        public void ResumeGame()
        {
            if (!pausing)
            {
                return;
            }
            pausing = false;
            eggController.OnResume();
            _cameraController.OnResume();
            lineController.OnResume();
            levelController.OnResume();
            clouds.OnResume();
            GameController.GetSingleton().audioController.OnResume();

            PauseAble[] pps = objectPoolParents.GetComponentsInChildren<PauseAble>();
            foreach (PauseAble item in pps)
            {
                item.OnResume();
            }
        }


        void OnApplicationPause(bool pauseStatus)
        {
            if (eggController.running && !pausing && !gameover)
            {
                levelsMenuController.ShowPauseMenu();
            }
        }
    }
}

