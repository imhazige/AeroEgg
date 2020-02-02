using System;
using UnityEngine;
using System.Collections;
using kazgame.utils;

namespace kazgame.ste
{
    public class AudioController : MonoBase, PauseAble
    {
        public AudioSource soundSource;
        public AudioSource speedAlertSource;
        public AudioSource musicSource;

        private float _curMusicLastTime;

        private string[] _playList;
        private int _curPlayIndex;

        private Coroutine _musicCor;

        private bool _pausing;

        void Awake()
        {
            InitSelf();
        }

        void Start()
        {
            float v = PlayerPrefs.GetFloat(Constants.prefkeySoundVolume, 1);
            volume = v;
            musicSource.volume = Constants.musicMaxVolume;
        }

        public bool muteSound
        {
            get
            {
                int muteSound = PlayerPrefs.GetInt(Constants.prefkeyMuteSound, 0);

                return muteSound > 0;
            }

            set
            {
                PlayerPrefs.SetInt(Constants.prefkeyMuteSound, value ? 1 : 0);
            }
        }

        #region PauseAble implementation

        public void OnPause()
        {
            musicSource.Pause();
            //			soundSource.Pause ();
            _pausing = true;
        }

        public void OnResume()
        {
            musicSource.UnPause();
            //			soundSource.UnPause ();
            _pausing = false;
        }

        #endregion

        public bool muteMusic
        {
            get
            {
                int muteSound = PlayerPrefs.GetInt(Constants.prefkeyMuteMusic, 0);

                bool mute = muteSound > 0;

                if (mute)
                {
                    StopPlayMusic();
                }

                return mute;
            }

            set
            {
                if (value)
                {
                    StopPlayMusic();
                }
                else
                {

                }
                PlayerPrefs.SetInt(Constants.prefkeyMuteMusic, value ? 1 : 0);
            }
        }

        public float volume
        {
            get
            {
                return AudioListener.volume;
            }
            set
            {
                AudioListener.volume = value;
                PlayerPrefs.SetFloat(Constants.prefkeySoundVolume, value);
            }
        }

        private AudioClip LoadAudio(string aname)
        {
            return Resources.Load("audio/" + aname) as AudioClip;
        }

        private void PlaySound(string aname)
        {
            if (muteSound)
            {
                return;
            }
            soundSource.PlayOneShot(LoadAudio(aname));
        }

        public void PlaySound(AudioClip clip)
        {
            if (muteSound)
            {
                return;
            }
            soundSource.PlayOneShot(clip);
        }

        private void PlayMusicNext()
        {
            if (null != _musicCor)
            {
                StopCoroutine(_musicCor);
            }
            if (muteMusic)
            {
                return;
            }
            _curPlayIndex = (_curPlayIndex + 1) % _playList.Length;
            string aname = _playList[_curPlayIndex];
            AudioClip clip = LoadAudio(aname);
            _curMusicLastTime = clip.length;
            musicSource.clip = clip;
            musicSource.Play();
            _musicCor = StartCoroutine(DoPlayCheck(_curMusicLastTime));

        }

        public void PlayMusic(string music)
        {
            PlayMusics(new string[] { music });
        }

        public void PlayMusics(string[] musics)
        {
            StopPlayMusic();
            if (muteMusic)
            {
                return;
            }
            if (null == musics || 0 == musics.Length)
            {
                return;
            }
            //			Log.Debug ("play " + musics);
            _curPlayIndex = -1;
            _playList = musics;
            PlayMusicNext();
        }

        public void PlayMainGameMusics()
        {
            string[] s = new string[] { "Carefree_Melody", "Locally_Sourced" };
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                //				Log.Debug ("========== reverse music..");
                Array.Reverse(s);
            }
            PlayMusics(s);
            //			PlayMusic ("Locally_Sourced");
        }

        public void PlayCountDown()
        {
            PlaySound("8-bit-countdown-ready");
        }

        public void PlayGo()
        {
            PlaySound("go");
        }

        public void PlayClick()
        {
            PlaySound("click");
        }

        public void PlaySpeedAlert()
        {
            if (muteSound)
            {
                return;
            }
            speedAlertSource.Stop();
            speedAlertSource.PlayOneShot(LoadAudio("cuckoo"));
        }

        public void StopSpeedAlert()
        {
            speedAlertSource.Stop();
        }

        public void PlayBirdFly()
        {
            PlaySound("pigeon-wings");
        }

        public void PlayGameOver()
        {
            PlaySound("gameover");
        }

        public void PlayLevelComplete()
        {
            PlaySound("cheer");
        }

        public void PlayMainMenuMusic()
        {
            PlayMusic("birds");
        }

        public void StopPlayMusic()
        {
            //			Log.Debug ("--------stop music");
            if (null != _musicCor)
            {
                StopCoroutine(_musicCor);
            }
            musicSource.Stop();
            _pausing = false;
            musicSource.UnPause();
        }

        public void StopPlaySound()
        {
            soundSource.Stop();
            speedAlertSource.Stop();
        }

        IEnumerator DoPlayCheck(float lastTime)
        {
            float playedTime = 0;

            const float waitsec = 5;
            while (playedTime < lastTime + 5)
            {
                if (!_pausing)
                {
                    playedTime += waitsec;
                    //					Log.Debug ("count.....");
                }
                else
                {
                    //					Log.Debug ("wait.....");
                }
                //				Log.Debug ("is  musicSource.isPlaying {0}", musicSource.isPlaying );
                yield return new WaitForSeconds(waitsec);
            }
            //			Log.Debug ("--------next music");

            PlayMusicNext();
        }
    }
}

