using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nautilus : Unit
{
    protected override void Initialize()
    {
        movementType = MoveType.pelagic;
        armorType = ArmorType.heavy;
        isArmored = false;
        isAnchored = false;
        isPoisoned = false;
        healOnKill = false;
        makesContact = false;

        baseTurnCooldown = 30;
        damageValue = 1;
        attackPenalty = 25;

        internalID = 5;

        baseMoveRange = 4;
        maxHealth = 6;
    }

    public override List<GridTile> GetAttackableTiles()
    {

        List<GridTile> attackableTiles = new List<GridTile>();
        if (currentTile.GetTileAbove(currentTile) != null)
        {
            attackableTiles.Add(currentTile.GetTileAbove(currentTile));
        }

        if (currentTile.GetTileBelow(currentTile) != null)
        {
            attackableTiles.Add(currentTile.GetTileBelow(currentTile));
        }

        if (currentTile.GetTileRight(currentTile) != null)
        {
            attackableTiles.Add(currentTile.GetTileRight(currentTile));
        }

        if (currentTile.GetTileLeft(currentTile) != null)
        {
            attackableTiles.Add(currentTile.GetTileLeft(currentTile));
        }

        return attackableTiles;
    }

    public override void StartTurn()
    {
        isArmored = false;
        UpdateIcons();
    }

    public override IEnumerator Attack(GridTile targetTile)
    {
        if (targetTile == currentTile.GetTileAbove(currentTile))
        {
            Instantiate(attackAnimation, currentTile.GetTileAbove(currentTile).transform.position, Quaternion.Euler(0f, 0f, 0f));
        }
        else if (targetTile == currentTile.GetTileBelow(currentTile))
        {
            Instantiate(attackAnimation, currentTile.GetTileBelow(currentTile).transform.position, Quaternion.Euler(0f, 0f, 180f));
        }
        else if (targetTile == currentTile.GetTileRight(currentTile))
        {
            Instantiate(attackAnimation, currentTile.GetTileRight(currentTile).transform.position, Quaternion.Euler(0f, 0f, 270f));
        }
        else if (targetTile == currentTile.GetTileLeft(currentTile))
        {
            Instantiate(attackAnimation, currentTile.GetTileLeft(currentTile).transform.position, Quaternion.Euler(0f, 0f, 90f));
        }
        soundEffects.clip = attackSound;
        soundEffects.Play();
        if (targetTile.occupyingUnit != null)
        {
            Unit targetUnit = targetTile.occupyingUnit;
            targetUnit.isMoving = true;
            if (targetUnit.playerID != playerID) yield return StartCoroutine(targetUnit.TakeDamageFrom(this, damageValue, 0));
            yield return new WaitForSeconds(Settings.TurnDelay / 2);
            if (targetTile == currentTile.GetTileAbove(currentTile) && targetTile.GetTileAbove(targetTile) != null)
            {
                yield return targetUnit.StartCoroutine("ForcedMoveTo", targetTile.GetTileAbove(targetTile));
                //targetTile.occupyingUnit.StartCoroutine("ForcedMoveTo", targetTile.GetTileAbove(targetTile));
            }
            else if (targetTile == currentTile.GetTileBelow(currentTile) && targetTile.GetTileBelow(targetTile) != null)
            {
                yield return targetUnit.StartCoroutine("ForcedMoveTo", targetTile.GetTileBelow(targetTile));
                //targetTile.occupyingUnit.StartCoroutine("ForcedMoveTo", targetTile.GetTileBelow(targetTile));
            }
            else if (targetTile == currentTile.GetTileRight(currentTile) && targetTile.GetTileRight(targetTile) != null)
            {
                yield return targetUnit.StartCoroutine("ForcedMoveTo", targetTile.GetTileRight(targetTile));
                //targetTile.occupyingUnit.StartCoroutine("ForcedMoveTo", targetTile.GetTileRight(targetTile));
            }
            else if (targetTile == currentTile.GetTileLeft(currentTile) && targetTile.GetTileLeft(targetTile) != null)
            {
                yield return targetUnit.StartCoroutine("ForcedMoveTo", targetTile.GetTileLeft(targetTile));
                //targetTile.occupyingUnit.StartCoroutine("ForcedMoveTo", targetTile.GetTileLeft(targetTile));
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

    public override void SetAlphaStatus(bool status)
    {
        isAlpha = status;
        transform.Find("crown").GetComponent<SpriteRenderer>().enabled = status;
        maxHealth = isAlpha ? 7 : 6;
        currentHealth = maxHealth;
        baseMoveRange = isAlpha ? 5 : 4;
        currentMoveRange = baseMoveRange;
        UpdateIcons();
    }
}