using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{

    private static TileManager _instance;
    public static TileManager Instance { get { return _instance; } }

    public enum TileType
    {
        Purple,
        Red,
        Teal,
        White,
        Yellow
    }

    public Material[] colors;

    Tile[] tiles;

    Dictionary<TileType, Material> materialsByTileType = new Dictionary<TileType, Material>();

    GameObject tileRespawnPointsParent;
    public List<Vector3> tileRespawnPoints = new List<Vector3>();

    int currentRespawnPointIndex, currentTileToRespawnIndex = 0;

    //list of tiles to be respawned
    public List<Tile> tilesToRespawn = new List<Tile>();
    //list of column indices for respawn tiles (to keep track of which column lost a tile so we can add a new one to it
    public List<int> columnIndicesForRespawn = new List<int>();

    static Coroutine respawnLoop;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
        tiles = GetComponentsInChildren<Tile>();
        tileRespawnPointsParent = transform.GetChild(0).gameObject;
        for (int i = 0; i < tileRespawnPointsParent.transform.childCount; i++)
        {
            tileRespawnPoints.Add(tileRespawnPointsParent.transform.GetChild(i).position);
        }
    }

    private void Start()
    {
        materialsByTileType.Add(TileType.Purple, colors[0]);
        materialsByTileType.Add(TileType.Red, colors[1]);
        materialsByTileType.Add(TileType.Teal, colors[2]);
        materialsByTileType.Add(TileType.White, colors[3]);
        materialsByTileType.Add(TileType.Yellow, colors[4]);
        AssignTiles();
    }

    void AssignTiles()
    {
        TileType[] tileTypes = (TileType[])System.Enum.GetValues(typeof(TileType));
        int tileNumber = 0;
        foreach(Tile t in tiles)
        {
            TileType randomBar = (TileType)tileTypes.GetValue(Random.Range(0, tileTypes.Length));
            t.RecieveType(randomBar, materialsByTileType[randomBar]);
            t.gameObject.name = "Tile" + ++tileNumber;
        }
    }

    public void AddTileToRespawnList(Tile tile)
    {
        tilesToRespawn.Add(tile);
        if(respawnLoop == null)
        {
            respawnLoop = StartCoroutine(RespawnLoopRoutine());
        }
    }

    IEnumerator RespawnLoopRoutine()
    {
        yield return new WaitForSeconds(.35f);
        tilesToRespawn[0].ResetTile(tileRespawnPoints[columnIndicesForRespawn[0] - 1]);
        tilesToRespawn.RemoveAt(currentTileToRespawnIndex);
        columnIndicesForRespawn.RemoveAt(currentTileToRespawnIndex);
        currentRespawnPointIndex++;
        if(currentRespawnPointIndex > 3)
        {
            currentRespawnPointIndex = 0;
        }
        if(tilesToRespawn.Count != 0)
        {
            StartCoroutine(RespawnLoopRoutine());
        }
        else
        {
            respawnLoop = null;
        }
    }

}
