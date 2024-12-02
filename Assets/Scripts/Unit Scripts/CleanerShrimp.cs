using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanerShrimp : Unit
{
    private bool targetedAlly;
    [SerializeField] private GameObject healAnimation;
    [SerializeField] private AudioClip healSound;

    protected override void Initialize()
    {
        movementType = MoveType.benthic;
        armorType = ArmorType.light;
        isArmored = true;
        isAnchored = false;
        isPoisoned = false;
        healOnKill = false;
        makesContact = true;

        targetedAlly = false;

        baseTurnCooldown = 15;
        damageValue = 1;
        attackPenalty = 25;

        internalID = 7;

        baseMoveRange = 4;
        maxHealth = 3;
    }

    public override float GetAttackDelay()
    {
        if (targetedAlly)
        {
            return healAnimation.GetComponent<AttackAnim>().GetTotalAnimationTime() * Settings.TurnDelay;
        }
        else
        {
            return attackAnimation.GetComponent<AttackAnim>().GetTotalAnimationTime() * Settings.TurnDelay;
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
                if (candidates[i].currentTurnCooldown >= 20)
                {
                    priorityArray[i] += 20;
                }
                else
                {
                    priorityArray[i] += candidates[i].currentTurnCooldown;
                }
                if (candidates[i].currentHealth < candidates[i].maxHealth && candidates[i].GetComponent<HermitCrab>() == null) priorityArray[i] *= 2;
                if (isAlpha && candidates[i].isPoisoned && candidates[i].GetComponent<Nudibranch>() == null) priorityArray[i] *= 2;
            }
            else
            {
                priorityArray[i] += damageValue;
                if (candidates[i].isArmored && candidates[i].armorType == ArmorType.heavy) priorityArray[i]--;
                if ((candidates[i].isArmored ? damageValue - 1 : damageValue) >= candidates[i].currentHealth) priorityArray[i] += 20;
            }
        }
        List<Unit> finalCandidates = new List<Unit>();
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

    public override List<GridTile> GetAttackableTiles()
    {

        List<GridTile> attackableTiles = new List<GridTile>();
        attackableTiles.Add(currentTile); //unit can attack (heal) itself
        currentTile.SetOutlinerActive(true, new Color(0.1f, 0.7f, 0.2f, 1f));
        if (currentTile.GetTileAbove(currentTile) != null)
        {
            attackableTiles.Add(currentTile.GetTileAbove(currentTile));
            try
            {
                if (currentTile.GetTileAbove(currentTile).occupyingUnit.playerID == this.playerID)
                {
                    currentTile.GetTileAbove(currentTile).SetOutlinerActive(true, new Color(0.1f, 0.7f, 0.2f, 1f));
                }
            }
            catch { }
        }

        if (currentTile.GetTileBelow(currentTile) != null)
        {
            attackableTiles.Add(currentTile.GetTileBelow(currentTile));
            try
            {
                if (currentTile.GetTileBelow(currentTile).occupyingUnit.playerID == this.playerID)
                {
                    currentTile.GetTileBelow(currentTile).SetOutlinerActive(true, new Color(0.1f, 0.7f, 0.2f, 1f));
                }
            }
            catch { }
        }

        if (currentTile.GetTileRight(currentTile) != null)
        {
            attackableTiles.Add(currentTile.GetTileRight(currentTile));
            try
            {
                if (currentTile.GetTileRight(currentTile).occupyingUnit.playerID == this.playerID)
                {
                    currentTile.GetTileRight(currentTile).SetOutlinerActive(true, new Color(0.1f, 0.7f, 0.2f, 1f));
                }
            }
            catch { }
        }

        if (currentTile.GetTileLeft(currentTile) != null)
        {
            attackableTiles.Add(currentTile.GetTileLeft(currentTile));
            try
            {
                if (currentTile.GetTileLeft(currentTile).occupyingUnit.playerID == this.playerID)
                {
                    currentTile.GetTileLeft(currentTile).SetOutlinerActive(true, new Color(0.1f, 0.7f, 0.2f, 1f));
                }
            }
            catch { }
        }
        if (currentTile.tileType == TileType.trench && movementType == MoveType.benthic)
        {
            foreach (GridTile tile in attackableTiles) tile.SetOutlinerActive(false, Color.white);
            attackableTiles.Clear();
        }
        return attackableTiles;
    }

    public override IEnumerator Attack(GridTile targetTile)
    {
        if (targetTile.occupyingUnit != null)
        {
            Unit targetUnit = targetTile.occupyingUnit;
            if (targetUnit.playerID == playerID)
            {
                targetedAlly = true;
                Instantiate(healAnimation, targetUnit.transform.position, transform.rotation);
                soundEffects.clip = healSound;
                soundEffects.Play();
                yield return new WaitForSeconds(healAnimation.GetComponent<AttackAnim>().GetTotalAnimationTime());
                if (targetUnit.currentHealth < targetUnit.maxHealth) yield return targetUnit.StartCoroutine("Heal", 1);
                if (isAlpha && targetUnit.isPoisoned && targetUnit.GetComponent<Nudibranch>() == null) targetUnit.isPoisoned = false;
                targetUnit.DelayTurnBy(-20);
            }
            else
            {
                targetedAlly = false;
                soundEffects.clip = attackSound;
                soundEffects.Play();
                Instantiate(attackAnimation, targetUnit.transform.position, transform.rotation);
                yield return new WaitForSeconds(attackAnimation.GetComponent<AttackAnim>().GetTotalAnimationTime());
                yield return StartCoroutine(targetUnit.TakeDamageFrom(this, damageValue, 0));
            }           
        }
        else
        {
            soundEffects.clip = attackSound;
            soundEffects.Play();
            Instantiate(attackAnimation, targetTile.transform.position, transform.rotation);
            yield return new WaitForSeconds(attackAnimation.GetComponent<AttackAnim>().GetTotalAnimationTime());
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

    public override void SetAlphaStatus(bool status)
    {
        isAlpha = status;
        transform.Find("crown").GetComponent<SpriteRenderer>().enabled = status;
        maxHealth = isAlpha ? 4 : 3;
        attackPenalty = isAlpha ? 20 : 25;
        currentHealth = maxHealth;
        passThroughEnemies = status;
        UpdateIcons();
    }
}
