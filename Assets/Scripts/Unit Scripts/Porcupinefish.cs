using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Porcupinefish : Unit
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

        //bumpImmune = true;

        baseTurnCooldown = 40;
        damageValue = 2;
        attackPenalty = 20;

        internalID = 10;

        baseMoveRange = 3;
        maxHealth = 5;
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

        if (attacker != null && attacker.makesContact) yield return StartCoroutine(attacker.TakeDamageFrom(null, 1, 0));

        if (currentHealth == 0)
        {
            StartCoroutine(GetKilledBy(attacker));
        }
    }

    public override void SetAlphaStatus(bool status)
    {
        isAlpha = status;
        transform.Find("crown").GetComponent<SpriteRenderer>().enabled = status;
        bumpImmune = status;
    }

    public override IEnumerator DealBumpDamage(Unit target)
    {
        if (target.bumpImmune)
        {
            yield return null;
        }
        else
        {
            yield return StartCoroutine(target.TakeDamageFrom(null, 2, 0));
        }
    }
}
