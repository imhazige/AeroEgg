using System;
using UnityEngine;
using kazgame.ui;
using kazgame.utils;
using UnityEngine.UI;
using UnityEngine.Scripting;
using System.Collections;

namespace kazgame.ste
{
    public class ScoreController
    {
        private int _score;

        public void RecordScore(int score)
        {
            _score = score;
        }

        public void ReportScore(long score)
        {
            //			if (Application.internetReachability != NetworkReachability.ReachableViaLocalAreaNetwork) {
            //				Log.Info ("do not report score as it is not a LAN network.");
            //				return;
            //			}
            // Social.ReportScore (score,Constants.gamecenterLeaderBoradScoreId,HighScoreCheck);
        }

        static void HighScoreCheck(bool success)
        {
            Log.Debug("report score {0}", success);
        }

        public void Reset()
        {
            _score = 0;
        }

        public int score
        {
            get
            {
                return _score;
            }
        }

    }
}

