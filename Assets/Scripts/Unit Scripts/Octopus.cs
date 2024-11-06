using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Octopus : Unit
{
    protected override void Initialize()
    {
        movementType = MoveType.pelagic;
        armorType = ArmorType.none;
        isArmored = false;
        isAnchored = true;
        isPoisoned = false;
        healOnKill = false;
        makesContact = true;

        baseTurnCooldown = 20;
        damageValue = 1;
        attackPenalty = 15;

        internalID = 8;

        baseMoveRange = 4;
        maxHealth = 8;
    }

    public override List<GridTile> GetAttackableTiles()
    {

        List<GridTile> attackableTiles = new List<GridTile>();
        if (currentTile.GetTileAbove(currentTile) != null)
        {          
            if (currentTile.GetTileAbove(currentTile.GetTileAbove(currentTile)) != null && currentTile.GetTileAbove(currentTile).occupyingUnit == null && !currentTile.GetTileAbove(currentTile).IsSolid())
            {
                attackableTiles.Add(currentTile.GetTileAbove(currentTile.GetTileAbove(currentTile)));
            }
            else
            {
                attackableTiles.Add(currentTile.GetTileAbove(currentTile));
            }
        }

        if (currentTile.GetTileBelow(currentTile) != null)
        {          
            if (currentTile.GetTileBelow(currentTile.GetTileBelow(currentTile)) != null && currentTile.GetTileBelow(currentTile).occupyingUnit == null && !currentTile.GetTileBelow(currentTile).IsSolid())
            {
                attackableTiles.Add(currentTile.GetTileBelow(currentTile.GetTileBelow(currentTile)));
            }
            else
            {
                attackableTiles.Add(currentTile.GetTileBelow(currentTile));
            }
        }

        if (currentTile.GetTileRight(currentTile) != null)
        {
            if (currentTile.GetTileRight(currentTile.GetTileRight(currentTile)) != null && currentTile.GetTileRight(currentTile).occupyingUnit == null && !currentTile.GetTileRight(currentTile).IsSolid())
            {
                attackableTiles.Add(currentTile.GetTileRight(currentTile.GetTileRight(currentTile)));
            }
            else
            {
                attackableTiles.Add(currentTile.GetTileRight(currentTile));
            }
        }

        if (currentTile.GetTileLeft(currentTile) != null)
        {          
            if (currentTile.GetTileLeft(currentTile.GetTileLeft(currentTile)) != null && currentTile.GetTileLeft(currentTile).occupyingUnit == null && !currentTile.GetTileLeft(currentTile).IsSolid())
            {
                attackableTiles.Add(currentTile.GetTileLeft(currentTile.GetTileLeft(currentTile)));
            }
            else
            {
                attackableTiles.Add(currentTile.GetTileLeft(currentTile));
            }
        }
        return attackableTiles;
    }

    public override List<GridTile> GetDestinationForAI(GridTile targetTile)
    {
        List<GridTile> destinations = new List<GridTile>();
        if (targetTile.GetTileAbove(targetTile) != null && (targetTile.GetTileAbove(targetTile).occupyingUnit == null || targetTile.GetTileAbove(targetTile).occupyingUnit == this) && !targetTile.GetTileAbove(targetTile).IsSolid())
        {
            destinations.Add(targetTile.GetTileAbove(targetTile));
            if (targetTile.GetTileAbove(targetTile.GetTileAbove(targetTile)) != null && (targetTile.GetTileAbove(targetTile.GetTileAbove(targetTile)).occupyingUnit == null || targetTile.GetTileAbove(targetTile.GetTileAbove(targetTile)).occupyingUnit == this) && !targetTile.GetTileAbove(targetTile.GetTileAbove(targetTile)).IsSolid())
            {
                destinations.Add(targetTile.GetTileAbove(targetTile.GetTileAbove(targetTile)));
            }
        }

        if (targetTile.GetTileBelow(targetTile) != null && (targetTile.GetTileBelow(targetTile).occupyingUnit == null || targetTile.GetTileBelow(targetTile).occupyingUnit == this) && !targetTile.GetTileBelow(targetTile).IsSolid())
        {
            destinations.Add(targetTile.GetTileBelow(targetTile));
            if (targetTile.GetTileBelow(targetTile.GetTileBelow(targetTile)) != null && (targetTile.GetTileBelow(targetTile.GetTileBelow(targetTile)).occupyingUnit == null || targetTile.GetTileBelow(targetTile.GetTileBelow(targetTile)).occupyingUnit == this) && !targetTile.GetTileBelow(targetTile.GetTileBelow(targetTile)).IsSolid())
            {
                destinations.Add(targetTile.GetTileBelow(targetTile.GetTileBelow(targetTile)));
            }
        }

        if (targetTile.GetTileRight(targetTile) != null && (targetTile.GetTileRight(targetTile).occupyingUnit == null || targetTile.GetTileRight(targetTile).occupyingUnit == this) && !targetTile.GetTileRight(targetTile).IsSolid())
        {
            destinations.Add(targetTile.GetTileRight(targetTile));
            if (targetTile.GetTileRight(targetTile.GetTileRight(targetTile)) != null && (targetTile.GetTileRight(targetTile.GetTileRight(targetTile)).occupyingUnit == null || targetTile.GetTileRight(targetTile.GetTileRight(targetTile)).occupyingUnit == this) && !targetTile.GetTileRight(targetTile.GetTileRight(targetTile)).IsSolid())
            {
                destinations.Add(targetTile.GetTileRight(targetTile.GetTileRight(targetTile)));
            }
        }

        if (targetTile.GetTileLeft(targetTile) != null && (targetTile.GetTileLeft(targetTile).occupyingUnit == null || targetTile.GetTileLeft(targetTile).occupyingUnit == this) && !targetTile.GetTileLeft(targetTile).IsSolid())
        {
            destinations.Add(targetTile.GetTileLeft(targetTile));
            if (targetTile.GetTileLeft(targetTile.GetTileLeft(targetTile)) != null && (targetTile.GetTileLeft(targetTile.GetTileLeft(targetTile)).occupyingUnit == null || targetTile.GetTileLeft(targetTile.GetTileLeft(targetTile)).occupyingUnit == this) && !targetTile.GetTileLeft(targetTile.GetTileLeft(targetTile)).IsSolid())
            {
                destinations.Add(targetTile.GetTileLeft(targetTile.GetTileLeft(targetTile)));
            }
        }
        return destinations;
    }

    public override IEnumerator Attack(GridTile targetTile)
    {
        if (targetTile.xCoordinate == currentTile.xCoordinate && targetTile.yCoordinate > currentTile.yCoordinate && currentTile.GetTileAbove(currentTile) != null)
        {
            Instantiate(attackAnimation, currentTile.GetTileAbove(currentTile).transform.position, Quaternion.Euler(0f, 0f, 350f));
        }
        else if (targetTile.xCoordinate == currentTile.xCoordinate && targetTile.yCoordinate < currentTile.yCoordinate && currentTile.GetTileBelow(currentTile) != null)
        {
            Instantiate(attackAnimation, currentTile.GetTileBelow(currentTile).transform.position, Quaternion.Euler(0f, 0f, 170f));
        }
        else if (targetTile.xCoordinate > currentTile.xCoordinate && targetTile.yCoordinate == currentTile.yCoordinate && currentTile.GetTileRight(currentTile) != null)
        {
            Instantiate(attackAnimation, currentTile.GetTileRight(currentTile).transform.position, Quaternion.Euler(0f, 0f, 260f));
        }
        else if (targetTile.xCoordinate < currentTile.xCoordinate && targetTile.yCoordinate == currentTile.yCoordinate && currentTile.GetTileLeft(currentTile) != null)
        {
            Instantiate(attackAnimation, currentTile.GetTileLeft(currentTile).transform.position, Quaternion.Euler(0f, 0f, 80f));
        }
        soundEffects.clip = attackSound;
        soundEffects.Play();
        yield return new WaitForSeconds(0.4f * Settings.TurnDelay);
        //tether distant enemy and drag it to tile in front, then reduce movement range
        if (targetTile.occupyingUnit != null)
        {
            Unit targetUnit = targetTile.occupyingUnit;
            targetUnit.isMoving = true;
            if (targetUnit.playerID != playerID) yield return StartCoroutine(targetUnit.TakeDamageFrom(this, damageValue, 0));
            yield return new WaitForSeconds(Settings.TurnDelay / 2);
            if (targetTile.xCoordinate == currentTile.xCoordinate && targetTile.yCoordinate == currentTile.yCoordinate + 2)
            {
                yield return targetUnit.StartCoroutine("ForcedMoveTo", currentTile.GetTileAbove(currentTile));
            }
            else if (targetTile.xCoordinate == currentTile.xCoordinate && targetTile.yCoordinate == currentTile.yCoordinate - 2)
            {
                yield return targetUnit.StartCoroutine("ForcedMoveTo", currentTile.GetTileBelow(currentTile));
            }
            else if (targetTile.xCoordinate == currentTile.xCoordinate + 2 && targetTile.yCoordinate == currentTile.yCoordinate)
            {
                yield return targetUnit.StartCoroutine("ForcedMoveTo", currentTile.GetTileRight(currentTile));
            }
            else if (targetTile.xCoordinate == currentTile.xCoordinate - 2 && targetTile.yCoordinate == currentTile.yCoordinate)
            {
                yield return targetUnit.StartCoroutine("ForcedMoveTo", currentTile.GetTileLeft(currentTile));
            }
            else
            {
                targetUnit.isMoving = false;
            }
            while (targetUnit.isMoving) yield return null;
            try
            {
                if (targetUnit.playerID != playerID)
                {
                    if (targetUnit.currentTile == currentTile.GetTileAbove(currentTile) || targetUnit.currentTile == currentTile.GetTileBelow(currentTile) || targetUnit.currentTile == currentTile.GetTileRight(currentTile) || targetUnit.currentTile == currentTile.GetTileLeft(currentTile)) targetUnit.currentMoveRange = 0;
                    targetUnit.UpdateIcons();
                }
            }
            catch
            {
                Debug.Log("stop it he's already dead");
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
        UpdateIcons();

        if (isAlpha && attacker != null && attacker.makesContact)
        {
            Instantiate(slowAnimation, attacker.transform.position, attacker.transform.rotation).transform.parent = attacker.transform;
            attacker.DelayTurnBy(20);
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
            if (isAlpha) target.DelayTurnBy(20);
            yield return StartCoroutine(target.TakeDamageFrom(null, 1, 0));
        }
    }
}
