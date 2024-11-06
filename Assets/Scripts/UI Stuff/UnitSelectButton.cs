using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectButton : MonoBehaviour
{
    public int unitIndex;
    private Cursor cursor;
    private SpriteRenderer buttonOutliner;

    // Start is called before the first frame update
    void Start()
    {
        cursor = GameObject.Find("Cursor").GetComponent<Cursor>();
        buttonOutliner = transform.Find("outliner").GetComponent<SpriteRenderer>();
        //buttonOutliner.color = new Color(1f, 0.85f, 0.15f, 0.7f);
        buttonOutliner.color = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        if (cursor.cursorUnitIndex == unitIndex)
        {
            buttonOutliner.color = new Color(0.1f, 0.2f, 0.7f, 1f);
        }
        else
        {
            buttonOutliner.color = Color.white;
        }
    }
}
