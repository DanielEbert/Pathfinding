using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour {

    int playerPosX, playerPosY;

    public LayerMask mouseScanLM;

    [HideInInspector]
    public GameObject playerIns;
    HexagonData playerHexagon;
    public MapGenerator mapGenerator;

    public GameObject playerPrefab;
    public Camera mainCam;
    //public GameObject playerUIPanel;

    void Awake()
    {
        mapGenerator = GetComponent<MapGenerator>();
    }

    void Start()
    {
        mapGenerator.GenerateMap();

        //Start at 22/24
        SpawnPlayer(22, 24);
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 500, mouseScanLM))
            {
                if (hit.transform.gameObject.layer == 9) //Hexagon Layer
                {
                    HexagonData hD = hit.transform.parent.GetComponent<HexagonData>();
                    //Color Path, uncolor path when clicking somewhere else
                    mapGenerator.FindPath(playerPosX, playerPosY, hD.posX, hD.posY);
                }
                else if(hit.transform.gameObject.layer == 8) //Player Layer
                {
                    //playerUIPanel.SetActive(!playerUIPanel.activeSelf);
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape) == true)
        {
            Application.Quit();
        }
    }

    void SpawnPlayer(int posX, int posY)
    {
        playerPosX = posX;
        playerPosY = posY;
        GameObject poly = MapGenerator.currentPoly[posX, posY];
        if(poly.GetComponent<HexagonData>().heightType == 0)
        {
            Debug.Log("Player can't spawn in Water!");
            return;
        }

        Vector3 pos = poly.transform.position;

        playerIns = Instantiate(playerPrefab, new Vector3(pos.x, poly.GetComponent<HexagonData>().height, pos.z), Quaternion.identity);
        playerHexagon = poly.GetComponent<HexagonData>();
    }
}
