using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile3D : PoolObject
{
    [SerializeField]
    Transform parent;
    string tempName = "tile (Clone)";
    public Node node;

    void Start()
    {
        if (parent == null) parent = GameObject.Find("Tile pool").transform;
    }

    public override void OnObjectReuse<T>(T args)
    {
        base.OnObjectReuse(args);

        Tile3DData data = args as Tile3DData;

        name = data.name;
        transform.parent = data.parent;

        GetComponent<Tile3D>().node = new Node
        {
            worldPosition = new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z),
            gridX = (int)transform.position.x,
            gridZ = (int)transform.position.z,
            colored = false,
            walkable = false
        };

        Grid3D.Instance.nodes.Add(gameObject);
        Grid3D.Instance.nodeGrid.Add(GetComponent<Tile3D>().node);
    }

    public override void Disable()
    {
        name = tempName;
        transform.parent = parent;
        base.Disable();
    }
}
