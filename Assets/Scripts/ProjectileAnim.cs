using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAnim : MonoBehaviour
{
    [SerializeField] private float timePerSquare;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator MoveToTarget(GridTile destination)
    {
        Vector3 startingPosition = transform.position;
        float startTime = Time.time;
        float totalAnimationTime = Settings.TurnDelay * timePerSquare * Vector3.Distance(transform.position, destination.transform.position);

        while (Time.time < startTime + totalAnimationTime)
        {
            transform.position = Vector3.Lerp(startingPosition, destination.transform.position, (Time.time - startTime) / totalAnimationTime);
            yield return null;
        }
        transform.position = destination.transform.position;
        Destroy(this.gameObject);
    }

}
