using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedButton : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color newColor;
    [SerializeField] private Color minColor;
    [SerializeField] private Color maxColor;
    [SerializeField] private float pulseRate;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (spriteRenderer.enabled)
        {
            newColor = Color.Lerp(minColor, maxColor, Mathf.PingPong(Time.time, pulseRate));
            spriteRenderer.color = newColor;
        }
    }
}
