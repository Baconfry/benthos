using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobbitWorm : Unit
{
    protected override void Initialize()
    {
        movementType = MoveType.benthic;
        armorType = ArmorType.light;
        isArmored = true;
        isAnchored = true;
        isPoisoned = false;
        healOnKill = true;
        makesContact = true;

        baseTurnCooldown = 30;
        damageValue = 4;
        attackPenalty = 40;

        internalID = 15;

        baseMoveRange = 1;
        maxHealth = 6;
    }

    public override IEnumerator ForcedMoveTo(GridTile destination)
    {
        yield return null;
        isMoving = false;
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
}
