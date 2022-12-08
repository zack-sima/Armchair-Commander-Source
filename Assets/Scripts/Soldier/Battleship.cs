using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battleship : Unit {
    public GameObject explosionPrefab;
    public TankAnimator tankAnimator;
    public new void Start() {
        base.Start();
    }
    public override void MoveSound() {
        Controller.instance.navySound.PlayOneShot(Controller.instance.navySound.clip, MyPlayerPrefs.instance.GetFloat("sounds") * Controller.instance.navySound.volume);
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
    IEnumerator attackWithDelay(float delay, Tile target) {
        Vector3 targetPosition = Vector3.zero;
        if (target != null)
            targetPosition = target.transform.position;
        for (float i = 0f; i < delay; i += Time.deltaTime)
            yield return null;
        StartCoroutine(tankAnimator.fireShip(target != null));
        for (float i = 0f; i < 1.1f; i += Time.deltaTime)
            yield return null;
        if (target != null)
            Instantiate(explosionPrefab, new Vector3(targetPosition.x, targetPosition.y, targetPosition.z - 2f), Quaternion.identity);
    }
    public override void animateAttack(float delay, Tile target) {
        StartCoroutine(attackWithDelay(delay, target));
    } 
    public override void updateLayering() {
        tankAnimator.updateLayering(); 
    }   
}
