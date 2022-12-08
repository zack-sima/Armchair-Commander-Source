using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//NOTE: THIS IS THE NEW POPUP TO REPLACE INGAMEPOPUP, USE INHERITANCE
public class Popup : MonoBehaviour {
    //texts that can be accessed (so getcomponent doesn't need to be used)
    public List<Text> texts;

    public void ToScene(int index) {
        SceneManager.LoadScene(index);
    }
    public void Dismiss() {
        Destroy(gameObject);
    }
}
