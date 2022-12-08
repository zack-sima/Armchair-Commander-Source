using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TutorialPopup : MonoBehaviour {
    public Transform arrowPivot;
    public Image rightArrow, leftArrow, upArrow, downArrow;
    List<Image> arrows;
    [HideInInspector]
    public Controller controller;
    [HideInInspector]
    public bool notUI; //if not UI, prevent calling clickedpopup
    float interpolateValue = 0f;
    Vector3 originalPosition;

    Button bindedButton = null; //binded button for UI tutorial
    void Start() {

        arrows = new List<Image>() { rightArrow, leftArrow, upArrow, downArrow };
        foreach (Image i in arrows) i.gameObject.SetActive(false);
        originalPosition = arrowPivot.position;
        if (!notUI) {
            StartCoroutine(FindButton());
        } else {
            SetSiblingIndexes();
        }
    }
    void SetSiblingIndexes() {
        transform.SetAsLastSibling();
        controller.pauseButton.transform.SetAsLastSibling();
        controller.pauseUI.SetAsLastSibling();

        foreach (Image i in arrows) i.gameObject.SetActive(true);
    }
    IEnumerator FindButton() {
        yield return null;
        while (controller.troopMoving || controller.passingRound) {
            transform.SetAsLastSibling();
            yield return null;
        }
        for (int i = 0; i < 3; i++) {
            transform.SetAsLastSibling();
            yield return null;
        }

       // print("is UI, checking");
        //find closest button

        float closestDistance = 1000000;
        Button closestButton = null;
        foreach (Button b in GameObject.FindObjectsOfType<Button>()) {
            float d = Vector2.Distance(b.transform.position, transform.position);
            if (d < closestDistance) {
                closestDistance = d;
                closestButton = b;
            }
        }
        if (closestButton) {
//            print(closestButton.name);
            bindedButton = closestButton;
            closestButton.onClick.AddListener(() => RealClickedPopup());
        } else {
            Debug.LogWarning("no button found");
        }
        SetSiblingIndexes();
    }
    //public void ClickedPopup() { //for UI
    //    if (!notUI) RealClickedPopup();
    //}

    public void RealClickedPopup() {
        if (!controller.paused) {
            controller.ContinuePopup();
            Destroy(gameObject);
        }
    }
    void Update() {
        if (Input.GetKeyDown(KeyCode.S)) StartCoroutine(FindButton());

        interpolateValue = Mathf.Sin(Time.time * 1.2f);
        arrowPivot.position = originalPosition + Vector3.right * interpolateValue * 20f;
    }
    void Destroy() {
        if (bindedButton)
            bindedButton.onClick.RemoveListener(() => RealClickedPopup());
    }
}