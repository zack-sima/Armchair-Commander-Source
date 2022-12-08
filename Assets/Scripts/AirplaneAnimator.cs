using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirplaneAnimator : MonoBehaviour {
    public int airplaneType, airplaneCategory;
    public int damage; 
    public SpriteRenderer muzzle;
    public Sprite normalBomb;
    public GameObject explosionPrefab;
    public float armorAttackBonus, infantryAttackBonus, artilleryAttackBonus, fortificationAttackBonus, navalAttackBonus, armorPierceAbility;
    [HideInInspector]
    public Tile targetTile;

    void Start() {
        switch (airplaneType) {
        case 0:
            StartCoroutine(shootSmg(0));
            break;
        case 1:
            StartCoroutine(dropBomb());
            break;
        case 2:
            StartCoroutine(paratroopSoldier());
            break;
        } 
    }
    public IEnumerator paratroopSoldier() {
        if (transform.GetChild(0).GetComponent<AudioSource>() != null)
            transform.GetChild(0).GetComponent<AudioSource>().PlayOneShot(transform.GetChild(0).GetComponent<AudioSource>().clip, MyPlayerPrefs.instance.GetFloat("sounds") * transform.GetChild(0).GetComponent<AudioSource>().volume);
        for (float i = 0; i < 0.9f; i += Time.deltaTime) {
            yield return null;
        }
    }
    public IEnumerator dropBomb() {
        float z = targetTile.transform.position.z - 2f;
        if (transform.GetChild(0).GetComponent<AudioSource>() != null)
            transform.GetChild(0).GetComponent<AudioSource>().PlayOneShot(transform.GetChild(0).GetComponent<AudioSource>().clip, MyPlayerPrefs.instance.GetFloat("sounds") * transform.GetChild(0).GetComponent<AudioSource>().volume);
        if (Controller.instance.incomingNuclearWarhead) {
            muzzle.sprite = normalBomb;

            Controller.instance.CheckNukesLeft();
        }
        for (float i = 0; i < 0.9f; i += Time.deltaTime) {
            if (i > 0.45f && !muzzle.enabled) {
                muzzle.enabled = true;
                muzzle.transform.parent = transform.parent;
            }
            if (i > 0.45f)
                muzzle.transform.position = new Vector3(muzzle.transform.position.x, muzzle.transform.position.y - Time.deltaTime * 1.5f, muzzle.transform.position.z);
            yield return null;
        }
        Destroy(muzzle.gameObject);

        if (Controller.instance.incomingNuclearWarhead) {
            Instantiate(Controller.instance.bigExplosionPrefab, new Vector3(targetTile.transform.position.x, targetTile.transform.position.y, z), Quaternion.identity);
            Controller.instance.incomingNuclearWarhead = false;
            if (!Controller.instance.usedNukes) {
                Controller.instance.usedNukes = true;
                Controller.instance.FirstTimeNuke();
            }
            if (GetComponent<AudioSource>() != null) {
                Controller.instance.nuclearSound.PlayOneShot(Controller.instance.nuclearSound.clip, MyPlayerPrefs.instance.GetFloat("sounds") * GetComponent<AudioSource>().volume);
            }
        } else {
            Instantiate(explosionPrefab, new Vector3(targetTile.transform.position.x, targetTile.transform.position.y, z), Quaternion.identity);

            if (GetComponent<AudioSource>() != null)
                GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip, MyPlayerPrefs.instance.GetFloat("sounds") * GetComponent<AudioSource>().volume);
        }
    }
    public IEnumerator shootSmg(int iteration) {
        muzzle.enabled = true;
        if (GetComponent<AudioSource>() != null)
            GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip, MyPlayerPrefs.instance.GetFloat("sounds") * GetComponent<AudioSource>().volume);
        if (iteration == 0) {
            if (transform.GetChild(0).GetComponent<AudioSource>() != null)
                transform.GetChild(0).GetComponent<AudioSource>().PlayOneShot(transform.GetChild(0).GetComponent<AudioSource>().clip, MyPlayerPrefs.instance.GetFloat("sounds") * transform.GetChild(0).GetComponent<AudioSource>().volume);

        }
        for (float i = 0; i < 0.18f; i += Time.deltaTime) {
            if (i > 0.09f && muzzle.enabled) {
                muzzle.enabled = false;
                if (GetComponent<AudioSource>() != null)
                    GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip, MyPlayerPrefs.instance.GetFloat("sounds") * GetComponent<AudioSource>().volume);

            }
            yield return null;
        }
        if (iteration < 3) {
            StartCoroutine(shootSmg(iteration + 1));
        } else {
            for (float i = 0; i < 0.07f; i +=Time.deltaTime)
                yield return null; 

            if (muzzle.GetComponent<AudioSource>() != null)
                muzzle.GetComponent<AudioSource>().PlayOneShot(muzzle.GetComponent<AudioSource>().clip, MyPlayerPrefs.instance.GetFloat("sounds") * muzzle.GetComponent<AudioSource>().volume);

        }
    }

    float count = 0f;
    void Update() {
        transform.Translate(Vector3.left * Time.deltaTime * 1.6f);
        float rotation = airplaneType == 2 || Controller.instance.airplaneType == 3 ? 16f : 70f; 
        transform.Rotate(new Vector3(0, 0, -rotation * Time.deltaTime));
        
        count += Time.deltaTime;
        if (count > 0.78f)
            GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, GetComponent<SpriteRenderer>().color.a - Time.deltaTime * 3f); 
    }
} 