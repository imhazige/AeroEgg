using System;
using UnityEngine;
using kazgame.utils;
using kazgame.ui;
using System.Collections;
using UnityEngine.Advertisements;
using kazgame.service;

namespace kazgame.ste
{
    public class LoadingPage : MonoBase
    {
        public SpriteUnitProgressbar loadingBar;
        public Transform brand;
        public GameObject otherPart;
        public bool autoStart = true;

        private UnityAds ads;

        void Awake()
        {
            InitSelf();
        }

        void Start()
        {
            ConnectGameCenter();
            if (!autoStart)
            {
                Log.Info("loading page auto start is false, do nothing.");

                return;
            }
            ShowLoadingPage();
        }

        protected override void DoInit()
        {
            base.DoInit();
            ads = GetComponent<UnityAds>();
        }

        void ShowLoadingPage()
        {
            gameObject.SetActive(true);
            if (null != otherPart)
            {
                otherPart.SetActive(true);
            }
            if (null != brand)
            {
                brand.gameObject.SetActive(true);
            }

            loadingBar.StartProgress();
            //load all the resources
            ResourceRequest req = Resources.LoadAsync("");

            StartCoroutine(DoLoading(req));
        }


        IEnumerator DoLoading(ResourceRequest req)
        {
            Log.Info("loaded " + req.progress);
            //			yield return new WaitForSeconds (1);
            //mock
            const int mockCount = 3;
            const float maockRate = .9f;

            for (int i = 0; i < mockCount; i++)
            {
                loadingBar.UpdateProgress(.3f * (i + 1));
                yield return new WaitForSeconds(1);
            }
            Log.Debug("loaded step1");
            //
            while (req.progress < maockRate)
            {
                yield return null;
            }
            Log.Debug("loaded step2");

            while (req.progress < 1f)
            {
                Log.Info("loaded " + req.progress);
                loadingBar.UpdateProgress(req.progress);
                //				yield return new WaitForSeconds (1);
                yield return null;
            }
            Log.Debug("loaded step3");
            loadingBar.UpdateProgress(1);
            yield return new WaitForSeconds(1);

            //			LoadMainMenu ();
            LoadAds();
        }

        private void LoadAds()
        {
            StoreManager storeManager = GameController.GetSingleton().storeManager;
            Log.Info("in LoadAds");
            Log.Info("bought noads? " + storeManager.havePurchasedNoAds);
            //do not load until play the game many times
            int playedTime = PlayerPrefs.GetInt(Constants.prefkeyGamePlayed, 0);
            // preload ads
            ads.WaitReady(HandleShowResult, 5);
            // LoadMainMenu();
            return;
            //let main menu show ads
            if (playedTime > Constants.showAdsUntilPlayed)
            {
                //only load when is in wifi or cable
                if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
                {

                    // check if purchased 
                    if (!storeManager.havePurchasedNoAds)
                    {
                        Log.Info("try show ads.");
                        ads.StartShow(HandleShowResult, 5);
                    }
                    else
                    {
                        Log.Info("user have purchased noads, ignore ads.");
                        LoadMainMenu();
                    }
                }
                else
                {
                    Log.Info("no network, ignore ads.");
                    LoadMainMenu();
                }
            }
            else
            {
                playedTime += 1;
                PlayerPrefs.SetInt(Constants.prefkeyGamePlayed, playedTime);
                Log.Debug("played time now is " + playedTime);
                LoadMainMenu();
            }
        }

        private void LoadMainMenu()
        {
            Log.Debug("in LoadMainMenu");
            gameObject.SetActive(false);
            if (null != otherPart)
            {
                otherPart.SetActive(false);
            }
            if (null != brand)
            {
                brand.gameObject.SetActive(false);
            }

            GameController.GetSingleton().mainMenuController.Show();
        }

        private void HandleShowResult(ShowResult result)
        {
            Log.Info("ads show result." + result);
            LoadMainMenu();
        }

        public void ConnectGameCenter()
        {
            SocialManager.Connect();
        }


    }
}

