using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfantryArtillery : Unit {
    public GameObject explosionPrefab; 
    public TankAnimator tankAnimator; 
    public new void Start() {
        base.Start();
    }
    public override void MoveSound() {
        if (currentTile.terrain != Terrain.water) {
            Controller.instance.truckSound.PlayOneShot(Controller.instance.truckSound.clip, MyPlayerPrefs.instance.GetFloat("sounds") * Controller.instance.truckSound.volume);
        } else {
            Controller.instance.navySound.PlayOneShot(Controller.instance.navySound.clip, MyPlayerPrefs.instance.GetFloat("sounds") * Controller.instance.navySound.volume);
        }
    }
    public override void updateSkin() {
        if (troopSkin == Skin.Japanese) {
            tankAnimator.hull.GetComponent<SpriteRenderer>().sprite = tankAnimator.japaneseHull;
        } else if (troopSkin == Skin.Soviet || troopSkin == Skin.American || troopSkin == Skin.British) {
            //artillery body 
            tankAnimator.hull.GetComponent<SpriteRenderer>().sprite = tankAnimator.sovietHull;
            tankAnimator.muzzle.transform.localPosition = new Vector3(5.98f, -0.35f, -1f);
        } else if (troopSkin == Skin.French) {
            tankAnimator.hull.GetComponent<SpriteRenderer>().sprite = tankAnimator.frenchHull;
            tankAnimator.muzzle.transform.localPosition = new Vector3(5.98f, -0.35f, -1f);
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
        StartCoroutine(tankAnimator.fireArtillery());
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
