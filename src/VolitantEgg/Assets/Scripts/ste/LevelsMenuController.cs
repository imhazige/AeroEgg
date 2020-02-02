using System;
using UnityEngine;
using UnityEngine.UI;
using kazgame.utils;
using kazgame.ui;
using System.Collections;

namespace kazgame.ste
{
    public class LevelsMenuController : MonoBase
    {
        //menu
        public Button btnPause;
        public Text tipSpeed;
        public Image speedimg;
        public UITextIntEffect tipHeight;
        public InkLengthUI uiLineInk;
        public UIMaskableColorFlashEffect tipStatus;

        //pause menu
        public Button btnPauseResume;
        public Button btnPauseMainMenu;
        public RectTransform menuPause;

        //pause menu go main menu confirm menu
        public Button btnPauseConfirmYes;
        public Button btnPauseConfirmCancel;
        public RectTransform menuPauseConfirm;

        //game over menu
        public RectTransform menuGameOver;
        public Button btnGameOverMainMenu;
        public Button btnGameOverTryAgain;
        public Button btnResetStart;
        public UITextIntEffect uiTextScore;
        public Text uiTextNewRecord;

        private const float alphaMask = 0.2f;

        protected override void DoInit()
        {
            base.DoInit();

            btnPause.onClick.AddListener(OnPauseClick);

            btnPauseResume.onClick.AddListener(OnPauseResumeClick);
            btnPauseMainMenu.onClick.AddListener(OnGameOverMainMenuClick);


            btnPauseConfirmYes.onClick.AddListener(OnPauseConfirmYesClick);
            btnPauseConfirmCancel.onClick.AddListener(OnPauseConfirmCancelClick);

            btnGameOverMainMenu.onClick.AddListener(OnGameOverMainMenuClick);
            btnGameOverTryAgain.onClick.AddListener(OnGameOverTryAgainClick);
            btnResetStart.onClick.AddListener(OnGameOverResetStartClick);
        }


        void Awake()
        {
            InitSelf();
        }

        void OnGameOverMainMenuClick()
        {
            PlayClickSound();
            GameController.GetSingleton().maskLayer.Unmask(true);
            GameController.GetSingleton().levelsController.StopShow();
            GameController.GetSingleton().mainMenuController.Show();
        }

        void OnGameOverTryAgainClick()
        {
            PlayClickSound();
            GameController.GetSingleton().maskLayer.Unmask();
            GameController.GetSingleton().levelsController.StopShow();
            GameController.GetSingleton().levelsController.StartShow(true);
        }

        void OnGameOverResetStartClick()
        {
            //重新开始的确认提示

            OnPauseMainMenuClick();

            // PlayClickSound();
            // GameController.GetSingleton().maskLayer.Unmask();
            // GameController.GetSingleton().levelsController.StopShow();
            // GameController.GetSingleton().levelsController.StartShow(true);
        }

        void OnPauseConfirmYesClick()
        {
            //重置
            PlayerPrefs.DeleteKey(Constants.prefkeyGameLastPointY);
            OnGameOverMainMenuClick();
        }

        void OnPauseConfirmCancelClick()
        {
            PlayClickSound();
            GameController.GetSingleton().maskLayer.Unmask();
            menuPauseConfirm.gameObject.SetActive(false);
        }

        void OnPauseResumeClick()
        {
            PlayClickSound();
            GameController.GetSingleton().levelsController.ResumeGame();
            GameController.GetSingleton().maskLayer.Unmask();
            menuPause.gameObject.SetActive(false);
        }

        public void ShowPauseMenu()
        {
            //			Log.Debug ("ShowPauseMenu--------");
            GameController.GetSingleton().levelsController.PauseGame();
            //			GameController.GetSingleton ().maskLayer.Unmask ();
            GameController.GetSingleton().maskLayer.Show(menuPause.transform, alphaMask);
            menuPause.gameObject.SetActive(true);
        }

        void OnPauseMainMenuClick()
        {
            //			Log.Debug ("on main menu clicked.");
            PlayClickSound();
            GameController.GetSingleton().maskLayer.Show(menuPauseConfirm.transform, alphaMask);
            menuPauseConfirm.gameObject.SetActive(true);
        }

        void OnPauseClick()
        {
            if (GameController.singleton.levelsController.gameover)
            {
                return;
            }
            //			Log.Debug ("on pause click...");
            PlayClickSound();
            ShowPauseMenu();
        }

        public void EnableAll(bool enable)
        {
            btnPause.enabled = enable;
        }

        IEnumerator ScoreFnAfterShown()
        {
            yield return new WaitForSeconds(0.5f);

            ScoreController scoreController = GameController.GetSingleton().levelsController.scoreController;
            int score = scoreController.score;
            GameController.GetSingleton().levelsController.scoreController.ReportScore(score);
            int lastScore = PlayerPrefs.GetInt(Constants.prefkeyLastScore, 0);
            //score
            uiTextScore.Reset();
            uiTextScore.ChangeValue(score);
            //check new record
            if (lastScore < score)
            {
                uiTextNewRecord.gameObject.SetActive(true);
                PlayerPrefs.SetInt(Constants.prefkeyLastScore, score);
            }

        }

        private void PlayClickSound()
        {
            GameController.singleton.audioController.PlayClick();
        }

        public void ShowGameOverMenu()
        {
            GameController.GetSingleton().maskLayer.Show(menuGameOver.transform);
            menuGameOver.gameObject.SetActive(true);
            StartCoroutine(ScoreFnAfterShown());
        }

        public void StartShow()
        {
            gameObject.SetActive(true);
            InitSelf();
            Reset();
        }

        public void StopShow()
        {
            gameObject.SetActive(false);
        }

        public void ShowStatusTip(string text)
        {
            Text ctxt = tipStatus.gameObject.GetComponent<Text>();
            if (ctxt.text == text && tipStatus.gameObject.activeSelf)
            {
                //				Log.Debug ("text are the same...");
                return;
            }
            tipStatus.gameObject.SetActive(true);
            tipStatus.StartFlash();

            ctxt.text = text;
        }

        public void HideStatusTip()
        {
            tipStatus.StopFlash();
            tipStatus.gameObject.SetActive(false);
            Text ctxt = tipStatus.gameObject.GetComponent<Text>();
            ctxt.text = "";
        }

        public void Reset()
        {
            EnableAll(true);
            menuPause.gameObject.SetActive(false);
            menuPauseConfirm.gameObject.SetActive(false);
            menuGameOver.gameObject.SetActive(false);
            uiTextNewRecord.gameObject.SetActive(false);

            HideStatusTip();

            tipHeight.Reset();
            tipSpeed.text = "0";
        }

    }
}

