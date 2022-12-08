using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public enum PurchaseCategory {
    infantry, armor, artillery, aerial, fortification, naval, nuclear
}
public class ProductBar : MonoBehaviour {
    public Dictionary<string, int> troopTypes, airportTroopTypes, troopIndustryRequirements, troopAirportRequirements, troopNuclearRequirements;
    public Dictionary<int, string> troopNamesByTypes;
    public Image statsBackgroundImage;
    public Text manpowerDisplay, industryDisplay, technologyDisplay, fuelDisplay;
    [HideInInspector]
    public int airplaneOilCost, airplaneIndustryCost; //temporary storage
    [HideInInspector]
    public int[] airplaneRanges;
    public int[][] troopManpowerCosts, troopIndustryCosts, troopOilCosts;
    public Dictionary<Skin, int[][]> customManpowerCosts, customIndustryCosts, customOilCosts;

    public string[][] troopNames;
    public GameObject[] airplanes;
    public Controller controller; 
    public PurchaseCategory selectedCategory;
    public Text uiTitleText;
    public Text stackSizeText;
    public int stackSize;
    public TroopPurchaseDisplay[] productCatalogues; //enable/disalble based on category of purchase (enumera)
    public Button[] categoryButtons;  
    public void changePurchaseCategory(int category) {
        


        if (controller.selectedCity == null || controller.selectedCity.currentTile.occupant == null || (controller.selectedCity.currentTile.city.airportTier <= 0 && controller.selectedCity.currentTile.city.nuclearTier <= 0)) {
            selectedCategory = (PurchaseCategory)category;
        } else if (category != 6 && controller.selectedCity.currentTile.city.airportTier > 0) {
            selectedCategory = PurchaseCategory.aerial;
        } else {
            selectedCategory = PurchaseCategory.nuclear;
        }
        if (selectedCategory == PurchaseCategory.aerial || selectedCategory == PurchaseCategory.fortification || selectedCategory == PurchaseCategory.nuclear)
            stackSize = 1;
        updateProducts();
    }
    void Start() {
        //shared by almanac
        troopTypes = SoldierPrefabsManager.troopTypes;

        troopNamesByTypes = new Dictionary<int, string>();
        for (int i = 0; i < troopTypes.Count; i++)
            troopNamesByTypes.Add(new List<int>(troopTypes.Values)[i], new List<string>(troopTypes.Keys)[i]);

        airportTroopTypes = new Dictionary<string, int>();
        airportTroopTypes.Add("Strafing Run", 0);
        airportTroopTypes.Add("Bombing Run", 1);
        airportTroopTypes.Add("Paratrooper Deployment", 2);
        airportTroopTypes.Add("Strategic Run", 3);


        troopIndustryRequirements = new Dictionary<string, int>();
        troopIndustryRequirements.Add("Rifle Infantry", 0);
        troopIndustryRequirements.Add("Assault Team", 0);
        troopIndustryRequirements.Add("Light Tank", 1);
        troopIndustryRequirements.Add("Medium Tank", 2);
        troopIndustryRequirements.Add("Heavy Tank", 3);
        troopIndustryRequirements.Add("Field Artillery", 2);
        troopIndustryRequirements.Add("Motorized Infantry", 1); //2
        troopIndustryRequirements.Add("Strafing Run", 0);
        troopIndustryRequirements.Add("Bombing Run", 0);
        troopIndustryRequirements.Add("Strategic Run", 0);

        troopIndustryRequirements.Add("Atomic Bomb", 0);
        troopIndustryRequirements.Add("Hydrogen Bomb", 0);
        troopIndustryRequirements.Add("Antimatter Bomb", 0);

        troopIndustryRequirements.Add("Paratrooper Deployment", 0);
        troopIndustryRequirements.Add("Mechanized Infantry", 2);
        troopIndustryRequirements.Add("Turret", 0);
        troopIndustryRequirements.Add("Coastal Turret", 0);

        troopIndustryRequirements.Add("Bunker", 0);
        troopIndustryRequirements.Add("Infantry Artillery", 1);
        troopIndustryRequirements.Add("Destroyer", 0);
        troopIndustryRequirements.Add("Submarine", 1);
        troopIndustryRequirements.Add("Cruiser", 2);
        troopIndustryRequirements.Add("Carrier", 3);
        troopIndustryRequirements.Add("Self-Propelled Gun", 3);
        troopIndustryRequirements.Add("Modern Tank", 4);
        troopIndustryRequirements.Add("Commando", 2);
        troopIndustryRequirements.Add("Rocket Artillery", 3);
        troopIndustryRequirements.Add("Missile Launcher", 4);

        troopIndustryRequirements.Add("Battleship", 4);

        troopAirportRequirements = new Dictionary<string, int>();
        troopAirportRequirements.Add("Rifle Infantry", 0);
        troopAirportRequirements.Add("Assault Team", 0);
        troopAirportRequirements.Add("Light Tank", 0);
        troopAirportRequirements.Add("Medium Tank", 0);
        troopAirportRequirements.Add("Heavy Tank", 0);
        troopAirportRequirements.Add("Turret", 0);
        troopAirportRequirements.Add("Coastal Turret", 0);
        troopAirportRequirements.Add("Bunker", 0);
        troopAirportRequirements.Add("Field Artillery", 0);
        troopAirportRequirements.Add("Motorized Infantry", 0); //2
        troopAirportRequirements.Add("Strafing Run", 1);
        troopAirportRequirements.Add("Bombing Run", 1);
        troopAirportRequirements.Add("Strategic Run", 3);

        troopAirportRequirements.Add("Atomic Bomb", 0);
        troopAirportRequirements.Add("Hydrogen Bomb", 0);
        troopAirportRequirements.Add("Antimatter Bomb", 0);

        troopAirportRequirements.Add("Paratrooper Deployment", 2);
        troopAirportRequirements.Add("Mechanized Infantry", 0);
        troopAirportRequirements.Add("Infantry Artillery", 0);
        troopAirportRequirements.Add("Destroyer", 0);
        troopAirportRequirements.Add("Submarine", 0);
        troopAirportRequirements.Add("Cruiser", 0);
        troopAirportRequirements.Add("Carrier", 0);
        troopAirportRequirements.Add("Self-Propelled Gun", 0);
        troopAirportRequirements.Add("Modern Tank", 0);
        troopAirportRequirements.Add("Commando", 0);
        troopAirportRequirements.Add("Rocket Artillery", 0);
        troopAirportRequirements.Add("Battleship", 0);
        troopAirportRequirements.Add("Missile Launcher", 0);

        troopNuclearRequirements = new Dictionary<string, int>();
        troopNuclearRequirements.Add("Rifle Infantry", 0);
        troopNuclearRequirements.Add("Assault Team", 0);
        troopNuclearRequirements.Add("Light Tank", 0);
        troopNuclearRequirements.Add("Medium Tank", 0);
        troopNuclearRequirements.Add("Heavy Tank", 0);
        troopNuclearRequirements.Add("Turret", 0);
        troopNuclearRequirements.Add("Coastal Turret", 0);
        troopNuclearRequirements.Add("Bunker", 0);
        troopNuclearRequirements.Add("Field Artillery", 0);
        troopNuclearRequirements.Add("Motorized Infantry", 0); //2
        troopNuclearRequirements.Add("Strafing Run", 0);
        troopNuclearRequirements.Add("Bombing Run", 0);
        troopNuclearRequirements.Add("Strategic Run", 0);

        troopNuclearRequirements.Add("Atomic Bomb", 1);
        troopNuclearRequirements.Add("Hydrogen Bomb", 2);
        troopNuclearRequirements.Add("Antimatter Bomb", 3);

        troopNuclearRequirements.Add("Paratrooper Deployment", 0);
        troopNuclearRequirements.Add("Mechanized Infantry", 0);
        troopNuclearRequirements.Add("Infantry Artillery", 0);
        troopNuclearRequirements.Add("Destroyer", 0);
        troopNuclearRequirements.Add("Submarine", 0);
        troopNuclearRequirements.Add("Cruiser", 0);
        troopNuclearRequirements.Add("Carrier", 0);
        troopNuclearRequirements.Add("Self-Propelled Gun", 0);
        troopNuclearRequirements.Add("Modern Tank", 0);
        troopNuclearRequirements.Add("Commando", 0);
        troopNuclearRequirements.Add("Rocket Artillery", 0);
        troopNuclearRequirements.Add("Battleship", 0);
        troopNuclearRequirements.Add("Missile Launcher", 0);

        troopNames = new string[][] {
            new string[] {"Rifle Infantry", "Assault Team", "Motorized Infantry", "Mechanized Infantry", "Commando"},
            new string[] {"Light Tank", "Medium Tank", "Heavy Tank", "Modern Tank"},
            new string[] {"Infantry Artillery", "Field Artillery", "Self-Propelled Gun", "Rocket Artillery" },
            new string[] {"Strafing Run", "Bombing Run", "Paratrooper Deployment", "Strategic Run"},
            new string[] {"Bunker", "Turret", "Coastal Turret", "Missile Launcher" },
            new string[] {"Destroyer", "Submarine", "Cruiser", "Carrier", "Battleship" },
            new string[] {"Atomic Bomb", "Hydrogen Bomb", "Antimatter Bomb" }
        };

        airplaneRanges = new int[] {
            7, 9, 7, 12
        };

        //**STANDARD PRICING**

        troopManpowerCosts = new int[][] {
            new int[] {60, 85, 120, 135, 155},
            new int[] {90, 120, 150, 175},
            new int[] {65, 90, 115, 120},
            new int[] {0, 0, 0, 0},
            new int[] {65, 80, 85, 125},
            new int[] {75, 55, 95, 150, 135},
            new int[] {150, 250, 350}
        };


        troopIndustryCosts = new int[][] {
            new int[] {3, 10, 20, 25, 20},
            new int[] {30, 45, 75, 105},
            new int[] {20, 35, 60, 60 },
            new int[] {20, 25, 15, 35},
            new int[] {25, 30, 35, 125},
            new int[] {35, 45, 65, 95, 75},
            new int[] {450, 750, 1000},

        };
        troopOilCosts = new int[][] {
            new int[] {0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0},
            new int[] {0, 0, 0, 0},
            new int[] {15, 20, 15, 30},
            new int[] {0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0},
            new int[] {0, 0, 0}
        };

        //**CUSTOM UNIT PRICINGS*******

        customManpowerCosts = new Dictionary<Skin, int[][]>();
        customIndustryCosts = new Dictionary<Skin, int[][]>();
        customOilCosts = new Dictionary<Skin, int[][]>();

        //**JAPAN (difference in cost)
        int[][] newManpower = new int[][] {
            new int[] {0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0},
            new int[] {0, 0, 0, 0},
            new int[] {0, 0, 0, 0},
            new int[] {0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0},
            new int[] {0, 0, 0}
         };
       

        int[][] newIndustry = new int[][] { //generally less industry required and tanks/artillery are crappier as a result
            new int[] {0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0},
            new int[] {0, 0, 0, 0},
            new int[] {0, 0, 0, 0},
            new int[] {0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0},
            new int[] {0, 0, 0}
         };
        

        int[][] newOil = new int[][] { //slightly less oil required for air
            new int[] {0, 0, 0, 0, 0}, //doesn't do anything
            new int[] {0, 0, 0, 0}, //doesn't do anything
            new int[] {0, 0, 0, 0}, //doesn't do anything
            new int[] {0, 0, 0, 0}, //CHANGE THIS
            new int[] {0, 0, 0, 0}, //doesn't do anything
            new int[] {0, 0, 0, 0, 0}, //doesn't do anything
            new int[] {0, 0, 0}
        };
        customManpowerCosts.Add(Skin.Japanese, newManpower);
        customIndustryCosts.Add(Skin.Japanese, newIndustry);
        customOilCosts.Add(Skin.Japanese, newOil);

        //**SOVIET

        //the costs here are differences from the original
        newManpower = new int[][] {
            new int[] {-5, -5, -5, -5, -10},
            new int[] {-5, -5, -10, -10},
            new int[] {0, 0, 0, 0},
            new int[] {0, 0, 0, 0},
            new int[] {0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0},
            new int[] {0, 0, 0}
        };

        newIndustry = new int[][] {
            new int[] {0, -2, -3, -3, -5},
            new int[] {-5, -5, -10, -15},
            new int[] {0, 0, 0, 0},
            new int[] {-2, -3, -3, -5},
            new int[] {0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0},
            new int[] {0, 0, 0}
         };

        newOil = new int[][] {
            new int[] {0, 0, 0, 0, 0}, //doesn't do anything
            new int[] {0, 0, 0, 0}, //doesn't do anything
            new int[] {0, 0, 0, 0}, //doesn't do anything
            new int[] {2, 3, 3, 5}, //CHANGE THIS
            new int[] {0, 0, 0, 0}, //doesn't do anything
            new int[] {0, 0, 0, 0, 0}, //doesn't do anything
            new int[] {0, 0, 0}
         };

        customManpowerCosts.Add(Skin.Soviet, newManpower);
        customIndustryCosts.Add(Skin.Soviet, newIndustry);
        customOilCosts.Add(Skin.Soviet, newOil);

        //**AMERICAN

        //the costs here are differences from the original
        newManpower = new int[][] {
            new int[] {-5, -5, -5, -5, -10},
            new int[] {-5, -5, -5, -10},
            new int[] {0, 0, 0, 10},
            new int[] {0, 0, 0, 0},
            new int[] {0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0},
            new int[] {0, 0, 0}
        };

        newIndustry = new int[][] {
            new int[] {0, -2, -3, -3, -5},
            new int[] {-5, -10, 0, -5},
            new int[] {0, 0, 0, 15},
            new int[] {0, 0, 0, 0},
            new int[] {0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0},
            new int[] {0, 0, 0}
         };

        newOil = new int[][] {
            new int[] {0, 0, 0, 0, 0}, //doesn't do anything
            new int[] {0, 0, 0, 0}, //doesn't do anything
            new int[] {0, 0, 0, 0}, //doesn't do anything
            new int[] {-2, -2, -2, -5}, //CHANGE THIS
            new int[] {0, 0, 0, 0}, //doesn't do anything
            new int[] {0, 0, 0, 0, 0}, //doesn't do anything
            new int[] {0, 0, 0}
         };

        customManpowerCosts.Add(Skin.American, newManpower);
        customIndustryCosts.Add(Skin.American, newIndustry);
        customOilCosts.Add(Skin.American, newOil);

        //**FRENCH

        //the costs here are differences from the original
        newManpower = new int[][] {
            new int[] {-5, -5, -5, -5, -10},
            new int[] {-5, -5, -5, -10},
            new int[] {0, 0, 0, 10},
            new int[] {0, 0, 0, 0},
            new int[] {0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0},
            new int[] {0, 0, 0}
        };

        newIndustry = new int[][] {
            new int[] {0, -2, -3, -3, -5},
            new int[] {-5, -10, 0, -5},
            new int[] {0, 0, 0, 15},
            new int[] {0, 0, 0, 0},
            new int[] {0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0},
            new int[] {0, 0, 0}
         };

        newOil = new int[][] {
            new int[] {0, 0, 0, 0, 0}, //doesn't do anything
            new int[] {0, 0, 0, 0}, //doesn't do anything
            new int[] {0, 0, 0, 0}, //doesn't do anything
            new int[] {-2, -2, -2, -5}, //CHANGE THIS
            new int[] {0, 0, 0, 0}, //doesn't do anything
            new int[] {0, 0, 0, 0, 0}, //doesn't do anything
            new int[] {0, 0, 0}
         };

        customManpowerCosts.Add(Skin.French, newManpower);
        customIndustryCosts.Add(Skin.French, newIndustry);
        customOilCosts.Add(Skin.French, newOil);

        //**BRITISH

        //the costs here are differences from the original
        newManpower = new int[][] {
            new int[] {0, 0, 0, 0, -5},
            new int[] {-5, -5, -5, -5},
            new int[] {0, 0, 0, 10},
            new int[] {0, 0, 0, 0},
            new int[] {0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0},
            new int[] {0, 0, 0}
        };

        newIndustry = new int[][] {
            new int[] {0, 0, 0, 0, -5},
            new int[] {0, 0, 0, 0},
            new int[] {0, 0, 0, 15},
            new int[] {0, 0, 0, 0},
            new int[] {0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0},
            new int[] {0, 0, 0}
         };

        newOil = new int[][] {
            new int[] {0, 0, 0, 0, 0}, //doesn't do anything
            new int[] {0, 0, 0, 0}, //doesn't do anything
            new int[] {0, 0, 0, 0}, //doesn't do anything
            new int[] {-2, -2, -2, -5}, //CHANGE THIS
            new int[] {0, 0, 0, 0}, //doesn't do anything
            new int[] {0, 0, 0, 0, 0}, //doesn't do anything
            new int[] {0, 0, 0}
         };

        customManpowerCosts.Add(Skin.British, newManpower);
        customIndustryCosts.Add(Skin.British, newIndustry);
        customOilCosts.Add(Skin.British, newOil);
        //**GERMAN

        //the costs here are differences from the original
        newManpower = new int[][] {
            new int[] {0, 0, 0, 0, 0},
            new int[] {0, 5, 10, 15},
            new int[] {0, 0, 0, 10},
            new int[] {0, 0, 0, 0},
            new int[] {0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0},
            new int[] {0, 0, 0}
        };

        newIndustry = new int[][] {
            new int[] {0, -2, -3, -3, -5},
            new int[] {0, 5, 10, 15},
            new int[] {0, 0, 0, 15},
            new int[] {0, 0, 0, 0},
            new int[] {0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0},
            new int[] {0, 0, 0}

         };

        newOil = new int[][] {
            new int[] {0, 0, 0, 0, 0}, //doesn't do anything
            new int[] {0, 0, 0, 0}, //doesn't do anything
            new int[] {0, 0, 0, 0}, //doesn't do anything
            new int[] {0, 0, 0, 0}, //CHANGE THIS
            new int[] {0, 0, 0, 0}, //doesn't do anything
            new int[] {0, 0, 0, 0, 0}, //doesn't do anything
            new int[] {0, 0, 0}
         };

        customManpowerCosts.Add(Skin.German, newManpower);
        customIndustryCosts.Add(Skin.German, newIndustry);
        customOilCosts.Add(Skin.German, newOil);



        updateProducts();
        controller.hideMUI();
    }
    public int CalculateTroopManpowerCost(int row, int column, string country="") {
        if (country != "" && customManpowerCosts.ContainsKey(CustomFunctions.DetermineTroopSkin(country, controller.originalCountriesIsAxis[country]))) {
            return troopManpowerCosts[row][column] + customManpowerCosts[CustomFunctions.DetermineTroopSkin(country, controller.originalCountriesIsAxis[country])][row][column];
        } else {
            return troopManpowerCosts[row][column];
        }
    }
    public int CalculateTroopIndustryCost(int row, int column, string country = "") {
        if (country != "" && customIndustryCosts.ContainsKey(CustomFunctions.DetermineTroopSkin(country, controller.originalCountriesIsAxis[country]))) {
            return troopIndustryCosts[row][column] + customIndustryCosts[CustomFunctions.DetermineTroopSkin(country, controller.originalCountriesIsAxis[country])][row][column];
        } else {
            return troopIndustryCosts[row][column];
        }
    }
    public int CalculateTroopOilCost(int row, int column, string country="") {
        if (country != "" && customOilCosts.ContainsKey(CustomFunctions.DetermineTroopSkin(country, controller.originalCountriesIsAxis[country]))) {
            return troopOilCosts[row][column] + customOilCosts[CustomFunctions.DetermineTroopSkin(country, controller.originalCountriesIsAxis[country])][row][column];
        } else {
            return troopOilCosts[row][column];
        }
    }
    public void purchaseProduct(int productId) {
        if (selectedCategory == PurchaseCategory.nuclear) {
            if (CalculateTroopManpowerCost((int)selectedCategory, productId, controller.playerCountry) <= controller.countryDatas[controller.playerCountry].manpower &&
                CalculateTroopIndustryCost((int)selectedCategory, productId, controller.playerCountry) <= controller.countryDatas[controller.playerCountry].industry) {
                City c = controller.selectedCity;
                c.roundsToBombProduction = 3 + productId;
                c.bombInProduction = productId;

                controller.countryDatas[controller.playerCountry].manpower -= CalculateTroopManpowerCost((int)selectedCategory, productId, controller.playerCountry);
                controller.countryDatas[controller.playerCountry].industry -= CalculateTroopIndustryCost((int)selectedCategory, productId, controller.playerCountry);

                productCatalogues[productId].unitNameDisplay.text = controller.selectedCity.roundsToBombProduction + " " + CustomFunctions.TranslateText("rounds");

            }
        } else if (airportTroopTypes.ContainsValue(0) && selectedCategory == PurchaseCategory.aerial) {
            if (CalculateTroopOilCost((int)selectedCategory, productId, controller.playerCountry) <= controller.countryDatas[controller.playerCountry].fuel &&
                CalculateTroopIndustryCost((int)selectedCategory, productId, controller.playerCountry) <= controller.countryDatas[controller.playerCountry].industry) {
                //aerial product
                airplaneIndustryCost = CalculateTroopIndustryCost((int)selectedCategory, productId, controller.playerCountry);
                airplaneOilCost = CalculateTroopOilCost((int)selectedCategory, productId, controller.playerCountry);
                controller.airplaneMode = true;
                
                if (productId == 2) {
                    //print("paratroop");
                    controller.findParatrooperRange(airplaneRanges[productId] + controller.CheckCountryTech(controller.playerCountry, Controller.TechTroopType.Air, Controller.TechCategory.Range));
                } else {
                   //print("air: " + controller.CheckCountryTech(controller.playerCountry, Controller.TechTroopType.Air, Controller.TechCategory.Range));
                    controller.findAirStrikeRange(airplaneRanges[productId] + controller.CheckCountryTech(controller.playerCountry, Controller.TechTroopType.Air, Controller.TechCategory.Range));
                }
                controller.airplaneType = productId;
                controller.buildButton.position = new Vector3(-1000f, controller.buildButton.position.y, 0f);
                StartCoroutine(delayHideUI());
                controller.canBuild = false; 
            }
        } else if (CalculateTroopManpowerCost((int)selectedCategory, productId, controller.playerCountry) * stackSize <= controller.countryDatas[controller.playerCountry].manpower &&
                CalculateTroopIndustryCost((int)selectedCategory, productId, controller.playerCountry) * stackSize <= controller.countryDatas[controller.playerCountry].industry && (controller.selectedBuildTile != null || (controller.selectedCity.currentTile.occupant == null && controller.selectedCity.currentTile.country == controller.playerCountry))) {
            controller.countryDatas[controller.playerCountry].manpower -= CalculateTroopManpowerCost((int)selectedCategory, productId, controller.playerCountry) * stackSize; 
            controller.countryDatas[controller.playerCountry].industry -= CalculateTroopIndustryCost((int)selectedCategory, productId, controller.playerCountry) * stackSize;

            Tile t = controller.selectedBuildTile;
            if (controller.selectedBuildTile == null)
                t = controller.selectedCity.currentTile;

            controller.spawnSoldier(t.coordinate,
                controller.playerCountry, controller.countriesIsAxis[controller.playerCountry],
                troopTypes[troopNames[(int)selectedCategory][productId]], 0, stackSize, false);
            t.occupant.moved = true;
            t.occupant.attacked = true;
            controller.buildButton.position = new Vector3(-1000f, controller.buildButton.position.y, 0f);
            StartCoroutine(delayHideUI());
            controller.canBuild = false; 
        }
    }
    IEnumerator delayHideUI() {
        yield return null;
        controller.hideMUI(); 
    }
    public bool findNearbyCityFactoryLevel(int level, string ownerCountry) { //for missile launcher and buildings that require nearby factory
        if (level == 0)
            return true;
        if (controller.selectedBuildTile != null) {
            Tile theTile = controller.selectedBuildTile;
            if (theTile.upperLeft != null && theTile.upperLeft.isCity && theTile.upperLeft.country == ownerCountry && theTile.upperLeft.city.factoryTier >= level ||
                theTile.upperRight != null && theTile.upperRight.isCity && theTile.upperRight.country == ownerCountry && theTile.upperRight.city.factoryTier >= level ||
                theTile.lowerLeft != null && theTile.lowerLeft.isCity && theTile.lowerLeft.country == ownerCountry && theTile.lowerLeft.city.factoryTier >= level ||
                theTile.top != null && theTile.top.isCity && theTile.top.country == ownerCountry && theTile.top.city.factoryTier >= level ||
                theTile.bottom != null && theTile.bottom.isCity && theTile.bottom.country == ownerCountry && theTile.bottom.city.factoryTier >= level ||
                theTile.lowerRight != null && theTile.lowerRight.isCity && theTile.lowerRight.country == ownerCountry && theTile.lowerRight.city.factoryTier >= level) {
                return true;
            } else {
                return false;
            }
        }
        return false;
    }
    public void updateProducts() {
        if (controller.selectedCity == null && controller.selectedBuildTile == null)
            return;
        for (int i = 0; i < productCatalogues.Length; i++) {
            foreach (Image j in productCatalogues[i].transform.GetComponentsInChildren<Image>())
                j.enabled = false;
            foreach (Text j in productCatalogues[i].transform.GetComponentsInChildren<Text>())
                j.enabled = false;
            foreach (Button j in productCatalogues[i].transform.GetComponentsInChildren<Button>())
                j.enabled = false;
        } 
        for (int i = 0; i < troopManpowerCosts[(int)selectedCategory].Length; i++) { 
            if (controller.selectedBuildTile != null && findNearbyCityFactoryLevel(troopIndustryRequirements[troopNames[(int)selectedCategory][i]], controller.playerCountry) ||
                controller.selectedBuildTile == null && controller.selectedCity.factoryTier >= troopIndustryRequirements[troopNames[(int)selectedCategory][i]] &&
                controller.selectedCity.airportTier >= troopAirportRequirements[troopNames[(int)selectedCategory][i]] &&
                controller.selectedCity.nuclearTier >= troopNuclearRequirements[troopNames[(int)selectedCategory][i]] &&
                (controller.selectedCity.currentTile.occupant == null || selectedCategory == PurchaseCategory.aerial || selectedCategory == PurchaseCategory.nuclear)) {

                productCatalogues[i].unitIndustryCost = CalculateTroopIndustryCost((int)selectedCategory, i, controller.playerCountry);
                productCatalogues[i].unitManpowerCost = CalculateTroopManpowerCost((int)selectedCategory, i, controller.playerCountry);
                productCatalogues[i].unitOilCost = CalculateTroopOilCost((int)selectedCategory, i, controller.playerCountry);

                if (selectedCategory == PurchaseCategory.nuclear && controller.selectedCity.roundsToBombProduction != 0 && i == controller.selectedCity.bombInProduction) {
                    productCatalogues[i].unitNameDisplay.text = controller.selectedCity.roundsToBombProduction + " " + CustomFunctions.TranslateText("rounds");
                } else
                    productCatalogues[i].unitNameDisplay.text = CustomFunctions.TranslateText(troopNames[(int)selectedCategory][i]);
                foreach (Image j in productCatalogues[i].transform.GetComponentsInChildren<Image>())
                    j.enabled = true;
                foreach (Text j in productCatalogues[i].transform.GetComponentsInChildren<Text>())
                    j.enabled = true;
                foreach (Button j in productCatalogues[i].transform.GetComponentsInChildren<Button>())
                    j.enabled = true; 
            } 
        }

    }
    public void reduceStackSize () {
        if (stackSize > 1)
            stackSize--;
        if (selectedCategory == PurchaseCategory.aerial || selectedCategory == PurchaseCategory.fortification)
            stackSize = 1;
        stackSizeText.text = stackSize.ToString();
    }
    public void addStackSize () {
        if (stackSize < 4)
            stackSize++;
        if (selectedCategory == PurchaseCategory.aerial || selectedCategory == PurchaseCategory.fortification)
            stackSize = 1;
        stackSizeText.text = stackSize.ToString();  
    }

    void Update() {
        if (controller.editMode)
            return;
        if (controller.selectedBuildTile == null && controller.selectedCity != null && !controller.selectedCity.isPort) {
            for (int i = 0; i < 5; i++) {
                categoryButtons[i].gameObject.SetActive(true);
                if (categoryButtons[i].interactable != ((int)selectedCategory != i))
                    categoryButtons[i].interactable = (int)selectedCategory != i;
            }
            if (selectedCategory == PurchaseCategory.nuclear) {
                categoryButtons[4].interactable = false;
            } else {
                categoryButtons[4].interactable = true;
            }
            //hide options not available in city
            if (controller.selectedCity != null && controller.selectedCity.currentTile.occupant != null) {
                categoryButtons[0].gameObject.SetActive(false);
                categoryButtons[1].gameObject.SetActive(false);
                categoryButtons[2].gameObject.SetActive(false);
            }
            if (controller.selectedCity != null/* && controller.selectedCity.currentTile.occupant == null*/ && controller.selectedCity.currentTile.city.airportTier <= 0) {
                categoryButtons[3].gameObject.SetActive(false);
            }
            if (controller.selectedCity != null/* && controller.selectedCity.currentTile.occupant == null */&& controller.selectedCity.currentTile.city.nuclearTier <= 0) {
                categoryButtons[4].gameObject.SetActive(false);
            }
            if (controller.selectedCity != null/* && controller.selectedCity.currentTile.occupant == null */&& controller.selectedCity.currentTile.city.factoryTier <= 0) {
                categoryButtons[1].gameObject.SetActive(false);
                categoryButtons[2].gameObject.SetActive(false);
            }

        } else {
            categoryButtons[0].interactable = false;
            categoryButtons[0].gameObject.SetActive(true);

        }
        if (selectedCategory == PurchaseCategory.aerial || selectedCategory == PurchaseCategory.fortification)
            stackSize = 1;
        manpowerDisplay.text = controller.countryDatas[controller.playerCountry].manpower.ToString();
        industryDisplay.text = controller.countryDatas[controller.playerCountry].industry.ToString();
        fuelDisplay.text = controller.countryDatas[controller.playerCountry].fuel.ToString();

        if (controller.selectedBuildTile != null || (controller.selectedCity != null && controller.selectedCity.isPort)) {
            if (categoryButtons[1].transform.position.y > -5000) {
                for (int i = 1; i < 5; i++) {
                    categoryButtons[i].transform.position = new Vector2(categoryButtons[i].transform.position.x, categoryButtons[i].transform.position.y - 10000f);
                }
                
            }
            if (controller.selectedBuildTile != null) {
                if (selectedCategory != PurchaseCategory.fortification)
                    changePurchaseCategory(4);

                categoryButtons[0].GetComponentInChildren<Text>().text = CustomFunctions.TranslateText("Buildings");
                uiTitleText.text = CustomFunctions.TranslateText("Construction Site");
            } else {
                if (selectedCategory != PurchaseCategory.naval)
                    changePurchaseCategory(5);

                categoryButtons[0].GetComponentInChildren<Text>().text = CustomFunctions.TranslateText("Navy");
                uiTitleText.text = CustomFunctions.TranslateText("Naval Production");
            }
        } else {
            if (categoryButtons[1].transform.position.y < -5000)
                for (int i = 1; i < 5; i++) {
                    categoryButtons[i].transform.position = new Vector2(categoryButtons[i].transform.position.x, categoryButtons[i].transform.position.y + 10000f);
                }
            if (uiTitleText.text != CustomFunctions.TranslateText("City Production")) {
                updateProducts();
                uiTitleText.text = CustomFunctions.TranslateText("City Production");
                categoryButtons[0].GetComponentInChildren<Text>().text = CustomFunctions.TranslateText("Infantry");
            }

            if (selectedCategory != PurchaseCategory.aerial && selectedCategory != PurchaseCategory.nuclear && controller.selectedCity != null && controller.selectedCity.currentTile.occupant != null && controller.selectedCity.currentTile.city.airportTier > 0) {
                changePurchaseCategory(3);
                updateProducts();
            }
            if (selectedCategory != PurchaseCategory.aerial && selectedCategory != PurchaseCategory.nuclear && controller.selectedCity != null && controller.selectedCity.currentTile.occupant != null && controller.selectedCity.currentTile.city.airportTier <= 0 && controller.selectedCity.currentTile.city.nuclearTier > 0) {
                changePurchaseCategory(6);
                updateProducts();
            }
        }
    }
}

