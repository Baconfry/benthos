using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnappingShrimp : Unit
{
    [SerializeField] private GameObject projectileAnimation;

    protected override void Initialize()
    {
        movementType = MoveType.benthic;
        armorType = ArmorType.light;
        isArmored = true;
        isAnchored = false;
        isPoisoned = false;
        healOnKill = false;
        makesContact = false;

        baseTurnCooldown = 35;
        damageValue = 2;
        attackPenalty = 30;

        internalID = 4;

        baseMoveRange = 3;
        maxHealth = 3;
    }

    public override List<GridTile> GetAttackableTiles()
    {
        List<GridTile> attackableTiles = new List<GridTile>();
        GridTile aboveTile = currentTile.GetTileAbove(currentTile);
       if (aboveTile != null)
        {
            while (aboveTile.occupyingUnit == null)
            {
                if (aboveTile.IsSolid()) break;
                if (aboveTile.GetTileAbove(aboveTile) != null)
                {
                    aboveTile = aboveTile.GetTileAbove(aboveTile);
                }
                else
                {
                    break;
                }
            }
            if (aboveTile != null) attackableTiles.Add(aboveTile);
        }
        GridTile belowTile = currentTile.GetTileBelow(currentTile);
        if (belowTile != null)
        {
            while (belowTile.occupyingUnit == null)
            {
                if (belowTile.IsSolid()) break;
                if (belowTile.GetTileBelow(belowTile) != null)
                {
                    belowTile = belowTile.GetTileBelow(belowTile);
                }
                else
                {
                    break;
                }
            }
            if (belowTile != null) attackableTiles.Add(belowTile);
        }
        GridTile rightTile = currentTile.GetTileRight(currentTile);
        if (rightTile != null)
        {
            while (rightTile.occupyingUnit == null)
            {
                if (rightTile.IsSolid()) break;
                if (rightTile.GetTileRight(rightTile) != null)
                {
                    rightTile = rightTile.GetTileRight(rightTile);
                }
                else
                {
                    break;
                }
            }
            if (rightTile != null) attackableTiles.Add(rightTile);
        }
        GridTile leftTile = currentTile.GetTileLeft(currentTile);
        if (leftTile != null)
        {
            while (leftTile.occupyingUnit == null)
            {
                if (leftTile.IsSolid()) break;
                if (leftTile.GetTileLeft(leftTile) != null)
                {
                    leftTile = leftTile.GetTileLeft(leftTile);
                }
                else
                {
                    break;
                }
            }
            if (leftTile != null) attackableTiles.Add(leftTile);
        }
        if (currentTile.tileType == TileType.trench && movementType == MoveType.benthic) attackableTiles.Clear();
        return attackableTiles;
    }

    public override List<GridTile> GetDestinationForAI(GridTile targetTile)
    {
        List<GridTile> destinations = new List<GridTile>();
        GridTile aboveTile = targetTile.GetTileAbove(targetTile);
        if (aboveTile != null)
        {
            while (aboveTile != null && (aboveTile.occupyingUnit == null || aboveTile.occupyingUnit == this) && !aboveTile.IsSolid())
            {
                destinations.Add(aboveTile);
                aboveTile = aboveTile.GetTileAbove(aboveTile);
            }
        }
        GridTile belowTile = targetTile.GetTileBelow(targetTile);
        if (belowTile != null)
        {
            while (belowTile != null && (belowTile.occupyingUnit == null || belowTile.occupyingUnit == this) && !belowTile.IsSolid())
            {
                destinations.Add(belowTile);
                belowTile = belowTile.GetTileBelow(belowTile);
            }
        }
        GridTile rightTile = targetTile.GetTileRight(targetTile);
        if (rightTile != null)
        {
            while (rightTile != null && (rightTile.occupyingUnit == null || rightTile.occupyingUnit == this) && !rightTile.IsSolid())
            {
                destinations.Add(rightTile);
                rightTile = rightTile.GetTileRight(rightTile);
            }
        }
        GridTile leftTile = targetTile.GetTileLeft(targetTile);
        if (leftTile != null)
        {
            while (leftTile != null && (leftTile.occupyingUnit == null || leftTile.occupyingUnit == this) && !leftTile.IsSolid())
            {
                destinations.Add(leftTile);
                leftTile = leftTile.GetTileLeft(leftTile);
            }
        }

        return destinations;
    }

    public override IEnumerator Attack(GridTile targetTile)
    {
        Quaternion projectileRotation = Quaternion.Euler(0f, 0f, 0f);
        if (targetTile.xCoordinate == currentTile.xCoordinate && targetTile.yCoordinate > currentTile.yCoordinate && currentTile.GetTileAbove(currentTile) != null)
        {
            Instantiate(attackAnimation, currentTile.GetTileAbove(currentTile).transform.position, Quaternion.Euler(0f, 0f, 0f));
            projectileRotation = Quaternion.Euler(0f, 0f, 90f);
        }
        else if (targetTile.xCoordinate == currentTile.xCoordinate && targetTile.yCoordinate < currentTile.yCoordinate && currentTile.GetTileBelow(currentTile) != null)
        {
            Instantiate(attackAnimation, currentTile.GetTileBelow(currentTile).transform.position, Quaternion.Euler(0f, 0f, 180f));
            projectileRotation = Quaternion.Euler(0f, 0f, 270f);
        }
        else if (targetTile.xCoordinate > currentTile.xCoordinate && targetTile.yCoordinate == currentTile.yCoordinate && currentTile.GetTileRight(currentTile) != null)
        {
            Instantiate(attackAnimation, currentTile.GetTileRight(currentTile).transform.position, Quaternion.Euler(0f, 0f, 270f));
            projectileRotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if (targetTile.xCoordinate < currentTile.xCoordinate && targetTile.yCoordinate == currentTile.yCoordinate && currentTile.GetTileLeft(currentTile) != null)
        {
            Instantiate(attackAnimation, currentTile.GetTileLeft(currentTile).transform.position, Quaternion.Euler(0f, 0f, 90f));
            projectileRotation = Quaternion.Euler(0f, 0f, 180f);

        }
        soundEffects.clip = attackSound;
        soundEffects.Play();
        yield return Instantiate(projectileAnimation, transform.position, projectileRotation).GetComponent<ProjectileAnim>().StartCoroutine("MoveToTarget", targetTile);

        if (targetTile.occupyingUnit != null)
        {
            Unit targetUnit = targetTile.occupyingUnit;
            targetUnit.isMoving = true;
            yield return StartCoroutine(targetUnit.TakeDamageFrom(this, damageValue, 0));
            yield return new WaitForSeconds(Settings.TurnDelay / 2);
            Instantiate(slowAnimation, targetUnit.transform.position, targetUnit.transform.rotation).transform.parent = targetUnit.transform;
            targetUnit.DelayTurnBy(10);
            if (targetTile.xCoordinate == currentTile.xCoordinate && targetTile.yCoordinate > currentTile.yCoordinate && targetTile.GetTileAbove(targetTile) != null)
            {
                yield return targetUnit.StartCoroutine("ForcedMoveTo", targetTile.GetTileAbove(targetTile));
                //targetTile.occupyingUnit.StartCoroutine("ForcedMoveTo", targetTile.GetTileAbove(targetTile));
            }
            else if (targetTile.xCoordinate == currentTile.xCoordinate && targetTile.yCoordinate < currentTile.yCoordinate && targetTile.GetTileBelow(targetTile) != null)
            {
                yield return targetUnit.StartCoroutine("ForcedMoveTo", targetTile.GetTileBelow(targetTile));
                //targetTile.occupyingUnit.StartCoroutine("ForcedMoveTo", targetTile.GetTileBelow(targetTile));
            }
            else if (targetTile.xCoordinate > currentTile.xCoordinate && targetTile.yCoordinate == currentTile.yCoordinate && targetTile.GetTileRight(targetTile) != null)
            {
                yield return targetUnit.StartCoroutine("ForcedMoveTo", targetTile.GetTileRight(targetTile));
                //targetTile.occupyingUnit.StartCoroutine("ForcedMoveTo", targetTile.GetTileRight(targetTile));
            }
            else if (targetTile.xCoordinate < currentTile.xCoordinate && targetTile.yCoordinate == currentTile.yCoordinate && targetTile.GetTileLeft(targetTile) != null)
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
        attackPenalty = isAlpha ? 20 : 30;
        passThroughEnemies = status;
    }
}
