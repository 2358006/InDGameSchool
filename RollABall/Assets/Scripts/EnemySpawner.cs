using UnityEngine;
using Mirror;
using Mirror.BouncyCastle.Cms;
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
        if (!isServer) return;

        playTime += Time.deltaTime;

        if (playTime > spawnTime)
        {
            spawnTime += SPAWN_TIME_DELTA;
            CmdSpawnEnemy(Random.Range(0, spawnPoint.Length));
            Debug.Log("Spawn Enemy");
        }
    }


    [ClientRpc]

    void RpcSpawnEnemy(int spawnID)
    {
        if (isServer)
        {
            CmdSpawnEnemy(spawnID);
        }
    }

    [Command]
    void CmdSpawnEnemy(int spawnID)
    {
        var sp = spawnPoint[spawnID];
        GameObject enemy = Instantiate(enemyPrefab, sp.position, sp.rotation);
        Destroy(enemy, 10f);
        NetworkServer.Spawn(enemy);
    }
}
