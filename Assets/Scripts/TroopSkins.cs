using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//save all the troop skins accessed by the soldier class here
public class TroopSkins : MonoBehaviour {
    public Sprite legsSoviet, leftArmSoviet, rightArmSoviet, bodySoviet;
    public Sprite legsUS, leftArmUS, rightArmUS, bodyUS;
    public Sprite legsUK, leftArmUK, rightArmUK, bodyUK;
    public Sprite bodyJapan;

    public static TroopSkins instance;
    void Start() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
}