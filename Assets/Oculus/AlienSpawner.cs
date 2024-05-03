
using UnityEngine;
using NaughtyAttributes;

public class AlienSpawner : MonoBehaviour
{
    public FindSpawnPositions spawnPos;
    public int maxAliensAllowed;
    public int currentAlienCount;
    public float numberoAliensSpawned;
    private void Start()
    {
        for (int i = 0; i < maxAliensAllowed; i++)
        {
            SpawnAlien();
            currentAlienCount++;
        }
    }
    [Button("ADD ALIEN")]
    public void SpawnAlien()
    {
        spawnPos.StartSpawn();
    }

    private void Update()
    {
        if (currentAlienCount < maxAliensAllowed)
        {
            SpawnAlien();
        }
    }
}
