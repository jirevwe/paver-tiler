﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public static InputController Instance;

    [SerializeField]
    public Tile3D touchedTile;

    [SerializeField]
    public Tile3D hoveredTile;

    GameObject start, end;
    RaycastHit raycastHit;

    [SerializeField]
    LayerMask layerForDetectTiles = 1 << 8;

    Grid3D grid;

    void OnEnable()
    {
        Instance = this;
    }

    void Awake()
    {
        grid = Grid3D.Instance;
    }

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray.origin, ray.direction, out raycastHit, Mathf.Infinity, layerForDetectTiles))
            {
                start = raycastHit.transform.gameObject;
            }
            else
            {
                // touchedTile = null;
            }
        }
    }

    void LateUpdate()
    {
        if (Input.GetMouseButtonUp(0) && start != null)
        {
            Tile3D tile = start.transform.GetComponent<Tile3D>();
            if (tile == null)
            {
                // touchedTile = null;
            }
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray.origin, ray.direction, out raycastHit, Mathf.Infinity, layerForDetectTiles))
                {
                    end = raycastHit.transform.gameObject;

                    if (start.transform.position == end.transform.position)
                    {
                        touchedTile = tile;
                        grid.SelectTile(touchedTile.node);
                    }
                }
            }
        }
    }
}
