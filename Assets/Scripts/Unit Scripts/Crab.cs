using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crab : Unit
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

        baseTurnCooldown = 40;
        damageValue = 2;
        attackPenalty = 15;

        internalID = 1;

        baseMoveRange = 4;
        maxHealth = 5;
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
        maxHealth = isAlpha ? 6 : 5;
        currentHealth = maxHealth;
        UpdateIcons();
        isAnchored = isAlpha;
    }
}
