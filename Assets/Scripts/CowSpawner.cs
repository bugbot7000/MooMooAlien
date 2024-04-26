using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using Unity.VisualScripting;
using UnityEngine;

public class CowSpawner : MonoBehaviour
{
    public FindSpawnPositions spawnPos;
    public void SpawnCow()
    {
    spawnPos.StartSpawn();
    }
}
