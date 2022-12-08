using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID
using Google.Play.Review;
#endif
#if UNITY_IOS
using UnityEngine.iOS;
#endif
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using NotificationSamples;
using System;

public class MenuManager : MonoBehaviour {
    public GameObject startPopupPrefab, editorPopupPrefab, tutorialPrefab;

    public GameObject termsPopupPrefab;
    MyPlayerPrefs myPlayerPrefs;

    public GameObject tutorialChooserPrefab;

    public Image background, socialMediaDot;
    public Sprite[] splashSprites;
    public Button loadButton, disableAdsButton;
    
    public GameNotificationsManager manager;
    public void ShowNotificationAfterDelay(int sec) {
        //add some tips?
        string[] notificationsTitlesEnglish = new string[] {
            "Try some new maps!",
            "Create your own maps!",
            "Check out new features!"
        };
        string[] notificationsBodiesEnglish = new string[] {
            "Community highlights are constantly changing!",
            "Try using events and diplomacy!",
            "Try customizing your own country!"
        };
        string[] notificationsTitlesChinese = new string[] {
            "来尝试一些新玩法吧！",
            "尝试一下做个地图？",
            "爽一局二战？"
        };
        string[] notificationsBodiesChinese = new string[] {
            "社区地图有很多新内容",
            "试试自定义国旗和国家",
            "玩一下地图吧！"
        };
        int chineseRandomFactor = UnityEngine.Random.Range(0, notificationsBodiesChinese.Length);
        int englishRandomFactor = UnityEngine.Random.Range(0, notificationsBodiesEnglish.Length);
        
        ShowNotificationAfterDelay(myPlayerPrefs.GetString("language") == "Chinese" ? notificationsTitlesChinese[chineseRandomFactor] : notificationsTitlesEnglish[englishRandomFactor], myPlayerPrefs.GetString("language") == "Chinese" ? notificationsBodiesChinese[chineseRandomFactor] : notificationsBodiesEnglish[englishRandomFactor], DateTime.Now.AddSeconds(sec));
    }
    public void GoToCommunity() {
        if (myPlayerPrefs.GetInt("agreeToPolicy") == 0) {
            Transform insItem = Instantiate(termsPopupPrefab, GameObject.Find("Canvas").transform).transform;
            insItem.position = new Vector2(Screen.width / 2, Screen.height / 2);
            insItem.GetComponent<IngamePopup>().menuMasta = this;
        } else {
            SceneManager.LoadScene(4);

        }
    }
    //will implement new multiplayer things soon; right now this goes straight to a mission and sets multiplayer to true
    public void GoToMultiplayer(int map) { //implement custom map json later
        myPlayerPrefs.SetInt("multiplayer", 1);
        if (myPlayerPrefs.GetString("multiplayerMap") == "") {
            myPlayerPrefs.SetInt("custom", 0);
        } else {
            myPlayerPrefs.SetInt("custom", 1);
        }
        myPlayerPrefs.SetInt("editor", 0);
        myPlayerPrefs.SetInt("saved", 0);
        myPlayerPrefs.SetInt("reward", 25);
        myPlayerPrefs.SetInt("level", map);
        myPlayerPrefs.SetString("country", "");
        myPlayerPrefs.SaveData();
        SceneManager.LoadScene(1);
    }
    public void SpawnTutorials() {
        Transform insItem = Instantiate(tutorialChooserPrefab, GameObject.Find("Canvas").transform).transform;
        insItem.position = new Vector2(Screen.width / 2, Screen.height / 2);
        insItem.GetComponent<IngamePopup>().menuMasta = this;

        myPlayerPrefs.SetInt("clickedMedia", 1);
        myPlayerPrefs.SaveData();
    }

    public void GoToTutorial(int tutorial) {
        myPlayerPrefs.SetInt("tutorial", tutorial + 1);
        myPlayerPrefs.SetInt("custom", 0);
        myPlayerPrefs.SetInt("editor", 0);
        myPlayerPrefs.SetInt("saved", 0);
        if (tutorial == 0) {
            myPlayerPrefs.SetInt("level", 0);
            myPlayerPrefs.SetInt("reward", BinaryPlayerSave.LoadData().completedLevels.Contains("Spanish Civil War") ? 10 : 250);
        } else {
            myPlayerPrefs.SetInt("level", 99);
            myPlayerPrefs.SetInt("reward", BinaryPlayerSave.LoadData().completedLevels.Contains("Advanced Tutorial") ? 10 : 250);
        }
        
        myPlayerPrefs.SetString("country", "");
        myPlayerPrefs.SaveData();

        SceneManager.LoadScene(1);
    }
    public void ShowNotificationAfterDelay(string title, string body, DateTime time) {
        IGameNotification createNotification = manager.CreateNotification();
        if (createNotification != null) {
            createNotification.Title = title;
            createNotification.Body = body;
            createNotification.DeliveryTime = time;

            var notificationToDisplay = manager.ScheduleNotification(createNotification);
            //notificationToDisplay.Reschedule = true; //to schedule here
        }
    }
    public void DisableAds() {
        if (!PlayerData.instance.playerData.removedAds && PlayerData.instance.playerData.money >= 1000) {
            PlayerData.instance.playerData.removedAds = true;
            PlayerData.instance.playerData.money -= 1000;
            PlayerData.instance.saveFile();
            Destroy(disableAdsButton.gameObject);
        }
    }
    public void ScheduleNotification() {
        var channel = new GameNotificationChannel("channel_1", "Default Game Channel", "Generic notifications");
        if (!manager.Initialized)
            manager.Initialize(channel);
        manager.CancelAllNotifications();
        manager.DismissAllNotifications();

        MyPlayerPrefs.instance.SetInt("notifications_scheduled", MyPlayerPrefs.instance.GetInt("notifications_scheduled") + 1);
        if (MyPlayerPrefs.instance.GetInt("notifications_scheduled") > 1) {
            ShowNotificationAfterDelay(UnityEngine.Random.Range(2000000, 3500000));
        } else {
            ShowNotificationAfterDelay(UnityEngine.Random.Range(200000, 350000));
        }
        myPlayerPrefs.SaveData();

    }

    IEnumerator LateStart() {
        yield return null;
        if (myPlayerPrefs.GetInt("disableAds") == 1 && !PlayerData.instance.playerData.removedAds) {
            PlayerData.instance.playerData.removedAds = true;
            PlayerData.instance.saveFile();
        }
        if (PlayerData.instance.playerData.removedAds && disableAdsButton != null) {
            Destroy(disableAdsButton.gameObject);
        } else {
            if (disableAdsButton != null)
                disableAdsButton.gameObject.SetActive(true);
        }
    }
    public void Start() {

        Application.targetFrameRate = 30;
        
#if !UNITY_IOS && !UNITY_ANDROID && !UNITY_EDITOR
            Destroy(disableAdsButton.gameObject);
#endif

        myPlayerPrefs = MyPlayerPrefs.instance;
        myPlayerPrefs.SetString("multiplayerMap", "");
        myPlayerPrefs.SetInt("isHost", 0);

        StartCoroutine(LateStart());

        if (myPlayerPrefs.GetInt("clickedMedia") == 0) {
            socialMediaDot.enabled = true;
        }
        
        if (myPlayerPrefs.GetInt("dontChangeBackground") == 1) {
            myPlayerPrefs.SetInt("dontChangeBackground", 0);
            background.sprite = splashSprites[myPlayerPrefs.GetInt("previousBackground")];
        } else {
            try {
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
        ScheduleNotification();
#endif
            } catch (Exception e) {
                print(e);
            }
            int b = 0;
            int iter = 0;
            do {
                b = UnityEngine.Random.Range(0, splashSprites.Length);
                if (iter > 100) {
                    print("wtf");
                    break;
                }
            } while (b == myPlayerPrefs.GetInt("previousBackground") && splashSprites.Length > 2); //last condition to prevent crashing with empty
            background.sprite = splashSprites[b];
            myPlayerPrefs.SetInt("previousBackground", b);
        }



        myPlayerPrefs.SetInt("tutorial", 0);
        myPlayerPrefs.SetInt("multiplayer", 0);
        
        if (myPlayerPrefs.GetInt("finishedTutorial") < 2) {
            print("asktutorial");
            Transform insItem = Instantiate(tutorialPrefab, GameObject.Find("Canvas").transform).transform;
            insItem.localPosition = Vector3.zero;
            insItem.GetComponent<IngamePopup>().menuMasta = this;
            insItem.GetComponent<IngamePopup>().tutorialNumber = myPlayerPrefs.GetInt("finishedTutorial");
        }

        if (myPlayerPrefs.GetInt("saved") == 0) {
            //print("unloadable");
            loadButton.interactable = false;
            loadButton.transform.GetChild(0).GetComponent<Text>().color = new Color(2f, 2f, 3f, 0.7f);
        }
        if (myPlayerPrefs.GetInt("reviewTime") == 10) {
            //NOTE: only ever ask for rating once now
            myPlayerPrefs.SetInt("reviewTime", 100);
            myPlayerPrefs.SaveData();
#if UNITY_ANDROID && !UNITY_EDITOR
            var reviewManager = new ReviewManager();

            // start preloading the review prompt in the background
            var playReviewInfoAsyncOperation = reviewManager.RequestReviewFlow();

            // define a callback after the preloading is done
            playReviewInfoAsyncOperation.Completed += playReviewInfoAsync => {
                if (playReviewInfoAsync.Error == ReviewErrorCode.NoError) {
                    // display the review prompt
                    var playReviewInfo = playReviewInfoAsync.GetResult();
                    reviewManager.LaunchReviewFlow(playReviewInfo);
                } else {
                    // handle error when loading review prompt
                }
            };
#elif UNITY_IOS && !UNITY_EDITOR
            Device.RequestStoreReview();
#endif
        }
    }
    public void startNewGame() {
        myPlayerPrefs.SetInt("reviewTime", myPlayerPrefs.GetInt("reviewTime") + 1);
        myPlayerPrefs.SaveData();
        SceneManager.LoadScene(2);
    }
    public void loadCustomGame() {
        Transform insItem = Instantiate(startPopupPrefab, GameObject.Find("Canvas").transform).transform;
        myPlayerPrefs.SetInt("reviewTime", myPlayerPrefs.GetInt("reviewTime") + 1);
        insItem.position = new Vector2(Screen.width / 2, Screen.height / 2);
    }
    public void loadScene(int index = 0) {
        myPlayerPrefs.SaveData();
        SceneManager.LoadScene(index);
    }
    public void loadGame() {
        myPlayerPrefs.SetInt("reviewTime", myPlayerPrefs.GetInt("reviewTime") + 1);
        myPlayerPrefs.SetInt("editor", 0);
        myPlayerPrefs.SetInt("saved", 1);
//        print("saved");

        myPlayerPrefs.SaveData();
        SceneManager.LoadScene(1);
    }
    public void loadMapEditor() {
        Transform insItem = Instantiate(editorPopupPrefab, GameObject.Find("Canvas").transform).transform;
        insItem.position = new Vector2(Screen.width / 2, Screen.height / 2);
    }
}