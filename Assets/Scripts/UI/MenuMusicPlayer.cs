using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMusicPlayer : MonoBehaviour {
    public AudioSource audioSource;
    static MenuMusicPlayer instance;
    static bool startedGame = false;
    void Start() {
        if (!instance) {
            instance = this;
            DontDestroyOnLoad(gameObject);
            if (!startedGame) {
                StartCoroutine(PlayMusicDelayed(3));
                startedGame = true;
            } else {
                StartCoroutine(PlayMusicDelayed(13));
            }
            
        } else {
            Destroy(gameObject);
        }
    }
    IEnumerator PlayMusicDelayed(float delay) {
        for (float i = 0; i < delay; i += Time.deltaTime)
            yield return null;

        audioSource.Play();
    }
    void Update() {
        audioSource.volume = MyPlayerPrefs.instance.GetInt("mutedMusicUI") == 0 ? 0.2f : 0;
    }
}