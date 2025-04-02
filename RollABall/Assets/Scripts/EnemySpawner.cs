using UnityEngine;
using Mirror;
public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] Transform[] spawnPoint;
    [SerializeField] GameObject enemyPrefab;

    float spawnTime = 1f;
    const float SPAWN_TIME_DELTA = 3f;
    [SyncVar] float playTime = 0f;

    void Awake()
    {
        foreach (var sp in spawnPoint)
        {
            var render = sp.GetComponent<Renderer>();
            render.enabled = false;
        }
    }

    void Update()
    {
        if (isServer)
        {
            playTime += Time.deltaTime;
            if (playTime > spawnTime)
            {
                spawnTime += SPAWN_TIME_DELTA;
                SpawnEnemy(Random.Range(0, spawnPoint.Length));
                Debug.Log("Spawn Enemy");
            }
        }
    }

    void SpawnEnemy(int spawnID)
    {
        var sp = spawnPoint[spawnID];
        GameObject enemy = Instantiate(enemyPrefab, sp.position, sp.rotation);
        Destroy(enemy, 5f);
        NetworkServer.Spawn(enemy);
    }
}
