using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    public bool walking = true;

    float moveSpeed = 2;

    int currentIndexPos = 0;
    float passageCompleted = 0f;

    void Awake()
    {
        TowerManager.units.Add(this.gameObject);
    }

	void Update () {
        Moveing();
	}

    void Moveing()
    {
        if (MapGenerator.path.Count <= (currentIndexPos + 1) || !walking) return;

        Node currentNode = MapGenerator.path[currentIndexPos];

        Vector3 pos = Vector3.Lerp(currentNode.worldPosition, MapGenerator.path[currentIndexPos + 1].worldPosition, passageCompleted);
        pos.y = MapGenerator.currentPoly[currentNode.gridX, currentNode.gridY].transform.localScale.y;
        transform.position = pos;

        passageCompleted += Time.deltaTime * moveSpeed;

        if(passageCompleted >= 1)
        {
            passageCompleted = 0;
            currentIndexPos++;
        }
    }
}
