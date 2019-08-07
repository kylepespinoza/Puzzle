using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectorManager : MonoBehaviour
{
    private static DetectorManager _instance;
    public static DetectorManager Instance { get { return _instance; } }

    static Detector[] detectors;

    //static List<Detector> rowOne = new List<Detector>();
    //static List<Detector> rowTwo = new List<Detector>();
    //static List<Detector> rowThree = new List<Detector>();
    //static List<Detector> rowFour = new List<Detector>();

    //static List<List<Detector>> detectorRows = new List<List<Detector>>() { rowOne, rowTwo, rowThree, rowFour };

    static int numberOfConsecutiveTiles = 1;

    //static Detector[,] detectorMatrix;

    public bool checkTiles;

    Tile previousTile, previousPreviousTile;


    public List<Tile> combinedTiles = new List<Tile>();

    bool detectingTiles;

    Coroutine checkDetectorRoutine;

    public List<TileCluster> tileClusters = new List<TileCluster>();
    public List<List<string>> clusterNames = new List<List<string>>();

    public LayerMask tileLayerMask;

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
        detectors = GetComponentsInChildren<Detector>();
        int column = 1;
        for (int i = 0; i < detectors.Length; i++)
        {
            detectors[i].columnIndex = column;
            column++;
            if (column > 4)
            {
                column = 1;
            }
        }

        //for (int i = 0; i < 4; i++)
        //{
        //    for (int j = 0; j < 4; j++)
        //    {
        //        detectorRows[i].Add(detectors[j + (4 * i)]);
        //    }
        //}
    }

    void Start()
    {
        //detectorMatrix = new Detector[4, 4]
        //{
        //    { rowOne[0], rowOne[1], rowOne[2], rowOne[3] },
        //    { rowTwo[0], rowTwo[1], rowTwo[2], rowTwo[3] },
        //    { rowThree[0], rowThree[1], rowThree[2], rowThree[3] },
        //    { rowFour[0], rowFour[1], rowFour[2], rowFour[3] }
        //};
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            KnockoutTiles();
        }
    }

    IEnumerator CheckDetectorsAfterSecond()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(CheckDetectors());
        checkDetectorRoutine = null;
    }

    public void CheckDetectorAfterDelay()
    {
            //Debug.Log("About to check detectors");
        if (checkDetectorRoutine != null)
        {
            StopCoroutine(checkDetectorRoutine);
            checkDetectorRoutine = null;
        }
            checkDetectorRoutine = StartCoroutine(CheckDetectorsAfterSecond());
    }

    //Need to wait for a frame for all the tiles to check eachother before clearing the board
    public IEnumerator CheckDetectors()
    {
        CheckTiles();
        yield return new WaitForEndOfFrame();
        KnockoutTiles();

    }

    //private void CheckRows()
    //{
    //    //From top to bottom, left to right, we check our tile types
    //    //Clear our previous tile reference
    //    //Debug.Log("About to Iterate over " + detectors.Length + " tiles");
    //    previousTile = previousPreviousTile = null;
    //    for (int i = 0; i < detectors.Length; i++)
    //    {
    //        //determine if starting a new row. if so, reset number of tiles in a row count 
    //        if ((i) % 4 == 0)
    //        {
    //            //Debug.Log("streak reset");
    //            numberOfConsecutiveTiles = 1;
    //            previousTile = previousPreviousTile = null;
    //        }
    //        //Debug.Log(detectors[i].activeTile.type);
    //        //Debug.Log("About to check: " + detectors[i].activeTile.name);

    //        //If we don't have a previous tile reference, don't do the following code 
    //        if (previousTile != null)
    //        {
    //            if (detectors[i].activeTile.type == previousTile.type)
    //            {
    //                //Debug.Log("2 in a row detected: " + detectors[i].activeTile.name + " and " + previousTile.name);
    //                numberOfConsecutiveTiles++;
    //                //Do we have two in a row?
    //                if (numberOfConsecutiveTiles > 1)
    //                {

    //                    TileCluster tileCluster = new TileCluster();
    //                    tileCluster.AddTileToCluster(previousTile);
    //                    tileClusters.Add(tileCluster);
    //                    Debug.Log("Tile Cluster created!");
    //                    List<string> tileNames = new List<string>();
    //                    tileNames.Add(previousTile.name);

    //                    //letting the previous tile know it's combined
    //                    if (!previousTile.combined)
    //                    {
    //                        previousTile.combined = true;
    //                        //Dont add the previous tile to the combined tile list twice
    //                        if (!combinedTiles.Contains(previousTile))
    //                        {
    //                            combinedTiles.Add(previousTile);
    //                        }
    //                    }
    //                    //letting our current tile know it's combined
    //                    detectors[i].activeTile.combined = true;
    //                    if (!combinedTiles.Contains(detectors[i].activeTile))
    //                    {
    //                        combinedTiles.Add(detectors[i].activeTile);
    //                        tileCluster.AddTileToCluster(detectors[i].activeTile);

    //                    }
    //                    //if(numberOfConsecutiveTiles > 2)
    //                    //{
    //                    //    previousPreviousTile.readyToKnockout = true;
    //                    //    previousTile.readyToKnockout = true;
    //                    //    detectors[i].activeTile.readyToKnockout = true;
    //                    //}
    //                    //detectors[i].activeTile.Knockout();
    //                    //detectors[i].activeTile.transform.localScale = new Vector3(1f, 1f, .1f);

    //                }
    //            }
    //            else
    //            {
    //                numberOfConsecutiveTiles = 1;
    //            }
    //        }
    //        previousPreviousTile = previousTile;
    //        previousTile = detectors[i].activeTile;
    //    }
    //}

    //private static void ResetTileCombined(int index)
    //{
    //    if (detectors[index].activeTile.combined == true)
    //    {
    //        detectors[index].activeTile.combined = false;
    //    }
    //}

    //private void CheckColumns()
    //{
    //    previousTile = previousPreviousTile = null;

    //    for (int i = 0; i < 4; i++)
    //    {
    //        for (int j = 0; j < 4; j++)
    //        {

    //            if ((j) % 4 == 0)
    //            {
    //                //Debug.Log("streak reset");
    //                numberOfConsecutiveTiles = 1;
    //                previousTile = previousPreviousTile = null;
    //            }
    //            //ResetTileCombined(i + (j * 4));
    //            //Debug.Log("About to check: " + detectors[i + (j * 4)].activeTile.name);
    //            if (previousTile != null)
    //            {
    //                if (detectors[i + (j * 4)].activeTile.type == previousTile.type)
    //                {
    //                    //Debug.Log("2 in a row detected: " + detectors[i + (j * 4)].activeTile.name + " and " + previousTile.name);
    //                    numberOfConsecutiveTiles++;
    //                    if (numberOfConsecutiveTiles > 1)
    //                    {
    //                        if (!previousTile.combined)
    //                        {
    //                            foreach (TileCluster t in tileClusters)
    //                            {
    //                                if (previousTile.currentCluster == t)
    //                                {
    //                                    if (!t.Contains(previousTile))
    //                                    {
    //                                        t.AddTileToCluster(previousTile);
    //                                    }
    //                                }
    //                            }
    //                            previousTile.combined = true;
    //                            //previousTile.Knockout();
    //                            //previousTile.gameObject.transform.localScale = new Vector3(1f, 1f, .1f);
    //                            //previousTile.TintColor();
    //                            if (!combinedTiles.Contains(previousTile))
    //                            {
    //                                combinedTiles.Add(previousTile);
    //                            }
    //                        }
    //                        detectors[i + (j * 4)].activeTile.combined = true;
    //                        if (!combinedTiles.Contains(detectors[i + (j * 4)].activeTile))
    //                        {
    //                            foreach (TileCluster t in tileClusters)
    //                            {
    //                                if (previousTile.currentCluster == t)
    //                                {
    //                                    if (!t.Contains(detectors[i + (j * 4)].activeTile))
    //                                    {
    //                                        t.AddTileToCluster(detectors[i + (j * 4)].activeTile);
    //                                    }
    //                                }
    //                            }
    //                            combinedTiles.Add(detectors[i + (j * 4)].activeTile);
    //                        }
    //                        //if (numberOfConsecutiveTiles > 2)
    //                        //{
    //                        //    previousPreviousTile.readyToKnockout = true;
    //                        //    previousTile.readyToKnockout = true;
    //                        //    detectors[i + (j * 4)].activeTile.readyToKnockout = true;
    //                        //}
    //                        //if(previousTile.readyToKnockout)
    //                        //{
    //                        //    detectors[i + (j * 4)].activeTile.readyToKnockout = true;
    //                        //}
    //                    }
    //                }
    //                else
    //                {
    //                    numberOfConsecutiveTiles = 1;
    //                }
    //            }
    //            previousPreviousTile = previousTile;
    //            previousTile = detectors[i + (j * 4)].activeTile;
    //        }
    //    }
    //}

    private void CheckTiles()
    {
        foreach (Detector d in detectors)
        {
            d.activeTile.DetectNeighborTiles();
        }
    }

    void KnockoutTiles()
    {
        foreach (Detector d in detectors)
        {
            if (d.activeTile.readyToKnockout == true)
            {
                d.activeTile.Knockout();
            }
        }
    }

    void KnockoutCombineds()
    {
        foreach (Tile t in combinedTiles)
        {
            t.Knockout();
        }
        combinedTiles.Clear();
    }

    void KnockoutClusters()
    {
        foreach (TileCluster t in tileClusters)
        {
            if (t.ReadyToKnockout)
            {
                foreach (Tile tile in t.GetTiles())
                {
                    tile.Knockout();
                }
            }
        }
        tileClusters.Clear();
        combinedTiles.Clear();
    }

    void HighlightClusters()
    {
        foreach (TileCluster t in tileClusters)
        {
            if (t.ReadyToKnockout)
            {
                foreach (Tile tile in t.GetTiles())
                {
                    tile.currentDetector.combined = true;
                }
            }
        }
    }
}
