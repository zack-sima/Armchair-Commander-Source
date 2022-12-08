using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDestruction : MonoBehaviour {
    public float delay; 
    // Start is called before the first frame update 
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        delay -= Time.deltaTime;
        if (delay <= 0f)
            Destroy(gameObject); 
    }
}
