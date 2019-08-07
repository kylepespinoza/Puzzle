using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Detector : MonoBehaviour
{

    public Tile activeTile;

    public int columnIndex;

    public bool combined;

    void Start()
    {

    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        activeTile = other.GetComponent<Tile>();
        activeTile.AddToDetector(this);
        activeTile.columnIndex = columnIndex;
        activeTile.gameObject.layer = LayerMask.NameToLayer("Tile");
        //Debug.Log("New Active Tile: " + activeTile.name);
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("trigger exit");
        if (other.GetComponent<Tile>() == activeTile)
        {
            activeTile.gameObject.layer = LayerMask.NameToLayer("Default");
            RemoveActiveTile();
        }
    }

    public void RemoveActiveTile()
    {
        DetectorManager.Instance.CheckDetectorAfterDelay();

        if (activeTile != null)
        {
            //Debug.Log("Active Tile: " + activeTile.name + " Removed");
            activeTile = null;
        }
    }

    void OnDrawGizmos()
    {

        if (activeTile != null)
        {
            if (activeTile.readyToKnockout)
            {
                Gizmos.color = Color.yellow;
            }
        }
        Gizmos.DrawCube(transform.position, new Vector3(.5f, .5f, .5f));
    }
}
