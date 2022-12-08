using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletExplosion : MonoBehaviour {
    float delay = 0.2f;

    public GameObject explosion;
    void Start() {
        StartCoroutine(MyUpdate());
    }

    // Update is called once per frame
    IEnumerator MyUpdate() {
        int spawned = 0;
        for (float i = 0; i < delay; i += Time.deltaTime) {
            if (spawned < i / delay * 10) {
                Instantiate(explosion, new Vector3(transform.position.x + UnityEngine.Random.Range(-0.5f, 0.5f), transform.position.y + UnityEngine.Random.Range(-0.5f, 0.5f), transform.position.z), Quaternion.identity);
                spawned++;
            }
            yield return null;
        }
        Destroy(gameObject); 
    }
}