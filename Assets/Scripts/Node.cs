using UnityEngine;
using System;

[System.Serializable]
public class Node
{
    private Vector3 worldPositionRaw = Vector3.zero;

    public Vector3 worldPosition
    {
        get { return worldPositionRaw; }
        set { worldPositionRaw = value; }
    }

    public int gridX = 0;

    public int gridZ = 0;

    public bool walkable = false;

    public bool colored = false;

    public Node(Vector3 _worldPos, int _gridX, int _gridZ, bool _walkable = true)
    {
        walkable = _walkable;
        worldPositionRaw = _worldPos;
        gridX = _gridX;
        gridZ = _gridZ;
    }

    public Node()
    {
    }

    public override int GetHashCode()
    {
        return (int)System.DateTime.Now.Ticks;
    }

    public override bool Equals(System.Object obj)
    {
        Node node = (Node)obj;
        if (node == null || this == null) return false;
        return node.gridX == gridX && gridZ == node.gridZ;
    }

    public void ResetNode()
    {
        worldPositionRaw = Vector3.zero;
        gridX = -1;
        gridZ = -1;
    }

    public override string ToString()
    {
        return gridX + ", " + gridZ;
    }
}