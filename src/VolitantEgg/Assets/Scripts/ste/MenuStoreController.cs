using System;
using UnityEngine;
using UnityEngine.UI;
using kazgame.utils;

namespace kazgame.ste
{
	public class MenuStoreController : MonoBase
	{
		public delegate void OnClose();

		public OnClose onClose;

		public Button btnLeft;
		public Button btnRight;

		public Button btnBuyNoAds;
		public Button btnRestore;

		public Button btnClose;

		private MenuStoreItem[] items;

		public Text title;

		private int _showIndex;

		void Awake(){
			InitSelf ();
		}


		protected override void DoInit ()
		{
			base.DoInit ();

			Reset ();
		}

		void Reset(){
			_showIndex = 0;
			btnLeft.onClick.AddListener(OnLeftClick);
			btnRight.onClick.AddListener(OnRightClick);
			btnBuyNoAds.onClick.AddListener(OnBuyNoAdsClick);
			btnRestore.onClick.AddListener(OnRestoreClick);
			btnClose.onClick.AddListener(HideMenu);
			btnLeft.gameObject.SetActive (false);
			btnRight.gameObject.SetActive (true);
			items = GetComponentsInChildren<MenuStoreItem> ();
		}

		private void PlayClickEffect ()
		{
			GameController.GetSingleton ().audioController.PlayClick ();
		}

		void OnLeftClick ()
		{
			if (null == items){
				return;
			}
			if (0 == _showIndex) {
				return;
			}

			PlayClickEffect ();
			ShowItem (_showIndex-1);
		}

		void OnRightClick ()
		{
			if (null == items){
				return;
			}
			if (items.Length == _showIndex + 1) {
				return;
			}

			PlayClickEffect ();
			ShowItem (_showIndex+1);
		}

		void OnBuyNoAdsClick ()
		{
			GameController.GetSingleton().storeManager.BuyNoAds ();
		}

		void OnRestoreClick(){
			GameController.GetSingleton().storeManager.RestorePurchases ();
		}

		void ShowItem(int index){
			if (null == items) {
				return;
			}
			items [_showIndex].Show (false);
			MenuStoreItem item =  items [index];
			item.Show (true);
			_showIndex = index;
			title.text = item.title;

			btnLeft.gameObject.SetActive (_showIndex > 0);
			btnRight.gameObject.SetActive (_showIndex != items.Length - 1);
		}

		public void ShowMenu(){
			InitSelf ();
			if (null != items){
				foreach (MenuStoreItem item in items) {
					item.Show (false);
				}
			}
			gameObject.SetActive (true);
			ShowItem (0);
		}

		public void HideMenu(){
			gameObject.SetActive (false);
			if (null != onClose){
				onClose ();
			}
		}
	}
}

