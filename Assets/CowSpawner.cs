using System;
using System.Collections;
using System.Collections.Generic;
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
        OVRInput.Update();
    }
}
