using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bunker : Unit {
    public SpriteRenderer constructionSprite, turretSprite;
    public SpriteRenderer muzzle1, muzzle2;
    public AudioSource fireAudio, hitAudio;
    public new void Start() {
        base.Start();
    }
    public override void MoveSound() {
    }
    public override void updateSkin() {

    }
    public override void toggleVisibility() {
        if (gameObject.layer == 0) {
            visible = false; 
            gameObject.layer = 8;
        } else {
            visible = true;
            gameObject.layer = 0;
        }
    }
    public override void select() {  
    }
    public override void deselect() {    
    }
    public new void Update() {
        base.Update();
        muzzle1.transform.position = new Vector3(muzzle1.transform.position.x, muzzle1.transform.position.y, transform.position.z - 0.1f);
        muzzle2.transform.position = new Vector3(muzzle2.transform.position.x, muzzle2.transform.position.y, transform.position.z - 0.1f);

        if (tier != 1) {
            turretSprite.enabled = false;
            constructionSprite.enabled = true;
        } else {
            turretSprite.enabled = true;
            constructionSprite.enabled = false;
        }
    }
    IEnumerator bullets() {
        float countdown = 0.09f;
        for (float i = 0f; i < 0.86; i += Time.deltaTime) {
            countdown -= Time.deltaTime;
            if (countdown > 0.052f) {
                muzzle1.enabled = true;
                muzzle2.enabled = true;
            } else {
                muzzle1.enabled = false;
                muzzle2.enabled = false;
            }
            if (countdown < 0f) {
                countdown = 0.09f;
                fireAudio.PlayOneShot(fireAudio.clip, MyPlayerPrefs.instance.GetFloat("sounds") * fireAudio.volume);
            }
            yield return null;
        }
        muzzle1.enabled = false;
        muzzle2.enabled = false;
    }
    IEnumerator attackWithDelay(float delay, Tile target) {
        Vector3 targetPosition = Vector3.zero;
        if (target != null)
            targetPosition = target.transform.position;
        for (float i = 0f; i < delay; i +=Time.deltaTime) 
            yield return null;
        StartCoroutine(bullets());
        fireAudio.PlayOneShot(fireAudio.clip, MyPlayerPrefs.instance.GetFloat("sounds") * fireAudio.volume);
        for (float i = 0f; i < 1.06f; i +=Time.deltaTime)
            yield return null;
        hitAudio.PlayOneShot(hitAudio.clip, MyPlayerPrefs.instance.GetFloat("sounds") * fireAudio.volume);
        if (target != null)
            Instantiate(controller.bulletExplosionPrefab, new Vector3(targetPosition.x, targetPosition.y, targetPosition.z - 2f), Quaternion.identity);

    }
    public override void animateAttack(float delay, Tile target) {
        StartCoroutine(attackWithDelay(delay, target));
    } 
    public override void updateLayering() {
        constructionSprite.transform.position = new Vector3(turretSprite.transform.position.x, turretSprite.transform.position.y, turretSprite.transform.position.z + 0.017f);
    }   
}