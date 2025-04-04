using UnityEngine;
using Mirror;
using System.Collections;
public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] Transform[] spawnPoint;
    [SerializeField] GameObject enemyPrefab;

    float spawnTime = 1f;
    const float SPAWN_TIME_DELTA = 3f;

    void Awake()
    {
        foreach (var sp in spawnPoint)
        {
            var render = sp.GetComponent<Renderer>();
            render.enabled = false;
        }
    }

    public override void OnStartServer()
    {
        InvokeRepeating(nameof(SpawnEnemy), spawnTime, SPAWN_TIME_DELTA);
    }

    [Server]
    void SpawnEnemy()
    {
        var spawnID = Random.Range(0, spawnPoint.Length);
        var sp = spawnPoint[spawnID];
        GameObject enemy = Instantiate(
            enemyPrefab,
            sp.position,
            sp.rotation);
        NetworkServer.Spawn(enemy);
        StartCoroutine(DestoryEnemyAfeterDelay(enemy, 10f));
    }

    IEnumerator DestoryEnemyAfeterDelay(GameObject enemy, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (enemy != null)
        {
            NetworkServer.Destroy(enemy);
        }
    }
}