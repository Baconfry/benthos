using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCursor : Definitions
{
    //private CustomGrid grid;
    private MapPreview mapPreview;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        mapPreview = GameObject.Find("MapPreview").GetComponent<MapPreview>();
        yield return null;
        yield return null;
        //grid = GameObject.FindWithTag("Grid").GetComponent<CustomGrid>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        transform.position = mousePos;

        if (Input.GetMouseButtonDown(0))
        {

            int layerMask = 1 << 5;
            Collider2D collider = Physics2D.OverlapCircle(transform.position, 0.01f, layerMask);
            if (collider != null)
            {
                switch (collider.gameObject.name)
                {
                    case "mapToggle":
                        Settings.MapIndex++;
                        if (Settings.MapIndex > 3) Settings.MapIndex = 0;
                        StartCoroutine(mapPreview.ReplaceMapFromIndex(Settings.MapIndex));
                        break;
                    case "styleToggle":
                        Settings.TileSetIndex++;
                        if (Settings.TileSetIndex > 1) Settings.TileSetIndex = 0;
                        CustomGrid grid = GameObject.FindWithTag("Grid").GetComponent<CustomGrid>();
                        grid.ChangeTileSetTo(GetTileSetFromIndex(Settings.TileSetIndex));
                        break;
                    case "startGame":
                        //Debug.Log(Settings.MapIndex);
                        SceneManager.LoadScene("Map_11x9");
                        break;
                    case "howToPlay":
                        Settings.TurnDelay = 0.5f;
                        SceneManager.LoadScene("Tutorial_Basics");
                        break;
                    default:
                        Debug.Log(collider.gameObject.name);
                        break;
                }
            }
        }
    }
}
