using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BlockItem : MonoBehaviour
{
    public List<BlockItem> connections = new List<BlockItem>();
    public List<int> connection = new List<int>();
    public int direction = 0;

    [SerializeField]
    private float shakeTime = 0.1f;
    [SerializeField]
    private float shakeSpeed = 1.0f;
    public bool isPowered;
    public bool canRotate;
    public bool isPowerSource;
    public bool isDestination;
    public Node node;

    void Start()
    {
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        yield return new WaitForSeconds(1f);
        transform.rotation = Quaternion.Euler(new Vector3(0, 90 * direction, 0));
        RunPowerCheck();
    }

    void StartShake()
    {
        float shakeAmt = (Mathf.Cos(Time.time * shakeSpeed) * 180) / Mathf.PI * 0.5f;
        shakeAmt = Mathf.Clamp(shakeAmt, -15, 15);
        transform.localRotation = Quaternion.Euler(0, shakeAmt, 0);
        Invoke("StopShake", shakeTime);
    }

    void StopShake()
    {
        transform.localRotation = Quaternion.identity;
        connection.Find(n => n == 1);
        connection.ToArray();
    }

    public bool Rotate()
    {
        if (canRotate && !done)
        {
            direction = (direction + 1) % 4;
            RunPowerCheck();
            DoRotate();
            return true;
        }
        return false;
    }

    void DoRotate()
    {
        var rotation = transform.eulerAngles;
        rotation.y += 90f;
        StartRotate(rotation);
        done = true;
    }

    public static void RunPowerCheck()
    {
        List<GameObject> powerSources = new List<GameObject>();

        for (int i = 0; i < Grid3D.Instance.nodes.Count; i++)
        {
            GameObject t = Grid3D.Instance.nodes[i];
            t.GetComponent<BlockItem>().isPowered = t.GetComponent<BlockItem>().isPowerSource;

            if (t.GetComponent<BlockItem>().isPowered)
            {
                powerSources.Add(t);
            }
        }

        // Now spread power from all powered tiles.
        for (int i = 0; i < powerSources.Count; i++)
        {
            powerSources[i].GetComponent<BlockItem>().SpreadPower();
        }
    }
    
    void SpreadPower()
    {
        List<int> potentialNeighbours = new List<int>();

        //NORTH
        if (node.gridZ < Grid3D.Instance.gridZ - 1)
            potentialNeighbours.Add(0);

        //SOUTH
        if (node.gridZ > 0)
            potentialNeighbours.Add(2);

        //WEST
        if (node.gridX > 0)
            potentialNeighbours.Add(3);

        //EAST
        if (node.gridX < Grid3D.Instance.gridX - 1)
            potentialNeighbours.Add(1);

        this.connections.Clear();

        for (int i = 0; i < potentialNeighbours.Count; i++)
        {
            // if we have a connection
            bool haveConnect = IsConnectedTo(potentialNeighbours[i]);

            if (haveConnect)
            {
                BlockItem t = GetNeighbourByDir(potentialNeighbours[i]).GetComponent<BlockItem>();
                connections.Add(t);

                if (!t.isPowered)
                {
                    t.isPowered = true;
                    t.SpreadPower();
                }
            }
        }
    }

    public GameObject GetNeighbourByDir(int dir)
    {
        Node otherNode = null;

        if (dir == 0)
            otherNode = Grid3D.Instance[node.gridX, node.gridZ + 1];
        if (dir == 1)
            otherNode = Grid3D.Instance[node.gridX + 1, node.gridZ];
        if (dir == 2)
            otherNode = Grid3D.Instance[node.gridX, node.gridZ - 1];
        if (dir == 3)
            otherNode = Grid3D.Instance[node.gridX - 1, node.gridZ];

        return Grid3D.Instance.nodes.Find(n => n.GetComponent<BlockItem>().node.Equals(otherNode));
    }

    public bool CheckConnection(int neighbourDirection)
    {
        try
        {
            BlockItem n = GetNeighbourByDir(neighbourDirection).GetComponent<BlockItem>();
            return connections.Find(x => x.node.Equals(n.node)) != null && IsConnectedTo(neighbourDirection);
        }
        catch (NullReferenceException)
        {
            return false;
        }
    }

    public bool IsConnectedTo(int neighbourDirection)
    {
        // NESW -> 0123
        if (HasConnectionTowards(neighbourDirection))
        {
            // I connect, but do they?
            BlockItem n = GetNeighbourByDir(neighbourDirection).GetComponent<BlockItem>();
            return n.HasConnectionTowards(neighbourDirection + 2);
        }

        return false;
    }

    public bool HasConnectionTowards(int dir) { return connection[(dir - direction + 4) % 4] == 1; }

    // Rotate the object from it's current rotation to "newRotation" over "duration" seconds
    void StartRotate(Vector3 newRotation, float duration = 0.5f)
    {
        if (SlerpRotation != null) // if the rotate coroutine is currently running, so let's stop it and begin rotating to the new rotation.
            StopCoroutine(SlerpRotation);

        SlerpRotation = Rotate(newRotation, duration);
        StartCoroutine(SlerpRotation);
    }

    IEnumerator SlerpRotation = null;
    bool done = false;

    IEnumerator Rotate(Vector3 newRotation, float duration)
    {
        Quaternion startRotation = transform.rotation; // You need to cache the current rotation so that you aren't slerping from the updated rotation each time
        Quaternion endRotation = Quaternion.Euler(newRotation);

        for (float elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
        {
            float t = elapsed / duration; // This is the normalized time. It will move from 0 to 1 over the duration of the loop.
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            yield return null;
        }

        done = false;

        transform.rotation = endRotation; // finally, set the rotation to the end rotation
        SlerpRotation = null; // Clear out the IEnumerator variable so we can tell that the coroutine has ended.
    }
}