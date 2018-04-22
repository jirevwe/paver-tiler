using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePool : MonoBehaviour
{
    [SerializeField]
    PoolManager poolManager;

    [SerializeField]
	int count;

    // Use this for initialization
    void Awake()
    {
        poolManager.CreatePool(Grid3D.Instance.tile, count);
    }
}
