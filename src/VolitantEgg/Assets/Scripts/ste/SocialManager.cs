using System;
using UnityEngine;
using kazgame.utils;
using UnityEngine.SocialPlatforms;

// #if UNITY_ANDROID
// using GooglePlayGames;
// using GooglePlayGames.BasicApi;
// #endif

#if UNITY_IPHONE
using UnityEngine.SocialPlatforms.GameCenter;
#endif
namespace kazgame.ste
{
    public static class SocialManager
    {
        private static bool scuecessed;

        public static void Connect()
        {
            //android
#if UNITY_IPHONE
			GameCenterPlatform.ShowDefaultAchievementCompletionBanner (true);
#endif

            // #if UNITY_ANDROID
            //             PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            //                 // enables saving game progress.
            //                 //				.EnableSavedGames()
            //                 // registers a callback to handle game invitations received while the game is not running.
            //                 //				.WithInvitationDelegate(<callback method>)
            //                 // registers a callback for turn based match notifications received while the
            //                 // game is not running.
            //                 //				.WithMatchDelegate(<callback method>)
            //                 // require access to a player's Google+ social graph (usually not needed)
            //                 //				.RequireGooglePlus()
            //                 .Build();

            //             PlayGamesPlatform.InitializeInstance(config);
            //             // recommended for debugging:
            //             PlayGamesPlatform.DebugLogEnabled = true;
            //             // Activate the Google Play Games platform
            //             PlayGamesPlatform.Activate();
            // #endif
            Social.localUser.Authenticate(success =>
            {
                scuecessed = success;
                if (scuecessed)
                {
                    //					ReportAchievement ();
                    Log.Debug("game center authenticate ok.");
                }
                else
                {
                    Debug.Log("game center authenticate fail.");
                }
            });
        }

        public static void ShowLeaderboardUI()
        {
            //how to do in android?
#if UNITY_IPHONE
			GameCenterPlatform.ShowLeaderboardUI (Constants.gamecenterLeaderBoradScoreId, TimeScope.AllTime);
#endif
            // #if UNITY_ANDROID
            //             ((PlayGamesPlatform)Social.Active).ShowLeaderboardUI(Constants.gamecenterLeaderBoradScoreId);
            // #endif
        }

        private static void ReportAchievement(string id, double rate)
        {
            Social.ReportProgress(id, rate, (result) => Log.Debug((result ? "Reported achievement" : "Failed to report achievement") + id));
        }

        public static void ReportInt(string id, int value, int max)
        {
            double rate = value >= max ? 100 : (value / max * 100);
            ReportAchievement(id, rate);
        }
    }
}

