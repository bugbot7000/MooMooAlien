using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public GameObject controllerCanvas, scoreCanvas;

    public void ToggleControllerCanvas()
    {
        controllerCanvas.gameObject.SetActive(!controllerCanvas.activeSelf);
    }
    public void ToggleScoreCanvas()
    {
        scoreCanvas.gameObject.SetActive(!scoreCanvas.activeSelf);
    }
}