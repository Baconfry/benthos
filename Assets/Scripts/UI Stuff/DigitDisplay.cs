using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigitDisplay : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private int powerTen;
    private SpriteRenderer spriteRenderer;

    public bool matchesPlayerID;
    private bool isHighlighted;
    private Color newColor;
    private Color minColor;
    private Color maxColor;
    private float pulseRate;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        pulseRate = 0.6f;
        minColor = new Color(1f, 0.85f, 0f, 1f);
        maxColor = new Color(0.25f, 0.21f, 0f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (spriteRenderer.enabled && isHighlighted && (matchesPlayerID || Settings.SandboxMode))
        {
            newColor = Color.Lerp(minColor, maxColor, Mathf.PingPong(Time.time, pulseRate));
            spriteRenderer.color = newColor;
        }
    }

    public void ChangeDisplayTo(int value)
    {
        //Debug.Log((int)(value / (Mathf.Pow(10, powerTen))));
        spriteRenderer.sprite = sprites[(int)(value/Mathf.Pow(10, powerTen)) % 10];
    }

    public void Highlight(bool highlight)
    {
        isHighlighted = highlight;
        if (isHighlighted)
        {
            spriteRenderer.color = Color.yellow;
        }
        else
        {
            spriteRenderer.color = Color.white;
        }
    }

    public void SetVisibility(bool isVisible)
    {
        spriteRenderer.enabled = isVisible;
    }

    public void AssignTeamColor(int playerID)
    {
        switch (playerID)
        {
            case 0:
                spriteRenderer.color = new Color(0.306f, 0.656f, 0.922f, 1f);
                break;
            case 1:
                spriteRenderer.color = new Color(0.922f, 0.247f, 0.314f, 1f);
                break;
            case 2:
                spriteRenderer.color = Color.green;
                break;
            case 3:
                spriteRenderer.color = Color.yellow;
                break;
            case 4:
                spriteRenderer.color = Color.white;
                break;
            default:
                Debug.Log("playerIDs can only range from 0-4");
                break;
        }
    }

}
