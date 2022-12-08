using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportCarrierAnimator : MonoBehaviour {
    Controller controller;
    public SpriteRenderer muzzle; 
    void Start() {
        controller = GameObject.Find("Controller").GetComponent<Controller>();
    }

    public IEnumerator shootSmg(int iteration, float delay) {
        for (float i = 0f; i < delay; i += Time.deltaTime)
            yield return null; 
        muzzle.enabled = true;
        if (GetComponent<AudioSource>() != null)
            GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip, MyPlayerPrefs.instance.GetFloat("sounds") * GetComponent<AudioSource>().volume);
        for (float i = 0; i < 0.18f; i += Time.deltaTime) {
            if (i > 0.09f && muzzle.enabled) {
                muzzle.enabled = false;
                
                if (GetComponent<AudioSource>() != null)
                    GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip, MyPlayerPrefs.instance.GetFloat("sounds") * GetComponent<AudioSource>().volume);
            }
            yield return null; 
        }

        if (iteration < 3.4) {
            StartCoroutine(shootSmg(iteration + 1, 0f)); 
        } else {
            for (float i = 0; i < 0.07f; i +=Time.deltaTime)
                yield return null; 

            if (GetComponent<AudioSource>() != null)
                transform.GetChild(0).GetComponent<AudioSource>().PlayOneShot(transform.GetChild(0).GetComponent<AudioSource>().clip, MyPlayerPrefs.instance.GetFloat("sounds") * transform.GetChild(0).GetComponent<AudioSource>().volume);

        }
    }
     
    void Update() {
    }
}

