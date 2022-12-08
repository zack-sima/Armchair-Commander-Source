using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralPerkDescriptionPopup : Popup {
    void Update() {
        if (Input.GetMouseButtonUp(0)) Dismiss();
    }
}