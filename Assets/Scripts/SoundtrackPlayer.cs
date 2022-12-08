using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundtrackPlayer : MonoBehaviour {
    public Controller controller;
    public AudioSource[] alliedMusic;
    public AudioSource[] axisMusic;

    //TODO: Add UK music, Chinese music, US music

    void Start() {
        
    }
    AudioSource currentSound;
    int deltaPieceIndex = 0;
    float changeMusicTimer = 10f;
    float currentSoundOriginalVolume = 1f;
    
    void Update() {
        changeMusicTimer -= Time.deltaTime;

        if (currentSound != null)
            currentSound.volume = currentSoundOriginalVolume * MyPlayerPrefs.instance.GetFloat("musics");
        if (changeMusicTimer <= 0f && !controller.editMode) {
            if (currentSound != null)
                currentSound.volume = currentSoundOriginalVolume;
            //all custom alliances will just play the allied soundtrack
            //TODO: make custom easter egg soundtrack for custom alliances?
            int randSnd = Random.Range(0, controller.playerIsAxis == 1 ? axisMusic.Length: alliedMusic.Length);
            while (randSnd == deltaPieceIndex) {
                randSnd = Random.Range(0, controller.playerIsAxis == 1 ? axisMusic.Length : alliedMusic.Length);
            }

            deltaPieceIndex = randSnd;
            if (controller.playerIsAxis == 1) {
                changeMusicTimer = axisMusic[randSnd].clip.length + 2f;
//                print(changeMusicTimer);

                //axisMusic[randSnd].PlayOneShot(axisMusic[randSnd].clip, MyPlayerPrefs.instance.GetFloat("musics"));
                axisMusic[randSnd].Play();
                currentSound = axisMusic[randSnd];
            } else {
                changeMusicTimer = alliedMusic[randSnd].clip.length + 2f;
                currentSound = alliedMusic[randSnd];
                alliedMusic[randSnd].Play();
                //alliedMusic[randSnd].PlayOneShot(alliedMusic[randSnd].clip, MyPlayerPrefs.instance.GetFloat("musics"));
            }
            currentSoundOriginalVolume = currentSound.volume;
        }
    }
}