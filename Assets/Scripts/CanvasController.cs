using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    Grid3D grid;

    [SerializeField]
    Slider gridSizeXSlider;

    [SerializeField]
    Slider gridSizeZSlider;

    [SerializeField]
    Text sizeXText;

    [SerializeField]
    Text sizeZText;

    [SerializeField]
    Button brownBrickButton;

    [SerializeField]
    Button grayBrickButton;

    void OnEnable()
    {
        brownBrickButton.onClick.AddListener(brownBrickButtonClicked);
        grayBrickButton.onClick.AddListener(grayBrickButtonClicked);

        gridSizeXSlider.onValueChanged.AddListener(onXValueChanged);
        gridSizeZSlider.onValueChanged.AddListener(onZValueChanged);
    }

    private void grayBrickButtonClicked()
    {
        grid.materialIndex = 1;
    }

    private void brownBrickButtonClicked()
    {
        grid.materialIndex = 0;
    }

    private void onZValueChanged(float value)
    {
        grid.gridZ = (int)value;
        sizeZText.text = ((int)value).ToString();

        StartCoroutine(grid.DoPostUpdate());
    }

    private void onXValueChanged(float value)
    {
        grid.gridX = (int)value;
        sizeXText.text = ((int)value).ToString();

		StartCoroutine(grid.DoPostUpdate());
    }

    void Start()
    {
        grid = Grid3D.Instance;

		gridSizeXSlider.value = grid.gridX;
		gridSizeZSlider.value = grid.gridZ;
		
		gridSizeXSlider.maxValue = grid.max.x;
		gridSizeZSlider.maxValue = grid.max.y;
		
		sizeXText.text = ((int)grid.gridX).ToString();
		sizeZText.text = ((int)grid.gridZ).ToString();

        StartCoroutine(grid.DoPostUpdate());
    }
}
