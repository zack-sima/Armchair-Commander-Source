using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;
using System;
using System.Linq;

[System.Serializable]
public class CountryData {

	public int manpower;
	public int industry;

	//new
	public int techLevel;

	public List<int> nukes;


	public int fuel;
	public string name;

	public Dictionary<string, int> aiGenerals; //only one assignment for AI, generals are the ones that are placed on troops at the beginning


	public CountryData(string name, int initialManpower, int initialIndustry, int techLevel, int initialFuel, Dictionary<string, int> aiGenerals = null, List<int> nukes = null) {
		this.manpower = initialManpower;
		this.industry = initialIndustry;
		this.name = name;

		this.techLevel = techLevel;

		this.fuel = initialFuel;
		this.aiGenerals = aiGenerals == null ? new Dictionary<string, int>() : new Dictionary<string, int>(aiGenerals);
		this.nukes = nukes == null ? new List<int>() { 0, 0, 0 } : new List<int>(nukes);
		while (this.nukes.Count < 3)
			this.nukes.Add(0);
	}
	public CountryData(CountryData oldData) {
		this.manpower = oldData.manpower;
		this.industry = oldData.industry;
		this.name = oldData.name;
		this.techLevel = oldData.techLevel;
		this.fuel = oldData.fuel;
		this.aiGenerals = oldData.aiGenerals == null ? new Dictionary<string, int>() : new Dictionary<string, int>(oldData.aiGenerals);
		this.nukes = oldData.nukes == null ? new List<int>() { 0, 0, 0 } : new List<int>(oldData.nukes);
		while (this.nukes.Count < 3)
			this.nukes.Add(0);
	}
}

[RequireComponent(typeof(MultiplayerLobby))]
public class Controller : MonoBehaviour {
	public static Controller instance;
	[HideInInspector]
	public PlayerData playerData;
	public GameObject tutorialPrefab;
	public GameObject statsBar;
	[HideInInspector]
	public bool editMode; //this mode is for creating maps
	public bool building;
	[HideInInspector]
	public int aiMoveThreshold, roundLimit, mapSize, victoryCondition; //number of tiles player's troops must be in range of for AI to move
	[HideInInspector]
	public string missionName;
	public int round;
	public int map;
	public int maxGenerals;
	[HideInInspector]
	public bool airplaneMode, inAirStrike; //true if using aerial units
	[HideInInspector]
	public int airplaneType;
	[HideInInspector]
	public bool canBuild, canSupply;
	public Sprite mapSprite, checkmark, saveSprite, loadSprite;
	public ProductBar productBar;
	[HideInInspector]
	public MapData mapData;
	public Text countdownMultiplayerText;
	float countdownMultiplayer = 520;
	public int playerIsAxis;
	public string playerCountry, originalPlayerCountry;
	public GameObject shipUnitTypePrefab;
	public GameObject diplomacyPrefab, checkInfoPrefab;
	public GameObject bulletExplosionPrefab, muzzleAnimationPrefab;
	public Button exitToMenuButton, diplomacyButton; //change text in multiplayer for exit button
	public Sprite infantryIcon, armorIcon, artilleryIcon;
	public RectTransform[] editModeUI, editModeHideUI, editModeEventUI;
	public InputField cityNameInput, countryOverrideInput, manpowerInput, industryInput, fuelInput, nuke1Input, nuke2Input, aiDetectionInput, maxGeneralsInput, roundLimitInput,
		missionNameInput, beginningTextInput, eventTitleInput, eventDescriptionInput, eventTriggerInput;
	public Dropdown editCountryDropdown, countryOverrideDropdown, playerCountryDropdown, techYearDropdown, editCategoryDropdown, countriesIsAxisDropdown, editCategory2Dropdown,
		editCategory3Dropdown, editCategory4Dropdown, victoryConditionDropdown, beginningGeneralDropdown, eventsDropdown, eventTargetCountryDropdown,
		eventTypeDropdown, eventEffectDropdown, eventConditionDropdown, nuclearWarheadDropdown;
	public Text triggerRoundText; //change this text for city capture event to prevent users from being confused thinking it has to be on that round
	[HideInInspector]
	public List<GameEvent> events; //imported in game and used to save events
	public Toggle deleteToggle, moveToggle, diplomacyToggle, overrideTechToggle, reuploadToggle, lessResourcesToggle, showFlagToggle;
	public Toggle autoSkipRoundToggle;
	public Button rateMapButton, manageGeneralsButton, deleteAllButton, pauseButton;
	public Dictionary<string, string> countryCustomNameOverrides, countryCustomFlagOverrides; //TODO: NOTE: THIS IS NEW FEATURE FOR CUSTOM NAMES
	public RectTransform[] UIRectTransforms;
	public RectTransform buildButton, reverseButton, supplyButton, generalAssignButton, infoButton;
	public RectTransform buildMenu;
	public GameObject eventMaster, editorMaster; //activeinhierarchy to true these at start so if they were accidentally turned off in editor they still work
	[HideInInspector]
	public GameObject[] troopPrefabs;
	public GameObject strategicPrefab, lockedPrefab;
	public GameObject cityPrefab, bigExplosionPrefab;
	public GameObject warningPrefab, transportCarrierPrefab, noOilPrefab;
	public GameObject generalIconPrefab;
	public GameObject damageTextPrefab;
	public Sprite warningImage, doubleWarningImage;
	public GameObject loadingCover;
	public AudioSource eventSound, marchingSound, tankSound, truckSound, navySound, nuclearSound;

	public Dictionary<Vector2, Tile> tiles;
	public Dictionary<Vector2, string> originalTiles;
	public List<City> cities;
	public Dictionary<string, Sprite> flags;

	public Dictionary<string, int> countriesIsAxis; //TODO: NOTE: NO LONGER REPRESENTS IS AXIS; NOW USES INTEGER TO STORE SEPARATE TEAMS
	[HideInInspector]
	public Dictionary<string, int> originalCountriesIsAxis;

	public List<string> countriesIsNeutral;
	public Dictionary<string, CountryData> countryDatas;
	public Sprite[] veterencySprites;

	MyPlayerPrefs myPlayerPrefs;

	public string[] flagNames;


	public Sprite[] flagSprites;
	private List<Sprite> customFlags;
	public Sprite[] stackedSprites;
	public List<Unit> soldiers;
	public List<Tile> canAttackTiles, oilRigs, industrialComplexes, villages; //terrains that produce resources like manpower/fuel/in
																			  //[HideInInspector]
	public City selectedCity;
	[HideInInspector]
	public Tile selectedTile, selectedBuildTile;
	//[HideInInspector]
	public Unit selectedSoldier, selectedEnemySoldier;
	public GameObject hex;

	public List<Sprite> eventPhotos;

	public bool passingRound, usedNukes, troopMoving; //player troop moving (no UI should be on)

	//List<int> testIntegers;

	//tile moving will turn this on
	public bool cantSelectTile;
	public Transform[] mapSizeControllers;
	public Vector2[] popupPositions; //based on anchor to center of screen, multiply by screen y
	int[] popupAlignments;
	float[] popupRangeAllowed;
	int[] arrowDirections;
	[HideInInspector]
	public int popupIndex = 0;
	public GameObject tutorialDisplay;
	string[] englishTutorial;
	TutorialPopup p = null;

	//temporary paint size
	int editBrushSize = 1;

	public bool buildButtonOn, supplyButtonOn, reverseButtonOn, generalsButtonOn;


	public bool incomingNuclearWarhead; //make bigger explosion

	Transform mapSizeTransform; //assign for tiles
	public void ContinuePopup() { //tutorial popup
		if (MyPlayerPrefs.instance.GetInt("tutorial") == 1 && popupIndex == 0) {
			Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, 2.5f, Camera.main.transform.position.z);
		}
		if (p != null) {
			Destroy(p.gameObject);
		}
		if (popupIndex >= popupPositions.Length) {
			isTutorial = false;
			myPlayerPrefs.SetInt("finishedTutorial", myPlayerPrefs.GetInt("finishedTutorial") + 1);
			print("finished tutorial");
			Destroy(tutorialDisplay);
			return;
		}
		Transform insItem = Instantiate(tutorialPrefab, GameObject.Find("Canvas").transform).transform;
		p = insItem.GetComponent<TutorialPopup>();
		p.controller = this;

		switch (arrowDirections[popupIndex]) {
			case 0:
				p.leftArrow.enabled = true;
				break;
			case 1:
				p.rightArrow.enabled = true;
				break;
			case 2:
				p.upArrow.enabled = true;
				break;
			case 3:
				p.upArrow.enabled = true;
				break;
		}
		tutorialDisplay.transform.GetChild(0).GetComponent<Text>().text = CustomFunctions.TranslateText(englishTutorial[popupIndex]);
		insItem.position = CalculateTutorialPos(popup: p);
		popupIndex++;
	}
	Vector2 CalculateTutorialPos(bool previous = false, TutorialPopup popup = null) {
		int i = previous ? popupIndex - 1 : popupIndex;
		if (tiles.ContainsKey(popupPositions[i])) {
			if (popup) popup.notUI = true;
			return RectTransformUtility.WorldToScreenPoint(GameObject.Find("Main Camera").GetComponent<Camera>(), tiles[popupPositions[i]].transform.position);
		} else {
			float h = Screen.height / 2f + popupPositions[i].y * Screen.width / 1920f;
			if (popupAlignments[i] == 1) {
				h = popupPositions[i].y * Screen.width / 1920f;
			} else if (popupAlignments[i] == 2) {
				h = Screen.height - popupPositions[i].y * Screen.width / 1920f;
			}
			return new Vector2(Screen.width / 2f + popupPositions[i].x * Screen.width / 1920f, h);
		}
	}
	public static string CheckCustomFlag(Dictionary<string, string> customFlagDictionary, string countryId) { //if not ID then it is custom flag name
		if (customFlagDictionary == null || !customFlagDictionary.ContainsKey(countryId))
			return "";
		return customFlagDictionary[countryId];
	}
	void UpdateFlags(bool initial) { //note: player must reload the game to see effect
		flags = new Dictionary<string, Sprite>();
		int index = 0;
		foreach (string i in flagNames) {
			if (!initial) {
				if (CheckCustomFlag(countryCustomFlagOverrides, i) != "") {
					bool found = false;
					foreach (Sprite s in customFlags) {
						if (s.name == CheckCustomFlag(countryCustomFlagOverrides, i)) {
							flags.Add(i, s);
							found = true;
							break;
						}
					}
					if (!found) {
						flags.Add(i, flagSprites[index]);
					}
				} else
					flags.Add(i, flagSprites[index]);
			} else
				flags.Add(i, flagSprites[index]);
			index++;
		}
	}
	public bool isTutorial;
	SoldierPrefabsManager soldierInformationController;
	void TutorialSetup() {
		switch (myPlayerPrefs.GetInt("tutorial")) {
			case 1:
				popupPositions = new Vector2[] {
				new Vector2(-4.900024f, -153.9f),
				new Vector2(-1f, 2.5f),
				new Vector2(0f, 2f),
				new Vector2(1f, 2.5f),
				new Vector2(-3f, 2.5f),

				new Vector2(-1f, 2.5f),
				new Vector2(2.099976f, 52f),
				new Vector2(-3f, 2.5f),
				new Vector2(-1f, 1.5f),
				new Vector2(0f, 1f),

				new Vector2(-3f, 2.5f),
				new Vector2(-6.900024f, 65f),
				new Vector2(443.1f, -352f),
				new Vector2(383.1f, 197.1f),
				new Vector2(-2f, 2f),

				new Vector2(-5.900024f, 59f),
				new Vector2(384.1f, 94.09998f),
				new Vector2(902.1f, 57f),
				new Vector2(160.1f, -149.9f),
				new Vector2(0.09997559f, -156.9f),

				new Vector2(-3f, 2.5f),
				new Vector2(-2f, 1f),
				new Vector2(0f, 2f),
				new Vector2(-3f, 2.5f),
				new Vector2(-3f, 2.5f),

				new Vector2(-121.1f, 62f),
				new Vector2(155.1f, -154.9f),
				new Vector2(3f, 0.5f),

			};
				popupAlignments = new int[] {
				0, -1, -1, -1, -1, -1, 1, -1, -1, -1,
				-1, 1, 0, 0, -1, 1, 0, 1, 0, 0,
				-1, -1, 0, -1, -1, 1, 0, -1,
			};

				popupRangeAllowed = new float[] {
				500, 85, 85, 85, 85, 85, 150, 85, 85, 85,
				85, 150, 85, 85, 85, 150, 85, 150, 85, 85,
				85, 85, 85, 85, 85, 150, 120, 9000000
			};

				arrowDirections = new int[] {
				1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
				1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
				1, 1, 1, 1, 1, 1, 1, 1
			};
				englishTutorial = new string[] {
				"Click to start the game",
				"Click to select the unit",
				"Click the green tile to move",
				"Select the enemy to attack",
				"Select the tank",

				"Click the indicated tile",
				"Click the reverse button to redo a move",
				"Select the tank once again",
				"Click the new tile below",
				"Attack the enemy",

				"Select the city",
				"Click the build button",
				"Click to increase the unit size",
				"Buy the infantry",
				"Select the forest",

				"Click the build button again",
				"Build a turret",
				"The turret will be ready in three rounds. Click the hourglass to skip the round",
				"Confirm to skip the round",
				"Click continue to keep playing",

				"Select the unit in the city",
				"Move it outside the city to make space",
				"Select the damaged unit",
				"Move the unit back to the city",
				"Click on the city again",
				"Click on the supply button to heal the unit",

				"Click yes",
				"Take the city to win the mission and finish this tutorial..."
			};
				break;
			case 2:
				popupPositions = new Vector2[] {

			new Vector2(6.099976f, -155.9f),
			new Vector2(-2f, 0f),
			new Vector2(1.099976f, 60f),
			new Vector2(385.1f, -115.9f),
			new Vector2(-5f, -2.5f),

			new Vector2(-0.9000244f, 57f),
			new Vector2(386.1f, -115.9f),
			new Vector2(-5f, -3.5f),
			new Vector2(2f, 1f),
			new Vector2(-1f, -1.5f),

			new Vector2(-75f, 60f),
			new Vector2(-313.9f, 188.1f),
			new Vector2(0f, -3f),
			new Vector2(3f, -1.5f),
			new Vector2(-1f, -1.5f),

			new Vector2(3f, -1.5f),
			new Vector2(0f, -2f),
			new Vector2(1f, -2.5f),
			new Vector2(1f, -1.5f),
			new Vector2(-1f, -0.5f),

			new Vector2(1f, 0.5f),
			new Vector2(2f, 1f),
			new Vector2(-2f, -2f),
			new Vector2(-125f, 59f),
			new Vector2(377.1f, -1.900024f),

			new Vector2(4f, 0f),
			new Vector2(4f, 0f),
			new Vector2(4f, 1f),

			};
				//0 is center, -1 means it's by tile, 1 means bottom alignment
				popupAlignments = new int[] {
				0, -1, 1, 0, -1, 1, 0, -1, -1, -1,
				1, 0, -1, -1, -1, -1, -1, -1, -1, -1,
				-1, -1, -1, 1, 0, -1, -1, -1
			};

				popupRangeAllowed = new float[] {
				350, 85, 85, 85, 85, 85, 85, 85, 85, 85,
				85, 85, 85, 85, 85, 85, 85, 85, 85, 85,
				85, 85, 85, 85, 85, 85, 85, 85
			};

				arrowDirections = new int[] {
				1, 1, 1, 1, 1, 1, 1,1,1,1,
				1, 1, 1, 1, 1, 1, 1 ,1,1,1,
				1,1,1,1,1,1, 1 ,1 ,1,1,1,1,1,1,1,1,1, 1 ,1 ,1,1,1,1,1,1,1,1,1
			};
				englishTutorial = new string[] {
				"Click to start the game",
				"Click on the port. Ports can build naval units.",
				"Click on the build button",
				"Construct an aircraft carrier",
				"Click the ground next to the city",

				"Click build",
				"Construct a missile launcher. These are very powerful but can only be placed near level IV factories.",
				"Select the missile launcher",
				"Attack the enemy ship. This weapon has an extremely high range and armor penetration.",
				"Select the aircraft carrier",

				"Click the general assignment button",
				"Assign a general. Each general can command multiple units and a general can be removed from an existing unit.",
				"Select the rocket artillery",
				"Attack the enemy destroyer. Notice that they have a far range and can damage additional enemy units behind the target.",
				"Select the aircraft carrier",

				"Attack the destroyer again. Carriers are movable airports with a free bombing run every round.",
				"Select the cruiser",
				"Move it to the adjacent water tile",
				"Notice that the enemy cruiser above is now encircled. Encircled units deal a reduced amount of damage to enemies.",
				"Select the submarine",

				"Move it to the battleship",
				"Attack the battleship. Submarines can only attack naval units, but they do not face retaliation. They are also good against large ships.",
				"Select the motorized infantry",
				"Click the build button",
				"Click the paratrooping button",

				"Paradrop your unit into enemy territories. All infantry units can be paradropped from cities with level II or higher airports.",
				"Select the paradropped unit again",
				"Attack behind the enemy lines. Paratrooped units can still perform actions after being paradropped if it didn't move or attack yet. Use these new abilities capture all units and locations with the yellow flag."

			};

				break;
		}
	}
	public void NuclearDropdownChanged() {
		if (nuclearWarheadDropdown.value != 0 && countryDatas[playerCountry].nukes[nuclearWarheadDropdown.value - 1] == 0) {
			nuclearWarheadDropdown.value = 0;
		}
	}
	void UpdateNuclearDisplay() {
		nuclearWarheadDropdown.options = new List<Dropdown.OptionData>();
		nuclearWarheadDropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText("Conventional Bomb")));
		nuclearWarheadDropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText("Atomic Bomb") + "  (" + countryDatas[playerCountry].nukes[0] + ")"));
		nuclearWarheadDropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText("Hydrogen Bomb") + "  (" + countryDatas[playerCountry].nukes[1] + ")"));
		//nuclearWarheadDropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText("Antimatter Bomb"))); currently only have two bombs

		nuclearWarheadDropdown.RefreshShownValue();
	}

	public static int CalculateTechPrices(int techCategory, int techLevel) {
		switch (techCategory) { //this is not TechCategory: this is trooptype
			case 0:
				return 200 + 75 * techLevel;
			case 1:
				return 200 + 75 * techLevel;
			case 2:
				return 200 + 75 * techLevel;
			case 3:
				return 150 + 50 * techLevel;
			case 4:
				return 150 + 50 * techLevel;
			case 5:
				return 150 + 50 * techLevel;
			default:
				return 2; //error
		}
	}
	//check missions; return -1 if no mission required
	public static int CheckTechRequirement(int techCategory, int techLevel) {
		if (techCategory == 0 || techCategory == 2 || techCategory == 5) {
			//use allied/WP lane for unlocks in infantry, artillery, and air
			if (techLevel == 2) {
				return 52; //mission 3 allies (completed)
			} else if (techLevel == 4) {
				return 55; //mission 6 allies
			} else if (techLevel == 6) {
				return 58; //mission 9 allies
			} else if (techLevel == 8) {
				return 61; //mission 12 allies
			} else if (techLevel == 10) {
				return 121; //mission 2 WP
			} else if (techLevel == 12) {
				return 123; //mission 4 WP
			} else if (techLevel == 14) {
				return 125; //mission 6 WP
			} else {
				return -1; //no requirement
			}
		} else {
			//use axis/NATO lane for all other unlocks
			if (techLevel == 2) {
				return 2; //mission 3 axis (completed)
			} else if (techLevel == 4) {
				return 5; //mission 6 axis
			} else if (techLevel == 6) {
				return 8; //mission 9 axis
			} else if (techLevel == 8) {
				return 11; //mission 12 axis
			} else if (techLevel == 10) {
				return 101; //mission 2 NATO
			} else if (techLevel == 12) {
				return 103; //mission 4 NATO
			} else if (techLevel == 14) {
				return 105; //mission 6 NATO
			} else {
				return -1; //no requirement
			}
		}
	}
	public static List<List<List<int>>> techLevels = new List<List<List<int>>> {
		new List<List<int>>() {
            //hp, attack, movement, armor, antiarmor, fuel, range
            //only one stat should change between years; tech of 1936 (element 0) represents no tech and list starts at 1937 (element 1)
            new List<int>() { 0, 3, 0, 0, 0, 0, 0 }, //1937
            new List<int>() { 0, 3, 0, 3, 0, 0, 0 }, //1938
            new List<int>() { 0, 3, 0, 3, 3, 0, 0 }, //1939
            new List<int>() { 0, 3, 1, 3, 3, 0, 0 }, //1940
            new List<int>() { 25, 6, 1, 3, 3, 0, 0 }, //1941

            new List<int>() { 25, 6, 2, 3, 7, 0, 0 }, //1942
            new List<int>() { 25, 6, 2, 7, 7, 0, 0 }, //1943
            new List<int>() { 25, 10, 2, 7, 7, 0, 0 }, //1944
            new List<int>() { 25, 10, 3, 7, 7, 0, 0 }, //1945
            new List<int>() { 55, 10, 3, 7, 7, 0, 0 }, //1950

            new List<int>() { 55, 15, 3, 7, 7, 0, 0 }, //1955
            new List<int>() { 55, 15, 4, 7, 7, 0, 0 }, //1960
            new List<int>() { 55, 15, 4, 7, 12, 0, 0 }, //1965
            new List<int>() { 55, 15, 4, 12, 12, 0, 0 }, //1970
            new List<int>() { 55, 20, 4, 12, 12, 0, 0 }, //1975
            new List<int>() { 90, 20, 4, 12, 12, 0, 0 }, //1980
        }, //0: infantry
        new List<List<int>>() {
            //hp, attack, movement, armor, antiarmor, fuel, range
            new List<int>() { 0, 3, 0, 0, 0, 0, 0 }, //1937
            new List<int>() { 0, 3, 0, 3, 0, 0, 0 }, //1938
            new List<int>() { 0, 3, 0, 3, 3, 0, 0 }, //1939
            new List<int>() { 0, 3, 1, 3, 3, 0, 0 }, //1940
            new List<int>() { 25, 7, 1, 3, 3, 0, 0 }, //1941
            new List<int>() { 25, 7, 2, 3, 7, 0, 0 }, //1942
            new List<int>() { 25, 7, 2, 3, 7, 1, 0 }, //1943
            new List<int>() { 25, 7, 2, 7, 7, 1, 0 }, //1944
            new List<int>() { 25, 7, 2, 7, 12, 1, 0 }, //1945
            new List<int>() { 55, 7, 2, 7, 12, 1, 0 }, //1946
            new List<int>() { 55, 7, 3, 7, 12, 1, 0 }, //1947
            new List<int>() { 55, 7, 3, 12, 12, 1, 0 }, //1948
            new List<int>() { 55, 12, 3, 12, 12, 1, 0 }, //1949
            new List<int>() { 55, 12, 3, 12, 18, 1, 0 }, //1950
            new List<int>() { 55, 12, 3, 18, 18, 1, 0 }, //1975
            new List<int>() { 90, 12, 3, 18, 18, 1, 0 }, //1980
        }, //1: armor
        new List<List<int>>() {
            //hp, attack, movement, armor, antiarmor, fuel, range
            new List<int>() { 0, 0, 0, 0, 4, 0, 0 }, //1937
            new List<int>() { 0, 3, 0, 0, 4, 0, 0 }, //1938
            new List<int>() { 0, 3, 1, 0, 4, 0, 0 }, //1939
            new List<int>() { 0, 3, 1, 3, 4, 0, 0 }, //1940
            new List<int>() { 20, 3, 1, 3, 4, 0, 0 }, //1941
            new List<int>() { 20, 3, 1, 7, 4, 0, 0 }, //1942
            new List<int>() { 20, 3, 2, 7, 4, 0, 0 }, //1943
            new List<int>() { 20, 7, 2, 7, 4, 0, 0 }, //1944
            new List<int>() { 20, 7, 2, 7, 9, 0, 0 }, //1945
            new List<int>() { 45, 7, 2, 12, 9, 0, 0 }, //1946
            new List<int>() { 45, 7, 2, 12, 9, 0, 0 }, //1947
            new List<int>() { 45, 7, 2, 12, 15, 0, 0 }, //1948
            new List<int>() { 45, 7, 3, 12, 15, 0, 0 }, //1949
            new List<int>() { 45, 12, 3, 12, 15, 0, 0 }, //1950
            new List<int>() { 45, 12, 3, 12, 22, 0, 0 },
			new List<int>() { 75, 12, 3, 12, 22, 0, 0 },
		}, //2: artillery
        new List<List<int>>() {
            //hp, attack, movement, armor, antiarmor, fuel, range
            new List<int>() { 15, 0, 0, 0, 0, 0, 0 }, //1937
            new List<int>() { 15, 0, 0, 3, 0, 0, 0 }, //1938
            new List<int>() { 15, 3, 0, 3, 0, 0, 0 }, //1939
            new List<int>() { 35, 3, 0, 3, 0, 0, 0 }, //1940
            new List<int>() { 35, 3, 0, 3, 2, 0, 0 }, //1941
            new List<int>() { 35, 3, 0, 7, 2, 0, 0 }, //1942
            new List<int>() { 35, 3, 0, 7, 5, 0, 0 }, //1943
            new List<int>() { 35, 7, 0, 7, 5, 0, 0 }, //1944
            new List<int>() { 60, 7, 0, 7, 5, 0, 0 }, //1945
            new List<int>() { 60, 12, 0, 7, 5, 0, 0 }, //1946

            new List<int>() { 60, 12, 0, 12, 5, 0, 0 }, //1947
            new List<int>() { 60, 12, 0, 12, 10, 0, 0 }, //1948
            new List<int>() { 60, 12, 0, 18, 10, 0, 0 }, //1949
            new List<int>() { 90, 12, 0, 18, 10, 0, 0 }, //1950
            new List<int>() { 90, 12, 0, 18, 16, 0, 0 },
			new List<int>() { 90, 18, 0, 18, 16, 0, 0 },
		}, //3: forts
        new List<List<int>>() {
            //hp, attack, movement, armor, antiarmor, fuel, range
            new List<int>() { 0, 3, 0, 0, 0, 0, 0 }, //1937
            new List<int>() { 0, 3, 0, 4, 0, 0, 0 }, //1938
            new List<int>() { 0, 3, 0, 4, 4, 0, 0 }, //1939
            new List<int>() { 0, 3, 1, 4, 4, 0, 0 }, //1940
            new List<int>() { 30, 8, 1, 4, 4, 0, 0 }, //1941
            new List<int>() { 30, 8, 2, 4, 10, 0, 0 }, //1942
            new List<int>() { 30, 8, 2, 4, 10, 1, 0 }, //1943
            new List<int>() { 30, 8, 2, 10, 10, 1, 0 }, //1944
            new List<int>() { 30, 8, 2, 10, 10, 1, 0 }, //1945
            new List<int>() { 70, 8, 2, 10, 10, 1, 0 }, //1946
            new List<int>() { 70, 8, 2, 10, 10, 1, 0 }, //1947
            new List<int>() { 70, 8, 2, 18, 10, 1, 0 }, //1948
            new List<int>() { 70, 15, 2, 18, 10, 1, 0 }, //1949
            new List<int>() { 70, 15, 2, 18, 18, 1, 0 }, //1950
            new List<int>() { 70, 15, 3, 18, 18, 1, 0 },
			new List<int>() { 120, 15, 3, 18, 18, 1, 0 },
		}, //4: navy
        new List<List<int>>() {
            //hp, attack, movement, armor, antiarmor, fuel, range
            new List<int>() { 0, 3, 0, 0, 0, 0, 0 }, //1937
            new List<int>() { 0, 3, 0, 0, 3, 0, 0 }, //1938
            new List<int>() { 0, 3, 0, 0, 3, 0, 1 }, //1939
            new List<int>() { 0, 6, 0, 0, 3, 0, 1 }, //1940
            new List<int>() { 0, 6, 0, 0, 7, 0, 1 }, //1941
            new List<int>() { 0, 9, 0, 0, 7, 0, 1 }, //1942
            new List<int>() { 0, 9, 0, 0, 12, 0, 1 }, //1943
            new List<int>() { 0, 9, 0, 0, 12, 0, 2 }, //1944
            new List<int>() { 0, 12, 0, 0, 12, 0, 2 }, //1945
            new List<int>() { 0, 12, 0, 0, 17, 0, 2 }, //1950
            new List<int>() { 0, 12, 0, 0, 17, 0, 3 }, //1955
            new List<int>() { 0, 15, 0, 0, 17, 0, 3 }, //1960
            new List<int>() { 0, 15, 0, 0, 17, 0, 4 }, //1965
            new List<int>() { 0, 15, 0, 0, 22, 0, 4 }, //1970
            new List<int>() { 0, 18, 0, 0, 22, 0, 4 },
			new List<int>() { 0, 18, 0, 0, 30, 0, 4 },
		}, //5: air
    };

	//TODO: NOTE: ALL TECH BOOSTS CALCULATIONS COME HERE
	public enum TechCategory { Health, Attack, Movement, Armor, Antiarmor, Fuel, Range }
	public enum TechTroopType { Infantry, Armor, Artillery, Fort, Navy, Air }
	public static int CheckTech(TechTroopType troopType, int level, TechCategory category) {
		if (level == 0) return 0;

		return techLevels[(int)troopType][level - 1][(int)category];
	}

	public int CheckCountryTech(string country, TechTroopType troopType, TechCategory category) {

		//player technology
		if (myPlayerPrefs.GetInt("multiplayer") == 0 && country == playerCountry && !mapData.overridePlayerTech[myPlayerPrefs.GetInt("level")]) {
			//            print("type: " + troopType + ", category: " + category + ", tech level: " + PlayerData.instance.playerData.techLevels[(int)troopType]);
			//print(CheckTech(troopType, PlayerData.instance.playerData.techLevels[(int)troopType], category));
			return CheckTech(troopType, PlayerData.instance.playerData.techLevels[(int)troopType], category);
		}

		//map preset technology (from data)
		return CheckTech(troopType, countryDatas[country].techLevel, category);
	}
	void Awake() {
		UnityEngine.Object[] sprites = Resources.LoadAll("CustomFlags", typeof(Sprite));

		customFlags = new List<Sprite>();
		foreach (UnityEngine.Object t in sprites) {
			customFlags.Add((Sprite)t);
		}
		foreach (KeyValuePair<string, byte[]> kv in PlayerData.instance.playerData.flags) {
			Texture2D texture = new(1, 1);
			texture.LoadImage(kv.Value);
			Sprite s = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
			s.name = kv.Key;
			customFlags.Add(s);
		}
		try {
			if (MyPlayerPrefs.instance.GetInt("optimization") == 1) {
				Application.targetFrameRate = 35;
			} else if (MyPlayerPrefs.instance.GetInt("optimization") == 0) {
				Application.targetFrameRate = 60;
			} else {
				Application.targetFrameRate = 100;
			}
			playerData = PlayerData.instance;
			instance = this;

			myPlayerPrefs = MyPlayerPrefs.instance;
			loadingCover.SetActive(true);
			soldierInformationController = transform.GetChild(1).GetComponent<SoldierPrefabsManager>();
			troopPrefabs = soldierInformationController.troopPrefabs;

			TutorialSetup();

			for (int i = 0; i < mapSizeControllers.Length; i++) {
				if (i != myPlayerPrefs.GetInt("mapSize")) {
					Destroy(mapSizeControllers[i].gameObject);
				} else {
					mapSizeTransform = mapSizeControllers[i];
				}
			}
			if (myPlayerPrefs.GetInt("editor") == 1) {
				editMode = true;
				if (myPlayerPrefs.GetInt("custom") == 0)
					myPlayerPrefs.SetInt("level", 1);
			} else {
				editMode = false;
			}

			countriesIsNeutral = CustomFunctions.CountriesIsNeutral;
			countriesIsAxis = new Dictionary<string, int>(CustomFunctions.CountriesIsAxis);

			originalCountriesIsAxis = new Dictionary<string, int>(CustomFunctions.CountriesIsAxis);

			try {
				UpdateFlags(true);
			} catch (Exception e) {
				print(e + ": flags failed");
			}


			try {
				playerCountryDropdown.options = new List<Dropdown.OptionData>();

				foreach (string i in countriesIsAxis.Keys)
					if (i != "")
						playerCountryDropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText(i), flags[i]));
				editCountryDropdown.options = new List<Dropdown.OptionData>();
				foreach (string i in countriesIsAxis.Keys)
					if (i != "")
						editCountryDropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText(i), flags[i]));

				countryOverrideDropdown.options = new List<Dropdown.OptionData>();
				countryOverrideDropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText("Default flag")));
			} catch (Exception e) {
				print(e + ": playerCountry failed");
			}
			try {
				string missingTranslationsList = ""; //for adding countries conveniently here
				foreach (Sprite s in customFlags) {
					countryOverrideDropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText(s.name), s));
					if (CustomFunctions.TranslateText(s.name) == s.name) {
						missingTranslationsList += s.name + "\n";
					}
				}
				print(missingTranslationsList);
			} catch (Exception e) {
				print(e + ": translations");
			}
			try {
				eventTargetCountryDropdown.options = new List<Dropdown.OptionData>(playerCountryDropdown.options);

				eventTypeDropdown.options = new List<Dropdown.OptionData>();
				foreach (string i in eventNames) {
					eventTypeDropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText(i)));
				}
			} catch (Exception e) {
				print(e + ": event problem");
			}


			mapData = GetComponentInChildren<MapData>();
		} catch (Exception e) {
			print(e + ": awake failed");
		}
	}
	//name of EventTypes
	private List<string> eventNames = new List<string>() { "Diplomacy", "Manpower", "Industry", "Fuel", "Health", "Veterency", "Begin Nuclear War" };
	public GameObject passRoundPopupPrefab, newsPopupPrefab, endGamePopupPrefab, supplyPopupPrefab, manageGeneralPopupPrefab;
	public Sprite flagSprite, shieldSprite;
	public void manageGeneralPopup() {
		if (!editMode) {
			Transform insItem = Instantiate(manageGeneralPopupPrefab, GameObject.Find("Canvas").transform).transform;
			IngamePopup p = insItem.GetComponent<IngamePopup>();
			p.controller = this;
			insItem.position = new Vector2(Screen.width / 2f, Screen.height / 2f);
		}
	}
	public void passRoundPopup() {
		Transform insItem = Instantiate(passRoundPopupPrefab, GameObject.Find("Canvas").transform).transform;
		IngamePopup p = insItem.GetComponent<IngamePopup>();
		p.controller = this;
		insItem.position = new Vector2(Screen.width / 2f, Screen.height / 2f);
		p.setText(CustomFunctions.TranslateText("Round") + " " + (round + 1).ToString());
		if (victoryCondition == 0) {
			p.setIcon(flagSprite);
			p.setText2(CustomFunctions.TranslateText("Capture all strategic cities and destroy all strategic units in ") + (roundLimit + 1).ToString() + " " + CustomFunctions.TranslateText("rounds"));
		} else if (victoryCondition == 1) {
			p.setIcon(shieldSprite);
			p.setText2(CustomFunctions.TranslateText("Hold all strategic cities for ") + (roundLimit + 1).ToString() + " " + CustomFunctions.TranslateText("rounds"));
		}
		//if (isTutorial) {
		//    insItem.SetSiblingIndex(insItem.GetSiblingIndex() - 1);
		//}
	}

	void UploadMap(bool viewOnly) {
		lastExportedFile = new BinaryData(myPlayerPrefs.GetInt("level"), countryDatas, tiles, soldiers, this);
		StartCoroutine(multiplayerController.UploadMap(lastExportedFile, viewOnly));
	}
	public void endGamePopup(bool win, string customMessage = "") {
		myPlayerPrefs.SetInt("saved", 0);
		if (gameOver)
			return;
		gameOver = true;


		print("cleared save");
		Transform insItem = Instantiate(endGamePopupPrefab, GameObject.Find("Canvas").transform).transform;
		insItem.GetComponent<IngamePopup>().controller = this;
		insItem.position = new Vector2(Screen.width / 2f, Screen.height / 2f);
		if (customMessage != "") {
			insItem.GetComponent<IngamePopup>().setText2(CustomFunctions.TranslateText(customMessage));

		} else {
			insItem.GetComponent<IngamePopup>().setText2(CustomFunctions.TranslateText("Mission time: ") + (round + 1) + " " + CustomFunctions.TranslateText("rounds"));
		}
		if (win) {
			if (myPlayerPrefs.GetInt("multiplayer") == 1) {
				UploadMap(false);
			}
			insItem.GetComponent<IngamePopup>().setText(CustomFunctions.TranslateText("Victory!"));
			if (myPlayerPrefs.GetInt("custom") != 1 && mapData.levelData[myPlayerPrefs.GetInt("level")] != null) {
				try {
					if (!playerData.playerData.levelsUnlocked.Contains(JsonUtility.FromJson<MapInfo>(mapData.levelData[myPlayerPrefs.GetInt("level") + 1].text).missionName)) {
						playerData.playerData.levelsUnlocked.Add(JsonUtility.FromJson<MapInfo>(mapData.levelData[myPlayerPrefs.GetInt("level") + 1].text).missionName);

					}
				} catch (Exception e) {
					print(e);
				}
				if (myPlayerPrefs.GetInt("multiplayer") == 0) {
					if (JsonUtility.FromJson<MapInfo>(mapData.levelData[myPlayerPrefs.GetInt("level")].text).roundLimit > 100) {
						if (!playerData.playerData.completedLevels.Contains(JsonUtility.FromJson<MapInfo>(mapData.levelData[myPlayerPrefs.GetInt("level")].text).missionName +
							JsonUtility.FromJson<MapInfo>(mapData.levelData[myPlayerPrefs.GetInt("level")].text).playerCountry))
							playerData.playerData.completedLevels.Add(JsonUtility.FromJson<MapInfo>(mapData.levelData[myPlayerPrefs.GetInt("level")].text).missionName +
								JsonUtility.FromJson<MapInfo>(mapData.levelData[myPlayerPrefs.GetInt("level")].text).playerCountry);
					} else {
						if (!playerData.playerData.completedLevels.Contains(JsonUtility.FromJson<MapInfo>(mapData.levelData[myPlayerPrefs.GetInt("level")].text).missionName))
							playerData.playerData.completedLevels.Add(JsonUtility.FromJson<MapInfo>(mapData.levelData[myPlayerPrefs.GetInt("level")].text).missionName);
					}
					playerData.playerData.money += myPlayerPrefs.GetInt("reward");
				}
				playerData.saveFile();
			}
		} else
			insItem.GetComponent<IngamePopup>().setText(CustomFunctions.TranslateText("Defeat"));
		StopAllCoroutines();
	}
	public Slider musicVolumeSlider;
	public Slider soundVolumeSlider;
	public Slider sensitivitySlider;

	IEnumerator ThumbsUpMapRequested() {
		myPlayerPrefs.SetInt("likedmaps" + myPlayerPrefs.GetString("mapName" + myPlayerPrefs.GetInt("map")) + myPlayerPrefs.GetString("author" + myPlayerPrefs.GetInt("map")), 1);

		int id = myPlayerPrefs.GetInt("mapserverid" + myPlayerPrefs.GetInt("map"));
		UnityWebRequest r = UnityWebRequest.Get(CustomFunctions.ConvertToUtf8(CustomFunctions.getURL() + "add_like_new?author=" +
			myPlayerPrefs.GetString("author" + myPlayerPrefs.GetInt("map")) + "&map_name=" + myPlayerPrefs.GetString("mapName" +
			myPlayerPrefs.GetInt("map")) + "&map_id=" + (id > 0 ? id : -1)));

		yield return r.SendWebRequest();
	}
	public void LikeMap() {
		rateMapButton.interactable = false;
		StartCoroutine(ThumbsUpMapRequested());
	}
	void Start() {
		if (GameObject.FindObjectOfType<MenuMusicPlayer>()) {
			FindObjectOfType<MenuMusicPlayer>().GetComponent<MenuMusicPlayer>().audioSource.Stop();
			Destroy(GameObject.FindObjectOfType<MenuMusicPlayer>());
		}

		passingRound = true;
		StartCoroutine(MyStart());

		eventMaster.SetActive(true);
		editorMaster.SetActive(true);
	}
	bool noReducedResources = false;
	[HideInInspector]
	public bool started = false; //change to true after started

	bool setCamera = false;
	IEnumerator MyStart() {
		countryDatas = mapData.countryDatas[myPlayerPrefs.GetInt("level")];
		CheckCountryData();
		countryCustomFlagOverrides = mapData.countryCustomFlagOverrides[myPlayerPrefs.GetInt("level")];
		countryCustomNameOverrides = mapData.countryCustomNameOverrides[myPlayerPrefs.GetInt("level")];

		if (editMode) {
			manageGeneralsButton.interactable = false;
			showFlagToggle.isOn = myPlayerPrefs.GetInt("showFlag") == 1 || myPlayerPrefs.GetInt("showFlag") == 0;
		}
		if (myPlayerPrefs.GetInt("editor") == 0 && myPlayerPrefs.GetInt("custom") == 1 && myPlayerPrefs.GetString("mapName" + myPlayerPrefs.GetInt("map")) != "" &&
			myPlayerPrefs.GetInt("likedmaps" + myPlayerPrefs.GetString("mapName" + myPlayerPrefs.GetInt("map")) + myPlayerPrefs.GetString("author" + myPlayerPrefs.GetInt("map"))) == 0) {
			rateMapButton.interactable = true;
		} else
			rateMapButton.interactable = false;

		musicVolumeSlider.value = myPlayerPrefs.GetFloat("realMusics");
		soundVolumeSlider.value = myPlayerPrefs.GetFloat("realSounds");
		sensitivitySlider.value = myPlayerPrefs.GetFloat("sensitivity");

		toggleHideUI();
		toggleEventUI();
		if (!editMode) {
			int index = 0;
			foreach (RectTransform i in editModeUI) {
				if (index != 6)
					Destroy(i.gameObject);
				index++;
			}
			foreach (RectTransform i in editModeHideUI)
				Destroy(i.gameObject);
		} else {
			StartCoroutine(AutoSaveMap());
			Destroy(statsBar);
			int index = 0;
			foreach (RectTransform i in UIRectTransforms) {
				//idk wtf this is; TODO: reform UI
				if (index < 9)
					i.transform.Translate(Vector3.down * 3000f);
				if (index == 13)
					Destroy(i.gameObject);
				index++;

			}

			editCategory = 0;
			editModeTempCountry = "Soviet";
		}
		BinaryData saveFil = null;
		if (myPlayerPrefs.GetInt("saved") == 1 && !editMode) {
			//            print("saved noncustom");
			myPlayerPrefs.SetInt("map", myPlayerPrefs.GetInt("savedMap"));
			myPlayerPrefs.SetInt("level", myPlayerPrefs.GetInt("savedLevel"));
			myPlayerPrefs.SetInt("custom", myPlayerPrefs.GetInt("savedCustom"));

			saveFil = BinarySaveSystem.LoadData();
			usedNukes = saveFil.usedNukes;
		} else if (!editMode) {
			//new game, clear generals
			playerData.playerData.generalsInUse = null;
		}


		if (!editMode || myPlayerPrefs.GetInt("custom") == 1) {
			if (myPlayerPrefs.GetInt("multiplayer") == 1) {
				Destroy(diplomacyButton.gameObject);
				//the first player to move
				originalPlayerCountry = mapData.playerCountry[myPlayerPrefs.GetInt("level")];
				playerCountry = myPlayerPrefs.GetString("playerCountry");
			} else {
				noReducedResources = mapData.lessResources[myPlayerPrefs.GetInt("level")];
				try {
					if (mapData.disableDiplomacy[myPlayerPrefs.GetInt("level")])
						Destroy(diplomacyButton.gameObject);

				} catch {
					print("no diplomacy implemented");
				}
				playerCountry = mapData.playerCountry[myPlayerPrefs.GetInt("level")];
				//conquest mode
				if (myPlayerPrefs.GetString("country") != "" && !editMode && myPlayerPrefs.GetInt("custom") != 1)
					playerCountry = myPlayerPrefs.GetString("country");
				if (editMode) {
					playerCountryDropdown.value = new List<string>(countriesIsAxis.Keys).IndexOf(playerCountry);
				}
			}
			aiMoveThreshold = mapData.aiDetection[myPlayerPrefs.GetInt("level")];
			try {
				foreach (KeyValuePair<string, int> d in mapData.countryTeams[myPlayerPrefs.GetInt("level")]) {
					//countriesIsAxis[d.Key] = d.Value % 2 == 1; //old system to determine is axis from save; now just imports directly team value
					countriesIsAxis[d.Key] = d.Value;
					if (countriesIsNeutral.Contains(d.Key) && d.Value != 2 && d.Value != 3) { //2 is the code for neutral (add custom teams after this column)
						countriesIsNeutral.Remove(d.Key);
					} else if (!countriesIsNeutral.Contains(d.Key) && (d.Value == 2 || d.Value == 3)) { //2 and 3 are both the same now but used to mean neutral axis/allies
						countriesIsNeutral.Add(d.Key);
					}
				}
			} catch (Exception e) {
				print("custom teams does not exist on this map, " + e);
			}
			playerIsAxis = countriesIsAxis[playerCountry];
			if (aiMoveThreshold == 0) {
				aiMoveThreshold = 25;
			}
			roundLimit = mapData.roundLimit[myPlayerPrefs.GetInt("level")];
			mapSize = mapData.mapSize[myPlayerPrefs.GetInt("level")];
			missionName = mapData.missionName[myPlayerPrefs.GetInt("level")];
		}

		if (!editMode)
			roundLimit--;

		victoryCondition = mapData.victoryCondition[myPlayerPrefs.GetInt("level")];
		maxGenerals = mapData.generalLimit[myPlayerPrefs.GetInt("level")];

		try {
			//events in game
			events = mapData.events[myPlayerPrefs.GetInt("level")];
			SortEvents();
		} catch {
			events = new List<GameEvent>();
		}

		UpdateEventDropdown();
		OnEventChanged();
		eventsDropdown.RefreshShownValue();

		if (playerData.playerData.generalsInUse == null) {
			//print("new general assignment");
			playerData.playerData.generalsInUse = new Dictionary<string, int>();
			playerData.playerData.generalsInUse.Add("default", 0);
			List<string> generalKeys = new List<string>(playerData.playerData.generals.Keys);
			List<int> generalValues = new List<int>(playerData.playerData.generals.Values);

			int assignedGeneralsCount = 0;

			switch (myPlayerPrefs.GetInt("tutorial")) {
				case 1:
					playerData.playerData.generalsInUse.Add("Guderian", 3);
					playerData.playerData.generalsInUse.Add("Manstein", 3);
					playerData.playerData.generalsInUse.Add("Rommel", 3);
					break;
				case 2:
					playerData.playerData.generalsInUse.Add("McArthur", 1);
					playerData.playerData.generalsInUse.Add("Spruance", 3);
					playerData.playerData.generalsInUse.Add("Kinkaid", 3);
					break;
				default:
					//automatic generals assignment: prioritize generals in player country, then same alliance, then everything else
					for (int i = 0; i < playerData.playerData.generals.Count; i++) {
						if (assignedGeneralsCount >= maxGenerals)
							break; //stop adding
						if (flags[playerData.generals[generalKeys[i]].country] == flags[myPlayerPrefs.GetInt("multiplayer") == 0 ? playerCountry : myPlayerPrefs.GetString("playerCountry")]) {
							//compare same flags because soviet1 and soviet2 and neutral soviet share generals
							playerData.playerData.generalsInUse.Add(generalKeys[i], generalValues[i]);
							assignedGeneralsCount++;
						}
					}
					for (int i = 0; i < playerData.playerData.generals.Count; i++) {
						if (assignedGeneralsCount >= maxGenerals)
							break; //stop adding
						if (playerData.playerData.generals.Count > i && !playerData.playerData.generalsInUse.ContainsKey(generalKeys[i]) &&
							countriesIsAxis[playerData.generals[generalKeys[i]].country] == countriesIsAxis[myPlayerPrefs.GetInt("multiplayer") == 0 ? playerCountry : myPlayerPrefs.GetString("playerCountry")]) {
							playerData.playerData.generalsInUse.Add(generalKeys[i], generalValues[i]);
							assignedGeneralsCount++;
						}
					}
					for (int i = 0; i < playerData.playerData.generals.Count; i++) {
						if (assignedGeneralsCount >= maxGenerals)
							break; //stop adding
						if (playerData.playerData.generals.Count > i && !playerData.playerData.generalsInUse.ContainsKey(generalKeys[i])) {
							playerData.playerData.generalsInUse.Add(generalKeys[i], generalValues[i]);
							assignedGeneralsCount++;
						}
					}
					break;
			}
		}
		if (editMode) {
			//only add this if the original owner allows duplication
			if (myPlayerPrefs.GetInt("canttouchmap" + myPlayerPrefs.GetInt("map")) == 1) {
				reuploadToggle.gameObject.SetActive(false);
			}

			if (myPlayerPrefs.GetInt("custom") == 0) {
				countryCustomNameOverrides = new Dictionary<string, string>();
				countryDatas = new Dictionary<string, CountryData>();
				foreach (string i in countriesIsAxis.Keys) {
					countryDatas.Add(i, new CountryData(i, 300, 100, 0, 150));
				}
			} else {
				try {
					//just tests whether or not the class actually exists TODO: may be redundant code, remove later
					foreach (string i in countriesIsAxis.Keys) {
						countryDatas[i].manpower += 0;
					}
				} catch {
					Dictionary<string, CountryData> tempDict = new Dictionary<string, CountryData>(countryDatas);
					countryCustomNameOverrides = new Dictionary<string, string>();
					countryDatas = new Dictionary<string, CountryData>();
					foreach (string i in countriesIsAxis.Keys) {
						if (tempDict.ContainsKey(i)) {
							countryDatas.Add(i, tempDict[i]);
						} else {
							countryDatas.Add(i, new CountryData(i, 300, 100, 0, 150));
						}
					}
				}
			}
		}
		UpdateFlags(false);
		buildButtonOn = false;
		//buildButton.position = new Vector2(-1000f, buildButton.position.y);
		//testIntegers = new List<int>();
		canAttackTiles = new List<Tile>();
		soldiers = new List<Unit>();
		tiles = new Dictionary<Vector2, Tile>();
		originalTiles = new Dictionary<Vector2, string>();
		if (saveFil != null && saveFil.originalTileCountries != null) {
			foreach (KeyValuePair<MyVector2, string> kv in saveFil.originalTileCountries) {
				originalTiles.Add(kv.Key.toVector2(), kv.Value);
			}
		}
		if (saveFil == null || myPlayerPrefs.GetInt("custom") == 1) {
			if (!editMode || myPlayerPrefs.GetInt("custom") == 1) {
				if (saveFil != null) {

					round = saveFil.round;
					countryDatas = saveFil.countryDatas;
					CheckCountryData();

					int ind = 0;

					foreach (MyVector2 i in saveFil.tilesCountries.Keys) {
						Vector2 coordinate = Vector2.zero;
						Tile insItem = null;
						if ((int)i.x % 2 == 0) {
							insItem = Instantiate(hex, new Vector3(i.x, i.y * 1.16f, 200f), Quaternion.identity).GetComponent<Tile>();
							coordinate = new Vector2(i.x, i.y);
							insItem.coordinate = coordinate;
							insItem.controller = this;
						} else {
							insItem = Instantiate(hex, new Vector3(i.x, (i.y - 0.5f) * 1.16f + 0.58f, 200f), Quaternion.identity).GetComponent<Tile>();
							coordinate = new Vector2(i.x, i.y);
							insItem.coordinate = coordinate;
							insItem.controller = this;
						}
						insItem.country = saveFil.tilesCountries[i];
						try {
							tiles.Add(coordinate, insItem);
						} catch (Exception e) {
							print(e);
						}
						ind++;
					}
					try {
						int d = 0;
						foreach (MyVector2 i in saveFil.tilesRadiationKeys) {
							tiles[i.toVector2()].radiationLeft = saveFil.tilesRadiationValues[d];
							d++;
						}
					} catch { print("no radiation here"); }
				} else {
					originalTiles = new Dictionary<Vector2, string>();
					int ind = 0;
					foreach (Vector2 i in mapData.countryCoordinates[myPlayerPrefs.GetInt("level")]) {

						Vector2 coordinate = Vector2.zero;
						Tile insItem = null;
						if ((int)i.x % 2 == 0) {
							insItem = Instantiate(hex, new Vector3(i.x, i.y * 1.16f, 200f), Quaternion.identity).GetComponent<Tile>();
							coordinate = new Vector2(i.x, i.y);
							insItem.coordinate = coordinate;
							insItem.controller = this;
						} else {
							insItem = Instantiate(hex, new Vector3(i.x, (i.y - 0.5f) * 1.16f + 0.58f, 200f), Quaternion.identity).GetComponent<Tile>();
							coordinate = new Vector2(i.x, i.y);
							insItem.coordinate = coordinate;
							insItem.controller = this;
						}
						insItem.country = mapData.countryCoordinateNames[myPlayerPrefs.GetInt("level")][ind];
						tiles.Add(coordinate, insItem);
						originalTiles.Add(coordinate, insItem.country);
						ind++;
					}
				}
			} else { //create new map
				isNewMap = true;
				mapSize = myPlayerPrefs.GetInt("mapSize");
				//map system swapped from collisions to just actually counting the tiles
				for (int i = -(int)(mapSizeTransform.localScale.x / 2f - 0.51f) - 1; i < /*55*/(int)(mapSizeTransform.localScale.x / 2f + 0.51f); i++) {
					for (int j = -(int)(mapSizeTransform.localScale.y / 2f - 0.51f); j < /*45*/(int)(mapSizeTransform.localScale.y / 2f + 0.51f); j++) {
						Vector2 coordinate;
						Tile insItem;
						if (i % 2 == 0) {
							insItem = Instantiate(hex, new Vector3(i, j * 1.16f, 200f), Quaternion.identity).GetComponent<Tile>();
							coordinate = new Vector2(i, j);
							insItem.coordinate = coordinate;
							insItem.controller = this;
						} else {
							insItem = Instantiate(hex, new Vector3(i, j * 1.16f + 0.58f, 200f), Quaternion.identity).GetComponent<Tile>();
							coordinate = new Vector2(i, j + 0.5f);
							insItem.coordinate = coordinate;
							insItem.controller = this;
							if (j == -(int)(mapSizeTransform.localScale.y / 2f - 0.51f)) {
								Tile insItem2 = Instantiate(hex, new Vector3(i, j * 1.16f - 0.58f, 200f), Quaternion.identity).GetComponent<Tile>();
								Vector2 coordinate2 = new Vector2(i, j - 0.5f);
								insItem2.coordinate = coordinate2;
								insItem2.controller = this;
								tiles.Add(coordinate2, insItem2);
								insItem2.country = "Switzerland"; //default tile country is a random country so users are not confused
							}
						}

						tiles.Add(coordinate, insItem);
						insItem.country = "Switzerland";

					}
				}

			}
			int index = 0;
			if (myPlayerPrefs.GetInt("custom") == 1) {
				aiDetectionInput.text = aiMoveThreshold.ToString();
				maxGeneralsInput.text = maxGenerals.ToString();

				roundLimitInput.text = roundLimit.ToString();
				victoryConditionDropdown.value = victoryCondition;
				missionNameInput.text = missionName;



				int i = 0;
				foreach (Dropdown.OptionData s in playerCountryDropdown.options) {
					if (s.text == playerCountry)
						playerCountryDropdown.value = i;
					i++;
				}
			}
			int mapLevel = myPlayerPrefs.GetInt("level");
			if (!editMode || myPlayerPrefs.GetInt("custom") == 1) {
				foreach (Vector2 i in mapData.cityCoordinates[mapLevel]) {
					tiles[i].terrain = Terrain.city;
					GameObject insItem = Instantiate(cityPrefab, tiles[i].transform.position, Quaternion.identity);
					insItem.transform.Translate(Vector3.back * 1.1f);
					insItem.GetComponent<City>().currentTile = tiles[i];
					insItem.GetComponent<City>().tier = mapData.cityTiers[mapLevel][index];
					if (mapData.cityDefenceTiers[mapLevel] != null) {
						insItem.GetComponent<City>().defenceTier = mapData.cityDefenceTiers[mapLevel][index];
						insItem.GetComponent<City>().health = insItem.GetComponent<City>().CalculateMaxHealth();
					}


					insItem.GetComponent<City>().isStrategic = mapData.cityStrategics[mapLevel][index];
					insItem.GetComponent<City>().factoryTier = mapData.cityFactoryTiers[mapLevel][index];
					insItem.GetComponent<City>().airportTier = mapData.cityAirportTiers[mapLevel][index];
					try {
						insItem.GetComponent<City>().nuclearTier = mapData.cityNuclearTiers[mapLevel][index];
					} catch { }

					insItem.GetComponent<City>().cityName = mapData.cityNames[mapLevel][index];
					cities.Add(insItem.GetComponent<City>());
					index++;
				}
				index = 0;
				foreach (Vector2 i in mapData.terrainCoordinates[mapLevel]) {
					tiles[i].terrain = (Terrain)mapData.terrain[mapLevel][index] - 1;
					if ((int)tiles[i].terrain < Enum.GetNames(typeof(Terrain)).Length) {

						if (tiles[i].terrain == Terrain.oil) {
							oilRigs.Add(tiles[i]);
						}
						if (tiles[i].terrain == Terrain.village) {
							villages.Add(tiles[i]);
						}
						if (tiles[i].terrain == Terrain.industrialComplex) {
							industrialComplexes.Add(tiles[i]);
						}
					} else {
						// print("terrain not updated");
						tiles[i].terrain = Terrain.plains;
					}
					index++;
				}
			}
		} else {
			round = saveFil.round;
			countryDatas = saveFil.countryDatas;
			CheckCountryData();

			int ind = 0;
			foreach (MyVector2 i in saveFil.tilesCountries.Keys) {
				Vector2 coordinate = Vector2.zero;
				Tile insItem = null;
				if ((int)i.x % 2 == 0) {
					insItem = Instantiate(hex, new Vector3(i.x, i.y * 1.16f, 200f), Quaternion.identity).GetComponent<Tile>();
					coordinate = new Vector2(i.x, i.y);
					insItem.coordinate = coordinate;
					insItem.controller = this;
				} else {
					insItem = Instantiate(hex, new Vector3(i.x, (i.y - 0.5f) * 1.16f + 0.58f, 200f), Quaternion.identity).GetComponent<Tile>();
					coordinate = new Vector2(i.x, i.y);
					insItem.coordinate = coordinate;
					insItem.controller = this;
				}
				insItem.country = saveFil.tilesCountries[i];
				tiles.Add(coordinate, insItem);
				ind++;
			}
			try {
				int d = 0;
				foreach (MyVector2 i in saveFil.tilesRadiationKeys) {
					tiles[i.toVector2()].radiationLeft = saveFil.tilesRadiationValues[d];
					d++;
				}
			} catch { print("no radiation here"); }
			int index = 0;
			foreach (Vector2 i in mapData.cityCoordinates[saveFil.map]) {
				if (tiles[i].terrain != Terrain.water)
					tiles[i].terrain = Terrain.city;
				GameObject insItem = Instantiate(cityPrefab, tiles[i].transform.position, Quaternion.identity);
				insItem.transform.Translate(Vector3.back * 1.1f);
				insItem.GetComponent<City>().currentTile = tiles[i];
				insItem.GetComponent<City>().tier = mapData.cityTiers[saveFil.map][index];
				if (mapData.cityDefenceTiers[saveFil.map] != null) {
					insItem.GetComponent<City>().defenceTier = mapData.cityDefenceTiers[saveFil.map][index];
					insItem.GetComponent<City>().health = insItem.GetComponent<City>().CalculateMaxHealth();
				}
				insItem.GetComponent<City>().factoryTier = mapData.cityFactoryTiers[saveFil.map][index];
				insItem.GetComponent<City>().isStrategic = mapData.cityStrategics[saveFil.map][index];
				insItem.GetComponent<City>().airportTier = mapData.cityAirportTiers[saveFil.map][index];
				//old maps won't have this data
				try {
					insItem.GetComponent<City>().nuclearTier = mapData.cityNuclearTiers[saveFil.map][index];
				} catch { }
				;

				insItem.GetComponent<City>().cityName = mapData.cityNames[saveFil.map][index];
				cities.Add(insItem.GetComponent<City>());
				index++;
			}
			index = 0;
			foreach (Vector2 i in mapData.terrainCoordinates[saveFil.map]) {
				tiles[i].terrain = (Terrain)mapData.terrain[saveFil.map][index] - 1;
				if (tiles[i].terrain == Terrain.oil) {
					oilRigs.Add(tiles[i]);
				}
				if (tiles[i].terrain == Terrain.village) {
					villages.Add(tiles[i]);
				}
				if (tiles[i].terrain == Terrain.industrialComplex) {
					industrialComplexes.Add(tiles[i]);
				}
				index++;
			}
		}
		//print(originalTiles.Count);
		if (editMode) {
			try {
				diplomacyToggle.isOn = mapData.disableDiplomacy[myPlayerPrefs.GetInt("level")];
				overrideTechToggle.isOn = mapData.overridePlayerTech[myPlayerPrefs.GetInt("level")];
				lessResourcesToggle.isOn = mapData.lessResources[myPlayerPrefs.GetInt("level")];
				reuploadToggle.isOn = mapData.allowReupload[myPlayerPrefs.GetInt("level")];
			} catch {

				//not implemented diplomacy/reuploading
				print("not implemented diplomacy");
			}
		}
		if (!editMode && !isTutorial) {

			foreach (City i in cities) {
				if (!i.isPort && i.currentTile.country == playerCountry) {
					Camera.main.transform.position = new Vector3(i.transform.position.x, i.transform.position.y, Camera.main.transform.position.z);
					setCamera = true;
					break;
				}
			}
		}
		//check if defensive mission is broken (if player doesn't own all strategic points on defense game must not instantly end)
		if (victoryCondition == 1) {
			foreach (City i in cities) {
				if (i.isStrategic && countriesIsAxis[i.currentTile.country] != countriesIsAxis[playerCountry] && !countriesIsNeutral.Contains(i.currentTile.country)) {
					cannotGameOver = true;
					print("broken defensive conditions");
					break;
				}
			}
		}

		started = true;
		StartCoroutine(delayedStart(saveFil));
		yield return null;

	}
	//check to see if nukes implemented
	private void CheckCountryData() {
		foreach (KeyValuePair<string, CountryData> d in countryDatas) {
			if (d.Value.nukes.Count == 0) {
				d.Value.nukes = new List<int>() { 0, 0, 0 };
			}
		}
	}
	bool updatingMap = false;

	public static string RemoveBackticksFromMapString(string file) {
		//CustomFunctions.CopyToClipboard(file);
		file = file.Replace("\\\\\\\"", "'");
		return file.Replace("\\", "");
	}
	//multiplayer/hot seat: delete all units and replace them, replace all tile countries, etc
	public bool LoadMidgameBinary(string file) {
		file = file.Substring(1, file.Length - 2);
		file = RemoveBackticksFromMapString(file);
		BinaryData saveFile;

		try {
			saveFile = BinarySaveSystem.ImportFromJson(file);
			usedNukes = saveFile.usedNukes;
		} catch (Exception e) {
			print(e);
			print("load from outside map failed");
			return false;
		}
		print("loaded binary");
		if (updateMapCoroutine != null) {
			try {
				updatingMap = false;
				StopCoroutine(updateMapCoroutine);
				print("killing slow update");
			} catch {
				print("coroutine already stopped");
			}
		}
		if (lastExportedFile == null || lastExportedFile.randomId != saveFile.randomId) { //find a way to prevent the uploader from getting the same map
			if (saveFile.viewOnly) {
				updateMapCoroutine = SlowlyUpdateMap(saveFile);
				StartCoroutine(updateMapCoroutine); //prevents lag when scrolling through
			} else {
				//countrydatas is not needed because players can just manager their resources locally
				int index = 0;
				foreach (MyVector2 i in saveFile.tilesCountriesKeys) {
					tiles[i.toVector2()].country = saveFile.tilesCountriesValues[index];
					tiles[i.toVector2()].updateTileColor();
					index++;
				}
				try {
					int d = 0;
					foreach (MyVector2 i in saveFile.tilesRadiationKeys) {
						tiles[i.toVector2()].radiationLeft = saveFile.tilesRadiationValues[d];
						d++;
					}
				} catch { print("no radiation here"); }

				foreach (Unit i in soldiers) {
					if (i != null)
						Destroy(i.gameObject);
				}
				try {
					for (int i = 0; i < saveFile.cityHealthKeys.Count; i++) {
						tiles[saveFile.cityHealthKeys[i].toVector2()].city.health = saveFile.cityHealthValues[i];
					}
				} catch {
					print("city health not available here");
				}
				try {
					for (int i = 0; i < saveFile.cityBombTypes.Count; i++) {
						tiles[saveFile.cityBombKeys[i].toVector2()].city.bombInProduction = saveFile.cityBombTypes[i];
						tiles[saveFile.cityBombKeys[i].toVector2()].city.roundsToBombProduction = saveFile.cityBombRounds[i];
					}
				} catch {
					print("city bombs not available here");
				}
				soldiers = new List<Unit>();
				for (int i = 0; i < saveFile.unitPositions.Count; i++) {
					bool unitFlippedHorizontal = false;
					try {
						unitFlippedHorizontal = saveFile.unitsFlippedHorizontal[i];
					} catch {
						//save file for rotation not found
					}
					int level = saveFile.unitGeneralsLevels != null ? saveFile.unitGeneralsLevels[i] : 0;
					Unit j = spawnSoldier(saveFile.unitPositions[i].toVector2(), saveFile.unitCountries[i], countriesIsAxis[saveFile.unitCountries[i]],
						saveFile.unitTypes[i], saveFile.unitXps[i], saveFile.unitTiers[i], true, saveFile.unitGenerals[i], level, saveFile.unitDefaultGenerals[i], unitFlippedHorizontal);
					j.health = saveFile.unitHealths[i];
					j.moved = false;
					j.supplied = false;
					j.attacked = false;
					j.isStrategic = saveFile.unitsIsStrategic[i];
					if (saveFile.unitsIsLocked != null) {
						try {
							j.isLocked = saveFile.unitsIsLocked[i];
						} catch (Exception e) {
							print(e);
						}
					}
				}
				foreach (Unit u in soldiers) {
					try {
						u.checkAttack(false);
					} catch (Exception e) {
						print(e);
					}
				}
			}
			if (multiplayerController.matchInfo.current_player == myPlayerPrefs.GetInt("playerId") && multiplayerController.matchInfo.map_view_only == 0) {

				EndRound();
			}
		} else {
			print("same save file");
		}
		downloadedMapBinary = true;
		return true;
	}
	//MULTIPLAYER
	public IEnumerator updateMapCoroutine = null;
	IEnumerator SlowlyUpdateMap(BinaryData saveFile) {
		updatingMap = true;
		List<Unit> originalSoldiers = new List<Unit>(soldiers);
		//countrydatas is not needed because players can just manager their resources locally
		int index = 0;
		int breakIndex = 25;
		foreach (MyVector2 i in saveFile.tilesCountriesKeys) {
			breakIndex--;
			if (breakIndex < 0) {
				breakIndex = 25;
				yield return null;
			}
			tiles[i.toVector2()].country = saveFile.tilesCountriesValues[index];
			tiles[i.toVector2()].updateTileColor();
			index++;
		}
		try {
			int d = 0;
			foreach (MyVector2 i in saveFile.tilesRadiationKeys) {
				tiles[i.toVector2()].radiationLeft = saveFile.tilesRadiationValues[d];
				d++;
			}
		} catch { print("no radiation here"); }
		breakIndex = 10;
		for (int i = 0; i < saveFile.unitPositions.Count; i++) {
			breakIndex--;
			if (breakIndex < 0) {
				breakIndex = 10;
				yield return null;
			}
			bool unitFlippedHorizontal = false;
			try {
				unitFlippedHorizontal = saveFile.unitsFlippedHorizontal[i];
			} catch {
				//save file for rotation not found
			}
			if (tiles[saveFile.unitPositions[i].toVector2()].occupant != null) {
				Destroy(tiles[saveFile.unitPositions[i].toVector2()].occupant.gameObject);
			}
			int level = saveFile.unitGeneralsLevels != null ? saveFile.unitGeneralsLevels[i] : 0;
			Unit j = spawnSoldier(saveFile.unitPositions[i].toVector2(), saveFile.unitCountries[i], countriesIsAxis[saveFile.unitCountries[i]], saveFile.unitTypes[i], saveFile.unitXps[i],
				saveFile.unitTiers[i], true, saveFile.unitGenerals[i], level, saveFile.unitDefaultGenerals[i], unitFlippedHorizontal);
			j.health = saveFile.unitHealths[i];
			j.moved = false;
			j.supplied = false;
			j.attacked = false;
			j.isStrategic = saveFile.unitsIsStrategic[i];
			if (saveFile.unitsIsLocked != null) {
				try {
					j.isLocked = saveFile.unitsIsLocked[i];
				} catch (Exception e) {
					print(e);
				}
			}
		}
		foreach (Unit i in originalSoldiers) {
			if (i != null)
				Destroy(i.gameObject);
		}
		updatingMap = false;
	}
	public void SaveGame() {
		myPlayerPrefs.SetInt("savedMap", myPlayerPrefs.GetInt("map"));
		myPlayerPrefs.SetInt("savedCustom", myPlayerPrefs.GetInt("custom"));
		myPlayerPrefs.SetInt("savedLevel", myPlayerPrefs.GetInt("level"));
		BinarySaveSystem.SaveData(new BinaryData(myPlayerPrefs.GetInt("level"), countryDatas, tiles, soldiers, this));
		if (myPlayerPrefs.GetInt("multiplayer") == 0) {
			myPlayerPrefs.SetInt("saved", 1);
			//print("saved");
		} else {
			myPlayerPrefs.SetInt("saved", 0);
		}
	}
	public void saveGameAndExit() {
		playerData.saveFile();
		if (building)
			return;
		if (editMode) {
			myPlayerPrefs.SetInt("showFlag", showFlagToggle.isOn ? 1 : -1);
			try {
				saveCustomData();
			} catch { }
		} else {
			SaveGame();
		}
		if (!isTutorial)
			myPlayerPrefs.SetInt("playAdsTimer", myPlayerPrefs.GetInt("playAdsTimer") + 1);

		if (myPlayerPrefs.GetInt("multiplayer") == 1) {
			multiplayerController.StopMatching();
		}
		myPlayerPrefs.SaveData();
		UnityEngine.SceneManagement.SceneManager.LoadScene(0);
	}
	//called by build button 
	public void showMUI() {
		if (!editMode) {
			justMoved = false;
			//if (selectedCity != null && selectedCity.currentTile.occupant != null && selectedCity.currentTile.occupant.moving)
			//    return;
			//if (selectedTile != null && selectedTile.occupant != null)
			//    selectedTile.occupant.deselect();
			//deselectTile(false, false);
			buildMenu.position = new Vector3(Screen.width / 2f, buildMenu.position.y, 0f);
			productBar.updateProducts();
			productBar.changePurchaseCategory(0);
			productBar.stackSize = 1; //123
			productBar.stackSizeText.text = "1";
			building = true;

		}
	}
	public RectTransform pauseUI;
	public Text pauseTitle, pauseBigTitle;
	public bool paused;

	public void togglePause() {
		pauseTitle.text = CustomFunctions.TranslateText(missionName);
		if (editMode) {
			pauseBigTitle.text = CustomFunctions.TranslateText("Paused");
		} else {
			if (passingRound || isTutorial) {
				generalButton.interactable = false;
			} else {
				generalButton.interactable = true;
			}
			pauseBigTitle.text = CustomFunctions.TranslateText("Round") + " " + (round + 1);
		}
		//print(passingRound);
		//print(pauseUI.position.x);
		//print(paused);
		if (!passingRound || myPlayerPrefs.GetInt("multiplayer") == 1)
			if (pauseUI.position.x < 0f) {
				pauseUI.position = new Vector2(Screen.width / 2f, Screen.height / 2f);
				paused = true;
			} else {
				pauseUI.position = new Vector2(-9000f, 30f);
				StartCoroutine(removePopupRestraint());
				paused = false;
			}

	}
	[HideInInspector]
	public GameObject diplomacyInstance;
	public void OpenDiplomacy() {
		if (!passingRound) {
			//open diplomacy tab
			Transform insItem = Instantiate(diplomacyPrefab, GameObject.Find("Canvas").transform).transform;

			insItem.GetComponent<DiplomacyControl>().controller = this;

			insItem.position = new Vector2(Screen.width / 2f, Screen.height / 2f);

			diplomacyInstance = insItem.gameObject;

		}
	}
	public Button beginningSpeechButton;
	public Button generalButton;
	public GameObject generalPopup;
	[HideInInspector]
	public Unit assignmentSoldier;
	public void assignGeneral() {
		if (selectedSoldier != null)
			assignmentSoldier = selectedSoldier;
		else {
			assignmentSoldier = selectedTile.occupant;
		}
		Transform insItem = Instantiate(generalPopup, GameObject.Find("Canvas").transform).transform;
		IngamePopup p = insItem.GetComponent<IngamePopup>();
		p.controller = this;
		insItem.position = new Vector2(Screen.width / 2f, Screen.height / 2f);
		//deselectTile();
		//if (selectedSoldier != null) {
		//    selectedSoldier.currentTile.hexRendererColor = selectedSoldier.currentTile.defaultTileColor;
		//    selectedSoldier.selected = false;
		//}

		//selectedSoldier = null;
		//selectedCity = null;
	}
	public void actualAssignGeneral(int assignmentNumber) {
		///////print(assignmentNumber);
		assignmentSoldier.general = new List<string>(playerData.playerData.generalsInUse.Keys)[assignmentNumber];
		assignmentSoldier.generalLevel = new List<int>(playerData.playerData.generalsInUse.Values)[assignmentNumber];
		assignmentSoldier.updateGeneral(true);
	}

	public void hideMUI() {
		buildMenu.position = new Vector3(-3800f, buildMenu.position.y, 0f);
		productBar.changePurchaseCategory(0);
		building = false;
		StartCoroutine(removePopupRestraint());
	}
	string beginningText = "", beginningGeneral = "";
	bool isNewMap = false; //if a map was just created
	private void SortEvents() {
		//SORTING METHOD BY https://stackoverflow.com/questions/3309188/how-to-sort-a-listt-by-a-property-in-the-object
		events = events.OrderBy(o => o.triggerRound).ToList(); //sorts events by round so only the first element of the round has to be checked
	}
	IEnumerator delayedStart(BinaryData saveFile) {
		yield return true;


		if (myPlayerPrefs.GetInt("multiplayer") == 1) {
			exitToMenuButton.transform.GetChild(0).GetComponent<Text>().text = CustomFunctions.TranslateText("Surrender");
		}

		int index = 0;
		if (!editMode || myPlayerPrefs.GetInt("custom") == 1) {
			if (saveFile != null) {
				if (saveFile.countriesIsAxis != null) {
					countriesIsAxis = new Dictionary<string, int>(saveFile.countriesIsAxis);
					countriesIsNeutral = new List<string>(saveFile.countriesIsNeutral);
					playerIsAxis = countriesIsAxis[playerCountry];
				} else {
					print("no preexisting countriesisaxis");
				}
				if (saveFile.events != null) {
					events = new List<GameEvent>(saveFile.events);
					SortEvents();
				} else {
					print("no events in save file yet");
				}
				try {
					for (int i = 0; i < saveFile.cityHealthKeys.Count; i++) {
						tiles[saveFile.cityHealthKeys[i].toVector2()].city.health = saveFile.cityHealthValues[i];
					}
				} catch {
					print("city health not available here");
				}
				try {
					for (int i = 0; i < saveFile.cityBombKeys.Count; i++) {
						tiles[saveFile.cityBombKeys[i].toVector2()].city.bombInProduction = saveFile.cityBombTypes[i];
						tiles[saveFile.cityBombKeys[i].toVector2()].city.roundsToBombProduction = saveFile.cityBombRounds[i];

					}
				} catch {
					print("city bomb not available here");
				}
				//LOADING SAVE
				for (int i = 0; i < saveFile.unitPositions.Count; i++) {
					if (index % 25 == 0) {
						//skip tick
						yield return null;
					}
					bool unitFlippedHorizontal = false;
					try {
						unitFlippedHorizontal = saveFile.unitsFlippedHorizontal[i];
					} catch {
						//save file for rotation not found
					}
					int level = saveFile.unitGeneralsLevels != null ? saveFile.unitGeneralsLevels[i] : 0;
					Unit j = spawnSoldier(saveFile.unitPositions[i].toVector2(), saveFile.unitCountries[i], countriesIsAxis[saveFile.unitCountries[i]],
						saveFile.unitTypes[i], saveFile.unitXps[i], saveFile.unitTiers[i], true, saveFile.unitGenerals[i], level, saveFile.unitDefaultGenerals[i], unitFlippedHorizontal);
					j.health = saveFile.unitHealths[i];
					j.moved = saveFile.unitsMoved[i];
					j.supplied = saveFile.unitsSupplied[i];
					j.attacked = saveFile.unitsAttacked[i];
					try {
						j.isStrategic = saveFile.unitsIsStrategic[i];
						j.isLocked = saveFile.unitsIsLocked[i];
					} catch {
					}
					index++;
				}
			} else {
				beginningText = mapData.beginningText[myPlayerPrefs.GetInt("level")];
				beginningGeneral = mapData.beginningTextGeneral[myPlayerPrefs.GetInt("level")];
				//print(beginningText);

				foreach (Vector2 i in mapData.soldierCoordinates[myPlayerPrefs.GetInt("level")]) {
					if (index % 25 == 0) {
						//skip tick
						yield return null;
					}
					Unit s = spawnSoldier(i, tiles[i].country, countriesIsAxis[tiles[i].country],
						mapData.soldierIds[myPlayerPrefs.GetInt("level")][index] - 1,
						mapData.soldierVeterencies[myPlayerPrefs.GetInt("level")][index],
						mapData.soldierTiers[myPlayerPrefs.GetInt("level")][index], true);

					try {
						s.general = mapData.soldierGenerals[myPlayerPrefs.GetInt("level")][index];
						s.defaultGeneral = s.general;
						s.generalLevel = mapData.soldierGeneralLevels[myPlayerPrefs.GetInt("level")][index];

						if (countryDatas[s.country].aiGenerals == null) {
							countryDatas[s.country].aiGenerals = new Dictionary<string, int>();
						}

						if (countryDatas[s.country].aiGenerals != null && !countryDatas[s.country].aiGenerals.ContainsKey(s.general) && s.general != "" && s.general != "default") {
							countryDatas[s.country].aiGenerals.Add(s.general, s.generalLevel);
						}

					} catch {
						print("could not use default general");
					}
					try {
						s.isStrategic = mapData.soldierIsStrategics[myPlayerPrefs.GetInt("level")][index];
						s.isLocked = mapData.soldierIsLocked[myPlayerPrefs.GetInt("level")][index];
					} catch {
					}
					index++;

				}
			}
		}
		yield return null;
		isTutorial = myPlayerPrefs.GetInt("tutorial") >= 1; //1 is tutorial 1; 2 is tutorial 2
		if (isTutorial) {
			Destroy(beginningSpeechButton.gameObject);
			tutorialDisplay.GetComponent<Image>().enabled = true;
			tutorialDisplay.transform.GetChild(0).GetComponent<Text>().enabled = true;
			ContinuePopup();
		} else {
			if (beginningText != "" && beginningText != null && !editMode) { //must have actual speech to say
				hasDialogue = true;

				tutorialDisplay.GetComponent<Image>().enabled = true;
				tutorialDisplay.transform.GetChild(1).GetComponent<Image>().enabled = true;
				try {
					tutorialDisplay.transform.GetChild(1).GetComponent<Image>().sprite = playerData.generalPhotos[beginningGeneral];
				} catch {
					tutorialDisplay.transform.GetChild(1).GetComponent<Image>().enabled = false;

				}
				tutorialDisplay.transform.GetChild(2).GetComponent<Text>().enabled = true;
				tutorialDisplay.transform.GetChild(2).GetComponent<Text>().text = CustomFunctions.TranslateText(beginningText);
				CalculateDialogueText();

			} else {
				Destroy(beginningSpeechButton.gameObject);
				Destroy(tutorialDisplay);
			}
		}
		if (myPlayerPrefs.GetInt("multiplayer") == 1) {
			if (myPlayerPrefs.GetInt("playerId") != 0) {
				passingRound = true; //prevent player from doing anything before turn
			} else {
				passingRound = false;
			}
			//get a custom multiplayer popup
			StartCoroutine(CallMultiplayerGames());
		} else {
			passingRound = false;
		}
		if (editMode) {
			beginningGeneralDropdown.options = new List<Dropdown.OptionData>();
			foreach (string i in playerData.generals.Keys) {
				if (i != "default") {
					try {
						beginningGeneralDropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText(i), flags[playerData.generals[i].country]));

					} catch {
						beginningGeneralDropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText(i)));

						print("flag not available");
					}
				} else {
					beginningGeneralDropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText(i)));

				}
			}
			if (!isNewMap) {
				beginningTextInput.text = mapData.beginningText[myPlayerPrefs.GetInt("level")];
				if (new List<string>(playerData.generals.Keys).Contains(mapData.beginningTextGeneral[myPlayerPrefs.GetInt("level")]))
					beginningGeneralDropdown.value = new List<string>(playerData.generals.Keys).IndexOf(mapData.beginningTextGeneral[myPlayerPrefs.GetInt("level")]);
				beginningGeneralDropdown.captionText.text = beginningGeneralDropdown.options[beginningGeneralDropdown.value].text;
			}

			techYearDropdown.RefreshShownValue();
			countriesIsAxisDropdown.RefreshShownValue();
			countryOverrideDropdown.RefreshShownValue();

			onDropdownChanged();
			onEditCategoryChanged();
		}
		if (!setCamera) {
			foreach (Unit i in soldiers) {
				if (i.country == playerCountry) {
					Camera.main.transform.position = new Vector3(i.transform.position.x, i.transform.position.y, Camera.main.transform.position.z);
					setCamera = true;
					break;
				}
			}
		}
		yield return null;
		if (!editMode) {
			if (!hasDialogue) {
				CheckEvent(true);

				passRoundPopup();
			}
			foreach (Unit i in soldiers) {
				if (i != null) {
					i.checkAttack(true);
				}
			}
			CheckEvent(true);
		}
		Destroy(loadingCover);
		fullyInitialized = true;
	}

	void CheckEvent(bool showPopup) {
		//print("check");
		for (int e = 0; e < events.Count; e++) {
			if (events.Count > 0 && events[e].triggerRound < round + 2) {
				if (events[e].cityCondition != null && events[e].cityCondition != "") {
					//print(events[e].cityCondition);
					foreach (City i in cities) {
						if (i.cityName == events[e].cityCondition) {
							//current country is not the country the city started with
							if (originalTiles[i.currentTile.coordinate] != i.currentTile.country) {
								if (showPopup) {
									NewsPopup(events[e]);
								}
								ConsumeEvent(events[e]);
								CheckEvent(showPopup);
								return;
							}
							break;
						}
					}
					//skip to next event if current event is a trigger that won't actually be consumed
					continue;
				}
				if (showPopup) {
					NewsPopup(events[e]);
				}
				ConsumeEvent(events[e]);
				CheckEvent(showPopup);
			}
			return;
		}
	}
	[HideInInspector]
	public bool hasDialogue;
	bool lastDialogue = false;
	public Text moreText;
	string currentDialogueText = ""; //trimmed with each click
	bool CalculateDialogueText() {
		moreText.enabled = true;
		//got this from https://answers.unity.com/questions/1249043/get-lines-of-a-ui-text.html
		Text myText = tutorialDisplay.transform.GetChild(2).GetComponent<Text>();
		if (currentDialogueText == "") { //assign the trimming
			currentDialogueText = myText.text;
		} else {
			myText.text = currentDialogueText;
		}
		Canvas.ForceUpdateCanvases();

		if (myText.cachedTextGenerator.lines.Count >= 5) {
			string oldString = "";
			string lastPiece = "";
			for (int i = 0; i < myText.cachedTextGenerator.lines.Count - 1; i++) {
				int startIndex = myText.cachedTextGenerator.lines[i].startCharIdx;
				int endIndex = (i == myText.cachedTextGenerator.lines.Count - 1) ? myText.text.Length : myText.cachedTextGenerator.lines[i + 1].startCharIdx;
				int length = endIndex - startIndex;
				if (i == 3) {
					lastPiece = myText.text.Substring(startIndex, length);
				} else {
					oldString += myText.text.Substring(startIndex, length);
				}
			}
			myText.text = oldString;
			moreText.text = lastPiece + "...";
			currentDialogueText = currentDialogueText.Substring(oldString.Length + lastPiece.Length);
			return true;

		} else if (myText.text != "" && !lastDialogue) {
			lastDialogue = true;
			moreText.enabled = false;
			return true;
		}
		return false; //finished


	}
	public void PassDialogue() {
		StartCoroutine(PassDialogueDelay());
	}
	IEnumerator PassDialogueDelay() {
		yield return null;

		if (!CalculateDialogueText() && fullyInitialized && hasDialogue) {
			hasDialogue = false;
			Destroy(beginningSpeechButton.gameObject);
			Destroy(tutorialDisplay);
		}
	}
	bool fullyInitialized = false;
	public MultiplayerLobby multiplayerController;
	IEnumerator CallMultiplayerGames() {
		float uploadTimer = 20f;
		while (true) {
			for (float i = 0f; i < 2.5f; i += Time.deltaTime, uploadTimer -= Time.deltaTime) {
				yield return null;
			}
			if (gameOver) {
				print("game is over");
				break;
			}
			if (!passingRound && uploadTimer <= 0f) {
				uploadTimer = 25f;
				while (updatingMap) {
					yield return null;
					print("waiting for map to finish updating");
				}
				UploadMap(true);
			}
			StartCoroutine(multiplayerController.GetMatchInfo());
		}
	}
	public Unit spawnSoldier(Vector2 coordinates, string country, int isAxis, int troopType, int veterency,
		int tier, bool isInitial, string general = "", int generalLevel = 0, string defaultGeneral = "", bool flippedHorizontal = false) {
		if (coordinates.y < 0f) {
			coordinates = new Vector2(coordinates.x, coordinates.y - 0.5f);
		}
		GameObject selectedPrefab;
		try {
			selectedPrefab = troopPrefabs[troopType];
		} catch {
			List<int> randomTroops = new() { 0, 1, 4, 5, 6 };

			//seed the random it based on the troop type, and then pick one from randomTroops so that even
			// wrong ids will give the same troop; use a new system random
			System.Random random = new System.Random(troopType);

			int selected = randomTroops[random.Next(randomTroops.Count)];
			selectedPrefab = troopPrefabs[selected]; //spawns in random if the corresponding soldier was not found
			troopType = selected;
		}
		GameObject ins = Instantiate(selectedPrefab, new Vector2(coordinates.x, (int)coordinates.y * 1.16f + (int)Mathf.Abs(coordinates.x) % 2 * 0.58f), Quaternion.identity);
		Tile t = tiles[new Vector2(coordinates.x, (int)(coordinates.y) + (int)Mathf.Abs(coordinates.x) % 2 * 0.5f)];
		t.occupant = ins.GetComponent<Unit>();
		Unit i = ins.GetComponent<Unit>();
		i.currentTile = t;
		i.controller = this;

		// Debug.Log(troopType);
		i.troopId = troopType;
		i.country = country;
		i.UpdateStats();
		i.isAxis = isAxis;
		i.general = general;

		if (general != "" && general != "default" && defaultGeneral != general && country == playerCountry) {
			try {
				i.generalLevel = playerData.playerData.generals[general];
			} catch (Exception e) {
				print(e);
			}
		} else {
			i.generalLevel = generalLevel;
		}

		if (t.terrain != Terrain.water) {
			if (flippedHorizontal) {
				i.flippedHorizontal = true;
			}
		}
		i.defaultGeneral = defaultGeneral;
		i.veterency = veterency;
		if (i.troopType == Troop.fortification && !isInitial) {
			i.tier = 4;
		} else
			i.tier = tier;

		if (i.troopType != Troop.fortification) {
			if (i.troopId != 14) {
				i.maxHealth *= 1f + (tier - 1f) * 0.7f;
				i.damage *= 1f + (tier - 1f) * 0.25f;
			} else {
				i.maxHealth *= 1f + (tier - 1f) * 0.75f;
				i.damage *= 1f + (tier - 1f) * 0.45f;
			}
		}
		if (editMode)
			ins.GetComponent<Unit>().currentTile = tiles[new Vector2(coordinates.x, (int)(coordinates.y) + (int)Mathf.Abs(coordinates.x) % 2 * 0.5f)];
		soldiers.Add(ins.GetComponent<Unit>());
		calculatePlayerPopulation();
		return ins.GetComponent<Unit>();
	}
	void deselectTile() {
		deselectTile(false);
	}
	void deselectTile(bool removeOccupant) {
		deselectTile(removeOccupant, true);
	}
	public void deselectTile(bool removeOccupant, bool stopAnimation) {
		if (selectedTile != null) {
			selectedTile.clearSearchedTiles();
			selectedTile.resetTilePathfinding(false);

			selectedTile.hexRendererColor = selectedTile.defaultTileColor;

			//selectedTile.GetComponentInChildren<SpriteRenderer>().color = selectedTile.defaultTileColor;

			selectedTile.selected = false;
			if (stopAnimation && selectedTile.occupant != null)
				selectedTile.occupant.deselect();
			if (removeOccupant)
				selectedTile.occupant = null;
			if (selectedSoldier) {
				selectedSoldier.selected = false;
				selectedSoldier = null;
			}
			//selectedCity = null;
			selectedTile = null;
		}
		foreach (Tile i in canAttackTiles) {
			i.movable = false;
			i.canBeAttacked = false;

			i.hexRendererColor = i.defaultTileColor;

			//i.GetComponentInChildren<SpriteRenderer>().color = i.defaultTileColor;
		}
		canAttackTiles = new List<Tile>();
	}
	public void aiProduceTroop(City city, string country) {
		produceTroop(city, country);
	}
	//ai production
	void produceTroop(City city, string paymentCountry) {
		//yield return null;
		if (city.currentTile.occupant == null) {
			int iteration = 0;
			while (true) {
				int i, j, k;
				if (city.isPort) {
					i = 5; //shipyard product
				} else if (countryDatas[paymentCountry].industry > countryDatas[paymentCountry].manpower && city.factoryTier >= 1) {
					//prioritize making tanks and artillery if more industry than manpower
					i = UnityEngine.Random.Range(1, 3);
				} else {
					i = UnityEngine.Random.Range(0, 3); //only include the first three categories (infantry, tank, artillery)
				}
				j = UnityEngine.Random.Range(0, productBar.troopIndustryCosts[i].Length); //specific unit
				k = UnityEngine.Random.Range(1, 4); //tier
													//only require high tier if affordable
				if (city.isStrategic && !city.isPort && countryDatas[paymentCountry].manpower > 120 && countryDatas[paymentCountry].industry > 50) {
					k++;
				}

				if (countryDatas[paymentCountry].manpower >= productBar.CalculateTroopManpowerCost(i, j, paymentCountry) * k &&
					countryDatas[paymentCountry].industry >= productBar.CalculateTroopIndustryCost(i, j, paymentCountry) * k &&
					(troopPrefabs[productBar.troopTypes[productBar.troopNames[i][j]]].GetComponent<Unit>().movementFuelCost / (float)countryDatas[paymentCountry].fuel < 0.25f) &&
					productBar.troopIndustryRequirements[productBar.troopNames[i][j]] <= city.factoryTier) {
					try {
						spawnSoldier(city.currentTile.coordinate, paymentCountry, countriesIsAxis[paymentCountry], productBar.troopTypes[productBar.troopNames[i][j]], 0, k, false);
					} catch {
						print(city.nameText.text);
						print(productBar.troopTypes[productBar.troopNames[i][j]]);
					}
					countryDatas[paymentCountry].manpower -= productBar.CalculateTroopManpowerCost(i, j, paymentCountry) * k;
					countryDatas[paymentCountry].industry -= productBar.CalculateTroopIndustryCost(i, j, paymentCountry) * k;
					break;
				}
				if (iteration > 120) {
					print("could not find affordable unit");
					print("strategic: " + city.isStrategic);
					print("manpower: " + countryDatas[paymentCountry].manpower);
					print("industry: " + countryDatas[paymentCountry].industry);
					print("fuel: " + countryDatas[paymentCountry].fuel);


					break;
				}
				iteration++;
			}
		} else {
			//print(city.nameText.text);
		}
	}
	IEnumerator AutoSaveMap() {
		while (true) {
			for (float i = 0; i < 120f; i += Time.deltaTime) {
				yield return null;
			}
			saveCustomData();
			myPlayerPrefs.SaveData();
		}
	}
	void saveCustomData() {
		while (true) {
			bool noNull = true;
			for (int i = 0; i < soldiers.Count; i++)
				if (soldiers[i] == null) {
					soldiers.RemoveAt(i);
					noNull = false;
				}
			if (noNull)
				break;
		}
		while (true) {
			bool noNull = true;
			for (int i = 0; i < cities.Count; i++)
				if (cities[i] == null) {
					cities.RemoveAt(i);
					noNull = false;
				}
			if (noNull)
				break;
		}

		if (myPlayerPrefs.GetString("customData" + myPlayerPrefs.GetInt("map")) == "") {
			print("new map save");
			myPlayerPrefs.SetInt("currentSaveIndex", myPlayerPrefs.GetInt("currentSaveIndex") + 1);
			if (myPlayerPrefs.GetString("customDataAll") == "") {
				myPlayerPrefs.SetString("customDataAll", myPlayerPrefs.GetInt("map").ToString());
			} else {
				myPlayerPrefs.SetString("customDataAll", myPlayerPrefs.GetString("customDataAll") + "," + myPlayerPrefs.GetInt("map"));
			}
		}
		myPlayerPrefs.SetString("customData" + myPlayerPrefs.GetInt("map"), JsonUtility.ToJson(new MapInfo(this)));
	}
	public bool multiplayerAIMovement = false; //this is so that the map is not updated when ai is processing
	IEnumerator passRound() {
		if (selectedEnemySoldier != null) {
			selectedEnemySoldier.currentTile.hexRendererColor = selectedEnemySoldier.currentTile.defaultTileColor;
			selectedEnemySoldier.enemySelected = false;
			selectedEnemySoldier = null;
		}
		yield return null;

		List<City> tempCities = new List<City>(cities);
		List<City> tempCities2 = new List<City>();

		try {
			int length = tempCities.Count;
			for (int i = 0; i < length; i++) {
				int changeIndex = UnityEngine.Random.Range(0, tempCities.Count);
				tempCities2.Add(tempCities[changeIndex]);
				tempCities.Remove(tempCities[changeIndex]);
			}
			cities = new List<City>(tempCities2);

			canSupply = false;
			canBuild = false;

			buildButtonOn = false;

			//buildButton.position = new Vector3(-1000f, buildButton.position.y, 0f);
			selectedBuildTile = null;

			selectedCity = null;
			justMoved = false;
			passingRound = true;

			multiplayerAIMovement = true;

			if (selectedSoldier != null)
				selectedSoldier.selected = false;

			selectedSoldier = null;

			if (selectedEnemySoldier != null)
				selectedEnemySoldier.enemySelected = false;

			selectedEnemySoldier = null;


			deselectTile(false);
		} catch (Exception e) { print(e); }
		try {
			foreach (City c in cities) {
				if (c.defenceTier > 0) {
					c.health += Mathf.Sqrt(c.tier * 50f);
				}
			}
		} catch (Exception e) { print(e); }
		try {

			//add health if on factory/village terrain
			foreach (Unit i in soldiers) {
				if (i.general != "" && playerData.generals[i.general].ContainsPerk(General.GeneralPerk.Logistics) && i.troopType == Troop.infantry) {
					i.health *= 1f + GeneralManagerPopup.GeneralPerksCalculation(General.GeneralPerk.Logistics, i.generalLevel);
				} else if (i.general != "" && playerData.generals[i.general].ContainsPerk(General.GeneralPerk.Mechanic) &&
					(i.troopType == Troop.armor || i.troopType == Troop.artillery || i.troopType == Troop.navy)) {
					i.health *= 1f + GeneralManagerPopup.GeneralPerksCalculation(General.GeneralPerk.Mechanic, i.generalLevel);
				}
				if (i != null && i.currentTile != null) {
					if ((i.currentTile.terrain == Terrain.village && i.troopType == Troop.infantry ||
						i.currentTile.terrain == Terrain.industrialComplex && i.troopType != Troop.infantry)) {
						i.health += 5f;
					}
				}
				i.health = Mathf.Min(i.health, i.maxHealth);
			}

			//multiplayer
			bool isHost = myPlayerPrefs.GetInt("isHost") == 1;

			int opponentTeam = 0;
			if (myPlayerPrefs.GetString("opponentCountry") != "" &&
				countriesIsAxis.ContainsKey(myPlayerPrefs.GetString("opponentCountry"))) {
				opponentTeam = countriesIsAxis[myPlayerPrefs.GetString("opponentCountry")];
			}


			bool notMultiplayer = myPlayerPrefs.GetInt("multiplayer") == 0;
			foreach (Tile t in tiles.Values) {
				if (t.radiationLeft > 0) {
					if (notMultiplayer || countriesIsAxis[t.country] == countriesIsAxis[playerCountry] || isHost && countriesIsAxis[t.country] != opponentTeam) {
						if (t.occupant) {
							t.occupant.health = Mathf.Max(t.occupant.health - t.occupant.maxHealth * 0.1f, 0f);
							if (t.occupant.health <= 0f) {
								KillUnit(t.occupant);
							}
						}
						if (t.isCity) {
							t.city.health = Mathf.Max(t.city.health - t.city.CalculateMaxHealth() * 0.1f, 0f);
							t.city.UpdateHealth();
						}
					}
					t.radiationLeft--;
				}
			}


			//an AI unit will not move if it cannot produce a new unit
			foreach (City i in cities) {
				if (notMultiplayer || countriesIsAxis[i.currentTile.country] == countriesIsAxis[playerCountry] || isHost && countriesIsAxis[i.currentTile.country] != opponentTeam) {
					if (i.roundsToBombProduction > 0) {
						i.roundsToBombProduction--;
						if (i.roundsToBombProduction == 0) {
							//make bomb
							//print("bomb: " + i.bombInProduction + ", " + countryDatas[i.currentTile.country].nukes.Count);
							countryDatas[i.currentTile.country].nukes[i.bombInProduction]++;
						}
					}
					if (i.currentTile.occupant != null) {
						//add health in cities scales differently
						if (i.currentTile.occupant.troopType == Troop.infantry) {
							i.currentTile.occupant.health += Mathf.Sqrt(i.tier) * 7f;
						} else {
							i.currentTile.occupant.health += Mathf.Sqrt(i.factoryTier) * 7f;
						}
						if (i.currentTile.occupant.health > i.currentTile.occupant.maxHealth)
							i.currentTile.occupant.health = i.currentTile.occupant.maxHealth;
					}

					if (((playerCountry != i.currentTile.country || deltaRoundAuto) && playerIsAxis == countriesIsAxis[i.currentTile.country] ||
						countriesIsNeutral.Contains(i.currentTile.country)) && !noReducedResources) {
						countryDatas[i.currentTile.country].manpower += i.tier * 5;
						countryDatas[i.currentTile.country].industry += i.factoryTier * 4;
					} else {
						countryDatas[i.currentTile.country].manpower += i.tier * 7;
						countryDatas[i.currentTile.country].industry += i.factoryTier * 5;
					}
					if ((i.currentTile.country != playerCountry || deltaRoundAuto) &&
						(i.isPort && countryDatas[i.currentTile.country].manpower >= productBar.CalculateTroopManpowerCost(5, 0, i.currentTile.country) &&
						countryDatas[i.currentTile.country].industry >= productBar.CalculateTroopIndustryCost(5, 0, i.currentTile.country) || !i.isPort &&
						countryDatas[i.currentTile.country].manpower >= productBar.CalculateTroopManpowerCost(0, 0, i.currentTile.country) * 1.5f &&
						countryDatas[i.currentTile.country].industry >= productBar.CalculateTroopIndustryCost(0, 0, i.currentTile.country))) {
						//will produce unit if country still has money
						aiProduceTroop(i, i.currentTile.country);
					}
				}
			}

			foreach (Tile i in oilRigs) {
				if (i && (notMultiplayer || countriesIsAxis[i.country] == countriesIsAxis[playerCountry] || isHost && countriesIsAxis[i.country] != opponentTeam)) {
					if (((playerCountry != i.country || deltaRoundAuto) && playerIsAxis == countriesIsAxis[i.country] || countriesIsNeutral.Contains(i.country)) && !noReducedResources) {
						countryDatas[i.country].fuel += 20;
					} else {
						countryDatas[i.country].fuel += 25;
					}
				}
			}
			foreach (Tile i in villages) {
				if (i && (notMultiplayer || countriesIsAxis[i.country] == countriesIsAxis[playerCountry] || isHost && countriesIsAxis[i.country] != opponentTeam)) {
					if (((playerCountry != i.country || deltaRoundAuto) && playerIsAxis == countriesIsAxis[i.country] || countriesIsNeutral.Contains(i.country)) && !noReducedResources) {
						countryDatas[i.country].manpower += 7;
					} else {
						countryDatas[i.country].manpower += 10;
					}
				}
			}
			foreach (Tile i in industrialComplexes) {
				if (i && (notMultiplayer || countriesIsAxis[i.country] == countriesIsAxis[playerCountry] || isHost && countriesIsAxis[i.country] != opponentTeam)) {
					if (((playerCountry != i.country || deltaRoundAuto) && playerIsAxis == countriesIsAxis[i.country] || countriesIsNeutral.Contains(i.country)) && !noReducedResources) {
						countryDatas[i.country].industry += 7;
					} else {
						countryDatas[i.country].industry += 10;
					}
				}
			}

			foreach (Unit i in soldiers) {
				if (notMultiplayer || countriesIsAxis[i.currentTile.country] == countriesIsAxis[playerCountry] || isHost && countriesIsAxis[i.currentTile.country] != opponentTeam)
					if (i.troopType == Troop.fortification && i.tier > 1)
						i.tier--;
			}

		} catch (Exception e) {
			print(e);
		}
		List<Unit> tempSoldiers = new List<Unit>(soldiers);
		int index = 0;
		foreach (Unit i in tempSoldiers) {
			if (i == null || i.gameObject == null || i.currentTile == null)
				continue;
			//if multiplayer only do player's allies
			if (myPlayerPrefs.GetInt("multiplayer") == 1 && countriesIsAxis[playerCountry] != countriesIsAxis[i.country] &&
				(myPlayerPrefs.GetInt("isHost") == 0 || countriesIsAxis[myPlayerPrefs.GetString("opponentCountry")] == countriesIsAxis[i.country]))
				continue;

			index++;

			try {
				i.passRound();
			} catch (Exception e) { print(e); }

			try {
				if ((i.country != playerCountry || deltaRoundAuto) && !i.isNeutral) {
					//ai movement
					Tile closestTile = null;
					float closestDistance = Mathf.Infinity;
					bool foundCity = false;
					bool haveFuel = countryDatas[i.country].fuel >= i.movementFuelCost * i.tier;
					if (haveFuel && i.movement != 0 && !i.isLocked)
						foreach (City j in cities) {
							if (i == null || j == null || j.currentTile == null)
								continue;
							if ((j.isPort || i.troopType != Troop.navy) && countriesIsAxis[j.currentTile.country] != i.isAxis &&
								(i.health <= 0f || i.tier == 0) && j.currentTile.occupant == null && Vector2.Distance(j.transform.position, i.transform.position) < 5f) {
								if (i.currentTile.aiFindPath(i.movement, j.currentTile, true, false) != null) {
									float strategicMultiplayer = j.isStrategic ? 2f : 1f;
									if ((i.currentTile.country != playerCountry || deltaRoundAuto) && !i.currentTile.isCity || (i.currentTile.city.isPort &&
										countryDatas[i.currentTile.country].manpower >= productBar.CalculateTroopManpowerCost(5, 0, i.currentTile.country) &&
										countryDatas[i.currentTile.country].industry >= productBar.CalculateTroopIndustryCost(5, 0, i.currentTile.country) ||
										!i.currentTile.city.isPort && countryDatas[i.currentTile.country].manpower >= productBar.CalculateTroopManpowerCost(0, 0, i.currentTile.country) * strategicMultiplayer * 1.5f &&
										countryDatas[i.currentTile.country].industry >= productBar.CalculateTroopIndustryCost(0, 0, i.currentTile.country) * strategicMultiplayer)) {


										Tile c = i.currentTile;
										foundCity = true;
										List<Vector2> myList = i.currentTile.aiFindPath(i.movement, j.currentTile, false, true);
										i.moveToDestination(myList, true, i.currentTile);

										if (c.isCity && c.country == i.country)
											aiProduceTroop(c.city, c.country);
										break;
									}
								}
							}
						}
					if (!foundCity && haveFuel && i.movement != 0 && !i.isLocked) {
						foreach (Tile j in oilRigs.Concat(villages).Concat(industrialComplexes)) {
							if (j == null)
								continue;
							if (i != null && i.troopType != Troop.navy && countriesIsAxis[j.country] != i.isAxis && !countriesIsNeutral.Contains(j.country) &&
								j.occupant == null && Vector2.Distance(j.transform.position, i.transform.position) < 5f) {
								if (i.currentTile.aiFindPath(i.movement, j, true, false) != null) {
									float strategicMultiplayer = 1f;
									if (i.currentTile.isCity)
										strategicMultiplayer = i.currentTile.city.isStrategic ? 2f : 1f;
									if (!i.currentTile.isCity || (i.currentTile.city.isPort && countryDatas[i.currentTile.country].manpower >= productBar.CalculateTroopManpowerCost(5, 0, i.currentTile.country) &&
										countryDatas[i.currentTile.country].industry >= productBar.CalculateTroopIndustryCost(5, 0, i.currentTile.country) ||
										!i.currentTile.city.isPort && countryDatas[i.currentTile.country].manpower >= productBar.CalculateTroopManpowerCost(0, 0, i.currentTile.country) * strategicMultiplayer * 1.5f &&
										countryDatas[i.currentTile.country].industry >= productBar.CalculateTroopIndustryCost(0, 0, i.currentTile.country) * strategicMultiplayer)) {
										Tile c = i.currentTile;
										foundCity = true;
										List<Vector2> myList = i.currentTile.aiFindPath(i.movement, j, false, true);
										i.moveToDestination(myList, true, i.currentTile);
										if (c.isCity && c.country == i.country)
											aiProduceTroop(c.city, c.country);
										break;
									}
								}
							}
						}
					}
					if (!foundCity) {
						i.checkAttack(true);
						if (!i.canAttack && haveFuel && i.movement != 0 && !i.isLocked) {
							if (!i.canAttack) {
								//inch closer to any generic city or nearby enemy
								foreach (City j in cities) {
									if (j == null)
										continue;
									if ((j.isPort || i.troopType != Troop.navy) && !countriesIsNeutral.Contains(j.currentTile.country) && countriesIsAxis[j.currentTile.country] != i.isAxis &&
										(Vector2.Distance(j.transform.position, i.transform.position) <= aiMoveThreshold + 0.3f || i.isAxis == playerIsAxis) &&
										Vector2.Distance(j.transform.position, i.transform.position) < closestDistance) {

										closestDistance = Vector2.Distance(j.transform.position, i.transform.position);
										closestTile = j.currentTile;
									}
								}
								if (!i.canAttack) {
									//for navy to search for naval
									if (i.troopType == Troop.navy) {
										foreach (Unit j in soldiers) {
											if (j == null)
												continue;

											if (j.isAxis != i.isAxis && !j.isNeutral &&
												(Vector2.Distance(j.transform.position, i.transform.position) <= aiMoveThreshold + 0.3f || i.isAxis == playerIsAxis) && j.currentTile.terrain == Terrain.water &&
												Vector2.Distance(j.transform.position, i.transform.position) < closestDistance) {

												closestDistance = Vector2.Distance(j.transform.position, i.transform.position);

												closestTile = j.currentTile;
											}
										}
									}
									foreach (Unit j in soldiers) {
										if (j == null)
											continue;

										if (j.isAxis != i.isAxis && !j.isNeutral &&
											(Vector2.Distance(j.transform.position, i.transform.position) <= aiMoveThreshold + 0.3f ||
											i.isAxis == playerIsAxis) && (i.troopId != 12 || j.currentTile.terrain == Terrain.water) &&
											Vector2.Distance(j.transform.position, i.transform.position) < closestDistance) {

											closestDistance = Vector2.Distance(j.transform.position, i.transform.position);

											closestTile = j.currentTile;
										}
									}
								}
							}
							if (closestTile != null && i.currentTile != null && haveFuel) {
								float strategicMultiplayer = 1f;
								if (i.currentTile.isCity)
									strategicMultiplayer = i.currentTile.city.isStrategic ? 2f : 1f;
								if (!i.currentTile.isCity || (i.currentTile.city.isPort && countryDatas[i.currentTile.country].manpower >= productBar.CalculateTroopManpowerCost(5, 0, i.currentTile.country) &&
										countryDatas[i.currentTile.country].industry >= productBar.CalculateTroopIndustryCost(5, 0, i.currentTile.country) || !i.currentTile.city.isPort &&
										countryDatas[i.currentTile.country].manpower >= productBar.CalculateTroopManpowerCost(0, 0, i.currentTile.country) * strategicMultiplayer * 1.5f &&
										countryDatas[i.currentTile.country].industry >= productBar.CalculateTroopIndustryCost(0, 0, i.currentTile.country) * strategicMultiplayer)) {
									Tile c = i.currentTile;
									i.moveToDestination(i.currentTile.aiFindPath(i.movement, closestTile, false, false), true, c);
									i.checkAttack(true);
									if (c.isCity && c.country == i.country) {
										//produce troops in city if unit left from said city; otherwise, don't allow unit to leave city 
										aiProduceTroop(c.city, c.country);
									}
								}
							}
						}
					}
					if (i.canAttack) {
						//attack here
						float damage = 0, cityDamage = 0;
						bool isCrit = false;
						if (i.closestTarget != null) {
							damage = calculateDamage(i, i.closestTarget); //automatically attack city if there is occupant
						} else {
							cityDamage = CalculateCityDamage(i);
						}

						if (CheckGeneralCrit(i)) {
							isCrit = true;
							damage *= 2f;
							cityDamage *= 1.5f;
						}

						i.RotateToTarget(i.closestTarget == null ? i.closestCity.currentTile : i.closestTarget.currentTile);
						if (i.closestTarget == null) {
							i.attack(0f, cityDamage, i.closestCity.currentTile, false, isCritical: isCrit);
						} else
							i.attack(0f, damage, i.closestTarget.currentTile, false, isCritical: isCrit);

						i.attacked = true;
						i.canAttack = false;
						i.moved = true;
						checkRocketArtilleryDamage(i.closestTarget == null ? i.closestCity.currentTile : i.closestTarget.currentTile, i, damage == 0 ? cityDamage : damage, 0f, false);

						//retaliation (no - damage becauese damage is instant), not possible with cities yet

						if (!CheckRetaliationChance(i) && i.closestTarget != null && i.closestTarget.troopId != 20 && (i.troopId != 21 || i.closestTarget.troopId != 12) /*submarines can't retaliate against coastal artillery*/
								&& i.troopId != 12 && i.troopId != 20 && i.troopId != 14 && i.closestTarget.troopId != 14 &&
								(i.closestTarget.troopType != Troop.fortification || i.closestTarget.tier == 1) && i.closestTarget.health > 0f &&
								(i.troopType != Troop.artillery || i.currentTile.terrain == Terrain.water) && i.closestTarget.range >= Vector2.Distance(i.transform.position, i.closestTarget.transform.position) - 0.35f) {
							i.closestTarget.RotateToTarget(i.currentTile);
							bool crt = false;
							float dmg = calculateDamage(i.closestTarget, i);
							if (CheckGeneralCrit(i.closestTarget)) {
								isCrit = true;
								dmg *= 2f;
							}
							i.closestTarget.attack(0.25f, dmg, i.currentTile, false, isCritical: crt);
							checkRocketArtilleryDamage(i.currentTile, i.closestTarget, dmg, 0.25f, false, isCritical: crt);
						}
					}
				}
			} catch (Exception e) {
				print("pass round error: " + e);
			}
			if (!deltaRoundAuto && index % 2 == 0 || index % 5 == 0)
				yield return null;
		}
		//ai general assignment
		try {
			if (myPlayerPrefs.GetInt("multiplayer") == 0) {
				Dictionary<string, CountryData> temporaryCountryData = new Dictionary<string, CountryData>();
				foreach (string i in countryDatas.Keys) {
					temporaryCountryData.Add(i, new CountryData(countryDatas[i]));
				}

				foreach (Unit i in soldiers) {
					try {
						if (i != null && i.general != "default" && i.general != "" && temporaryCountryData[i.country].aiGenerals != null &&
							temporaryCountryData[i.country].aiGenerals.ContainsKey(i.general)) {
							//remove general from bank of assignable units
							temporaryCountryData[i.country].aiGenerals.Remove(i.general);
						}
					} catch (Exception e) {
						print(e);
					}
				}
				foreach (Unit i in soldiers) {
					try {
						if (i != null && i.country != playerCountry && (i.general == "default" || i.general == "") && temporaryCountryData[i.country].aiGenerals != null &&
							temporaryCountryData[i.country].aiGenerals.Count > 0 && i.troopType != Troop.fortification && i.health / i.maxHealth > 0.5f &&
							(i.tier > 1 || i.troopId == 16 || i.troopId == 14 || i.troopId == 2 || i.troopId == 19 || i.troopId == 13 || i.troopId == 9)) {
							//add if tier is 2 or more or if it is battleship, carrier, cruiser, modern tank, heavy tank, or medium tank

							int chosenGeneral = -1;
							int generalIndex = 0;

							//generals should only be assigned to fitting category
							foreach (string j in temporaryCountryData[i.country].aiGenerals.Keys) {
								//print(j);
								if (j != "" && j != "default" && ((
									playerData.generals[j].skillBranch == General.GeneralBranch.Infantry ||
									playerData.generals[j].skillBranch == General.GeneralBranch.Air ||
									playerData.generals[j].skillBranch == General.GeneralBranch.InfantryArtillery ||
									playerData.generals[j].skillBranch == General.GeneralBranch.InfantryPanzer) && i.troopType == Troop.infantry ||
									(playerData.generals[j].skillBranch == General.GeneralBranch.Panzer ||
									playerData.generals[j].skillBranch == General.GeneralBranch.AirNavy ||
									playerData.generals[j].skillBranch == General.GeneralBranch.InfantryPanzer) && i.troopType == Troop.armor ||
									(playerData.generals[j].skillBranch == General.GeneralBranch.Artillery ||
									playerData.generals[j].skillBranch == General.GeneralBranch.InfantryArtillery) && i.troopType == Troop.artillery ||
									(playerData.generals[j].skillBranch == General.GeneralBranch.Navy ||
									playerData.generals[j].skillBranch == General.GeneralBranch.AirNavy) && i.troopType == Troop.navy
									)) {
									chosenGeneral = generalIndex;
									break;
								}
								generalIndex++;
							}

							if (chosenGeneral != -1) {
								i.general = temporaryCountryData[i.country].aiGenerals.ToList()[chosenGeneral].Key;
								i.generalLevel = temporaryCountryData[i.country].aiGenerals.ToList()[chosenGeneral].Value;
								i.updateGeneral(true);

								//print(i.general);
								temporaryCountryData[i.country].aiGenerals.Remove(i.general);
							}
						}
					} catch (Exception e) {
						print(e);
					}
				}
			}
		} catch (Exception e) { print(e); }

		//ai air attack
		foreach (City i in cities) {
			try {
				if (i != null && i.currentTile != null && (myPlayerPrefs.GetInt("multiplayer") == 0 ||
					countriesIsAxis[i.currentTile.country] == countriesIsAxis[playerCountry]) && (i.currentTile.country != playerCountry || deltaRoundAuto)
					&& !countriesIsNeutral.Contains(i.currentTile.country) && countryDatas[i.currentTile.country].manpower > 35 &&
					countryDatas[i.currentTile.country].industry > 100 && countryDatas[i.currentTile.country].fuel > 75 && i.airportTier > 0) {
					Collider2D[] t = Physics2D.OverlapCircleAll(i.transform.position, i.airportTier >= 3 ?
						(10 + CheckTech(TechTroopType.Air, countryDatas[i.currentTile.country].techLevel, TechCategory.Range)) :
						(7 + CheckTech(TechTroopType.Air, countryDatas[i.currentTile.country].techLevel, TechCategory.Range)));
					int fattestStack = 0;
					Unit bestTarget = null;
					foreach (Collider2D j in t) {
						if (j.GetComponent<Tile>() != null && j.GetComponent<Tile>().occupant != null &&
							!j.GetComponent<Tile>().occupant.isNeutral && j.GetComponent<Tile>().occupant.isAxis != countriesIsAxis[i.currentTile.country]) {
							if (j.GetComponent<Tile>().occupant.tier > fattestStack) {
								bestTarget = j.GetComponent<Tile>().occupant;
								fattestStack = j.GetComponent<Tile>().occupant.tier;
							}
						}
					}
					if (bestTarget != null) {
						if (i.airportTier >= 3) {
							aiAirStrike(i, i.currentTile.country, bestTarget, 3);
						} else
							aiAirStrike(i, i.currentTile.country, bestTarget, 0);
					}
				}
			} catch (Exception e) {
				print(e);
			}
		}

		//make a delay timer that catches errors
		StartCoroutine(EndOfRoundAIMustDo());
	}
	IEnumerator EndOfRoundAIMustDo() {
		foreach (Tile t in tiles.Values) {
			if (t != null) {
				t.movable = false;
			}
		}
		if (myPlayerPrefs.GetInt("multiplayer") == 0) { //all AI actions are only for single player
			round++;

			try {
				foreach (Unit i in soldiers) {
					if (i != null && i.country == playerCountry) {
						if (!autoSkipRoundToggle.isOn && deltaRoundAuto) {
							i.moved = false;
							i.supplied = false;
							i.attacked = false;
						}

						i.checkAttack(true);
					}
				}
			} catch (Exception e) { print(e); }
			deltaRoundAuto = autoSkipRoundToggle.isOn;
			EndRound();
		} else { //multiplayer components
				 //multiplayer does not end round here; it only ends round when it is player's turn again
			for (float i = 0f; i < 1.5f; i += Time.deltaTime)
				yield return null;
			UploadMap(false);
			multiplayerAIMovement = false;
		}
	}
	public bool CheckRetaliationChance(Unit i) {
		if (i.general != "") {
			if (playerData.generals[i.general].ContainsPerk(General.GeneralPerk.Guerilla) && i.troopType == Troop.infantry) {
				int random = UnityEngine.Random.Range(0, 100);
				if (random < (int)(GeneralManagerPopup.GeneralPerksCalculation(General.GeneralPerk.Guerilla, i.generalLevel) * 100)) {
					//print("raid!");
					return true;
				}
			} else if (playerData.generals[i.general].ContainsPerk(General.GeneralPerk.Blitzkrieg) && i.troopType == Troop.armor) {
				int random = UnityEngine.Random.Range(0, 100);
				//                print(random + ", " + (int)(GeneralManagerPopup.GeneralPerksCalculation(General.GeneralPerk.Blitzkrieg, i.generalLevel) * 100f));
				if (random < (int)(GeneralManagerPopup.GeneralPerksCalculation(General.GeneralPerk.Blitzkrieg, i.generalLevel) * 100f)) {
					//print("blitzkrieg!");
					return true;
				}
			}
		}
		return false;
	}
	public bool CheckGeneralCrit(Unit i) {
		if (i.general != "") {
			if (playerData.generals[i.general].ContainsPerk(General.GeneralPerk.Infantry) && i.troopType == Troop.infantry) {
				int random = UnityEngine.Random.Range(0, 100);
				if (random < (int)(GeneralManagerPopup.GeneralPerksCalculation(General.GeneralPerk.Infantry, i.generalLevel) * 100)) {
					return true;
				}
			} else if (playerData.generals[i.general].ContainsPerk(General.GeneralPerk.Armor) && i.troopType == Troop.armor) {
				int random = UnityEngine.Random.Range(0, 100);
				if (random < (int)(GeneralManagerPopup.GeneralPerksCalculation(General.GeneralPerk.Armor, i.generalLevel) * 100)) {
					return true;
				}
			} else if (playerData.generals[i.general].ContainsPerk(General.GeneralPerk.Artillery) && i.troopType == Troop.artillery) {
				int random = UnityEngine.Random.Range(0, 100);
				if (random < (int)(GeneralManagerPopup.GeneralPerksCalculation(General.GeneralPerk.Infantry, i.generalLevel) * 100)) {
					return true;
				}
			} else if (playerData.generals[i.general].ContainsPerk(General.GeneralPerk.Navy) && i.troopType == Troop.navy) {
				int random = UnityEngine.Random.Range(0, 100);
				if (random < (int)(GeneralManagerPopup.GeneralPerksCalculation(General.GeneralPerk.Infantry, i.generalLevel) * 100)) {
					return true;
				}
			}
		}
		return false;
	}
	public bool downloadedMapBinary;
	public BinaryData lastExportedFile = null;
	public void EndRound() {
		downloadedMapBinary = false;

		checkVictoryConditions();
		if (!gameOver) {
			if (!autoSkipRoundToggle.isOn) {
				passRoundPopup();
				CheckEvent(true);
			} else {
				CheckEvent(false);
			}
			SaveGame();
			playerData.saveFile();
			myPlayerPrefs.SaveData();
		}
		if (myPlayerPrefs.GetInt("multiplayer") == 1) {
			foreach (Unit i in soldiers) {
				if (i != null) {
					i.moved = false;
					i.supplied = false;
					i.attacked = false;
					i.checkAttack(true);
				}
			}
		}
		passingRound = false;
		if (autoSkipRoundToggle.isOn && !gameOver) {
			actuallyPassRound();
		}
	}
	void checkRocketArtilleryDamage(Tile target, Unit self, float damage, float delay, bool animate, bool isCritical = false) {
		if (self.troopId == 18) {
			//extra damage
			Tile damageTile1, damageTile2;
			if (self.currentTile.transform.position.x <= target.transform.position.x + 0.1f && self.currentTile.transform.position.y <= target.transform.position.y ||
				self.currentTile.transform.position.x < target.transform.position.x - 0.1f && self.currentTile.transform.position.y > target.transform.position.y) {
				damageTile1 = target.lowerRight;
				damageTile2 = target.upperRight;
			} else {
				damageTile1 = target.upperLeft;
				damageTile2 = target.lowerLeft;
			}
			if (damageTile1 != null && damageTile1.occupant != null && damageTile1.occupant.isAxis != self.isAxis) {
				self.currentTile.occupant.attack(delay, damage * (UnityEngine.Random.Range(20, 36) / 100f), damageTile1, animate, isCritical);
			} else if (damageTile1 != null && damageTile1.isCity && damageTile1.city.health > 0f && damageTile1.city.tier > 0 && countriesIsAxis[damageTile1.country] != self.isAxis) {
				self.currentTile.occupant.attack(delay, damage * (UnityEngine.Random.Range(20, 36) / 100f), damageTile1, animate, isCritical);
			}
			if (damageTile2 != null && damageTile2.occupant != null && damageTile2.occupant.isAxis != self.isAxis) {
				self.currentTile.occupant.attack(delay, damage * (UnityEngine.Random.Range(20, 36) / 100f), damageTile2, animate, isCritical);
			} else if (damageTile2 != null && damageTile2.isCity && damageTile2.city.health > 0f && damageTile2.city.tier > 0 && countriesIsAxis[damageTile2.country] != self.isAxis) {
				self.currentTile.occupant.attack(delay, damage * (UnityEngine.Random.Range(20, 36) / 100f), damageTile2, animate, isCritical);
			}


		}
	}
	public bool CheckCountryCapitulate(string country, string newOwner) {
		foreach (Unit i in soldiers) {
			if (i != null && i.country == country)
				return false;
		}
		foreach (City i in cities) {
			if (i != null && i.currentTile != null && i.currentTile.country == country)
				return false;
		}
		ControlAllTilesOfCountry(country, newOwner);
		return true;
	}
	//used for capitulation (when all units and cities/ports have been taken)
	void ControlAllTilesOfCountry(string country, string newOwner) {
		foreach (Tile i in tiles.Values) {
			if (i.country == country) {
				i.country = newOwner;
				i.updateTileColor();
			}
		}
	}
	public bool gameOver = false;
	public IEnumerator removePopupRestraint() {
		popupEnabled = true;
		yield return null;
		popupEnabled = false;
	}
	bool cannotGameOver = false; //if this is set to true then the mission will not have an ending due to a condition error so the map is still playable

	public void checkVictoryConditions(bool ignoreStrategicCities = false) {
		if (roundLimit == 0)
			roundLimit = 10000;
		if (!passingRound) {
			CheckEvent(true);
		}
		bool noStrategicCities = true;
		//multiplayer: should NOT have strategic soldiers or defensive missions (only conquest/take all)
		if (victoryCondition == 0) {

			bool enemyOwnsAllCities = true;
			bool playerOwnsAllCities = true;
			foreach (City i in cities) {
				if (noStrategicCities && i.isStrategic) {
					noStrategicCities = false;
				}
				if (i.isStrategic && (countriesIsAxis[i.currentTile.country] != countriesIsAxis[playerCountry]/* || countriesIsNeutral.Contains(i.currentTile.country)*/)) {
					playerOwnsAllCities = false;
					break;
				}
			}
			foreach (Unit i in soldiers) {
				if (noStrategicCities && i.isStrategic) {
					noStrategicCities = false;
				}
				if (i.isStrategic && (countriesIsAxis[i.currentTile.country] != countriesIsAxis[playerCountry]/* || countriesIsNeutral.Contains(i.currentTile.country)*/)) {
					playerOwnsAllCities = false;
					break;
				}
			}

			foreach (City i in cities) {
				if (i.isStrategic && countriesIsAxis[i.currentTile.country] == countriesIsAxis[playerCountry]) {
					enemyOwnsAllCities = false;
					break;
				}
			}
			foreach (Unit i in soldiers) {
				if (noStrategicCities && i.isStrategic) {
					noStrategicCities = false;
				}
				if (i.isStrategic && i.isAxis != countriesIsAxis[playerCountry]) {
					playerOwnsAllCities = false;
					break;
				}
			}
			if (noStrategicCities && !cannotGameOver && !ignoreStrategicCities) {
				cannotGameOver = true;
				print("broken winning condition");
			}
			if (!cannotGameOver) {
				if (enemyOwnsAllCities && myPlayerPrefs.GetInt("multiplayer") == 1) {
					endGamePopup(false);
				} else if (playerOwnsAllCities) {
					//win game
					endGamePopup(true);
					print("you win");
				} else if (round > roundLimit && myPlayerPrefs.GetInt("multiplayer") == 0) { //ignore rounds in multiplayer
					endGamePopup(false);
					print("you lose");
				}
			}
		} else if (victoryCondition == 1) {
			bool playerOwnsAllCities = true;
			foreach (City i in cities) {
				if (noStrategicCities && i.isStrategic) {
					noStrategicCities = false;
				}
				if (i.isStrategic && countriesIsAxis[i.currentTile.country] != countriesIsAxis[playerCountry]) {
					playerOwnsAllCities = false;
					break;
				}
			}
			foreach (Unit i in soldiers) {
				if (noStrategicCities && i.isStrategic) {
					noStrategicCities = false;
					break;
				}
			}
			if (noStrategicCities && !cannotGameOver) {
				cannotGameOver = true;
				print("broken winning condition");
			}
			if (!cannotGameOver) {
				if (playerOwnsAllCities) {
					if (round > roundLimit) {
						//win game
						endGamePopup(true);
						print("you win");
					}
				} else {
					endGamePopup(false);
					print("you lose");
				}
			}
		}
	}
	void aiAirStrike(City sender, string senderCountry, Unit target, int airstrikeType) {
		countryDatas[senderCountry].industry -= productBar.CalculateTroopIndustryCost(3, airstrikeType, senderCountry);
		countryDatas[senderCountry].manpower -= productBar.CalculateTroopManpowerCost(3, airstrikeType, senderCountry);

		float damage = calculateAirStrikeDamage(sender, productBar.airplanes[airstrikeType].GetComponent<AirplaneAnimator>(), target);
		airplaneType = airstrikeType;
		if (airstrikeType == 3 && usedNukes && target.health > 200f && countryDatas[senderCountry].nukes[0] > 0) {
			countryDatas[senderCountry].nukes[0]--;
			damage = UnityEngine.Random.Range(450f, 550f);
			damage *= 1f - CheckAirDefences(sender.currentTile, target.currentTile, false);
			StartCoroutine(EnableFallout(0f, target.currentTile, 3));
			foreach (Tile t in target.currentTile.neighbors) {
				StartCoroutine(EnableFallout(0f, t, 2));
				if (t.occupant != null) {
					AIAirStrikeDamageTarget(t, damage * 0.15f);
				}
			}
			AIAirStrikeDamageTarget(target.currentTile, damage);
			print("AI use nuke bombing on " + target.currentTile.coordinate.x + ", " + target.currentTile.coordinate.y);
		} else if (airstrikeType == 3 && usedNukes && target.health > 300f && countryDatas[senderCountry].nukes[1] > 0) {
			countryDatas[senderCountry].nukes[1]--;
			damage = UnityEngine.Random.Range(920f, 1082f);
			damage *= 1f - CheckAirDefences(sender.currentTile, target.currentTile, false);
			StartCoroutine(EnableFallout(0f, target.currentTile, 3));
			foreach (Tile t in target.currentTile.neighbors) {
				StartCoroutine(EnableFallout(0f, t, 2));
				if (t.occupant != null) {
					AIAirStrikeDamageTarget(t, damage * 0.15f);
				}
			}
			AIAirStrikeDamageTarget(target.currentTile, damage);
			print("AI use nuke bombing on " + target.currentTile.coordinate.x + ", " + target.currentTile.coordinate.y);
		} else {
			damage *= 1f - CheckAirDefences(sender.currentTile, target.currentTile, false);
			AIAirStrikeDamageTarget(target.currentTile, damage);
		}

	}
	private void AIAirStrikeDamageTarget(Tile t, float damage) {
		Unit target = t.occupant;
		if (t.isCity && t.city.health > 0f && target) {
			t.city.health -= damage * 0.7f;
			target.health -= damage * 0.5f;
		} else if (target) {
			target.health -= damage;
		} else if (!target && t.isCity && t.city.health > 0f) {
			t.city.health -= damage;
		}
		if (target && target.health <= 0f && target.currentTile != null) {
			TargetUnitDied(target);
		}
	}
	private void TargetUnitDied(Unit target) {
		Tile c = target.currentTile;

		bool i = target.isStrategic;
		int s = target.isAxis;
		soldiers.Remove(target);
		Destroy(target.gameObject);
		calculatePlayerPopulation();
		if (i) {
			if (s == playerIsAxis) {
				endGamePopup(false);
			} else {
				checkVictoryConditions(true);
			}
		}
		c.resetTilePathfinding(false);
		c.canBeAttacked = false;
		c.occupant = null;

		c.hexRendererColor = c.defaultTileColor;

		if (c.occupant != null)
			c.occupant.StartCoroutine(c.occupant.delayCheck(c, false));
	}
	bool paratrooping = false;
	IEnumerator paratroopDelay(Tile sender, Tile target) {
		paratrooping = true;
		for (float i = 0f; i < 1.33f; i += Time.deltaTime) {

			yield return null;
		}
		paratrooping = false;
		sender.occupant.teleportToDestination(target);
		checkVictoryConditions();
		inAirStrike = false;
	}
	//call when killing up a unit
	private void KillUnit(Unit target, bool airStrike = false) {
		try {
			Tile c = target.currentTile;
			target.StartCoroutine(target.delayCheck(c, true));
			c.resetTilePathfinding(false);
			if (!airStrike || !c.isCity || c.city.health <= 0f)
				c.canBeAttacked = false;
			c.hexRendererColor = c.defaultTileColor;
			c.occupant = null;



			//c.GetComponentInChildren<SpriteRenderer>().color = c.defaultTileColor;
		} catch (Exception e) {
			print(e);
		}
	}

	void DoCityStrike(City target, float damage, bool onlyCity) {
		StartCoroutine(CityHealthDisplay((int)damage, new Vector3(target.transform.position.x, target.transform.position.y, -300f), onlyCity));
		target.health -= damage;
		if (target.health <= 0f && target.currentTile.occupant == null) {
			target.currentTile.canBeAttacked = false;
			target.currentTile.hexRendererColor = target.currentTile.defaultTileColor;
		}
	}
	IEnumerator CityHealthDisplay(int damage, Vector3 position, bool onlyCity) {
		if (!onlyCity) {
			for (float i = 0; i < 0.75f; i += Time.deltaTime) {
				yield return null;
			}
		}
		GameObject insItem = Instantiate(damageTextPrefab, position, Quaternion.identity);
		insItem.GetComponent<Text>().text = "-" + damage;
	}
	private void AirStrikeDelay(City sender, Tile target) {
		float damage = -1;
		bool isNuke = false;
		bool collat = false;
		if (airplaneType == 3 && nuclearWarheadDropdown.value > 0) {
			switch (nuclearWarheadDropdown.value) {
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
		}
		if (collat) {
			foreach (Tile t in target.neighbors) {
				StartCoroutine(EnableFallout(0.8f, t, 2));
				if (t.occupant != null) {
					StartCoroutine(airStrikeDelay(sender, t.occupant, damage == -1 ? -1 : damage * UnityEngine.Random.Range(0.1f, 0.2f), true));
				} else if (t.isCity && t.city.health > 0) {
					StartCoroutine(airStrikeDelay(sender, t.city, damage == -1 ? -1 : damage * UnityEngine.Random.Range(0.1f, 0.2f), true));
				}
			}
		}
		if (target.occupant) {
			if (isNuke)
				StartCoroutine(EnableFallout(0.8f, target, 3));
			StartCoroutine(airStrikeDelay(sender, target.occupant, damage));
		} else if (target.isCity) {
			if (isNuke)
				StartCoroutine(EnableFallout(1.33f, target, 3));
			StartCoroutine(airStrikeDelay(sender, target.city, damage));
		} else {
			Debug.LogWarning("air attack failed");
		}
	}
	IEnumerator EnableFallout(float delay, Tile t, int falloutAmount) {
		for (float i = 0; i < delay; i += Time.deltaTime) {
			yield return null;
		}
		t.radiationLeft = falloutAmount;
	}
	IEnumerator airStrikeDelay(City sender, City target, float damage = -1, bool isCollat = false) {
		if (damage == -1)
			damage = calculateAirStrikeDamage(sender, productBar.airplanes[airplaneType].GetComponent<AirplaneAnimator>(), target);
		if (!isCollat)
			damage *= 1f - CheckAirDefences(sender.currentTile, target.currentTile, true);
		bool didDamage = false;
		for (float i = 0f; i < 1.33f; i += Time.deltaTime) {
			if (i > 0.8f && !didDamage) {
				didDamage = true;
				DoCityStrike(target, damage, true);

			}
			yield return null;
		}
		inAirStrike = false;
	}
	IEnumerator airStrikeDelay(City sender, Unit target, float damage = -1, bool isCollat = false) {
		if (damage == -1)
			damage = calculateAirStrikeDamage(sender, productBar.airplanes[airplaneType].GetComponent<AirplaneAnimator>(), target);
		if (!isCollat)
			damage *= 1f - CheckAirDefences(sender.currentTile, target.currentTile, true);
		bool didDamage = false;
		for (float i = 0f; i < 1.33f; i += Time.deltaTime) {
			if (i > 0.8f && !didDamage) {
				if (target.currentTile.isCity && target.currentTile.city.health > 0f) {
					DoCityStrike(target.currentTile.city, damage * 0.7f, false);
					damage *= 0.5f;
				}
				didDamage = true;
				GameObject insItem = Instantiate(damageTextPrefab, new Vector3(target.transform.position.x, target.transform.position.y, -300f), Quaternion.identity);
				insItem.GetComponent<Text>().text = "-" + (int)damage;
				target.health -= damage;
				if (target.health <= 0f && target.currentTile != null) {
					KillUnit(target, true);
				}

			}
			yield return null;
		}
		inAirStrike = false;
	}
	public float CheckAirDefences(Tile originTile, Tile target, bool animateRetaliation) {
		//check target tile itself as well as neighboring tiles
		List<Tile> tempTiles = new List<Tile>(target.neighbors);
		tempTiles.Add(target);
		float totalDefenceValue = 0f;
		foreach (Tile i in tempTiles) {
			if (i != null && i.occupant != null && countriesIsAxis[i.occupant.country] != countriesIsAxis[originTile.country] && (i.occupant.troopType != Troop.fortification || i.occupant.tier == 1)) {
				bool dontAnimate = false;
				float defenceValue = 0f;
				switch (i.occupant.troopId) {
					case 10: //bunker
						switch (airplaneType) {
							//0 strafing, 1 bombing, 3 strategic
							case 0:
								defenceValue += 20;
								break;
							case 1:
								defenceValue += 10;
								break;
							case 3:
								//don't animate because can't actually attack
								defenceValue += 0;
								dontAnimate = true;
								break;
							case 4:
								//don't animate because can't actually attack
								defenceValue += 0;
								dontAnimate = true;
								break;
						}
						break;
					case 8: //turret
						switch (airplaneType) {
							//0 strafing, 1 bombing, 3 strategic
							case 0:
								defenceValue += 25;
								break;
							case 1:
								defenceValue += 20;
								break;
							case 3:
								defenceValue += 10;
								break;
							case 4:
								//don't animate because can't actually attack
								defenceValue += 0;
								dontAnimate = true;
								break;
						}
						break;
					case 21: //coastal
						switch (airplaneType) {
							//0 strafing, 1 bombing, 3 strategic
							case 0:
								defenceValue += 25;
								break;
							case 1:
								defenceValue += 20;
								break;
							case 3:
								defenceValue += 10;
								break;
							case 4:
								//don't animate because can't actually attack
								defenceValue += 0;
								dontAnimate = true;
								break;
						}
						break;
					case 13: //cruiser
						switch (airplaneType) {
							//0 strafing, 1 bombing, 3 strategic
							case 0:
								defenceValue += 20;
								break;
							case 1:
								defenceValue += 15;
								break;
							case 3:
								//don't animate because can't actually attack
								defenceValue += 0;
								dontAnimate = true;
								break;
							case 4:
								//don't animate because can't actually attack
								defenceValue += 0;
								dontAnimate = true;
								break;
						}
						break;
					case 19: //battleship
						switch (airplaneType) {
							//0 strafing, 1 bombing, 3 strategic
							case 0:
								defenceValue += 20;
								break;
							case 1:
								defenceValue += 15;
								break;
							case 3:
								//don't animate because can't actually attack
								defenceValue += 0;
								dontAnimate = true;
								break;
							case 4:
								//don't animate because can't actually attack
								defenceValue += 0;
								dontAnimate = true;
								break;
						}
						break;

					case 20: //missile
						switch (airplaneType) {
							//0 strafing, 1 bombing, 3 strategic
							case 0:
								defenceValue += 30;
								break;
							case 1:
								defenceValue += 25;
								break;
							case 3:
								defenceValue += 20;
								break;
							case 4:
								defenceValue += 10;
								break;
						}
						break;
					default:
						dontAnimate = true;
						break;
				}
				defenceValue *= 0.7f + i.occupant.veterencyLevel / 10f;
				totalDefenceValue += defenceValue;
				if (!dontAnimate && animateRetaliation) {
					i.occupant.animateAttack(UnityEngine.Random.Range(0.15f, 0.35f), null);
					if (i != target)
						i.occupant.RotateToTarget(target);
				}
			}
		}
		totalDefenceValue = Mathf.Min(70f, totalDefenceValue);
		return totalDefenceValue / 100f;
	}
	float calculateAirStrikeDamage(City originCity, AirplaneAnimator sender, City target) {
		float myDamage = sender.damage;
		myDamage += CheckCountryTech(originCity.currentTile.country, TechTroopType.Air, TechCategory.Attack);

		if (originCity.currentTile.occupant != null && originCity.currentTile.occupant.general != "")
			myDamage *= 1f + playerData.generals[originCity.currentTile.occupant.general].airAtk[originCity.currentTile.occupant.generalLevel] / 100f;
		float damage = myDamage * UnityEngine.Random.Range(0.89f, 1.1f) * (0.5f + Mathf.Clamp(target.health / target.CalculateMaxHealth() + 0.35f, 0f, 1f) / 2f);
		damage *= 1f + sender.fortificationAttackBonus;
		return damage;
	}
	float calculateAirStrikeDamage(City originCity, AirplaneAnimator sender, Unit target) {
		float myDamage = sender.damage;
		myDamage += CheckCountryTech(originCity.currentTile.country, TechTroopType.Air, TechCategory.Attack);

		float armorDefence = target.armor - (sender.armorPierceAbility + CheckCountryTech(originCity.currentTile.country, TechTroopType.Air, TechCategory.Antiarmor));
		if (armorDefence < 0)
			armorDefence = 0;
		if (originCity.currentTile.occupant != null && originCity.currentTile.occupant.general != "")
			myDamage *= 1f + playerData.generals[originCity.currentTile.occupant.general].airAtk[originCity.currentTile.occupant.generalLevel] / 100f;
		myDamage *= 1f - armorDefence / 51f;
		float damage = myDamage * UnityEngine.Random.Range(0.89f, 1.1f) * (1 + (target.tier - 1) * 0.25f) * (0.5f + Mathf.Clamp(target.health / target.maxHealth + 0.35f, 0f, 1f) / 2f) * (1f - target.veterencyLevel * 0.07f);

		if (target.general != "" && playerData.generals[target.general].ContainsPerk(General.GeneralPerk.ShelterExpert)) {
			damage *= 1f - GeneralManagerPopup.GeneralPerksCalculation(General.GeneralPerk.ShelterExpert, target.generalLevel);
		}

		if (target.troopType == Troop.armor) {
			damage *= 1f + sender.armorAttackBonus;
		} else if (target.troopType == Troop.artillery) {
			damage *= 1f + sender.artilleryAttackBonus;
		} else if (target.troopType == Troop.artillery) {
			damage *= 1f + sender.infantryAttackBonus;
		} else if (target.troopType == Troop.fortification) {
			damage *= 1f + sender.fortificationAttackBonus;
		} else if (target.troopType == Troop.navy) {
			damage *= 1f + sender.navalAttackBonus;
		}

		damage *= 1f - target.terrainDefenceBonuses[(int)target.currentTile.terrain];
		return damage;
	}
	public float CalculateCityDamage(Unit sender, float percentageDamage = 1) { //damage on the city (percentage is not 1 if damage is shared)
																				//print("in here");

		float damage = percentageDamage * sender.damage * UnityEngine.Random.Range(0.89f, 1.1f) * Mathf.Clamp(sender.health / sender.maxHealth + 0.2f, 0f, 1f) * (1f + sender.veterencyLevel * 0.1f);
		if (sender.general != "") {
			damage *= CalculateTerrainGeneralPerksModifier(sender);
			switch (sender.troopType) {
				case Troop.armor:
					damage *= 1 + playerData.generals[sender.general].armorAtk[sender.generalLevel] / 100f;
					break;
				case Troop.infantry:
					damage *= 1 + playerData.generals[sender.general].infAtk[sender.generalLevel] / 100f;
					break;
				case Troop.artillery:
					damage *= 1 + playerData.generals[sender.general].artilleryAtk[sender.generalLevel] / 100f;
					break;
				case Troop.navy:
					if (sender.troopId == 14) {
						damage *= 1 + playerData.generals[sender.general].airAtk[sender.generalLevel] / 150f + playerData.generals[sender.general].navyAtk[sender.generalLevel] / 150f;
					} else {
						damage *= 1 + playerData.generals[sender.general].navyAtk[sender.generalLevel] / 100f;
					}
					break;
			}
		}

		damage *= 1f + sender.fortificationAttackBonus;

		if (sender.encircled)
			damage *= 0.8f;
		else if (sender.doubleEncircled)
			damage *= 0.57f;

		return damage;
	}
	public float CalculateTerrainGeneralPerksModifier(Unit sender) {
		float damage = 1f;
		if (sender.currentTile.terrain == Terrain.plains && playerData.generals[sender.general].ContainsPerk(General.GeneralPerk.Plains)) {
			damage *= 1f + GeneralManagerPopup.GeneralPerksCalculation(General.GeneralPerk.Plains, sender.generalLevel);
		} else if (sender.currentTile.terrain == Terrain.forest && playerData.generals[sender.general].ContainsPerk(General.GeneralPerk.Forest)) {
			damage *= 1f + GeneralManagerPopup.GeneralPerksCalculation(General.GeneralPerk.Forest, sender.generalLevel);
		} else if (sender.currentTile.terrain == Terrain.mountains && playerData.generals[sender.general].ContainsPerk(General.GeneralPerk.Mountains)) {
			damage *= 1f + GeneralManagerPopup.GeneralPerksCalculation(General.GeneralPerk.Mountains, sender.generalLevel);
		} else if (sender.currentTile.terrain == Terrain.snow && playerData.generals[sender.general].ContainsPerk(General.GeneralPerk.Snow)) {
			damage *= 1f + GeneralManagerPopup.GeneralPerksCalculation(General.GeneralPerk.Snow, sender.generalLevel);
		} else if (sender.currentTile.terrain == Terrain.city && playerData.generals[sender.general].ContainsPerk(General.GeneralPerk.City)) {
			damage *= 1f + GeneralManagerPopup.GeneralPerksCalculation(General.GeneralPerk.City, sender.generalLevel);
		} else if (sender.currentTile.terrain == Terrain.desert && playerData.generals[sender.general].ContainsPerk(General.GeneralPerk.DefenseExpert)) {
			damage *= 1f + GeneralManagerPopup.GeneralPerksCalculation(General.GeneralPerk.Desert, sender.generalLevel);
		}
		return damage;
	}
	//for unit info
	public float calculateBaseDamage(Unit sender) {
		float myDamage = sender.damage;
		float damage = myDamage * Mathf.Clamp(sender.health / sender.maxHealth + 0.2f, 0f, 1f);
		if (sender.general != "") {
			damage *= CalculateTerrainGeneralPerksModifier(sender);
			switch (sender.troopType) {
				case Troop.armor:
					damage *= 1 + playerData.generals[sender.general].armorAtk[sender.generalLevel] / 100f;
					break;
				case Troop.infantry:
					damage *= 1 + playerData.generals[sender.general].infAtk[sender.generalLevel] / 100f;
					break;
				case Troop.artillery:
					damage *= 1 + playerData.generals[sender.general].artilleryAtk[sender.generalLevel] / 100f;
					break;
				case Troop.navy:
					if (sender.troopId == 14) {
						damage *= 1 + playerData.generals[sender.general].airAtk[sender.generalLevel] / 150f + playerData.generals[sender.general].navyAtk[sender.generalLevel] / 150f;
					} else {
						damage *= 1 + playerData.generals[sender.general].navyAtk[sender.generalLevel] / 100f;
					}
					break;
			}
		}

		if (sender.currentTile != null)
			damage *= 1f + sender.terrainAttackBonuses[(int)sender.currentTile.terrain];

		if (sender.encircled)
			damage *= 0.8f;
		else if (sender.doubleEncircled)
			damage *= 0.57f;
		return damage;
	}
	public float calculateDamage(Unit sender, Unit target, float percentageDamage = 1) {
		float myDamage = sender.damage * percentageDamage;
		float armorDefence = target.armor - sender.armorPierce;
		if (armorDefence < 0)
			armorDefence = 0;
		myDamage *= 1f - armorDefence / 51f;
		float damage = myDamage * UnityEngine.Random.Range(0.89f, 1.1f) * Mathf.Clamp(sender.health / sender.maxHealth + 0.2f, 0f, 1f) *
			(1f + sender.veterencyLevel * 0.1f - target.veterencyLevel * 0.1f);
		if (target.general != "") {
			if (playerData.generals[target.general].ContainsPerk(General.GeneralPerk.DefenseExpert) &&
				(sender.troopType == Troop.infantry || sender.troopType == Troop.armor)) {
				damage *= 1f - GeneralManagerPopup.GeneralPerksCalculation(General.GeneralPerk.DefenseExpert, target.generalLevel);
			} else if (playerData.generals[target.general].ContainsPerk(General.GeneralPerk.ShelterExpert) && sender.troopType == Troop.artillery) {
				damage *= 1f - GeneralManagerPopup.GeneralPerksCalculation(General.GeneralPerk.ShelterExpert, target.generalLevel);
			}
		}
		if (sender.general != "") {
			damage *= CalculateTerrainGeneralPerksModifier(sender);
			switch (sender.troopType) {
				case Troop.armor:
					damage *= 1 + playerData.generals[sender.general].armorAtk[sender.generalLevel] / 100f;
					break;
				case Troop.infantry:
					damage *= 1 + playerData.generals[sender.general].infAtk[sender.generalLevel] / 100f;
					break;
				case Troop.artillery:
					damage *= 1 + playerData.generals[sender.general].artilleryAtk[sender.generalLevel] / 100f;
					break;
				case Troop.navy:
					if (sender.troopId == 14) {
						//aircraft carriers are half-half for skill use
						damage *= 1 + playerData.generals[sender.general].airAtk[sender.generalLevel] /
							150f + playerData.generals[sender.general].navyAtk[sender.generalLevel] / 150f;

						//air defence removed
						//if (target.general != "")
						//    damage *= 1 - playerData.generals[target.general].healthBonus[sender.generalLevel] / 100f;
					} else {
						damage *= 1 + playerData.generals[sender.general].navyAtk[sender.generalLevel] / 100f;
					}
					break;
			}
		}
		if (target.troopType == Troop.navy || target.currentTile != null && target.currentTile.terrain == Terrain.water) {
			damage *= 1f + sender.navalAttackBonus;
		} else if (target.troopType == Troop.armor) {
			damage *= 1f + sender.armorAttackBonus;
		} else if (target.troopType == Troop.artillery) {
			damage *= 1f + sender.artilleryAttackBonus;
		} else if (target.troopType == Troop.infantry) {
			damage *= 1f + sender.infantryAttackBonus;
		} else if (target.troopType == Troop.fortification) {
			damage *= 1f + sender.fortificationAttackBonus;
		}
		if (sender.troopId == 12 && (target.troopId == 13 || target.troopId == 14 || target.troopId == 19))
			damage *= 1.35f;
		if (sender.troopId == 11 && target.troopId == 12)
			damage *= 1.35f;


		if (target.currentTile != null)
			damage *= 1f - target.terrainDefenceBonuses[(int)target.currentTile.terrain];
		if (sender.currentTile != null)
			damage *= 1f + sender.terrainAttackBonuses[(int)sender.currentTile.terrain];
		if (sender.encircled)
			damage *= 0.8f;
		else if (sender.doubleEncircled)
			damage *= 0.57f;
		return damage;
	}
	public void CheckNukesLeft() {
		if (nuclearWarheadDropdown.value > 0 && countryDatas[playerCountry].nukes[nuclearWarheadDropdown.value - 1] <= 0) {
			nuclearWarheadDropdown.value = 0;
		}
	}

	[HideInInspector]
	public bool popupEnabled;
	bool dropdownDeltaHierarchy = false;
	public bool mouseInUI() {
		if (popupEnabled || diplomacyInstance != null || dropdownDeltaHierarchy)
			return true;
		//NOTE: CODE MAY CHANGE HERE AND THIS IS A TEMPORARY SOLUTION
		if (editMode && (editCategoryDropdown.transform.childCount > 5 || editCategory2Dropdown.transform.childCount > 4 ||
			editCategory3Dropdown.transform.childCount > 4 || editCategory4Dropdown.transform.childCount > 4))
			return true;
		if (UIRects != null) {
			for (int i = 0; i < UIRects.Length; i++) {
				if (UIRects[i].Contains(Input.mousePosition) && UIRectTransforms[i].gameObject.activeInHierarchy) {
					return true;
				}
			}
		}
		if (editMode && (editCategory == 12 || editCategory == 15)) {
			RectTransform i = editCategory == 12 ? editModeHideUI[0] : editModeEventUI[0]; //first item background
			Rect r = CustomFunctions.GeneratePointDetectionRect(new Vector2(i.position.x, i.position.y), i.rect);
			if (r.Contains(Input.mousePosition)) {
				return true;
			}
		}
		return false;
	}
	public GameObject passRoundConfirmPopupPreab;
	public void actuallyPassRound() {
		if (!passingRound) {
			countdownMultiplayer += 225;
			countdownMultiplayerText.text = (int)countdownMultiplayer + "s";
			StartCoroutine(passRound());
			if (myPlayerPrefs.GetInt("multiplayer") == 1) {
				round++;
			}

		}
	}
	IngamePopup passroundPopupObject; //instantiated check
	public void mobilePassRound() {
		if (!cantSelectTile && !passingRound && !building && !editMode && passroundPopupObject == null) {
			Transform insItem = Instantiate(passRoundConfirmPopupPreab, GameObject.Find("Canvas").transform).transform;
			passroundPopupObject = insItem.GetComponent<IngamePopup>();
			passroundPopupObject.controller = this;
			insItem.position = new Vector2(Screen.width / 2f, Screen.height / 2f);
		}
	}
	Rect[] UIRects;
	Vector2 mouseDownPosition = Vector2.zero;
	[HideInInspector]
	public bool justMoved;
	[HideInInspector]
	public Unit justMovedUnit;


	public void reverseMove() {
		deselectTile();
		justMovedUnit.reverseMove();
		justMoved = false;
	}

	public void CheckInfo() {
		//TODO: IMPLEMENT INFO CHECKING FOR SELECTEDENEMYUNIT
		UnitInfo p = Instantiate(checkInfoPrefab, GameObject.Find("Canvas").transform).GetComponent<UnitInfo>();

		p.transform.position = new Vector2(Screen.width / 2f, Screen.height / 2f);

		Unit chosenUnit = selectedEnemySoldier == null ? (selectedSoldier == null ? selectedTile.occupant : selectedSoldier) : selectedEnemySoldier;
		p.unit = chosenUnit;
		if (chosenUnit.general != "")
			p.generalImage.sprite = PlayerData.instance.generalPhotos[chosenUnit.general];
		p.SetPerks(chosenUnit.general);

		Dictionary<int, string> troopNames = new Dictionary<int, string>();
		foreach (KeyValuePair<string, int> pair in SoldierPrefabsManager.troopTypes) {
			if (pair.Value < 70) {
				troopNames.Add(pair.Value, pair.Key);
			}
		}
		p.troopTypeText.text = CustomFunctions.TranslateText(troopNames[chosenUnit.troopId]);
		p.healthText.text = ((int)chosenUnit.health).ToString();
		p.shieldText.text = ((int)chosenUnit.armor).ToString();
		p.piercingText.text = ((int)chosenUnit.armorPierce).ToString();
		p.damageText.text = ((int)calculateBaseDamage(chosenUnit)).ToString();
	}
	public void supplyTroop() {
		IngamePopup p = Instantiate(supplyPopupPrefab, GameObject.Find("Canvas").transform).GetComponent<IngamePopup>();
		p.transform.position = new Vector2(Screen.width / 2f, Screen.height / 2f);
		p.controller = this;
		int[] supplyCosts = calculateSupplyCost(selectedCity.currentTile);
		p.setText(supplyCosts[0].ToString());
		p.setText2(supplyCosts[1].ToString());
	}
	public int[] calculateSupplyCost(Tile t) {
		float manpowerCost = 0, industryCost = 0;
		Unit healingUnit = t.occupant;
		float newHealth = Mathf.Min(healingUnit.health + healingUnit.maxHealth * 0.3f, healingUnit.maxHealth); //only heal 30% now

		int purchaseCategory = (int)healingUnit.troopType;
		if (purchaseCategory == 4)
			purchaseCategory = 5; //navy unit
		for (int i = 0; i < productBar.troopManpowerCosts[purchaseCategory].Length; i++) {
			if (productBar.troopNames[purchaseCategory][i] == productBar.troopNamesByTypes[healingUnit.troopId]) {
				manpowerCost = productBar.CalculateTroopManpowerCost(purchaseCategory, i, t.occupant.country) * t.occupant.tier * 0.9f + 1f;
				industryCost = productBar.CalculateTroopIndustryCost(purchaseCategory, i, t.occupant.country) * t.occupant.tier * 0.9f + 1f;

				break;
			}
		}
		if (healingUnit.general != "" && healingUnit.general != "default") {
			//extra fees if generals have extra health
			try {
				manpowerCost *= 1f + playerData.generals[healingUnit.general].healthBonus[healingUnit.generalLevel] / 100f;
				industryCost *= 1f + playerData.generals[healingUnit.general].healthBonus[healingUnit.generalLevel] / 100f;
			} catch (Exception ex) {
				print(ex);
			}
		}
		manpowerCost *= (newHealth - healingUnit.health) / healingUnit.maxHealth;
		industryCost *= (newHealth - healingUnit.health) / healingUnit.maxHealth;

		return new int[] { (int)(manpowerCost + 0.5f), (int)(industryCost + 0.5f) };
	}
	public void confirmedSupplyTroop() {
		float newHealth = Mathf.Min(selectedCity.currentTile.occupant.health + selectedCity.currentTile.occupant.maxHealth * 0.3f, selectedCity.currentTile.occupant.maxHealth);

		countryDatas[playerCountry].manpower -= calculateSupplyCost(selectedCity.currentTile)[0];
		countryDatas[playerCountry].industry -= calculateSupplyCost(selectedCity.currentTile)[1];
		int newVeterency = (int)(selectedCity.currentTile.occupant.veterency * 0.8f);
		if (newVeterency < selectedCity.currentTile.occupant.veterency)
			selectedCity.currentTile.occupant.veterency = newVeterency;
		selectedCity.currentTile.occupant.health = newHealth;
		selectedCity.currentTile.occupant.supplied = true;
	}
	string editModeTempCountry = "";
	int editModeTempInt;
	[HideInInspector]
	public int editCategory;
	int editCityPopulation, editCityIndustry, editCityAirport, editCityNuclear, editCityDefence;

	bool hideEditUI = false;
	void toggleHideUI() {
		hideEditUI = !hideEditUI;
		if (hideEditUI) {
			foreach (RectTransform i in editModeHideUI) {
				if (i != null)
					i.transform.Translate(Vector3.up * 3000f);
			}
		} else {
			foreach (RectTransform i in editModeHideUI) {
				if (i != null)
					i.transform.Translate(Vector3.up * -3000f);
			}
		}
	}
	bool hideEventUI = false;
	void toggleEventUI() {
		hideEventUI = !hideEventUI;
		if (hideEventUI) {
			foreach (RectTransform i in editModeEventUI) {
				if (i != null)
					i.transform.Translate(Vector3.up * 3000f);
			}
		} else {
			foreach (RectTransform i in editModeEventUI) {
				if (i != null)
					i.transform.Translate(Vector3.up * -3000f);
			}
		}
	}
	//call from onvaluechanged for map editor country dropdown
	public void onDropdownChanged() {
		List<string> countries = new List<string>(countriesIsAxis.Keys);
		if (countriesIsAxis[countries[editCountryDropdown.value]] <= 2) {
			countriesIsAxisDropdown.value = countriesIsAxis[countries[editCountryDropdown.value]];
		} else {
			countriesIsAxisDropdown.value = countriesIsAxis[countries[editCountryDropdown.value]] - 1;
		}
		if (countriesIsNeutral.Contains(countries[editCountryDropdown.value])) {
			countriesIsAxisDropdown.value = 2;
		}

		techYearDropdown.value = countryDatas[countries[editCountryDropdown.value]].techLevel;

		countryOverrideDropdown.value = 0;
		int index = 0;
		foreach (Dropdown.OptionData i in countryOverrideDropdown.options) {
			if (i.image != null && i.image.name == CheckCustomFlag(countryCustomFlagOverrides, countries[editCountryDropdown.value])) {
				countryOverrideDropdown.value = index;

				break;
			}
			index++;
		}
		countryOverrideDropdown.RefreshShownValue();
		countryOverrideInput.text = CheckCustomFlag(countryCustomNameOverrides, countries[editCountryDropdown.value]);

		manpowerInput.text = countryDatas[countries[editCountryDropdown.value]].manpower.ToString();
		industryInput.text = countryDatas[countries[editCountryDropdown.value]].industry.ToString();
		fuelInput.text = countryDatas[countries[editCountryDropdown.value]].fuel.ToString();
		nuke1Input.text = countryDatas[countries[editCountryDropdown.value]].nukes[0].ToString();
		nuke2Input.text = countryDatas[countries[editCountryDropdown.value]].nukes[1].ToString();
		if (manpowerInput.text == "0") manpowerInput.text = "";
		if (industryInput.text == "0") industryInput.text = "";
		if (fuelInput.text == "0") fuelInput.text = "";
		if (nuke1Input.text == "0") nuke1Input.text = "";
		if (nuke2Input.text == "0") nuke2Input.text = "";

	}
	List<string> eventCities; //corresponds to dropdown list choice (corresponding dropdown value entered here to save the untranslated city name
							  //event dropdown change, for map editor only (will not be called in game)
	public void OnEventChanged() {
		if (events.Count == 0) //empty list of events (none available)
			return;
		OnEventTypeChanged();
		eventTargetCountryDropdown.value = new List<string>(countriesIsAxis.Keys).IndexOf(events[eventsDropdown.value].countryTarget);
		eventTargetCountryDropdown.RefreshShownValue();
		eventTypeDropdown.value = (int)events[eventsDropdown.value].eventType;
		eventTypeDropdown.RefreshShownValue();
		eventTriggerInput.text = events[eventsDropdown.value].triggerRound.ToString();
		eventTitleInput.text = events[eventsDropdown.value].title;
		eventDescriptionInput.text = events[eventsDropdown.value].description;
		eventEffectDropdown.value = events[eventsDropdown.value].eventValue;
		eventEffectDropdown.RefreshShownValue();

		EventCitiesChanged(true);
	}
	void EventCitiesChanged(bool changeDropdownValue) {
		eventConditionDropdown.options = new List<Dropdown.OptionData>();
		eventConditionDropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText("No conditions")));

		eventCities = new List<string>();
		//print("new cities");
		foreach (City i in cities) {
			if (i != null && i.cityName != "" && !eventCities.Contains(i.cityName)) {
				eventCities.Add(i.cityName);
				eventConditionDropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText(i.cityName) + " " + CustomFunctions.TranslateText("captured")));
			}
		}
		if (changeDropdownValue && events.Count > 0) {
			eventConditionDropdown.value = 0;
			for (int i = 0; i < eventCities.Count; i++) {
				if (events[eventsDropdown.value].cityCondition == eventCities[i]) {
					eventConditionDropdown.value = i + 1;
					break;
				}
			}
			eventConditionDropdown.RefreshShownValue();
		}
	}
	public void NewsPopup(GameEvent gameEvent, bool useNuclearPhoto = false) {
		eventSound.PlayOneShot(eventSound.clip, myPlayerPrefs.GetFloat("sounds") * eventSound.volume);
		Transform insItem = Instantiate(newsPopupPrefab, GameObject.Find("Canvas").transform).transform;
		IngamePopup p = insItem.GetComponent<IngamePopup>();
		p.controller = this;
		insItem.position = new Vector2(Screen.width / 2f, Screen.height / 2f);
		p.setText(CustomFunctions.TranslateText(gameEvent.title));
		p.setText2(CustomFunctions.TranslateText(gameEvent.description));
		string target = "";
		if (gameEvent.eventType != EventType.NuclearWar) {
			target = CheckCustomFlag(countryCustomNameOverrides, gameEvent.countryTarget) == "" ?
				gameEvent.countryTarget : CheckCustomFlag(countryCustomNameOverrides, gameEvent.countryTarget);
			p.secondContentText.text = CustomFunctions.TranslateText(target) + ": " +
				CustomFunctions.TranslateText(eventEffectNames[gameEvent.eventType][gameEvent.eventValue]);
		} else {
			p.secondContentText.text = CustomFunctions.TranslateText("AI will now use nukes");

		}
		switch (gameEvent.eventType) {
			case EventType.Diplomacy:
				p.setIcon(eventPhotos[0]);
				break;
			case EventType.Industry:
				if (gameEvent.eventValue < 5) {
					p.setIcon(eventPhotos[2]);
				} else {
					p.setIcon(eventPhotos[5]);
				}
				break;
			case EventType.Veterency:
				p.setIcon(eventPhotos[4]);
				break;
			case EventType.Health:
				p.setIcon(eventPhotos[4]);
				break;
			case EventType.Manpower:
				if (gameEvent.eventValue < 5) {
					p.setIcon(eventPhotos[3]);
				} else {
					p.setIcon(eventPhotos[5]);
				}
				break;
			case EventType.Fuel:
				if (gameEvent.eventValue < 5) {
					p.setIcon(eventPhotos[1]);
				} else {
					p.setIcon(eventPhotos[5]);
				}
				break;
			case EventType.NuclearWar:
				p.setIcon(eventPhotos[7]);
				break;
			default:
				break;
		}
		if (useNuclearPhoto) p.setIcon(eventPhotos[6]);
	}
	public void ConsumeEvent(GameEvent gameEvent) { //normally called with newspopup except for when ai automatching is turned on
		try {
			switch (gameEvent.eventType) {
				case EventType.Diplomacy:
					if (countriesIsNeutral.Contains(gameEvent.countryTarget)) {
						countriesIsNeutral.Remove(gameEvent.countryTarget);
					}
					switch (gameEvent.eventValue) {
						case 0:
							countriesIsAxis[gameEvent.countryTarget] = 0;
							break;
						case 1:
							countriesIsAxis[gameEvent.countryTarget] = 1;
							break;
						case 2:
							if (!countriesIsNeutral.Contains(gameEvent.countryTarget)) {
								countriesIsNeutral.Add(gameEvent.countryTarget);
							}
							countriesIsAxis[gameEvent.countryTarget] = 2;
							break;
						case 3:
							countriesIsAxis[gameEvent.countryTarget] = 4;
							break;
						case 4:
							countriesIsAxis[gameEvent.countryTarget] = 5;
							break;
						default:
							break;
					}

					if (playerCountry == gameEvent.countryTarget)
						playerIsAxis = countriesIsAxis[gameEvent.countryTarget];
					//update unit health bars and re-check all attacks
					foreach (Unit u in soldiers) {
						if (u != null) {
							try {
								u.CheckCountry();
							} catch (Exception e) {
								print(e);
							}

						}
					}

					break;
				case EventType.Veterency:
					int changeValue;
					if (gameEvent.eventValue < 3) {
						changeValue = gameEvent.eventValue + 1;
					} else {
						changeValue = -gameEvent.eventValue + 2;
					}
					foreach (Unit u in soldiers) {
						if (u != null && u.country == gameEvent.countryTarget) {
							u.setVeterencyLevel(u.veterencyLevel + changeValue);
						}
					}
					break;
				case EventType.Fuel:
					switch (gameEvent.eventValue) {
						case 0:
							countryDatas[gameEvent.countryTarget].fuel += 100;
							break;
						case 1:
							countryDatas[gameEvent.countryTarget].fuel += 200;

							break;
						case 2:
							countryDatas[gameEvent.countryTarget].fuel += 500;

							break;
						case 3:
							countryDatas[gameEvent.countryTarget].fuel += 1000;

							break;
						case 4:
							countryDatas[gameEvent.countryTarget].fuel += 2000;

							break;
						case 5:
							countryDatas[gameEvent.countryTarget].fuel -= 100;

							break;
						case 6:
							countryDatas[gameEvent.countryTarget].fuel -= 200;

							break;
						case 7:
							countryDatas[gameEvent.countryTarget].fuel -= 500;

							break;
						case 8:
							countryDatas[gameEvent.countryTarget].fuel -= 1000;

							break;
						case 9:
							countryDatas[gameEvent.countryTarget].fuel -= 2000;

							break;
						default:
							break;
					}
					break;
				case EventType.Health:
					float changeValue1;
					if (gameEvent.eventValue < 5) {
						changeValue1 = gameEvent.eventValue + 1f;
					} else {
						changeValue1 = -gameEvent.eventValue + 4f;
					}
					foreach (Unit u in soldiers) {
						if (u != null && u.country == gameEvent.countryTarget) {
							u.health += changeValue1 * 0.05f * u.maxHealth;
							if (u.health > u.maxHealth)
								u.health = u.maxHealth;
							if (u.health <= 0f) {
								KillUnit(u);
							}
						}
					}
					break;
				case EventType.Industry:
					switch (gameEvent.eventValue) {
						case 0:
							countryDatas[gameEvent.countryTarget].industry += 100;
							break;
						case 1:
							countryDatas[gameEvent.countryTarget].industry += 200;

							break;
						case 2:
							countryDatas[gameEvent.countryTarget].industry += 500;

							break;
						case 3:
							countryDatas[gameEvent.countryTarget].industry += 1000;

							break;
						case 4:
							countryDatas[gameEvent.countryTarget].industry += 2000;

							break;
						case 5:
							countryDatas[gameEvent.countryTarget].industry -= 100;

							break;
						case 6:
							countryDatas[gameEvent.countryTarget].industry -= 200;

							break;
						case 7:
							countryDatas[gameEvent.countryTarget].industry -= 500;

							break;
						case 8:
							countryDatas[gameEvent.countryTarget].industry -= 1000;

							break;
						case 9:
							countryDatas[gameEvent.countryTarget].industry -= 2000;

							break;
						default:
							break;
					}
					break;
				case EventType.Manpower:
					switch (gameEvent.eventValue) {
						case 0:
							countryDatas[gameEvent.countryTarget].manpower += 100;
							break;
						case 1:
							countryDatas[gameEvent.countryTarget].manpower += 200;

							break;
						case 2:
							countryDatas[gameEvent.countryTarget].manpower += 500;

							break;
						case 3:
							countryDatas[gameEvent.countryTarget].manpower += 1000;

							break;
						case 4:
							countryDatas[gameEvent.countryTarget].manpower += 2000;

							break;
						case 5:
							countryDatas[gameEvent.countryTarget].manpower -= 100;

							break;
						case 6:
							countryDatas[gameEvent.countryTarget].manpower -= 200;

							break;
						case 7:
							countryDatas[gameEvent.countryTarget].manpower -= 500;

							break;
						case 8:
							countryDatas[gameEvent.countryTarget].manpower -= 1000;

							break;
						case 9:
							countryDatas[gameEvent.countryTarget].manpower -= 2000;

							break;
						default:
							break;
					}
					break;
				case EventType.NuclearWar:
					usedNukes = true;
					break;
				default:
					break;
			}
		} catch (Exception e) {
			print(e);
		}
		events.Remove(gameEvent);
	}
	private readonly Dictionary<EventType, List<string>> eventEffectNames = new Dictionary<EventType, List<string>>() {
		{EventType.Diplomacy, new List<string>() {"Alliance to Allies/WP", "Alliance to Axis/NATO", "Alliance to Neutral", "Alliance to Custom Team 1", "Alliance to Custom Team 2" } },
		{EventType.Fuel, new List<string>() {"+100 fuel", "+200 fuel", "+500 fuel", "+1000 fuel", "+2000 fuel", "-100 fuel", "-200 fuel", "-500 fuel", "-1000 fuel", "-2000 fuel" } },
		{EventType.Manpower, new List<string>() {"+100 manpower", "+200 manpower", "+500 manpower", "+1000 manpower", "+2000 manpower", "-100 manpower", "-200 manpower", "-500 manpower", "-1000 manpower", "-2000 manpower" } },
		{EventType.Industry, new List<string>() {"+100 industry", "+200 industry", "+500 industry", "+1000 industry", "+2000 industry", "-100 industry", "-200 industry", "-500 industry", "-1000 industry", "-2000 industry" } },
		{EventType.Health, new List<string>() {"+5% health", "+10% health", "+15% health", "+20% health", "+25% health", "-5% health", "-10% health", "-15% health", "-20% health", "-25% health" } },
		{EventType.Veterency, new List<string>() {"+1 veterency", "+2 veterency", "+3 veterency", "-1 veterency", "-2 veterency", "-3 veterency"} },

	};
	//updates the event effect values, etc
	public void OnEventTypeChanged() {
		if (editCategory != 15)
			return;
		eventEffectDropdown.options = new List<Dropdown.OptionData>();
		if ((EventType)eventTypeDropdown.value == EventType.NuclearWar) {
			eventEffectDropdown.interactable = false;
		} else {
			eventEffectDropdown.interactable = true;

			foreach (string i in eventEffectNames[(EventType)eventTypeDropdown.value]) {
				eventEffectDropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText(i)));
			}
		}
		eventEffectDropdown.value = 0;
		eventEffectDropdown.RefreshShownValue();

	}
	private void UpdateEventDropdown() {
		//recreates the event dropdown
		eventsDropdown.options = new List<Dropdown.OptionData>();
		for (int i = 0; i < events.Count; i++) {
			eventsDropdown.options.Add(new Dropdown.OptionData((i + 1).ToString() + ". " + CustomFunctions.TranslateText("Round") + " " + events[i].triggerRound.ToString()));
		}
		eventsDropdown.RefreshShownValue();
		if (events.Count == 0) {
			//disable all the editing features
			eventTargetCountryDropdown.gameObject.SetActive(false);
			eventTypeDropdown.gameObject.SetActive(false);
			eventTriggerInput.gameObject.SetActive(false);
			eventTitleInput.gameObject.SetActive(false);
			eventDescriptionInput.gameObject.SetActive(false);
			eventEffectDropdown.gameObject.SetActive(false);
			eventConditionDropdown.gameObject.SetActive(false);
		} else {
			eventTargetCountryDropdown.gameObject.SetActive(true);
			eventTypeDropdown.gameObject.SetActive(true);
			eventTriggerInput.gameObject.SetActive(true);
			eventTitleInput.gameObject.SetActive(true);
			eventDescriptionInput.gameObject.SetActive(true);
			eventEffectDropdown.gameObject.SetActive(true);
			eventConditionDropdown.gameObject.SetActive(true);
		}
	}
	public void AddEvent() {
		events.Add(new GameEvent());
		UpdateEventDropdown();
		eventsDropdown.value = events.Count - 1;
		OnEventChanged();

		eventsDropdown.RefreshShownValue();
	}
	public void RemoveEvent() {
		if (events.Count > 0) {
			events.Remove(events[eventsDropdown.value]);
			UpdateEventDropdown();
			if (eventsDropdown.value > 0)
				eventsDropdown.value--;
			eventsDropdown.RefreshShownValue();
			OnEventChanged();
		}

	}
	bool confirmingSetTech = false;
	public void SetAllTech(Text button) {
		if (!confirmingSetTech) {
			confirmingSetTech = true;
			button.text = CustomFunctions.TranslateText("Confirm?");
			StartCoroutine(SetTechCoroutine(button));
			return;
		}
		confirmingSetTech = false;
		button.text = CustomFunctions.TranslateText("Set to All");

		foreach (CountryData c in countryDatas.Values) {
			c.techLevel = techYearDropdown.value;
		}
	}
	IEnumerator SetTechCoroutine(Text button) {
		for (float i = 0; i < 2; i += Time.deltaTime) {
			if (!confirmingSetTech)
				break;
			yield return null;
		}
		confirmingSetTech = false;
		button.text = CustomFunctions.TranslateText("Set to All");
	}
	bool confirmingDeleteEv = false;
	public void DeleteAllEvents(Text button) {
		if (!confirmingDeleteEv) {
			confirmingDeleteEv = true;
			button.text = CustomFunctions.TranslateText("Confirm?");
			StartCoroutine(DeleteEventsCoroutine(button));
			return;
		}
		confirmingDeleteEv = false;
		button.text = CustomFunctions.TranslateText("Delete Events");

		events = new List<GameEvent>();
		UpdateEventDropdown();
		eventsDropdown.RefreshShownValue();

	}
	IEnumerator DeleteEventsCoroutine(Text button) {
		for (float i = 0; i < 2; i += Time.deltaTime) {
			if (!confirmingDeleteEv)
				break;
			yield return null;
		}
		confirmingDeleteEv = false;
		button.text = CustomFunctions.TranslateText("Delete Events");

	}
	bool confirmingDeleteAll = false;
	public void DeleteAll(Text button) {
		if (deleteToggle.isOn) {
			if (!confirmingDeleteAll) {
				confirmingDeleteAll = true;
				button.text = CustomFunctions.TranslateText("Confirm?");
				StartCoroutine(DeleteAllCoroutine(button));
				return;
			}
			button.text = CustomFunctions.TranslateText("Remove All");
			confirmingDeleteAll = false;

			switch (editCategoryDropdown.value) {
				case 2: //all units
					foreach (Unit i in soldiers) {
						if (i != null)
							Destroy(i.gameObject);
					}
					break;
				case 5: //cities
					foreach (City i in cities) {
						if (i != null) {
							i.currentTile.isCity = false;
							Destroy(i.gameObject);
						}
					}
					break;
				case 13: //strategics
					foreach (City i in cities) {
						if (i != null)
							i.isStrategic = false;
					}
					foreach (Unit i in soldiers) {
						if (i != null)
							i.isStrategic = false;
					}
					break;
				case 16: //locked
					foreach (Unit i in soldiers) {
						if (i != null)
							i.isLocked = false;
					}
					break;
			}
		}
	}
	IEnumerator DeleteAllCoroutine(Text button) {
		for (float i = 0; i < 2; i += Time.deltaTime) {
			if (!confirmingDeleteAll)
				break;
			yield return null;
		}
		button.text = CustomFunctions.TranslateText("Remove All");

		confirmingDeleteAll = false;
	}
	public void onEditCategoryChanged() {
		editCategory = editCategoryDropdown.value;
		if (editCategory != 6) {
			cityNameInput.text = "";
		}
		if (editCategory == 12) {
			if (hideEditUI)
				toggleHideUI();
			if (!hideEventUI && editCategory != 15) {
				toggleEventUI();
			}
		} else {
			if (editCategory == 15 && hideEventUI || !hideEventUI && editCategory != 15) {
				toggleEventUI();
				EventCitiesChanged(true);
				UpdateEventDropdown();


				OnEventChanged();

			}
			if (!hideEditUI) {
				toggleHideUI();
			}
		}


		if (editCategory != 2 && editCategory != 5 && editCategory != 13 && editCategory != 16) {
			if (deleteToggle.transform.position.x > 0)
				deleteToggle.transform.Translate(Vector3.left * 3000f);
		} else {
			if (deleteToggle.transform.position.x < 0)
				deleteToggle.transform.Translate(Vector3.right * 3000f);
		}
		if (editCategory != 0 && editCategory != 1 && editCategory != 3 && editCategory != 4 && editCategory != 11) {
			if (editCategory4Dropdown.transform.position.x > 0)
				editCategory4Dropdown.transform.Translate(Vector3.left * 3000f);
		} else {
			if (editCategory4Dropdown.transform.position.x < 0)
				editCategory4Dropdown.transform.Translate(Vector3.right * 3000f);
		}
		if (editCategory != 14) {
			if (editCategory3Dropdown.transform.position.x > 0)
				editCategory3Dropdown.transform.Translate(Vector3.left * 3000f);
		} else {
			if (editCategory3Dropdown.transform.position.x < 0)
				editCategory3Dropdown.transform.Translate(Vector3.right * 3000f);
		}
		if (editCategory == 5 || editCategory == 12 || editCategory == 6 || editCategory == 13 || editCategory == 15 || editCategory == 16) { //not shown
			if (editCategory2Dropdown.transform.position.x > 0)
				editCategory2Dropdown.transform.Translate(Vector3.left * 3000f);
		} else {
			if (editCategory2Dropdown.transform.position.x < 0)
				editCategory2Dropdown.transform.Translate(Vector3.right * 3000f);
		}
		editCategory2Dropdown.options = new List<Dropdown.OptionData>();

		switch (editCategory) {
			case 0:
				foreach (string i in countriesIsAxis.Keys) {
					if (i != "") {
						editCategory2Dropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText(i), flags[i]));
					}
				}
				break;
			case 1:
				//editCategory2Dropdown.options = new List<Dropdown.OptionData>();
				editCategory2Dropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText("Plains")));
				editCategory2Dropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText("Forest")));
				editCategory2Dropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText("Mountains")));
				editCategory2Dropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText("Water")));
				editCategory2Dropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText("Oil")));
				editCategory2Dropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText("Desert")));
				editCategory2Dropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText("HighMountains")));
				editCategory2Dropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText("Snow")));
				editCategory2Dropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText("Village")));
				editCategory2Dropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText("Industrial Complex")));
				editCategory2Dropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText("Railroad")));




				break;
			case 2:
				foreach (KeyValuePair<string, int> i in SoldierPrefabsManager.troopTypes) {
					//not aircraft
					if (i.Value < 70)
						editCategory2Dropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText(i.Key)));
				}
				break;
			case 3:
				for (int i = 0; i < 6; i++)
					editCategory2Dropdown.options.Add(new Dropdown.OptionData(i.ToString()));
				break;
			case 4:
				for (int i = 1; i < 5; i++)
					editCategory2Dropdown.options.Add(new Dropdown.OptionData(i.ToString()));
				break;
			case 7:
				for (int i = 1; i < 6; i++)
					editCategory2Dropdown.options.Add(new Dropdown.OptionData(i.ToString()));
				break;
			case 8:
				for (int i = 0; i < 5; i++)
					editCategory2Dropdown.options.Add(new Dropdown.OptionData(i.ToString()));
				break;

			case 9:
				for (int i = 0; i < 4; i++)
					editCategory2Dropdown.options.Add(new Dropdown.OptionData(i.ToString()));
				break;
			case 10:
				for (int i = 0; i < 3; i++)
					editCategory2Dropdown.options.Add(new Dropdown.OptionData(i.ToString()));
				break;
			case 11:
				for (int i = 0; i < 6; i++)
					editCategory2Dropdown.options.Add(new Dropdown.OptionData(i.ToString()));
				break;
			case 14:
				foreach (string i in playerData.generals.Keys) {
					if (i != "default") {
						try {
							editCategory2Dropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText(i), flags[playerData.generals[i].country]));

						} catch {
							editCategory2Dropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText(i)));

							print("flag not available");
						}
					} else {
						editCategory2Dropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText(i)));

					}
				}
				break;
		}
		if (editCategory2Dropdown.options.Count > 0) {
			editCategory2Dropdown.value = 0;
			editCategory2Dropdown.captionText.text = editCategory2Dropdown.options[editCategory2Dropdown.value].text;
		}
	}
	//currently does not serve a purpose?
	public int calculatePlayerPopulation() {
		int p = 0;
		foreach (Unit i in soldiers) {
			if (i.country == playerCountry) {
				if (i.troopType != Troop.fortification)
					p += i.tier;
			}
		}
		//   countryDatas[playerCountry].technology = p;
		return p;
	}
	//player uses nuke for the first time
	public void FirstTimeNuke() {
		GameEvent myEvent = new GameEvent();

		myEvent.eventType = EventType.Veterency;
		myEvent.eventValue = 0;

		myEvent.title = CustomFunctions.TranslateText("Duck and Cover!");
		myEvent.description = CustomFunctions.TranslateText(playerCountry) + " " + CustomFunctions.TranslateText("used a nuclear weapon! [AI will now use nukes]");
		myEvent.countryTarget = playerCountry;

		NewsPopup(myEvent, true);
		ConsumeEvent(myEvent);
	}

	public void UpdateEventSelections() {
		try {
			if (events.Count > 0 && editCategory == 15) {
				//update event dropdown
				events[eventsDropdown.value].countryTarget = new List<string>(countriesIsAxis.Keys)[eventTargetCountryDropdown.value];
				events[eventsDropdown.value].title = eventTitleInput.text;
				events[eventsDropdown.value].description = eventDescriptionInput.text;
				events[eventsDropdown.value].eventValue = eventEffectDropdown.value;
				events[eventsDropdown.value].eventType = (EventType)eventTypeDropdown.value;
				int.TryParse(eventTriggerInput.text, out events[eventsDropdown.value].triggerRound);
				if (eventConditionDropdown.value == 0) {
					events[eventsDropdown.value].cityCondition = "";
					eventTriggerInput.interactable = true;
				} else {
					eventTriggerInput.interactable = false;
					eventTriggerInput.text = "0";
					events[eventsDropdown.value].cityCondition = eventCities[eventConditionDropdown.value - 1];
				}

			}
		} catch (Exception e) {
			print(e);
		}
	}

	//temporary position tracker
	List<Vector2> positions;
	List<int> alignments;

	public Vector2 lowbounds, highbounds;
	bool deltaRoundAuto = false; //if last round was still controlled by player, nothing from the player should be controlled by AI

	public static float tileColorInterpolation = 1; //this interpolation value is for tiles flashing when ready to move

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) togglePause();

		tileColorInterpolation = 0.7f + 0.2f * Mathf.Abs(Mathf.Sin(Time.time));

		lowbounds = Camera.main.ScreenToWorldPoint(Vector2.zero);
		highbounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
		if (myPlayerPrefs.GetInt("multiplayer") == 1) {
			if (!countdownMultiplayerText.enabled)
				countdownMultiplayerText.enabled = true;
			if (!passingRound) {
				if (countdownMultiplayer > 0f) {
					countdownMultiplayer -= Time.deltaTime;
					countdownMultiplayerText.text = (int)countdownMultiplayer + "s";
				} else {
					actuallyPassRound();
				}
			} else {
				try {
					int opponentTimeout = myPlayerPrefs.GetInt("playerId") == 0 ? int.Parse(multiplayerController.matchInfo.player_timeouts[1]) :
						int.Parse(multiplayerController.matchInfo.player_timeouts[0]);

					if (opponentTimeout < 62.5f) {
						countdownMultiplayerText.text = CustomFunctions.TranslateText("Opponent's turn; timeout: ") + opponentTimeout + "s";
					} else {
						countdownMultiplayerText.text = CustomFunctions.TranslateText("Opponent's turn");
					}
				} catch {
					print("game should already be ended since there is only 1 player left");
				}
			}
		} else {
			if ((passingRound || autoSkipRoundToggle.isOn) && myPlayerPrefs.GetInt("custom") == 1 && started) {
				if (!autoSkipRoundToggle.gameObject.activeInHierarchy)
					autoSkipRoundToggle.gameObject.SetActive(true);

			} else {
				if (autoSkipRoundToggle.gameObject.activeInHierarchy)
					autoSkipRoundToggle.gameObject.SetActive(false);
				if (deltaRoundAuto) { deltaRoundAuto = false; }
			}
		}
		/////////tutorial
		if (Input.GetMouseButtonDown(0)) {
			if (positions == null) {
				positions = new List<Vector2>();
			}
			if (alignments == null) {
				alignments = new List<int>();
			}

			RaycastHit2D hi = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

			if (hi.collider != null && hi.collider.GetComponent<Tile>() != null/* && !Input.GetKey(KeyCode.N)*/) {

				Tile myTile = hi.collider.GetComponent<Tile>();

				//tutorial

				//Input.mousePosition - new Vector3(Screen.width / 2f, Screen.height / 2f, 0f)
				positions.Add(myTile.coordinate);
				alignments.Add(-1);
			} else {
				if (Input.mousePosition.y < 150f) {
					alignments.Add(1);
					positions.Add(Input.mousePosition - new Vector3(Screen.width / 2f - 0.1f, 0f));
				} else if (Input.mousePosition.y > Screen.height - 150f) {
					positions.Add(Input.mousePosition - new Vector3(Screen.width / 2f - 0.1f, Screen.height, 0f));
					alignments.Add(2);

				} else {
					positions.Add(Input.mousePosition - new Vector3(Screen.width / 2f - 0.1f, Screen.height / 2f - 0.1f, 0f));
					alignments.Add(0);

				}

			}
		}
		//if (Input.GetKeyDown(KeyCode.R)) {
		//    string i = "";
		//    foreach (Vector2 p in positions) {
		//        i += "new Vector2(" + p.x + "f, " + p.y + "f),\n";
		//    }
		//    foreach (int p in alignments) {
		//        i += p + ", ";
		//    }
		//    print(i);
		//}

		myPlayerPrefs.SetFloat("realSounds", soundVolumeSlider.value);
		myPlayerPrefs.SetFloat("realMusics", musicVolumeSlider.value);
		myPlayerPrefs.SetFloat("sensitivity", sensitivitySlider.value);

		myPlayerPrefs.SetFloat("sounds", Mathf.Pow(soundVolumeSlider.value, 2f));
		myPlayerPrefs.SetFloat("musics", Mathf.Pow(musicVolumeSlider.value, 2f));

#if UNITY_EDITOR
		if (Input.GetKey(KeyCode.N) && Input.GetKey(KeyCode.W) && Input.GetKeyDown(KeyCode.I))
			endGamePopup(true);
#endif

		if (editMode && fullyInitialized) {
			editBrushSize = editCategory4Dropdown.value + 1;

			if (editCategory != 0) {
				if (showFlagToggle.transform.position.x > 0)
					showFlagToggle.transform.Translate(Vector3.left * 9000f);
			} else {
				if (showFlagToggle.transform.position.x < 0)
					showFlagToggle.transform.Translate(Vector3.right * 9000f);
			}
			if (editCategory != 2 && editCategory != 5 && editCategory != 12 && editCategory != 15 || !deleteToggle.isOn) {
				if (deleteAllButton.transform.position.x > 0)
					deleteAllButton.transform.Translate(Vector3.left * 9000f);
			} else {
				if (deleteAllButton.transform.position.x < 0)
					deleteAllButton.transform.Translate(Vector3.right * 9000f);
			}
			playerCountry = new List<string>(countriesIsAxis.Keys)[playerCountryDropdown.value];

			if (countriesIsAxisDropdown.value == 2) {
				if (!countriesIsNeutral.Contains(new List<string>(countriesIsAxis.Keys)[editCountryDropdown.value]))
					countriesIsNeutral.Add(new List<string>(countriesIsAxis.Keys)[editCountryDropdown.value]);
			} else {
				if (countriesIsNeutral.Contains(new List<string>(countriesIsAxis.Keys)[editCountryDropdown.value]))
					countriesIsNeutral.Remove(new List<string>(countriesIsAxis.Keys)[editCountryDropdown.value]);
				if (countriesIsAxisDropdown.value < 2) { //original axis/allies
					countriesIsAxis[new List<string>(countriesIsAxis.Keys)[editCountryDropdown.value]] = countriesIsAxisDropdown.value;
				} else {
					//value 3 is reserved because of old maps' neutral alliances
					countriesIsAxis[new List<string>(countriesIsAxis.Keys)[editCountryDropdown.value]] = countriesIsAxisDropdown.value + 1;
				}
			}
			if (editCategory == 12) {
				Sprite newName = countryOverrideDropdown.options[countryOverrideDropdown.value].image;

				string country = new List<string>(countriesIsAxis.Keys)[editCountryDropdown.value];

				if (!countryCustomFlagOverrides.ContainsKey(country)) {
					countryCustomFlagOverrides.Add(country, "");
				}
				if (!countryCustomNameOverrides.ContainsKey(country)) {
					countryCustomNameOverrides.Add(country, "");
				}

				countryCustomFlagOverrides[country] = newName == null ? "" : newName.name;
				countryCustomNameOverrides[country] = countryOverrideInput.text;

				countryDatas[country].techLevel = techYearDropdown.value;

				try {
					//update custom country data dropdown
					if (manpowerInput.text != "")
						countryDatas[country].manpower = int.Parse(manpowerInput.text);
					else {
						countryDatas[country].manpower = 0;
					}

					if (industryInput.text != "")
						countryDatas[country].industry = int.Parse(industryInput.text);
					else {
						countryDatas[country].industry = 0;
					}
					if (fuelInput.text != "")
						countryDatas[country].fuel = int.Parse(fuelInput.text);
					else {
						countryDatas[country].fuel = 0;
					}
					if (nuke1Input.text != "")
						countryDatas[country].nukes[0] = int.Parse(nuke1Input.text);
					else {
						countryDatas[country].nukes[0] = 0;
					}
					if (nuke2Input.text != "")
						countryDatas[country].nukes[1] = int.Parse(nuke2Input.text);
					else {
						countryDatas[country].nukes[1] = 0;
					}
				} catch {
					print("cannot parse");
				}
				try {
					aiMoveThreshold = int.Parse(aiDetectionInput.text);
				} catch {
					aiMoveThreshold = 0;
				}

				try {
					maxGenerals = int.Parse(maxGeneralsInput.text);
				} catch {
					maxGenerals = 0;
				}
				try {
					roundLimit = int.Parse(roundLimitInput.text);
				} catch {
					roundLimit = 0;
				}
			}
			UpdateEventSelections();

			missionName = missionNameInput.text;
			if (missionName == "")
				missionName = CustomFunctions.TranslateText("Untitled");
			try {
				victoryCondition = victoryConditionDropdown.value;
			} catch {
				victoryCondition = 0;
			}

		}
		if (editMode) {
			UIRectTransforms = editModeUI;
			if (nuclearWarheadDropdown.gameObject.activeInHierarchy)
				nuclearWarheadDropdown.gameObject.SetActive(false);
		} else {
			if (!passingRound && (airplaneMode && airplaneType == 3 || selectedSoldier != null && selectedSoldier.troopId == 20 && selectedSoldier.canAttack)) {
				//turn on nuclear dropdown selection
				if (!nuclearWarheadDropdown.gameObject.activeInHierarchy) {
					UpdateNuclearDisplay();
					nuclearWarheadDropdown.gameObject.SetActive(true);
				}
			} else {
				if (passingRound && airplaneMode) {
					airplaneMode = false;
				}
				if (nuclearWarheadDropdown.gameObject.activeInHierarchy) {
					nuclearWarheadDropdown.gameObject.SetActive(false);
					nuclearWarheadDropdown.value = 0;
				}
			}
		}
		if (selectedBuildTile != null && selectedBuildTile.occupant != null)
			selectedBuildTile = null;

		UIRects = new Rect[UIRectTransforms.Length];
		for (int i = 0; i < UIRectTransforms.Length; i++) {
			if (UIRectTransforms[i] != null)
				UIRects[i] = CustomFunctions.GeneratePointDetectionRect(new Vector2(UIRectTransforms[i].position.x, UIRectTransforms[i].position.y), UIRectTransforms[i].rect); //*/
		}
		if (!airplaneMode && selectedTile == null && selectedCity != null && (selectedSoldier == null || !selectedSoldier.moving)) {
			selectedTile = selectedCity.currentTile;
		}
		if (selectedTile != null && selectedTile.isCity && selectedCity == null)
			selectedCity = selectedTile.city;
		if (selectedTile != null && selectedTile.occupant != null && selectedSoldier == null && selectedTile.occupant.country == playerCountry)
			selectedSoldier = selectedTile.occupant;

		if (selectedSoldier == null) {
			canSupply = false;
		}
		supplyButtonOn = canSupply;
		if (justMoved) {
			selectedCity = null;
			canBuild = false;
			supplyButtonOn = false;
			buildButtonOn = false;

		}
		if (supplyButtonOn && selectedCity != null && (selectedCity.currentTile.occupant.moving || selectedCity.currentTile.occupant.troopType == Troop.fortification)) {
			supplyButtonOn = false;
		}

		#region FixButtons
		if (justMoved) {
			reverseButtonOn = true;
			//if (canSupply && !selectedCity.currentTile.occupant.moving && selectedCity.currentTile.occupant.troopType != Troop.fortification) {
			//    //can supply
			//    supplyButton.position = new Vector2(Screen.width / 2f + 106f * CustomFunctions.getUIScale(), supplyButton.position.y);
			//} else {
			//    //if (supplyButton.position.x > 0f)
			//    //    supplyButton.position = new Vector2(-2000f, supplyButton.position.y);
			//}
			//if (!canBuild) {
			//    //can reverse
			//    reverseButton.position = new Vector2(Screen.width / 2f, reverseButton.position.y);
			//} else
			//    reverseButton.position = new Vector2(Screen.width / 2f - 106f * CustomFunctions.getUIScale(), reverseButton.position.y);
		} else {
			reverseButtonOn = false;
			//if (reverseButton.position.x > 0f)
			//    reverseButton.position = new Vector2(-2000f, reverseButton.position.y);
			//if (canSupply && selectedCity != null && !selectedCity.currentTile.occupant.moving && selectedCity.currentTile.occupant.troopType != Troop.fortification) {
			//    if (!canBuild)
			//        supplyButton.position = new Vector2(Screen.width / 2f, supplyButton.position.y);
			//    else {
			//        supplyButton.position = new Vector2(Screen.width / 2f + 106f * CustomFunctions.getUIScale(), supplyButton.position.y);
			//    }
			//} else {
			//    if (supplyButton.position.x > 0f)
			//        supplyButton.position = new Vector2(-2000f, supplyButton.position.y);
			//}
		}
		Unit s = selectedSoldier;
		if (s == null && selectedTile != null)
			s = selectedTile.occupant;

		if (!justMoved && s != null && !airplaneMode && s.troopType != Troop.fortification && (!s.moved || s.general == "")) {
			generalsButtonOn = true;
			//can assign general
			//if (buildButton.position.x > 0 && reverseButton.position.x < 0) {
			//    generalAssignButton.position = new Vector2(Screen.width / 2f - 106f * CustomFunctions.getUIScale(), generalAssignButton.position.y);

			//} else if (buildButton.position.x > 0 && reverseButton.position.x > 0) {
			//    generalAssignButton.position = new Vector2(Screen.width / 2f - 213f * CustomFunctions.getUIScale(), generalAssignButton.position.y);

			//} else if (supplyButton.position.x > 0 && buildButton.position.x < 0 && reverseButton.position.x < 0 || reverseButton.position.x > 0) {
			//    generalAssignButton.position = new Vector2(Screen.width / 2f - 106f * CustomFunctions.getUIScale(), generalAssignButton.position.y);

			//} else {
			//    generalAssignButton.position = new Vector2(Screen.width / 2f, generalAssignButton.position.y);

			//}
		} else {
			generalsButtonOn = false;
			//if (generalAssignButton.position.x > 2f)
			//generalAssignButton.position = new Vector2(-2000f, generalAssignButton.position.y);
		}
		List<Transform> buttonsOn = new List<Transform>();
		if (buildButtonOn) {
			buttonsOn.Add(buildButton);
		} else {
			buildButton.position = new Vector2(-2000, buildButton.position.y);
		}
		if (supplyButtonOn)
			buttonsOn.Add(supplyButton);
		else {
			supplyButton.position = new Vector2(-2000, supplyButton.position.y);
		}
		if (generalsButtonOn)
			buttonsOn.Add(generalAssignButton);
		else {
			generalAssignButton.position = new Vector2(-2000, generalAssignButton.position.y);
		}
		if (!justMoved && (selectedEnemySoldier != null || selectedSoldier != null || selectedTile != null && selectedTile.occupant != null)) {
			buttonsOn.Add(infoButton);
		} else
			infoButton.position = new Vector2(-2000, infoButton.position.y);

		if (reverseButtonOn)
			buttonsOn.Add(reverseButton);
		else {
			reverseButton.position = new Vector2(-2000, reverseButton.position.y);
		}
		if (!troopMoving) {
			for (int i = 0; i < buttonsOn.Count; i++) {
				buttonsOn[i].position = new Vector2(Screen.width / 2f + 106f * CustomFunctions.getUIScale() * (i - (buttonsOn.Count - 1) / 2f), buttonsOn[i].position.y);
			}
		} else {
			for (int i = 0; i < buttonsOn.Count; i++) {
				buttonsOn[i].position = new Vector2(-2000, buttonsOn[i].position.y);
			}
		}

		#endregion



		if (!canBuild && selectedCity != null && !airplaneMode) {
			if (selectedTile.occupant == null || selectedCity.airportTier >= 1 || selectedCity.nuclearTier >= 1) {
				buildButtonOn = true;

				//buildButton.position = new Vector3(Screen.width / 2f, buildButton.position.y, 0f);
				canBuild = true;
			}
			//if (selectedTile.occupant != null && selectedTile.occupant.country == selectedTile.country) {
			//    selectedSoldier = selectedTile.occupant;
			//}
		}
		if (selectedBuildTile != null) {
			buildButtonOn = true;

			//buildButton.position = new Vector3(Screen.width / 2f, buildButton.position.y, 0f);
		}
		//if (selectedSoldier != null && selectedSoldier.moving && selectedCity != null) {
		//    selectedCity.currentTile.hexRendererColor = selectedCity.currentTile.defaultTileColor;
		//    selectedCity = null;
		//    deselectTile(false, false);
		//    canBuild = false;
		//    buildButtonOn = false;
		//}

		//for (int i = 0; i < 9; i++) {
		//	if (Input.GetKeyDown((KeyCode)(48 + i))) {
		//		testIntegers.Add(i);
		//	}
		//}
		if (Input.GetMouseButtonDown(0))
			mouseDownPosition = Input.mousePosition;
		if (editMode)
			try {
				if (editCategory != 6) {
					if (cityNameInput.transform.position.x > 0f)
						cityNameInput.transform.Translate(-3000f, 0f, 0f);

				} else {
					if (cityNameInput.transform.position.x < 0f)
						cityNameInput.transform.Translate(3000f, 0f, 0f);

				}
			} catch {
			}
		if (editMode && hideEditUI && !mouseInUI() && !paused) {
			List<string> countries = new List<string>(countriesIsAxis.Keys);

			editModeTempInt = editCategory2Dropdown.value;



			switch (editCategory) {
				case 0:
					editModeTempCountry = countries[editModeTempInt];
					break;
				case 1:
					//city is not a terrain so it is skipped
					if (editCategory2Dropdown.value >= 3)
						editModeTempInt++;
					break;
			}

			if (editCategory == 7)
				editCityPopulation = editModeTempInt + 1;
			if (editCategory == 8)
				editCityIndustry = editModeTempInt;
			if (editCategory == 9)
				editCityAirport = editModeTempInt;
			if (editCategory == 10)
				editCityNuclear = editModeTempInt;

			if (editCategory == 11)
				editCityDefence = editModeTempInt;

			if (Input.GetMouseButton(0) && !mouseInUI() && !paused) {
				RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
				if (hit.collider != null && hit.collider.GetComponent<Tile>() != null) {
					Tile t = hit.collider.GetComponent<Tile>();
					if (moveToggle.isOn) {
						//don't do anything!
					} else if (editCategory == 0) {
						foreach (Collider2D c in Physics2D.OverlapCircleAll(t.transform.position, editBrushSize - 0.75f)) {
							if (c.GetComponent<Tile>() != null) {
								Tile nt = c.GetComponent<Tile>();
								nt.country = editModeTempCountry;
								if (nt.occupant != null && nt.occupant.country != nt.country) {
									nt.occupant.isAxis = countriesIsAxis[nt.country];
									nt.occupant.updateCountry();
								}
								nt.updateTileColor();
							}
						}

					} else if (editCategory == 1) {
						foreach (Collider2D c in Physics2D.OverlapCircleAll(t.transform.position, editBrushSize - 0.75f)) {
							if (c.GetComponent<Tile>() != null) {
								Tile nt = c.GetComponent<Tile>();
								if ((nt.occupant == null || nt.occupant.troopType != Troop.navy) && nt.terrain != (Terrain)editModeTempInt && t.city == null &&
									!nt.spawningTerrain && ((Terrain)editModeTempInt != Terrain.water || nt.occupant == null || nt.occupant.troopType != Troop.fortification)) {
									nt.terrain = (Terrain)editModeTempInt;
									nt.updateTerrain();
								}
							}
						}
					} else if (editCategory == 2) {
						if (t.occupant == null) {
							List<int> troopList = new List<int>(SoldierPrefabsManager.troopTypes.Values);
							if (Input.GetMouseButtonDown(0) && !deleteToggle.isOn && (t.terrain != Terrain.water ||
								troopPrefabs[troopList[editModeTempInt]].GetComponent<Unit>().troopType != Troop.fortification) &&
								(t.terrain == Terrain.water || troopPrefabs[troopList[editModeTempInt]].GetComponent<Unit>().troopType != Troop.navy))
								spawnSoldier(t.coordinate, t.country, countriesIsAxis[t.country], troopList[editModeTempInt], 0, 1, true);
						} else {
							if (deleteToggle.isOn && Input.GetMouseButtonDown(0)) {
								//delete unit
								Destroy(t.occupant.gameObject);
							}
						}
					} else if (editCategory == 3) {
						foreach (Collider2D c in Physics2D.OverlapCircleAll(t.transform.position, editBrushSize - 0.75f)) {
							if (c.GetComponent<Tile>() != null) {
								Tile nt = c.GetComponent<Tile>();
								if (nt.occupant != null) {
									nt.occupant.setVeterencyLevel(editModeTempInt);
									nt.occupant.checkVeterency();
								}
							}
						}
						//if (t.occupant != null) {
						//    t.occupant.setVeterencyLevel(editModeTempInt);
						//    t.occupant.checkVeterency();
						//}
					} else if (editCategory == 4) {
						foreach (Collider2D c in Physics2D.OverlapCircleAll(t.transform.position, editBrushSize - 0.75f)) {
							if (c.GetComponent<Tile>() != null) {
								Tile nt = c.GetComponent<Tile>();
								if (nt.occupant != null) {
									if (editModeTempInt > 3)
										editModeTempInt = 3;
									nt.occupant.tier = editModeTempInt + 1;
									nt.occupant.updateStack();
								}
							}
						}
						//if (t.occupant != null) {
						//    if (editModeTempInt > 3)
						//        editModeTempInt = 3;
						//    t.occupant.tier = editModeTempInt + 1;
						//    t.occupant.updateStack();
						//}
					} else if (editCategory == 5) {
						if (t.city != null) {
							if (deleteToggle.isOn && Input.GetMouseButtonDown(0)) {
								t.isCity = false;
								Destroy(t.city.gameObject);
							}
						} else if (!deleteToggle.isOn && Input.GetMouseButtonDown(0)) {
							if (t.terrain != Terrain.plains && t.terrain != Terrain.water) {
								t.terrain = Terrain.plains;
								t.updateTerrain();
							}
							GameObject insItem = Instantiate(cityPrefab, t.transform.position, Quaternion.identity);
							insItem.transform.Translate(Vector3.back * 1.1f);
							insItem.GetComponent<City>().currentTile = t;
							insItem.GetComponent<City>().tier = 1;
							insItem.GetComponent<City>().factoryTier = 1;
							insItem.GetComponent<City>().defenceTier = 1;
							insItem.GetComponent<City>().airportTier = 0;
							insItem.GetComponent<City>().nuclearTier = 0;

							insItem.GetComponent<City>().cityName = "";
							t.city = insItem.GetComponent<City>();
							cities.Add(insItem.GetComponent<City>());
						}
					} else if (editCategory == 6) {
						if (t.city != null) {
							t.city.cityName = cityNameInput.text;
						}
					} else if (editCategory == 7) {
						if (t.city != null) {
							t.city.tier = editCityPopulation;
							t.city.Start();
						}
					} else if (editCategory == 8) {
						if (t.city != null) {
							t.city.factoryTier = editCityIndustry;
							t.city.Start();
						}
					} else if (editCategory == 9) {
						if (t.city != null) {
							t.city.airportTier = editCityAirport;
							t.city.Start();
						}
					} else if (editCategory == 10) {
						if (t.city != null) {
							t.city.nuclearTier = editCityNuclear;
							t.city.Start();
						}
					} else if (editCategory == 11) {
						foreach (Collider2D c in Physics2D.OverlapCircleAll(t.transform.position, editBrushSize - 0.75f)) {
							if (c.GetComponent<Tile>() != null) {
								Tile nt = c.GetComponent<Tile>();
								if (nt.city != null && !nt.city.isPort) {
									nt.city.defenceTier = editCityDefence;
								}
							}
						}
					} else if (editCategory == 13) {
						if (t.city != null) {
							t.city.isStrategic = !deleteToggle.isOn;
						} else if (t.occupant != null) {
							t.occupant.isStrategic = !deleteToggle.isOn;
						}
					} else if (editCategory == 14) {
						if (t.occupant != null && t.occupant.troopType != Troop.fortification) {
							//add dropdown 2
							t.occupant.general = new List<string>(playerData.generals.Keys)[editCategory2Dropdown.value];
							t.occupant.generalLevel = editCategory3Dropdown.value;
							t.occupant.updateGeneral(false, true);
						}
					} else if (editCategory == 16) {
						if (t.occupant != null) {
							t.occupant.isLocked = !deleteToggle.isOn;
						}
					}

				}
			}
		} else if (!editMode && !building && (!isTutorial || tiles.ContainsKey(popupPositions[popupIndex - 1]) && !paratrooping)) {
			bool tutorialClicked = false;
			if (isTutorial && !cantSelectTile && !passingRound && !mouseInUI() && Input.GetMouseButtonUp(0) &&
				Vector2.Distance(Input.mousePosition, mouseDownPosition) < 30f && !cantSelectTile && !troopMoving) {
				RaycastHit2D tutorialRaycast = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
				if (tutorialRaycast.collider != null && tutorialRaycast.collider.GetComponent<Tile>() != null &&
					tutorialRaycast.collider.GetComponent<Tile>().coordinate == popupPositions[popupIndex - 1]) {
					TutorialPopup p = FindObjectOfType<TutorialPopup>();
					if (p != null) {
						p.RealClickedPopup();
						tutorialClicked = true;
					}
				}
			}

			if ((!isTutorial || tutorialClicked) && Input.GetMouseButtonUp(0) && !hasDialogue && !passingRound &&
				!mouseInUI() && !paused && !popupEnabled && Vector2.Distance(Input.mousePosition, mouseDownPosition) < 30f && !cantSelectTile) {
				bool deltaJustMoved = justMoved;
				justMoved = false;
				bool deselected = false;
				bool noSelection = false;
				RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
				if (airplaneMode) {
					if (selectedSoldier != null) {
						selectedSoldier.selected = false;
						selectedSoldier = null;
					}
					if (selectedEnemySoldier != null) {
						selectedEnemySoldier.enemySelected = false;
						selectedEnemySoldier = null;
					}
					Tile t = hit.collider.GetComponent<Tile>();
					if (!inAirStrike) {
						if (countryDatas[playerCountry].fuel >= productBar.airplaneOilCost && countryDatas[playerCountry].industry >= productBar.airplaneIndustryCost && hit.collider != null &&
							t != null && (t.occupant != null && t.canBeAttacked || t.movable || t.isCity && t.city.health > 0f && airplaneType != 2 && countriesIsAxis[t.country] != playerIsAxis)) {

							AirplaneAnimator ins = Instantiate(productBar.airplanes[airplaneType], new Vector3(hit.collider.transform.position.x + 0.9f, hit.collider.transform.position.y + 0.92f, -350f),
								productBar.airplanes[airplaneType].transform.rotation).GetComponent<AirplaneAnimator>();
							ins.targetTile = hit.collider.GetComponent<Tile>();
							inAirStrike = true;

							if (airplaneType != 2) {
								countryDatas[playerCountry].fuel -= productBar.airplaneOilCost;
								countryDatas[playerCountry].industry -= productBar.airplaneIndustryCost;
								findAirStrikeRange(productBar.airplaneRanges[airplaneType] + CheckCountryTech(playerCountry, TechTroopType.Air, TechCategory.Range));
								AirStrikeDelay(selectedCity, t);
								//if (t.occupant != null) {
								//    StartCoroutine(airStrikeDelay(selectedCity, t.occupant));
								//} else {
								//    StartCoroutine(airStrikeDelay(selectedCity, t.city));
								//}
								if (airplaneType == 3 && nuclearWarheadDropdown.value > 0) {
									countryDatas[playerCountry].nukes[nuclearWarheadDropdown.value - 1]--;
									UpdateNuclearDisplay();
									//nuclearWarheadDropdown.value = 0;
									incomingNuclearWarhead = true;
								}
							} else {
								countryDatas[playerCountry].fuel -= productBar.airplaneOilCost * selectedCity.currentTile.occupant.tier;
								countryDatas[playerCountry].industry -= productBar.airplaneIndustryCost * selectedCity.currentTile.occupant.tier;
								StartCoroutine(paratroopDelay(selectedCity.currentTile, t));
							}
							if (airplaneType == 2 || countryDatas[playerCountry].fuel < productBar.airplaneOilCost || countryDatas[playerCountry].industry < productBar.airplaneIndustryCost) {
								airplaneMode = false;
								selectedCity = null;

								foreach (Tile i in canAttackTiles) {
									i.movable = false;
									i.canBeAttacked = false;
									i.updateTileColor();
								}
							}

						} else {
							airplaneMode = false;
							selectedCity = null;
							foreach (Tile i in canAttackTiles) {
								i.movable = false;
								i.canBeAttacked = false;
								i.updateTileColor();
							}
						}
					}
				} else {
					if (hit.collider != null && hit.collider.GetComponent<Tile>() != null) {

						bool dontDeselectSoldier = false;
						Tile deltaSelectedBuildTile = selectedBuildTile;
						Tile myTile = hit.collider.GetComponent<Tile>();

						if (myTile.country == playerCountry && myTile.occupant == null && !myTile.isCity &&
							myTile.terrain != Terrain.water && myTile.terrain != Terrain.oil && !myTile.movable &&
							!airplaneMode) {
							if (selectedBuildTile != hit.collider.GetComponent<Tile>()) {
								selectedBuildTile = hit.collider.GetComponent<Tile>();
								selectedCity = null;
								if (selectedSoldier != null) {
									selectedSoldier.selected = false;
									selectedSoldier = null;
									dontDeselectSoldier = true;
								}
							} else
								selectedBuildTile = null;
						} else
							selectedBuildTile = null;

						bool enemySelection = false; //if not this one cancel enemy

						if (deltaSelectedBuildTile != null && selectedBuildTile == null)
							deltaSelectedBuildTile.hexRendererColor = deltaSelectedBuildTile.defaultTileColor;

						//deltaSelectedBuildTile.GetComponentInChildren<SpriteRenderer>().color = deltaSelectedBuildTile.defaultTileColor;

						if (myTile.movable && myTile.occupant == null && (!myTile.isCity || countriesIsAxis[myTile.country] == playerIsAxis || myTile.city.health <= 0f)) {
							selectedTile.occupant.moveToDestination(selectedTile.findPathToDestination(myTile), false, selectedTile);

							myTile.occupant = selectedTile.occupant;
							selectedTile.hexRendererColor = selectedTile.defaultTileColor;
							deselectTile(true, false);
							if (selectedSoldier != null) {
								selectedSoldier.selected = false;
								selectedSoldier = null;
								dontDeselectSoldier = true;
							}
							selectedCity = null;
						} else if (myTile.selected) {
							//print("out 1");
							noSelection = true;
							deselected = true;
							if (selectedSoldier != null) {
								deselectTile(false, false);
								//dontDeselectSoldier = true;
							} else {
								deselectTile(false);
							}

						} else if (myTile.occupant != null && !myTile.occupant.moving && !myTile.occupant.moved &&
							countryDatas[myTile.occupant.country].fuel >= myTile.occupant.movementFuelCost * hit.collider.GetComponent<Tile>().occupant.tier &&
							hit.collider.GetComponent<Tile>().occupant.country == playerCountry) {

							//print("out 2");
							if (selectedSoldier != null) {
								selectedSoldier.selected = false;
								selectedSoldier = null;
								dontDeselectSoldier = true;
							}
							if (selectedTile != null) {
								selectedTile.clearSearchedTiles();
								selectedTile.resetTilePathfinding(false);

								selectedTile.hexRendererColor = selectedTile.defaultTileColor;
								//selectedTile.GetComponentInChildren<SpriteRenderer>().color = selectedTile.defaultTileColor;


								selectedTile.selected = false;
								selectedCity = null;

								deselectTile(false, !dontDeselectSoldier);
							}
							selectedTile = hit.collider.GetComponent<Tile>();
							selectedTile.selected = true;
							selectedTile.occupant.select();

							selectedTile.hexRendererColor = Color.yellow;

							//selectedTile.GetComponentInChildren<SpriteRenderer>().color = Color.yellow;
							selectedTile.findAvailableTiles(selectedTile.occupant.movement);
						} else if (myTile.canBeAttacked) {
							//attack here

							float damage;
							if (myTile.occupant == null) {
								damage = CalculateCityDamage(selectedTile.occupant);
							} else
								damage = calculateDamage(selectedTile.occupant, myTile.occupant);

							bool isCrit = false;
							if (CheckGeneralCrit(selectedTile.occupant)) {
								isCrit = true;
								damage *= 2f;
							}
							selectedTile.occupant.RotateToTarget(myTile);

							selectedTile.occupant.attack(0f, damage, myTile, true, isCritical: isCrit);

							checkRocketArtilleryDamage(myTile, selectedTile.occupant, damage, 0f, true, isCritical: isCrit);

							selectedTile.occupant.attacked = true;
							selectedTile.occupant.canAttack = false;
							selectedTile.occupant.moved = true;

							//retaliation 
							if (!CheckRetaliationChance(selectedTile.occupant) && myTile.occupant != null && myTile.occupant.troopId != 20 &&
									selectedTile.occupant.troopId != 12 && (selectedTile.occupant.troopId != 21 || myTile.occupant.troopId != 12) &&
									selectedTile.occupant.troopId != 20 && selectedTile.occupant.troopId != 14 && myTile.occupant.troopId != 14 && (myTile.occupant.troopType != Troop.fortification ||
									myTile.occupant.tier == 1) && (myTile.isCity && myTile.city.health > 0f && myTile.occupant.health - damage * 0.5f > 0f || myTile.occupant.health - damage > 0f) &&
									(selectedTile.occupant.troopType != Troop.artillery || selectedTile.terrain == Terrain.water) &&
									myTile.occupant.range >= Vector2.Distance(myTile.occupant.transform.position, selectedTile.transform.position) - 0.35f) {
								//print("in");
								myTile.occupant.RotateToTarget(selectedTile);
								float dmg = calculateDamage(myTile.occupant, selectedTile.occupant);
								bool crt = false;
								if (CheckGeneralCrit(myTile.occupant)) {
									crt = true;
									damage *= 2f;
								}
								myTile.occupant.attack(0.25f, dmg, selectedTile, true, isCritical: crt);
								checkRocketArtilleryDamage(selectedTile, myTile.occupant, dmg, 0.25f, true, isCritical: crt);
							}
							selectedCity = null;
							if (selectedSoldier != null) {
								selectedSoldier.selected = false;
								selectedSoldier = null;
							}
							deselectTile(false, false);
						} else if (myTile.occupant != null && myTile.occupant.country == playerCountry) {

							canBuild = false;
							if (selectedSoldier == myTile.occupant && !deltaJustMoved) { //TODO: CHECK HERE
																						 // print("out3");
								noSelection = true;


								//dontDeselectSoldier = true;
								//selectedSoldier = null;
							} else/* if ((myTile.occupant.general == "" || !myTile.occupant.moved && countryDatas[playerCountry].fuel >= myTile.occupant.movementFuelCost * myTile.occupant.tier) &&
                                hit.collider.GetComponent<Tile>().occupant.troopType != Troop.fortification)*/ {
								//print("out4");
								selectedCity = null;
								deselectTile(false, false);
								if (selectedSoldier != null) {
									selectedSoldier.selected = false;
									selectedSoldier.currentTile.hexRendererColor = selectedSoldier.currentTile.defaultTileColor;
									selectedSoldier = null;
								}

								selectedSoldier = myTile.occupant;
								selectedSoldier.selected = true;

								selectedSoldier.currentTile.hexRendererColor = Color.yellow;
								if (selectedTile != null) {
									selectedTile.hexRendererColor = selectedTile.defaultTileColor;
								}
								//selectedCity = null;
								//deselectTile(false, false);
							}
						} else if (myTile.occupant != null && myTile.occupant.country != playerCountry && !airplaneMode) {
							//print("out5");
							canBuild = false;
							noSelection = true;
							if (selectedEnemySoldier != myTile.occupant) {
								if (selectedEnemySoldier != null) {
									selectedEnemySoldier.enemySelected = false;
									selectedEnemySoldier.currentTile.hexRendererColor = selectedEnemySoldier.currentTile.defaultTileColor;
								}
								enemySelection = true;
								selectedEnemySoldier = myTile.occupant;
								selectedEnemySoldier.enemySelected = true;

								selectedEnemySoldier.currentTile.hexRendererColor = Color.yellow;
								if (selectedTile != null) {
									selectedTile.hexRendererColor = selectedTile.defaultTileColor;
								}
								if (selectedSoldier != null) {
									selectedSoldier.selected = false;
									selectedSoldier = null;
								}
								deselectTile(false, false);
							}
						} else if (myTile.occupant == null && myTile.isCity && myTile.country == playerCountry) {
							if (myTile.city != selectedCity) {
								deselectTile(false, false);
								if (selectedSoldier != null) {
									selectedSoldier.selected = false;
									selectedSoldier.currentTile.hexRendererColor = selectedSoldier.currentTile.defaultTileColor;
									selectedSoldier = null;
								}
								selectedCity = myTile.city;
							} else {
								noSelection = true;
							}

						} else if (myTile.occupant != null && selectedSoldier == null) {
							print("out5");
							selectedSoldier = hit.collider.GetComponent<Tile>().occupant;
							selectedSoldier.selected = true;

							selectedSoldier.currentTile.hexRendererColor = Color.yellow;
						} else {
							noSelection = true;
						}
						if (!enemySelection && selectedEnemySoldier != null) {
							selectedEnemySoldier.currentTile.hexRendererColor = selectedEnemySoldier.currentTile.defaultTileColor;
							selectedEnemySoldier.enemySelected = false;
							selectedEnemySoldier = null;
						}


						if (myTile.occupant != null && !myTile.occupant.moving && (!myTile.occupant.attacked && myTile.occupant.canAttack) && myTile.occupant.country == playerCountry && !deselected) {
							noSelection = selectCanAttackTile(noSelection, myTile);
						}

						if (myTile.isCity && (!canBuild || selectedCity != myTile.city) && (myTile.occupant == null || myTile.city.airportTier >= 1) &&
							(myTile.country == playerCountry || myTile.occupant != null && myTile.occupant.country == playerCountry)) {
							selectedCity = myTile.city;
							buildButtonOn = true;
							//buildButton.position = new Vector3(Screen.width / 2f, buildButton.position.y, 0f);
							canBuild = true;
							if (selectedSoldier != null)
								dontDeselectSoldier = true;
						} else {
							buildButtonOn = false;


							//buildButton.position = new Vector3(-1000f, buildButton.position.y, 0f);
							canBuild = false;
						}
						if (selectedSoldier && selectedSoldier.moved && (!selectedSoldier.canAttack || selectedSoldier.attacked)) dontDeselectSoldier = true;

						if (selectedTile && selectedTile.isCity && !selectedCity) selectedCity = selectedTile.city;
						if (selectedSoldier && selectedSoldier.currentTile.isCity && !selectedCity) selectedCity = selectedSoldier.currentTile.city;
						if (selectedCity && selectedCity.currentTile.occupant && selectedSoldier == null) selectedSoldier = selectedCity.currentTile.occupant;

						if (!troopMoving && selectedSoldier != null && selectedCity != null && selectedSoldier.country == playerCountry && !selectedSoldier.supplied &&
							(!selectedSoldier.currentTile.city.isPort || selectedSoldier.troopType == Troop.navy) && !selectedSoldier.encircled &&
							!selectedSoldier.doubleEncircled && selectedSoldier.health < selectedSoldier.maxHealth &&
							countryDatas[playerCountry].manpower >= calculateSupplyCost(selectedSoldier.currentTile)[0] &&
							countryDatas[playerCountry].industry >= calculateSupplyCost(selectedSoldier.currentTile)[1]) {
							canSupply = true;
							//selectedCity = myTile.city;
						} else {
							//if (selectedSoldier) print(selectedSoldier.supplied);
							canSupply = false;
						}
						if (noSelection) {
							deselectTile(false, !dontDeselectSoldier);
							if (selectedSoldier != null) {
								selectedSoldier.currentTile.hexRendererColor = selectedSoldier.currentTile.defaultTileColor;
								selectedSoldier.selected = false;
								selectedSoldier = null;
							}

							selectedCity = null;
							buildButtonOn = false;
							canBuild = false;
						} else if (selectedSoldier != null && selectedTile != null && selectedTile.occupant != selectedSoldier) {
							selectedSoldier.selected = false;
							selectedSoldier.currentTile.hexRendererColor = selectedSoldier.currentTile.defaultTileColor;
							selectedSoldier = null;
						}

						if (selectedSoldier != null) {
							selectedBuildTile = null;
						}
					} else {
						deselectTile(false);
						buildButtonOn = false;
						//buildButton.position = new Vector3(-1000f, buildButton.position.y, 0f);
						canBuild = false;
						canSupply = false;
						if (selectedSoldier != null)
							selectedSoldier.selected = false;
						selectedSoldier = null;
					}
					if (selectedTile != null && selectedSoldier != null && selectedTile != selectedSoldier.currentTile) {
						selectedSoldier.selected = false;
						selectedSoldier = null;
					}
				}
			}
		}
		dropdownDeltaHierarchy = nuclearWarheadDropdown.transform.childCount > 3;
		//print(dropdownDeltaHierarchy);
	}
	public void findParatrooperRange(int tileCount) {
		Collider2D[] t = Physics2D.OverlapCircleAll(selectedCity.transform.position, tileCount + 3f /*change according to specific capacity*/);
		canAttackTiles = new List<Tile>();
		foreach (Collider2D i in t) {
			Tile ti = i.GetComponent<Tile>();
			if (ti != null && ti.terrain != Terrain.highMountains && ti.terrain != Terrain.water && ti.occupant == null && (!ti.isCity || ti.city.health <= 0) && findDistanceBetweenTiles(ti, selectedCity.currentTile) <= tileCount) {
				//i.GetComponentInChildren<SpriteRenderer>().color = Color.green;
				ti.hexRendererColor = Color.green;

				ti.movable = true;
				canAttackTiles.Add(i.GetComponent<Tile>());
			}
		}
	}

	public void findAirStrikeRange(int tileCount) {
		Collider2D[] t = Physics2D.OverlapCircleAll(selectedCity.transform.position, tileCount + 3f /*change according to specific capacity*/);

		canAttackTiles = new List<Tile>();
		foreach (Collider2D i in t) {
			Tile ti = i.GetComponent<Tile>();
			if (ti != null && ti.occupant != null && ti.occupant.isAxis != playerIsAxis &&
				!countriesIsNeutral.Contains(ti.country) && findDistanceBetweenTiles(ti, selectedCity.currentTile) <= tileCount) {
				ti.hexRendererColor = new Color(1f, 0.3f, 0.3f);

				ti.canBeAttacked = true;
				canAttackTiles.Add(i.GetComponent<Tile>());
			} else {
				//attack city
				if (ti != null && ti.isCity && ti.city.tier > 0 && ti.city.health > 0f && countriesIsAxis[ti.country] != playerIsAxis &&
					!countriesIsNeutral.Contains(ti.country) && findDistanceBetweenTiles(ti, selectedCity.currentTile) <= tileCount) {
					ti.hexRendererColor = new Color(1f, 0.3f, 0.3f);

					ti.canBeAttacked = true;
					canAttackTiles.Add(i.GetComponent<Tile>());
				}
			}
		}
	}
	public Dictionary<Tile, int> FindNavigatableDistanceBetweenTiles(Tile tileA, Tile tileB, bool onlyWater) {
		int totalCostIterations = 0;
		if (onlyWater && tileB.terrain != Terrain.water) {
			//impossible to reach
			return null;
		}
		try {
			//returns cost of moving from tile a to tile b
			Dictionary<Tile, int> tileCurrentMovementCosts = new Dictionary<Tile, int>(); //the current cost of moving from tile A to designated tile
			Dictionary<Tile, Tile> tileDirectingFrom = new Dictionary<Tile, Tile>(); //where each tile came from
			List<Tile> closedTiles = new List<Tile>(); //closed tiles have already been searched and will be skipped
			int iteration = 0;
			tileCurrentMovementCosts.Add(tileA, 0);
			while (true) {
				bool allTilesClosed = true;
				//double foreach loop goes through all tiles searched and looks at their "neighbors" (six adjacent tiles)
				Dictionary<Tile, int> temporaryTileCur = new Dictionary<Tile, int>(tileCurrentMovementCosts);
				foreach (KeyValuePair<Tile, int> k in temporaryTileCur) {
					//k.Key is self tile and t is searched new tile
					if (!closedTiles.Contains(k.Key)) {
						bool tileClosed = true;
						foreach (Tile t in k.Key.neighbors) {
							totalCostIterations++;
							//only worry about enemy in the way if not first tile
							if (t != null && t.terrain != Terrain.highMountains && (k.Value > 0 || t == tileB || t.occupant == null ||
								countriesIsAxis[t.country] == countriesIsAxis[tileA.country]) && (!onlyWater || t.terrain == Terrain.water)) {
								if (tileCurrentMovementCosts.ContainsKey(t)) {
									//the movement cost is the minimum of that tile
									if (tileCurrentMovementCosts[t] > k.Value + t.terrainCosts[t.terrain]) {
										tileCurrentMovementCosts[t] = k.Value + t.terrainCosts[t.terrain];
										//updated cost, should search tiles again
										tileClosed = false;
										if (closedTiles.Contains(t))
											closedTiles.Remove(t);
										tileDirectingFrom[t] = k.Key;
									}
								} else {
									tileCurrentMovementCosts.Add(t, k.Value + t.terrainCosts[t.terrain]);
									tileDirectingFrom.Add(t, k.Key);

									//tile isn't closed because there is a new neighbor added to the searched tiles
									tileClosed = false;
									allTilesClosed = false;
								}

							}
						}
						if (tileClosed) {
							closedTiles.Add(k.Key);
							if (k.Key == tileB) {
								//print(totalCostIterations);
								Dictionary<Tile, int> routeTiles = new Dictionary<Tile, int>(); //tiles that are on the route between a and b if existing
								Tile currentTile = tileB;
								for (int c = 0; c < 1000; c++) {
									routeTiles.Add(currentTile, tileCurrentMovementCosts[currentTile]);
									if (tileDirectingFrom.ContainsKey(currentTile)) {
										currentTile = tileDirectingFrom[currentTile];
										//print(currentTile.coordinate);
									} else {
										return routeTiles;
									}

									if (c == 999) {
										print("issue occured pathing");
									}
								}
							}
						} else {
							allTilesClosed = false;
						}
					}

				}

				if (allTilesClosed) {
					print(totalCostIterations);
					print("impossible path");
					break;
				}
				if ((iteration > aiMoveThreshold - 1 || iteration > 20) && totalCostIterations > 5000) {
					print(aiMoveThreshold);
					print(totalCostIterations);
					print("error finding distance");
					break;
				}
			}
		} catch (Exception e) {
			print(e);
		}
		return null;
	}
	public int findDistanceBetweenTiles(Tile tileA, Tile tileB) {
		if (tileA == null || tileB == null)
			return -1; //pathfind failed
		int distance = 1;
		Tile current = tileA;
		for (int i = 0; i < 1000; i++) {
			Tile bestTile = null;
			float shortestDistanceToTarget = Mathf.Infinity;

			Tile tempTile = current.lowerLeft;
			if (tempTile != null && Vector2.Distance(tempTile.transform.position, tileB.transform.position) < shortestDistanceToTarget) {
				shortestDistanceToTarget = Vector2.Distance(tempTile.transform.position, tileB.transform.position);
				bestTile = tempTile;
			}
			tempTile = current.lowerRight;
			if (tempTile != null && Vector2.Distance(tempTile.transform.position, tileB.transform.position) < shortestDistanceToTarget) {
				shortestDistanceToTarget = Vector2.Distance(tempTile.transform.position, tileB.transform.position);
				bestTile = tempTile;
			}
			tempTile = current.upperLeft;
			if (tempTile != null && Vector2.Distance(tempTile.transform.position, tileB.transform.position) < shortestDistanceToTarget) {
				shortestDistanceToTarget = Vector2.Distance(tempTile.transform.position, tileB.transform.position);
				bestTile = tempTile;
			}
			tempTile = current.upperRight;
			if (tempTile != null && Vector2.Distance(tempTile.transform.position, tileB.transform.position) < shortestDistanceToTarget) {
				shortestDistanceToTarget = Vector2.Distance(tempTile.transform.position, tileB.transform.position);
				bestTile = tempTile;
			}
			tempTile = current.top;
			if (tempTile != null && Vector2.Distance(tempTile.transform.position, tileB.transform.position) < shortestDistanceToTarget) {
				shortestDistanceToTarget = Vector2.Distance(tempTile.transform.position, tileB.transform.position);
				bestTile = tempTile;
			}
			tempTile = current.bottom;
			if (tempTile != null && Vector2.Distance(tempTile.transform.position, tileB.transform.position) < shortestDistanceToTarget) {
				bestTile = tempTile;
			}

			current = bestTile;
			if (current == tileB) {
				break;
			}
			distance++;
		}
		return distance;
	}

	public bool selectCanAttackTile(bool noSelection, Tile target) {
		if (selectedTile != null && noSelection) {
			selectedTile.clearSearchedTiles();
			selectedTile.resetTilePathfinding(false);

			selectedTile.hexRendererColor = selectedTile.defaultTileColor;

			selectedTile.selected = false;
			deselectTile(false);
		}
		Collider2D[] t = Physics2D.OverlapCircleAll(target.transform.position, target.GetComponent<Tile>().occupant.range + 3f);
		canAttackTiles = new List<Tile>();
		foreach (Collider2D i in t) {
			Tile myT = i.GetComponent<Tile>(); //target is self
			if (myT != null && myT.occupant != null && !myT.occupant.isNeutral && myT.occupant.isAxis != playerIsAxis &&
				(target.GetComponent<Tile>().terrain == Terrain.water || myT.occupant.troopId != 12 ||
				target.GetComponent<Tile>().occupant.troopId == 21) && (target.GetComponent<Tile>().occupant.troopId != 12 || myT.terrain == Terrain.water)) {

				if (findDistanceBetweenTiles(myT, target) <= target.occupant.range) {
					myT.hexRendererColor = new Color(1f, 0.3f, 0.3f);

					myT.canBeAttacked = true;
					canAttackTiles.Add(myT);
					noSelection = false;
				}
			} else if (target.GetComponent<Tile>().occupant.troopId != 12 && myT != null && myT.isCity && myT.city.health > 0 &&
				myT.city.tier > 0 && !countriesIsNeutral.Contains(myT.country) && countriesIsAxis[myT.country] != playerIsAxis) {
				if (findDistanceBetweenTiles(myT, target) <= target.occupant.range) {
					myT.hexRendererColor = new Color(1f, 0.3f, 0.3f);

					myT.canBeAttacked = true;
					canAttackTiles.Add(myT);
					noSelection = false;
				}
			}
		}
		if (!noSelection) {
			selectedTile = target.GetComponent<Tile>();
			selectedTile.selected = true;
			if (selectedTile != null && (!selectedTile.occupant.moved &&
				countryDatas[selectedTile.occupant.country].fuel >= selectedTile.occupant.movementFuelCost * selectedTile.occupant.tier ||
				!selectedTile.occupant.attacked && selectedTile.occupant.canAttack))
				selectedTile.occupant.select();

			selectedTile.hexRendererColor = Color.yellow;
			//selectedTile.GetComponentInChildren<SpriteRenderer>().color = Color.yellow;
		}
		return noSelection;
	}
}
