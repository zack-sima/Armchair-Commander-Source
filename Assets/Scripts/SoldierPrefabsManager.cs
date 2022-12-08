using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierPrefabsManager : MonoBehaviour
{
    public Sprite sovietSMG, sovietLMG, americanSMG, americanLMG;
    public GameObject[] troopPrefabs;
    public static Dictionary<string, int> troopTypes = new Dictionary<string, int>() {
        {"Rifle Infantry", 0},
        {"Assault Team", 1},
        {"Motorized Infantry", 5},
        {"Mechanized Infantry", 6},
        {"Commando", 17},

        {"Light Tank", 4},
        {"Medium Tank", 2},
        {"Heavy Tank", 9},
        {"Modern Tank", 16},

        {"Infantry Artillery", 7},
        {"Field Artillery", 3},
        {"Self-Propelled Gun", 15},
        {"Rocket Artillery", 18},

        {"Destroyer", 11},
        {"Submarine", 12},
        {"Cruiser", 13},
        {"Carrier", 14},
        {"Battleship", 19},

        {"Bunker", 10},
        {"Turret", 8},
        {"Coastal Turret", 21},
        {"Missile Launcher", 20},

        {"Strafing Run", 70},
        {"Bombing Run", 71},

        {"Paratrooper Deployment", 72},
        {"Strategic Run", 73},
    };
    
}
