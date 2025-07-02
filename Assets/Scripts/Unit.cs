using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System;
[SerializeField]
public enum Troop { infantry, armor, artillery, fortification, navy }
[SerializeField]
public enum Skin { German, Soviet, American, Japanese, French, British }


[Serializable]
public class TroopData {
	public Skin troopCountry;
	public int movementFuelCost;
	public float maxHealth, damage, armor, armorPierce;
	public Terrain terrainCheck; //just for checking array
	public List<float> terrainAttackBonuses;
	public List<float> terrainDefenceBonuses;


	[Range(-0.5f, 0.5f)]
	public float infantryAttackBonus, armorAttackBonus, artilleryAttackBonus, fortificationAttackBonus, navalAttackBonus;

	public int range, movement;
	//0 = plains
	//1 = forest
	//2 = mountains
	//3 = city
	//4 = water
	//5 = oil
	//6 = desert

}
public abstract class Unit : MonoBehaviour {
	[HideInInspector]
	public Tile previousTile;

	[HideInInspector]
	public Controller controller;
	[HideInInspector]
	public Tile currentTile;
	[HideInInspector]
	public Unit closestTarget; //for ai use only
	[HideInInspector]
	public City closestCity; //CHECK CLOSESTTARGET AND THIS FOR ATTACKS
	[HideInInspector]
	public string country;
	public Troop troopType;
	[HideInInspector]
	public Skin troopSkin;
	[HideInInspector]
	public int troopId;
	[HideInInspector]
	public int isAxis;
	[HideInInspector]
	public bool isNeutral, visible, isStrategic, isLocked;

	public Canvas healthDisplayCanvas;
	[HideInInspector]
	public Image flag;
	[HideInInspector]
	public Image veterencyImage, stackedImage;

	[HideInInspector]
	public string general, defaultGeneral;


	public int generalLevel;
	TransportCarrierAnimator carrier;
	[HideInInspector]
	public bool selected, enemySelected; //for info only (enemy selected here to avoid bugs)

	[HideInInspector]
	public int veterency, veterencyLevel, tier; //stacked number

	[HideInInspector]
	public RectTransform healthDisplayBar;
	[HideInInspector]
	public bool moved, attacked, supplied, moving, canAttack, encircled, doubleEncircled;
	Image encircledImage;
	Image noFuelImage;
	Image strategicImage, lockedImage;
	SpriteRenderer generalImage, shipIconImage;
	int originalRange;

	int originalMovement;
	GameObject damageTextPrefab;




	[HideInInspector]
	public float health, originalArmor, originalArmorPierce;
	public List<TroopData> customCountryValues;
	//replace these inside the class if needed by country
	public int movementFuelCost;
	public float maxHealth, damage, armor, armorPierce;
	float originalMaxHealth;
	public Terrain terrainCheck; //just for checking array
	public List<float> terrainAttackBonuses;
	public List<float> terrainDefenceBonuses;
	public int range, movement;
	[Range(0f, 1f)]
	public float infantryAttackBonus, armorAttackBonus, artilleryAttackBonus, fortificationAttackBonus, navalAttackBonus;

	public abstract void toggleVisibility();
	public abstract void updateSkin();
	public abstract void select();
	public abstract void deselect();
	public abstract void animateAttack(float delay, Tile target);

	public abstract void MoveSound();

	private void CheckNukes(ref float damage, ref bool collat, ref bool isNuke, bool player, Tile target) {
		if (player) {
			if (controller.nuclearWarheadDropdown.value > 0) {
				switch (controller.nuclearWarheadDropdown.value) {
					case 1:
						damage = 500 * UnityEngine.Random.Range(0.9f, 1.1f);
						collat = true;
						isNuke = true;
						break;
					case 2:
						damage = 1000 * UnityEngine.Random.Range(0.9f, 1.1f); //add collateral damage
						collat = true;
						isNuke = true;
						break;
				}
				controller.countryDatas[controller.playerCountry].nukes[controller.nuclearWarheadDropdown.value - 1]--;
				//controller.nuclearWarheadDropdown.value = 0;
				controller.incomingNuclearWarhead = true;
			}
		} else {
			float health = 0;
			try {
				health = target.occupant != null && target.city != null && target.city.health > 0 ? Mathf.Max(target.city.health, target.occupant.health) : (target.isCity ? target.city.health : target.occupant.health);
			} catch { print("calculation error"); }
			if (controller.usedNukes && controller.countryDatas[currentTile.country].nukes[0] > 0 && health > 200f) {
				damage = 500 * UnityEngine.Random.Range(0.9f, 1.1f);
				collat = true;
				isNuke = true;
				controller.countryDatas[currentTile.country].nukes[0]--;
				print("AI use nuke on " + target.coordinate.x + ", " + target.coordinate.y);
			} else if (controller.usedNukes && controller.countryDatas[currentTile.country].nukes[1] > 0 && health > 350f) {
				damage = 1000 * UnityEngine.Random.Range(0.9f, 1.1f); //add collateral damage
				collat = true;
				isNuke = true;
				controller.countryDatas[currentTile.country].nukes[1]--;
				print("AI use nuke on " + target.coordinate.x + ", " + target.coordinate.y);
			}
		}
	}
	public void attack(float delay, float damage, Tile target, bool animate, bool isCritical = false) {
		bool collat = false, isNuke = false;
		if (troopId == 20 && currentTile.country == controller.playerCountry) {
			CheckNukes(ref damage, ref collat, ref isNuke, true, target);
		} else if (troopId == 20 && controller.usedNukes && (controller.countryDatas[currentTile.country].nukes[0] > 0 || controller.countryDatas[currentTile.country].nukes[1] > 0)) {
			CheckNukes(ref damage, ref collat, ref isNuke, false, target);
		}
		//for nuke:
		if (collat) {
			foreach (Tile t in target.neighbors) {
				//print("fallout on " + t.coordinate.x + "," + t.coordinate.y + " neighbor of " + target.coordinate.x + "-" + target.coordinate.y);
				StartCoroutine(EnableFallout(animate ? delay + 1.2f : 0f, t, 2));
				if (t.occupant != null) {
					attack(delay, damage * UnityEngine.Random.Range(0.1f, 0.2f), t.occupant, animate, true, true);
				} else if (t.isCity && t.city.health > 0) {
					attack(delay, damage * UnityEngine.Random.Range(0.1f, 0.2f), t.city, animate, true);
				}
			}
		}
		//print("fallout on " + target.coordinate.x + "," + target.coordinate.y);
		if (isNuke)
			StartCoroutine(EnableFallout(animate ? delay + 1.2f : 0, target, 3));
		if (target.occupant != null) {
			attack(delay, damage, target.occupant, animate, false, isNuke, isCritical: isCritical);
		} else if (target.isCity) {
			attack(delay, damage, target.city, animate, false, isCritical: isCritical);
		} else {
			Debug.LogWarning("attack failed");
		}
	}
	private void attack(float delay, float damage, Unit target, bool animate, bool isCollat = false, bool isNuke = false, bool isCritical = false) {
		if (!isCollat)
			RotateToTarget(target.currentTile);
		if (troopId == 14) {
			controller.airplaneType = 1;
			if (!isCollat)
				damage *= 1f - controller.CheckAirDefences(currentTile, target.currentTile, animate);
		} else if (troopId == 20) {
			controller.airplaneType = 4; //missile
			if (!isCollat)
				damage *= 1f - controller.CheckAirDefences(currentTile, target.currentTile, animate);
		}
		if (target.currentTile.isCity && target.currentTile.city.health > 0f && target.currentTile.city.defenceTier > 0) {
			StartCoroutine(doDamage(animate ? 1.2f : 0f, isNuke ? damage * 0.7f : controller.CalculateCityDamage(this, 0.7f), target.currentTile.city, animate, false, isCritical: isCritical));
			damage *= 0.5f; //cut down damage if attacking both
		}
		if (animate) {
			if (!isCollat) {
				if (currentTile.terrain == Terrain.water && troopType != Troop.navy) {
					StartCoroutine(carrier.shootSmg(0, delay));
					StartCoroutine(CarrierImpact(target.currentTile));
				} else {
					animateAttack(delay, target.currentTile);
				}
			}
			StartCoroutine(doDamage(delay + 1.2f, damage, target, true, isCritical: isCritical));
		} else {
			StartCoroutine(doDamage(0f, damage, target, false, isCritical: isCritical));
		}
	}
	IEnumerator EnableFallout(float delay, Tile t, int falloutAmount) {
		for (float i = 0; i < delay; i += Time.deltaTime) {
			yield return null;
		}
		t.radiationLeft = falloutAmount;
	}
	private void attack(float delay, float damage, City target, bool animate, bool isCollat = false, bool isCritical = false) {
		RotateToTarget(target.currentTile);
		if (troopId == 14) {
			controller.airplaneType = 1;
			damage *= 1f - controller.CheckAirDefences(currentTile, target.currentTile, animate);
		} else if (troopId == 20) {
			controller.airplaneType = 4; //missile

			damage *= 1f - controller.CheckAirDefences(currentTile, target.currentTile, animate);
		}
		if (animate) {
			if (!isCollat) {
				if (currentTile.terrain == Terrain.water && troopType != Troop.navy) {
					StartCoroutine(carrier.shootSmg(0, delay));
					StartCoroutine(CarrierImpact(target.currentTile));
				} else {
					animateAttack(delay, target.currentTile);
				}
			}
			StartCoroutine(doDamage(delay + 1.2f, damage, target, true, isCritical: isCritical));
		} else {
			StartCoroutine(doDamage(0f, damage, target, false, isCritical: isCritical));
		}
	}
	IEnumerator CarrierImpact(Tile target) {
		Vector3 targetPosition = target.transform.position;

		for (float i = 0f; i < 1.2f; i += Time.deltaTime)
			yield return null;

		Instantiate(controller.bulletExplosionPrefab, new Vector3(targetPosition.x, targetPosition.y, targetPosition.z - 2f), Quaternion.identity);

	}
	IEnumerator doDamage(float delay, float d, City target, bool animate, bool onlyCity = true, bool isCritical = false) {
		controller.cantSelectTile = true;
		for (float i = 0f; i < delay; i += Time.deltaTime)
			yield return null;
		if (target != null) {
			target.health -= d;
			if (general != "" && PlayerData.instance.generals[general].ContainsPerk(General.GeneralPerk.Training)) {
				veterency += (int)(d * 0.7f * (1f + GeneralManagerPopup.GeneralPerksCalculation(General.GeneralPerk.Training, generalLevel)));
			} else {
				veterency += (int)(d * 0.7f);
			}
		}
		if (!onlyCity) {
			for (float i = 0f; i < 0.75f; i += Time.deltaTime)
				yield return null;
		}
		if (target != null) {
			if (animate) {
				GameObject insItem = Instantiate(damageTextPrefab, new Vector3(target.transform.position.x, target.transform.position.y, -300f), Quaternion.identity);
				insItem.GetComponent<Text>().text = "-" + (int)d;
				if (isCritical) insItem.GetComponent<Text>().color = Color.red;
			}
		}
		controller.cantSelectTile = false;
	}
	IEnumerator doDamage(float delay, float d, Unit target, bool animate, bool isCritical = false) {
		controller.cantSelectTile = true;
		for (float i = 0f; i < delay; i += Time.deltaTime)
			yield return null;
		if (target != null) {
			target.health -= d;
			if (animate) {
				GameObject insItem = Instantiate(damageTextPrefab, new Vector3(target.transform.position.x, target.transform.position.y, -300f), Quaternion.identity);
				insItem.GetComponent<Text>().text = "-" + (int)d;
				if (isCritical) insItem.GetComponent<Text>().color = Color.red;
			}
			if (target.health <= 0f && target.currentTile != null) {
				bool s = target.isStrategic;
				int i = target.isAxis;

				string c = target.country;
				Tile t = target.currentTile;
				controller.soldiers.Remove(target);
				Destroy(target.gameObject);
				controller.calculatePlayerPopulation();

				controller.CheckCountryCapitulate(c, country);


				if (s) {
					if (i == controller.playerIsAxis) {
						controller.endGamePopup(false);
					} else {
						controller.checkVictoryConditions(true);
					}
				}
				t.resetTilePathfinding(false);
				t.canBeAttacked = false;
				t.occupant = null;
				t.GetComponentInChildren<SpriteRenderer>().color = t.defaultTileColor;
				StartCoroutine(delayCheck(t, false));
			}
			if (general != "" && PlayerData.instance.generals[general].ContainsPerk(General.GeneralPerk.Training)) {
				veterency += (int)(d * (1f + GeneralManagerPopup.GeneralPerksCalculation(General.GeneralPerk.Training, generalLevel)));
			} else {
				veterency += (int)d;
			}


		}
		controller.cantSelectTile = false;
	}
	public IEnumerator delayCheck(Tile c, bool destroy) {
		yield return null;
		yield return null;

		foreach (Collider2D i in Physics2D.OverlapCircleAll(currentTile.transform.position, 15f)) {
			if (i.GetComponent<Tile>() != null && i.GetComponent<Tile>().occupant != null && i.GetComponent<Tile>().occupant.country == controller.playerCountry) {
				i.GetComponent<Tile>().occupant.checkAttack(true);
			}
		}
		yield return null;
		controller.cantSelectTile = false;
		if (destroy) {
			Destroy(gameObject);
		}
	}
	public abstract void updateLayering();
	public void updateBoat() {
		shipIconImage.transform.parent.position = new Vector3(generalImage.enabled ? shipIconImage.transform.parent.parent.position.x - 0.37f : shipIconImage.transform.parent.parent.position.x, shipIconImage.transform.parent.parent.position.y + 0.7f, shipIconImage.transform.parent.parent.position.z - 0.15f);
		//shipIconImage.flipX = flippedHorizontal;

		shipIconImage.transform.position = new Vector3(shipIconImage.transform.position.x, shipIconImage.transform.position.y, shipIconImage.transform.parent.position.z - 0.01f);

		if (troopType == Troop.infantry) {
			shipIconImage.sprite = controller.infantryIcon;
		} else if (troopType == Troop.armor)
			shipIconImage.sprite = controller.armorIcon;
		else if (troopType == Troop.artillery)
			shipIconImage.sprite = controller.artilleryIcon;
	}

	public void passRound() {
		moved = false;
		attacked = false;
		supplied = false;
	}



	public void updateCountry() {
		country = currentTile.country;
		flag.sprite = controller.flags[country];
		if (controller.countriesIsNeutral.Contains(country)) {
			isNeutral = true;
		}
		if (isNeutral) {
			healthDisplayBar.GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 1f);
		}
		Unit soldier = controller.spawnSoldier(currentTile.coordinate, country, isAxis, troopId, veterency, tier, true);
		soldier.updateSkin();
		Destroy(gameObject);
	}

	public void UpdateStats(bool outsideGame = false, SoldierPrefabsManager manager = null, int techLevel = 0) {
		int skinIsAxis;
		if (!outsideGame) {
			skinIsAxis = controller.originalCountriesIsAxis[country]; //might be different because of custom alliances
			maxHealth += controller.CheckCountryTech(country, (Controller.TechTroopType)(int)troopType, Controller.TechCategory.Health);
			damage += controller.CheckCountryTech(country, (Controller.TechTroopType)(int)troopType, Controller.TechCategory.Attack);
			armor += controller.CheckCountryTech(country, (Controller.TechTroopType)(int)troopType, Controller.TechCategory.Armor);
			armorPierce += controller.CheckCountryTech(country, (Controller.TechTroopType)(int)troopType, Controller.TechCategory.Antiarmor);
			movement += controller.CheckCountryTech(country, (Controller.TechTroopType)(int)troopType, Controller.TechCategory.Movement);
			movementFuelCost -= controller.CheckCountryTech(country, (Controller.TechTroopType)(int)troopType, Controller.TechCategory.Fuel);
		} else {
			//not in game, so destroy health canvas (almanac only)
			Destroy(healthDisplayCanvas.gameObject);
			skinIsAxis = CustomFunctions.CountriesIsAxis[country];

			maxHealth += Controller.CheckTech((Controller.TechTroopType)(int)troopType, techLevel, Controller.TechCategory.Health);
			damage += Controller.CheckTech((Controller.TechTroopType)(int)troopType, techLevel, Controller.TechCategory.Attack);
			armor += Controller.CheckTech((Controller.TechTroopType)(int)troopType, techLevel, Controller.TechCategory.Armor);
			armorPierce += Controller.CheckTech((Controller.TechTroopType)(int)troopType, techLevel, Controller.TechCategory.Antiarmor);
			movement += Controller.CheckTech((Controller.TechTroopType)(int)troopType, techLevel, Controller.TechCategory.Movement);
			movementFuelCost -= Controller.CheckTech((Controller.TechTroopType)(int)troopType, techLevel, Controller.TechCategory.Fuel);

		}
		//*****TROOPSKIN DEFINING MOVED TO CUSTOM (class)
		troopSkin = CustomFunctions.DetermineTroopSkin(country, skinIsAxis);
		foreach (TroopData d in customCountryValues) {
			if (d.troopCountry == troopSkin) {
				movementFuelCost += d.movementFuelCost;
				maxHealth += d.maxHealth;
				damage += d.damage;
				armor += d.armor;
				armorPierce += d.armorPierce;
				//only assign if needed (check empty)
				if (d.terrainAttackBonuses.Count > 1)
					terrainAttackBonuses = d.terrainAttackBonuses;
				if (d.terrainDefenceBonuses.Count > 1)
					terrainDefenceBonuses = d.terrainDefenceBonuses;
				movement += d.movement;

				infantryAttackBonus += d.infantryAttackBonus;
				armorAttackBonus += d.armorAttackBonus;
				artilleryAttackBonus += d.artilleryAttackBonus;
				fortificationAttackBonus += d.fortificationAttackBonus;
				navalAttackBonus += d.navalAttackBonus;
				break;
			}
		}
	}
	private GameObject animatorParent; //controls the flipping of units
	private void SetupAnimatorParent() {
		animatorParent = new GameObject();
		//put everything that isn't healthcanvas into animatorParent
		animatorParent.transform.SetParent(transform);
		animatorParent.name = "AnimatorParent";

		animatorParent.transform.localPosition = Vector3.zero;
		animatorParent.transform.localRotation = transform.rotation;

		foreach (Transform t in transform.GetComponentsInChildren<Transform>()) {
			if (t == null)
				continue;

			//TODO: NOTE THAT ALL TROOPS SHOULD HAVE ONE OF THE ANIMATORS
			if (t.GetComponent<TankAnimator>() || t.GetComponent<SoldierAnimator>() || t.GetComponent<TransportCarrierAnimator>()) {
				t.SetParent(animatorParent.transform, true);
			}
		}
	}
	protected void Start() {
		// print(troopId);
		while (terrainAttackBonuses.Count < 12) {
			if (terrainAttackBonuses.Count == 8) {
				//add village (same as plains)
				terrainAttackBonuses.Add(terrainAttackBonuses[(int)Terrain.plains]);

			} else if (terrainAttackBonuses.Count == 9) {
				//add industrial complex (same as city)
				terrainAttackBonuses.Add(terrainAttackBonuses[(int)Terrain.city]);
			} else {
				terrainAttackBonuses.Add(0f);
			}
		}
		while (terrainDefenceBonuses.Count < 12) {
			if (terrainDefenceBonuses.Count == 8) {
				//add village (same as plains)
				terrainDefenceBonuses.Add(terrainDefenceBonuses[(int)Terrain.plains]);

			} else if (terrainDefenceBonuses.Count == 9) {
				//add industrial complex (same as city)
				terrainDefenceBonuses.Add(terrainDefenceBonuses[(int)Terrain.city]);
			} else {
				terrainDefenceBonuses.Add(0f);
			}
		}
		StartCoroutine(MyStart());
	}
	IEnumerator MyStart() {
		while (!controller.started)
			yield return null;
		ActualStart();
	}
	Vector3 strategicLocation = new Vector3(0.253f, 0.61f, -0.52f), lockedLocation = new Vector3(0.253f, 0.31f, -0.52f);
	void ActualStart() {
		originalMaxHealth = maxHealth;
		if (maxHealth == 0) //prevent issue
			maxHealth = 1;

		if (controller.countriesIsNeutral.Contains(country)) {
			isNeutral = true;
		}
		flag = healthDisplayCanvas.transform.GetChild(1).GetComponent<Image>();
		healthDisplayBar = healthDisplayCanvas.transform.GetChild(3).GetComponent<RectTransform>();
		veterencyImage = healthDisplayCanvas.transform.GetChild(4).GetComponent<Image>();
		veterencyImage.enabled = false;
		stackedImage = healthDisplayCanvas.transform.GetChild(0).GetComponent<Image>();

		shipIconImage = Instantiate(controller.shipUnitTypePrefab, healthDisplayCanvas.transform.position, Quaternion.identity).transform.GetChild(0).GetComponent<SpriteRenderer>();
		shipIconImage.transform.parent.SetParent(healthDisplayCanvas.transform.parent);

		shipIconImage.transform.parent.gameObject.SetActive(false);

		generalImage = Instantiate(controller.generalIconPrefab, healthDisplayCanvas.transform.position, transform.rotation).GetComponent<SpriteRenderer>();
		generalImage.transform.SetParent(healthDisplayCanvas.transform.parent);
		generalImage.transform.localPosition = new Vector3(0f, 0.35f, -0.29f);

		generalImage.size = new Vector2(3.5f, 4.5f);
		//generalImage.flipX = flippedHorizontal; //affects only saves

		lockedImage = Instantiate(controller.lockedPrefab, healthDisplayCanvas.transform.position, Quaternion.identity).GetComponent<Image>();
		lockedImage.transform.SetParent(healthDisplayCanvas.transform);
		lockedImage.transform.localPosition = lockedLocation;

		lockedImage.enabled = false;

		strategicImage = Instantiate(controller.strategicPrefab, healthDisplayCanvas.transform.position, Quaternion.identity).GetComponent<Image>();
		strategicImage.transform.SetParent(healthDisplayCanvas.transform);
		strategicImage.transform.localPosition = strategicLocation;

		strategicImage.enabled = false;

		updateGeneral(false, controller.editMode);

		StartCoroutine(deltaStart());
		originalArmor = armor;
		originalArmorPierce = armorPierce;
		originalMovement = movement;
		updateSkin();
		updateLayering();
		stackedImage.enabled = false;
		flag.sprite = controller.flags[country];
		if (isNeutral) {
			healthDisplayBar.GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 1f);
		}
		if (transform.position.z != transform.position.y)
			transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
		if (controller.editMode) {
			healthDisplayCanvas.transform.position = new Vector3(transform.position.x - 0.25f, transform.position.y - 0.25f, transform.position.z - 0.25f);
			checkVeterency();
			return;
		}
		damageTextPrefab = controller.damageTextPrefab;

		carrier = Instantiate(controller.transportCarrierPrefab, transform.position, Quaternion.identity).GetComponent<TransportCarrierAnimator>(); ;
		carrier.transform.SetParent(transform);
		carrier.GetComponent<SpriteRenderer>().enabled = false;

		originalRange = range;
		if ((int)health == 0)
			health = maxHealth;
		noFuelImage = Instantiate(controller.noOilPrefab, healthDisplayCanvas.transform.position, Quaternion.identity).GetComponent<Image>();
		noFuelImage.transform.SetParent(healthDisplayCanvas.transform);
		noFuelImage.transform.localPosition = new Vector2(0.45f, 0.39f);
		noFuelImage.enabled = false;
		encircledImage = Instantiate(controller.warningPrefab, healthDisplayCanvas.transform.position, Quaternion.identity).GetComponent<Image>();
		encircledImage.transform.SetParent(healthDisplayCanvas.transform);
		encircledImage.transform.localPosition = new Vector2(0f, 0.39f);
		encircledImage.enabled = false;
		if (isNeutral) {
			healthDisplayBar.GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 1f);
		} else if (controller.countriesIsAxis[country] != controller.playerIsAxis) {
			healthDisplayBar.GetComponent<Image>().color = Color.red;
		}
		SetupAnimatorParent();
	}
	//when a country's alliance is changed in game (use updateCountry for map editing)
	public void CheckCountry() {
		if (controller.countriesIsNeutral.Contains(country)) {
			isNeutral = true;
		}
		isAxis = controller.countriesIsAxis[country];
		isNeutral = controller.countriesIsNeutral.Contains(country);
		checkAttack(true);
		if (isNeutral) {
			healthDisplayBar.GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 1f);
		} else if (controller.countriesIsAxis[country] != controller.playerIsAxis) {
			healthDisplayBar.GetComponent<Image>().color = Color.red;
		} else {
			healthDisplayBar.GetComponent<Image>().color = Color.green;

		}
	}
	public void updateStack() {
		if (controller.editMode) {
			setVeterencyLevel(veterencyLevel);
			checkVeterency();
		}
		if (tier > 1 && troopType != Troop.fortification) {
			stackedImage.enabled = true;
			stackedImage.sprite = controller.stackedSprites[tier - 2];
		} else
			stackedImage.enabled = false;
	}
	IEnumerator deltaStart() {
		yield return null;
		updateStack();
		updateBoat();
		yield return null;
		yield return null;
		if (currentTile == null && !controller.editMode) {
			Destroy(gameObject);
		}
		visible = true;
	}
	bool deltaMoved = false;

	public void updateGeneral(bool newAssignment, bool editMode = false) {
		if (general != "" && general != "default" && controller.playerData.generals.ContainsKey(general)) {
			if (originalMaxHealth == maxHealth && !editMode) {
				//  print("old: " + maxHealth);
				float healthPercentage = health / maxHealth;
				maxHealth = originalMaxHealth * (1f + controller.playerData.generals[general].healthBonus[generalLevel] / 100f);
				if (newAssignment)
					health = maxHealth * healthPercentage;
				//  print("new: " + maxHealth);
			}
			generalImage.enabled = true;
			generalImage.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
			generalImage.sprite = controller.playerData.generalPhotos[general];
			if (general != defaultGeneral) {
				//print(defaultGeneral);
				defaultGeneral = ""; // get rid of default general
			}
		} else {
			generalImage.enabled = false;
			generalImage.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
			general = "";

			if (maxHealth != originalMaxHealth && !editMode) {
				//  print("before sacking: " + maxHealth);
				float healthPercentage = health / maxHealth;
				maxHealth = originalMaxHealth;
				health = maxHealth * healthPercentage;
				// print("after sacking:  " + maxHealth);
			}
		}
		updateBoat();
	}
	string deltaGeneral = "";

	protected void Update() {
		if (!controller.editMode)
			flippedHorizontal = animatorParent.transform.rotation != Quaternion.identity;
		//NOTE: "DEFAULT" GENERAL WILL BE CHANGED TO EMPTY BECAUSE OTHERWISE THERE ARE MULTIPLE CHECKS FOR NO GENERAL
		if (general == "default") {
			general = "";
			if (generalLevel != 0)
				generalLevel = 0;
		} else if (general != "" && general != "default" && generalLevel >= controller.playerData.generals[general].movement.Length)
			generalLevel = controller.playerData.generals[general].movement.Length - 1;
		deltaMoved = moved;


		if (general != deltaGeneral && deltaGeneral != "") {
			veterency = (int)(veterency * 0.52f);
		}
		if (isStrategic && strategicImage.transform.rotation != Quaternion.identity) {
			strategicImage.transform.rotation = Quaternion.identity;
		}


		if (generalImage.enabled) {
			generalImage.transform.localScale = new Vector2((Camera.main.orthographicSize + 3f) / 5f * 0.15f, (Camera.main.orthographicSize + 3f) / 5f * 0.15f);

			if (!controller.editMode)
				generalImage.transform.position = new Vector3(generalImage.transform.position.x, generalImage.transform.parent.position.y + ((currentTile.upperLeft != null || currentTile.upperRight != null) ? 0.35f : -0.15f) + (Camera.main.orthographicSize + 3f) / 5f * 0.52f, generalImage.transform.parent.position.z - 0.29f);
			else {
				generalImage.transform.position = new Vector3(generalImage.transform.position.x, generalImage.transform.parent.position.y + 0.35f + (Camera.main.orthographicSize + 3f) / 5f * 0.52f, generalImage.transform.parent.position.z - 0.29f);

			}
			if (controller.selectedTile != null && controller.selectedTile == currentTile.top ||
				controller.selectedSoldier != null && controller.selectedSoldier.currentTile == currentTile.top ||
				controller.selectedEnemySoldier != null && controller.selectedEnemySoldier.currentTile == currentTile.top) {
				generalImage.color = new Color(1f, 2f, 3f, 0.52f);
			} else {
				generalImage.color = new Color(1f, 2f, 1f, 0.85f);
			}
		}


		if (isStrategic) {
			if (!strategicImage.enabled) {
				strategicImage.enabled = true;
			}
		} else if (strategicImage.enabled) {
			strategicImage.enabled = false;
		}
		if (isLocked && controller.editMode) {
			if (!lockedImage.enabled) {
				lockedImage.enabled = true;
			}
		} else if (lockedImage.enabled) {
			lockedImage.enabled = false;
		}
		if (controller.editMode) {
			if (troopType == Troop.fortification && tier != 1) {
				tier = 1;
				updateStack();
			}
			return;
		}
		if (troopType == Troop.fortification) {
			moved = true;
			if (tier > 1)
				attacked = true;
			if (!deltaMoved)
				checkAttack(true);
		}
		checkEncirclement();
		if (movementFuelCost != 0)
			checkFuel();

		if (currentTile != null) {
			if (currentTile.terrain == Terrain.water && troopType != Troop.navy) {
				movement = 12;
				if (general != "" && general != "default")
					movement += controller.playerData.generals[general].movement[generalLevel];
				armorPierce = 0;
				armor = 0;
				range = 1;
				if (!carrier.GetComponent<SpriteRenderer>().enabled) {
					carrier.GetComponent<SpriteRenderer>().enabled = true;
				}
				if (!shipIconImage.transform.parent.gameObject.activeSelf) {
					shipIconImage.transform.parent.gameObject.SetActive(true);
					updateBoat();
				}

				if (visible)
					toggleVisibility();
			} else {
				movement = originalMovement;
				if (general != "" && general != "default") {
					int length = controller.playerData.generals[general].movement.Length;
					if (generalLevel >= length)
						generalLevel = length - 1;
					movement += controller.playerData.generals[general].movement[generalLevel];
				}
				armorPierce = originalArmorPierce;
				armor = originalArmor;
				range = originalRange;
				if (carrier.GetComponent<SpriteRenderer>().enabled) {
					carrier.GetComponent<SpriteRenderer>().enabled = false;
					shipIconImage.transform.parent.gameObject.SetActive(false);
					updateBoat();
				}
				if (!visible)
					toggleVisibility();
			}
		}
		if (currentTile == null)
			return;


		//update layering
		if (healthDisplayCanvas.transform.position != new Vector3(transform.position.x - 0.25f, transform.position.y - 0.25f, transform.position.z - 0.25f)) {
			healthDisplayCanvas.transform.position = new Vector3(transform.position.x - 0.25f, transform.position.y - 0.25f, transform.position.z - 0.25f);
		}
		//if (healthDisplayCanvas.transform.rotation != Quaternion.identity)
		//    healthDisplayCanvas.transform.rotation = Quaternion.identity;
		if (health < 0) health = 0;
		if (healthDisplayBar.localScale != new Vector3(health / maxHealth, 1f, 1f))
			healthDisplayBar.localScale = new Vector3(health / maxHealth, 1f, 1f);
		deltaGeneral = general;
		checkVeterency();
	}
	void checkFuel() {
		if ((!controller.justMoved || moved) && controller.countryDatas[country].fuel < movementFuelCost * tier) {
			if (!moved && controller.playerCountry == country) {
				checkAttack(true);
				moved = true;
			} else {
				moved = true;
			}
			if (controller.playerCountry == country) {
				noFuelImage.enabled = true;
			} else
				noFuelImage.enabled = false;
		} else
			noFuelImage.enabled = false;
	}
	void checkEncirclement() {
		if (troopType != Troop.fortification && !isNeutral && currentTile != null) {
			if (currentTile.lowerLeft != null && currentTile.lowerLeft.occupant != null && !currentTile.lowerLeft.occupant.moving && currentTile.lowerLeft.occupant.isAxis != isAxis && currentTile.upperRight != null && currentTile.upperRight.occupant != null && currentTile.upperRight.occupant.isAxis != isAxis && !currentTile.upperRight.occupant.moving ||
				currentTile.lowerRight != null && currentTile.lowerRight.occupant != null && !currentTile.lowerRight.occupant.moving && currentTile.lowerRight.occupant.isAxis != isAxis && currentTile.upperLeft != null && currentTile.upperLeft.occupant != null && currentTile.upperLeft.occupant.isAxis != isAxis && !currentTile.upperLeft.occupant.moving ||
				currentTile.top != null && currentTile.top.occupant != null && !currentTile.top.occupant.moving && currentTile.top.occupant.isAxis != isAxis && currentTile.bottom != null && currentTile.bottom.occupant != null && currentTile.bottom.occupant.isAxis != isAxis && !currentTile.bottom.occupant.moving) {
				if (currentTile.lowerLeft != null && currentTile.lowerLeft.occupant != null && !currentTile.lowerLeft.occupant.moving && currentTile.lowerLeft.occupant.isAxis != isAxis && currentTile.upperRight != null && currentTile.upperRight.occupant != null && currentTile.upperRight.occupant.isAxis != isAxis && !currentTile.upperRight.occupant.moving &&
					currentTile.lowerRight != null && currentTile.lowerRight.occupant != null && !currentTile.lowerRight.occupant.moving && currentTile.lowerRight.occupant.isAxis != isAxis && currentTile.upperLeft != null && currentTile.upperLeft.occupant != null && currentTile.upperLeft.occupant.isAxis != isAxis && !currentTile.upperLeft.occupant.moving &&
					currentTile.top != null && currentTile.top.occupant != null && !currentTile.top.occupant.moving && currentTile.top.occupant.isAxis != isAxis && currentTile.bottom != null && currentTile.bottom.occupant != null && currentTile.bottom.occupant.isAxis != isAxis && !currentTile.bottom.occupant.moving) {
					doubleEncircled = true;
					encircled = false;
				} else {
					doubleEncircled = false;
					encircled = true;
				}
			} else
				encircled = false;
			if (encircled && !moving) {
				encircledImage.enabled = true;
				encircledImage.sprite = controller.warningImage;
			} else if (doubleEncircled && !moving) {
				encircledImage.enabled = true;
				encircledImage.sprite = controller.doubleWarningImage;
			} else
				encircledImage.enabled = false;
		} else
			encircledImage.enabled = false;
	}
	public void checkVeterency() {
		if (veterency < 100 * (1 + (tier - 1) * 0.5f)) {
			veterencyLevel = 0;
			veterencyImage.enabled = false;
			veterencyImage.sprite = null;
		} else {
			veterencyImage.enabled = true;
			if (veterency < 250 * (1 + (tier - 1) * 0.5f)) {
				veterencyLevel = 1;
				if (veterencyImage.sprite != controller.veterencySprites[0])
					veterencyImage.sprite = controller.veterencySprites[0];
			} else if (veterency < 450 * (1 + (tier - 1) * 0.5f)) {
				veterencyLevel = 2;
				if (veterencyImage.sprite != controller.veterencySprites[1])
					veterencyImage.sprite = controller.veterencySprites[1];
			} else if (veterency < 700 * (1 + (tier - 1) * 0.5f)) {
				veterencyLevel = 3;
				if (veterencyImage.sprite != controller.veterencySprites[2])
					veterencyImage.sprite = controller.veterencySprites[2];
			} else if (veterency < 1000 * (1 + (tier - 1) * 0.5f)) {
				veterencyLevel = 4;
				if (veterencyImage.sprite != controller.veterencySprites[3])
					veterencyImage.sprite = controller.veterencySprites[3];
			} else if (veterency >= 1000 * (1 + (tier - 1) * 0.5f)) {
				veterencyLevel = 5;
				if (veterencyImage.sprite != controller.veterencySprites[4]) {
					veterencyImage.sprite = controller.veterencySprites[4];
				}
			}
		}
	}
	//edit mode only
	public void setVeterencyLevel(int level) {
		if (level < 0) {
			level = 0;
		} else if (level > 5)
			level = 5;
		switch (level) {
			case 0:
				veterency = 0;
				break;
			case 1:
				veterency = (int)(102 * (1 + (tier - 1) * 0.5f));
				break;
			case 2:
				veterency = (int)(252 * (1 + (tier - 1) * 0.5f));
				break;
			case 3:
				veterency = (int)(452 * (1 + (tier - 1) * 0.5f));
				break;
			case 4:
				veterency = (int)(702 * (1 + (tier - 1) * 0.5f));
				break;
			case 5:
				veterency = (int)(1002 * (1 + (tier - 1) * 0.5f));
				break;
		}
	}
	public bool checkAttack(bool passingRound, bool movingBack = false) {
		if (transform == null || (troopType == Troop.fortification && tier != 1) || isNeutral)
			return false;
		//check for attack availability
		canAttack = false;
		if (!attacked) {
			Collider2D[] c = Physics2D.OverlapCircleAll(transform.position, range + 3f);
			foreach (Collider2D i in c) {
				try {
					Tile t = i.GetComponent<Tile>();

					if (currentTile != null && t != null && t.occupant != null && t.occupant.isAxis != isAxis && !controller.countriesIsNeutral.Contains(t.occupant.country)) {
						if ((t.occupant.troopId != 12 || troopId == 21 || currentTile.terrain == Terrain.water) && (troopId != 12 || t.terrain == Terrain.water) &&
						controller.findDistanceBetweenTiles(t, currentTile) <= (movingBack ? originalRange : range)) {

							closestTarget = t.occupant;
							canAttack = true;
							if (country == controller.playerCountry && !passingRound) {
								t.canBeAttacked = true;
							} else {
								break;
							}
						}
					} else if (currentTile != null && t != null && t.isCity && t.city.tier > 0f && t.city.health > 0f && controller.countriesIsAxis[t.country] != isAxis && !controller.countriesIsNeutral.Contains(t.country)) {
						if (controller.findDistanceBetweenTiles(t, currentTile) <= (movingBack ? originalRange : range)) {
							closestCity = t.city;
							canAttack = true;
							if (country == controller.playerCountry && !passingRound) {
								t.canBeAttacked = true;
							} else {
								break;
							}
						}
					}
				} catch (Exception e) {

					print(e.ToString() + " " + troopId);
				}
			}
		}
		if (canAttack) {
			return true;
		}
		return false;
	}
	List<Tile> changedTiles;
	List<string> changedTilesOriginalCountries;
	int previousFuelCost = 0;
	public void reverseMove() {
		controller.buildButton.position = new Vector3(-1000f, controller.buildButton.position.y, 0f);
		controller.canBuild = false;

		moved = false;
		int index = 0;
		foreach (Tile i in changedTiles) {
			i.country = changedTilesOriginalCountries[index];
			i.updateTileColor();
			index++;
		}
		currentTile.occupant = null;
		currentTile = previousTile;
		previousTile.occupant = this;
		transform.position = new Vector3(previousTile.transform.position.x, previousTile.transform.position.y, previousTile.transform.position.y);
		controller.countryDatas[country].fuel += previousFuelCost;
		controller.selectedCity = null;
		controller.canSupply = false;
		controller.canBuild = false;
		checkAttack(true, true);
		controller.selectedSoldier = null;
	}


	//for paratroopers
	public void teleportToDestination(Tile targetTile) {
		moving = false;
		transform.position = new Vector2(targetTile.transform.position.x, targetTile.transform.position.y);
		checkAttack(true);
		transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
		controller.checkVictoryConditions();
		currentTile.occupant = null;
		currentTile = targetTile;
		currentTile.occupant = this;
		if (controller.countriesIsAxis[targetTile.country] != isAxis && !controller.countriesIsNeutral.Contains(targetTile.country)) {
			targetTile.country = country;
			targetTile.updateTileColor();
		}
		if (moved) {
			attacked = true;
			canAttack = false;
		}
		if (country == controller.playerCountry) {
			//checkSupply();
			controller.selectedSoldier = null;
			controller.selectedCity = null;
			controller.selectedTile = null;
		}
		checkAttack(true);
	}
	//public void checkSupply() {
	//    if (!supplied && currentTile.isCity && (!currentTile.city.isPort || troopType == Troop.navy) && currentTile.country == controller.playerCountry && !encircled && !doubleEncircled &&
	//        health < maxHealth && controller.countryDatas[controller.playerCountry].manpower >= controller.calculateSupplyCost(currentTile)[0] && controller.countryDatas[controller.playerCountry].industry >= controller.calculateSupplyCost(currentTile)[1]) {
	//        controller.canSupply = true;
	//        controller.selectedCity = currentTile.city;
	//    } else {
	//        controller.canSupply = false;
	//    }
	//}
	public void moveToDestination(List<Vector2> points, bool instant, Tile originalTile) {
		cannotReverse = false;
		changedTiles = new List<Tile>();
		changedTilesOriginalCountries = new List<string>();
		if (points == null)
			return;
		moved = true;
		moving = true;
		if (instant) {
			foreach (Vector2 i in points) {
				foreach (Collider2D j in Physics2D.OverlapAreaAll(i, i)) {
					if (i == points[0] && j.GetComponent<Tile>() != null) {
						j.GetComponent<Tile>().occupant = this;
						currentTile = j.GetComponent<Tile>();
					}
					if (j.GetComponent<Tile>() != null && (controller.countriesIsAxis[j.GetComponent<Tile>().country] != isAxis && !controller.countriesIsNeutral.Contains(j.GetComponent<Tile>().country))) {
						MoveCheckTile(j.GetComponent<Tile>());
					}
				}
			}
			moving = false;
			transform.position = points[0];
			checkAttack(true);
			transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
			controller.countryDatas[country].fuel -= movementFuelCost * tier;

		} else {
			MoveSound();
			controller.cantSelectTile = true;
			controller.troopMoving = true;
			StartCoroutine(moveToPoint(points, 0, originalTile));
		}
	}
	void MoveCheckTile(Tile t) {
		string originalCountry = t.country;
		t.country = country;
		t.updateTileColor();
		if (t.isCity && originalCountry != country) {
			controller.CheckCountryCapitulate(originalCountry, country);
			cannotReverse = true;
		}

		if (t.isCity && t.city.isStrategic) {
			controller.checkVictoryConditions();
		}
	}
	[HideInInspector]
	public bool flippedHorizontal = false;
	public void RotateToTarget(Tile target) {
		if (Mathf.Abs(target.transform.position.x - currentTile.transform.position.x) > 0.1f) {
			if (target.transform.position.x < currentTile.transform.position.x) {
				animatorParent.transform.eulerAngles = new Vector3(0f, 180f, 0f);
			} else {
				animatorParent.transform.eulerAngles = new Vector3();
			}
		} else if (target.transform.position.y > currentTile.transform.position.y) {
			animatorParent.transform.eulerAngles = new Vector3();
		} else {
			animatorParent.transform.eulerAngles = new Vector3(0f, 180f, 0f);
		}
		updateBoat();

		updateLayering();
	}

	bool cannotReverse = false;
	IEnumerator moveToPoint(List<Vector2> points, int iteration, Tile originalTile) {
		transform.position = new Vector3(transform.position.x, transform.position.y, points[points.Count - iteration - 1].y - 3f);
		if (Mathf.Abs(points[points.Count - iteration - 1].x - transform.position.x) > 0.1f) {
			if (points[points.Count - iteration - 1].x < transform.position.x) {
				animatorParent.transform.eulerAngles = new Vector3(0f, 180f, 0f);
			} else {
				animatorParent.transform.eulerAngles = new Vector3();
			}
			strategicImage.transform.localPosition = strategicLocation;
			//lockedImage.transform.localPosition = lockedLocation;

			updateBoat();

			updateLayering();
		}
		Vector2 startingPosition = transform.position;

		for (float i = 0f; i < 0.25f; i += Time.deltaTime) {
			transform.position = Vector2.Lerp(startingPosition, points[points.Count - iteration - 1], i / 0.25f);
			transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y - 3f);

			yield return null;
		}
		foreach (Collider2D j in Physics2D.OverlapAreaAll(new Vector2(transform.position.x - 0.2f, transform.position.y - 0.2f), new Vector2(transform.position.x + 0.2f, transform.position.y + 0.2f))) {
			if (j.GetComponent<Tile>() != null && (/*j.GetComponent<Tile>().isCity || */controller.countriesIsAxis[j.GetComponent<Tile>().country] != isAxis && !controller.countriesIsNeutral.Contains(j.GetComponent<Tile>().country))) {
				Tile t = j.GetComponent<Tile>();

				changedTiles.Add(t);
				changedTilesOriginalCountries.Add(t.country);

				MoveCheckTile(t);
			}
		}
		if (iteration < points.Count - 1) {
			StartCoroutine(moveToPoint(points, iteration + 1, originalTile));
		} else {
			controller.troopMoving = false;
			transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
			transform.position = new Vector3(points[0].x, points[0].y, transform.position.z);
			moving = false;
			controller.cantSelectTile = false;

			previousTile = originalTile;
			if (!cannotReverse)
				controller.justMoved = true;
			controller.justMovedUnit = this;

			controller.countryDatas[country].fuel -= movementFuelCost * tier;
			previousFuelCost = movementFuelCost * tier;

			if (currentTile.isCity) {
				controller.selectedCity = currentTile.city;
				if (currentTile.city.airportTier > 0) {
					controller.buildButton.position = new Vector3(Screen.width / 2f, controller.buildButton.position.y, 0f);
					controller.canBuild = true;
				}
			}
			//if (country == controller.playerCountry)
			//    checkSupply();
			//check for attack availability
			if (checkAttack(true)) {
				controller.selectCanAttackTile(false, currentTile);
			} else {
				controller.selectedSoldier = null;
				controller.selectedCity = null;
				controller.selectedTile = null;
			}
		}
	}
}
