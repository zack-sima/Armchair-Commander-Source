using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketArtillery : Unit {
    public GameObject explosionPrefab, rocketPrefab;
    public TankAnimator tankAnimator;
    public Vector2 sovietRocketAnchor, americanRocketAnchor;
    public float sovietRotationZ, americanRotationZ;
    float rocketRotationZ;

    public Transform muzzle;
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
        rocketRotationZ = muzzle.eulerAngles.z;
        if (troopSkin == Skin.Japanese) {
            //artillery body 
            tankAnimator.hull.GetComponent<SpriteRenderer>().sprite = tankAnimator.japaneseHull;
        } else if (troopSkin == Skin.Soviet) {
            //artillery body 
            tankAnimator.hull.GetComponent<SpriteRenderer>().sprite = tankAnimator.sovietHull;
            muzzle.localPosition = sovietRocketAnchor;
            rocketRotationZ = sovietRotationZ;
        } else if (troopSkin == Skin.American) {
            tankAnimator.hull.GetComponent<SpriteRenderer>().sprite = tankAnimator.americanHull;
            muzzle.localPosition = americanRocketAnchor;
            rocketRotationZ = americanRotationZ;
        } else if (troopSkin == Skin.French) {
            tankAnimator.hull.GetComponent<SpriteRenderer>().sprite = tankAnimator.frenchHull;
            muzzle.localPosition = americanRocketAnchor;
            rocketRotationZ = americanRotationZ;
        } else if (troopSkin == Skin.British) {
            tankAnimator.hull.GetComponent<SpriteRenderer>().sprite = tankAnimator.englishHull;
            muzzle.localPosition = americanRocketAnchor;
            rocketRotationZ = americanRotationZ;
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
        
        StartCoroutine(tankAnimator.FireRocketArtillery(new Vector3(muzzle.position.x, muzzle.position.y, transform.position.z - 0.12f), rocketRotationZ, rocketPrefab));
        for (float i = 0f; i < 1.25f; i +=Time.deltaTime)
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
