using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomGrid : Definitions
{
    public int xLength;
    public int yLength;
    public GridTile[,] gridTiles;
    [SerializeField] private AudioClip[] musicTracks;
    private AudioSource musicPlayer;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        gridTiles = new GridTile[xLength, yLength];
        yield return null;
        yield return null;
        ChangeTileSetTo(GetTileSetFromIndex(Settings.TileSetIndex));
        /*if (GameObject.FindWithTag("Tutorial") != null)
        {
            ChangeTileSetTo(TileSet.set1);
        }
        else
        {
            ChangeTileSetTo(GetTileSetFromIndex(Settings.TileSetIndex));
        }*/
        if (GameObject.Find("MusicPlayer") != null)
        {
            musicPlayer = GameObject.Find("MusicPlayer").GetComponent<AudioSource>();
            if (musicPlayer.clip != musicTracks[Settings.TileSetIndex] && SceneManager.GetActiveScene().name != "Start_Menu")
            {
                musicPlayer.clip = musicTracks[Settings.TileSetIndex];
                musicPlayer.Play();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddTile(GridTile newTile)
    {
        gridTiles[newTile.xCoordinate, newTile.yCoordinate] = newTile;
    }

    public void ChangeTileSetTo(TileSet newSet)
    {
        for (int i = 0; i < gridTiles.GetLength(0); i++)
        {
            for (int j = 0; j < gridTiles.GetLength(1); j++)
            {
                try { gridTiles[i, j].UpdateTileSet(newSet); }
                catch { }
            }
        }
    }

    public void ResetAllOutliners()
    {
        for (int i = 0; i < gridTiles.GetLength(0); i++)
        {
            for (int j = 0; j < gridTiles.GetLength(1); j++)
            {
                gridTiles[i, j].SetOutlinerActive(false, Color.white);
            }
        }
    }

    public void ColorAllTiles(Color newColor)
    {
        for (int i = 0; i < gridTiles.GetLength(0); i++)
        {
            for (int j = 0; j < gridTiles.GetLength(1); j++)
            {
                gridTiles[i, j].GetComponent<SpriteRenderer>().color = newColor;
            }
        }
    }

    public void ClearAllPaths()
    {
        for (int i = 0; i < gridTiles.GetLength(0); i++)
        {
            for (int j = 0; j < gridTiles.GetLength(1); j++)
            {
                gridTiles[i, j].path.Clear();
            }
        }
    }

    public void HighlightAllStartingTiles()
    {
        for (int i = 0; i < gridTiles.GetLength(0); i++)
        {
            for (int j = 0; j < gridTiles.GetLength(1); j++)
            {
                if (gridTiles[i, j].isStartingTile)
                {
                    gridTiles[i, j].SetOutlinerActive(true, Color.white);
                }
                else
                {
                    gridTiles[i, j].SetOutlinerActive(false, Color.white);
                }
            }
        }
    }
    public void DecayAllSandPiles()
    {
        for (int i = 0; i < gridTiles.GetLength(0); i++)
        {
            for (int j = 0; j < gridTiles.GetLength(1); j++)
            {
                gridTiles[i, j].DecaySandPile(1);
            }
        }
    }

    public List<GridTile> GetEnemySpawnPoints()
    {
        List<GridTile> spawnPoints = new List<GridTile>();
        for (int i = 0; i < gridTiles.GetLength(0); i++)
        {
            for (int j = 0; j < gridTiles.GetLength(1); j++)
            {
                if (gridTiles[i, j].isEnemySpawnPoint) spawnPoints.Add(gridTiles[i, j]);
            }
        }
        return spawnPoints;
    }

    public List<GridTile> GetPlayerSpawnPoints()
    {
        List<GridTile> spawnPoints = new List<GridTile>();
        for (int i = 0; i < gridTiles.GetLength(0); i++)
        {
            for (int j = 0; j < gridTiles.GetLength(1); j++)
            {
                if (gridTiles[i, j].isStartingTile) spawnPoints.Add(gridTiles[i, j]);
            }
        }
        return spawnPoints;
    }

    public List<GridTile> GetTilesWithinRangeOf(int range, int x, int y)
    {
        List<GridTile> tilesInRange = new List<GridTile>();
        for (int i = 0; i < gridTiles.GetLength(0); i++)
        {
            for (int j = 0; j < gridTiles.GetLength(1); j++)
            {
                if (gridTiles[x, y].GetDistanceBetweenTiles(gridTiles[x, y], gridTiles[i, j]) <= range) tilesInRange.Add(gridTiles[i, j]);
            }
        }
        return tilesInRange;
    }

    public List<GridTile> Get8SurroundingTiles(int x, int y)
    {
        List<GridTile> tilesInRange = new List<GridTile>();
        for (int i = 0; i < gridTiles.GetLength(0); i++)
        {
            for (int j = 0; j < gridTiles.GetLength(1); j++)
            {
                if (Mathf.Abs(x - i) <= 1 && Mathf.Abs(y - j) <= 1 && gridTiles[x, y] != gridTiles[i, j]) tilesInRange.Add(gridTiles[i, j]);
            }
        }
        return tilesInRange;
    }

    public void SwitchTeamPositions()
    {
        for (int i = 0; i < gridTiles.GetLength(0); i++)
        {
            for (int j = 0; j < gridTiles.GetLength(1); j++)
            {
                if (gridTiles[i, j].isStartingTile)
                {
                    gridTiles[i, j].isEnemySpawnPoint = true;
                    gridTiles[i, j].isStartingTile = false;
                }
                else if (gridTiles[i, j].isEnemySpawnPoint)
                {
                    gridTiles[i, j].isEnemySpawnPoint = false;
                    gridTiles[i, j].isStartingTile = true;
                }
            }
        }
    }
}
