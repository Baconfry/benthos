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
        damageValue = 3;
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
        //digitDisplay.ChangeDisplayTo(currentHealth);
    }

    public override IEnumerator Attack(GridTile targetTile)
    {
        yield return null;
    }

    public override List<GridTile> GetAttackableTiles()
    {
        List<GridTile> attackableTiles = new List<GridTile>();
        return attackableTiles;
    }

    public override List<GridTile> GetDestinationForAI(GridTile targetTile)
    {
        List<GridTile> destinations = new List<GridTile>();
        return destinations;
    }

    protected override IEnumerator GetKilledBy(Unit attacker) //reward attacker for kill, if applicable
    {
        while (isMoving)
        {
            yield return null;
        }

        //yield return new WaitForSeconds(Settings.TurnDelay);
        while (attacker != null && attacker.isMoving) yield return null;

        //Instantiate(attackAnimation, currentTile.transform.position, transform.rotation);
        soundEffects.clip = attackSound;
        soundEffects.Play();

        foreach (GridTile tile in currentTile.Get8SurroundingTiles(currentTile))
        {
            Instantiate(attackAnimation, tile.transform.position, transform.rotation);
            if (tile.occupyingUnit != null)
            {
                yield return StartCoroutine(tile.occupyingUnit.TakeDamageFrom(null, damageValue, 0));
            }
            if (tile.tileType == TileType.coral)
            {
                tile.ChangeTileTo(TileType.stone);
            }
            if (tile.tileType == TileType.fireCoral)
            {
                tile.DecaySandPile(20);
            }
        }

        yield return new WaitForSeconds(GetAttackDelay());

        GameObject.Find("UnitTracker").GetComponent<UnitTracker>().RemoveUnit(this);
        foreach (SpriteRenderer spriteRenderer in GetComponentsInChildren<SpriteRenderer>())
        {
            spriteRenderer.enabled = false;
        }
        yield return new WaitForSeconds(Settings.TurnDelay);
        //transform.position = new Vector3(99f, 99f, 0f); //incredibly cursed unit removal method
        //this.enabled = false;
        Destroy(this.gameObject, 1f);
    }

}
