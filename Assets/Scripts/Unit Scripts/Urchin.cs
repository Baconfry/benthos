using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Urchin : Unit
{
    protected override void Initialize()
    {
        movementType = MoveType.benthic;
        armorType = ArmorType.none;
        isArmored = false;
        isAnchored = false;
        isPoisoned = false;
        healOnKill = false;
        makesContact = false;
        canMove = false;

        baseTurnCooldown = 50;
        damageValue = 0;
        attackPenalty = 50;

        internalID = 16;

        baseMoveRange = 0;
        maxHealth = 1;
    }

    public override void SetAlphaStatus(bool status)
    {
        isAlpha = status;
        transform.Find("crown").GetComponent<SpriteRenderer>().enabled = status;
        /*attackPenalty = isAlpha ? 20 : 30;
        baseMoveRange = isAlpha ? 6 : 5;
        currentMoveRange = baseMoveRange;*/
    }

    public override void UpdateIcons()
    {
        digitDisplay.ChangeDisplayTo(currentHealth);
    }

    void OnDestroy()
    {
        
    }

}
