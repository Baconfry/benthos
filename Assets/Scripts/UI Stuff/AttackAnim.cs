using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAnim : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private float secondsPerFrame;
    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(PlayAnimation());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator PlayAnimation()
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            spriteRenderer.sprite = sprites[i];
            yield return new WaitForSeconds(secondsPerFrame * Settings.TurnDelay);
        }
        Destroy(this.gameObject);
    }

    public float GetTotalAnimationTime()
    {
        return (secondsPerFrame * sprites.Length * Settings.TurnDelay);
    }

}
