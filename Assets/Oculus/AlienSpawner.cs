using UnityEngine;
using NaughtyAttributes;
using System.Collections;

public class AlienSpawner : MonoBehaviour
{
    public FindSpawnPositions spawnPos;
    public int maxAliensAllowed;
    public int spawnLimit = 5; // Maximum number of aliens to spawn via the timer
    public int currentAlienCount;
    private int spawnedViaTimer;

    private void Start()
    {
        currentAlienCount = 0;
        spawnedViaTimer = 0;
        StartCoroutine(SpawnAliensCoroutine());
    }

    [Button("ADD ALIEN")]
    public void SpawnAlien()
    {
        if (currentAlienCount < maxAliensAllowed)
        {
            spawnPos.StartSpawn();
            currentAlienCount++;
        }
    }

    private IEnumerator SpawnAliensCoroutine()
    {
        while (spawnedViaTimer < spawnLimit && currentAlienCount < maxAliensAllowed)
        {
            float spawnInterval = Random.Range(4f, 6f); // Random interval between 4 and 6 seconds
            yield return new WaitForSeconds(spawnInterval);

            SpawnAlien();
            spawnedViaTimer++;
        }
    }
}