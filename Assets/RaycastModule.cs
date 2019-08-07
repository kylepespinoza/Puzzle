using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastModule : MonoBehaviour
{

    public Camera cam;

    public LayerMask checkingLayers;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 20f, checkingLayers))
            {
                Transform objectHit = hit.transform;
                //Debug.Log("Hit: " + objectHit.name);
                objectHit.gameObject.GetComponent<Tile>().DestroyTile();
            }
        }
    }

}
