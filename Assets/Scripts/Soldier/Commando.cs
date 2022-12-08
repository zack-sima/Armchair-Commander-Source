using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Commando : Unit {
    public SoldierAnimator soldier1, soldier2, soldier3;
    public GameObject explosionPrefab;
    public new void Start() {
        base.Start();
    }
    public override void MoveSound() {
        if (currentTile.terrain != Terrain.water) {
            Controller.instance.marchingSound.PlayOneShot(Controller.instance.marchingSound.clip, MyPlayerPrefs.instance.GetFloat("sounds") * Controller.instance.marchingSound.volume);
        } else {
            Controller.instance.navySound.PlayOneShot(Controller.instance.navySound.clip, MyPlayerPrefs.instance.GetFloat("sounds") * Controller.instance.navySound.volume);
        }
    }
    public override void toggleVisibility() {
        if (gameObject.layer == 0) {
            visible = false; 
            gameObject.layer = 8; 
            foreach (Transform i in soldier1.GetComponentsInChildren<Transform>()) {
                i.gameObject.layer = 8;
            }
            foreach (Transform i in soldier2.GetComponentsInChildren<Transform>()) {
                i.gameObject.layer = 8;
            }
            foreach (Transform i in soldier3.GetComponentsInChildren<Transform>()) {
                i.gameObject.layer = 8;
            }
        } else {
            visible = true; 
            gameObject.layer = 0; 
            foreach (Transform i in soldier1.GetComponentsInChildren<Transform>()) {
                i.gameObject.layer = 0; 
            }
            foreach (Transform i in soldier2.GetComponentsInChildren<Transform>()) {
                i.gameObject.layer = 0; 
            }
            foreach (Transform i in soldier3.GetComponentsInChildren<Transform>()) {
                i.gameObject.layer = 0; 
            }
        }    
    }
    public override void updateSkin() {
        SoldierPrefabsManager manager = FindObjectOfType<SoldierPrefabsManager>();
        switch (troopSkin) {
        case Skin.Soviet:
            soldier1.leftArm.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.leftArmSoviet;
            soldier1.rightArm.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.rightArmSoviet;
            soldier1.legs.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.legsSoviet;
            soldier1.bodyHead.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.bodySoviet;
            soldier2.leftArm.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.leftArmSoviet;
            soldier2.rightArm.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.rightArmSoviet;
            soldier2.legs.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.legsSoviet;
            soldier2.bodyHead.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.bodySoviet;
            soldier3.leftArm.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.leftArmSoviet;
            soldier3.rightArm.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.rightArmSoviet;
            soldier3.legs.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.legsSoviet;
            soldier3.bodyHead.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.bodySoviet;
            break;
        case Skin.Japanese:
            soldier1.leftArm.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.leftArmSoviet;
            soldier1.rightArm.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.rightArmSoviet;
            soldier1.legs.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.legsSoviet;
            soldier1.bodyHead.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.bodyJapan;
            soldier2.leftArm.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.leftArmSoviet;
            soldier2.rightArm.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.rightArmSoviet;
            soldier2.legs.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.legsSoviet;
            soldier2.bodyHead.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.bodyJapan;
            soldier3.leftArm.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.leftArmSoviet;
            soldier3.rightArm.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.rightArmSoviet;
            soldier3.legs.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.legsSoviet;
            soldier3.bodyHead.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.bodyJapan;
            break;
        case Skin.British:
            soldier1.leftArm.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.leftArmUK;
            soldier1.rightArm.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.rightArmUK;
            soldier1.legs.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.legsUK;
            soldier1.bodyHead.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.bodyUK;
            soldier2.leftArm.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.leftArmUK;
            soldier2.rightArm.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.rightArmUK;
            soldier2.legs.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.legsUK;
            soldier2.bodyHead.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.bodyUK;
            soldier3.leftArm.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.leftArmUK;
            soldier3.rightArm.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.rightArmUK;
            soldier3.legs.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.legsUK;
            soldier3.bodyHead.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.bodyUK;
            break;
        case Skin.French:
            soldier1.leftArm.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.leftArmUK;
            soldier1.rightArm.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.rightArmUK;
            soldier1.legs.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.legsUK;
            soldier1.bodyHead.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.bodyUK;
            soldier2.leftArm.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.leftArmUK;
            soldier2.rightArm.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.rightArmUK;
            soldier2.legs.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.legsUK;
            soldier2.bodyHead.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.bodyUK;
            soldier3.leftArm.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.leftArmUK;
            soldier3.rightArm.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.rightArmUK;
            soldier3.legs.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.legsUK;
            soldier3.bodyHead.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.bodyUK;
            break;
        case Skin.American:
            soldier1.leftArm.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.leftArmUS;
            soldier1.rightArm.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.rightArmUS;
            soldier1.legs.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.legsUS;
            soldier1.bodyHead.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.bodyUS;
            soldier2.leftArm.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.leftArmUS;
            soldier2.rightArm.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.rightArmUS;
            soldier2.legs.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.legsUS;
            soldier2.bodyHead.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.bodyUS;
            soldier3.leftArm.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.leftArmUS;
            soldier3.rightArm.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.rightArmUS;
            soldier3.legs.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.legsUS;
            soldier3.bodyHead.GetComponent<SpriteRenderer>().sprite = TroopSkins.instance.bodyUS;
            break;
        }
        if (troopSkin == Skin.Soviet) {
            //soldier 1 holds bazooka/panzershreck
            soldier2.gun.GetComponent<SpriteRenderer>().sprite = manager.sovietSMG;
            soldier3.gun.GetComponent<SpriteRenderer>().sprite = manager.sovietSMG;
            soldier2.sovietSmg = true;
            soldier3.sovietSmg = true;
        } else if (troopSkin == Skin.American || troopSkin == Skin.French || troopSkin == Skin.British) {
            soldier2.gun.GetComponent<SpriteRenderer>().sprite = manager.americanSMG;
            soldier3.gun.GetComponent<SpriteRenderer>().sprite = manager.americanSMG;
        }
    }
    public override void select() {
        soldier1.StartCoroutine(soldier1.raiseGun());  
        soldier2.StartCoroutine(soldier2.raiseGun()); 
        soldier3.StartCoroutine(soldier3.raiseGun());     
    }
    public override void deselect() {    
        soldier1.StartCoroutine(soldier1.lowerGun());  
        soldier2.StartCoroutine(soldier2.lowerGun()); 
        soldier3.StartCoroutine(soldier3.lowerGun());
    }
    IEnumerator attackWithDelay(float delay, Tile target) {
        Vector3 targetPosition = target.transform.position;
        for (float i = 0f; i < delay; i +=Time.deltaTime) 
            yield return null; 
        
        soldier1.StartCoroutine(soldier1.shootBazooka());
        soldier2.StartCoroutine(soldier2.shootSmg(0));
        soldier3.StartCoroutine(soldier3.shootSmg(0));

        for (float i = 0f; i < 1; i += Time.deltaTime)
            yield return null;

        Instantiate(explosionPrefab, new Vector3(targetPosition.x, targetPosition.y, targetPosition.z - 2f), Quaternion.identity);
    } 
    public override void animateAttack(float delay, Tile target) {
        StartCoroutine(attackWithDelay(delay, target));
    } 
    public override void updateLayering() {
        soldier1.updateLayering();
        soldier2.updateLayering();
        soldier3.updateLayering(); 
    }   
}
