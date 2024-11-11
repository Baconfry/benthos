using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HermitCrab : Unit
{
    public bool changedForm;
    private bool formChangeMoveImmune;
    [SerializeField] private Sprite nakedSprite;
    [SerializeField] private AudioClip breakingSound;
    
    protected override void Initialize()
    {
        movementType = MoveType.benthic;
        armorType = ArmorType.none;
        isArmored = true;
        isAnchored = false;
        isPoisoned = false;
        healOnKill = false;
        makesContact = true;

        changedForm = false;
        formChangeMoveImmune = false;

        baseTurnCooldown = 35;
        damageValue = 2;
        attackPenalty = 40;

        internalID = 14;

        baseMoveRange = 3;
        maxHealth = 4;
    }

    public override void StartTurn()
    {
        if (!armorBroken) isArmored = true;
        formChangeMoveImmune = false;
        UpdateIcons();
    }

    public override IEnumerator ApplyTileEffects()
    {
        if (currentMoveRange == 1 && movementType == MoveType.benthic && currentTile.tileType != TileType.trench) currentMoveRange = baseMoveRange;
        switch (currentTile.tileType)
        {
            case TileType.stone:
            case TileType.algae:
            case TileType.sand:
                break;
            case TileType.current:
                isMoving = true;
                yield return StartCoroutine("ForcedMoveTo", currentTile.GetFacingTile(currentTile));
                break;
            case TileType.anemone:
                if (changedForm) DirectPoison();
                break;
            case TileType.vent:
                Instantiate(currentTile.lavaAnimation, transform.position, transform.rotation);
                yield return StartCoroutine(TakeDamageFrom(null, 10, 0));
                break;
            case TileType.outcrop:
            case TileType.coral:
            case TileType.fireCoral:
                Debug.Log("unit is standing on inaccessible tile");
                break;
            case TileType.trench:
                if (movementType == MoveType.benthic)
                {
                    if (currentMoveRange > 1) currentMoveRange = 1;
                }
                break;
            default:
                Debug.Log("attempted to reference invalid type");
                break;
        }
        UpdateIcons();
    }

    public override bool IsValidForAI(GridTile tile)
    {
        if (tile.tileType == TileType.vent)
        {
            return false;
        }
        else if (tile.tileType == TileType.anemone)
        {
            return !changedForm;
        }
        else if (!isAnchored && tile.tileType == TileType.current && tile.GetFacingTile(tile).occupyingUnit != null && tile.GetFacingTile(tile).occupyingUnit.playerID == playerID && tile.GetFacingTile(tile).occupyingUnit != this)
        {
            return false;
        }
        else if (changedForm && tile.tileType == TileType.current && (tile.GetFacingTile(tile).occupyingUnit != null || tile.GetFacingTile(tile).IsSolid()) && tile.GetFacingTile(tile).occupyingUnit != this)
        {
            return false;
        }
        else if (!isAnchored && tile.tileType == TileType.current && tile.GetFacingTile(tile).tileType != TileType.current && tile.GetFacingTile(tile).occupyingUnit == null)
        {
            return IsValidForAI(tile.GetFacingTile(tile));
        }
        else
        {
            return true;
        }
    }

    public override IEnumerator Heal(int healingAmount)
    {
        yield return null;
        //Instantiate(damageNumber, transform.position, transform.rotation).GetComponent<DamageNumber>().value = 0;
        if (healingAmount < 0)
        {
            Instantiate(damageNumber, transform.position, transform.rotation).GetComponent<DamageNumber>().value = -healingAmount;
            currentHealth += healingAmount;
        }

        if (currentHealth > 0)
        {
            digitDisplay.ChangeDisplayTo(currentHealth);
        }
        else
        {
            yield return StartCoroutine(GetKilledBy(null));
        }
    }

    public override IEnumerator ForcedMoveTo(GridTile destination)
    {
        bool startedInTrench = false;
        if (currentTile.tileType == TileType.trench) startedInTrench = true;
        if (formChangeMoveImmune)
        {
            isMoving = false;
            yield break;
        }
        if (isAnchored && currentTile.tileType != TileType.sand && currentTile.tileType != TileType.trench)
        {
            isMoving = false;
            yield break;
        }
        if (destination == null)
        {
            isMoving = false;
            yield break;
        }
        if (destination.tileType == TileType.outcrop || destination.tileType == TileType.coral)
        {
            yield return StartCoroutine("BumpAgainst", destination);
            yield return StartCoroutine(ReceiveBumpDamage(null));
            if (currentTile.tileType == TileType.anemone && !formChangeMoveImmune) yield return StartCoroutine(ApplyTileEffects()); //testing
        }
        else if (destination.tileType == TileType.fireCoral)
        {
            yield return StartCoroutine("BumpAgainst", destination);
            yield return StartCoroutine(ReceiveBumpDamage(null));
            if (currentTile.tileType == TileType.anemone && !formChangeMoveImmune) yield return StartCoroutine(ApplyTileEffects()); //testing
            destination.DecaySandPile(20);
        }
        else if (destination.occupyingUnit == null)
        {
            if (currentTile.CanMoveTo(destination, this) || destination.tileType == TileType.trench)
            {
                currentTile.occupyingUnit = null;
                yield return StartCoroutine("MoveTo", destination);
                currentTile = destination;
                currentTile.occupyingUnit = this;
                yield return StartCoroutine(ApplyTileEffects());
            }
        }
        else if (destination.occupyingUnit != null)
        {
            yield return StartCoroutine("BumpAgainst", destination);
            yield return StartCoroutine(ReceiveBumpDamage(destination.occupyingUnit));
            if (currentTile.tileType == TileType.anemone && !formChangeMoveImmune) yield return StartCoroutine(ApplyTileEffects()); //testing
            yield return StartCoroutine(DealBumpDamage(destination.occupyingUnit));
        }
        else
        {
            Debug.Log("attempted to forced move but nothing happened");
        }
        if (movementType == MoveType.benthic && currentTile.tileType != TileType.trench && startedInTrench) currentMoveRange = baseMoveRange;
        isMoving = false;
    }

    public override IEnumerator TakeDamageFrom(Unit attacker, int damage, int poisonValue)
    {
        yield return null;
        formChangeMoveImmune = false;
        int finalDamage = damage;
        if (isArmored) finalDamage--;
        if (finalDamage > 1 && finalDamage < 9) finalDamage = 1;
        //if (attacker != null && attacker.GetComponent<MantisShrimp>() != null && currentHealth > 2) finalDamage = attacker.damageValue;
        if (finalDamage > currentHealth) finalDamage = currentHealth;
        if (currentHealth > 0) Instantiate(damageNumber, transform.position, transform.rotation).GetComponent<DamageNumber>().value = finalDamage;
        currentHealth -= finalDamage;
        isArmored = false;
        UpdateIcons();

        if (currentHealth == 0)
        {
            StartCoroutine(GetKilledBy(attacker));
        }
        else if (currentHealth == 1)
        {
            ChangeForm();
            UpdateIcons();
        }
    }

    private void ChangeForm()
    {
        selectOutline.enabled = false;
        selectOutline = transform.Find("exposedOutline").gameObject.GetComponent<SpriteRenderer>();
        baseTurnCooldown = 15;
        currentTurnCooldown = currentTurnCooldown % 5;
        attackPenalty = 10;
        baseMoveRange = 5;
        internalID = -2;
        //damageValue = 2;
        currentMoveRange = baseMoveRange;
        armorBroken = true;
        isAnchored = false;
        spriteRenderer.sprite = nakedSprite;
        changedForm = true;
        formChangeMoveImmune = true;
        soundEffects.clip = breakingSound;
        soundEffects.Play();
    }

    public override void DirectPoison()
    {
        if (changedForm) isPoisoned = true;
    }

    public override void SetAlphaStatus(bool status)
    {
        isAlpha = status;
        transform.Find("crown").GetComponent<SpriteRenderer>().enabled = status;
        maxHealth = isAlpha ? 5 : 4;
        currentHealth = maxHealth;
        UpdateIcons();
        isAnchored = isAlpha;
    }
}
