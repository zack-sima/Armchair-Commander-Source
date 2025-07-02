using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;
#if UNITY_IOS
using Balaso;
#endif

public class AdMobInitializer : MonoBehaviour
{
	RewardedAdsButton controller = null;
	private static GameObject instance;
	public RewardedAd rewardedAd;
	public InterstitialAd interstitialAd;

	public bool initialized;

	private void AdmobStart()
	{
		initialized = true;
		Debug.Log("began initialization ads");

		if (Application.isMobilePlatform || Application.isEditor)
		{
			MobileAds.Initialize(initCompleteAction => { InitComplete(); });
		}
	}
#if UNITY_ANDROID
	readonly string rewardedUnitId = "unexpected_platform";
	readonly string interstitialUnitId = "unexpected_platform";
#elif UNITY_IOS
	readonly string rewardedUnitId = "unexpected_platform";
	readonly string interstitialUnitId = "unexpected_platform";
#else
	readonly string rewardedUnitId = "unexpected_platform";
	readonly string interstitialUnitId = "unexpected_platform";
#endif
	public void ShowRewardedAd()
	{
		if (rewardedAd != null && rewardedAd.CanShowAd())
		{
			rewardedAd.Show((Reward reward) =>
			{
				//Reward the user.
				controller.RewardPlayer();
				UnityEngine.SceneManagement.SceneManager.LoadScene(0);
			});
		}
	}
	public void ShowInterstitialAd()
	{
		if (interstitialAd != null && interstitialAd.CanShowAd())
		{
			MyPlayerPrefs.instance.SetInt("playAdsTimer", UnityEngine.Random.Range(0, 2));
			MyPlayerPrefs.instance.SaveData();
			Debug.Log("Showing interstitial ad.");
			interstitialAd.Show();
		}
		else
		{
			Debug.LogError("Interstitial ad is not ready yet.");
		}
	}
	private void RegisterReloadHandler(RewardedAd ad)
	{
		// Raised when the ad closed full screen content.
		ad.OnAdFullScreenContentClosed += () =>
		{
			Debug.Log("Rewarded Ad full screen content closed.");

			// Reload the ad so that we can show another as soon as possible.
			LoadRewardedAd();
		};
		// Raised when the ad failed to open full screen content.
		ad.OnAdFullScreenContentFailed += (AdError error) =>
		{
			Debug.LogError("Rewarded ad failed to open full screen content " +
						   "with error : " + error);

			// Reload the ad so that we can show another as soon as possible.
			LoadRewardedAd();
		};
	}
	private void RegisterReloadHandler(InterstitialAd ad)
	{
		// Raised when the ad closed full screen content.
		ad.OnAdFullScreenContentClosed += () =>
		{
			Debug.Log("Interstitial Ad full screen content closed.");

			// Reload the ad so that we can show another as soon as possible.
			LoadInterstitialAd();
		};
		// Raised when the ad failed to open full screen content.
		ad.OnAdFullScreenContentFailed += (AdError error) =>
		{
			Debug.LogError("Interstitial ad failed to open full screen content " +
						   "with error : " + error);

			// Reload the ad so that we can show another as soon as possible.
			LoadInterstitialAd();
		};
	}
	public void LoadInterstitialAd()
	{
		// Clean up the old ad before loading a new one.
		if (interstitialAd != null)
		{
			interstitialAd.Destroy();
			interstitialAd = null;
		}

		Debug.Log("Loading the interstitial ad.");

		// create our request used to load the ad.
		var adRequest = new AdRequest();
		adRequest.Keywords.Add("unity-admob-sample");

		// send the request to load the ad.
		InterstitialAd.Load(interstitialUnitId, adRequest, (InterstitialAd ad, LoadAdError error) =>
		{
			// if error is not null, the load request failed.
			if (error != null || ad == null)
			{
				Debug.LogWarning("interstitial ad failed to load an ad " + "with error : " + error);
				return;
			}

			Debug.Log("Interstitial ad loaded with response : " + ad.GetResponseInfo());

			interstitialAd = ad;
			RegisterReloadHandler(interstitialAd);
		});
	}
	public void LoadRewardedAd()
	{
		// Clean up the old ad before loading a new one.
		if (rewardedAd != null)
		{
			rewardedAd.Destroy();
			rewardedAd = null;
		}

		Debug.Log("Loading the rewarded ad.");

		// create our request used to load the ad.
		var adRequest = new AdRequest();
		adRequest.Keywords.Add("unity-admob-sample");

		// send the request to load the ad.
		RewardedAd.Load(rewardedUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
		{
			// if error is not null, the load request failed.
			if (error != null || ad == null)
			{
				Debug.LogError("Rewarded ad failed to load an ad " + "with error : " + error);
				return;
			}
			Debug.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());

			rewardedAd = ad;
			Debug.Log($"[LOAD CALLBACK] new RewardedAd created: canShowAd={rewardedAd.CanShowAd()}");
			RegisterReloadHandler(rewardedAd);
		});
	}
	void InitComplete()
	{
		LoadInterstitialAd();
		LoadRewardedAd();
	}

	//public void HandleUserEarnedReward(object sender, Reward args) {
	//	string type = args.Type;
	//	double amount = args.Amount;
	//	controller.RewardPlayer();
	//	MonoBehaviour.print("HandleRewardedAdRewarded event received for "
	//		+ amount.ToString() + " " + type);
	//}
	//static so it can be easily referred
	//public static IPurchaseListener plistener;
	void Start()
	{
		DontDestroyOnLoad(gameObject);
		if (instance == null)
		{
			instance = gameObject;
			AdmobStart();
		}
		else
		{
			Destroy(gameObject);
		}
		controller = GameObject.Find("RewardedVid").GetComponent<RewardedAdsButton>();

#if UNITY_IOS
		AppTrackingTransparency.OnAuthorizationRequestDone += OnAuthorizationRequestDone;

		AppTrackingTransparency.AuthorizationStatus currentStatus = AppTrackingTransparency.TrackingAuthorizationStatus;
		Debug.Log(string.Format("Current authorization status: {0}", currentStatus.ToString()));
		if (currentStatus != AppTrackingTransparency.AuthorizationStatus.AUTHORIZED) {
			Debug.Log("Requesting authorization...");
			AppTrackingTransparency.RequestTrackingAuthorization();
		} else {
			Debug.Log("Already authorized");
		}
#endif
	}

	void Awake()
	{
#if UNITY_IOS
		AppTrackingTransparency.RegisterAppForAdNetworkAttribution();
		AppTrackingTransparency.UpdateConversionValue(3);
#endif
	}
#if UNITY_IOS

	/// <summary>
	/// Callback invoked with the user's decision
	/// </summary>
	/// <param name="status"></param>
	private void OnAuthorizationRequestDone(AppTrackingTransparency.AuthorizationStatus status) {
		switch (status) {
			case AppTrackingTransparency.AuthorizationStatus.NOT_DETERMINED:
				Debug.Log("AuthorizationStatus: NOT_DETERMINED");
				break;
			case AppTrackingTransparency.AuthorizationStatus.RESTRICTED:
				Debug.Log("AuthorizationStatus: RESTRICTED");
				break;
			case AppTrackingTransparency.AuthorizationStatus.DENIED:
				Debug.Log("AuthorizationStatus: DENIED");
				break;
			case AppTrackingTransparency.AuthorizationStatus.AUTHORIZED:
				Debug.Log("AuthorizationStatus: AUTHORIZED");
				break;
		}

		// Obtain IDFA
		Debug.Log($"IDFA: {AppTrackingTransparency.IdentifierForAdvertising()}");
	}
#endif
}
