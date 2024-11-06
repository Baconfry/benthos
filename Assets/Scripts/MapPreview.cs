using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPreview : Definitions
{
    public GameObject[] maps;
    private GameObject currentMap;
    private CustomGrid grid;
    [SerializeField] private AudioClip menuMusic;
    private AudioSource musicPlayer;
    // Start is called before the first frame update

    /*void Awake()
    {
        grid = GameObject.FindWithTag("Grid").GetComponent<CustomGrid>();
        currentMap = grid.gameObject;
    }*/

    void Start()
    {
        musicPlayer = GameObject.Find("MusicPlayer").GetComponent<AudioSource>();
        if (musicPlayer.clip != menuMusic)
        {
            musicPlayer.clip = menuMusic;
            musicPlayer.Play();
        }
        StartCoroutine(AddNewMapFromIndex(Settings.MapIndex));
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator AddNewMapFromIndex(int index)
    {
        yield return null;
        currentMap = Instantiate(maps[index], transform.position, transform.rotation);
        currentMap.transform.parent = this.gameObject.transform;
        grid = currentMap.GetComponent<CustomGrid>();
        yield return null;
        yield return null;
        grid.ChangeTileSetTo(GetTileSetFromIndex(Settings.TileSetIndex));
        grid.ColorAllTiles(new Color(1f, 1f, 1f, 0.5f));
    }

    public IEnumerator ReplaceMapFromIndex(int index)
    {
        Destroy(currentMap.gameObject);
        yield return null;
        currentMap = Instantiate(maps[index], transform.position, transform.rotation);
        currentMap.transform.parent = this.gameObject.transform;
        grid = currentMap.GetComponent<CustomGrid>();
        yield return null;
        yield return null;
        grid.ChangeTileSetTo(GetTileSetFromIndex(Settings.TileSetIndex));
        grid.ColorAllTiles(new Color(1f, 1f, 1f, 0.5f));
    }
}
