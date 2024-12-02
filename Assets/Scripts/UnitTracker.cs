using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnitTracker : Definitions
{
    public List<Unit> unitList = new List<Unit>();
    public List<Unit> availableUnits = new List<Unit>();

    public Unit activeUnit = null;
    private CustomGrid grid;
    private Cursor cursor;
    private AIControl ai;
    public int humanPlayerID;
    [SerializeField] private int unitLimit;
    public bool canChooseUnits;
    private bool delaySucceeded;
    private bool gameStarted;
    [SerializeField] private string sceneToLoad;

    void Awake()
    {
        ai = GameObject.Find("AIController").GetComponent<AIControl>();
        cursor = GameObject.Find("Cursor").GetComponent<Cursor>();
        grid = GameObject.FindWithTag("Grid").GetComponent<CustomGrid>();
        //if (Settings.TurnDelay == 0f) Settings.TurnDelay = 0.5f;
    }

    IEnumerator Start()
    {
        gameStarted = false;
        //Settings.AutopilotMode = false;
        if (this.gameObject.tag == "Tutorial")
        {
            //Settings.TurnDelay = 0.5f;
            //cursor.UpdateSpeedIcons();
            Settings.SandboxMode = false;
            Settings.AutopilotMode = false;
            GameObject.Find("auto").SetActive(false);
            GameObject.Find("sandbox").GetComponent<SpriteRenderer>().enabled = false;
            GameObject.Find("sandbox").GetComponent<BoxCollider2D>().enabled = false;
        }
        if (canChooseUnits) 
        {
            //GameObject.Find("DefaultPlayerUnits").SetActive(false);
            yield return null;
            if (this.gameObject.tag != "Tutorial" && Random.Range(0, 2) == 0) grid.SwitchTeamPositions();
            StartCoroutine(ChangeOutcropTransparency());
            yield return StartCoroutine(DeployUnits()); 
        }
        else
        {
            GameObject.Find("UnitSelectButtons").SetActive(false);
            /*if (this.gameObject.tag == "Tutorial")
            {
                Settings.TurnDelay = 0.8f;
                cursor.UpdateSpeedIcons();
                GameObject.Find("auto").SetActive(false);
                GameObject.Find("sandbox").SetActive(false);
            }*/
            yield return null;
            StartCoroutine(ChangeOutcropTransparency());
        }
        yield return StartCoroutine(TurnControl());
    }

    IEnumerator ChangeOutcropTransparency()
    {
        yield return null;
        while (true)
        {
            grid.ColorAllLargeSprites(Color.white);
            grid.UnblockUnits(new Color(1f, 1f, 1f, 0.6f));
            if (cursor.GetActiveTile() != null)
            {
                GridTile activeTile = cursor.GetActiveTile();
                if (activeTile.GetTileBelow(activeTile) != null && activeTile.GetTileBelow(activeTile).IsSolid())// && !activeTile.IsSolid())
                {
                    activeTile.GetTileBelow(activeTile).largeSprite.color = new Color(1f, 1f, 1f, 0.4f);
                }             
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public List<Unit> GetAllEnemiesOfID(int id)
    {
        List<Unit> enemies = new List<Unit>();
        foreach (Unit unit in unitList)
        {
            if (unit.playerID != id && unit.canMove) enemies.Add(unit);
        }
        return enemies;
    }

    public List<Unit> GetAllAlliesOfID(int id)
    {
        List<Unit> allies = new List<Unit>();
        foreach (Unit unit in unitList)
        {
            if (unit.playerID == id) allies.Add(unit);
        }
        return allies;
    }

    public void AddUnit(Unit newUnit)
    {
        unitList.Add(newUnit);
    }

    public void RemoveUnit(Unit removedUnit)
    {
        removedUnit.currentTile.occupyingUnit = null;
        if (unitList.Contains(removedUnit)) unitList.Remove(removedUnit);
        if (availableUnits.Contains(removedUnit)) availableUnits.Remove(removedUnit);
        ai.RemoveAIUnit(removedUnit);

        //check to see if a side won
        if (gameStarted)
        {
            List<int> survivingIDs = new List<int>();
            foreach (Unit unit in unitList)
            {
                if (!survivingIDs.Contains(unit.playerID) && unit.canMove) survivingIDs.Add(unit.playerID);
            }
            if (survivingIDs.Count == 1)
            {
                //Debug.Log("player " + survivingIDs[0] + " won");
                unitList.Clear();
                availableUnits.Clear();
            }
        }
    }

    public IEnumerator ChangeEnemyUnits()
    {
        while (!gameStarted && Settings.SandboxMode)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.S))
            {
                if (cursor.GetActiveTile() != null && cursor.GetActiveTile().occupyingUnit != null && cursor.GetActiveTile().occupyingUnit.playerID != humanPlayerID)
                {
                    int id = cursor.GetActiveTile().occupyingUnit.playerID;
                    GameObject unitToReplace = cursor.GetActiveTile().occupyingUnit.gameObject;
                    RemoveUnit(unitToReplace.GetComponent<Unit>());
                    Destroy(unitToReplace.gameObject);
                    Unit spawnedUnit = Instantiate(cursor.selectedUnit, cursor.GetActiveTile().transform.position, transform.rotation).GetComponent<Unit>();
                    spawnedUnit.playerID = id;
                    spawnedUnit.transform.parent = this.transform;
                }
            }
            yield return null;
        }
    }

    public void AssignAllAlphas()
    {
        bool hasNonAlphaUnit = false;
        List<Unit> eligibleUnits = new List<Unit>();

        foreach (Unit unit in unitList)
        {
            if (unit.canMove) eligibleUnits.Add(unit);
        }

        foreach (Unit unit in eligibleUnits)
        {
            if (!unit.isAlpha) hasNonAlphaUnit = true;
        }
        foreach (Unit unit in eligibleUnits)
        {
            unit.SetAlphaStatus(hasNonAlphaUnit ? true : false);
        }
    }

    IEnumerator DeployUnits()
    {
        int unitsDeployed = 0;
        if (Settings.SandboxMode) StartCoroutine(ChangeEnemyUnits());
        yield return null;   
        grid.HighlightAllStartingTiles();
        GameObject startButton = GameObject.Find("start");
        GameObject visiblePreview = Instantiate(cursor.unitPreview, transform.position, transform.rotation);
        while (!(cursor.FindButtonName() == "start" && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.S))))// && !(Settings.AutopilotMode && unitsDeployed == unitLimit))
        {
            if (cursor.GetActiveTile() != null && cursor.GetActiveTile().isStartingTile && unitsDeployed < unitLimit && cursor.GetComponent<SpriteRenderer>().sprite == null)
            {
                visiblePreview.GetComponent<SpriteRenderer>().enabled = true;
                visiblePreview.GetComponent<SpriteRenderer>().sprite = cursor.selectedUnit.GetComponent<SpriteRenderer>().sprite;
                visiblePreview.transform.position = cursor.GetActiveTile().transform.position;
            }
            else
            {
                visiblePreview.GetComponent<SpriteRenderer>().enabled = false;
            }
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.S))
            {
                if (cursor.GetActiveTile() != null && cursor.GetActiveTile().occupyingUnit == null && cursor.GetActiveTile().isStartingTile && unitsDeployed < unitLimit)
                {
                    Unit spawnedUnit = Instantiate(cursor.selectedUnit, cursor.GetActiveTile().transform.position, transform.rotation).GetComponent<Unit>();
                    spawnedUnit.playerID = humanPlayerID;
                    spawnedUnit.transform.parent = this.transform;
                    unitsDeployed++;
                    if (unitsDeployed == unitLimit)
                    {
                        startButton.GetComponent<BoxCollider2D>().enabled = true;
                        startButton.GetComponent<SpriteRenderer>().enabled = true;
                    }
                }
                else if (cursor.GetActiveTile() != null && cursor.GetActiveTile().occupyingUnit != null && cursor.GetActiveTile().isStartingTile)
                {
                    GameObject unitToReplace = cursor.GetActiveTile().occupyingUnit.gameObject;
                    RemoveUnit(unitToReplace.GetComponent<Unit>());
                    Destroy(unitToReplace.gameObject);
                    Unit spawnedUnit = Instantiate(cursor.selectedUnit, cursor.GetActiveTile().transform.position, transform.rotation).GetComponent<Unit>();
                    spawnedUnit.playerID = humanPlayerID;
                    spawnedUnit.transform.parent = this.transform;
                }
                else
                {
                    cursor.ChangeDeployedUnit();
                    if (cursor.FindButtonName() == "random")
                    {
                        startButton.GetComponent<BoxCollider2D>().enabled = false;
                        startButton.GetComponent<SpriteRenderer>().enabled = false;
                        List<GameObject> unitsToDeploy = new List<GameObject>();
                        for (int i = 0; i < 2; i++)
                        {
                            foreach (GameObject unit in cursor.unitTypes)
                            {
                                unitsToDeploy.Add(unit);
                            }
                        }
                        List<GridTile> tilesToDeployOn = new List<GridTile>();
                        foreach (GridTile tile in grid.GetPlayerSpawnPoints())
                        {
                            if (tile.occupyingUnit != null)
                            {
                                Unit unitToRemove = tile.occupyingUnit;
                                RemoveUnit(unitToRemove);
                                Destroy(unitToRemove.gameObject);
                            }
                            tilesToDeployOn.Add(tile);
                        }
                        unitsDeployed = 0;
                        while (unitsDeployed < unitLimit)
                        {
                            int unitIndex = Random.Range(0, unitsToDeploy.Count);
                            int tileIndex = Random.Range(0, tilesToDeployOn.Count);
                            Unit spawnedUnit = Instantiate(unitsToDeploy[unitIndex], tilesToDeployOn[tileIndex].transform.position, transform.rotation).GetComponent<Unit>();
                            spawnedUnit.playerID = humanPlayerID;
                            spawnedUnit.transform.parent = this.gameObject.transform;
                            unitsToDeploy.RemoveAt(unitIndex);
                            tilesToDeployOn.RemoveAt(tileIndex);
                            unitsDeployed++;
                            yield return new WaitForSeconds(0.1f);
                        }
                        startButton.GetComponent<BoxCollider2D>().enabled = true;
                        startButton.GetComponent<SpriteRenderer>().enabled = true;
                    }
                }
                 
            }
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                if (cursor.GetActiveTile() != null && cursor.GetActiveTile().occupyingUnit != null && cursor.GetActiveTile().isStartingTile)
                {
                    GameObject unitToReplace = cursor.GetActiveTile().occupyingUnit.gameObject;
                    RemoveUnit(unitToReplace.GetComponent<Unit>());
                    Destroy(unitToReplace.gameObject);
                    unitsDeployed--;
                    if (unitsDeployed < unitLimit)
                    {
                        startButton.GetComponent<BoxCollider2D>().enabled = false;
                        startButton.GetComponent<SpriteRenderer>().enabled = false;
                    }
                }
            }
            yield return null;
        }
        yield return null;

        int playerAlphaCount = 0;
        foreach (Unit unit in GetAllAlliesOfID(humanPlayerID)) 
        {
            if (unit.isAlpha) playerAlphaCount++;
        }
        if (playerAlphaCount > 0)
        {
            for (int i = 0; i < 4; i++)
            {
                if (i != humanPlayerID && GetAllAlliesOfID(i).Count > 0)
                {
                    int enemyAlphaCount = 0;
                    List<Unit> enemyTeam = GetAllAlliesOfID(i);
                    while (enemyAlphaCount < playerAlphaCount)
                    {
                        int index = Random.Range(0, enemyTeam.Count);
                        enemyTeam[index].SetAlphaStatus(true);
                        enemyTeam.RemoveAt(index);
                        enemyAlphaCount++;
                    }
                }
            }
        }


        if (Settings.SandboxMode)
        {
            //GameObject.Find("auto").SetActive(false);
            GameObject.Find("sandbox").GetComponent<BoxCollider2D>().enabled = false;
            //Settings.AutopilotMode = false;
        }
        else
        {
            GameObject.Find("sandbox").SetActive(false);
        }
        startButton.SetActive(false);
        visiblePreview.SetActive(false);
        try { GameObject.Find("alpha").SetActive(false); } catch { }
        GameObject.Find("UnitSelectButtons").SetActive(false);
        grid.ResetAllOutliners();
    }

    IEnumerator TurnControl()
    {
        gameStarted = true;
        yield return null;
        foreach (Unit unit in unitList)
        {
            if (unit.canMove) unit.ChangeColorTo(new Color(0.75f, 0.75f, 0.75f, 1f));
        }
        while (unitList.Count > 0) //end game if all units die
        {
            foreach (Unit unit in unitList)
            {              
                if (unit.currentTurnCooldown == 0 && unit.canMove)
                {
                    availableUnits.Add(unit);
                    unit.ChangeColorTo(Color.white);
                }               
            }

            if (availableUnits.Count > 0)
            {
                yield return StartCoroutine(TakeTurn(availableUnits));
            }
            else
            {
                foreach (Unit unit in unitList)
                {
                    unit.currentTurnCooldown--;
                    unit.UpdateIcons();
                }
                grid.DecayAllSandPiles();
            }
            yield return new WaitForSeconds(0.06f * Settings.TurnDelay);
            //yield return new WaitForSeconds(0.03f);

        }
        //Debug.Log("match concluded");
        yield return new WaitForSeconds(1f);
        if (this.gameObject.tag == "Tutorial")
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            SceneManager.LoadScene("Start_Menu");
        }

    }

    IEnumerator TakeTurn(List<Unit> availableUnits)
    {
        int id = availableUnits[0].playerID;
        foreach (Unit unit in availableUnits)
        {
            unit.StartTurn();
        }
        if ((id == humanPlayerID && !Settings.AutopilotMode) || Settings.SandboxMode)
        {
            while (availableUnits.Count > 0)
            {
                GridTile startingTile = null;
                int unitIndex = 0;

                while (activeUnit == null) //choose an active unit from those available
                {
                    if (Settings.AutopilotMode)
                    {
                        yield return ai.StartCoroutine("TakeAITurn", availableUnits);
                        if (availableUnits.Count <= 0) break;
                    }
                    foreach (Unit unit in availableUnits)
                    {
                        if (!Settings.AutopilotMode) unit.EnableOutline(true);
                    }
                    try
                    {
                        if (cursor.GetActiveTile().occupyingUnit != null)
                        {
                            if (!Input.GetKey(KeyCode.LeftShift)) grid.ResetAllOutliners();
                            cursor.GetActiveTile().GetTilesInRange(cursor.GetActiveTile().occupyingUnit, new Color(1f, 0.85f, 0.15f, 0.7f));
                        }
                        else
                        {
                            if (!Input.GetKey(KeyCode.LeftShift)) grid.ResetAllOutliners();
                        }
                    }
                    catch { }
                    if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.A))
                    {
                        grid.ResetAllOutliners();
                        activeUnit = availableUnits[0];
                        //activeUnit.StartTurn();
                        activeUnit.currentTile.GetTilesInRange(activeUnit, new Color(1f, 0.85f, 0.15f, 1f));
                        activeUnit.currentTile.SetOutlinerActive(true, new Color(0.1f, 0.2f, 0.7f, 1f));
                        startingTile = activeUnit.currentTile;
                    }
                    else if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.S))
                    {
                        GridTile selectedTile = cursor.GetActiveTile();
                        try
                        {
                            if (selectedTile.occupyingUnit != null && availableUnits.Contains(selectedTile.occupyingUnit))
                            {
                                activeUnit = selectedTile.occupyingUnit;
                                //activeUnit.StartTurn();
                                selectedTile.GetTilesInRange(selectedTile.occupyingUnit, new Color(1f, 0.85f, 0.15f, 1f));
                                selectedTile.SetOutlinerActive(true, new Color(0.1f, 0.2f, 0.7f, 1f));
                                startingTile = selectedTile;
                            }
                        }
                        catch
                        {
                            //Debug.Log("selected nothing");
                        }
                    }
                    yield return null;
                }
                
                while (activeUnit != null) //perform actions as activeUnit, set to null when done
                {
                    activeUnit.EnableOutline(false);
                    if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.S)) //try to choose a square to move to
                    {
                        GridTile selectedTile = cursor.GetActiveTile();
                        if (startingTile.GetTilesInRange(activeUnit, new Color(1f, 0.85f, 0.15f, 1f)).Contains(selectedTile))
                        {
                            if (selectedTile.occupyingUnit == null || selectedTile.occupyingUnit == activeUnit)
                            {
                                yield return activeUnit.StartCoroutine("FollowPath", selectedTile.path);
                                if (selectedTile != startingTile) 
                                {
                                    activeUnit.currentTurnCooldown += activeUnit.baseTurnCooldown; 
                                }

                                activeUnit.UpdateIcons();
                                grid.ResetAllOutliners();
                                yield return StartCoroutine(UnitAction(activeUnit, startingTile));
                                try
                                {
                                    activeUnit.UpdateIcons();
                                }
                                catch { }
                                if (!availableUnits.Contains(activeUnit))
                                {
                                    activeUnit = null;
                                }
                                else
                                {
                                    startingTile.GetTilesInRange(activeUnit, new Color(1f, 0.85f, 0.15f, 1f));
                                    startingTile.SetOutlinerActive(true, new Color(0.1f, 0.2f, 0.7f, 1f));
                                }
                            }
                            else
                            {
                                startingTile.SetOutlinerActive(true, new Color(0.1f, 0.2f, 0.7f, 1f));
                            }
                        }
                        else
                        {
                            startingTile.SetOutlinerActive(true, new Color(0.1f, 0.2f, 0.7f, 1f));
                        }
                    }
                    else if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape)) //cancel action
                    {
                        activeUnit.EnableOutline(true);
                        activeUnit = null;
                        grid.ResetAllOutliners();
                    }
                    else if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.A))
                    {
                        grid.ResetAllOutliners();
                        unitIndex++;
                        if (unitIndex >= availableUnits.Count) unitIndex = 0;
                        activeUnit = availableUnits[unitIndex];
                        //activeUnit.StartTurn();
                        activeUnit.currentTile.GetTilesInRange(activeUnit, new Color(1f, 0.85f, 0.15f, 1f));
                        activeUnit.currentTile.SetOutlinerActive(true, new Color(0.1f, 0.2f, 0.7f, 1f));
                        startingTile = activeUnit.currentTile;
                        
                    }
                    else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.D))
                    {
                        grid.ResetAllOutliners();
                        if (activeUnit.currentMoveRange == 0)// && activeUnit.currentTile.tileType == TileType.trench && activeUnit.movementType == MoveType.benthic)
                        {
                            activeUnit.ResetLoweredMovement();
                            activeUnit.DelayTurnBy(activeUnit.baseTurnCooldown);
                        }
                        else
                        {
                            //activeUnit.ResetLoweredMovement();
                            activeUnit.DelayTurnBy(5);
                        }

                        availableUnits.Remove(activeUnit);
                        activeUnit.UpdateIcons();
                        activeUnit.ChangeColorTo(Color.gray);
                        activeUnit = null;

                    }
                    else if (Input.GetKeyDown(KeyCode.Space)) //choose ally to delay turn to match
                    {
                        grid.ResetAllOutliners();
                        List<Unit> alliedUnits = new List<Unit>();
                        foreach (Unit unit in unitList)
                        {
                            if (unit.playerID == activeUnit.playerID && unit != activeUnit && unit.currentTurnCooldown > activeUnit.currentTurnCooldown) //filter by same playerID
                            {
                                alliedUnits.Add(unit);
                                unit.currentTile.SetOutlinerActive(true, new Color(0.1f, 0.2f, 0.7f, 1f));
                            }
                        }
                        if (alliedUnits.Count > 0) yield return StartCoroutine(ChooseDelayTarget(alliedUnits)); //either successfully chose delay target, or cancelled with RMB
                        grid.ResetAllOutliners();

                        if (delaySucceeded) //successful delay
                        {
                            if (activeUnit.currentTurnCooldown >= activeUnit.baseTurnCooldown) activeUnit.ResetLoweredMovement();
                            availableUnits.Remove(activeUnit);
                            activeUnit.UpdateIcons();
                            //yield return activeUnit.StartCoroutine("EndTurn");
                            activeUnit.ChangeColorTo(Color.gray);
                            activeUnit = null;
                        }
                        else //cancelled action
                        {
                            activeUnit.currentTile.GetTilesInRange(activeUnit, new Color(1f, 0.85f, 0.15f, 1f));
                            activeUnit.currentTile.SetOutlinerActive(true, new Color(0.1f, 0.2f, 0.7f, 1f));
                        }
                    }
                    yield return null;
                }

                foreach (Unit unit in GetAllAlliesOfID(id)) //allows shrimp to add allies to available units, and for pistol to remove them
                {
                    if (unit.currentTurnCooldown == 0 && !availableUnits.Contains(unit))
                    {
                        availableUnits.Add(unit);
                        unit.StartTurn();
                        unit.EnableOutline(true);
                        unit.ChangeColorTo(Color.white);
                    }
                    else if (unit.currentTurnCooldown > 0 && availableUnits.Contains(unit))
                    {
                        availableUnits.Remove(unit);
                        unit.EnableOutline(false);
                        unit.ChangeColorTo(Color.gray);
                        //Debug.Log("you delayed yourself during your own turn");
                    }

                }
                if (Settings.AutopilotMode)
                {
                    break;
                }
            }
        }
        else
        {
            yield return ai.StartCoroutine("TakeAITurn", availableUnits);
        }
    }

    IEnumerator ChooseDelayTarget(List<Unit> alliedUnits)
    {
        Unit targetUnit = null;
        delaySucceeded = false;
        while (targetUnit == null)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.S))
            {
                try
                {
                    if (alliedUnits.Contains(cursor.GetActiveTile().occupyingUnit))
                    {
                        targetUnit = cursor.GetActiveTile().occupyingUnit;
                    }
                }
                catch { }
            }
            else if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {         
                break;
            }
            yield return null;
        }
        if (targetUnit != null && activeUnit.currentTurnCooldown < targetUnit.currentTurnCooldown)
        {
            activeUnit.currentTurnCooldown = targetUnit.currentTurnCooldown;
            delaySucceeded = true;
        }
    }

    IEnumerator UnitAction(Unit activeUnit, GridTile startingTile)
    {
        List<GridTile> attackableTiles = activeUnit.GetAttackableTiles();
        activeUnit.SetWaitButtonActive(true);
        foreach (GridTile tile in attackableTiles)
        {
            if (!tile.GetOutlinerActive()) tile.SetOutlinerActive(true, new Color(1f, 0.2f, 0.2f, 1f));
        }

        GridTile targetTile = null;
        while (targetTile == null)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.S))
            {
                if (cursor.CheckForOptionChange() == "wait")
                {
                    activeUnit.SetWaitButtonActive(false);
                    if (activeUnit.currentTile == startingTile)
                    {
                        if (activeUnit.currentMoveRange == 0)// && activeUnit.currentTile.tileType == TileType.trench && activeUnit.movementType == MoveType.benthic)
                        {
                            activeUnit.ResetLoweredMovement();
                            activeUnit.DelayTurnBy(activeUnit.baseTurnCooldown);
                            yield return activeUnit.StartCoroutine("EndTurn");
                        }
                        else
                        {
                            //activeUnit.ResetLoweredMovement();
                            activeUnit.DelayTurnBy(5);
                        }
                    }

                    availableUnits.Remove(activeUnit);
                    if (activeUnit.currentTile != startingTile) yield return activeUnit.StartCoroutine("EndTurn");
                    activeUnit.ChangeColorTo(Color.gray);
                    try
                    {
                        activeUnit.UpdateIcons();
                    }
                    catch { }
                    break;
                }
                else if (attackableTiles.Contains(cursor.GetActiveTile()))
                {
                    activeUnit.SetWaitButtonActive(false);
                    targetTile = cursor.GetActiveTile();
                    yield return activeUnit.StartCoroutine("Attack", targetTile);
                    availableUnits.Remove(activeUnit);
                    yield return new WaitForSeconds(activeUnit.GetAttackDelay());
                    //yield return new WaitForSeconds(Settings.TurnDelay);
                    yield return activeUnit.StartCoroutine("EndTurn");
                    activeUnit.ChangeColorTo(Color.gray);
                    try
                    {
                        activeUnit.UpdateIcons();
                    }
                    catch { }

                }
            }
            else if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                activeUnit.SetWaitButtonActive(false);
                activeUnit.currentTile.occupyingUnit = null;
                activeUnit.transform.position = startingTile.transform.position;
                //yield return activeUnit.StartCoroutine("MoveTo", startingTile);
                activeUnit.currentTile = startingTile;
                startingTile.occupyingUnit = activeUnit;

                activeUnit.currentTurnCooldown = 0;
                activeUnit.UpdateIcons();
                //startingTile.SetOutlinerActive(true, new Color(0.1f, 0.2f, 0.7f, 1f));
                yield return null;
                break;
            }
            else if (Input.GetKeyDown(KeyCode.Space)) //choose ally to delay turn to match
            {
                grid.ResetAllOutliners();
                activeUnit.SetWaitButtonActive(false);
                List<Unit> alliedUnits = new List<Unit>();
                foreach (Unit unit in unitList)
                {
                    if (unit.playerID == activeUnit.playerID && unit != activeUnit && unit.currentTurnCooldown > activeUnit.currentTurnCooldown) //filter by same playerID
                    {
                        alliedUnits.Add(unit);
                        unit.currentTile.SetOutlinerActive(true, new Color(0.1f, 0.2f, 0.7f, 1f));
                    }
                }
                if (alliedUnits.Count > 0)
                {
                    yield return StartCoroutine(ChooseDelayTarget(alliedUnits)); //either successfully chose delay target, or cancelled with RMB
                }
                else
                {
                    delaySucceeded = false;
                }
                grid.ResetAllOutliners();

                if (delaySucceeded) //successful delay
                {
                    availableUnits.Remove(activeUnit);
                    activeUnit.UpdateIcons();
                    if (activeUnit.currentTile != startingTile) yield return activeUnit.StartCoroutine("EndTurn");
                    activeUnit.ChangeColorTo(Color.gray);
                    activeUnit = null;
                    delaySucceeded = false;
                    break;
                }
                else //cancelled action
                {
                    activeUnit.GetAttackableTiles();
                    foreach (GridTile tile in attackableTiles)
                    {
                        if (!tile.GetOutlinerActive()) tile.SetOutlinerActive(true, new Color(1f, 0.2f, 0.2f, 1f));
                    }
                    activeUnit.SetWaitButtonActive(true);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.D)) //end turn
            {
                activeUnit.SetWaitButtonActive(false);
                if (startingTile == activeUnit.currentTile)
                {
                    if (activeUnit.currentMoveRange == 0)
                    {
                        activeUnit.ResetLoweredMovement();
                        activeUnit.DelayTurnBy(activeUnit.baseTurnCooldown);
                        yield return activeUnit.StartCoroutine("EndTurn");
                    }
                    else
                    {
                        activeUnit.DelayTurnBy(5);
                    }

                }
                else
                {
                    yield return activeUnit.StartCoroutine("EndTurn");
                }
                availableUnits.Remove(activeUnit);

                activeUnit.ChangeColorTo(Color.gray);
                try
                {
                    activeUnit.UpdateIcons();
                }
                catch { }
                break;
            }

            if (activeUnit.GetComponent<TorpedoRay>() != null)
            {
                if (activeUnit.GetAttackableTiles().Contains(cursor.GetActiveTile()) && cursor.GetActiveTile().occupyingUnit != null)
                {
                    grid.ResetAllOutliners();
                    cursor.GetActiveTile().SetOutlinerActive(true, new Color(1f, 0.2f, 0.2f, 1f));
                    foreach (Unit unit in activeUnit.GetBorderingUnits(cursor.GetActiveTile()))
                    {
                        unit.currentTile.SetOutlinerActive(true, new Color(1f, 0.2f, 0.2f, 1f));
                    }
                }
                else
                {
                    grid.ResetAllOutliners();
                    foreach (GridTile tile in attackableTiles)
                    {
                        if (!tile.GetOutlinerActive()) tile.SetOutlinerActive(true, new Color(1f, 0.2f, 0.2f, 1f));
                    }
                }
            }


            yield return null;
        }

        foreach (GridTile tile in attackableTiles)
        {
            tile.SetOutlinerActive(false, Color.white);
        }
    }

}
