using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snail : Unit
{
    protected override void Initialize()
    {
        movementType = MoveType.benthic;
        armorType = ArmorType.heavy;
        isArmored = false;
        isAnchored = true;
        isPoisoned = false;
        healOnKill = true;
        makesContact = true;

        baseTurnCooldown = 30;
        damageValue = 1;
        attackPenalty = 30;

        internalID = 2;

        baseMoveRange = 3;
        maxHealth = 7;
    }

    public override void StartTurn()
    {
        isArmored = false;
        UpdateIcons();
    }

    public override IEnumerator Attack(GridTile targetTile)
    {
        Instantiate(attackAnimation, targetTile.transform.position, transform.rotation);
        soundEffects.clip = attackSound;
        soundEffects.Play();
        yield return new WaitForSeconds(attackAnimation.GetComponent<AttackAnim>().GetTotalAnimationTime());
        if (targetTile.occupyingUnit != null)
        {
            yield return StartCoroutine(targetTile.occupyingUnit.TakeDamageFrom(this, damageValue, 1));           
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
        if (!armorBroken) isArmored = true;
        UpdateIcons();

        if (currentHealth == 0)
        {
            StartCoroutine(GetKilledBy(attacker));
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
            if ((candidates[i].isArmored ? damageValue - 1 : damageValue) > 0 && !candidates[i].isPoisoned) priorityArray[i] *= 2;
            if ((candidates[i].isArmored ? damageValue - 1 : damageValue) >= candidates[i].currentHealth) priorityArray[i] += 20;
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
        return finalCandidates;
    }

    public override void SetAlphaStatus(bool status)
    {
        isAlpha = status;
        transform.Find("crown").GetComponent<SpriteRenderer>().enabled = status;
        damageValue = isAlpha ? 2 : 1;
        maxHealth = isAlpha ? 8 : 7;
        currentHealth = maxHealth;
        UpdateIcons();
    }
}
