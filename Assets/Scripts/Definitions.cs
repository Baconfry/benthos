using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Definitions : MonoBehaviour
{

    public enum TileType { stone, outcrop, algae, sand, coral, fireCoral, trench, anemone, current, vent };
    public enum TileSet { set1, set2, set3 };
    public enum MoveType { pelagic, benthic };
    public enum ArmorType { none, light, heavy };

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public TileSet GetTileSetFromIndex(int index)
    {
        switch (index)
        {
            case 0:
                return TileSet.set1;
            case 1:
                return TileSet.set2;
            case 2:
                return TileSet.set3;
            default:
                Debug.Log("wrong index, returned set1");
                return TileSet.set1;
        }
    }

}
