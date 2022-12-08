using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class City : MonoBehaviour {
    [SerializeField]
    private Sprite greenCity, redCity, portSprite, citySprite, portIcon;
    public GameObject display;
    [HideInInspector]
    public Tile currentTile;
    [SerializeField]
    private Image flagImage, cityImage, factoryImage, airportImage, nuclearImage, strategicImage, cityShield;
    [SerializeField]
    private Sprite shieldSprite, crackedShieldSprite;
    [SerializeField]
    private SpriteRenderer cityVisualImage;
    [SerializeField]
    private GameObject healthDisplay;
    [SerializeField]
    private RectTransform healthBar;
    public Text tierText, factoryTierText, airportTierText, nuclearTierText, nameText, defenceTierText;
    private float maxHealth;
    public int bombInProduction; //0-2
    public int roundsToBombProduction; //>0, reduced in end round and added to country stockpile
    public float health; //only allow enemies entering if health is exactly 0; city treated as a soldier and recalculates half of the damage (counted as fortress) if there is unit on city
    public int tier, factoryTier, airportTier, nuclearTier, defenceTier; //if tier is 0 and editmode enable/disable health bar and move strategic icon
    public bool isStrategic, isPort;
    public string cityName;
    public float CalculateMaxHealth() {
        float maxHealth = -1;
        switch (defenceTier) {
        case 1:
            maxHealth = 75;
            break;
        case 2:
            maxHealth = 175;
            break;
        case 3:
            maxHealth = 300;
            break;
        case 4:
            maxHealth = 500;
            break;
        case 5:
            maxHealth = 800;
            break;
        }
        return maxHealth;
    }
    public void Start() {
        maxHealth = CalculateMaxHealth();
        if (defenceTier > 0) {
            healthDisplay.SetActive(true);
            strategicImage.transform.localPosition = new Vector2(0.25f, 1.42f);
        } else {
            healthDisplay.SetActive(false);
            strategicImage.transform.localPosition = new Vector2(0.25f, 1.133f);
        }
        //if (!currentTile.controller.editMode) {
        display.transform.position = new Vector3(display.transform.position.x, display.transform.position.y, transform.position.y - 0.3f);
        nameText.transform.position = new Vector3(nameText.transform.position.x, nameText.transform.position.y, transform.position.y - 35f);
        //} else {
        //    display.transform.position = new Vector3(display.transform.position.x, display.transform.position.y, transform.position.y - 120f);
        //    nameText.transform.position = new Vector3(nameText.transform.position.x, nameText.transform.position.y, transform.position.y - 125f);
        //}
        if (currentTile.terrain == Terrain.water)
            isPort = true;
        if (isPort) {
            cityImage.sprite = portIcon;
            cityVisualImage.sprite = portSprite;
            airportTier = 0;
            nuclearTier = 0;
            tier = 1;
            tierText.enabled = false;
        } else {
            cityVisualImage.sprite = citySprite;
        }
        if (isPort && defenceTier != 0)
            defenceTier = 0;

        factoryImage.enabled = false;
        airportImage.enabled = false;
        nuclearImage.enabled = false;
        factoryTierText.enabled = false;
        airportTierText.enabled = false;
        nuclearTierText.enabled = false;
        currentTile.isCity = true;
        currentTile.city = this;

        int numberEnabled = (factoryTier >= 1 ? 1 : 0) + (airportTier >= 1 ? 1 : 0) + (nuclearTier >= 1 ? 1 : 0);
        List<int> tiers = new List<int>() { factoryTier, airportTier, nuclearTier };
        List<Transform> tierTransforms = new List<Transform>() { factoryImage.transform, airportImage.transform, nuclearImage.transform };
        List<Text> tierTexts = new List<Text>() { factoryTierText, airportTierText, nuclearTierText };

        float startX = 0.25f - (numberEnabled * 0.221f);
        cityImage.transform.localPosition = new Vector2(startX, cityImage.transform.localPosition.y);
        startX += 0.442f;

        for (int i = 0; i < 3; i++) {
            if (tiers[i] > 0) {
                tierTransforms[i].localPosition = new Vector2(startX, tierTransforms[i].localPosition.y);
                startX += 0.442f;
                tierTransforms[i].GetComponent<Image>().enabled = true;
                tierTexts[i].enabled = true;
            }
        }
        //if (factoryTier >= 1 && airportTier == 0) {
        //    factoryImage.enabled = true;
        //    cityImage.transform.localPosition = new Vector2(0.029f, cityImage.transform.localPosition.y);
        //    factoryTierText.enabled = true;
        //} else if (factoryTier == 0 && airportTier >= 1) {
        //    airportImage.enabled = true;
        //    cityImage.transform.localPosition = new Vector2(0.029f, cityImage.transform.localPosition.y);
        //    airportTierText.enabled = true;
        //} else if (factoryTier >= 0 && airportTier >= 1) {
        //    airportImage.enabled = true;
        //    cityImage.transform.localPosition = new Vector2(-0.183f, cityImage.transform.localPosition.y);
        //    airportTierText.enabled = true;
        //    factoryImage.transform.localPosition = new Vector2(0.25f, factoryImage.transform.localPosition.y);
        //    factoryImage.enabled = true;
        //    airportImage.transform.localPosition = new Vector2(0.691f, airportImage.transform.localPosition.y);
        //    factoryTierText.enabled = true;
        //}

    }
    int currentFactoryTier = -1, currentAirportTier = -1, currentNuclearTier = -1, currentTier = -1, currentDefenceTier = -1;
    bool setFlag = false;

    public void UpdateHealth() {
        if (health <= 0f)
            health = 0f;
        if (health > maxHealth)
            health = maxHealth;
        if (healthBar.localScale != new Vector3(health / maxHealth, 1f, 1f))
            healthBar.localScale = new Vector3(health / maxHealth, 1f, 1f);
        if (health == 0f) {
            cityShield.sprite = crackedShieldSprite;
        } else {
            cityShield.sprite = shieldSprite;
        }
    }
    string country = "";
    float deltaHealth = 0;
    void Update() {
        if (strategicImage.enabled != isStrategic) {
            strategicImage.enabled = isStrategic;
        }
        if (cityName != "") {
            if (MyPlayerPrefs.instance.GetInt("editor") == 1 || nameText.text == "") {
                nameText.text = CustomFunctions.TranslateText(cityName);
            }
            nameText.transform.localScale = new Vector2((Camera.main.orthographicSize + 3f) / 7f * 0.007f, (Camera.main.orthographicSize + 3f) / 7f * 0.0072f);
        } else
            nameText.text = "";

        if (MyPlayerPrefs.instance.GetInt("editor") == 1 || !setFlag || country != currentTile.country) {
            flagImage.sprite = currentTile.controller.flags[currentTile.country];
            country = currentTile.country;
            setFlag = true;
        }
        if (cityName == "\"" && Controller.instance.editMode) {
            cityName = "'";
        }
        if (Controller.instance.editMode) {
            if (defenceTier > 0 && !healthDisplay.activeInHierarchy) {
                healthDisplay.SetActive(true);
                strategicImage.transform.localPosition = new Vector2(0.25f, 1.375f);
            } else if (defenceTier == 0 && healthDisplay.activeInHierarchy) {
                healthDisplay.SetActive(false);
                strategicImage.transform.localPosition = new Vector2(0.25f, 1.133f);
            }
        } else if (defenceTier > 0) {
            if (deltaHealth != health) {
                UpdateHealth();
            }
            if (Controller.instance.countriesIsNeutral.Contains(country)) {
                if (healthBar.GetComponent<Image>().color != new Color(0.8f, 0.8f, 0.8f, 1f))
                    healthBar.GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 1f);
            } else if (Controller.instance.countriesIsAxis[country] != Controller.instance.playerIsAxis) {
                if (healthBar.GetComponent<Image>().color != Color.red)
                    healthBar.GetComponent<Image>().color = Color.red;
            } else if (healthBar.GetComponent<Image>().color != Color.green) {
                healthBar.GetComponent<Image>().color = Color.green;
            }
        }

        if (!isPort) {
            if (currentTile.controller.countriesIsAxis[currentTile.country] == currentTile.controller.playerIsAxis || currentTile.controller.countriesIsNeutral.Contains(currentTile.country)) {
                if (cityImage.sprite != greenCity)
                    cityImage.sprite = greenCity;
            } else {
                if (cityImage.sprite != redCity)
                    cityImage.sprite = redCity;
            }
        }
        if (defenceTier != currentDefenceTier) {
            switch (defenceTier) {
            case 1:
                defenceTierText.text = "I";
                break;
            case 2:
                defenceTierText.text = "II";
                break;
            case 3:
                defenceTierText.text = "III";
                break;
            case 4:
                defenceTierText.text = "IV";
                break;
            case 5:
                defenceTierText.text = "V";
                break;
            }
        }
        if (factoryTier != currentFactoryTier) {
            switch (factoryTier) {
            case 1:
                factoryTierText.text = "I";
                break;
            case 2:
                factoryTierText.text = "II";
                break;
            case 3:
                factoryTierText.text = "III";
                break;
            case 4:
                factoryTierText.text = "IV";
                break;
            case 5:
                factoryTierText.text = "V";
                break;
            }
        }
        if (airportTier != currentAirportTier) {
            switch (airportTier) {
            case 1:
                airportTierText.text = "I";
                break;
            case 2:
                airportTierText.text = "II";
                break;
            case 3:
                airportTierText.text = "III";
                break;
            }
        }
        if (nuclearTier != currentNuclearTier) {
            switch (nuclearTier) {
            case 1:
                nuclearTierText.text = "I";
                break;
            case 2:
                nuclearTierText.text = "II";
                break;
            case 3:
                nuclearTierText.text = "III";
                break;
            }
        }
        if (currentTier != tier) {
            switch (tier) {
            case 1:
                tierText.text = "I";
                break;
            case 2:
                tierText.text = "II";
                break;
            case 3:
                tierText.text = "III";
                break;
            case 4:
                tierText.text = "IV";
                break;
            case 5:
                tierText.text = "V";
                break;
            }
        }
        currentFactoryTier = factoryTier;
        currentDefenceTier = defenceTier;
        currentAirportTier = airportTier;
        currentNuclearTier = nuclearTier;
        currentTier = tier;
        deltaHealth = health;
    }
}