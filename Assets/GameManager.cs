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

    public float  tc, t,add;
    // Start is called before the first frame update
    void Start()
    {
        //Invoke("StartGame",20);
    }
    [Button("Start Game")]
    public void StartGame()
    {
        onGameStart.Invoke();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (tc < t)
        {
            tc += Time.deltaTime;
        }
        else
        {
            add += tc;
            Debug.Log("Fired at  " + (add));
            tc = 0;
        }
    }
}
