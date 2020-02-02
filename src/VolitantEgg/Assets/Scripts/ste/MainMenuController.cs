using System;
using UnityEngine;
using UnityEngine.UI;
using kazgame.ui;
using UnityEngine.SocialPlatforms;
using kazgame.utils;


namespace kazgame.ste
{
    public class MainMenuController : MonoBase
    {
        public Button buttonStart;
        public Button buttonHowToPlay;
        public Button buttonSetting;
        public Button buttonLeadBoard;
        public Button buttonCredits;
        public Button buttonStore;

        public Slider soundSlider;

        //setting
        public Transform menuSetting;
        public Button btnSettingOK;
        public Toggle toggleMusic;
        public Toggle toggleSound;

        public MenuCreditsController menuCreditsController;
        public MenuStoreController menuStoreController;


        protected override void DoInit()
        {
            base.DoInit();

            buttonStart.onClick.AddListener(OnStartClick);
            buttonHowToPlay.onClick.AddListener(OnHowToPlayClick);

            buttonLeadBoard.onClick.AddListener(OnLeaderBoardClick);

            soundSlider.onValueChanged.AddListener(OnSoundValueChange);

            //setting
            buttonSetting.onClick.AddListener(OnSettingClick);
            btnSettingOK.onClick.AddListener(OnSettingOKClick);
            toggleMusic.onValueChanged.AddListener(OnMusicMuteableValueChange);
            toggleSound.onValueChanged.AddListener(OnSoundMutableValueChange);

            //credits
            buttonCredits.onClick.AddListener(ShowCompleteCredits);
            menuCreditsController.onClose += OnCreditsClose;

            menuStoreController.onClose += OnStoreClose;
            buttonStore.onClick.AddListener(OnStoreClick);
        }

        void Awake()
        {
            InitSelf();
        }

        void Start()
        {
            Reset();
        }

        void OnStoreClick()
        {
            PlayClickEffect();
            menuStoreController.ShowMenu();
        }

        void OnStoreClose()
        {
            PlayClickEffect();
            GameController.GetSingleton().maskLayer.Unmask();
        }

        private void PlayClickEffect()
        {
            GameController.GetSingleton().audioController.PlayClick();
        }

        public void Reset()
        {
            GameController.GetSingleton().maskLayer.Unmask(true);
            menuStoreController.HideMenu();
            //menuSetting.gameObject.SetActive (false);
            //menuCreditsController.HideMenu ();
        }

        void OnSoundValueChange(float value)
        {
            GameController.GetSingleton().audioController.volume = value;
            PlayClickEffect();
        }

        void OnSoundMutableValueChange(bool check)
        {
            GameController.GetSingleton().audioController.muteSound = !check;
        }

        void OnMusicMuteableValueChange(bool check)
        {
            GameController.GetSingleton().audioController.muteMusic = !check;
        }


        void OnSettingClick()
        {
            PlayClickEffect();
            menuSetting.gameObject.SetActive(true);
            GameController.GetSingleton().maskLayer.Show(menuSetting.transform);

            toggleMusic.isOn = !GameController.GetSingleton().audioController.muteMusic;
            toggleSound.isOn = !GameController.GetSingleton().audioController.muteSound;
            soundSlider.value = GameController.GetSingleton().audioController.volume;
        }

        void OnSettingOKClick()
        {
            PlayClickEffect();
            GameController.GetSingleton().maskLayer.Unmask();
            menuSetting.gameObject.SetActive(false);
        }

        void OnLeaderBoardClick()
        {
            PlayClickEffect();
            //			Mask ();
            SocialManager.ShowLeaderboardUI();
        }

        void OnCreditsClose()
        {
            PlayClickEffect();
            GameController.GetSingleton().maskLayer.Unmask();
        }

        void OnArchievementClick()
        {
            PlayClickEffect();
        }

        void OnStartClick()
        {
            PlayClickEffect();
            StopAndHide();
            GameController.GetSingleton().levelsController.StopShow();
            // :加载上次
            float lasty = PlayerPrefs.GetFloat(Constants.prefkeyGameLastPointY, default(float));
            Log.Info("GET LAST ---- {0}", lasty);
            if (lasty != default(float))
            {

                GameController.GetSingleton().levelsController.StartShow(true, new Vector3(0, lasty, 0));
            }
            else
            {
                GameController.GetSingleton().levelsController.StartShow();
            }

        }

        void OnHowToPlayClick()
        {
            PlayClickEffect();
            Log.Info("how to play clicked");
            Handheld.PlayFullScreenMovie("tutorial.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput);
            Log.Info("how to play played");
        }

        public void Show()
        {
            Reset();
            gameObject.SetActive(true);
            GameController.GetSingleton().audioController.PlayMainMenuMusic();
        }

        public void StopAndHide()
        {
            GameController gameController = GameController.GetSingleton();
            //			Log.Debug (">>{0}",gameController);
            gameController.audioController.StopPlayMusic();
            gameObject.SetActive(false);
            Reset();
        }

        private void EnableMainLevel(bool enable)
        {
            buttonStart.enabled = enable;
            buttonSetting.enabled = enable;
            buttonLeadBoard.enabled = enable;
            buttonStore.enabled = enable;
        }

        public void ShowCompleteCredits()
        {
            PlayClickEffect();
            menuCreditsController.gameObject.SetActive(true);
            GameController.GetSingleton().maskLayer.Show(menuCreditsController.transform);
            TextAsset str = Resources.Load("Text/credits") as TextAsset;
            menuCreditsController.maskTextEffect.ShowText(str.text);
        }
    }
}

