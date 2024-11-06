using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSelection : Definitions
{
    public GameObject[] maps;
    //public TileSet tileSet;
    private CustomGrid grid;

    // Start is called before the first frame update
    void Awake()
    {
        if (maps.Length > 0) 
        {
            //GameObject newMap = Instantiate(maps[Random.Range(0, maps.Length)], transform.position, transform.rotation);
            GameObject newMap = Instantiate(maps[Settings.MapIndex], transform.position, transform.rotation);
            newMap.transform.parent = this.gameObject.transform;
            grid = newMap.GetComponent<CustomGrid>();
        }
        else
        {
            grid = GameObject.FindWithTag("Grid").GetComponent<CustomGrid>();
        }
    }

    IEnumerator Start()
    {
        yield return null;
        yield return null;
        grid.ChangeTileSetTo(GetTileSetFromIndex(Settings.TileSetIndex));
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
