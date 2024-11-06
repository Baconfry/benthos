using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfoText : Definitions
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
        if (cursor.GetActiveTile() != null && cursor.GetActiveTile().occupyingUnit != null)
        {
            Unit selectedUnit = cursor.GetActiveTile().occupyingUnit;
            DisplayUnitInfo(selectedUnit.isAlpha ? selectedUnit.internalID + 100 : selectedUnit.internalID);
        }
        else
        {
            DisplayUnitInfo(cursor.CheckMouseoverUnit());
        }
    }

    void DisplayUnitInfo(int id) //name\movement\damage\effect\
    {
        switch (id)
        {
            case -2:
                displayText.text = "HERMIT CRAB (exposed)\nbenthic\n2 DMG\ncannot be healed\n\n\nRANGE: 5\nHEALTH: 1\nMOV COST: 15\nATT COST: 10";
                break;
            case -1:
                displayText.text = "SARDINE\npelagic\n1 DMG\ntutorial-only\n\n\nRANGE: 4\nHEALTH: 2\nMOV COST: 35\nATT COST: 20";
                break;
            case 0:
                displayText.text = "TRIGGERFISH\npelagic\n2 DMG, heals on kill\n\n\n\nRANGE: 5\nHEALTH: 6\nMOV COST: 15\nATT COST: 30";
                break;
            case 1:
                displayText.text = "CRAB\nbenthic\n2 DMG\n\n\ngains 1-use armor once per turn\nRANGE: 4\nHEALTH: 5\nMOV COST: 40\nATT COST: 15";
                break;
            case 2:
                displayText.text = "CONE SNAIL\nbenthic, resists pushing\n1 DMG, heals on kill\nattack inflicts poison\n\ngains armor after being hit\nRANGE: 3\nHEALTH: 7\nMOV COST: 30\nATT COST: 30";
                break;
            case 3:
                displayText.text = "LAMPREY\npelagic\n2 DMG, steals HP\n\n\n\nRANGE: 5\nHEALTH: 4\nMOV COST: 20\nATT COST: 25";
                break;
            case 4:
                displayText.text = "PISTOL SHRIMP\nbenthic\n2 DMG in straight line, pushes\ndelays target by 10\n\ngains 1-use armor once per turn\nRANGE: 3\nHEALTH: 3\nMOV COST: 35\nATT COST: 30";
                break;
            case 5:
                displayText.text = "NAUTILUS\npelagic\n1 DMG, pushes\nno negative effect on allies\n\ngains armor after being hit\nRANGE: 4\nHEALTH: 6\nMOV COST: 30\nATT COST: 25";
                break;
            case 6:
                displayText.text = "JAWFISH\npelagic\n1 DMG within 2-tile range\ncreates sand pile on empty spaces\ndelays target by 20, kills if over 100\n\nRANGE: 4\nHEALTH: 5\nMOV COST: 20\nATT COST: 25";
                break;
            case 7:
                displayText.text = "CLEANER SHRIMP\nbenthic\n1 DMG\ntarget allies to heal them by 1\nreduces ally's delay by 20\ngains 1-use armor once per turn\nRANGE: 4\nHEALTH: 3\nMOV COST: 15\nATT COST: 25";
                break;
            case 8:
                displayText.text = "OCTOPUS\npelagic, resists pushing\n1 DMG up to 2 tiles away\ndrags to front, prevents movement\nno negative effect on allies\n\nRANGE: 4\nHEALTH: 8\nMOV COST: 20\nATT COST: 15";
                break;
            case 9:
                displayText.text = "MANTIS SHRIMP\nbenthic\n2 DMG, pushes\npermanently destroys armor\n\ngains 1-use armor once per turn\nRANGE: 4\nHEALTH: 3\nMOV COST: 20\nATT COST: 25";
                break;
            case 10:
                displayText.text = "PORCUPINEFISH\npelagic\n2 DMG, heals on kill\ndamages on contact\n\n\nRANGE: 3\nHEALTH: 5\nMOV COST: 40\nATT COST: 20";
                break;
            case 11:
                displayText.text = "MAN-O-WAR\npelagic\n1 DMG, heals on kill\nattack inflicts poison\n2x damage to poisoned targets\npoisons on contact\nRANGE: 3\nHEALTH: 6\nMOV COST: 25\nATT COST: 30";
                break;
            case 12:
                displayText.text = "NUDIBRANCH\nbenthic, resists pushing\n1 DMG, heals on kill\nattack inflicts poison when poisoned\n2x damage to poisoned targets\nheals when poisoned\nRANGE: 4\nHEALTH: 5\nMOV COST: 30\nATT COST: 20";
                break;
            case 13:
                displayText.text = "TORPEDO RAY\npelagic\n2 DMG (1 DMG chain)\nchains to units next to target\nhits both enemies and allies\n\nRANGE: 4\nHEALTH: 6\nMOV COST: 20\nATT COST: 25";
                break;
            case 14:
                displayText.text = "HERMIT CRAB\nbenthic\n2 DMG\ncannot be poisoned or healed\nreduces incoming damage to 1\ngains 1-use armor once per turn\nRANGE: 3\nHEALTH: 4\nMOV COST: 35\nATT COST: 40";
                break;
            case 98:
                displayText.text = "alpha HERMIT CRAB (exposed)\nbenthic\n2 DMG\ncannot be healed\n\n\nRANGE: 5\nHEALTH: 1\nMOV COST: 15\nATT COST: 10";
                break;
            case 99:
                displayText.text = "alpha SARDINE\npelagic\n1 DMG\ntutorial-only\n\n\nRANGE: 4\nHEALTH: 2\nMOV COST: 35\nATT COST: 20";
                break;
            case 100:
                displayText.text = "alpha TRIGGERFISH\npelagic\n2 DMG, heals on kill\n\n\n\nRANGE: 6\nHEALTH: 6\nMOV COST: 15\nATT COST: 20";
                break;
            case 101:
                displayText.text = "alpha CRAB\nbenthic, resists pushing\n2 DMG\n\n\ngains 1-use armor once per turn\nRANGE: 4\nHEALTH: 6\nMOV COST: 40\nATT COST: 15";
                break;
            case 102:
                displayText.text = "alpha CONE SNAIL\nbenthic, resists pushing\n2 DMG, heals on kill\nattack inflicts poison\n\ngains armor after being hit\nRANGE: 3\nHEALTH: 8\nMOV COST: 30\nATT COST: 30";
                break;
            case 103:
                displayText.text = "alpha LAMPREY\npelagic\n2 DMG, steals HP\n\n\n\nRANGE: 5\nHEALTH: 6\nMOV COST: 20\nATT COST: 25";
                break;
            case 104:
                displayText.text = "alpha PISTOL SHRIMP\nbenthic, passes through enemies\n2 DMG in straight line, pushes\ndelays target by 10\n\ngains 1-use armor once per turn\nRANGE: 3\nHEALTH: 3\nMOV COST: 35\nATT COST: 20";
                break;
            case 105:
                displayText.text = "alpha NAUTILUS\npelagic\n1 DMG, pushes\nno negative effect on allies\n\ngains armor after being hit\nRANGE: 5\nHEALTH: 7\nMOV COST: 30\nATT COST: 25";
                break;
            case 106:
                displayText.text = "alpha JAWFISH\npelagic\n1 DMG within 3-tile range\ncreates sand pile on empty spaces\ndelays target by 20, kills if over 100\n\nRANGE: 4\nHEALTH: 5\nMOV COST: 20\nATT COST: 25";
                break;
            case 107:
                displayText.text = "alpha CLEANER SHRIMP\nbenthic, passes through enemies\n1 DMG\ntarget allies to heal them by 1\nreduces ally's delay by 20, cures poison\ngains 1-use armor once per turn\nRANGE: 4\nHEALTH: 4\nMOV COST: 15\nATT COST: 20";
                break;
            case 108:
                displayText.text = "alpha OCTOPUS\npelagic, resists pushing\n1 DMG up to 2 tiles away\ndrags to front, prevents movement\nno negative effect on allies\ndelays by 20 on contact\nRANGE: 4\nHEALTH: 8\nMOV COST: 20\nATT COST: 15";
                break;
            case 109:
                displayText.text = "alpha MANTIS SHRIMP\nbenthic, passes through enemies\n2 DMG, pushes\npermanently destroys armor\n\ngains 1-use armor once per turn\nRANGE: 4\nHEALTH: 4\nMOV COST: 20\nATT COST: 25";
                break;
            case 110:
                displayText.text = "alpha PORCUPINEFISH\npelagic\n2 DMG, heals on kill\ndamages on contact\nimmune to bump damage\n\nRANGE: 3\nHEALTH: 5\nMOV COST: 40\nATT COST: 20";
                break;
            case 111:
                displayText.text = "alpha MAN-O-WAR\npelagic\n1 DMG, heals on kill\nattack inflicts poison, delays by 10\n2x damage to poisoned targets\npoisons and delays by 10 on contact\nRANGE: 3\nHEALTH: 7\nMOV COST: 25\nATT COST: 30";
                break;
            case 112:
                displayText.text = "alpha NUDIBRANCH\nbenthic, resists pushing\n1 DMG, heals on kill\nattack inflicts poison when poisoned\n2x damage to poisoned targets\nheals when poisoned, poisoned at start\nRANGE: 4\nHEALTH: 6\nMOV COST: 30\nATT COST: 15";
                break;
            case 113:
                displayText.text = "alpha TORPEDO RAY\npelagic\n2 DMG (1 DMG chain)\nchains to units next to target\nhits both enemies and allies\ndamages on contact\nRANGE: 4\nHEALTH: 7\nMOV COST: 20\nATT COST: 25";
                break;
            case 114:
                displayText.text = "alpha HERMIT CRAB\nbenthic, resists pushing\n2 DMG\ncannot be poisoned or healed\nreduces incoming damage to 1\ngains 1-use armor once per turn\nRANGE: 3\nHEALTH: 5\nMOV COST: 35\nATT COST: 40";
                break;
            default:
                displayText.text = "";
                break;
        }
    }
}
