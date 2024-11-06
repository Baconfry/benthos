using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manowar : Unit
{
    protected override void Initialize()
    {
        movementType = MoveType.pelagic;
        armorType = ArmorType.none;
        isArmored = false;
        isAnchored = false;
        isPoisoned = false;
        healOnKill = true;
        makesContact = true;

        baseTurnCooldown = 25;
        damageValue = 1;
        attackPenalty = 30;

        internalID = 11;

        baseMoveRange = 3;
        maxHealth = 6;
    }

    public override IEnumerator Attack(GridTile targetTile)
    {
        if (targetTile == currentTile.GetTileLeft(currentTile) || targetTile == currentTile.GetTileRight(currentTile))
        {
            Instantiate(attackAnimation, targetTile.transform.position, Quaternion.Euler(0, 0, 90));
        }
        else
        {
            Instantiate(attackAnimation, targetTile.transform.position, transform.rotation);
        }
        soundEffects.clip = attackSound;
        soundEffects.Play();

        if (targetTile.occupyingUnit != null)
        {
            if (!targetTile.occupyingUnit.isArmored && targetTile.occupyingUnit.isPoisoned)
            {
                yield return StartCoroutine(targetTile.occupyingUnit.TakeDamageFrom(this, damageValue + 1, 1));
            }
            else
            {
                yield return StartCoroutine(targetTile.occupyingUnit.TakeDamageFrom(this, damageValue, 1));
            }
            if (isAlpha && targetTile.occupyingUnit != null) 
            {
                Instantiate(slowAnimation, targetTile.occupyingUnit.transform.position, targetTile.occupyingUnit.transform.rotation).transform.parent = targetTile.occupyingUnit.transform;
                targetTile.occupyingUnit.DelayTurnBy(10);
            }
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
        UpdateIcons();

        if (attacker != null && attacker.makesContact && !attacker.isArmored)
        {
            if (isAlpha)
            {
                Instantiate(slowAnimation, attacker.transform.position, attacker.transform.rotation).transform.parent = attacker.transform;
                attacker.DelayTurnBy(10);
            }
            attacker.DirectPoison();
        }

        if (currentHealth == 0)
        {
            StartCoroutine(GetKilledBy(attacker));
        }
    }

    public override IEnumerator DealBumpDamage(Unit target)
    {
        if (target.bumpImmune)
        {
            yield return null;
        }
        else
        {
            if (isAlpha) target.DelayTurnBy(10);
            yield return StartCoroutine(target.TakeDamageFrom(null, 1, 1));
        }
    }

    public override List<Unit> AssignAIPriority(List<Unit> candidates)
    {
        int[] priorityArray = new int[candidates.Count];
        for (int i = 0; i < candidates.Count; i++)
        {
            priorityArray[i] = 0;
            priorityArray[i] += damageValue;
            if (candidates[i].isArmored && candidates[i].armorType == ArmorType.heavy) priorityArray[i]--;
            if (candidates[i].isPoisoned) priorityArray[i] *= 2;
            if ((candidates[i].isArmored ? damageValue - 1 : damageValue) >= candidates[i].currentHealth)
            {
                priorityArray[i] += 20;
            }
            else if (!candidates[i].isPoisoned && !candidates[i].isArmored)
            {
                priorityArray[i] += 10;
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

    public override void SetAlphaStatus(bool status)
    {
        isAlpha = status;
        transform.Find("crown").GetComponent<SpriteRenderer>().enabled = status;
        maxHealth = isAlpha ? 7 : 6;
        currentHealth = maxHealth;
        UpdateIcons();
    }
}
