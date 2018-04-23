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

    [SerializeField]
    Button rotateLeftButton;

    [SerializeField]
    Button rotateRightButton;

    [SerializeField]
    Button cancelButton;

    [SerializeField]
    Button resetButton;

    public void ShowHideRotateButtons(bool show)
    {
        rotateLeftButton.gameObject.SetActive(show);
        rotateRightButton.gameObject.SetActive(show);
    }

    void OnEnable()
    {
        brownBrickButton.onClick.AddListener(brownBrickButtonClicked);
        grayBrickButton.onClick.AddListener(grayBrickButtonClicked);
        resetButton.onClick.AddListener(resetButtonClicked);
        cancelButton.onClick.AddListener(cancelButtonClicked);
        rotateLeftButton.onClick.AddListener(rotateLeftButtonClicked);
        rotateRightButton.onClick.AddListener(rotateRightButtonClicked);

        gridSizeXSlider.onValueChanged.AddListener(onXValueChanged);
        gridSizeZSlider.onValueChanged.AddListener(onZValueChanged);
    }

    private void rotateRightButtonClicked()
    {
        grid.RotateTile(InputController.Instance.touchedTile.node, false);
    }

    private void rotateLeftButtonClicked()
    {
        grid.RotateTile(InputController.Instance.touchedTile.node, true);
    }

    private void cancelButtonClicked()
    {
        grid.materialIndex = -1;
        brownBrickButton.transform.GetChild(0).gameObject.SetActive(false);
        grayBrickButton.transform.GetChild(0).gameObject.SetActive(false);
        ShowHideRotateButtons(true);
    }

    private void resetButtonClicked()
    {
        grid.ResetTiles();
        brownBrickButton.transform.GetChild(0).gameObject.SetActive(false);
        grayBrickButton.transform.GetChild(0).gameObject.SetActive(false);
        ShowHideRotateButtons(true);
    }

    private void grayBrickButtonClicked()
    {
        grid.materialIndex = 1;
        brownBrickButton.transform.GetChild(0).gameObject.SetActive(false);
        grayBrickButton.transform.GetChild(0).gameObject.SetActive(true);
    }

    private void brownBrickButtonClicked()
    {
        grid.materialIndex = 0;
        brownBrickButton.transform.GetChild(0).gameObject.SetActive(true);
        grayBrickButton.transform.GetChild(0).gameObject.SetActive(false);
    }

    private void onZValueChanged(float value)
    {
        grid.gridZ = (int)value;
        sizeZText.text = ((int)value).ToString();

        StartCoroutine(grid.DoPostUpdate());

        brownBrickButton.transform.GetChild(0).gameObject.SetActive(false);
        grayBrickButton.transform.GetChild(0).gameObject.SetActive(false);
    }

    private void onXValueChanged(float value)
    {
        grid.gridX = (int)value;
        sizeXText.text = ((int)value).ToString();

        StartCoroutine(grid.DoPostUpdate());

        brownBrickButton.transform.GetChild(0).gameObject.SetActive(false);
        grayBrickButton.transform.GetChild(0).gameObject.SetActive(false);
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
