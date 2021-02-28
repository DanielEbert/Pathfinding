using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour {

    string state = "Idle";

    public GameObject tower;

    public LayerMask mousePosRaycastLM;

    public Camera cam;

    public static List<GameObject> units = new List<GameObject>();

    RaycastHit CamRaycast()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 500, mousePosRaycastLM))
        {
            return hit;
        }
        return new RaycastHit();
    }

    void Update()
    {
        if(state == "Idle")
        {
            if(Input.GetMouseButtonDown(0))
            {
                RaycastHit hit = CamRaycast();

                if (hit.collider != null)
                {
                    if(hit.transform.gameObject.layer == 10)    //10: Tower
                    {

                    }
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            state = "TowerInHand";
        }

        if(state == "TowerInHand")
        {
            TowerInHand();
        }
    }

    void TowerInHand()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit = CamRaycast();

            if (hit.transform.gameObject.layer == 9 && hit.transform.parent.localScale.y >= 2)
            {
                GameObject go = Instantiate(tower);
                go.transform.localScale = .5f * Vector3.one;
                go.transform.position = hit.transform.parent.position + new Vector3(0, hit.transform.parent.localScale.y);
                hit.transform.gameObject.layer = 0;
                //go.Getcomp Tower, hexagonPlatform = hit.transform.parent;

                state = "Idle";
            }
        }
    }
}
