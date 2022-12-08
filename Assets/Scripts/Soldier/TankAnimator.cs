using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankAnimator : MonoBehaviour {
    public Sprite sovietTurret, sovietHull, americanTurret, americanHull, japaneseTurret, japaneseHull, frenchTurret, frenchHull, englishTurret, englishHull;
    public Transform turret, hull; 
    public SpriteRenderer muzzle;
    public SpriteRenderer muzzle2;
    public bool isTurret;
    void Start() {
        updateLayering();
    }

    public IEnumerator FireRocketArtillery(Vector3 anchor, float rotationZ, GameObject rocketGameObj) {
        for (int i = 0; i < 3; i++) {
            GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip, MyPlayerPrefs.instance.GetFloat("sounds") * GetComponent<AudioSource>().volume);

            //spawn rocket animation

            Instantiate(rocketGameObj, anchor, Quaternion.Euler(0f, hull.eulerAngles.y, rotationZ));
            for (float j = 0f; j < 0.2f; j += Time.deltaTime) {
                
                yield return null;
            }
        }
        for (float i = 0f; i < 0.35f; i += Time.deltaTime) {
            yield return null;
        }
        AudioSource a = transform.GetChild(0).GetComponent<AudioSource>();
        a.PlayOneShot(a.clip, MyPlayerPrefs.instance.GetFloat("sounds") * 0.5f/* a.volume*/);
    }
    public IEnumerator fireArtillery() { 
        Vector3 originalPosition = transform.localPosition;
        Instantiate(Controller.instance.muzzleAnimationPrefab, muzzle.transform.position, muzzle.transform.rotation);
        //muzzle.enabled = true;
        GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip, MyPlayerPrefs.instance.GetFloat("sounds") * GetComponent<AudioSource>().volume);
        for (float i = 0f; i < 0.7f; i += Time.deltaTime) {
            if (i < 0.1f) {
                transform.Translate(Vector3.left * Time.deltaTime * 0.38f);
            } else {
                //muzzle.enabled = false;
                transform.Translate(Vector3.right * Time.deltaTime * 0.067f);
            }
            yield return null;
        }
        transform.localPosition = originalPosition;
        for (float i = 0f; i < 0.35f; i += Time.deltaTime) {
            yield return null;
        }
        transform.GetChild(0).GetComponent<AudioSource>().PlayOneShot(transform.GetChild(0).GetComponent<AudioSource>().clip, MyPlayerPrefs.instance.GetFloat("sounds") * transform.GetChild(0).GetComponent<AudioSource>().volume);
    }
    public IEnumerator fireShip(bool playSecondSound = true) {
        Vector3 originalPosition = transform.localPosition;
        Instantiate(Controller.instance.muzzleAnimationPrefab, muzzle.transform.position, muzzle.transform.rotation);

        //muzzle.enabled = true;
        GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip, MyPlayerPrefs.instance.GetFloat("sounds") * GetComponent<AudioSource>().volume);


        for (float i = 0f; i < 0.35f; i += Time.deltaTime) {
            if (i < 0.061f) {
                transform.Translate(Vector3.left * Time.deltaTime * 0.38f);
            } else {
                muzzle.enabled = false;
                transform.Translate(Vector3.right * Time.deltaTime * 0.067f);
            }
            yield return null;
        }
        transform.localPosition = originalPosition;
        for (float i = 0f; i < 0.57f; i += Time.deltaTime) {
            yield return null;
        }
        if (playSecondSound) {
            AudioSource a = transform.GetChild(0).GetComponent<AudioSource>();
            a.PlayOneShot(a.clip, MyPlayerPrefs.instance.GetFloat("sounds") * a.volume);
        }

    }
    public IEnumerator fireSub() {
        GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip, MyPlayerPrefs.instance.GetFloat("sounds") * GetComponent<AudioSource>().volume);
        for (float i = 0f; i < 0.35f; i += Time.deltaTime) {
            yield return null;
        }
        for (float i = 0f; i < 0.57f; i += Time.deltaTime) {
            yield return null;
        }

        AudioSource a = transform.GetChild(0).GetComponent<AudioSource>();
        a.PlayOneShot(a.clip, MyPlayerPrefs.instance.GetFloat("sounds") * a.volume);
    }
    public IEnumerator fireMissile(bool playSecondSound = true) {
        Vector3 originalPosition = turret.localPosition;
        GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip, MyPlayerPrefs.instance.GetFloat("sounds") * GetComponent<AudioSource>().volume);

        for (float i = 0f; i < 0.67f; i += Time.deltaTime) {
            yield return null;
            turret.GetComponent<SpriteRenderer>().color = new Color(1f, 1, 1.3f, 1 - i / 0.67f);
            turret.Translate(Vector3.right * Time.deltaTime * 2.5f);

        }
        turret.localPosition = originalPosition;
        for (float i = 0f; i < 0.62f; i += Time.deltaTime) {
            yield return null;
        }
        turret.GetComponent<SpriteRenderer>().color = Color.white;
        if (playSecondSound) {
            AudioSource a = transform.GetChild(0).GetComponent<AudioSource>();
            if (Controller.instance.incomingNuclearWarhead) {
                Controller.instance.nuclearSound.PlayOneShot(Controller.instance.nuclearSound.clip, MyPlayerPrefs.instance.GetFloat("sounds") * GetComponent<AudioSource>().volume);
            } else
                a.PlayOneShot(a.clip, MyPlayerPrefs.instance.GetFloat("sounds") * a.volume);
        }
    }
    public IEnumerator fireTank(bool playSecondSound = true) {
        Vector3 originalPosition = turret.localPosition;
        Instantiate(Controller.instance.muzzleAnimationPrefab, muzzle.transform.position, muzzle.transform.rotation);

        //muzzle.enabled = true;
        GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip, MyPlayerPrefs.instance.GetFloat("sounds") * GetComponent<AudioSource>().volume);

        for (float i = 0f; i < 0.7f; i += Time.deltaTime) {
            if (i < 0.1f) {
                turret.Translate(Vector3.left * Time.deltaTime * 0.38f);
            } else {
                muzzle.enabled = false;
                turret.Translate(Vector3.right * Time.deltaTime * 0.0672f);
            }
            yield return null;
        }
        turret.localPosition = originalPosition;
        for (float i = 0f; i < 0.35f; i += Time.deltaTime) {
            yield return null;
        }
        if (playSecondSound) {
            AudioSource a = transform.GetChild(0).GetComponent<AudioSource>();
            a.PlayOneShot(a.clip, MyPlayerPrefs.instance.GetFloat("sounds") * a.volume);
        }
    }



    public void updateLayering() {
        if (isTurret) {
            if (muzzle != null)
                muzzle.transform.position = new Vector3(muzzle.transform.position.x, muzzle.transform.position.y, transform.position.z - 0.012f);
            if (muzzle2 != null)
                muzzle2.transform.position = new Vector3(muzzle2.transform.position.x, muzzle2.transform.position.y, transform.position.z - 0.19f);
            hull.position = new Vector3(hull.position.x, hull.position.y, transform.position.z + 0.017f);
            turret.position = new Vector3(turret.position.x, turret.position.y, transform.position.z + 0.018f);
        } else {
            if (muzzle != null)
                muzzle.transform.position = new Vector3(muzzle.transform.position.x, muzzle.transform.position.y, transform.position.z - 0.012f);
            if (muzzle2 != null)
                muzzle2.transform.position = new Vector3(muzzle2.transform.position.x, muzzle2.transform.position.y, transform.position.z - 0.2f);
            hull.position = new Vector3(hull.position.x, hull.position.y, transform.position.z + 0.018f);
            turret.position = new Vector3(turret.position.x, turret.position.y, transform.position.z + 0.017f);
        }
    }
    public IEnumerator shootSmg(int iteration) {
        muzzle2.enabled = true;
        if (transform.GetChild(1).GetComponent<AudioSource>() != null)
            transform.GetChild(1).GetComponent<AudioSource>().PlayOneShot(transform.GetChild(1).GetComponent<AudioSource>().clip, MyPlayerPrefs.instance.GetFloat("sounds") * transform.GetChild(1).GetComponent<AudioSource>().volume);
        for (float i = 0; i < 0.18f; i += Time.deltaTime) {
            if (i > 0.09f && muzzle2.enabled) {
                muzzle2.enabled = false;
                AudioSource a = transform.GetChild(1).GetComponent<AudioSource>();
                if (a) {
                    a.PlayOneShot(a.clip, MyPlayerPrefs.instance.GetFloat("sounds") * a.volume);
                }
            }
            yield return null;
        }
        if (iteration < 3) {
            StartCoroutine(shootSmg(iteration + 1));
        }
    }
    void Update() {
    }
} 