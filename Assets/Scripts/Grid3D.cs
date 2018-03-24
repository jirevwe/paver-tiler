using System;
using System.Collections.Generic;
using UnityEngine;

public class Grid3D : MonoBehaviour
{
    public static Grid3D Instance;

    public Action<int, int> OnGridResized;

    [Range(1, 10)] public int gridX = 1;
    [Range(1, 10)] public int gridZ = 1;

    public Vector2 min, max;

    public bool editMode = false;

    public int currentIndex = 0;

    [Header("Block Types")]
    public GameObject tile;
    GameObject start;

    [Header("Other Stuff")]
    public GameObject tileHolder;

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
        var start = GameObject.FindGameObjectWithTag("start");

        for (int i = 0; i < gridX; i++)
        {
            for (int j = 0; j < gridZ; j++)
            {
                this[i, j] = new Node(new Vector3(i, 0, j), i, j, true);
            }
        }

        for (int i = 0; i < gridX; i++)
        {
            for (int j = 0; j < gridZ; j++)
            {
                Transform child = start.transform.Find(string.Format("{0}, {1}", i, j).ToString());
                GameObject tile = child.gameObject;

                this[i, j] = new Node(new Vector3(i, 0, j), i, j, true);

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

    public IEnumerator<WaitForEndOfFrame> DoPostUpdate()
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
                    L.SafeDestroy(tile);
                }
            }
        }

        nodeGrid.Clear();
        nodes.Clear();

        for (int i = 0; i < gridX; i++)
        {
            for (int j = 0; j < gridZ; j++)
            {
                Transform child = start.transform.Find(string.Format("{0}, {1}", i, j).ToString());
                if (child == null)
                {
                    GameObject g = GameObject.Instantiate(tile, new Vector3(i, 0, j), Quaternion.identity);
                    g.name = string.Format("{0}, {1}", i, j);
                    g.transform.parent = start.transform;
                    g.GetComponent<Tile3D>().node = new Node(g.transform.position, i, j);

                    nodes.Add(g);
                    nodeGrid.Add(g.GetComponent<Tile3D>().node);
                }
            }
        }

        if (OnGridResized != null) OnGridResized(gridX, gridZ);
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
                        nodes[(i * gridZ) + j].GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/square");
                    }
                    else
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
                        this[i, j] = new Node(new Vector3(i, 0, j), i, j, true);
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