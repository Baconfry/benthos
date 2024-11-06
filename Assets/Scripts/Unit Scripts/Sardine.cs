using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sardine : Unit
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

        baseTurnCooldown = 35;
        damageValue = 1;
        attackPenalty = 20;

        internalID = -1;

        baseMoveRange = 4;
        maxHealth = 2;
    }
}
