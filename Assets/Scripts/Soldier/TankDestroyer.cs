using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankDestroyer : Unit {
    public GameObject explosionPrefab; 
    public TankAnimator tankAnimator; 
    public new void Start() {
        base.Start();
    }
    public override void MoveSound() {
        if (currentTile.terrain != Terrain.water) {
            Controller.instance.tankSound.PlayOneShot(Controller.instance.tankSound.clip, MyPlayerPrefs.instance.GetFloat("sounds") * Controller.instance.tankSound.volume);
        } else {
            Controller.instance.navySound.PlayOneShot(Controller.instance.navySound.clip, MyPlayerPrefs.instance.GetFloat("sounds") * Controller.instance.navySound.volume);
        }
    }
    public override void updateSkin() {
        if (troopSkin == Skin.Japanese) {
            tankAnimator.hull.GetComponent<SpriteRenderer>().sprite = tankAnimator.japaneseHull;
            tankAnimator.turret.GetComponent<SpriteRenderer>().sprite = tankAnimator.japaneseTurret;
        } else if (troopSkin == Skin.American) {
            tankAnimator.hull.GetComponent<SpriteRenderer>().sprite = tankAnimator.americanHull;
            tankAnimator.turret.GetComponent<SpriteRenderer>().sprite = tankAnimator.americanTurret;
            tankAnimator.muzzle.transform.localPosition = new Vector3(9.28f, 2.01f, 0f);
        } else if (troopSkin == Skin.French) {
            tankAnimator.hull.GetComponent<SpriteRenderer>().sprite = tankAnimator.frenchHull;
            tankAnimator.turret.GetComponent<SpriteRenderer>().sprite = tankAnimator.frenchTurret;
            tankAnimator.muzzle.transform.localPosition = new Vector3(9.28f, 1.33f, 0f);
        } else if (troopSkin == Skin.British) {
            tankAnimator.hull.GetComponent<SpriteRenderer>().sprite = tankAnimator.englishHull;
            tankAnimator.turret.GetComponent<SpriteRenderer>().sprite = tankAnimator.englishTurret;
            tankAnimator.muzzle.transform.localPosition = new Vector3(9.28f, 1.33f, 0f);
        } else if (troopSkin == Skin.Soviet) {
            tankAnimator.hull.GetComponent<SpriteRenderer>().sprite = tankAnimator.sovietHull;
            tankAnimator.turret.GetComponent<SpriteRenderer>().sprite = tankAnimator.sovietTurret;
            tankAnimator.muzzle.transform.localPosition = new Vector3(9.28f, 1.330f, 0f);

        }
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
        Vector3 targetPosition = target.transform.position; 
        for (float i = 0f; i < delay; i +=Time.deltaTime) 
            yield return null;
        StartCoroutine(tankAnimator.fireTank());
        for (float i = 0f; i < 1.1f; i +=Time.deltaTime)
            yield return null;
        Instantiate(explosionPrefab, new Vector3(targetPosition.x, targetPosition.y, targetPosition.z - 2f), Quaternion.identity);
    } 
    public override void animateAttack(float delay, Tile target) {
        StartCoroutine(attackWithDelay(delay, target));
    } 
    public override void updateLayering() {
        tankAnimator.updateLayering(); 
    }   
}
