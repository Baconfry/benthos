using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColorOnMouseover : MonoBehaviour
{
    [SerializeField] private Color mouseOnColor;
    [SerializeField] private Color mouseOffColor;
    private Cursor cursor;
    private SpriteRenderer spriteRenderer;
    
    // Start is called before the first frame update
    void Awake()
    {
        cursor = GameObject.Find("Cursor").GetComponent<Cursor>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cursor.CheckForOptionChange() == this.gameObject.name)
        {
            spriteRenderer.color = mouseOnColor;
        }
        else
        {
            spriteRenderer.color = mouseOffColor;
        }
    }
}
