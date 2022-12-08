using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

//NOTE: DEPRECATED, USE BUTTON DIMMER
public class DimText : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {
    public Image dimImage, dimImage2; //optional
    public Text dimText;
    public Text dimText2;
    public bool callback, dimUninteractable; //contains a callback function to replace the button
    public Button myButton;
    void Callbacked() {
        controller.ContinuePopup();
    }
    void Awake() {
        myButton = GetComponent<Button>();
    }
    void Start() {
        //deltaInteractable = GetComponent<Button>().interactable;
        //GameObject c = GameObject.Find("Controller");
        //if (c != null) {
        //    controller = c.GetComponent<Controller>();
        //    if (MyPlayerPrefs.instance.GetInt("tutorial") > 0 && callback) {
        //        myButton.onClick.AddListener(Callbacked);
        //    }
        //}



    }
    public bool completelyTransparent;

    public void OnPointerClick(PointerEventData eventData) {
        
    }
    public void OnPointerDown(PointerEventData eventData) {
        if (!callback && GetComponent<Button>().interactable) {
            if (dimText)
                dimText.color = new Color(dimText.color.r, dimText.color.g, dimText.color.b, 0.75f);
            if (dimText2)
                dimText2.color = new Color(dimText2.color.r, dimText2.color.g, dimText2.color.b, 0.75f);
            if (dimImage)
                dimImage.color = new Color(dimImage.color.r, dimImage.color.g, dimImage.color.b, 0.75f);
            if (dimImage2)
                dimImage2.color = new Color(dimImage2.color.r, dimImage2.color.g, dimImage2.color.b, 0.75f);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {

    }
    public void OnPointerExit(PointerEventData eventData) {
        
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (!callback && GetComponent<Button>().interactable) {
            if (dimText)
                dimText.color = new Color(dimText.color.r, dimText.color.g, dimText.color.b, 1f);
            if (dimText2)
                dimText2.color = new Color(dimText2.color.r, dimText2.color.g, dimText2.color.b, 1f);
            if (dimImage)
                dimImage.color = new Color(dimImage.color.r, dimImage.color.g, dimImage.color.b, 1f);
            if (dimImage2)
                dimImage2.color = new Color(dimImage2.color.r, dimImage2.color.g, dimImage2.color.b, 1f);
        }
    }
    public Controller controller;
    bool deltaInteractable = false;
    void Update() {
        if (!callback && !GetComponent<Button>().interactable && !dimUninteractable) {
            if (completelyTransparent) {
                transform.GetChild(0).GetComponent<Text>().color = new Color(0.8f, 0.8f, 0.8f, 0f);
                if (dimImage != null) {
                    dimImage.color = new Color(dimImage.color.r, dimImage.color.g, dimImage.color.b, 0f);
                    dimText.color = new Color(dimText.color.r, dimText.color.g, dimText.color.b, 0f);
                }
                if (dimImage2 != null) {
                    dimImage2.color = new Color(dimImage2.color.r, dimImage2.color.g, dimImage2.color.b, 0f);
                    dimText.color = new Color(dimText.color.r, dimText.color.g, dimText.color.b, 0f);
                }
            } else {
                transform.GetChild(0).GetComponent<Text>().color = new Color(1,1,1, 0.75f);
                if (dimImage != null) {
                    dimImage.color = new Color(dimImage.color.r, dimImage.color.g, dimImage.color.b, 0.75f);
                    dimText.color = new Color(dimText.color.r, dimText.color.g, dimText.color.b, 0.75f);
                }
                if (dimImage2 != null) {
                    dimImage2.color = new Color(dimImage2.color.r, dimImage2.color.g, dimImage2.color.b, 0.75f);
                    dimText.color = new Color(dimText.color.r, dimText.color.g, dimText.color.b, 0.75f);
                }
            }
            return;
        } else {
            try {
                if (completelyTransparent || GetComponent<Button>().interactable && deltaInteractable != GetComponent<Button>().interactable) {
                    transform.GetChild(0).GetComponent<Text>().color = Color.white;

                }
            } catch (Exception e) {
                //Debug.LogWarning(e);
            }
                        
        }
        deltaInteractable = GetComponent<Button>().interactable;
    }
}
