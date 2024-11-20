using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorpedoRay : Unit
{
    [SerializeField] private GameObject secondaryAttackAnim;
    
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
        damageValue = 2;
        attackPenalty = 25;

        internalID = 13;

        baseMoveRange = 4;
        maxHealth = 6;
    }

    public override IEnumerator Attack(GridTile targetTile)
    {
        if (targetTile == currentTile.GetTileRight(currentTile) || targetTile == currentTile.GetTileLeft(currentTile))
        {
            Instantiate(attackAnimation, targetTile.transform.position, Quaternion.Euler(0f, 0f, 90f));
        }
        else
        {
            Instantiate(attackAnimation, targetTile.transform.position, Quaternion.Euler(0f, 0f, 0f));
        }
        soundEffects.clip = attackSound;
        soundEffects.Play();
        yield return new WaitForSeconds(attackAnimation.GetComponent<AttackAnim>().GetTotalAnimationTime());
        if (targetTile.occupyingUnit != null)
        {
            yield return StartCoroutine(targetTile.occupyingUnit.TakeDamageFrom(this, damageValue, 0));
            //yield return new WaitForSeconds(attackAnimation.GetComponent<AttackAnim>().GetTotalAnimationTime());
            foreach (Unit unit in GetBorderingUnits(targetTile))
            {
                if (unit.currentTile == targetTile.GetTileRight(targetTile) || unit.currentTile == targetTile.GetTileLeft(targetTile))
                {
                    Instantiate(secondaryAttackAnim, unit.transform.position, Quaternion.Euler(0f, 0f, 90f));
                }
                else
                {
                    Instantiate(secondaryAttackAnim, unit.transform.position, Quaternion.Euler(0f, 0f, 0f));
                }

                yield return StartCoroutine(unit.TakeDamageFrom(this, damageValue - 1, 0));
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

        if (isAlpha && attacker != null && attacker.makesContact) yield return StartCoroutine(attacker.TakeDamageFrom(null, 1, 0));

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
            if (candidates[i].playerID == playerID)
            {
                if (!(candidates[i].isArmored && candidates[i].armorType == ArmorType.heavy)) priorityArray[i]--;
                if (!candidates[i].isArmored && candidates[i].currentHealth <= (damageValue - 1)) priorityArray[i] -= 20;
                foreach (Unit unit in GetBorderingUnits(candidates[i].currentTile))
                {
                    if (unit.playerID == playerID)
                    {
                        if (!(unit.isArmored && unit.armorType == ArmorType.heavy)) priorityArray[i]--;
                        if (!unit.isArmored && unit.currentHealth <= (damageValue - 1)) priorityArray[i] -= 20;
                    }
                    else
                    {
                        priorityArray[i]++;
                        if (unit.isArmored && unit.armorType == ArmorType.heavy) priorityArray[i]--;
                        if (!unit.isArmored && unit.currentHealth <= (damageValue - 1)) priorityArray[i] += 10;
                    }
                }
            }
            else
            {
                priorityArray[i] += damageValue;
                if (candidates[i].isArmored && candidates[i].armorType == ArmorType.heavy) priorityArray[i]--;
                if ((candidates[i].isArmored ? damageValue - 1 : damageValue) >= candidates[i].currentHealth) priorityArray[i] += 10;
                foreach (Unit unit in GetBorderingUnits(candidates[i].currentTile))
                {
                    if (unit.playerID == playerID)
                    {
                        if (!(unit.isArmored && unit.armorType == ArmorType.heavy)) priorityArray[i]--;
                        if (!unit.isArmored && unit.currentHealth <= (damageValue - 1)) priorityArray[i] -= 20;
                    }
                    else
                    {
                        priorityArray[i]++;
                        if (unit.isArmored && unit.armorType == ArmorType.heavy) priorityArray[i]--;
                        if (!unit.isArmored && unit.currentHealth <= (damageValue - 1)) priorityArray[i] += 10;
                    }
                }
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

    public override IEnumerator DealBumpDamage(Unit target)
    {
        if (target.bumpImmune)
        {
            yield return null;
        }
        else if (isAlpha)
        {
            yield return StartCoroutine(target.TakeDamageFrom(null, 2, 0));
        }
        else
        {
            yield return StartCoroutine(target.TakeDamageFrom(null, 1, 0));
        }
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
