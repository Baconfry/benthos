using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cursor : Definitions
{
    //Handles cursor during GAMEPLAY ONLY.
    
    private GridTile activeTile;
    private CustomGrid grid;
    private UnitTracker unitTracker;
    public GameObject[] unitTypes;
    public GameObject selectedUnit;
    public int cursorUnitIndex;
    public GameObject unitPreview;

    private SpriteRenderer autoSprite;
    private SpriteRenderer fastSprite;
    private SpriteRenderer normalSprite;
    private SpriteRenderer slowSprite;
    private SpriteRenderer sandboxSprite;
    private SpriteRenderer turboSprite;
    private SpriteRenderer startSprite;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        unitTracker = GameObject.Find("UnitTracker").GetComponent<UnitTracker>();
        GameObject canvas = GameObject.Find("Canvas");
        autoSprite = canvas.transform.Find("auto").GetComponent<SpriteRenderer>();
        fastSprite = canvas.transform.Find("fast").GetComponent<SpriteRenderer>();
        normalSprite = canvas.transform.Find("normal").GetComponent<SpriteRenderer>();
        slowSprite = canvas.transform.Find("slow").GetComponent<SpriteRenderer>();
        sandboxSprite = canvas.transform.Find("sandbox").GetComponent<SpriteRenderer>();
        turboSprite = canvas.transform.Find("turbo").GetComponent<SpriteRenderer>();
        startSprite = canvas.transform.Find("start").GetComponent<SpriteRenderer>();

        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        grid = GameObject.FindWithTag("Grid").GetComponent<CustomGrid>();
        selectedUnit = unitTypes[cursorUnitIndex];
        UpdateSpeedIcons();
        if (!Settings.SandboxMode)
        {
            sandboxSprite.color = Color.white;
        }
        else
        {
            sandboxSprite.color = new Color(1f, 0.85f, 0.15f, 1f);
        }
        if (!Settings.AutopilotMode)
        {
            autoSprite.color = Color.white;
        }
        else
        {
            autoSprite.color = new Color(1f, 0.85f, 0.15f, 1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        transform.position = mousePos;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.S))
        {
            if (AllowClick()) CheckForOptionChange();
        }

        if (Input.GetMouseButtonDown(2))
        {
            //Debug.Log(GameObject.Find("AIController").GetComponent<AIControl>().aiUnits.Count);
            //Debug.Log(GetActiveTile().sandPileTimer);

            /*foreach (Unit unit in unitTracker.GetAllEnemiesOfID(0))
            {
                Debug.Log(unit.gameObject.name);
            }*/
            try
            {
                Debug.Log(unitTracker.availableUnits[0]);
                //Debug.Log(GetActiveTile().occupyingUnit.currentTile.transform.localPosition.x + " " + GetActiveTile().occupyingUnit.currentTile.transform.localPosition.y);
                //Debug.Log(GetActiveTile().occupyingUnit.transform.localPosition.x + " " + GetActiveTile().occupyingUnit.transform.localPosition.y);
                //Debug.Log(GetActiveTile().occupyingUnit.currentHealth + "/" + GetActiveTile().occupyingUnit.maxHealth + " health");
                /*for (int i = 0; i < GetActiveTile().path.Count; i++)
                {
                    Debug.Log(GetActiveTile().path[i]);
                }*/
                //Debug.Log(GetActiveTile().xCoordinate + ", " + GetActiveTile().yCoordinate);
            }
            catch
            {

            }
        }
    }

    public void UpdateSpeedIcons()
    {
        switch (Settings.TurnDelay)
        {
            case 0.01f:
                turboSprite.color = new Color(1f, 0.85f, 0.15f, 1f);
                fastSprite.color = Color.white;
                normalSprite.color = Color.white;
                slowSprite.color = Color.white;
                break;
            case 0.2f:
                turboSprite.color = Color.white;
                fastSprite.color = new Color(1f, 0.85f, 0.15f, 1f);
                normalSprite.color = Color.white;
                slowSprite.color = Color.white;
                break;
            case 0.5f:
                turboSprite.color = Color.white;
                normalSprite.color = new Color(1f, 0.85f, 0.15f, 1f);
                fastSprite.color = Color.white;
                slowSprite.color = Color.white;
                break;
            case 0.8f:
                turboSprite.color = Color.white;
                slowSprite.color = new Color(1f, 0.85f, 0.15f, 1f);
                fastSprite.color = Color.white;
                normalSprite.color = Color.white;
                break;
            default:
                break;
        }
        //Debug.Log(Settings.TurnDelay);
    }

    public GridTile GetActiveTile()
    {
        GridTile returnTile = null;
        int layerMask = 1 << 3;
        Collider2D collider = Physics2D.OverlapCircle(transform.position, 0.01f, layerMask);
        if (collider != null)
        {
            returnTile = collider.gameObject.GetComponent<GridTile>();
        }
        return returnTile;
    }

    public void ChangeDeployedUnit()
    {
        int layerMask = 1 << 5;
        Collider2D collider = Physics2D.OverlapCircle(transform.position, 0.01f, layerMask);
        if (collider != null && collider.gameObject.GetComponent<UnitSelectButton>() != null)
        {
            cursorUnitIndex = collider.gameObject.GetComponent<UnitSelectButton>().unitIndex;
            selectedUnit = unitTypes[cursorUnitIndex];
            unitPreview.GetComponent<SpriteRenderer>().sprite = selectedUnit.GetComponent<SpriteRenderer>().sprite;
        }
    }

    public string FindButtonName()
    {
        int layerMask = 1 << 5;
        Collider2D collider = Physics2D.OverlapCircle(transform.position, 0.01f, layerMask);
        if (collider != null)
        {
            return collider.gameObject.name;
        }
        else
        {
            return null;
        }
    }

    public string CheckForOptionChange()
    {
        int layerMask = 1 << 5;
        Collider2D collider = Physics2D.OverlapCircle(transform.position, 0.01f, layerMask);
        string buttonName;
        if (collider != null)
        {
            switch (collider.gameObject.name)
            {
                case "auto":
                    if (!Settings.AutopilotMode)
                    {
                        Settings.AutopilotMode = true;
                        //Debug.Log("enabled autopilot");
                        collider.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 0.85f, 0.15f, 1f);
                    }
                    else
                    {
                        Settings.AutopilotMode = false;
                        //Debug.Log("disabled autopilot");
                        collider.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                    }
                    break;
                case "turbo":
                    turboSprite.color = new Color(1f, 0.85f, 0.15f, 1f);
                    fastSprite.color = Color.white;
                    normalSprite.color = Color.white;
                    slowSprite.color = Color.white;
                    Settings.TurnDelay = 0.01f;
                    break;
                case "fast":
                    turboSprite.color = Color.white;
                    fastSprite.color = new Color(1f, 0.85f, 0.15f, 1f);
                    normalSprite.color = Color.white;
                    slowSprite.color = Color.white;
                    Settings.TurnDelay = 0.2f;
                    break;
                case "normal":
                    turboSprite.color = Color.white;
                    fastSprite.color = Color.white;
                    normalSprite.color = new Color(1f, 0.85f, 0.15f, 1f);
                    slowSprite.color = Color.white;
                    Settings.TurnDelay = 0.5f;
                    break;
                case "slow":
                    turboSprite.color = Color.white;
                    fastSprite.color = Color.white;
                    normalSprite.color = Color.white;
                    slowSprite.color = new Color(1f, 0.85f, 0.15f, 1f);
                    Settings.TurnDelay = 0.8f;
                    break;
                case "sandbox":
                    if (Settings.SandboxMode)
                    {
                        collider.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                        Settings.SandboxMode = false;
                        foreach (Unit unit in unitTracker.unitList)
                        {
                            if (unit.playerID != unitTracker.humanPlayerID) unit.SetAlphaStatus(false);
                        }
                        StopCoroutine(unitTracker.ChangeEnemyUnits());
                    }
                    else
                    {
                        collider.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 0.85f, 0.15f, 1f);
                        Settings.SandboxMode = true;
                        StartCoroutine(unitTracker.ChangeEnemyUnits());
                    }
                    break;
                case "reset":
                    collider.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 0.85f, 0.15f, 1f);
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    break;
                case "tutorial":
                    collider.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 0.85f, 0.15f, 1f);
                    Settings.TurnDelay = 0.5f;
                    SceneManager.LoadScene("Tutorial_Basics");
                    break;
                case "return":
                    collider.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 0.85f, 0.15f, 1f);
                    SceneManager.LoadScene("Map_11x9");
                    break;
                case "start":
                    break;
                case "main":
                    collider.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 0.85f, 0.15f, 1f);
                    SceneManager.LoadScene("Start_Menu");
                    break;
                case "alpha":
                    StartCoroutine(AssignAlpha(collider.gameObject));
                    break;
                case "allAlpha":
                    unitTracker.AssignAllAlphas();
                    break;
                default:
                    //Debug.Log(collider.gameObject.name);
                    break;

            }
            buttonName = collider.gameObject.name;
        }
        else
        {
            buttonName = "nothing";
        }
        return buttonName;
    }

    IEnumerator AssignAlpha(GameObject crownIcon)
    {
        spriteRenderer.sprite = crownIcon.GetComponent<SpriteRenderer>().sprite;
        while (!Input.GetMouseButtonUp(0))
        {
            yield return null;
        }
        spriteRenderer.sprite = null;
        if (GetActiveTile() != null && GetActiveTile().occupyingUnit != null && (Settings.SandboxMode || GetActiveTile().occupyingUnit.playerID == unitTracker.humanPlayerID))
        {
            Unit selectedUnit = GetActiveTile().occupyingUnit;
            /*foreach (Unit unit in unitTracker.GetAllAlliesOfID(selectedUnit.playerID))
            {
                unit.SetAlphaStatus(false);
            }*/
            selectedUnit.SetAlphaStatus(true);
        }

    }

    public bool AllowClick()
    {
        int layerMask = 1 << 7;
        Collider2D collider = Physics2D.OverlapCircle(transform.position, 0.01f, layerMask);
        if (GameObject.FindWithTag("RestrictArea") != null)
        {
            return (collider != null && collider.gameObject.tag == "RestrictArea");
        }
        else
        {
            return true;
        }
        
    }

    public int CheckMouseoverUnit()
    {
        int layerMask = 1 << 5;
        int returnValue = -10;
        Collider2D collider = Physics2D.OverlapCircle(transform.position, 0.01f, layerMask);
        if (collider != null && collider.gameObject.GetComponent<UnitSelectButton>() != null)
        {
            returnValue = collider.gameObject.GetComponent<UnitSelectButton>().unitIndex;
        }
        return returnValue;
    }

    public TutorialTextBox CheckForTutorialBox()
    {
        int layerMask = 1 << 6;
        TutorialTextBox textBox = null;
        Collider2D collider = Physics2D.OverlapCircle(transform.position, 0.01f, layerMask);
        if (collider != null)
        {
            textBox = collider.gameObject.transform.parent.gameObject.GetComponent<TutorialTextBox>();
        }
        return textBox;
    }
}
