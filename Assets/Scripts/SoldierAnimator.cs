using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierAnimator : MonoBehaviour {
    public int soldierLayer;
    //public Sprite leftArmSoviet, rightArmSoviet, bodyHeadSoviet, legsSoviet; 
    public Transform leftArm, rightArm, bodyHead, legs, shoes, gun; 
    public Transform rightArmAnchor, leftArmAnchor, gunAnchor;
    public Unit unitController;
    public SpriteRenderer muzzle;
    [HideInInspector]
    public bool sovietSmg;
    void Start() {
        updateLayering(); 
    }
    bool attacking = false;
    public IEnumerator shootRifle(int iteration) {
        attacking = true;
        rightArmAnchor.localEulerAngles = new Vector3(0f, 0f, 60f);
        leftArmAnchor.localEulerAngles = new Vector3(0f, 0f, 65f);
        gunAnchor.localEulerAngles = new Vector3(0f, 0f, -25f);
        muzzle.enabled = true;
        if (GetComponent<AudioSource>() != null)
            GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip, MyPlayerPrefs.instance.GetFloat("sounds") * GetComponent<AudioSource>().volume);
        for (float i = 0; i < 0.35f; i += Time.deltaTime) { 
            if (i > 0.18f && muzzle.enabled)
                muzzle.enabled = false;
            if (i <= 0.05f) {
                rightArmAnchor.localEulerAngles = new Vector3(0f, 0f, i / 0.05f * 9f + 60f);
                leftArmAnchor.localEulerAngles = new Vector3(0f, 0f, i / 0.05f * 9f + 65f);
                yield return null;
            } else {
                rightArmAnchor.localEulerAngles = new Vector3(0f, 0f, (0.35f - i) / 0.3f * 9f + 60f);
                leftArmAnchor.localEulerAngles = new Vector3(0f, 0f, (0.35f - i) / 0.3f * 9f + 65f);
                yield return null;
            }

        }
        rightArmAnchor.localEulerAngles = new Vector3(0f, 0f, 60f);
        leftArmAnchor.localEulerAngles = new Vector3(0f, 0f, 65f);
        gunAnchor.localEulerAngles = new Vector3(0f, 0f, -25f);

        for (float i = 0; i < 0.1f; i += Time.deltaTime) {
            yield return null;
        }

        if (iteration < 1) {
            StartCoroutine(shootRifle(iteration + 1));
        } else {
            StartCoroutine(lowerGun());
            for (float i = 0; i < 0.07f; i +=Time.deltaTime)
                yield return null;
            if (GetComponent<AudioSource>() != null)
                transform.GetChild(0).GetComponent<AudioSource>().PlayOneShot(transform.GetChild(0).GetComponent<AudioSource>().clip, MyPlayerPrefs.instance.GetFloat("sounds") * transform.GetChild(0).GetComponent<AudioSource>().volume);

            attacking = false;
        }
    }
    public IEnumerator shootBazooka() {
        attacking = true;
        rightArmAnchor.localEulerAngles = new Vector3(0f, 0f, 60f);
        leftArmAnchor.localEulerAngles = new Vector3(0f, 0f, 65f);
        gunAnchor.localEulerAngles = new Vector3(0f, 0f, -25f);

        Instantiate(Controller.instance.muzzleAnimationPrefab, muzzle.transform.position, muzzle.transform.rotation);

        //muzzle.enabled = true;
        if (GetComponent<AudioSource>() != null)
            GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip, MyPlayerPrefs.instance.GetFloat("sounds") * GetComponent<AudioSource>().volume);
        for (float i = 0; i < 0.35f; i += Time.deltaTime) {
            if (i > 0.18f && muzzle.enabled)
                muzzle.enabled = false;
            if (i <= 0.05f) {
                rightArmAnchor.localEulerAngles = new Vector3(0f, 0f, i / 0.05f * 9f + 60f);
                leftArmAnchor.localEulerAngles = new Vector3(0f, 0f, i / 0.05f * 9f + 65f);
                yield return null;
            } else {
                rightArmAnchor.localEulerAngles = new Vector3(0f, 0f, (0.35f - i) / 0.3f * 9f + 60f);
                leftArmAnchor.localEulerAngles = new Vector3(0f, 0f, (0.35f - i) / 0.3f * 9f + 65f);
                yield return null;
            }

        }
        rightArmAnchor.localEulerAngles = new Vector3(0f, 0f, 60f);
        leftArmAnchor.localEulerAngles = new Vector3(0f, 0f, 65f);
        gunAnchor.localEulerAngles = new Vector3(0f, 0f, -25f);

        for (float i = 0; i < 0.1f; i += Time.deltaTime) {
            yield return null;
        }
        

        StartCoroutine(lowerGun());
        for (float i = 0; i < 0.53f; i += Time.deltaTime)
            yield return null;
        if (transform.GetChild(0).GetComponent<AudioSource>() != null)
            transform.GetChild(0).GetComponent<AudioSource>().PlayOneShot(transform.GetChild(0).GetComponent<AudioSource>().clip, MyPlayerPrefs.instance.GetFloat("sounds") * transform.GetChild(0).GetComponent<AudioSource>().volume);

        attacking = false;
    }
    public IEnumerator shootSmg(int iteration) {
        attacking = true;
        rightArmAnchor.localEulerAngles = new Vector3(0f, 0f, 60f);
        leftArmAnchor.localEulerAngles = new Vector3(0f, 0f, 65f);
        gunAnchor.localEulerAngles = new Vector3(0f, 0f, -25f);
        muzzle.enabled = true;
        if (GetComponent<AudioSource>() != null) {
            if (sovietSmg)
                GetComponent<AudioSource>().pitch = 1.25f;
            GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip, MyPlayerPrefs.instance.GetFloat("sounds") * GetComponent<AudioSource>().volume);
        }
        for (float i = 0; i < 0.18f; i += Time.deltaTime) {
            if (i > 0.09f && muzzle.enabled) {
                muzzle.enabled = false;
                if (GetComponent<AudioSource>() != null)
                    GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip, MyPlayerPrefs.instance.GetFloat("sounds") * GetComponent<AudioSource>().volume);



            }
            if (i <= 0.05f) {
                rightArmAnchor.localEulerAngles = new Vector3(0f, 0f, i / 0.05f * 1f + 60f);
                leftArmAnchor.localEulerAngles = new Vector3(0f, 0f, i / 0.05f * 1f + 65f);
                yield return null;
            } else {
                rightArmAnchor.localEulerAngles = new Vector3(0f, 0f, (0.18f - i) / 0.1f * 1f + 60f);
                leftArmAnchor.localEulerAngles = new Vector3(0f, 0f, (0.18f - i) / 0.1f * 1f + 65f);
                yield return null;
            }

        }
        rightArmAnchor.localEulerAngles = new Vector3(0f, 0f, 60f);
        leftArmAnchor.localEulerAngles = new Vector3(0f, 0f, 65f);
        gunAnchor.localEulerAngles = new Vector3(0f, 0f, -25f);

        if (iteration < 3.4f) {
            StartCoroutine(shootSmg(iteration + 1));
        } else {
            StartCoroutine(lowerGun());
            for (float i = 0; i < 0.07f; i +=Time.deltaTime)
                yield return null; 

            if (transform.GetChild(0).GetComponent<AudioSource>() != null)
                transform.GetChild(0).GetComponent<AudioSource>().PlayOneShot(transform.GetChild(0).GetComponent<AudioSource>().clip, MyPlayerPrefs.instance.GetFloat("sounds") * transform.GetChild(0).GetComponent<AudioSource>().volume);
            attacking = false;
        }
    }
    bool isLowered = true;
    public IEnumerator lowerGun() {
        isLowered = true;
        float duration = 0.16f; 
        for (float i = duration; i > 0f; i -= Time.deltaTime) {
            rightArmAnchor.localEulerAngles = new Vector3(0f, 0f, i / duration * 60f);
            leftArmAnchor.localEulerAngles = new Vector3(0f, 0f, i / duration * 65f);
            gunAnchor.localEulerAngles = new Vector3(0f, 0f, i / duration * -25f);
            yield return null;
        }
        rightArmAnchor.localEulerAngles = Vector3.zero; 
        leftArmAnchor.localEulerAngles = Vector3.zero; 
        gunAnchor.localEulerAngles = Vector3.zero;
    }
    public IEnumerator raiseGun() {
        isLowered = false;
        float duration = 0.2f; 
        for (float i = 0; i < duration; i +=Time.deltaTime) {
            rightArmAnchor.localEulerAngles= new Vector3(0f, 0f, i / duration * 60f);
            leftArmAnchor.localEulerAngles = new Vector3(0f, 0f, i / duration * 65f);
            gunAnchor.localEulerAngles = new Vector3(0f, 0f, i / duration * -25f); 
            yield return null; 
        }
        rightArmAnchor.localEulerAngles = new Vector3(0f, 0f, 60f);
        leftArmAnchor.localEulerAngles = new Vector3(0f, 0f, 65f);
        gunAnchor.localEulerAngles = new Vector3(0f, 0f, -25f);
    }
    public void updateLayering() {
        leftArm.position = new Vector3(leftArm.position.x, leftArm.position.y, transform.position.z + 0.018f - soldierLayer / 10f);
        bodyHead.position = new Vector3(bodyHead.position.x, bodyHead.position.y, transform.position.z + 0.017f - soldierLayer / 10f);
        legs.position = new Vector3(legs.position.x, legs.position.y, transform.position.z + 0.016f - soldierLayer / 10f);
        rightArm.position = new Vector3(rightArm.position.x, rightArm.position.y, transform.position.z + 0.015f - soldierLayer / 10f);
        gun.position = new Vector3(gun.position.x, gun.position.y, transform.position.z + 0.012f - soldierLayer / 10f);
    }
    void Update() {
        if (!isLowered && !attacking && !unitController.currentTile.selected) {
            StartCoroutine(lowerGun());
        }
    }
} 