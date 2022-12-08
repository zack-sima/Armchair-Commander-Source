using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carrier : Unit {
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
        } else {
            visible = true; 
            gameObject.layer = 0;
        }
    }
    public override void select() {  
    }
    public override void deselect() {    
    }
    IEnumerator attackWithDelay(float delay, Tile target) {
        for (float i = 0f; i < delay; i +=Time.deltaTime) 
            yield return null;
        AirplaneAnimator ins = Instantiate(controller.productBar.airplanes[1], new Vector3(target.transform.position.x + 0.9f, target.transform.position.y + 0.92f, -350f), controller.productBar.airplanes[1].transform.rotation).GetComponent<AirplaneAnimator>();
        ins.targetTile = target;
        
        for (float i = 0f; i < 1.1f; i +=Time.deltaTime)
            yield return null;
    } 
    public override void animateAttack(float delay, Tile target) {
        StartCoroutine(attackWithDelay(delay, target));
    } 
    public override void updateLayering() {
    }   
}
