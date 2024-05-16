using NaughtyAttributes;
using UnityEngine;
using System.Collections;

public class CowSpawner : MonoBehaviour
{
    public FindSpawnPositions spawnPos;
    public int maxCowsAllowed;
    public int spawnLimit = 5; // Maximum number of cows to spawn via the coroutine
    private int currentCowCount;
    private int spawnedViaCoroutine;
    public float SpawnRand1, SpawnRand2;
    public delegate void CowSpawnerDelegate();

    public CowSpawnerDelegate _cowSpawned;
    private void Start()
    {
        
        currentCowCount = 0;
        spawnedViaCoroutine = 0;
        StartCoroutine(SpawnCowsCoroutine());
    }

    [Button("ADD COW")]
    public void SpawnCow()
    {
        if (currentCowCount < maxCowsAllowed)
        {
            spawnPos.StartSpawn();
            currentCowCount++;
            _cowSpawned?.Invoke();
        }
    }
    
    private IEnumerator SpawnCowsCoroutine()
    {
        while (spawnedViaCoroutine < spawnLimit && currentCowCount < maxCowsAllowed)
        {
            float spawnInterval = Random.Range(SpawnRand1, SpawnRand2); // Random interval between 4 and 6 seconds
            yield return new WaitForSeconds(spawnInterval);

            SpawnCow(); // Increment `currentCowCount` within this method
            spawnedViaCoroutine++;
        }
    }
}