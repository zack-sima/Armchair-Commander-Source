using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TextFade : MonoBehaviour {
    float delay = 1.8f; 
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        transform.Translate(Vector3.up * Time.deltaTime * 0.7f);
        Text t = GetComponent<Text>();
        t.color = new Color(t.color.r, t.color.g, t.color.b, delay - 0.2f);
        GetComponent<Outline>().effectColor = new Color(0f, 0f, 0f, delay - 0.2f);

        delay -= Time.deltaTime; 
        if (delay <= 0f) {
            Destroy(gameObject);
        } 
    }
}
