using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraController : MonoBehaviour
{
    Vector3 cameraPosition = Vector3.zero;

    void OnEnable()
    {
        Grid3D.Instance.OnGridResized += OnGridResized;
    }

    private void OnGridResized(int gridX, int gridZ)
    {
        cameraPosition.z = gridZ % 2 == 0 ? gridZ / 2 - .5f : gridZ / 2;
        cameraPosition.x = gridX % 2 == 0 ? gridX / 2 - .5f : gridX / 2;

        cameraPosition.y = gridX > gridZ ? gridX * 1.5f: gridZ * 1.5f;

        transform.position = cameraPosition;
    }

    void Awake()
    {
        transform.position = cameraPosition;
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
