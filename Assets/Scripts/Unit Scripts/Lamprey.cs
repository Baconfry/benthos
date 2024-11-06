using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamprey : Unit
{
    protected override void Initialize()
    {
        movementType = MoveType.pelagic;
        armorType = ArmorType.none;
        isArmored = false;
        isAnchored = false;
        isPoisoned = false;
        healOnKill = false;
        makesContact = true;

        baseTurnCooldown = 20;
        damageValue = 2;
        attackPenalty = 25;

        internalID = 3;

        baseMoveRange = 5;
        maxHealth = 4;
    }

    public override IEnumerator Attack(GridTile targetTile)
    {
        Instantiate(attackAnimation, targetTile.transform.position, transform.rotation);
        soundEffects.clip = attackSound;
        soundEffects.Play();
        if (targetTile.occupyingUnit != null)
        {
            int leechAmount = damageValue;
            if (targetTile.occupyingUnit.isArmored || targetTile.occupyingUnit.GetComponent<HermitCrab>() != null) leechAmount--;
            if (leechAmount > targetTile.occupyingUnit.currentHealth) leechAmount = targetTile.occupyingUnit.currentHealth;

            yield return StartCoroutine(targetTile.occupyingUnit.TakeDamageFrom(this, damageValue, 0));
            if (leechAmount + currentHealth > maxHealth) leechAmount = maxHealth - currentHealth;
            if (currentHealth < maxHealth)
            {
                yield return new WaitForSeconds(Settings.TurnDelay);
                yield return StartCoroutine("Heal", leechAmount);
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

    public override void SetAlphaStatus(bool status)
    {
        isAlpha = status;
        transform.Find("crown").GetComponent<SpriteRenderer>().enabled = status;
        maxHealth = isAlpha ? 6 : 4;
        currentHealth = maxHealth;
        UpdateIcons();
    }
}
