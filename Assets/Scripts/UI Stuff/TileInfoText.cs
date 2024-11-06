using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileInfoText : Definitions
{
    private Cursor cursor;
    private Text displayText;

    // Start is called before the first frame update
    void Start()
    {
        cursor = GameObject.Find("Cursor").GetComponent<Cursor>();
        displayText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cursor.GetActiveTile() != null)
        {
            DisplayTileInfo();
        }
        else if (cursor.FindButtonName() != null)
        {
            DisplayButtonInfo(cursor.FindButtonName());
        }
        else {
            displayText.text = "";
        }
    }

    void DisplayButtonInfo(string buttonName)
    {
        switch (buttonName)
        {
            case "sandbox":
                displayText.text = "SANDBOX MODE\nallows you to control both sides\nunavailable after game start";
                break;
            case "auto":
                displayText.text = "AUTOPILOT MODE\nCPU will control both sides\ncan be disabled at any time";
                break;
            case "alpha":
                displayText.text = "ALPHA UPGRADE\ndrag onto your unit to upgrade it\nrandom enemy will also be upgraded";
                break;
            case "random":
                displayText.text = "RANDOM\nfills all slots with random units\nno more than 2 of each type";
                break;
            case "allAlpha":
                displayText.text = "ALPHA UPGRADE (ALL)\nupgrade all units on the field";
                break;
            default:
                displayText.text = "";
                break;
        }
    }

    void DisplayTileInfo()
    {
        TileType thisTileType = cursor.GetActiveTile().tileType;
        switch (thisTileType)
        {
            case TileType.stone:
                displayText.text = "STONE\nno special effect";
                break;
            case TileType.algae:
                displayText.text = "ALGAE\nheals 1 health, then becomes stone";
                break;
            case TileType.sand:
                displayText.text = "SAND\nunits cannot resist being pushed";
                break;
            case TileType.current:
                displayText.text = "CURRENT\npushes unit in facing direction";
                break;
            case TileType.anemone:
                displayText.text = "ANEMONE\npoisons unit if it is not armored";
                break;
            case TileType.vent:
                displayText.text = "HYDROTHERMAL VENT\nkills unit instantly";
                break;
            case TileType.outcrop:
                displayText.text = "OUTCROP\nunits cannot pass through";
                break;
            case TileType.coral:
                displayText.text = "CORAL\nunits cannot pass through\ncan be destroyed by most attacks";
                break;
            case TileType.fireCoral:
                displayText.text = "SAND PILE\nunits cannot pass through\ndisappears after a while\ncan be destroyed by most attacks";
                break;
            case TileType.trench:
                displayText.text = "TRENCH\nonly swimming units can pass\nothers drop to 1 range, can't attack\nunits cannot resist being pushed";
                break;
            default:
                Debug.Log("no tile selected");
                break;
        }
    }
}
