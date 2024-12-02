using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GridTile : Definitions
{
    private SpriteRenderer spriteRenderer;
    private GameObject outliner;

    private Sprite[] sprites = new Sprite[10];
    [SerializeField] private Sprite[] spriteSet1 = new Sprite[10];
    [SerializeField] private Sprite[] spriteSet2 = new Sprite[10];
    [SerializeField] private Sprite[] spriteSet3 = new Sprite[10];

    public TileType tileType;
    public bool isStartingTile;
    public bool isEnemySpawnPoint;
    public int sandPileTimer;
    public int xCoordinate;
    public int yCoordinate;

    private CustomGrid grid;

    public Unit occupyingUnit = null;
    public List<GridTile> path = new List<GridTile>();
    public GameObject lavaAnimation;
    public GameObject sandAnimation;
    public GameObject currentAnimation;
    private DigitDisplay sandTimerDisplay1;
    private DigitDisplay sandTimerDisplay2;
    public SpriteRenderer largeSprite;
    [SerializeField] private Sprite[] largeSprites;
    
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        sandTimerDisplay1 = transform.Find("sandTimerDisplay1").GetComponent<DigitDisplay>();
        sandTimerDisplay2 = transform.Find("sandTimerDisplay2").GetComponent<DigitDisplay>();
        outliner = transform.Find("outliner").gameObject;
        grid = GameObject.FindWithTag("Grid").GetComponent<CustomGrid>();
        currentAnimation = transform.Find("currentAnim").gameObject;
        largeSprite = transform.Find("largePart").GetComponent<SpriteRenderer>();
    }
    // Start is called before the first frame update
    IEnumerator Start()
    {
        sandPileTimer = 20;
        SetOutlinerActive(false, Color.white);
        ChangeTileTo(tileType);
        xCoordinate = (int)transform.localPosition.x + (int)grid.xLength/2;
        yCoordinate = (int)transform.localPosition.y + (int)grid.yLength/2;
        yield return null;
        grid.AddTile(this);
        //UpdateTileSet(GetTileSetFromIndex(Settings.TileSetIndex));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateTileSet(TileSet newSet)
    {
        switch (newSet)
        {
            case TileSet.set1:
                sprites = spriteSet1;
                break;
            case TileSet.set2:
                sprites = spriteSet2;
                break;
            case TileSet.set3:
                sprites = spriteSet3;
                break;
            default:
                break;
        }
        ChangeTileTo(tileType);
    }

    public List<GridTile> FilterTilesByType(List<GridTile> tiles, TileType type)
    {
        List<GridTile> filteredTypes = new List<GridTile>();
        foreach (GridTile tile in tiles)
        {
            if (tile.tileType == type) filteredTypes.Add(tile);
        }
        return filteredTypes;
    }

    public List<GridTile> GetTilesInRange(Unit unit, Color color)
    {
        grid.ClearAllPaths();
        List<GridTile> accessibleTiles = new List<GridTile>();
        accessibleTiles.Add(unit.currentTile);
        if (unit.currentMoveRange > 0)
        {
            for (int i = 0; i < unit.currentMoveRange; i++)
            {
                List<GridTile> startingTileList = new List<GridTile>();
                foreach (GridTile tile in accessibleTiles)
                {
                    startingTileList.Add(tile);
                }

                foreach (GridTile tile in startingTileList)
                {
                    if (GetTileAbove(tile) != null && !accessibleTiles.Contains(GetTileAbove(tile)) && CanMoveTo(GetTileAbove(tile), unit))
                    {
                        accessibleTiles.Add(GetTileAbove(tile));
                        for (int j = 0; j < tile.path.Count; j++)
                        {
                            GetTileAbove(tile).path.Add(tile.path[j]);
                        }
                        GetTileAbove(tile).path.Add(GetTileAbove(tile));
                    }

                    if (GetTileBelow(tile) != null && !accessibleTiles.Contains(GetTileBelow(tile)) && CanMoveTo(GetTileBelow(tile), unit))
                    {
                        accessibleTiles.Add(GetTileBelow(tile));
                        for (int j = 0; j < tile.path.Count; j++)
                        {
                            GetTileBelow(tile).path.Add(tile.path[j]);
                        }
                        GetTileBelow(tile).path.Add(GetTileBelow(tile));
                    }

                    if (GetTileRight(tile) != null && !accessibleTiles.Contains(GetTileRight(tile)) && CanMoveTo(GetTileRight(tile), unit))
                    {
                        accessibleTiles.Add(GetTileRight(tile));
                        for (int j = 0; j < tile.path.Count; j++)
                        {
                            GetTileRight(tile).path.Add(tile.path[j]);
                        }
                        GetTileRight(tile).path.Add(GetTileRight(tile));
                    }

                    if (GetTileLeft(tile) != null && !accessibleTiles.Contains(GetTileLeft(tile)) && CanMoveTo(GetTileLeft(tile), unit))
                    {
                        accessibleTiles.Add(GetTileLeft(tile));
                        for (int j = 0; j < tile.path.Count; j++)
                        {
                            GetTileLeft(tile).path.Add(tile.path[j]);
                        }
                        GetTileLeft(tile).path.Add(GetTileLeft(tile));
                    }
                }
            }
        }
        
        foreach (GridTile tile in accessibleTiles)
        {
            tile.SetOutlinerActive(true, color);
        }
        return accessibleTiles;
    }

    public List<GridTile> GetClosestAccessibleTilesTo(GridTile targetTile, List<GridTile> allAccessibleTiles)
    {
        List<GridTile> closestTiles = new List<GridTile>();
        //targetTile.SetOutlinerActive(true, Color.blue);
        closestTiles.Add(this);
        foreach (GridTile tile in allAccessibleTiles)
        {
            if (GetDistanceBetweenTiles(tile, targetTile) < GetDistanceBetweenTiles(closestTiles[0], targetTile) && tile.occupyingUnit == null)
            {
                closestTiles.Clear();
                closestTiles.Add(tile);
            }
            else if (GetDistanceBetweenTiles(tile, targetTile) == GetDistanceBetweenTiles(closestTiles[0], targetTile) && tile.occupyingUnit == null)
            {
                closestTiles.Add(tile);
            }
        }
        return closestTiles;
    }

    public int GetDistanceBetweenTiles(GridTile startTile, GridTile endTile)
    {
        return Math.Abs(startTile.xCoordinate - endTile.xCoordinate) + Math.Abs(startTile.yCoordinate - endTile.yCoordinate);
    }

    public void SetOutlinerActive(bool isActive, Color color)
    {        
        outliner.GetComponent<SpriteRenderer>().enabled = isActive;
        if (isActive)
        {
            outliner.GetComponent<SpriteRenderer>().color = color;
        }
    }

    public bool GetOutlinerActive()
    {
        return outliner.GetComponent<SpriteRenderer>().enabled;
    }

    public bool IsSolid()
    {
        if (tileType == TileType.coral || tileType == TileType.fireCoral || tileType == TileType.outcrop)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CanMoveTo(GridTile tile, Unit unit)
    {
        bool canMoveTo;

        if (tile.tileType != TileType.sand && unit.GetComponent<BobbitWorm>() != null) return false;

        switch (tile.tileType)
        {
            case TileType.stone:
            case TileType.algae:
            case TileType.sand:
            case TileType.current:
            case TileType.anemone:
            case TileType.vent:
                canMoveTo = true;
                break;
            case TileType.outcrop:
            case TileType.coral:
            case TileType.fireCoral:
                canMoveTo = false;
                break;
            case TileType.trench:
                canMoveTo = unit.movementType == MoveType.pelagic;
                break;
            default:
                Debug.Log("attempted to reference invalid type");
                canMoveTo = false;
                break;
        }
        if (unit.movementType != MoveType.pelagic && tile.occupyingUnit != null && tile.occupyingUnit.playerID != unit.playerID && !unit.passThroughEnemies)
        {
            canMoveTo = false;
        }
        if (unit.currentTile == tile)
        {
            canMoveTo = true;
        }

        return canMoveTo;
    }

    public GridTile GetTileAbove(GridTile tile)
    {
        try
        {
            return grid.gridTiles[tile.xCoordinate, tile.yCoordinate + 1];
        }
        catch
        {
            return null;
        }
    }
    public GridTile GetTileBelow(GridTile tile)
    {
        try
        {
            return grid.gridTiles[tile.xCoordinate, tile.yCoordinate - 1];
        }
        catch
        {
            return null;
        }
    }
    public GridTile GetTileRight(GridTile tile)
    {
        try
        {
            return grid.gridTiles[tile.xCoordinate + 1, tile.yCoordinate];
        }
        catch
        {
            return null;
        }
    }
    public GridTile GetTileLeft(GridTile tile)
    {
        try
        {
            return grid.gridTiles[tile.xCoordinate - 1, tile.yCoordinate];
        }
        catch
        {
            return null;
        }
    }

    public List<GridTile> Get8SurroundingTiles(GridTile tile)
    {
        return grid.Get8SurroundingTiles(tile.xCoordinate, tile.yCoordinate);
    }

    public GridTile GetFacingTile(GridTile tile)
    {
        GridTile destination = null;
        try
        {
            switch (transform.rotation.eulerAngles.z)
            {
                case 0:
                    destination = grid.gridTiles[tile.xCoordinate, tile.yCoordinate + 1];
                    break;
                case 90:
                    destination = grid.gridTiles[tile.xCoordinate - 1, tile.yCoordinate];
                    break;
                case 180:
                    destination = grid.gridTiles[tile.xCoordinate, tile.yCoordinate - 1];
                    break;
                case 270:
                    destination = grid.gridTiles[tile.xCoordinate + 1, tile.yCoordinate];
                    break;
                default:
                    Debug.Log("misaligned tile");
                    break;
            }
        }
        catch 
        {
            return null;
        }
        return destination;
    }

    public void ChangeTileTo(TileType newType)
    {
        tileType = newType;
        currentAnimation.SetActive(tileType == TileType.current);
        
        if (currentAnimation.activeSelf) 
        {
            Color tileColor = GetComponent<SpriteRenderer>().color;
            Color prefabColor = currentAnimation.GetComponent<SpriteRenderer>().color;
            currentAnimation.GetComponent<SpriteRenderer>().color = new Color(prefabColor.r, prefabColor.g, prefabColor.b, tileColor.a);
        }
        switch (tileType)
        {
            case TileType.stone:
                spriteRenderer.sprite = sprites[0];
                break;
            case TileType.outcrop:
                spriteRenderer.sprite = sprites[1];
                break;
            case TileType.algae:
                spriteRenderer.sprite = sprites[2];
                break;
            case TileType.sand:
                spriteRenderer.sprite = sprites[3];
                if (occupyingUnit != null && occupyingUnit.currentMoveRange == 1) occupyingUnit.currentMoveRange = occupyingUnit.baseMoveRange;
                break;
            case TileType.coral:
                spriteRenderer.sprite = sprites[4];
                break;
            case TileType.fireCoral:
                spriteRenderer.sprite = sprites[5];
                sandPileTimer = 20;
                sandTimerDisplay1.SetVisibility(true);
                sandTimerDisplay2.SetVisibility(true);
                UpdateSandTimer();
                break;
            case TileType.trench:
                spriteRenderer.sprite = sprites[6];
                break;
            case TileType.anemone:
                spriteRenderer.sprite = sprites[7];
                break;
            case TileType.current:
                spriteRenderer.sprite = sprites[8];
                break;
            case TileType.vent:
                spriteRenderer.sprite = sprites[9];
                break;
            default:
                Debug.Log("attempted to change to invalid type");
                break;
        }
        //spriteRenderer.sortingOrder = IsSolid() ? 12 - yCoordinate: -1;
        if (IsSolid())
        {
            largeSprite.enabled = true;
            largeSprite.sortingOrder = 12 - yCoordinate;
            switch (tileType)
            {
                case TileType.outcrop:
                    switch (Settings.TileSetIndex)
                    {
                        case 0:
                            largeSprite.sprite = largeSprites[0];
                            break;
                        case 1:
                            largeSprite.sprite = largeSprites[1];
                            break;
                        case 2:
                            break;
                        default:
                            break;
                    }                  
                    break;
                case TileType.coral:
                    largeSprite.sprite = largeSprites[2];
                    break;
                case TileType.fireCoral:
                    largeSprite.sprite = largeSprites[3];
                    break;
                default:
                    Debug.Log("some other solid tile detected");
                    break;
            }

        }
        else
        {
            largeSprite.enabled = false;
        }
    }

    public void UpdateSandTimer()
    {
        sandTimerDisplay1.ChangeDisplayTo(sandPileTimer);
        sandTimerDisplay2.ChangeDisplayTo(sandPileTimer);
    }

    public void DecaySandPile(int amount)
    {
        if (tileType == TileType.fireCoral)
        {
            sandPileTimer -= amount;
            if (sandPileTimer < 0) sandPileTimer = 0;
            if (sandPileTimer > 95) sandPileTimer = 95;
            UpdateSandTimer();

            if (sandPileTimer == 0)
            {
                Instantiate(sandAnimation, transform.position, transform.rotation);
                ChangeTileTo(TileType.sand);
                sandPileTimer = 20;
                sandTimerDisplay1.SetVisibility(false);
                sandTimerDisplay2.SetVisibility(false);
            }
        }
    }
}
