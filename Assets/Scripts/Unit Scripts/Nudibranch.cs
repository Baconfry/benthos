using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nudibranch : Unit
{
    [SerializeField] private GameObject stingAnimation;
    [SerializeField] private AudioClip stingSound;

    protected override void Initialize()
    {
        movementType = MoveType.benthic;
        armorType = ArmorType.none;
        isArmored = false;
        isAnchored = true;
        isPoisoned = false;
        healOnKill = true;
        makesContact = true;

        baseTurnCooldown = 30;
        damageValue = 1;
        attackPenalty = 20;

        internalID = 12;

        baseMoveRange = 4;
        maxHealth = 5;
    }

    public override IEnumerator EndTurn()
    {
        yield return null;
        ResetLoweredMovement();
        yield return StartCoroutine(ApplyTileEffects());
        if (isPoisoned && currentHealth < maxHealth)
        {
            yield return new WaitForSeconds(Settings.TurnDelay);
            yield return StartCoroutine("Heal", 1);
        }
    }

    public override IEnumerator Attack(GridTile targetTile)
    {
        if (isPoisoned)
        {
            if (targetTile == currentTile.GetTileLeft(currentTile) || targetTile == currentTile.GetTileRight(currentTile))
            {
                Instantiate(stingAnimation, targetTile.transform.position, Quaternion.Euler(0, 0, 90));
            }
            else
            {
                Instantiate(stingAnimation, targetTile.transform.position, transform.rotation);
            }
            soundEffects.clip = stingSound;
            soundEffects.Play();
        }
        else
        {
            Instantiate(attackAnimation, targetTile.transform.position, transform.rotation);
            soundEffects.clip = attackSound;
            soundEffects.Play();
        }
        yield return new WaitForSeconds(attackAnimation.GetComponent<AttackAnim>().GetTotalAnimationTime());

        if (targetTile.occupyingUnit != null)
        {
            if (isPoisoned)
            {
                if (!targetTile.occupyingUnit.isArmored && targetTile.occupyingUnit.isPoisoned)
                {
                    yield return StartCoroutine(targetTile.occupyingUnit.TakeDamageFrom(this, damageValue + 1, 1));
                }
                else
                {
                    yield return StartCoroutine(targetTile.occupyingUnit.TakeDamageFrom(this, damageValue, 1));
                }
            }
            else
            {
                yield return StartCoroutine(targetTile.occupyingUnit.TakeDamageFrom(this, damageValue, 0));
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

    public override bool IsValidForAI(GridTile tile)
    {
        if (tile.tileType == TileType.vent)
        {
            return false;
        }
        else if (tile.tileType == TileType.anemone)
        {
            return true;
        }
        else if (!isAnchored && tile.tileType == TileType.current && tile.GetFacingTile(tile).occupyingUnit != null && tile.GetFacingTile(tile).occupyingUnit.playerID == playerID && tile.GetFacingTile(tile).occupyingUnit != this)
        {
            return false;
        }
        else if (!isAnchored && tile.tileType == TileType.current && tile.GetFacingTile(tile).tileType != TileType.current && tile.GetFacingTile(tile).occupyingUnit == null)
        {
            return IsValidForAI(tile.GetFacingTile(tile));
        }
        else
        {
            return true;
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
                priorityArray[i] = 2 + candidates[i].currentHealth - candidates[i].maxHealth;
            }
            else
            {
                priorityArray[i] += damageValue;
                if (candidates[i].isArmored && candidates[i].armorType == ArmorType.heavy) priorityArray[i]--;
                if (!candidates[i].isArmored && isPoisoned && candidates[i].isPoisoned) priorityArray[i] *= 2;
                if ((candidates[i].isArmored ? damageValue - 1 : damageValue) >= candidates[i].currentHealth)
                {
                    priorityArray[i] += 20;
                }
                else if (isPoisoned && !candidates[i].isPoisoned && !candidates[i].isArmored)
                {
                    priorityArray[i] += 10;
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

    public override void SetAlphaStatus(bool status)
    {
        isAlpha = status;
        transform.Find("crown").GetComponent<SpriteRenderer>().enabled = status;
        maxHealth = isAlpha ? 6 : 5;
        attackPenalty = isAlpha ? 15 : 20;
        currentHealth = maxHealth;
        isPoisoned = status;
        UpdateIcons();
    }
}
