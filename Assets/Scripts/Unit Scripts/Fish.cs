using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : Unit
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

        baseTurnCooldown = 15;
        damageValue = 2;
        attackPenalty = 30;

        internalID = 0;

        baseMoveRange = 5;
        maxHealth = 6;
    }

    public override void SetAlphaStatus(bool status)
    {
        isAlpha = status;
        transform.Find("crown").GetComponent<SpriteRenderer>().enabled = status;
        attackPenalty = isAlpha ? 20 : 30;
        baseMoveRange = isAlpha ? 6 : 5;
        currentMoveRange = baseMoveRange;
    }
}
