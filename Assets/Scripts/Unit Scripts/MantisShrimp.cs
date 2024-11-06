using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MantisShrimp : Unit
{   
    protected override void Initialize()
    {
        movementType = MoveType.benthic;
        armorType = ArmorType.light;
        isArmored = true;
        isAnchored = false;
        isPoisoned = false;
        healOnKill = false;
        makesContact = true;

        baseTurnCooldown = 20;
        damageValue = 2;
        attackPenalty = 25;

        internalID = 9;

        baseMoveRange = 4;
        maxHealth = 3;
    }

    public override void StartTurn()
    {
        if (!armorBroken) isArmored = true;
        UpdateIcons();
    }

    public override IEnumerator TakeDamageFrom(Unit attacker, int damage, int poisonValue)
    {
        yield return null;
        int finalDamage = damage;
        if (isArmored && finalDamage > 0) finalDamage--;
        if (poisonValue > 0 && finalDamage > 0)
        {
            isPoisoned = true;
        }
        if (finalDamage > currentHealth) finalDamage = currentHealth;
        if (currentHealth > 0) Instantiate(damageNumber, transform.position, transform.rotation).GetComponent<DamageNumber>().value = finalDamage;
        currentHealth -= finalDamage;
        isArmored = false;
        UpdateIcons();

        if (currentHealth == 0)
        {
            StartCoroutine(GetKilledBy(attacker));
        }
    }

    public override IEnumerator Attack(GridTile targetTile)
    {
        Instantiate(attackAnimation, targetTile.transform.position, transform.rotation);
        soundEffects.clip = attackSound;
        soundEffects.Play();
        if (targetTile.occupyingUnit != null)
        {
            Unit targetUnit = targetTile.occupyingUnit;
            targetUnit.isArmored = false;
            targetUnit.armorBroken = true;
            targetUnit.isMoving = true;
            yield return StartCoroutine(targetUnit.TakeDamageFrom(this, damageValue, 0));
            yield return new WaitForSeconds(Settings.TurnDelay / 2);
            if (targetTile == currentTile.GetTileAbove(currentTile) && targetTile.GetTileAbove(targetTile) != null)
            {
                yield return targetUnit.StartCoroutine("ForcedMoveTo", targetTile.GetTileAbove(targetTile));
            }
            else if (targetTile == currentTile.GetTileBelow(currentTile) && targetTile.GetTileBelow(targetTile) != null)
            {
                yield return targetUnit.StartCoroutine("ForcedMoveTo", targetTile.GetTileBelow(targetTile));
            }
            else if (targetTile == currentTile.GetTileRight(currentTile) && targetTile.GetTileRight(targetTile) != null)
            {
                yield return targetUnit.StartCoroutine("ForcedMoveTo", targetTile.GetTileRight(targetTile));
            }
            else if (targetTile == currentTile.GetTileLeft(currentTile) && targetTile.GetTileLeft(targetTile) != null)
            {
                yield return targetUnit.StartCoroutine("ForcedMoveTo", targetTile.GetTileLeft(targetTile));
            }
            else
            {
                targetUnit.isMoving = false;
            }
            while (targetUnit.isMoving) yield return null;
        }
        if (targetTile.tileType == TileType.coral)
        {
            targetTile.ChangeTileTo(TileType.stone);
        }
        if (targetTile.tileType == TileType.fireCoral)
        {
            targetTile.DecaySandPile(20);
        }
        currentTurnCooldown += attackPenalty;
        UpdateIcons();
        yield return null;
    }

    public override List<Unit> AssignAIPriority(List<Unit> candidates)
    {
        int[] priorityArray = new int[candidates.Count];
        for (int i = 0; i < candidates.Count; i++)
        {
            priorityArray[i] = 0;
            priorityArray[i] += damageValue;
            if (candidates[i].armorType != ArmorType.none && !candidates[i].armorBroken) priorityArray[i]++;
            if (priorityArray[i] >= candidates[i].currentHealth) priorityArray[i] += 20;
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

    public override void SetAlphaStatus(bool status)
    {
        isAlpha = status;
        transform.Find("crown").GetComponent<SpriteRenderer>().enabled = status;
        maxHealth = isAlpha ? 4 : 3;
        currentHealth = maxHealth;
        passThroughEnemies = status;
        UpdateIcons();
    }
}
