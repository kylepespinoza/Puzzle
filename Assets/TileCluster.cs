using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCluster
{
    List<Tile> tilesInCluster = new List<Tile>();

    int numberOfTilesInCluster;

    private bool readyToKnockout;
    public bool ReadyToKnockout { get { return readyToKnockout; } set { readyToKnockout = value; } }

    public void AddTileToCluster(Tile newTile)
    {
        numberOfTilesInCluster++;
        tilesInCluster.Add(newTile);
        newTile.AddToCluster(this);
        if(numberOfTilesInCluster > 2)
        {
            ReadyToKnockout = true;
            Debug.Log("Tile cluster ready to knockout");
        }
    }

    public List<Tile> GetTiles()
    {
        return tilesInCluster;
    }

    public bool Contains(Tile tile)
    {
        foreach(Tile t in tilesInCluster)
        {
            if(tile == t)
            {
                return true;
            }
        }
        return false;
    }
}
