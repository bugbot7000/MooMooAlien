using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public GameObject controllerCanvas, scoreCanvas;
    public bool GameOver;

    private void Start()
    {
        GameOver = false;
    }

    public void ToggleControllerCanvas()
    {
        controllerCanvas.gameObject.SetActive(!controllerCanvas.activeSelf);
    }
    public void ToggleScoreCanvas()
    {
        if (GameOver == true)
        {
            scoreCanvas.gameObject.SetActive(!scoreCanvas.activeSelf);
        }
    }
}