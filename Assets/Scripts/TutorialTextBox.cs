using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialTextBox : MonoBehaviour
{
    private enum Condition { starting, unitEndTurn, unitStartTurn, unitDie, tileHasUnit, tileOccupied }
    [SerializeField] private Condition appearCondition;
    [SerializeField] private Condition disappearCondition;
    [SerializeField] private Unit appearCondUnit;
    [SerializeField] private Unit disappearCondUnit;
    [SerializeField] private List<GridTile> conditionTiles = new List<GridTile>();
    private UnitTracker unitTracker;
    private SpriteRenderer background;
    private BoxCollider2D[] boxColliders;
    private Text infoText;
    private bool appearUnitActive;
    private bool disappearUnitActive;
    private bool cursorWithinBounds = false;
    private AIControl ai;

    private Cursor cursor;

    void Awake()
    {
        infoText = GetComponent<Text>();
        background = GetComponentInChildren<SpriteRenderer>();
        boxColliders = GetComponentsInChildren<BoxCollider2D>();
        unitTracker = GameObject.Find("UnitTracker").GetComponent<UnitTracker>();
        ai = GameObject.Find("AIController").GetComponent<AIControl>();
        cursor = GameObject.Find("Cursor").GetComponent<Cursor>();
    }
    // Start is called before the first frame update
    void Start()
    {
        background.enabled = false;
        foreach(BoxCollider2D boxCollider in boxColliders)
        {
            boxCollider.enabled = false;
        }
        infoText.enabled = false;
        appearUnitActive = false;
        if (appearCondition == Condition.starting)
        {
            background.enabled = true;
            foreach (BoxCollider2D boxCollider in boxColliders)
            {
                boxCollider.enabled = true;
            }
            infoText.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (background.enabled && cursor.CheckForTutorialBox() == this && !cursorWithinBounds)
        {
            cursorWithinBounds = true;
            StartCoroutine(Fade());
        }

        if (unitTracker.activeUnit == appearCondUnit || ai.aiUnits.Contains(appearCondUnit))
        {
            appearUnitActive = true;
        }
        if (unitTracker.activeUnit == disappearCondUnit || ai.aiUnits.Contains(disappearCondUnit))
        {
            disappearUnitActive = true;
        }

        switch (appearCondition)
        {
            case Condition.unitStartTurn:
                if (unitTracker.activeUnit == appearCondUnit)
                {
                    background.enabled = true;
                    foreach (BoxCollider2D boxCollider in boxColliders)
                    {
                        boxCollider.enabled = true;
                    }
                    infoText.enabled = true;
                }
                break;
            case Condition.unitEndTurn:
                if (appearUnitActive && unitTracker.activeUnit == null && !ai.aiUnits.Contains(appearCondUnit))
                {
                    background.enabled = true;
                    foreach (BoxCollider2D boxCollider in boxColliders)
                    {
                        boxCollider.enabled = true;
                    }
                    infoText.enabled = true;
                }
                break;
            case Condition.unitDie:
                if (appearCondUnit.currentHealth == 0)
                {
                    background.enabled = true;
                    foreach (BoxCollider2D boxCollider in boxColliders)
                    {
                        boxCollider.enabled = true;
                    }
                    infoText.enabled = true;
                }
                break;
            case Condition.tileHasUnit:
                if (conditionTiles.Contains(appearCondUnit.currentTile) && appearUnitActive && unitTracker.activeUnit == null && !ai.aiUnits.Contains(appearCondUnit))
                {
                    background.enabled = true;
                    foreach (BoxCollider2D boxCollider in boxColliders)
                    {
                        boxCollider.enabled = true;
                    }
                    infoText.enabled = true;
                }
                break;
            case Condition.tileOccupied:
                bool occupied = false;
                foreach (Unit unit in unitTracker.unitList)
                {
                    if (conditionTiles.Contains(unit.currentTile)) occupied = true;
                }
                if (occupied)
                {
                    background.enabled = true;
                    foreach (BoxCollider2D boxCollider in boxColliders)
                    {
                        boxCollider.enabled = true;
                    }
                    infoText.enabled = true;
                }               
                break;
            default:
                break;
        }

        switch (disappearCondition)
        {
            case Condition.unitStartTurn:
                if (unitTracker.activeUnit == disappearCondUnit)
                {
                    background.enabled = false;
                    foreach (BoxCollider2D boxCollider in boxColliders)
                    {
                        boxCollider.enabled = false;
                    }
                    infoText.enabled = false;
                    this.enabled = false;
                }
                break;
            case Condition.unitEndTurn:
                if (disappearUnitActive && unitTracker.activeUnit == null && !ai.aiUnits.Contains(disappearCondUnit))
                {
                    background.enabled = false;
                    foreach (BoxCollider2D boxCollider in boxColliders)
                    {
                        boxCollider.enabled = false;
                    }
                    infoText.enabled = false;
                    this.enabled = false;
                }
                break;
            case Condition.unitDie:
                if (disappearCondUnit.currentHealth == 0)
                {
                    background.enabled = false;
                    foreach (BoxCollider2D boxCollider in boxColliders)
                    {
                        boxCollider.enabled = false;
                    }
                    infoText.enabled = false;
                    this.enabled = false;
                }
                break;
            case Condition.tileHasUnit:
                if (conditionTiles.Contains(disappearCondUnit.currentTile) && disappearUnitActive && unitTracker.activeUnit == null && !ai.aiUnits.Contains(disappearCondUnit))
                {
                    background.enabled = false;
                    foreach (BoxCollider2D boxCollider in boxColliders)
                    {
                        boxCollider.enabled = false;
                    }
                    infoText.enabled = false;
                    this.enabled = false;
                }
                break;
            case Condition.tileOccupied:
                bool occupied = false;
                foreach (Unit unit in unitTracker.unitList)
                {
                    if (conditionTiles.Contains(unit.currentTile)) occupied = true;
                }
                if (occupied)
                {
                    background.enabled = false;
                    foreach (BoxCollider2D boxCollider in boxColliders)
                    {
                        boxCollider.enabled = false;
                    }
                    infoText.enabled = false;
                    this.enabled = false;
                }
                break;
            default:
                break;
        }
    }

    IEnumerator Fade()
    {
        Color originalColor = background.color;
        Color originalTextColor = infoText.color;
        float fadeDuration = 0.3f;
        float startTime = Time.time;
        while (cursor.CheckForTutorialBox() == this)
        {
            while (Time.time < startTime + fadeDuration) 
            { 
                background.color = Color.Lerp(originalColor, new Color(0f, 0f, 0f, 0.25f), (Time.time - startTime) / fadeDuration);
                infoText.color = Color.Lerp(originalTextColor, new Color(1f, 1f, 1f, 0.25f), (Time.time - startTime) / fadeDuration);
                if (cursor.CheckForTutorialBox() != this) break;
                yield return null;
            }
            yield return null;
        }
        background.color = originalColor;
        infoText.color = originalTextColor;
        cursorWithinBounds = false;
    }
}
