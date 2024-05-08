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
        }
    }

    private IEnumerator SpawnCowsCoroutine()
    {
        while (spawnedViaCoroutine < spawnLimit && currentCowCount < maxCowsAllowed)
        {
            float spawnInterval = Random.Range(4f, 6f); // Random interval between 4 and 6 seconds
            yield return new WaitForSeconds(spawnInterval);

            SpawnCow(); // Increment `currentCowCount` within this method
            spawnedViaCoroutine++;
        }
    }
}