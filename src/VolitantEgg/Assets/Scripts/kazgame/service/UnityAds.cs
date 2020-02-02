using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;
using kazgame.utils;

namespace kazgame.service
{
    public class UnityAds : MonoBase
    {
        public delegate void OnComplete(ShowResult result);

        private bool _ready;
        private OnComplete _fn;
        private int _timeout;

        void Start()
        {
            //			ShowAdPlacement (); 
        }

        public void WaitReady(OnComplete fn, int timeout = 5)
        {
            Log.Debug("ads WaitReady");
            _fn = fn;
            _timeout = timeout;
            StartCoroutine(DoPreLoad());
        }

        public void ShowPreload(OnComplete fn)
        {
            _fn = fn;
            ShowOptions options = new ShowOptions();
            options.resultCallback = HandleShowResult;
            Log.Debug("invoke show.");
            Advertisement.Show(null, options);
        }

        IEnumerator DoPreLoad()
        {
            //timeout
            float time = Time.time;
            while (!Advertisement.IsReady())
            {
                Log.Debug("Time.time - time = {0}", Time.time - time);
                if (Time.time - time > _timeout)
                {
                    Log.Debug("adstimeout.");
                    if (null != _fn)
                    {
                        _fn(ShowResult.Failed);
                    }
                    yield break;
                }

                yield return null;
            }

            _fn(ShowResult.Finished);

            Log.Debug("ads ready.");
        }

        public void StartShow(OnComplete fn, int timeout = 5)
        {
            Log.Debug("ads StartShow");
            _fn = fn;
            _timeout = timeout;
            if (_ready)
            {
                Log.Debug("already ready, show.");
                ShowAdPlacement();

                return;
            }
            StartCoroutine(DoReady());
        }

        IEnumerator DoReady()
        {
            //timeout
            float time = Time.time;
            while (!Advertisement.IsReady())
            {
                Log.Debug("Time.time - time = {0}", Time.time - time);
                if (Time.time - time > _timeout)
                {
                    Log.Debug("adstimeout.");
                    if (null != _fn)
                    {
                        _fn(ShowResult.Failed);
                    }
                    yield break;
                }

                yield return null;
            }

            Log.Debug("ads ready and show.");
            ShowAdPlacement();
        }

        void ShowAdPlacement()
        {
            ShowOptions options = new ShowOptions();
            options.resultCallback = HandleShowResult;
            Log.Debug("invoke show.");
            Advertisement.Show(null, options);
        }

        private void HandleShowResult(ShowResult result)
        {
            Log.Debug("ads show result." + result);
            _ready = true;
            if (null != _fn)
            {
                _fn(result);
            }
            switch (result)
            {
                case ShowResult.Finished:
                    Debug.Log("Video completed. Offer a reward to the player.");
                    break;
                case ShowResult.Skipped:
                    Debug.LogWarning("Video was skipped.");
                    break;
                case ShowResult.Failed:
                    Debug.LogError("Video failed to show." + result);
                    break;
            }
        }
    }
}

