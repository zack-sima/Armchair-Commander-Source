using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;


public class AdmobDebugTest : MonoBehaviour {
	//and ca-app-pub-9659065879138366~5264821859
	//ios ca-app-pub-9659065879138366~2060074010
	RewardedAdsButton controller = null;
	private static GameObject instance;
	public RewardedAd rewardedAd;

	public InterstitialAd interstitialAd;
	public bool initialized;

	void Start() {
		AdmobStart();

	}
	void Update() {
		//if (rewardedAd.IsLoaded())
		//    rewardedAd.Show();
	}
	public void AdmobStart() {
		initialized = true;
		Debug.Log("began initialization ads");
		MobileAds.Initialize(initCompleteAction => { InitComplete(); });
	}
	void InitComplete() {
#if UNITY_ANDROID
        string rewardedUnitId = "ca-app-pub-9659065879138366/5012706265";
        string interstitialUnitId = "ca-app-pub-9659065879138366/9577463775";
#elif UNITY_IOS
		string rewardedUnitId = "ca-app-pub-9659065879138366/7424097245";/**/  /*"ca-app-pub-3940256099942544/1712485313";/**/ //test placement right now
		string interstitialUnitId = "ca-app-pub-9659065879138366/5801870296";
#else
        string rewardedUnitId = "unexpected_platform";
        string interstitialUnitId = "unexpected_platform";
#endif

		// Initialize an InterstitialAd.
		//this.interstitialAd = new InterstitialAd(interstitialUnitId);
		//// Create an empty ad request.
		//AdRequest requestInterstitial = new AdRequest.Builder().Build();
		//// Load the interstitial with the request.
		//this.interstitialAd.LoadAd(requestInterstitial);

		//this.rewardedAd = new RewardedAd(rewardedUnitId);

		//// Called when an ad request has successfully loaded.
		//this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
		//// Called when an ad request failed to load.
		//this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
		//// Called when an ad is shown.
		//this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
		//// Called when an ad request failed to show.
		//this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
		//// Called when the user should be rewarded for interacting with the ad.
		//this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
		//// Called when the ad is closed.
		//this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

		// Create an empty ad request.
		// AdRequest request = new AdRequest.Builder().Build();
		// Load the rewarded ad with the request.
		//this.rewardedAd.LoadAd(request);
	}
	public void HandleRewardedAdLoaded(object sender, EventArgs args) {
		MonoBehaviour.print("HandleRewardedAdLoaded event received");
	}

	// public void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args) {
	// 	MonoBehaviour.print(
	// 		"HandleRewardedAdFailedToLoad event received with message: "
	// 						 + args.LoadAdError.GetMessage());
	// }

	public void HandleRewardedAdOpening(object sender, EventArgs args) {
		MonoBehaviour.print("HandleRewardedAdOpening event received");
	}

	// public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args) {
	// 	MonoBehaviour.print(
	// 		"HandleRewardedAdFailedToShow event received with message: "
	// 						 + args.Message);
	// }

	public void HandleRewardedAdClosed(object sender, EventArgs args) {
		MonoBehaviour.print("HandleRewardedAdClosed event received");
		controller.ClosedAd();
	}

	public void HandleUserEarnedReward(object sender, Reward args) {
		string type = args.Type;
		double amount = args.Amount;
		controller.RewardPlayer();
		MonoBehaviour.print(
			"HandleRewardedAdRewarded event received for "
						+ amount.ToString() + " " + type);
	}
}
