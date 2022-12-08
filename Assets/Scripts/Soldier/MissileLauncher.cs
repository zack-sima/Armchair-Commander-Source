using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncher : Unit {
    public GameObject explosionPrefab;
    public TankAnimator tankAnimator;
    public SpriteRenderer constructionSprite, turretSprite1, turretSprite2;
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
            foreach (Transform i in tankAnimator.GetComponentsInChildren<Transform>()) {
                i.gameObject.layer = 8;
            }
        } else {
            visible = true;
            gameObject.layer = 0;
            foreach (Transform i in tankAnimator.GetComponentsInChildren<Transform>()) {
                i.gameObject.layer = 0;
            }
        }
    }
    public override void select() {  
    }
    public override void deselect() {    
    }
    public new void Update() {
        base.Update();
        if (tier != 1) {
            turretSprite1.enabled = false;
            turretSprite2.enabled = false;
            constructionSprite.enabled = true;
        } else {
            turretSprite1.enabled = true;
            turretSprite2.enabled = true;
            constructionSprite.enabled = false;
        }
    }
    IEnumerator attackWithDelay(float delay, Tile target) {
        //null detection for anti aircraft role (explosion not shown)
        Vector3 targetPosition = Vector3.zero;
        if (target != null)
            targetPosition = target.transform.position;
        for (float i = 0f; i < delay; i += Time.deltaTime)
            yield return null;
        StartCoroutine(tankAnimator.fireMissile(target != null));
        for (float i = 0f; i < 1.35f; i +=Time.deltaTime)
            yield return null;
        if (target != null) {
            if (controller.incomingNuclearWarhead) {
                Instantiate(controller.bigExplosionPrefab, new Vector3(targetPosition.x, targetPosition.y, targetPosition.z - 2f), Quaternion.identity);
                controller.incomingNuclearWarhead = false;
                if (!controller.usedNukes) {
                    controller.usedNukes = true;
                    Controller.instance.FirstTimeNuke();
                }
                controller.CheckNukesLeft();
            } else {
                Instantiate(explosionPrefab, new Vector3(targetPosition.x, targetPosition.y, targetPosition.z - 2f), Quaternion.identity);
            }
        }
    } 
    public override void animateAttack(float delay, Tile target) {
        StartCoroutine(attackWithDelay(delay, target));
    } 
    public override void updateLayering() {
        constructionSprite.transform.position = new Vector3(constructionSprite.transform.position.x, constructionSprite.transform.position.y, turretSprite1.transform.position.z + 0.017f);
        tankAnimator.updateLayering(); 
    }   
}

