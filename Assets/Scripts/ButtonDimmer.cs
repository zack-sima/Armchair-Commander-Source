using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

///this class should be attached to buttons; it allows multiple text/images to be dimmed when a button is pressed down,
///replacing the Unity button interface that only allows one image element to be dimmed
///TODO: THIS IS THE NEW SCRIPT, REPLACE DIMTEXT
[RequireComponent(typeof(Button))]
public class ButtonDimmer : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {
    [SerializeField]
    private bool includeAllChildren; //automatically dim all children of this button (and itself)
    [SerializeField]
    private List<Text> dimTexts;
    [SerializeField]
    private List<Image> dimImages;
    Button self;
    public void OnPointerClick(PointerEventData eventData) {

    }
    public void OnPointerDown(PointerEventData eventData) {
        if (self.interactable) {
            foreach (Text t in dimTexts) {
                if (t)
                    t.color = new Color(t.color.r, t.color.g, t.color.b, 0.7f);
            }
            foreach (Image i in dimImages) {
                if (i)
                    i.color = new Color(i.color.r, i.color.g, i.color.b, 0.7f);
            }
        }
    }
    public void OnPointerEnter(PointerEventData eventData) { }
    public void OnPointerExit(PointerEventData eventData) { }
    public void OnPointerUp(PointerEventData eventData) {
        if (self.interactable) {
            foreach (Text t in dimTexts) {
                if (t)
                    t.color = new Color(t.color.r, t.color.g, t.color.b, 1f);
            }
            foreach (Image i in dimImages) {
                if (i)
                    i.color = new Color(i.color.r, i.color.g, i.color.b, 1f);
            }
        }
    }
    void Start() {
        self = GetComponent<Button>();
        if (includeAllChildren) {
            foreach (Image i in GetComponentsInChildren<Image>()) {
                dimImages.Add(i);
            }
            foreach (Text t in GetComponentsInChildren<Text>()) {
                dimTexts.Add(t);
            }
        }
    }
    bool deltaInteractable = true;
    void Update() {
        if (!self.interactable) {
            if (deltaInteractable) {
                foreach (Text t in dimTexts) {
                    if (t)
                        t.color = new Color(t.color.r, t.color.g, t.color.b, 0.55f);
                }
                foreach (Image i in dimImages) {
                    if (i)
                        i.color = new Color(i.color.r, i.color.g, i.color.b, 0.5f);
                }
            }
        } else if (!deltaInteractable) {
            foreach (Text t in dimTexts) {
                if (t)
                    t.color = new Color(t.color.r, t.color.g, t.color.b, 1f);
            }
            foreach (Image i in dimImages) {
                if (i)
                    i.color = new Color(i.color.r, i.color.g, i.color.b, 1f);
            }
        }
        deltaInteractable = self.interactable;
    }
}
