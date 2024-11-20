using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goby : Unit
{
    [SerializeField] private GameObject projectileAnimation;

    protected override void Initialize()
    {
        movementType = MoveType.pelagic;
        armorType = ArmorType.none;
        isArmored = false;
        isAnchored = false;
        isPoisoned = false;
        healOnKill = false;
        makesContact = false;

        baseTurnCooldown = 20;      
        damageValue = 1;
        attackPenalty = 25;

        internalID = 6;

        baseMoveRange = 4;
        maxHealth = 5;
    }

    public override IEnumerator Attack(GridTile targetTile)
    {
        yield return Instantiate(projectileAnimation, transform.position, transform.rotation).GetComponent<ProjectileAnim>().StartCoroutine("MoveToTarget", targetTile);
        Instantiate(attackAnimation, targetTile.transform.position, transform.rotation);
        soundEffects.clip = attackSound;
        soundEffects.Play();
        yield return new WaitForSeconds(attackAnimation.GetComponent<AttackAnim>().GetTotalAnimationTime());
        if (targetTile.occupyingUnit != null)
        {
            int targetInitialTurnDelay = targetTile.occupyingUnit.currentTurnCooldown;
            Instantiate(slowAnimation, targetTile.occupyingUnit.transform.position, targetTile.occupyingUnit.transform.rotation).transform.parent = targetTile.occupyingUnit.transform;
            if (targetInitialTurnDelay + 20 < 100)
            {
                yield return StartCoroutine(targetTile.occupyingUnit.TakeDamageFrom(this, damageValue, 0));
            }
            else
            {
                yield return StartCoroutine(targetTile.occupyingUnit.TakeDamageFrom(this, 10, 0));
            }
            if (targetTile.occupyingUnit != null) targetTile.occupyingUnit.DelayTurnBy(20);
        }
        switch (targetTile.tileType)
        {
            case TileType.stone:
            case TileType.sand:
            case TileType.algae:
            case TileType.current:
            case TileType.anemone:
            case TileType.coral:
            case TileType.trench:
            case TileType.vent:
                if (targetTile.occupyingUnit != null)
                {
                    if (targetTile.occupyingUnit.currentHealth <= 0)
                    {
                        targetTile.ChangeTileTo(TileType.fireCoral);
                    }
                    else 
                    { 
                        targetTile.ChangeTileTo(TileType.sand); 
                    }                  
                }
                else
                {
                    targetTile.ChangeTileTo(TileType.fireCoral);
                }
                targetTile.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                break;
            case TileType.outcrop:
                break;
            case TileType.fireCoral:
                targetTile.DecaySandPile(-20);
                break;
            default:
                Debug.Log("attempted to reference invalid type");
                break;
        }
        currentTurnCooldown += attackPenalty;
        UpdateIcons();
        yield return null;
    }

    public override List<GridTile> GetAttackableTiles()
    {
        List<GridTile> attackableTiles = grid.GetTilesWithinRangeOf(isAlpha ? 3 : 2, currentTile.xCoordinate, currentTile.yCoordinate);
        if (attackableTiles.Contains(currentTile)) attackableTiles.Remove(currentTile);

        return attackableTiles;
    }

    public override List<GridTile> GetDestinationForAI(GridTile targetTile)
    {
        List<GridTile> destinations = new List<GridTile>();
        List<GridTile> surroundingTiles = grid.GetTilesWithinRangeOf(isAlpha ? 3 : 2, targetTile.xCoordinate, targetTile.yCoordinate);
        foreach (GridTile tile in surroundingTiles)
        {
            if ((tile.occupyingUnit == null || tile.occupyingUnit == this) && currentTile.CanMoveTo(tile, this))
            {
                destinations.Add(tile);
            }
            if (destinations.Contains(targetTile)) destinations.Remove(targetTile);
        }
        return destinations;
    }

    public override List<Unit> AssignAIPriority(List<Unit> candidates)
    {
        int[] priorityArray = new int[candidates.Count];
        for (int i = 0; i < candidates.Count; i++)
        {
            priorityArray[i] = 0;
            priorityArray[i] += damageValue;
            if (candidates[i].isArmored && candidates[i].armorType == ArmorType.heavy) priorityArray[i]--;
            if ((candidates[i].isArmored ? damageValue - 1 : damageValue) >= candidates[i].currentHealth)
            {
                priorityArray[i] += 20;
            }
            else
            {
                priorityArray[i] = (100 - candidates[i].currentTurnCooldown) / 10;
                if (!candidates[i].isArmored) priorityArray[i] *= 2;
            }
            //Debug.Log(candidates[i].gameObject.name + " " + priorityArray[i]);
        }
        List<Unit> finalCandidates = new List<Unit>();
        //finalCandidates.Add(candidates[0]);
        int startingPriority = 0;
        for (int i = 0; i < priorityArray.Length; i++)
        {
            if (priorityArray[i] > startingPriority)
            {
                finalCandidates.Clear();
                finalCandidates.Add(candidates[i]);
                startingPriority = priorityArray[i];
            }
            else if (priorityArray[i] == startingPriority)
            {
                finalCandidates.Add(candidates[i]);
            }
        }
        //if (finalCandidates.Count == 0) finalCandidates.Add(candidates[0]);
        /*foreach (Unit candidate in finalCandidates)
        {
            candidate.currentTile.SetOutlinerActive(true, Color.white);
        }*/
        return finalCandidates;
    }
}
