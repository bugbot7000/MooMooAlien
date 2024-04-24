using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using Unity.VisualScripting;
using UnityEngine;

public class CowSpawner : MonoBehaviour
{
    public FindSpawnPositions spawnPos;
    void SpawnCow()
    {
        if (OVRInput.Get(OVRInput.Button.One))
        {
            spawnPos.StartSpawn();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            spawnPos.StartSpawn();
        }
    }
}
