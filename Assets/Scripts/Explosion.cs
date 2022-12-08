using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {
    float originalDelay;
    public bool isMuzzle;
    public float delay;
    public Sprite[] explosionSprites; 
    void Start() {
        originalDelay = delay;
        if (isMuzzle) {
            transform.Translate(new Vector3(0.15f, 0.05f, 0f));
        }
    }

    // Update is called once per frame
    void Update() {
        delay -= Time.deltaTime;
        try {
            GetComponent<SpriteRenderer>().sprite = explosionSprites[Mathf.Min((int)((originalDelay - delay) / originalDelay * (explosionSprites.Length - 1f)), explosionSprites.Length - 1)];
        } catch (Exception e){
            print(e);
        }
        if (delay <= 0f)
            Destroy(gameObject); 
    }
}