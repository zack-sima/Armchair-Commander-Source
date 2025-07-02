using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
[Serializable]
public enum EventType { Diplomacy, Manpower, Industry, Fuel, Health, Veterency, NuclearWar }
[Serializable]
public class GameEvent {
	public string title, description, countryTarget, cityCondition;

	public int eventValue, triggerRound; //event dropdown value is brought by a function which keeps all the possible changes and translates it into actual values

	public EventType eventType;
}
[Serializable]
public class MapInfo {
	public MyVector2[] cityCoordinates, countryCoordinates, terrainCoordinates, soldierCoordinates;
	public int[] cityTiers, terrain, cityFactoryTiers, cityAirportTiers, cityNuclearTiers, soldierIds, soldierVeterencies, soldierTiers;
	public string[] cityNames, countryCoordinateNames;
	public bool[] cityStrategics, soldierIsStrategics;
	public int[] cityDefenceTiers; //optional NEW FEATURE
	public string[] soldierGenerals;
	public int[] soldierGeneralLevels;
	public string playerCountry, missionName;
	public List<string> countryTeamsKeys;
	public List<int> countryTeamsValues;
	public string beginningText, beginningTextGeneral;
	public bool allowReupload, disableDiplomacy, overridePlayerTech, lessResources;
	public bool[] soldierIsLocked;

	public List<GameEvent> events;

	public int aiDetection, roundLimit, mapSize, victoryCondition, generalLimit;
	public List<string> countryDataKeys;
	public List<CountryData> countryDataValues;

	public List<string> countryCustomNameOverridesKeys; //TODO: NOTE: THIS IS NEW FEATURE FOR CUSTOM NAMES
	public List<string> countryCustomNameOverridesValues;

	public List<string> countryCustomFlagOverridesKeys; //separate
	public List<string> countryCustomFlagOverridesValues;

	public MapInfo(Controller controller) {
		Dictionary<string, int> countryTeams = new Dictionary<string, int>();
		foreach (KeyValuePair<string, int> d in controller.countriesIsAxis) {
			countryTeams.Add(d.Key, d.Value);
		}
		foreach (string d in controller.countriesIsNeutral) {
			//TODO: NOTE: CODES 2/3 USED TO BE FOR NEUTRAL AXIS/ALLIES, SO TEAMS 2/3 MUST BE RESERVED FOR THIS PURPOSE(?)
			//countryTeams[d] = countryTeams[d] + 2;
			countryTeams[d] = 2;
		}




		allowReupload = controller.reuploadToggle.isOn;
		disableDiplomacy = controller.diplomacyToggle.isOn;
		overridePlayerTech = controller.overrideTechToggle.isOn;
		lessResources = controller.lessResourcesToggle.isOn;
		countryTeamsKeys = new List<string>(countryTeams.Keys);
		countryTeamsValues = new List<int>(countryTeams.Values);
		countryCustomFlagOverridesKeys = new List<string>(controller.countryCustomFlagOverrides.Keys);
		countryCustomFlagOverridesValues = new List<string>(controller.countryCustomFlagOverrides.Values);
		countryCustomNameOverridesKeys = new List<string>(controller.countryCustomNameOverrides.Keys);
		countryCustomNameOverridesValues = new List<string>(controller.countryCustomNameOverrides.Values);

		beginningText = controller.beginningTextInput.text;
		beginningTextGeneral = new List<string>(controller.playerData.generals.Keys)[controller.beginningGeneralDropdown.value];
		playerCountry = controller.playerCountry;
		aiDetection = controller.aiMoveThreshold;
		roundLimit = controller.roundLimit;
		mapSize = controller.mapSize;
		missionName = controller.missionName;
		generalLimit = controller.maxGenerals;
		if (!controller.editMode) {
			roundLimit++;
		}
		victoryCondition = controller.victoryCondition;
		generalLimit = controller.maxGenerals;

		int i;

		countryDataKeys = new List<string>(controller.countryDatas.Keys);
		countryDataValues = new List<CountryData>(controller.countryDatas.Values);

		events = controller.events;
		cityCoordinates = new MyVector2[controller.cities.Count];
		for (i = 0; i < controller.cities.Count; i++)
			cityCoordinates[i] = new MyVector2(controller.cities[i].currentTile.coordinate);

		cityStrategics = new bool[controller.cities.Count];
		for (i = 0; i < controller.cities.Count; i++)
			cityStrategics[i] = controller.cities[i].isStrategic;

		countryCoordinates = new MyVector2[controller.tiles.Count];

		i = 0;
		foreach (Vector2 a in controller.tiles.Keys) {
			countryCoordinates[i] = new MyVector2(a);
			i++;
		}
		countryCoordinateNames = new string[controller.tiles.Count];
		i = 0;
		foreach (Vector2 a in controller.tiles.Keys) {
			countryCoordinateNames[i] = controller.tiles[a].country;
			i++;
		}
		i = 0;
		terrainCoordinates = new MyVector2[controller.tiles.Count];
		foreach (Vector2 a in controller.tiles.Keys) {
			terrainCoordinates[i] = new MyVector2(a);
			i++;
		}
		soldierCoordinates = new MyVector2[controller.soldiers.Count];
		for (i = 0; i < controller.soldiers.Count; i++) {
			soldierCoordinates[i] = new MyVector2(controller.soldiers[i].currentTile.coordinate);
		}
		cityTiers = new int[controller.cities.Count];
		for (i = 0; i < controller.cities.Count; i++)
			cityTiers[i] = controller.cities[i].tier;

		cityDefenceTiers = new int[controller.cities.Count];
		for (i = 0; i < controller.cities.Count; i++)
			cityDefenceTiers[i] = controller.cities[i].defenceTier;

		cityNames = new string[controller.cities.Count];
		for (i = 0; i < controller.cities.Count; i++)
			cityNames[i] = controller.cities[i].cityName;

		terrain = new int[controller.tiles.Count];
		i = 0;
		foreach (Vector2 a in controller.tiles.Keys) {
			terrain[i] = (int)controller.tiles[a].terrain;
			i++;
		}
		cityFactoryTiers = new int[controller.cities.Count];
		for (i = 0; i < controller.cities.Count; i++)
			cityFactoryTiers[i] = controller.cities[i].factoryTier;

		cityAirportTiers = new int[controller.cities.Count];
		for (i = 0; i < controller.cities.Count; i++)
			cityAirportTiers[i] = controller.cities[i].airportTier;

		cityNuclearTiers = new int[controller.cities.Count];
		for (i = 0; i < controller.cities.Count; i++)
			cityNuclearTiers[i] = controller.cities[i].nuclearTier;

		soldierIds = new int[controller.soldiers.Count];
		for (i = 0; i < controller.soldiers.Count; i++) {
			soldierIds[i] = controller.soldiers[i].troopId;
		}
		soldierGenerals = new string[controller.soldiers.Count];
		for (i = 0; i < controller.soldiers.Count; i++) {
			string general = controller.soldiers[i].general;
			if (CustomFunctions.GeneralUpdatedNames.ContainsKey(general)) {
				general = CustomFunctions.GeneralUpdatedNames[general];
			}
			soldierGenerals[i] = general;
		}
		soldierGeneralLevels = new int[controller.soldiers.Count];
		for (i = 0; i < controller.soldiers.Count; i++) {
			soldierGeneralLevels[i] = controller.soldiers[i].generalLevel;
		}

		soldierVeterencies = new int[controller.soldiers.Count];
		for (i = 0; i < controller.soldiers.Count; i++) {
			soldierVeterencies[i] = controller.soldiers[i].veterency;
		}
		soldierIsStrategics = new bool[controller.soldiers.Count];
		for (i = 0; i < controller.soldiers.Count; i++) {
			soldierIsStrategics[i] = controller.soldiers[i].isStrategic;
		}

		soldierIsLocked = new bool[controller.soldiers.Count];
		for (i = 0; i < controller.soldiers.Count; i++) {
			soldierIsLocked[i] = controller.soldiers[i].isLocked;
		}

		soldierTiers = new int[controller.soldiers.Count];
		for (i = 0; i < controller.soldiers.Count; i++) {
			soldierTiers[i] = controller.soldiers[i].tier;
		}
	}
}
public class MapData : MonoBehaviour {
	public Vector2[][] cityCoordinates, countryCoordinates, terrainCoordinates, soldierCoordinates;
	public int[][] cityTiers, terrain, cityFactoryTiers, cityDefenceTiers, cityAirportTiers, cityNuclearTiers, soldierIds, soldierVeterencies, soldierTiers, soldierGeneralLevels;
	public string[][] cityNames, countryCoordinateNames, soldierGenerals;
	public bool[][] cityStrategics, soldierIsStrategics, soldierIsLocked;
	[HideInInspector]
	public bool[] disableDiplomacy, overridePlayerTech, allowReupload, lessResources;
	//[HideInInspector]
	public string[] playerCountry, missionName, beginningText, beginningTextGeneral;
	[HideInInspector]
	public int[] aiDetection, roundLimit, mapSize, victoryCondition, generalLimit;
	public Dictionary<string, CountryData>[] countryDatas;
	public Dictionary<string, int>[] countryTeams;
	public Dictionary<string, string>[] countryCustomNameOverrides, countryCustomFlagOverrides; //TODO: NOTE: THIS IS NEW FEATURE FOR CUSTOM NAMES
	public List<GameEvent>[] events;

	public TextAsset[] levelData;
	[HideInInspector]
	public MapInfo[] mapInfos;

	//DISPABLES EVERYTHING EXCEPT SHOWING OF MAPS
	public bool isLevelSelect;

	void Awake() {
		//import datafiles (mapdata)
		if (!isLevelSelect && MyPlayerPrefs.instance.GetInt("saved") == 1 && MyPlayerPrefs.instance.GetInt("editor") == 0) {
			MyPlayerPrefs.instance.SetInt("map", MyPlayerPrefs.instance.GetInt("savedMap"));
			MyPlayerPrefs.instance.SetInt("level", MyPlayerPrefs.instance.GetInt("savedLevel"));
			MyPlayerPrefs.instance.SetInt("custom", MyPlayerPrefs.instance.GetInt("savedCustom"));
		}

		if (!isLevelSelect && MyPlayerPrefs.instance.GetInt("custom") == 1) {
			MyPlayerPrefs.instance.SetInt("level", 0);
			mapInfos = new MapInfo[1];
			if (MyPlayerPrefs.instance.GetString("multiplayerMap") != "") {
				mapInfos[0] = JsonUtility.FromJson<MapInfo>(MyPlayerPrefs.instance.GetString("multiplayerMap"));
			} else {
				mapInfos[0] = JsonUtility.FromJson<MapInfo>(MyPlayerPrefs.instance.GetString("customData" + MyPlayerPrefs.instance.GetInt("map")));
			}
			//CustomFunctions.CopyToClipboard(MyPlayerPrefs.instance.GetString("customData" + MyPlayerPrefs.instance.GetInt("map")));

			string[] generals = mapInfos[0].soldierGenerals;
			if (generals != null) {
				for (int j = 0; j < generals.Length; j++) {
					if (CustomFunctions.GeneralUpdatedNames.ContainsKey(generals[j])) {
						generals[j] = CustomFunctions.GeneralUpdatedNames[generals[j]];
					}
				}
				mapInfos[0].soldierGenerals = generals;
			}
		} else {
			mapInfos = new MapInfo[levelData.Length];
			for (int i = 0; i < mapInfos.Length; i++) {
				if (levelData[i] == null) {
					mapInfos[i] = mapInfos[0];
					continue;
				}

				try {
					mapInfos[i] = JsonUtility.FromJson<MapInfo>(levelData[i].text);
					//print(mapInfos[i].missionName);
				} catch {
					mapInfos[i] = JsonUtility.FromJson<MapInfo>(levelData[0].text);
				}
			}
		}
		countryTeams = new Dictionary<string, int>[mapInfos.Length];
		try {
			for (int i = 0; i < mapInfos.Length; i++) {
				countryTeams[i] = new Dictionary<string, int>();
				for (int j = 0; j < mapInfos[i].countryTeamsKeys.Count; j++) {
					countryTeams[i].Add(mapInfos[i].countryTeamsKeys[j], mapInfos[i].countryTeamsValues[j]);
				}
			}
		} catch {
			print("country teams not available");
		}

		countryDatas = new Dictionary<string, CountryData>[mapInfos.Length];
		for (int i = 0; i < mapInfos.Length; i++) {
			countryDatas[i] = new Dictionary<string, CountryData>();
			for (int j = 0; j < mapInfos[i].countryDataKeys.Count; j++) {
				countryDatas[i].Add(mapInfos[i].countryDataKeys[j], mapInfos[i].countryDataValues[j]);
			}
		}
		countryCustomFlagOverrides = new Dictionary<string, string>[mapInfos.Length];
		for (int i = 0; i < mapInfos.Length; i++) {
			countryCustomFlagOverrides[i] = new Dictionary<string, string>();
			if (mapInfos[i].countryCustomFlagOverridesKeys != null) {
				for (int j = 0; j < mapInfos[i].countryCustomFlagOverridesKeys.Count; j++) {
					countryCustomFlagOverrides[i].Add(mapInfos[i].countryCustomFlagOverridesKeys[j], mapInfos[i].countryCustomFlagOverridesValues[j]);
				}
			}
		}
		countryCustomNameOverrides = new Dictionary<string, string>[mapInfos.Length];
		for (int i = 0; i < mapInfos.Length; i++) {
			countryCustomNameOverrides[i] = new Dictionary<string, string>();
			if (mapInfos[i].countryCustomNameOverridesKeys != null) {
				for (int j = 0; j < mapInfos[i].countryCustomNameOverridesKeys.Count; j++) {
					countryCustomNameOverrides[i].Add(mapInfos[i].countryCustomNameOverridesKeys[j], mapInfos[i].countryCustomNameOverridesValues[j]);
				}
			}
		}

		allowReupload = new bool[mapInfos.Length];
		for (int i = 0; i < mapInfos.Length; i++) {
			try {
				allowReupload[i] = mapInfos[i].allowReupload;
			} catch {
				allowReupload[i] = false;
				print("no info");
			}
		}
		overridePlayerTech = new bool[mapInfos.Length];
		for (int i = 0; i < mapInfos.Length; i++) {
			try {
				overridePlayerTech[i] = mapInfos[i].overridePlayerTech;
			} catch {
				overridePlayerTech[i] = false;
				print("no tech");
			}
		}

		disableDiplomacy = new bool[mapInfos.Length];

		for (int i = 0; i < mapInfos.Length; i++) {
			disableDiplomacy[i] = mapInfos[i].disableDiplomacy;
		}

		lessResources = new bool[mapInfos.Length];

		for (int i = 0; i < mapInfos.Length; i++) {
			lessResources[i] = mapInfos[i].lessResources;
		}
		playerCountry = new string[mapInfos.Length];
		for (int i = 0; i < mapInfos.Length; i++)
			playerCountry[i] = mapInfos[i].playerCountry;

		beginningText = new string[mapInfos.Length];
		for (int i = 0; i < mapInfos.Length; i++) {
			try {
				beginningText[i] = mapInfos[i].beginningText;
			} catch {
				//print("no text");
				beginningText[i] = "";
			}
		}

		beginningTextGeneral = new string[mapInfos.Length];
		for (int i = 0; i < mapInfos.Length; i++) {
			try {
				beginningTextGeneral[i] = mapInfos[i].beginningTextGeneral;
			} catch {
				//print("no beginning general");
				beginningTextGeneral[i] = "";
			}
		}

		aiDetection = new int[mapInfos.Length];
		for (int i = 0; i < mapInfos.Length; i++)
			aiDetection[i] = mapInfos[i].aiDetection;

		missionName = new string[mapInfos.Length];
		for (int i = 0; i < mapInfos.Length; i++)
			missionName[i] = mapInfos[i].missionName;

		roundLimit = new int[mapInfos.Length];
		for (int i = 0; i < mapInfos.Length; i++)
			roundLimit[i] = mapInfos[i].roundLimit;

		mapSize = new int[mapInfos.Length];
		for (int i = 0; i < mapInfos.Length; i++)
			mapSize[i] = mapInfos[i].mapSize;

		victoryCondition = new int[mapInfos.Length];
		for (int i = 0; i < mapInfos.Length; i++)
			victoryCondition[i] = mapInfos[i].victoryCondition;

		generalLimit = new int[mapInfos.Length];
		for (int i = 0; i < mapInfos.Length; i++)
			generalLimit[i] = mapInfos[i].generalLimit;

		cityNames = new string[mapInfos.Length][];
		for (int i = 0; i < mapInfos.Length; i++)
			cityNames[i] = mapInfos[i].cityNames;

		cityStrategics = new bool[mapInfos.Length][];
		for (int i = 0; i < mapInfos.Length; i++) {
			if (mapInfos[i].cityStrategics == null)
				cityStrategics[i] = new bool[mapInfos[i].cityNames.Length];
			else {
				cityStrategics[i] = mapInfos[i].cityStrategics;
			}
		}

		countryCoordinateNames = new string[mapInfos.Length][];
		for (int i = 0; i < mapInfos.Length; i++)
			countryCoordinateNames[i] = mapInfos[i].countryCoordinateNames;

		cityTiers = new int[mapInfos.Length][];
		for (int i = 0; i < mapInfos.Length; i++)
			cityTiers[i] = mapInfos[i].cityTiers;

		//TODO: THIS MAY BE NULL, CHECK NULL WHEN CREATING MAP
		cityDefenceTiers = new int[mapInfos.Length][];
		for (int i = 0; i < mapInfos.Length; i++)
			cityDefenceTiers[i] = mapInfos[i].cityDefenceTiers;

		terrain = new int[mapInfos.Length][];
		for (int i = 0; i < mapInfos.Length; i++) {
			terrain[i] = mapInfos[i].terrain;
			for (int j = 0; j < terrain[i].Length; j++)
				terrain[i][j]++;
		}

		cityFactoryTiers = new int[mapInfos.Length][];
		for (int i = 0; i < mapInfos.Length; i++)
			cityFactoryTiers[i] = mapInfos[i].cityFactoryTiers;

		cityAirportTiers = new int[mapInfos.Length][];
		for (int i = 0; i < mapInfos.Length; i++)
			cityAirportTiers[i] = mapInfos[i].cityAirportTiers;

		cityNuclearTiers = new int[mapInfos.Length][];
		for (int i = 0; i < mapInfos.Length; i++) {
			try {
				cityNuclearTiers[i] = mapInfos[i].cityNuclearTiers;
			} catch { print("nuclear tab not found in map"); }
		}
		soldierIds = new int[mapInfos.Length][];
		for (int i = 0; i < mapInfos.Length; i++) {
			soldierIds[i] = mapInfos[i].soldierIds;
			for (int j = 0; j < soldierIds[i].Length; j++)
				soldierIds[i][j]++;
		}
		try {
			soldierGeneralLevels = new int[mapInfos.Length][];
			soldierGenerals = new string[mapInfos.Length][];
			for (int i = 0; i < mapInfos.Length; i++) {
				string[] generals = mapInfos[i].soldierGenerals;
				int[] levels = mapInfos[i].soldierGeneralLevels;
				if (generals != null) {
					for (int j = 0; j < generals.Length; j++) {
						if (CustomFunctions.GeneralUpdatedNames.ContainsKey(generals[j])) {
							generals[j] = CustomFunctions.GeneralUpdatedNames[generals[j]];
						}
					}
					soldierGenerals[i] = generals;
					soldierGeneralLevels[i] = levels;
				}



			}
		} catch (Exception e) {
			print("no generals in data set: " + e);
		}

		soldierVeterencies = new int[mapInfos.Length][];
		for (int i = 0; i < mapInfos.Length; i++)
			soldierVeterencies[i] = mapInfos[i].soldierVeterencies;

		soldierTiers = new int[mapInfos.Length][];
		for (int i = 0; i < mapInfos.Length; i++)
			soldierTiers[i] = mapInfos[i].soldierTiers;

		soldierIsStrategics = new bool[mapInfos.Length][];
		for (int i = 0; i < mapInfos.Length; i++)
			soldierIsStrategics[i] = mapInfos[i].soldierIsStrategics;

		soldierIsLocked = new bool[mapInfos.Length][];
		for (int i = 0; i < mapInfos.Length; i++)
			soldierIsLocked[i] = mapInfos[i].soldierIsLocked;

		cityCoordinates = new Vector2[mapInfos.Length][];
		for (int i = 0; i < mapInfos.Length; i++) {
			cityCoordinates[i] = new Vector2[mapInfos[i].cityCoordinates.Length];
			for (int j = 0; j < cityCoordinates[i].Length; j++) {
				cityCoordinates[i][j] = mapInfos[i].cityCoordinates[j].toVector2();
			}
		}
		countryCoordinates = new Vector2[mapInfos.Length][];
		for (int i = 0; i < mapInfos.Length; i++) {
			countryCoordinates[i] = new Vector2[mapInfos[i].countryCoordinates.Length];
			for (int j = 0; j < countryCoordinates[i].Length; j++) {
				countryCoordinates[i][j] = mapInfos[i].countryCoordinates[j].toVector2();
			}
		}
		terrainCoordinates = new Vector2[mapInfos.Length][];
		for (int i = 0; i < mapInfos.Length; i++) {
			terrainCoordinates[i] = new Vector2[mapInfos[i].terrainCoordinates.Length];
			for (int j = 0; j < terrainCoordinates[i].Length; j++) {
				terrainCoordinates[i][j] = mapInfos[i].terrainCoordinates[j].toVector2();
			}
		}
		soldierCoordinates = new Vector2[mapInfos.Length][];
		for (int i = 0; i < mapInfos.Length; i++) {
			soldierCoordinates[i] = new Vector2[mapInfos[i].soldierCoordinates.Length];
			for (int j = 0; j < soldierCoordinates[i].Length; j++) {
				soldierCoordinates[i][j] = mapInfos[i].soldierCoordinates[j].toVector2();
			}
		}
		try {
			events = new List<GameEvent>[mapInfos.Length];
			for (int i = 0; i < mapInfos.Length; i++) {
				events[i] = new List<GameEvent>(mapInfos[i].events);
			}
		} catch {
			print("no events in this map");
		}

	}
	//TEST FUNCTION FOR TRANSLATIONS IN MAPS
	void TestTranslations() {
		if (Input.GetKeyDown(KeyCode.A)) {
			List<string> untranslatedNames = new List<string>();
			for (int i = 0; i < levelData.Length; i++) {
				try {
					MapInfo m = JsonUtility.FromJson<MapInfo>(levelData[i].text);
					//cities
					foreach (string n in m.cityNames) {
						if (n != "" && !untranslatedNames.Contains(n) && CustomFunctions.TranslateText(n) == n) {
							untranslatedNames.Add(n);
						}
					}
					//beginning text translations
					if (m.beginningText != "" && !untranslatedNames.Contains(m.beginningText) && CustomFunctions.TranslateText(m.beginningText) == m.beginningText) {
						untranslatedNames.Add(m.beginningText);
					}
					//events
					foreach (GameEvent g in m.events) {
						if (!untranslatedNames.Contains(g.title) && CustomFunctions.TranslateText(g.title) == g.title) {
							untranslatedNames.Add(g.title);
						}
						if (!untranslatedNames.Contains(g.description) && CustomFunctions.TranslateText(g.description) == g.description) {
							untranslatedNames.Add(g.description);
						}
					}
				} catch {
				}
			}
			string s = "";
			foreach (string i in untranslatedNames) {
				s += i + "\n";
			}
			print(s);
		}
	}

	void Update() {
		//TestTranslations();
	}
}