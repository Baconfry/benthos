using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeEnemy : MonoBehaviour
{
    [SerializeField] private int numberOfAIUnits;
    [SerializeField] private List<GameObject> unitsToSpawn;
    private List<GridTile> emptySpawnPoints = new List<GridTile>();
    private CustomGrid grid;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        grid = GameObject.FindWithTag("Grid").GetComponent<CustomGrid>();
        yield return null;
        yield return null;
        emptySpawnPoints = grid.GetEnemySpawnPoints();
        for (int i = 0; i < numberOfAIUnits; i++)
        {
            GridTile tile = emptySpawnPoints[Random.Range(0, emptySpawnPoints.Count)];
            GameObject unit = unitsToSpawn[Random.Range(0, unitsToSpawn.Count)];
            Unit spawnedUnit = Instantiate(unit, tile.transform.position, transform.rotation).GetComponent<Unit>();
            spawnedUnit.playerID = 1;
            spawnedUnit.transform.parent = this.gameObject.transform;
            emptySpawnPoints.Remove(tile);
            unitsToSpawn.Remove(unit);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
