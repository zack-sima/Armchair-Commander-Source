using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class BinarySaveSystem {
    public static string ExportToJson(BinaryData data) {
        return JsonUtility.ToJson(data);
    }
    public static BinaryData ImportFromJson(string data) {
        return JsonUtility.FromJson<BinaryData>(data);
    }
    public static void SaveData(BinaryData data) {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(MyPlayerPrefs.GetDataPath() + "/playerdata.sav", FileMode.Create);

        bf.Serialize(stream, data);
        stream.Close();
    }
    //add integer parameter for different maps
    public static BinaryData LoadData() {
        if (File.Exists(MyPlayerPrefs.GetDataPath() + "/playerdata.sav")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(MyPlayerPrefs.GetDataPath() + "/playerdata.sav", FileMode.Open);
            BinaryData data = bf.Deserialize(stream) as BinaryData;
            stream.Close();
            return data;
        } else {
            return null;
        }
    }
}
[System.Serializable]
public class MyVector2 {
    public float x, y;
    public MyVector2(float x, float y) {
        this.x = x;
        this.y = y; 
    }
    public MyVector2(Vector2 coordinates) {
        x = coordinates.x;
        y = coordinates.y; 
    }
    public MyVector2() {
    }
    public Vector2 toVector2() {
        return new Vector2(x, y); 
    }
}
[System.Serializable]
public class BinaryData {
    public int map;
    public Dictionary<string, CountryData> countryDatas;
    public Dictionary<MyVector2, string> tilesCountries, originalTileCountries;
    //for multiplayer json
    public List<MyVector2> tilesCountriesKeys;
    public List<string> tilesCountriesValues;
    public List<MyVector2> tilesRadiationKeys;
    public List<int> tilesRadiationValues;
    public List<MyVector2> cityHealthKeys, cityBombKeys;
    public List<float> cityHealthValues;
    public List<int> cityBombTypes, cityBombRounds;
    public List<MyVector2> unitPositions; 
    public List<int> unitTypes, unitTiers, unitXps;
    public List<string> unitDefaultGenerals; //prevents generals from being taken away if assigned at default
    public List<string> unitCountries, unitGenerals;

    public List<int> unitGeneralsLevels; //NOTE: levels were not saved until recent update

    public List<float> unitHealths;
    public List<bool> unitsMoved, unitsAttacked, unitsSupplied, unitsIsStrategic, unitsIsLocked;
    public List<bool> unitsFlippedHorizontal; //remember units' rotations
    public int round;
    public bool viewOnly;
    public bool usedNukes; //ai won't use nukes unless player uses them first
    //used in multiplayer to check if it is the same round (would be so rare to have the same round it is neglectable)
    public double randomId;

    public Dictionary<string, int> countriesIsAxis; //NEW ADDITION FOR DIPLOMACY
    public List<string> countriesIsNeutral;

    public List<GameEvent> events; //NEW ADDITION EVENTS
    

    public BinaryData(int map, Dictionary<string, CountryData> countryDatas, Dictionary<Vector2, Tile> tiles, List<Unit> units, Controller controller) {
        this.map = map;
        this.round = controller.round;
        this.countryDatas = new Dictionary<string, CountryData>(countryDatas);
        //foreach (string i in countryDatas.Keys) {
        //    this.countryDatas.Add(i, countryDatas[i]);
        //}
        viewOnly = !controller.passingRound;
        usedNukes = controller.usedNukes;
        
        tilesCountries = new Dictionary<MyVector2, string>();
        //used to check whether the tile has been captured
        originalTileCountries = new Dictionary<MyVector2, string>();
        cityHealthKeys = new List<MyVector2>();
        cityHealthValues = new List<float>();
        cityBombKeys = new List<MyVector2>();
        cityBombRounds = new List<int>();
        cityBombTypes = new List<int>();
        unitPositions = new List<MyVector2>();
        unitTypes = new List<int>();
        unitTiers = new List<int>();
        unitXps = new List<int>();
        unitHealths = new List<float>();
        unitsMoved = new List<bool>();
        unitsSupplied = new List<bool>();
        unitsAttacked = new List<bool>();
        unitsIsStrategic = new List<bool>();
        unitsIsLocked = new List<bool>();
        unitCountries = new List<string>();
        unitGenerals = new List<string>();
        unitGeneralsLevels = new List<int>();
        unitDefaultGenerals = new List<string>();
        unitsFlippedHorizontal = new List<bool>();
        countriesIsAxis = new Dictionary<string, int>(controller.countriesIsAxis);
        countriesIsNeutral = new List<string>(controller.countriesIsNeutral);
        events = new List<GameEvent>(controller.events); //this is saved here as well because the controller will delete events that have already happened

        randomId = Time.deltaTime * Time.realtimeSinceStartup * Random.Range(0f, 1f);

        foreach (City i in controller.cities) {
            if (i != null && i.currentTile != null) {
                cityHealthKeys.Add(new MyVector2(i.currentTile.coordinate));
                cityHealthValues.Add(i.health);
                cityBombKeys.Add(new MyVector2(i.currentTile.coordinate));
                cityBombRounds.Add(i.roundsToBombProduction);
                cityBombTypes.Add(i.bombInProduction);
            }
        }
        foreach (Tile i in tiles.Values) { 
            if (i != null) {
                tilesCountries.Add(new MyVector2(i.coordinate), i.country);
            }
        }
        foreach (KeyValuePair<Vector2, string> kv in controller.originalTiles) {
            originalTileCountries.Add(new MyVector2(kv.Key), kv.Value);
        }

        tilesRadiationKeys = new List<MyVector2>();
        tilesRadiationValues = new List<int>();
        foreach (Tile t in tiles.Values) {
            tilesRadiationKeys.Add(new MyVector2(t.coordinate));
            tilesRadiationValues.Add(t.radiationLeft);
        }

        tilesCountriesKeys = new List<MyVector2>(tilesCountries.Keys);
        tilesCountriesValues = new List<string>(tilesCountries.Values);
        foreach (Unit i in units) {
            if (i != null && i.currentTile != null) {
                unitDefaultGenerals.Add(i.defaultGeneral);
                unitPositions.Add(new MyVector2(i.currentTile.coordinate));
                unitTypes.Add(i.troopId);
                unitTiers.Add(i.tier);
                unitXps.Add(i.veterency);
                unitHealths.Add(i.health);
                unitsMoved.Add(i.moved);
                unitsSupplied.Add(i.supplied);
                unitsAttacked.Add(i.attacked);
                unitsIsStrategic.Add(i.isStrategic);
                unitsIsLocked.Add(i.isLocked);
                unitCountries.Add(i.country);
                unitGenerals.Add(i.general);
                unitGeneralsLevels.Add(i.generalLevel);
                unitsFlippedHorizontal.Add(i.flippedHorizontal);
            }
        }
    }
}
