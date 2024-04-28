using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using Unity.VisualScripting;
using UnityEngine;

public class CowSpawner : MonoBehaviour
{
    public FindSpawnPositions spawnPos;
    public int maxCowsAllowed;
    public int currentCowCount;
    public float numberofCowsSpawned;
    private void Start()
    {
        for (int i = 0; i < maxCowsAllowed; i++)
        {
            SpawnCow();
            currentCowCount++;
        }
    }

    public void SpawnCow()
    {
        spawnPos.StartSpawn();
    }

    private void Update()
    {
        if (currentCowCount < maxCowsAllowed)
        {
            SpawnCow();
        }
    }
}
