using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid3D : MonoBehaviour
{
    public static Grid3D Instance;

    public Action<int, int> OnGridResized;

    [Range(1, 10)] public int gridX = 1;
    [Range(1, 10)] public int gridZ = 1;

    public Vector2Int min, max;

    public bool editMode = false;

    public int currentIndex = 0;

    [Header("Block Types")]
    public GameObject tile;
    GameObject start;

    [Header("Other Stuff")]
    public GameObject tileHolder;

    [SerializeField]
    public Material[] nodeMaterials;
    public int materialIndex = 0;
    
    public List<GameObject> nodes = new List<GameObject>();
    public List<Node> nodeGrid = new List<Node>();

    public Node this[int x, int z]
    {
        get
        {
            return (x < 0 || z < 0 || x >= gridX || z >= gridZ) ? null : nodeGrid[(x * gridZ) + z];
        }
        set
        {
            nodeGrid[(x * gridZ) + z] = value;
        }
    }

    void OnEnable()
    {
        Instance = this;
    }

    void Awake()
    {
        RunScan();
    }

    void Start()
    {
        start = GameObject.FindGameObjectWithTag("start");
    }

    public void RunScan()
    {
        GameObject start = GameObject.FindGameObjectWithTag("start");

        for (int i = 0; i < gridX; i++)
        {
            for (int j = 0; j < gridZ; j++)
            {
                Transform child = start.transform.Find(string.Format("{0}, {1}", i, j).ToString());
                GameObject tile = child.gameObject;

                this[i, j] = new Node(new Vector3Int(i, 0, j), i, j, true);

                nodes.Add(tile);
            }
        }
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        return this[(int)worldPosition.x, (int)worldPosition.z];
    }

    public GameObject GetTile(Node node)
    {
        return nodes.Find(n => n.GetComponent<BlockItem>().node.gridX == node.gridX && n.GetComponent<BlockItem>().node.gridZ == node.gridZ);
    }

    public IEnumerator DoPostUpdate()
    {
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < max.x; i++)
        {
            for (int j = 0; j < max.y; j++)
            {
                Transform child = start.transform.Find(string.Format("{0}, {1}", i, j).ToString());
                if (child != null)
                {
                    GameObject tile = child.gameObject;
                    tile.GetComponent<Tile3D>().Disable();
                }
            }
        }

        nodeGrid.Clear();
        nodes.Clear();

        yield return new WaitForSeconds(0.01f);

        for (int i = 0; i < gridX; i++)
        {
            for (int j = 0; j < gridZ; j++)
            {
                Transform child = start.transform.Find(string.Format("{0}, {1}", i, j).ToString());
                if (child == null)
                {
                    Tile3DData tile3DData = new Tile3DData
                    {
                        name = string.Format("{0}, {1}", i, j),
                        parent = start.transform
                    };

                    PoolManager.instance.ReuseObject<Tile3DData>(tile, new Vector3Int(i, 0, j), Quaternion.identity, tile3DData);
                }
            }
        }

        if (OnGridResized != null) OnGridResized(gridX, gridZ);
    }

    public void HoverOnTile(Node node)
    {
        try
        {
            int x = node.gridX, z = node.gridZ;

            for (int i = 0; i < gridX; i++)
            {
                for (int j = 0; j < gridZ; j++)
                {
                    if (i == x && j == z)
                    {
                        nodes[(i * gridZ) + j].GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/square transparent");
                    }
                }
            }
        }
        catch (ArgumentOutOfRangeException)
        {

        }
    }

    public void SelectTile(Node node)
    {
        try
        {
            int x = node.gridX, z = node.gridZ;

            for (int i = 0; i < gridX; i++)
            {
                for (int j = 0; j < gridZ; j++)
                {
                    if (i == x && j == z)
                    {
                        nodes[(i * gridZ) + j].GetComponent<MeshRenderer>().material = nodeMaterials[materialIndex];
                    }
                    // else
                    // {
                    //     nodes[(i * gridZ) + j].GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/square transparent");
                    // }
                }
            }
        }
        catch (ArgumentOutOfRangeException)
        {

        }
    }

    public void ResetTiles()
    {
        try
        {
            for (int i = 0; i < gridX; i++)
            {
                for (int j = 0; j < gridZ; j++)
                {
                    nodes[(i * gridZ) + j].GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/square");
                }
            }
        }
        catch (ArgumentOutOfRangeException)
        {

        }
    }

    void OnDrawGizmos()
    {
        try
        {
            if (!Application.isPlaying)
            {
                nodeGrid.Clear();
                for (int i = 0; i < gridZ * gridX; i++)
                {
                    nodeGrid.Add(new Node());
                }

                for (int i = 0; i < gridX; i++)
                {
                    for (int j = 0; j < gridZ; j++)
                    {
                        this[i, j] = new Node(new Vector3Int(i, 0, j), i, j, true);
                    }
                }
            }

            if (editMode)
            {
                for (int i = 0; i < gridX * gridZ; i++)
                {
                    Gizmos.DrawWireCube(nodeGrid[i].worldPosition, Vector3.one);
                }
            }
        }
        catch (ArgumentOutOfRangeException e)
        {
            L.e(e);
        }
    }
}