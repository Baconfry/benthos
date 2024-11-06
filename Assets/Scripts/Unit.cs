using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : Definitions
{
    protected SpriteRenderer spriteRenderer;

    public MoveType movementType;
    public ArmorType armorType;
    public bool isArmored;
    public bool isAnchored;
    public bool isPoisoned;
    public bool armorBroken;
    public bool healOnKill;
    public bool makesContact;

    public bool isMoving;
    public bool canMove = true;
    
    public bool bumpImmune = false;
    public bool passThroughEnemies = false;
    public bool isAlpha = false;

    public int playerID;
    public int baseMoveRange;
    public int currentMoveRange;
    public int maxHealth;
    public int currentHealth;
    public int damageValue;

    public int internalID;

    public int baseTurnCooldown;
    public int currentTurnCooldown;
    public int attackPenalty;

    public GridTile currentTile;
    protected SpriteRenderer armorIcon;
    protected SpriteRenderer poisonIcon;
    protected DigitDisplay digitDisplay;
    protected DigitDisplay cooldownDisplay1;
    protected DigitDisplay cooldownDisplay2;
    protected CustomGrid grid;
    protected SpriteRenderer constrictIcon;
    protected SpriteRenderer brokenIcon;
    protected SpriteRenderer selectOutline;

    [SerializeField] protected GameObject attackAnimation;
    [SerializeField] protected GameObject slowAnimation;
    [SerializeField] protected GameObject damageNumber;
    protected GameObject waitButton;
    protected AudioSource soundEffects;
    [SerializeField] protected AudioClip attackSound;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        armorIcon = transform.Find("shield_icon").gameObject.GetComponent<SpriteRenderer>();
        poisonIcon = transform.Find("poison_icon").gameObject.GetComponent<SpriteRenderer>();
        constrictIcon = transform.Find("constrict").gameObject.GetComponent<SpriteRenderer>();
        brokenIcon = transform.Find("broken_icon").gameObject.GetComponent<SpriteRenderer>();
        digitDisplay = transform.Find("health_icon").gameObject.GetComponent<DigitDisplay>();
        cooldownDisplay1 = transform.Find("CD_display1").gameObject.GetComponent<DigitDisplay>();
        cooldownDisplay2 = transform.Find("CD_display2").gameObject.GetComponent<DigitDisplay>();
        selectOutline = transform.Find("outline").gameObject.GetComponent<SpriteRenderer>();
        waitButton = transform.Find("wait").gameObject;
        soundEffects = GetComponent<AudioSource>();

        try 
        { 
            GameObject.Find("UnitTracker").GetComponent<UnitTracker>().AddUnit(this); 
        }
        catch
        {
            spriteRenderer.enabled = false;
            digitDisplay.gameObject.SetActive(false);
            armorIcon.gameObject.SetActive(false);
            cooldownDisplay1.gameObject.SetActive(false);
            cooldownDisplay2.gameObject.SetActive(false);
            this.enabled = false;
            yield break;
        }
        grid = GameObject.FindWithTag("Grid").GetComponent<CustomGrid>();
        Initialize();
        currentTurnCooldown = baseTurnCooldown;
        currentTurnCooldown += playerID; //ensures that opposing units will never be active at the same time. playerID must be between 0 and 4
        currentMoveRange = baseMoveRange;
        currentHealth = maxHealth;      
        yield return null;
        UpdateIcons();
        //EnableOutline(false);
        digitDisplay.AssignTeamColor(playerID);
        yield return null;
        currentTile = grid.gridTiles[(int)transform.localPosition.x + (int)grid.xLength / 2, (int)transform.localPosition.y + (int)grid.yLength / 2];
        currentTile.occupyingUnit = this;
        if (GameObject.Find("UnitTracker").GetComponent<UnitTracker>().humanPlayerID == playerID)
        {
            cooldownDisplay1.matchesPlayerID = true;
            cooldownDisplay2.matchesPlayerID = true;
        }
    }

    protected virtual void Initialize()
    {
        movementType = MoveType.benthic;
        armorType = ArmorType.none;
        isArmored = false;
        isAnchored = false;
        isPoisoned = false;
        healOnKill = false;
        armorBroken = false;
        makesContact = true;

        baseTurnCooldown = 99;
        attackPenalty = 0;

        baseMoveRange = 1;
        maxHealth = 1;
        Debug.Log("Initialized with default parameters");
    }

    public virtual void UpdateIcons()
    {
        if (currentHealth > 0)
        {
            armorIcon.enabled = isArmored;
            brokenIcon.enabled = armorBroken;
            poisonIcon.enabled = isPoisoned;
            constrictIcon.enabled = currentMoveRange == 0;
        }
        else
        {
            armorIcon.enabled = false;
            brokenIcon.enabled = false;
            poisonIcon.enabled = false;
            constrictIcon.enabled = false;
        }
        digitDisplay.ChangeDisplayTo(currentHealth);

        cooldownDisplay1.ChangeDisplayTo(currentTurnCooldown);
        cooldownDisplay2.ChangeDisplayTo(currentTurnCooldown);
        
        if (currentTurnCooldown == 0)
        {
            cooldownDisplay1.Highlight(true);
            cooldownDisplay2.Highlight(true);
            //selectOutline.enabled = true;
        }
        else
        {
            cooldownDisplay1.Highlight(false);
            cooldownDisplay2.Highlight(false);
            //selectOutline.enabled = false;
        }
    }

    public void EnableOutline(bool status)
    {
        selectOutline.enabled = status;
    }

    public virtual void StartTurn()
    {

    }

    public virtual IEnumerator EndTurn()
    {
        yield return null;
        ResetLoweredMovement();
        yield return StartCoroutine(ApplyTileEffects());
        if (isPoisoned)
        {
            yield return new WaitForSeconds(Settings.TurnDelay);
            yield return StartCoroutine("Heal", -1);
        }
    }

    public virtual float GetAttackDelay()
    {
        return attackAnimation.GetComponent<AttackAnim>().GetTotalAnimationTime();
    }

    public List<Unit> GetBorderingUnits(GridTile targetTile) //for torpedo ray
    {
        List<Unit> borderingUnits = new List<Unit>();
        if (targetTile.GetTileAbove(targetTile) != null && targetTile.GetTileAbove(targetTile).occupyingUnit != null && targetTile.GetTileAbove(targetTile).occupyingUnit != this) borderingUnits.Add(targetTile.GetTileAbove(targetTile).occupyingUnit);
        if (targetTile.GetTileBelow(targetTile) != null && targetTile.GetTileBelow(targetTile).occupyingUnit != null && targetTile.GetTileBelow(targetTile).occupyingUnit != this) borderingUnits.Add(targetTile.GetTileBelow(targetTile).occupyingUnit);
        if (targetTile.GetTileRight(targetTile) != null && targetTile.GetTileRight(targetTile).occupyingUnit != null && targetTile.GetTileRight(targetTile).occupyingUnit != this) borderingUnits.Add(targetTile.GetTileRight(targetTile).occupyingUnit);
        if (targetTile.GetTileLeft(targetTile) != null && targetTile.GetTileLeft(targetTile).occupyingUnit != null && targetTile.GetTileLeft(targetTile).occupyingUnit != this) borderingUnits.Add(targetTile.GetTileLeft(targetTile).occupyingUnit);


        return borderingUnits;
    }

    public bool TileTypeWithinRange(TileType type)
    {
        bool foundTile = false;
        foreach(GridTile tile in currentTile.GetTilesInRange(this, new Color(1f, 0.85f, 0.15f, 1f)))
        {
            if (tile.tileType == type && tile.occupyingUnit == null) foundTile = true;
        }
        return foundTile;
    }

    public void ResetLoweredMovement()
    {
        if (currentTile.tileType == TileType.trench && movementType == MoveType.benthic)
        {
            currentMoveRange = 1;
        }
        else
        {
            currentMoveRange = baseMoveRange;
        }
    }

    public void DelayTurnBy(int value)
    {
        currentTurnCooldown += value;
        if (currentTurnCooldown < 0) currentTurnCooldown = 0;
        if (currentTurnCooldown >= 100) currentTurnCooldown = 95 + (currentTurnCooldown % 5);
        UpdateIcons();
    }

    public virtual void SetAlphaStatus(bool status)
    {
        isAlpha = status;
        transform.Find("crown").GetComponent<SpriteRenderer>().enabled = status;
    }

    public virtual IEnumerator ApplyTileEffects()
    {
        if (currentMoveRange == 1 && movementType == MoveType.benthic && currentTile.tileType != TileType.trench) currentMoveRange = baseMoveRange;
        switch (currentTile.tileType)
        {
            case TileType.stone:
                break;
            case TileType.algae:
                if (currentHealth < maxHealth)
                {
                    yield return StartCoroutine("Heal", 1);
                    currentTile.ChangeTileTo(TileType.stone);
                }
                break;
            case TileType.sand:
                break;
            case TileType.current:
                yield return StartCoroutine ("ForcedMoveTo", currentTile.GetFacingTile(currentTile));
                break;
            case TileType.anemone:
                if (!isArmored)
                {
                    DirectPoison();
                }
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

    public virtual IEnumerator Attack(GridTile targetTile)
    {
        Instantiate(attackAnimation, targetTile.transform.position, transform.rotation);
        soundEffects.clip = attackSound;
        soundEffects.Play();
        if (targetTile.occupyingUnit != null)
        {
            yield return StartCoroutine(targetTile.occupyingUnit.TakeDamageFrom(this, damageValue, 0));
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

    public virtual List<GridTile> GetAttackableTiles()
    {

        List<GridTile> attackableTiles = new List<GridTile>();
        if (currentTile.GetTileAbove(currentTile) != null)
        {
            attackableTiles.Add(currentTile.GetTileAbove(currentTile));
        }

        if (currentTile.GetTileBelow(currentTile) != null)
        {
            attackableTiles.Add(currentTile.GetTileBelow(currentTile));
        }

        if (currentTile.GetTileRight(currentTile) != null)
        {
            attackableTiles.Add(currentTile.GetTileRight(currentTile));
        }

        if (currentTile.GetTileLeft(currentTile) != null)
        {
            attackableTiles.Add(currentTile.GetTileLeft(currentTile));
        }
        if (currentTile.tileType == TileType.trench && movementType == MoveType.benthic) attackableTiles.Clear();
        return attackableTiles;
    }

    public virtual List<GridTile> GetDestinationForAI(GridTile targetTile)
    {
        List<GridTile> destinations = new List<GridTile>();
        if (targetTile.GetTileAbove(targetTile) != null && (targetTile.GetTileAbove(targetTile).occupyingUnit == null || targetTile.GetTileAbove(targetTile).occupyingUnit == this) && currentTile.CanMoveTo(targetTile.GetTileAbove(targetTile), this))
        {
            destinations.Add(targetTile.GetTileAbove(targetTile));
        }

        if (targetTile.GetTileBelow(targetTile) != null && (targetTile.GetTileBelow(targetTile).occupyingUnit == null || targetTile.GetTileBelow(targetTile).occupyingUnit == this) && currentTile.CanMoveTo(targetTile.GetTileBelow(targetTile), this))
        {
            destinations.Add(targetTile.GetTileBelow(targetTile));
        }

        if (targetTile.GetTileRight(targetTile) != null && (targetTile.GetTileRight(targetTile).occupyingUnit == null || targetTile.GetTileRight(targetTile).occupyingUnit == this) && currentTile.CanMoveTo(targetTile.GetTileRight(targetTile), this))
        {
            destinations.Add(targetTile.GetTileRight(targetTile));
        }

        if (targetTile.GetTileLeft(targetTile) != null && (targetTile.GetTileLeft(targetTile).occupyingUnit == null || targetTile.GetTileLeft(targetTile).occupyingUnit == this) && currentTile.CanMoveTo(targetTile.GetTileLeft(targetTile), this))
        {
            destinations.Add(targetTile.GetTileLeft(targetTile));
        }

        return destinations;
    }

    public virtual bool IsValidForAI(GridTile tile)
    {
        if (tile.tileType == TileType.vent)
        {
            if (currentHealth == 1 && isPoisoned)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (tile.tileType == TileType.anemone)
        {
            if (isArmored || isPoisoned)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (!isAnchored && tile.tileType == TileType.current && tile.GetFacingTile(tile).occupyingUnit != null && tile.GetFacingTile(tile).occupyingUnit.playerID == playerID && tile.GetFacingTile(tile).occupyingUnit != this)
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

    public virtual List<Unit> AssignAIPriority(List<Unit> candidates)
    {
        int[] priorityArray = new int[candidates.Count];
        for(int i = 0; i < candidates.Count; i++)
        {
            priorityArray[i] = 0;
            priorityArray[i] += damageValue;
            if (candidates[i].isArmored && candidates[i].armorType == ArmorType.heavy) priorityArray[i]--;
            if ((candidates[i].isArmored ? damageValue - 1 : damageValue) >= candidates[i].currentHealth) priorityArray[i] += 20;
            //Debug.Log(candidates[i].gameObject.name + " " + priorityArray[i]);
        }
        List<Unit> finalCandidates = new List<Unit>();
        //finalCandidates.Add(candidates[0]);
        int startingPriority = 0;
        for (int i = 0; i < priorityArray.Length; i++)
        {
            if (priorityArray[i] > startingPriority)
            {
                finalCandidates.Clear();
                finalCandidates.Add(candidates[i]);
                startingPriority = priorityArray[i];
            }
            else if (priorityArray[i] == startingPriority)
            {
                finalCandidates.Add(candidates[i]);
            }
        }
        //if (finalCandidates.Count == 0) finalCandidates.Add(candidates[0]);
        /*foreach (Unit candidate in finalCandidates)
        {
            candidate.currentTile.SetOutlinerActive(true, Color.white);
        }*/
        return finalCandidates;
    }

    public virtual IEnumerator TakeDamageFrom(Unit attacker, int damage, int poisonValue)
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

        if (currentHealth == 0)
        {
            StartCoroutine(GetKilledBy(attacker));
        }
    }

    public IEnumerator MoveTo(GridTile destination)
    {
        Vector3 startingPosition = transform.position;
        float startTime = Time.time;
        float totalAnimationTime = Mathf.Sqrt(Settings.TurnDelay) * 0.2f;
        while (Time.time < startTime + totalAnimationTime)
        {
            transform.position = Vector3.Lerp(startingPosition, destination.transform.position, (Time.time - startTime) / totalAnimationTime);
            yield return null;
        }
        transform.position = destination.transform.position;
        //yield return new WaitForSeconds(0.02f);
    }

    public IEnumerator BumpAgainst(GridTile destination) //handles visual movement only
    {
        Vector3 startingPosition = transform.position;
        float startTime = Time.time;
        float totalAnimationTime = 0.1f;
        while (Time.time < startTime + totalAnimationTime)
        {
            transform.position = Vector3.Lerp(startingPosition, destination.transform.position, (Time.time - startTime) / totalAnimationTime);
            yield return null;
        }
        startTime = Time.time;
        while (Time.time < startTime + totalAnimationTime)
        {
            transform.position = Vector3.Lerp(destination.transform.position, startingPosition, (Time.time - startTime) / totalAnimationTime);
            yield return null;
        }
        transform.position = startingPosition;
    }

    public IEnumerator FollowPath(List<GridTile> path)
    {
        //yield return null;
        cooldownDisplay1.Highlight(false);
        cooldownDisplay2.Highlight(false);
        currentTile.occupyingUnit = null;
        for (int i = 0; i < path.Count; i++)
        {
            yield return StartCoroutine("MoveTo", path[i]);
            currentTile = path[i];
            //Debug.Log(path[i]);
        }
        
        currentTile.occupyingUnit = this;
    }

    public virtual IEnumerator ForcedMoveTo(GridTile destination)
    {
        bool startedInTrench = false;
        if (currentTile.tileType == TileType.trench) startedInTrench = true;
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
            if (currentTile.tileType == TileType.anemone) yield return StartCoroutine(ApplyTileEffects()); //testing
        }
        else if (destination.tileType == TileType.fireCoral)
        {
            yield return StartCoroutine("BumpAgainst", destination);
            yield return StartCoroutine(ReceiveBumpDamage(null));
            if (currentTile.tileType == TileType.anemone) yield return StartCoroutine(ApplyTileEffects()); //testing
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
            if (currentTile.tileType == TileType.anemone) yield return StartCoroutine(ApplyTileEffects()); //testing
            yield return StartCoroutine(DealBumpDamage(destination.occupyingUnit));
        }
        else
        {
            Debug.Log("attempted to forced move but nothing happened");
        }
        if (movementType == MoveType.benthic && currentTile.tileType != TileType.trench && startedInTrench) currentMoveRange = baseMoveRange;
        isMoving = false;
    }

    public virtual IEnumerator DealBumpDamage(Unit target)
    {
        if (target.bumpImmune)
        {
            yield return null;
        }
        else
        {
            yield return StartCoroutine(target.TakeDamageFrom(null, 1, 0));
        }

    }

    public virtual IEnumerator ReceiveBumpDamage(Unit otherUnit)
    {
        if (bumpImmune)
        {
            yield return null;
        }
        else if (otherUnit == null)
        {
            yield return StartCoroutine(TakeDamageFrom(null, 1, 0));
        }
        else
        {
            yield return StartCoroutine(otherUnit.DealBumpDamage(this));
        }
    }

    public virtual void DirectPoison()
    {
        isPoisoned = true;
    }

    public void ChangeColorTo(Color newColor)
    {
        try
        {
            spriteRenderer.color = newColor;
        }
        catch { }
    }

    public virtual IEnumerator Heal(int healingAmount) //use for heals or for defense-ignoring damage
    {
        yield return null;
        if (currentHealth > 0)
        {
            Instantiate(damageNumber, transform.position, transform.rotation).GetComponent<DamageNumber>().value = -healingAmount;
            currentHealth += healingAmount;
        }
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
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

    protected IEnumerator GetKilledBy(Unit attacker) //reward attacker for kill, if applicable
    {
        while (isMoving)
        {
            yield return null;
        }

        if (attacker != null && attacker.healOnKill && attacker.currentHealth < attacker.maxHealth)
        {
            yield return new WaitForSeconds(Settings.TurnDelay);
            yield return attacker.StartCoroutine("Heal", 1);
        }

        if (currentHealth <= 0)
        {
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

    public void SetWaitButtonActive(bool status)
    {
        waitButton.SetActive(status);
    }
}
