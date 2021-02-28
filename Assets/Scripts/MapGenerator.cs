using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    public int mapWidth;
    public int mapHeight;
    public float noiseScale;

    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public bool autoUpdate; 

    public Transform mapBody;
    public GameObject polygon;
    public static GameObject[,] currentPoly;
    public bool clear = false;
    public Material mat;

    Node[,] grid;
    public static List<Node> path = new List<Node>();
    public static float currentPathWalkCost = 0;

    public Material[] mats;

    //void Start()
    //{
    //    GenerateMap();
    //}

    public void GenerateMap()
    {
        ClearOldMap();

        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);

        currentPoly = new GameObject[mapWidth, mapHeight];

        int rowCount = 0;
        for(int y = 0; y < mapHeight; y++) {
            for(int x = 0; x < mapWidth; x++) {
                Vector3 pos = new Vector3(x,0,y * .75f);
                if (rowCount % 2 == 1) pos.x += .5f;
                GameObject pol = Instantiate(polygon, pos, Quaternion.identity);

                pol.transform.localScale = new Vector3(1, 1 + noiseMap[x, y] * 5, 1);
                pol.transform.parent = mapBody;

                HexagonData hData = pol.GetComponent<HexagonData>();
                hData.posX = x;
                hData.posY = y;
                hData.height = 1 + noiseMap[x, y] * 5;

                currentPoly[x, y] = pol;
            }
            rowCount++;
        }

        CreateGrid();

        foreach(GameObject go in currentPoly)
        {
            ApplyColor(go);
        }
        
        FindPath(0,0,49,49);
    }

    void CreateGrid()
    {
        grid = new Node[mapWidth, mapHeight];

        int rowCount = 0;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                Vector3 pos = new Vector3(x, 0, y * .75f);
                if (rowCount % 2 == 1) pos.x += .5f;
                grid[x, y] = new Node(!(currentPoly[x, y].transform.localScale.y < 2), pos, x, y);
            }
            rowCount++;
        }
    }

    void ApplyColor(GameObject go)
    {
            MeshRenderer ren = go.GetComponentInChildren<MeshRenderer>();
            
            HexagonData hData = go.GetComponent<HexagonData>();
            float height = hData.height;

            if (height < 2)
            {
                ren.material = mats[0];
                hData.heightType = 0;
            }
            else if (height < 4)
            {
                ren.material = mats[1];
                hData.heightType = 1;
            }
            else if (height < 5.5f)
            {
                ren.material = mats[2];
                hData.heightType = 2;
            }
            else
            {
                ren.material = mats[3];
                hData.heightType = 3;
            }
    }

    public void FindPath(int startNodeX, int startNodeY, int targetNodeX, int targetNodeY)
    {
        //Color Back old Path
        for(int i = 0; i < path.Count; i++)
        {
            ApplyColor(currentPoly[path[i].gridX, path[i].gridY]);
        }
        currentPathWalkCost = 0;

        Node startNode = grid[startNodeX, startNodeY];
        Node targetNode = grid[targetNodeX, targetNodeY];

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);
        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for(int i = 1; i < openSet.Count; i++)
            {
                if(openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if(currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }
            
            foreach(Node neightbour in GetNeightbour(currentNode))
            {
                if(!neightbour.walkable || closedSet.Contains(neightbour))
                {
                    continue;
                }

                float newMovementCostToNeightbour = currentNode.gCost + GetDistance(currentNode, neightbour);
                if(newMovementCostToNeightbour < neightbour.gCost || !openSet.Contains(neightbour))
                {
                    neightbour.gCost = newMovementCostToNeightbour;
                    neightbour.hCost = GetDistance(neightbour, targetNode);
                    neightbour.parent = currentNode;

                    if(!openSet.Contains(neightbour))
                    {
                        openSet.Add(neightbour);
                    }
                }
            }
        }
    }

    void RetracePath(Node startNode, Node endNode)
    {
        path.Clear();
        Node currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Add(startNode);

        path.Reverse();

        foreach(Node node in path)
        {
            currentPoly[node.gridX, node.gridY].GetComponentInChildren<MeshRenderer>().material = mat;
            currentPathWalkCost += node.hCost;
        }
    }

    float GetDistance(Node a, Node b)
    {
        return Vector3.Distance(a.worldPosition, b.worldPosition) + Mathf.Pow(currentPoly[a.gridX, a.gridY].transform.localScale.y - currentPoly[b.gridX, b.gridY].transform.localScale.y, 2) * 1000;

    }

    List<Node> GetNeightbour(Node node)
    {
        List<Node> neightbours = new List<Node>();

        if(node.gridY % 2 == 1)
        {
            if (node.gridX >= 0 && node.gridX < mapWidth && node.gridY - 1 >= 0 && node.gridY - 1 < mapHeight) neightbours.Add(grid[node.gridX, node.gridY - 1]);
            if (node.gridX + 1 >= 0 && node.gridX + 1 < mapWidth && node.gridY - 1 >= 0 && node.gridY - 1 < mapHeight) neightbours.Add(grid[node.gridX + 1, node.gridY - 1]);
            if (node.gridX + 1 >= 0 && node.gridX + 1 < mapWidth && node.gridY >= 0 && node.gridY < mapHeight) neightbours.Add(grid[node.gridX + 1, node.gridY]);
            if (node.gridX >= 0 && node.gridX < mapWidth && node.gridY + 1 >= 0 && node.gridY + 1 < mapHeight) neightbours.Add(grid[node.gridX, node.gridY + 1]);
            if (node.gridX + 1 >= 0 && node.gridX + 1 < mapWidth && node.gridY + 1 >= 0 && node.gridY + 1 < mapHeight) neightbours.Add(grid[node.gridX + 1, node.gridY + 1]);
            if (node.gridX - 1 >= 0 && node.gridX - 1 < mapWidth && node.gridY >= 0 && node.gridY < mapHeight) neightbours.Add(grid[node.gridX - 1, node.gridY]);
        }
        else
        {
            if (node.gridX >= 0 && node.gridX < mapWidth && node.gridY - 1 >= 0 && node.gridY - 1 < mapHeight) neightbours.Add(grid[node.gridX, node.gridY - 1]);
            //if (node.gridX + 1 >= 0 && node.gridX + 1 < mapWidth && node.gridY - 1 >= 0 && node.gridY - 1 < mapHeight) neightbours.Add(grid[node.gridX + 1, node.gridY - 1]);
            if (node.gridX - 1 >= 0 && node.gridX - 1 < mapWidth && node.gridY - 1 > 0 && node.gridY - 1 < mapHeight) neightbours.Add(grid[node.gridX - 1, node.gridY - 1]);
            if (node.gridX + 1 >= 0 && node.gridX + 1 < mapWidth && node.gridY >= 0 && node.gridY < mapHeight) neightbours.Add(grid[node.gridX + 1, node.gridY]);
            if (node.gridX >= 0 && node.gridX < mapWidth && node.gridY + 1 >= 0 && node.gridY + 1 < mapHeight) neightbours.Add(grid[node.gridX, node.gridY + 1]);
            //if (node.gridX + 1 >= 0 && node.gridX + 1 < mapWidth && node.gridY + 1 >= 0 && node.gridY + 1 < mapHeight) neightbours.Add(grid[node.gridX + 1, node.gridY + 1]);
            if (node.gridX - 1 >= 0 && node.gridX - 1 < mapWidth && node.gridY + 1 > 0 && node.gridY + 1 < mapHeight) neightbours.Add(grid[node.gridX - 1, node.gridY + 1]);
            if (node.gridX - 1 >= 0 && node.gridX - 1 < mapWidth && node.gridY >= 0 && node.gridY < mapHeight) neightbours.Add(grid[node.gridX - 1, node.gridY]);
        }

        return neightbours;
    }

    void ClearOldMap()
    {
        if (currentPoly != null)
        {
            foreach (GameObject go in currentPoly)
            {
                Destroy(go.gameObject);
            }
        }
    }

    void OnValidate()
    {
        if (mapWidth < 1)
            mapWidth = 1;

        if (mapHeight < 1)
            mapHeight = 1;

        if (lacunarity < 1)
            lacunarity = 1;

        if (octaves < 0)
            octaves = 0;

        if (clear)
        {
            clear = false;
            ClearOldMap();
        }
    }
}
