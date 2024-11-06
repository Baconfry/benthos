using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControl : Definitions
{
    private CustomGrid grid;
    private UnitTracker unitTracker;
    public List<Unit> aiUnits = new List<Unit>();
    
    // Start is called before the first frame update
    void Start()
    {
        grid = GameObject.FindWithTag("Grid").GetComponent<CustomGrid>();
        unitTracker = GameObject.Find("UnitTracker").GetComponent<UnitTracker>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RemoveAIUnit(Unit removedUnit)
    {
        if (aiUnits.Contains(removedUnit)) aiUnits.Remove(removedUnit);
    }

    public IEnumerator TakeAITurn(List<Unit> availableUnits)
    {
        aiUnits = availableUnits;
        int aiID = availableUnits[0].playerID;
        foreach (Unit unit in availableUnits)
        {
            unit.EnableOutline(false);
        }

        while (aiUnits.Count > 0)
        {
            Unit activeUnit = aiUnits[0];
            //activeUnit.StartTurn();
            GridTile startingTile = activeUnit.currentTile;
            List<GridTile> accessibleTiles = activeUnit.currentTile.GetTilesInRange(activeUnit, new Color(1f, 0.85f, 0.15f, 1f));
            activeUnit.currentTile.SetOutlinerActive(true, new Color(0.1f, 0.2f, 0.7f, 1f));
            List<GridTile> tilesToAttackFrom = new List<GridTile>();

            List<Unit> enemyUnits = unitTracker.GetAllEnemiesOfID(activeUnit.playerID);
            List<Unit> attackableEnemyUnits = new List<Unit>();

            foreach (Unit unit in enemyUnits)
            {
                foreach (GridTile tile in activeUnit.GetDestinationForAI(unit.currentTile))
                {
                    if ((!(activeUnit.movementType == MoveType.benthic && tile.tileType == TileType.trench)) && accessibleTiles.Contains(tile) && (activeUnit.IsValidForAI(tile) || (tile == activeUnit.currentTile && accessibleTiles.Count == 1)))
                    {
                        if (!tilesToAttackFrom.Contains(tile)) tilesToAttackFrom.Add(tile);      
                        if (!attackableEnemyUnits.Contains(unit)) attackableEnemyUnits.Add(unit);
                    }
                }
            }

            if (activeUnit.GetComponent<CleanerShrimp>() != null)
            {
                List<Unit> allyUnits = unitTracker.GetAllAlliesOfID(activeUnit.playerID);
                foreach (Unit unit in allyUnits)
                {
                    foreach (GridTile tile in activeUnit.GetDestinationForAI(unit.currentTile))
                    {                      
                        if (accessibleTiles.Contains(tile) && activeUnit.IsValidForAI(tile) && !(unit.GetComponent<CleanerShrimp>() != null && unit.currentHealth == unit.maxHealth) && unit != activeUnit)
                        {
                            if (!tilesToAttackFrom.Contains(tile)) tilesToAttackFrom.Add(tile);
                            if (!attackableEnemyUnits.Contains(unit)) attackableEnemyUnits.Add(unit);
                        }
                    }
                }
            }

            if (activeUnit.GetComponent<Nudibranch>() != null)
            {
                List<Unit> allyUnits = unitTracker.GetAllAlliesOfID(activeUnit.playerID);
                foreach (Unit unit in allyUnits)
                {
                    foreach (GridTile tile in activeUnit.GetDestinationForAI(unit.currentTile))
                    {
                        if (accessibleTiles.Contains(tile) && activeUnit.IsValidForAI(tile) && unit.GetComponent<Nudibranch>() != null && !unit.isPoisoned && activeUnit.isPoisoned)
                        {
                            if (!tilesToAttackFrom.Contains(tile)) tilesToAttackFrom.Add(tile);
                            if (!attackableEnemyUnits.Contains(unit)) attackableEnemyUnits.Add(unit);
                        }
                    }
                }
            }

            bool foundEnemy = false;
            //while (!Input.GetMouseButtonDown(0)) yield return null;
            if (attackableEnemyUnits.Count > 0 && !(activeUnit.GetComponent<Nudibranch>() != null && !activeUnit.isPoisoned && activeUnit.TileTypeWithinRange(TileType.anemone)))
            {
                List<Unit> highestPriorityUnits = activeUnit.AssignAIPriority(attackableEnemyUnits);
                //while (!Input.GetMouseButtonDown(0)) yield return null;
                if (highestPriorityUnits.Count > 0)
                {
                    List<Unit> highestPriorityBorderingUnits = new List<Unit>();
                    foreach (Unit unit in highestPriorityUnits)
                    {
                        if (activeUnit.GetDestinationForAI(unit.currentTile).Contains(startingTile)) highestPriorityBorderingUnits.Add(unit);
                    }
                    Unit enemyToAttack;
                    if (highestPriorityBorderingUnits.Count > 0)
                    {
                        enemyToAttack = highestPriorityBorderingUnits[Random.Range(0, highestPriorityBorderingUnits.Count)];
                    }
                    else
                    {
                        enemyToAttack = highestPriorityUnits[Random.Range(0, highestPriorityUnits.Count)];
                    }
                    List<GridTile> finalTiles = new List<GridTile>();
                    List<GridTile> tilesAroundTarget = activeUnit.GetDestinationForAI(enemyToAttack.currentTile);

                    foreach (GridTile tile in tilesAroundTarget)
                    {
                        //tile.SetOutlinerActive(true, Color.white);
                        if (!(activeUnit.movementType == MoveType.benthic && tile.tileType == TileType.trench))
                        {
                            if (tilesToAttackFrom.Contains(tile) && (tile.occupyingUnit == null || tile.occupyingUnit == activeUnit)) finalTiles.Add(tile);
                        }

                    }

                    GridTile destination;
                    if (finalTiles.Count > 0)
                    {
                        if (finalTiles.Contains(startingTile))
                        {
                            destination = startingTile;
                        }
                        else
                        {
                            destination = finalTiles[Random.Range(0, finalTiles.Count)];
                        }
                        if (enemyToAttack.playerID == activeUnit.playerID && activeUnit.GetComponent<CleanerShrimp>() != null)
                        {
                            enemyToAttack.currentTile.SetOutlinerActive(true, new Color(0.1f, 0.7f, 0.2f, 1f));
                        }
                        else
                        {
                            enemyToAttack.currentTile.SetOutlinerActive(true, new Color(1f, 0.2f, 0.2f, 1f));
                            if (activeUnit.GetComponent<TorpedoRay>() != null)
                            {
                                foreach (Unit unit in activeUnit.GetBorderingUnits(enemyToAttack.currentTile))
                                {
                                    unit.currentTile.SetOutlinerActive(true, new Color(1f, 0.2f, 0.2f, 1f));
                                }
                            }
                        }

                        destination.SetOutlinerActive(true, new Color(0.1f, 0.2f, 0.7f, 1f));
                        //while (!Input.GetMouseButtonDown(0)) yield return null;
                        if (destination != startingTile) yield return activeUnit.StartCoroutine("FollowPath", destination.path);
                        try
                        {
                            if (destination != startingTile) activeUnit.currentTurnCooldown = activeUnit.baseTurnCooldown;
                            activeUnit.UpdateIcons();
                        }
                        catch { Debug.Log(activeUnit + "'s movement cost was not updated"); }
                        yield return new WaitForSeconds(Settings.TurnDelay);
                        yield return activeUnit.StartCoroutine("Attack", enemyToAttack.currentTile);
                        foundEnemy = true;
                        yield return new WaitForSeconds(activeUnit.GetAttackDelay());
                        //yield return new WaitForSeconds(Settings.TurnDelay);
                        aiUnits.Remove(activeUnit);
                    }
                    else
                    {
                        activeUnit.currentTurnCooldown = activeUnit.baseTurnCooldown;
                        activeUnit.UpdateIcons();
                        aiUnits.Remove(activeUnit);
                        Debug.Log("ai didn't find anywhere to move to");
                    }
                }                              
            }
            if (!foundEnemy) //move towards random enemy
            {               
                if (!(activeUnit.movementType == MoveType.benthic && activeUnit.currentTile.tileType == TileType.trench))
                {
                    bool foundOtherTargetTile = false;
                    if (activeUnit.GetComponent<Nudibranch>() != null && !activeUnit.isPoisoned)
                    {
                        List<GridTile> anemoneTiles = new List<GridTile>();
                        foreach (GridTile tile in accessibleTiles)
                        {
                            if (tile.tileType == TileType.anemone && tile.occupyingUnit == null) anemoneTiles.Add(tile);
                        }
                        if (anemoneTiles.Count > 0)
                        {
                            GridTile destination = anemoneTiles[Random.Range(0, anemoneTiles.Count)];
                            destination.SetOutlinerActive(true, new Color(0.1f, 0.2f, 0.7f, 1f));
                            yield return activeUnit.StartCoroutine("FollowPath", destination.path);
                            foundOtherTargetTile = true;
                        }
                    }
                    if (!foundOtherTargetTile)
                    {
                        Unit enemyToMoveTowards = enemyUnits[Random.Range(0, enemyUnits.Count)];
                        for (int i = activeUnit.currentMoveRange; i > 0; i--)
                        {
                            List<GridTile> tilesToCheck = activeUnit.currentTile.GetTilesInRange(activeUnit, new Color(1f, 0.85f, 0.15f, 1f));
                            List<GridTile> closestAccessibleTiles = activeUnit.currentTile.GetClosestAccessibleTilesTo(enemyToMoveTowards.currentTile, tilesToCheck);
                            List<GridTile> closestValidTiles = new List<GridTile>();
                            foreach (GridTile tile in closestAccessibleTiles)
                            {
                                if (activeUnit.IsValidForAI(tile) && tile.occupyingUnit == null) closestValidTiles.Add(tile);
                            }
                            activeUnit.currentTile.SetOutlinerActive(true, new Color(0.1f, 0.2f, 0.7f, 1f));
                            if (closestValidTiles.Count > 0)
                            {
                                GridTile destination = closestValidTiles[Random.Range(0, closestValidTiles.Count)];
                                destination.SetOutlinerActive(true, new Color(0.1f, 0.2f, 0.7f, 1f));
                                yield return activeUnit.StartCoroutine("FollowPath", destination.path);
                                break;
                            }
                            activeUnit.currentMoveRange--;
                            if (activeUnit.currentMoveRange == 0)
                            {
                                activeUnit.currentMoveRange = activeUnit.baseMoveRange;
                                activeUnit.currentTile.GetTilesInRange(activeUnit, new Color(1f, 0.85f, 0.15f, 1f));
                                if (!activeUnit.IsValidForAI(activeUnit.currentTile))
                                {
                                    List<GridTile> randomValidTiles = new List<GridTile>();
                                    foreach (GridTile tile in tilesToCheck)
                                    {
                                        if (activeUnit.IsValidForAI(tile) && tile.occupyingUnit == null) randomValidTiles.Add(tile);
                                    }
                                    if (randomValidTiles.Count > 0)
                                    {
                                        GridTile destination = randomValidTiles[Random.Range(0, randomValidTiles.Count)];
                                        destination.SetOutlinerActive(true, new Color(0.1f, 0.2f, 0.7f, 1f));
                                        yield return activeUnit.StartCoroutine("FollowPath", destination.path);
                                        break;
                                    }
                                }                              
                            }
                        }
                    }
                }
                else //exit trench
                {
                    List<GridTile> tilesThatArentTrench = new List<GridTile>();
                    foreach (GridTile tile in activeUnit.currentTile.GetTilesInRange(activeUnit, new Color(1f, 0.85f, 0.15f, 1f)))
                    {
                        if (tile.tileType != TileType.trench && tile.occupyingUnit == null && activeUnit.IsValidForAI(tile)) tilesThatArentTrench.Add(tile);
                    }
                    if (tilesThatArentTrench.Count > 0)
                    {
                        GridTile destination = tilesThatArentTrench[Random.Range(0, tilesThatArentTrench.Count)];
                        destination.SetOutlinerActive(true, new Color(0.1f, 0.2f, 0.7f, 1f));
                        yield return activeUnit.StartCoroutine("FollowPath", destination.path);
                    }
                }

                
                activeUnit.currentTurnCooldown = activeUnit.baseTurnCooldown;
                activeUnit.UpdateIcons();
                aiUnits.Remove(activeUnit);
            }

            foreach (Unit unit in unitTracker.GetAllAlliesOfID(aiID))
            {
                if (unit.currentTurnCooldown == 0 && !aiUnits.Contains(unit))
                {
                    aiUnits.Add(unit);
                    unit.StartTurn();
                    unit.ChangeColorTo(Color.white);
                }
                else if (unit.currentTurnCooldown > 0 && aiUnits.Contains(unit))
                {
                    aiUnits.Remove(unit);
                    unit.ChangeColorTo(Color.gray);
                    //Debug.Log("you delayed yourself during your own turn");
                }
            }

            if (activeUnit.canMove) yield return new WaitForSeconds(Settings.TurnDelay);
            grid.ResetAllOutliners();
            
            yield return activeUnit.StartCoroutine("EndTurn");
            activeUnit.ChangeColorTo(Color.gray);
            try
            {
                activeUnit.UpdateIcons();
            }
            catch { }
            if (activeUnit.canMove) yield return new WaitForSeconds(Settings.TurnDelay);
            if (activeUnit.playerID == unitTracker.humanPlayerID && !Settings.AutopilotMode) aiUnits.Clear();
            if (activeUnit.playerID != unitTracker.humanPlayerID && !Settings.AutopilotMode && Settings.SandboxMode) aiUnits.Clear();
        }
    }
}
