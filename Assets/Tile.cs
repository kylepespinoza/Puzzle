using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{

    MeshRenderer meshRenderer;
    Collider col;

    public TileManager.TileType type;

    private bool _isCombined;
    private int _combinedCount;

    public bool IsCombined { get { return _isCombined; } set { _isCombined = value; } }

    public bool combined;

    public float forwardBlastForce;
    public float torqueForce;

    Rigidbody rigid;

    public Detector currentDetector;

    Quaternion startingRot;

    //Which column is this tile in?
    public int columnIndex;

    public bool readyToKnockout;

    public TileCluster currentCluster = new TileCluster();

    public List<Tile> combinedTiles = new List<Tile>();

    LayerMask layerMask = new LayerMask();

    private int combinedCount = 1;
    public int CombinedCount
    {
        get { return combinedCount; }
        set
        {
            //If combined count becomes less than 2, dont knockout
            if (value < 2 && readyToKnockout == true)
            {
                readyToKnockout = false;
            }
            combinedCount = value;
        }
    }

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        col = GetComponent<Collider>();
        rigid = GetComponent<Rigidbody>();
    }

    void Start()
    {
        //GetComponent<MeshRenderer>().material = colors[Random.Range(0, colors.Length)];
        startingRot = transform.rotation;
        layerMask = LayerMask.GetMask("Tile");
    }

    void Update()
    {
    }

    public void DestroyTile()
    {
        //meshRenderer.enabled = false;
        //col.enabled = false;
        Knockout();
    }

    public void RecieveType(TileManager.TileType tileType, Material material)
    {
        type = tileType;
        meshRenderer.material = material;
    }

    public void Knockout()
    {
        //seperate into physics logic and other logic
        rigid.constraints = RigidbodyConstraints.None;
        rigid.AddForce(new Vector3(0f, Random.Range(0f, 4), forwardBlastForce), ForceMode.Impulse);
        rigid.AddRelativeTorque(new Vector3(Random.Range(-torqueForce, torqueForce), Random.Range(-torqueForce, torqueForce), Random.Range(-torqueForce, torqueForce)));
        col.enabled = false;
        StartCoroutine(RemoveFromDetector());
        StartCoroutine(ResetAfterTime());
        TileManager.Instance.columnIndicesForRespawn.Add(columnIndex);

    }

    public IEnumerator RemoveFromDetector()
    {
        yield return new WaitForEndOfFrame();
        if (currentDetector != null)
        {
            currentDetector.RemoveActiveTile();
            currentDetector = null;
            readyToKnockout = false;
            //Debug.Log("Removed: " + gameObject.name + " from detector");

        }
    }

    public void AddToDetector(Detector d)
    {
        currentDetector = d;
    }

    private IEnumerator ResetAfterTime()
    {
        combined = false;
        yield return new WaitForSeconds(1.5f);
        transform.rotation = startingRot;
        rigid.Sleep();
        rigid.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        col.enabled = true;
        TileManager.Instance.AddTileToRespawnList(this);
        //Debug.Log("Tile: " + name + " added to respawn list");
    }

    public void ResetTile(Vector3 position)
    {
        rigid.Sleep();
        transform.position = position;
        gameObject.layer = LayerMask.NameToLayer("Default");
    }

    public void AddToCluster(TileCluster cluster)
    {
        currentCluster = cluster;
    }

    public void DetectNeighborTiles()
    {
        if(gameObject.layer != LayerMask.NameToLayer("Tile"))
        {
            gameObject.layer = LayerMask.NameToLayer("Tile");

        }
        ResetCombine();
        DetectInDirection(Vector3.right);
        DetectInDirection(Vector3.down);
        DetectInDirection(Vector3.up);
        DetectInDirection(Vector3.left);

    }

    private RaycastHit DetectInDirection(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Tile"))
            {
                //Debug.Log(hit.collider.gameObject.name);
                Tile t = hit.collider.gameObject.GetComponent<Tile>();
                //Debug.Log("I'm " + gameObject.name + " and I'm comparing against " + t.gameObject.name);
                if (t.type == type)
                {
                    //Debug.Log("Combine!");
                    if (!combinedTiles.Contains(t))
                    {
                        combinedTiles.Add(t);
                        CombinedCount++;
                    }

                    if (!t.combinedTiles.Contains(this))
                    {
                        t.combinedTiles.Add(this);
                        t.CombinedCount++;

                    }

                    StartCoroutine(CheckCombinedCountAfterTime(t));
                }
            }
        }

        return hit;
    }

    private void ResetCombine()
    {
        readyToKnockout = false;
        combinedTiles.Clear();
        combinedCount = 1;
    }

    void CheckCombinedCount()
    {
        //Debug.Log(gameObject.name + "'s combined count is: " + CombinedCount);
        if (CombinedCount > 2)
        {
            DestroySelfAndNeighbors();
        }
    }

    void DestroySelfAndNeighbors()
    {
        readyToKnockout = true;
        //Knockout();
        Debug.Log("Marked for knockout!");
        foreach (Tile t in combinedTiles)
        {
            t.readyToKnockout = true;
            //t.Knockout();
        }
    }

    IEnumerator CheckCombinedCountAfterTime(Tile neighbor)
    {
        yield return new WaitForEndOfFrame();
        CheckCombinedCount();
        neighbor.CheckCombinedCount();
    }

    //public void TintColor()
    //{
    //    meshRenderer.material.color = new Color(meshRenderer.material.color.r + .2f, meshRenderer.material.color.g + .2f, meshRenderer.material.color.b + .2f, meshRenderer.material.color.a);
    //}
}
