using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public CowSpawner cowMaker;

    public AlienSpawner alienMaker;

    public UnityEvent onGameStart;

    public GameObject controllerCanvas, scoreKeeperCanvas;

    public int shotsFired, shotsHit, shotsMissed;
    
// Start is called before the first frame update
    
    [Button("Start Game")]
    public void StartGame()
    {
        controllerCanvas.gameObject.SetActive(false);
        onGameStart.Invoke();
    }
    
    
}
