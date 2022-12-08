using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//for the map editor; therefore, will only start assigning dropdown values once
public class DropdownFilter : MonoBehaviour {
    public InputField inputField;
    private Dropdown dropdown;
    void Start() {
        dropdown = GetComponent<Dropdown>();
    }
    public void FilterDropdown(string input) {
        if (input == "")
            return;
        int moveIndex = -1; //if -1 don't move the scrollbar
        for (int i = 0; i < dropdown.options.Count; i++) {
            if (dropdown.options[i].text.Length >= input.Length && dropdown.options[i].text.Substring(0, input.Length).ToLower() == input.ToLower()) {
                moveIndex = i;
                break;
            }
        }
        if (moveIndex == -1) {
            //anything that contains will be used
            for (int i = 0; i < dropdown.options.Count; i++) {
                if (dropdown.options[i].text.ToLower().Contains(input.ToLower())) {
                    moveIndex = i;
                    break;
                }
            }
        }
        if (moveIndex != -1) {
            dropdown.Show();
            foreach (Scrollbar r in GetComponentsInChildren<Scrollbar>()) {
                if (!r.gameObject.activeInHierarchy)
                    continue;
                r.value = 1f - (float)moveIndex / ((float)dropdown.options.Count - 3);
                //dropdown.template.GetComponent<ScrollRect>().verticalScrollbar.onValueChanged.Invoke(0);
            }
        }
    }
    bool deltaFocused = false;
    void Update() {
        if (!inputField.isFocused && inputField.text != "" && deltaFocused) {
            FilterDropdown(inputField.text);
        }
        deltaFocused = inputField.isFocused;
    }
}