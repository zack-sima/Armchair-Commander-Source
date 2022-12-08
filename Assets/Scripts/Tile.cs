using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI; 
[SerializeField]
public enum Terrain {plains, forest, mountains, city, water, oil, desert, highMountains, snow, village, industrialComplex, railroad} //plains is not present in terrain prefabs (nothing) hence prefabs are one short
public class Tile : MonoBehaviour {
    Vector3 normalPosition;
    public Sprite emptyTileSprite, emptyRectangle, emptyTrapezoid, hexTileSprite, emptyCutout, filledCutout, trapezoidSprite, rectangleSprite;
    public SpriteRenderer topTrapezoid, bottomTrapezoid, leftCutout, rightCutout, flagImage, radiationRenderer; //flag image is for water tile in map editor
    public GameObject borderPrefab;
    public Unit occupant;
    [HideInInspector] 
    public Controller controller;
    [HideInInspector]
    public List<Tile> searchedTiles, unreachableTiles;
    public bool selected, movable, canBeAttacked, isCity;
    public City city; 
    public Tile top, bottom, upperLeft, upperRight, lowerLeft, lowerRight;
    public int movementCost;
    public int radiationLeft; //if none, left at 0

    [HideInInspector]
    public Color defaultTileColor;
    public string country;
    public Terrain terrain;
    public GameObject[] terrainPrefabs;
    public Dictionary<Terrain, int> terrainCosts;

    [HideInInspector]
    public List<Tile> neighbors = new List<Tile>();

    [HideInInspector]
    public int sumCost;

    [HideInInspector] //tile has no nearby tiles active, tile is not in searchedtiles list yet 
    public bool locked, searched;

    [HideInInspector]
    public Tile deltaTile; 

    //a little different from position 
    public Vector2 coordinate;



    public void resetTilePathfinding(bool searched) {
        movable = false;
        sumCost = -1;
        locked = false;
        this.searched = searched; 
    }

    //for edit mode only
    public void updateTerrain() {
        Destroy(currentTerrainSprite);
        StartCoroutine(spawnTerrain());
    }
    public void clearSearchedTiles() {
        foreach (Tile i in searchedTiles) {
            i.hexRendererColor = i.defaultTileColor; 
 
            i.resetTilePathfinding(false); 
        }
        foreach (Tile i in unreachableTiles) {
            i.hexRendererColor = i.defaultTileColor;

            i.resetTilePathfinding(false);
        }
    }
    public List<Vector2> aiFindPath(int movementLim, Tile targetTile, bool checking, bool addTargetTile) { //addTargetTile moves ai into target tile instead of adjacent to it
        searched = true;
        locked = true;
        searchedTiles = new List<Tile>();
        unreachableTiles = new List<Tile>();
        if (occupant == null)
            return null;

        bool canLand = occupant.troopType != Troop.navy;
        foreach (Tile t in neighbors) {
            if (t != null && (canLand || t.terrain == Terrain.water) && (t.city == null || t.city.health <= 0 || t.city.tier == 0 || controller.countriesIsAxis[t.country] == occupant.isAxis) && (t.occupant == null || !controller.countriesIsNeutral.Contains(t.occupant.country) && t.occupant.isAxis == occupant.isAxis) && t.CalculateMovementCost(occupant) <= movementLim) {
                searchedTiles.Add(t);
                t.resetTilePathfinding(true);
                t.deltaTile = this;
            }
        }
        int it = 0; //iteration counter  
        while (true) {
            int leastTotalCost = 1000000;
            Tile leastTotalTile = null;
            foreach (Tile i in searchedTiles) {
                if (!i.locked) {
                    if (i.sumCost == -1) {
                        i.sumCost = i.CalculateMovementCost(occupant);
                    }
                    if (i.sumCost < leastTotalCost && i.sumCost < movementLim && ((i.terrain == Terrain.water && terrain == Terrain.water) || (i.terrain != Terrain.water && terrain != Terrain.water))) {
                        leastTotalCost = i.sumCost;
                        leastTotalTile = i;
                    }
                }
            }
            if (leastTotalTile != null) {
                Tile currentSelectedTile = null;
                leastTotalCost = 1000000;
                foreach (Tile t in leastTotalTile.neighbors) {
                    if (t != null && (canLand || t.terrain == Terrain.water) && ((t.terrain != Terrain.water && terrain != Terrain.water) || (t.terrain == Terrain.water && terrain == Terrain.water)) &&
                        (t.city == null || t.city.health <= 0 || t.city.tier == 0 || controller.countriesIsAxis[t.country] == occupant.isAxis)
                        && (t.occupant == null || !controller.countriesIsNeutral.Contains(t.occupant.country) && t.occupant.isAxis == occupant.isAxis) && t.CalculateMovementCost(occupant) < leastTotalCost && !t.searched) {
                        leastTotalCost = t.CalculateMovementCost(occupant);
                        currentSelectedTile = t;
                    }
                }

                if (currentSelectedTile == null) {
                    leastTotalTile.locked = true;
                    it++;
                    continue;
                }
                if (leastTotalTile.sumCost + currentSelectedTile.CalculateMovementCost(occupant) > movementLim) {
                    it++;
                    currentSelectedTile.searched = true;
                    unreachableTiles.Add(currentSelectedTile);
                    continue;
                }
                currentSelectedTile.deltaTile = leastTotalTile;
                currentSelectedTile.sumCost = leastTotalTile.sumCost + currentSelectedTile.CalculateMovementCost(occupant);
                currentSelectedTile.searched = true;
                try {
                    if (leastTotalTile.top.searched && leastTotalTile.bottom.searched && leastTotalTile.upperLeft.searched && leastTotalTile.upperRight.searched && leastTotalTile.lowerLeft.searched && leastTotalTile.lowerRight.searched) {
                        //tile is locked
                        leastTotalTile.locked = true;
                    }
                } catch {
                    //border
                }
                searchedTiles.Add(currentSelectedTile);
            } else {
                //pathfinding complete
                break;
            }
            if (it > 1000) {
                print("pathfinding is stucks ");
                break;
            }
            it++;
        }
        float closestDistance = 0;
        Tile closestTile = null;


        Dictionary<Tile, int> onPathTiles = null;
        if (MyPlayerPrefs.instance.GetInt("optimization") == -1)
            onPathTiles = controller.FindNavigatableDistanceBetweenTiles(this, targetTile, occupant != null && occupant.troopType == Troop.navy);
        
        //if no actual route to target use traditional distance measurement
        if (onPathTiles == null)
            closestDistance = Mathf.Infinity;

        bool noPathFound = true;
        if (onPathTiles != null) {
            foreach (Tile i in searchedTiles) {
                if (i.occupant == null) {
                    if (onPathTiles.ContainsKey(i) && onPathTiles[i] > closestDistance) {
                        closestDistance = onPathTiles[i];
                        closestTile = i;
                        noPathFound = false;
                    }
                }
            }
            if (noPathFound)
                closestDistance = Mathf.Infinity;
        }
        foreach (Tile i in searchedTiles) {
            if (i == targetTile && checking) {
                resetTilePathfinding(false);
                clearSearchedTiles();
                return new List<Vector2>(); 
            }
            if (i.occupant == null) {
                i.movable = true;
                if (onPathTiles == null || noPathFound) {
                    if (Vector2.Distance(i.transform.position, targetTile.transform.position) < closestDistance) {
                        closestDistance = Vector2.Distance(i.transform.position, targetTile.transform.position);
                        closestTile = i;
                    }
                }
            }
        }
        if (checking) {
            resetTilePathfinding(false);
            clearSearchedTiles();
            return null; 
        } else {
            List<Vector2> pathTiles = null;
            if (addTargetTile) {
                targetTile.occupant = occupant;
                occupant = null;
                resetTilePathfinding(false);
                pathTiles = findPathToDestination(targetTile);
                clearSearchedTiles();
            } else {
                if (closestTile != null) {
                    closestTile.occupant = occupant;
                    occupant = null;
                    resetTilePathfinding(false);
                    pathTiles = findPathToDestination(closestTile);
                    clearSearchedTiles();
                }
            }
            return pathTiles; 
        }
    }
    public List<Vector2> findPathToDestination(Tile tile ) {
        List<Vector2> pathTiles = new List<Vector2>();
        //if (addTile)
        //    pathTiles.Add(tile.tra); 
        Tile currentTile = tile;
        int it = 0; 
        while (currentTile != this) {
            pathTiles.Add(currentTile.transform.position);
            currentTile = currentTile.deltaTile; 
            if (it > 20) {
                print("pathfinding error");
                break; 
            }
            it++;
        }
        return pathTiles; 
    }

    public void findAvailableTiles(int movementLim) {
        searched = true;
        locked = true;
        searchedTiles = new List<Tile>();
        unreachableTiles = new List<Tile>();

        bool canLand = occupant.troopType != Troop.navy;

        foreach (Tile t in neighbors) {
            if (t != null && (canLand || t.terrain == Terrain.water) && (!t.isCity || controller.countriesIsAxis[t.country] == occupant.isAxis || t.city.health <= 0f || t.city.defenceTier == 0) && (t.occupant == null || !controller.countriesIsNeutral.Contains(t.occupant.country) && t.occupant.isAxis == occupant.isAxis) && t.CalculateMovementCost(occupant) <= movementLim) {
                searchedTiles.Add(t);
                t.resetTilePathfinding(true);
                t.deltaTile = this;
            }
        }
        int it = 0; //iteration counter  
        while (true) {
            int leastTotalCost = 1000000; 
            Tile leastTotalTile = null; 
            foreach (Tile i in searchedTiles) {
                if (!i.locked) {
                    if (i.sumCost == -1) {
                        i.sumCost = i.CalculateMovementCost(occupant);
                    }
                    if (i.sumCost < leastTotalCost && i.sumCost < movementLim && ((i.terrain == Terrain.water && terrain == Terrain.water) || (i.terrain != Terrain.water && terrain != Terrain.water))) {
                        leastTotalCost = i.sumCost;
                        leastTotalTile = i; 
                    }
                }
            }
            if (leastTotalTile != null) {
                Tile currentSelectedTile = null; 
                leastTotalCost = 1000000;
                foreach (Tile t in leastTotalTile.neighbors) {
                    if (t != null && (!t.isCity || controller.countriesIsAxis[t.country] == occupant.isAxis || t.city.health <= 0f || t.city.defenceTier == 0) && (canLand || t.terrain == Terrain.water)
                        && ((t.terrain != Terrain.water && terrain != Terrain.water) || (t.terrain == Terrain.water && terrain == Terrain.water)) &&
                        (t.occupant == null || !controller.countriesIsNeutral.Contains(t.occupant.country) && t.occupant.isAxis == occupant.isAxis) && t.CalculateMovementCost(occupant) < leastTotalCost && !t.searched) {
                        leastTotalCost = t.CalculateMovementCost(occupant);
                        currentSelectedTile = t;
                    }
                }
                
                if (currentSelectedTile == null) {
                    leastTotalTile.locked = true;
                    it++;
                    continue;
                }
                if (leastTotalTile.sumCost + currentSelectedTile.CalculateMovementCost(occupant) > movementLim) {
                    it++;
                    currentSelectedTile.searched = true;
                    unreachableTiles.Add(currentSelectedTile); 
                    continue; 
                }
                currentSelectedTile.deltaTile = leastTotalTile; 
                currentSelectedTile.sumCost = leastTotalTile.sumCost + currentSelectedTile.CalculateMovementCost(occupant);
                currentSelectedTile.searched = true;
                try {
                    if (leastTotalTile.top.searched && leastTotalTile.bottom.searched && leastTotalTile.upperLeft.searched && leastTotalTile.upperRight.searched && leastTotalTile.lowerLeft.searched && leastTotalTile.lowerRight.searched) {
                        //tile is locked
                        leastTotalTile.locked = true;
                    } 
                } catch { } 
                searchedTiles.Add(currentSelectedTile); 
            } else {
                //pathfinding complete
                break; 
            }
            if (it > 1000) { 
                print("pathfinding is stucks "); 
                break; 
            }
            it++; 
        }
        foreach (Tile i in searchedTiles) {
            if (i.occupant == null) {
                i.hexRendererColor = Color.green;

                //i.GetComponentInChildren<SpriteRenderer>().color = Color.green;
                i.movable = true;
            }
        }
    }
    void Start() {
        StartCoroutine(MyStart());
    }
    public void UpdateWater() {
        StartCoroutine(MyStart(false));
    }
    
    IEnumerator MyStart(bool useTerrain=true) {
        while (!controller.started) {
            yield return null;
        }
        terrainCosts = new Dictionary<Terrain, int>();
        terrainCosts.Add(Terrain.plains, 3);
        terrainCosts.Add(Terrain.forest, 5);
        terrainCosts.Add(Terrain.mountains, 7);
        terrainCosts.Add(Terrain.city, 3);
        terrainCosts.Add(Terrain.water, 3);
        terrainCosts.Add(Terrain.oil, 3);
        terrainCosts.Add(Terrain.desert, 3);
        terrainCosts.Add(Terrain.highMountains, 100); //should be unpassable
        terrainCosts.Add(Terrain.snow, 4);
        terrainCosts.Add(Terrain.village, 3);
        terrainCosts.Add(Terrain.industrialComplex, 3);
        terrainCosts.Add(Terrain.railroad, 1);



        if (neighbors.Count == 0) {
            if (controller.tiles.ContainsKey(new Vector2(coordinate.x, coordinate.y + 1f))) {
                top = controller.tiles[new Vector2(coordinate.x, coordinate.y + 1f)];
                neighbors.Add(top);
            }
            if (controller.tiles.ContainsKey(new Vector2(coordinate.x, coordinate.y - 1f))) {
                bottom = controller.tiles[new Vector2(coordinate.x, coordinate.y - 1f)]; neighbors.Add(bottom);
            }
            if (controller.tiles.ContainsKey(new Vector2(coordinate.x - 1f, coordinate.y + 0.5f))) {
                upperLeft = controller.tiles[new Vector2(coordinate.x - 1f, coordinate.y + 0.5f)]; neighbors.Add(upperLeft);
            }
            if (controller.tiles.ContainsKey(new Vector2(coordinate.x + 1f, coordinate.y + 0.5f))) {
                upperRight = controller.tiles[new Vector2(coordinate.x + 1f, coordinate.y + 0.5f)]; neighbors.Add(upperRight);
            }
            if (controller.tiles.ContainsKey(new Vector2(coordinate.x - 1f, coordinate.y - 0.5f))) {
                lowerLeft = controller.tiles[new Vector2(coordinate.x - 1f, coordinate.y - 0.5f)]; neighbors.Add(lowerLeft);
            }
            if (controller.tiles.ContainsKey(new Vector2(coordinate.x + 1f, coordinate.y - 0.5f))) {
                lowerRight = controller.tiles[new Vector2(coordinate.x + 1f, coordinate.y - 0.5f)]; neighbors.Add(lowerRight);
            }
        }
        if (useTerrain) {
            if (!controller.editMode || MyPlayerPrefs.instance.GetInt("custom") == 1)
                StartCoroutine(spawnTerrain());
        }
        normalPosition = transform.position;
        updateTileColor();

        StartCoroutine(delayStart());
    }
    List<GameObject> borderPrefabs; //delete water borders when editing maps if water is updated
    IEnumerator delayStart() {
        yield return null;
        if (lowerLeft == null && upperLeft == null) {
            leftCutout.enabled = true;
            GetComponentInChildren<SpriteRenderer>().enabled = false;
            if (top == null) {
                topTrapezoid.sprite = rectangleSprite;
                topTrapezoid.transform.Translate(Vector2.right * 0.12f);
                topTrapezoid.transform.Translate(Vector3.forward * 0.12f);
            }
            if (bottom == null) {
                bottomTrapezoid.sprite = rectangleSprite;
                bottomTrapezoid.transform.Translate(Vector2.left * 0.12f);
                bottomTrapezoid.transform.Translate(Vector3.forward * 0.12f);
            }
        }
        if (lowerRight == null && upperRight == null) {
            rightCutout.enabled = true;
            GetComponentInChildren<SpriteRenderer>().enabled = false;
            if (top == null) {
                topTrapezoid.sprite = rectangleSprite;
                topTrapezoid.transform.Translate(Vector2.left * 0.12f);
                topTrapezoid.transform.Translate(Vector3.forward * 0.12f);

            }
            if (bottom == null) {
                bottomTrapezoid.sprite = rectangleSprite;
                bottomTrapezoid.transform.Translate(Vector2.right * 0.12f);
                bottomTrapezoid.transform.Translate(Vector3.forward * 0.12f);
            }
        }
        if (top == null && (int)coordinate.x % 2 == 0) {
            topTrapezoid.enabled = true;
            bottomTrapezoid.enabled = false;
        } else if (bottom == null && (int)coordinate.x % 2 == 0) {
            topTrapezoid.enabled = false;
            bottomTrapezoid.enabled = true;
        }
        yield return null;
        if (borderPrefabs != null) {
            foreach (GameObject i in borderPrefabs) {
                if (i != null) {
                    Destroy(i);
                }
            }
        }
        borderPrefabs = new List<GameObject>();
        if (terrain != Terrain.water) {
            if (lowerLeft != null && lowerLeft.terrain == Terrain.water) {
                borderPrefabs.Add(Instantiate(borderPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.01f), Quaternion.Euler(0f, 0f, -60f)));
                if (bottomTrapezoid.enabled) {
                    borderPrefabs.Add(Instantiate(borderPrefab, new Vector3(transform.position.x, transform.position.y - (top.transform.position.y - transform.position.y), transform.position.z - 0.01f), Quaternion.Euler(0f, 0f, -120f)));
                }
                if (lowerLeft.topTrapezoid.enabled) {
                    borderPrefabs.Add(Instantiate(borderPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.01f), Quaternion.Euler(0f, 0f, -120)));
                }
            }
            if (lowerRight != null && lowerRight.terrain == Terrain.water) {
                borderPrefabs.Add(Instantiate(borderPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.10f), Quaternion.Euler(0f, 0f, 60f)));
                if (bottomTrapezoid.enabled) {
                    borderPrefabs.Add(Instantiate(borderPrefab, new Vector3(transform.position.x, transform.position.y - (top.transform.position.y - transform.position.y), transform.position.z - 0.01f), Quaternion.Euler(0f, 0f, 120f)));
                }
                if (lowerRight.topTrapezoid.enabled) {
                    borderPrefabs.Add(Instantiate(borderPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.01f), Quaternion.Euler(0f, 0f, 120)));
                }
            }
            if (upperLeft != null && upperLeft.terrain == Terrain.water) {
                borderPrefabs.Add(Instantiate(borderPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.01f), Quaternion.Euler(0f, 0f, -120f)));
                if (upperLeft.bottomTrapezoid.enabled) {
                    borderPrefabs.Add(Instantiate(borderPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.01f), Quaternion.Euler(0f, 0f, -60f)));
                }
                if (topTrapezoid.enabled) {
                    borderPrefabs.Add(Instantiate(borderPrefab, new Vector3(transform.position.x, transform.position.y + (transform.position.y - bottom.transform.position.y), transform.position.z - 0.01f), Quaternion.Euler(0f, 0f, -60)));
                }
            }
            if (upperRight != null && upperRight.terrain == Terrain.water) {
                borderPrefabs.Add(Instantiate(borderPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.01f), Quaternion.Euler(0f, 0f, 120f)));
                if (upperRight.bottomTrapezoid.enabled) {
                    borderPrefabs.Add(Instantiate(borderPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.01f), Quaternion.Euler(0f, 0f, 60f)));
                }
                if (topTrapezoid.enabled) {
                    borderPrefabs.Add(Instantiate(borderPrefab, new Vector3(transform.position.x, transform.position.y + (transform.position.y - bottom.transform.position.y), transform.position.z - 0.01f), Quaternion.Euler(0f, 0f, 60)));
                }
            }
            if (top != null && top.terrain == Terrain.water) {
                borderPrefabs.Add(Instantiate(borderPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.01f), Quaternion.Euler(0f, 0f, 180f)));
                if (leftCutout.enabled) {
                    borderPrefabs.Add(Instantiate(borderPrefab, new Vector3(transform.position.x - 0.2f, transform.position.y, transform.position.z - 0.01f), Quaternion.Euler(0f, 0f, 180f))); 
                }
                if (rightCutout.enabled) {
                    borderPrefabs.Add(Instantiate(borderPrefab, new Vector3(transform.position.x + 0.2f, transform.position.y, transform.position.z - 0.01f), Quaternion.Euler(0f, 0f, 180f)));
                }
            }
            if (bottom != null && bottom.terrain == Terrain.water) {
                borderPrefabs.Add(Instantiate(borderPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.01f), Quaternion.identity));
                if (rightCutout.enabled) {
                    borderPrefabs.Add(Instantiate(borderPrefab, new Vector3(transform.position.x + 0.2f, transform.position.y, transform.position.z - 0.01f), Quaternion.Euler(0f, 0f, 0f)));
                }
                if (leftCutout.enabled) {
                    borderPrefabs.Add(Instantiate(borderPrefab, new Vector3(transform.position.x - 0.2f, transform.position.y, transform.position.z - 0.01f), Quaternion.Euler(0f, 0f, 0f)));
                }
            }
        }
    }
    [HideInInspector]
    public bool spawningTerrain;
    //hold onto sprite if deletion is needed for edit mode 
    GameObject currentTerrainSprite;
    public int CalculateMovementCost (Unit troop) {
        int myCost = movementCost;

        if (terrain == Terrain.forest || terrain == Terrain.mountains) {
            //tanks, rocket artillery, self propelled gun, motorized, and mechanized get +1 movement penalty
            if (troop.troopId == 5 || troop.troopId == 6 || troop.troopId == 15 || troop.troopId == 18 || troop.troopType == Troop.armor) {
                myCost++;
            } else if (troop.troopId == 17) {
                //commandos get bonus movement
                myCost--;
            }
        }

        return myCost;

    }

    IEnumerator spawnTerrain() {
        spawningTerrain = true;
        movementCost = terrainCosts[terrain]; 
        for (double i = 0; i < 0.1; i += Time.deltaTime) {
            yield return null; 
        }
        if (terrain != Terrain.plains && terrain != Terrain.water && terrain != Terrain.city && !isCity) {
            GameObject insItem = Instantiate(terrainPrefabs[(int)terrain - 1], new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.1f), Quaternion.identity);
            currentTerrainSprite = insItem; 
        }
        spawningTerrain = false;
        updateTileColor();
    }
    public void updateTileColor() {
        if (terrain == Terrain.water) { 
            defaultTileColor = new Color(0.52f, 0.72f, 1.1f, 1f); 
        } else {
            switch (country) {
                case "Soviet":
                    defaultTileColor = new Color(0.9f, 0.67f, 0.67f, 1f);
                    break;
            case "NeutralSoviet":
                defaultTileColor = new Color(0.8f, 0.67f, 0.681f, 1f);
                break;
            case "Soviet2":
                    defaultTileColor = new Color(0.98f, 0.62f, 0.62f, 1f);
                    break;
                case "German":
                    defaultTileColor = new Color(0.61f, 0.6f, 0.62f, 1f);
                    break;
            case "Austria":
                defaultTileColor = new Color(0.75f, 0.52f, 0.62f, 1f);
                break;
            case "Czechoslovakia":
                defaultTileColor = new Color(0.52f, 0.57f, 0.92f, 1f);
                break;
            case "Slovakia":
                defaultTileColor = new Color(0.72f, 0.37f, 0.82f, 1f);
                break;
            case "Afghanistan":
                defaultTileColor = new Color(0.32f, 0.77f, 0.52f, 1f);
                break;
            case "ChineseSichuan":
                defaultTileColor = new Color(0.39f, 0.52f, 0.52f, 1f);
                break;
            case "Phillipeans":
                defaultTileColor = new Color(0.29f, 0.33f, 0.92f, 1f);
                break;
            case "WestGermany":
                defaultTileColor = new Color(0.61f, 0.6f, 0.62f, 1f);
                break;
            case "EastGermany":
                defaultTileColor = new Color(0.9125112f, 0.6f, 0.62f, 1f);
                break;
            case "German2":
                    defaultTileColor = new Color(0.52f, 0.53f, 0.52f, 1f);
                    break;
                case "RepublicanSpain":
                    defaultTileColor = new Color(0.97f, 0.6f, 0.67f, 1f);
                    break;
                case "FascistSpain":
                    defaultTileColor = new Color(0.52f, 0.53f, 0.621f, 1f);
                    break;
            case "NeutralSpain":
                defaultTileColor = new Color(0.52f, 0.53f, 0.621f, 1f);
                break;
            case "Switzerland":
                defaultTileColor = new Color(0.75f, 0.92f, 0.92f, 1f);
                break;
            case "Ireland":
                defaultTileColor = new Color(0.65f, 0.82f, 0.83f, 1f);
                break;
            case "Iran":
                defaultTileColor = new Color(0.65f, 0.82f, 0.87f, 1f);
                break;
            case "Iraq":
                defaultTileColor = new Color(0.65f, 0.92f, 0.51f, 1f);
                break;
            case "CommieIraq":
                defaultTileColor = new Color(0.601f, 0.82f, 0.81f, 1f);
                break;
            case "Sweden":
                defaultTileColor = new Color(0.66f, 0.817f, 0.86f, 1f);
                break;
            case "Portugal":
                defaultTileColor = new Color(0.65f, 0.8f, 0.83f, 1f);
                break;
            case "Latvia":
                defaultTileColor = new Color(0.68f, 0.85f, 0.83f, 1f);
                break;
            case "Lithuania":
                defaultTileColor = new Color(0.67f, 0.7f, 0.83f, 1f);
                break;
            case "Estonia":
                defaultTileColor = new Color(0.62f, 0.76f, 0.79f, 1f);
                break;
            case "Poland":
                    defaultTileColor = new Color(0.3f, 0.2f, 0.2f, 1f);
                    break;
            case "CommiePoland":
                defaultTileColor = new Color(0.5f, 0.2f, 0.2f, 1f);
                break;
            case "HomeArmyPoland":
                defaultTileColor = new Color(0.3f, 0.26f, 0.23f, 1f);
                break;
            case "NorthKorea":
                defaultTileColor = new Color(0.3f, 0.27f, 0.2f, 1f);
                break;
            case "SouthKorea":
                defaultTileColor = new Color(0.7f, 0.8f, 0.92f, 1f);
                break;
            case "Turkey":
                defaultTileColor = new Color(0.3f, 0.2f, 0.27f, 1f);
                break;
            case "SaudiArabia":
                defaultTileColor = new Color(0.3f, 0.21f, 0.97f, 1f);
                break;
            case "India":
                defaultTileColor = new Color(0.3f, 0.21f, 0.35f, 1f);
                break;
            case "NatoIndia":
                defaultTileColor = new Color(0.3f, 0.27f, 0.35f, 1f);
                break;
            case "Pakistan":
                defaultTileColor = new Color(0.3f, 0.21f, 0.79f, 1f);
                break;
            case "Egypt":
                defaultTileColor = new Color(0.25f, 0.9f, 0.79f, 1f);
                break;
            case "Liberia":
                defaultTileColor = new Color(0.27f, 0.15f, 0.29f, 1f);
                break;
            case "Ethiopia":
                defaultTileColor = new Color(0.35f, 0.9f, 0.95f, 1f);
                break;
            case "Albania":
                defaultTileColor = new Color(0.35f, 0.35f, 0.62f, 1f);
                break;
            case "SouthAfrica":
                defaultTileColor = new Color(0.35f, 0.95f, 0.62f, 1f);
                break;
            case "NatoTurkey":
                defaultTileColor = new Color(0.25f, 0.2700000f, 0.225f, 1f);
                break;
            case "Vietnam":
                defaultTileColor = new Color(0.25f, 0.298f, 0.21f, 1f);
                break;
            case "Finland":
                    defaultTileColor = new Color(0.7f, 0.86f, 0.85f, 1f);
                    break;
            case "SouthVietnam":
                defaultTileColor = new Color(0.2f, 0.35f, 0.85f, 1f);
                break;
            case "Brazil":
                defaultTileColor = new Color(0.2f, 0.76f, 0.15f, 1f);
                break;
            case "Argentina":
                defaultTileColor = new Color(0.1f, 0.76f, 0.8f, 1f);
                break;
            case "Malaysia":
                defaultTileColor = new Color(0.2f, 0.36f, 0.57f, 1f);
                break;
            case "CommieGreece":
                defaultTileColor = new Color(0.9f, 0.15f, 0.25f, 1f);
                break;
            case "Singapore":
                defaultTileColor = new Color(0.89f, 0.75f, 0.832f, 1f);
                break;
            case "Chile":
                defaultTileColor = new Color(0.59f, 0.75f, 0.832f, 1f);
                break;
            case "Colombia":
                defaultTileColor = new Color(0.59f, 0.65f, 0.732f, 1f);
                break;
            case "Indonesia":
                defaultTileColor = new Color(0.39f, 0.55f, 0.132f, 1f);
                break;
            case "Mexico":
                defaultTileColor = new Color(0.2f, 0.25f, 0.37f, 1f);
                break;
            case "Cuba":
                defaultTileColor = new Color(0.7f, 0.2f, 0.17f, 1f);
                break;
            case "Canada":
                defaultTileColor = new Color(0.789f, 0.6720201f, 0.85f, 1f);
                break;
            case "Greece":
                    defaultTileColor = new Color(0.67f, 0.79f, 0.9f, 1f);
                    break;
            case "Australia":
                defaultTileColor = new Color(0.37f, 0.59f, 0.98f, 1f);
                break;
            case "NewZealand":
                defaultTileColor = new Color(0.67f, 0.72f, 0.98f, 1f);
                break;
            case "Bulgaria":
                    defaultTileColor = new Color(0.1f, 0.76f, 0.1f, 1f);
                    break;
            case "Romania":
                defaultTileColor = new Color(0.8f, 0.6f, 0.17f, 1f);
                break;
            case "Hungary":
                defaultTileColor = new Color(0.71f, 0.71f, 0.35f, 1f);
                break;
            case "Yugoslavia":
                defaultTileColor = new Color(0.8f, 0.72f, 0.7f, 1f);
                break;
            case "Serbia":
                defaultTileColor = new Color(0.99f, 0.72f, 0.7f, 1f);
                break;
            case "Croatia":
                defaultTileColor = new Color(0.75f, 0.62f, 0.52f, 1f);
                break;
            case "Belgium":
                defaultTileColor = new Color(0.25f, 0.35f, 0.85f, 1f);
                break;
            case "UK":
                defaultTileColor = new Color(0.1f, 0.72f, 0.78f);
                break;
            case "NatoUK":
                defaultTileColor = new Color(0.17f, 0.82f, 0.78f);
                break;
            case "NeutralUK":
                defaultTileColor = new Color(0.1f, 0.78f, 0.61f);
                break;
            case "Italy":
                defaultTileColor = new Color(0.8f, 0.2f, 0.7f);
                break;
            case "Denmark":
                defaultTileColor = new Color(0.86f, 0.2f, 0.85f);
                break;
            case "Luxembourg":
                defaultTileColor = new Color(0.82f, 0.25f, 0.7f);
                break;
            case "Netherlands":
                defaultTileColor = new Color(0.89f, 0.351f, 0.617002f);
                break;
            case "Norway":
                defaultTileColor = new Color(0.82f, 0.25f, 0.9f);
                break;
            case "ROC":
                defaultTileColor = new Color(0.67f, 0.79f, 0.97f, 1f);
                break;
            case "NatoROC":
                defaultTileColor = new Color(0.57f, 0.79f, 0.97f, 1f);
                break;
            case "ChineseGuangxi":
                defaultTileColor = new Color(0.25f, 0.72f, 0.52f, 1f);
                break;
            case "ChineseShanxi":
                defaultTileColor = new Color(0.67f, 0.52f, 0.920f, 1f);
                break;
            case "ChineseSinkiang":
                defaultTileColor = new Color(0.67f, 0.79f, 0.625f, 1f);
                break;
            case "ChineseTibet":
                defaultTileColor = new Color(0.73f, 0.67f, 0.97f, 1f);
                break;
            case "ChineseXibeiSanma":
                defaultTileColor = new Color(0.57f, 0.89f, 0.97f, 1f);
                break;
            case "ChineseYunnan":
                defaultTileColor = new Color(0.87f, 0.67f, 0.97f, 1f);
                break;
            case "Mongolia":
                defaultTileColor = new Color(0.87f, 0.59f, 0.97f, 1f);
                break;
            case "USA":
                defaultTileColor = new Color(0.1f, 0.72f, 0.97f, 1f);
                break;
            case "NatoUSA":
                defaultTileColor = new Color(0.17f, 0.82f, 0.9f, 1f);
                break;
            case "France":
                defaultTileColor = new Color(0.67f, 0.572f, 0.97f, 1f);
                break;
            case "NatoFrance":
                defaultTileColor = new Color(0.67f, 0.572f, 0.912f, 1f);
                break;
            case "Israel":
                defaultTileColor = new Color(0.602f, 0.572f, 0.912f, 1f);
                break;
            case "NeutralFrance":
                defaultTileColor = new Color(0.63f, 0.578f, 0.95f, 1f);
                break;
            case "Thailand":
                defaultTileColor = new Color(0.3f, 0.39f, 0.52f, 1f);
                break;
            case "NeutralThailand":
                defaultTileColor = new Color(0.325f, 0.39f, 0.52f, 1f);
                break;
            case "Japan2":
                defaultTileColor = new Color(0.35f, 0.81f, 0.25f, 1f);
                break;
            case "Manchukuo":
                defaultTileColor = new Color(0.25f, 0.9f, 0.9f, 1f);
                break;
            case "Mengkukuo":
                defaultTileColor = new Color(0.15f, 0.85f, 0.7f, 1f);
                break;
            case "PRC":
                defaultTileColor = new Color(0.98f, 0.67f, 0.63f, 1f);
                break;
            case "PRCNew":
                defaultTileColor = new Color(1f, 0.52f, 0.63f, 1f);
                break;
            case "PuppetROC":
                defaultTileColor = new Color(0.67f, 0.701f, 0.93f, 1f);
                break;
            case "Japan":
                defaultTileColor = new Color(0.3f, 0.8f, 0.35f, 1f);
                break;
            default:
                defaultTileColor = Color.white;
                break;
            }
        }
        hexRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        hexRendererColor = defaultTileColor;
    }

    bool deltaIsBuildTile = false;
    bool deltaWaterTile = false; //if changed in editor update tile to match visual
    SpriteRenderer hexRenderer;
    [HideInInspector]
    public Color hexRendererColor = Color.white; //this color is set to hexrenderer but adds flashing for grey (movable troops)

    //only access this update cycle every few other frames if using optimized mode
    float updateIntervalOptimized = 0f;

    void Update() {
        if (MyPlayerPrefs.instance.GetInt("optimization") == 1 && controller.started) {
            if (updateIntervalOptimized >= 0f) {
            updateIntervalOptimized++;

            } else {
                updateIntervalOptimized += Time.deltaTime;
            }
            if (updateIntervalOptimized >= 2) {
                updateIntervalOptimized = 0f;
            } else if (updateIntervalOptimized >= 0f) {
                return;
            }
        }

        if (radiationLeft > 0) {
            //if (terrain == Terrain.water) {
            //    radiationLeft = 0;
            //} else
            if (!radiationRenderer.enabled)
                radiationRenderer.enabled = true;
        } else if (radiationRenderer.enabled && radiationLeft <= 0) {
            radiationRenderer.enabled = false;
        }
        if (topTrapezoid.enabled && topTrapezoid.color != defaultTileColor)
            topTrapezoid.color = defaultTileColor;
        if (bottomTrapezoid.enabled && bottomTrapezoid.color != defaultTileColor)
            bottomTrapezoid.color = defaultTileColor;
        if (leftCutout.enabled && leftCutout.color != transform.GetChild(0).GetComponent<SpriteRenderer>().color)
            leftCutout.color = transform.GetChild(0).GetComponent<SpriteRenderer>().color;
        if (rightCutout.enabled && rightCutout.color != transform.GetChild(0).GetComponent<SpriteRenderer>().color)
            rightCutout.color = transform.GetChild(0).GetComponent<SpriteRenderer>().color;

        if (controller.editMode) {
            if (terrain == Terrain.water) {
                if (hexRenderer.sprite != emptyTileSprite)
                    hexRenderer.sprite = emptyTileSprite;
            } else {
                if (hexRenderer.sprite != hexTileSprite)
                    hexRenderer.sprite = hexTileSprite;
            }
            hexRendererColor = defaultTileColor;
            if (!isCity && occupant == null && controller.showFlagToggle.isOn) {
                if (flagImage.sprite != controller.flags[country]) {
                    flagImage.sprite = controller.flags[country];
                }
                if (!flagImage.enabled) {
                    flagImage.enabled = true;
                }
            } else if (flagImage.enabled) {
                flagImage.enabled = false;
            }
            if (deltaWaterTile != (terrain == Terrain.water)) {
//                print("updated waterways");
                UpdateWater();
                foreach (Tile t in neighbors) {
                    if (t != null) {
                        t.UpdateWater();
                    }
                }
            }
            deltaWaterTile = terrain == Terrain.water;
        } else {
            if (isCity && controller.selectedCity == city && (controller.canBuild || controller.canSupply) || occupant != null && occupant.selected || occupant != null && occupant.enemySelected) {
                hexRendererColor = Color.yellow;
            } else if (occupant != null && (!occupant.moved || occupant.canAttack) && !controller.passingRound && occupant.country == controller.playerCountry && !selected) {
                hexRendererColor = new Color(0.8f, 0.8f, 0.8f);
            } else if (hexRendererColor == new Color(0.8f, 0.8f, 0.8f) || (hexRendererColor == Color.yellow && isCity && (occupant == null || occupant.moving || (!occupant.canAttack && occupant.moved && occupant.country == controller.playerCountry && ((occupant.moved && !occupant.canAttack) || occupant.attacked)))))
                hexRendererColor = defaultTileColor;

            if (controller.selectedBuildTile == this) {
                hexRendererColor = Color.yellow;
            } else if (deltaIsBuildTile && hexRendererColor == Color.yellow)
                hexRendererColor = defaultTileColor;
            
            if (occupant != null && !occupant.selected && (occupant.moving || ((!controller.canBuild && !controller.canSupply || !isCity || !controller.selectedCity == city) && (!occupant.canAttack && occupant.moved && occupant.country == controller.playerCountry && ((occupant.moved && !occupant.canAttack) || occupant.attacked)))))
                hexRendererColor = defaultTileColor;
            if (canBeAttacked)
                hexRendererColor = new Color(1f, 0.3f, 0.3f);
            if (occupant != null) {
                occupant.currentTile = this;
            }
        }
        if (hexRenderer.color != hexRendererColor)
            hexRenderer.color = hexRendererColor;
        //NOTE: NO COUNTRY'S COLOR SHOULD BE THIS VALUE
        if (hexRendererColor == new Color(0.8f, 0.8f, 0.8f)) {
            hexRenderer.color = Controller.tileColorInterpolation * Color.white * 0.95f; //Color.Lerp(defaultTileColor, new Color(0.8f, 0.8f, 0.8f), Controller.tileColorInterpolation);
            hexRenderer.color = new Color(hexRenderer.color.r, hexRenderer.color.g, hexRenderer.color.b, 1f);
        }
        if (terrain == Terrain.water && !controller.passingRound) {
            if (upperRight == null || upperLeft == null || lowerRight == null || lowerLeft == null) {
                topTrapezoid.sprite = emptyRectangle;
                bottomTrapezoid.sprite = emptyRectangle;
            } else {
                topTrapezoid.sprite = emptyTrapezoid;
                bottomTrapezoid.sprite = emptyTrapezoid;
            }
            if (!controller.editMode) {
                if ((occupant == null || !occupant.moving) && (hexRendererColor == Color.yellow || (selected && (occupant == null || occupant.general == "" || !occupant.moved)) || canBeAttacked || (movable && !controller.passingRound) || (occupant != null && occupant.country == controller.playerCountry && (!occupant.moved || (occupant.canAttack && !occupant.attacked))))) {
                    rightCutout.sprite = filledCutout;
                    leftCutout.sprite = filledCutout;
                    hexRenderer.sprite = hexTileSprite;
                    transform.position = normalPosition;
                } else {
                    rightCutout.sprite = emptyCutout;
                    leftCutout.sprite = emptyCutout;
                    hexRenderer.sprite = emptyTileSprite;
                    transform.position = new Vector3(normalPosition.x, normalPosition.y, normalPosition.z + 0.1f);
                }
            }

        } else if (terrain == Terrain.water && (controller.passingRound)) {
            rightCutout.sprite = emptyCutout;
            leftCutout.sprite = emptyCutout;
            hexRenderer.sprite = emptyTileSprite;
            transform.position = new Vector3(normalPosition.x, normalPosition.y, normalPosition.z + 0.1f);
        } else if (terrain != Terrain.water && controller.editMode) {
            hexRenderer.sprite = hexTileSprite;
        }
        deltaIsBuildTile = controller.selectedBuildTile == this;
    }
}