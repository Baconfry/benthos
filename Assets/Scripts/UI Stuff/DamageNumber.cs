using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    public int value;
    [SerializeField] private Sprite[] sprites;
    private SpriteRenderer spriteRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[Mathf.Abs(value)];
        if (value >= 0)
        {
            spriteRenderer.color = new Color(1f, 0f, 0f, 0.6f);
        }
        else
        {
            spriteRenderer.color = new Color(0f, 1f, 0f, 0.6f);
        }
        Destroy(this.gameObject, 0.5f);

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + Time.deltaTime, transform.position.z);
    }
}
