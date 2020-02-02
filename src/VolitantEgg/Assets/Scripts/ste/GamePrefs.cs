using System;
using kazgame.security;

namespace kazgame.ste
{
    public class GamePrefs
    {
        private const string secrityKey = "somekey";
        public readonly LocalPalyerPrefs localPalyerPrefs;
        private static readonly GamePrefs _instance = new GamePrefs();

        public GamePrefs()
        {
            localPalyerPrefs = new LocalPalyerPrefs(secrityKey);

            //check the des function work?
            //			Log.Info("test1="+localPalyerPrefs.GetString("test1"));
            //			localPalyerPrefs.SetString("test1","test1v");
            //			Log.Info("test2="+localPalyerPrefs.GetInt("test2"));
            //			localPalyerPrefs.SetInt("test2",1);
            //			Log.Info("test3="+localPalyerPrefs.GetFloat("test3"));
            //			localPalyerPrefs.SetFloat("test3",2.3f);
        }

        public static GamePrefs GetSingleton()
        {
            return _instance;
        }
    }
}

