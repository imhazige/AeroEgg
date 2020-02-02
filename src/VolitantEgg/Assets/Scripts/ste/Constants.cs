using System;
using UnityEngine;
using kazgame.utils;

namespace kazgame.ste
{
    public static class Constants
    {

        /// <summary>
        /// The speed when the egg crack
        /// </summary>
        //FIXME
        public const int crack_speed = 60;


        public const int size_poll_actor = 5;

        /// <summary>
        /// The o3 crash speed by the egg
        /// </summary>
        public const float o3_crash_speed = 30;

        public const float speed_cloud_move_max = 0.5f;
        public const float speed_butterfly_move_max = 1.5f;

        public const float egg_fly_length_unit_time = 2f;
        public const float egg_fly_speed_unit_time = 10;

        public static Color color_alert = ColorUtils.HexToColor("FF8604FF");

        public static Color color_go = Color.green;

        public const int showAdsUntilPlayed = -1;

        public const float musicMaxVolume = .09f;

        public const string prefkeySoundVolume = "sound.volume";

        public const string prefkeyMuteMusic = "music.mute";

        public const string prefkeyMuteSound = "sound.mute";

        public const string prefkeyLastScore = "score.last";

        public const string prefkeyGamePlayed = "game.played.count";

        public const string prefkeyGameLastPointY = "game.last.point.y";

        public const string prefkeyMainGameStarted = "game.main.played";

        public const string pref_completed_level = "pref_completed_level";

        //FIXME check when publish
        public const bool debug_model = false;

        public const bool debug_nofail = debug_model && false;

        public const bool debug_show_debug_tool = debug_model && true;

        public const int score_unit_combo = 30;

        public const int score_unit_caught = 50;

        public const float score_combo_time = 3f;

        public const string store_savetheegg_noads = "store_savetheegg_noads";

#if UNITY_IPHONE
        public const string gamecenterLeaderBoradScoreId = "kazgame_savetheegg_free_height";
#endif
    }
}

