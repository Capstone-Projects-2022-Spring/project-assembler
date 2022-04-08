using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EnemtSpawnerScript : NetworkBehaviour
{
    public float spawnIntervel = 10f;
    public GameObject enemyObject;

    // Start is called before the first frame update
    public override void OnStartServer()
    {
        InvokeRepeating(nameof(SpawnEnemy), 0f, spawnIntervel);
    }
   

    [ServerCallback]
    void SpawnEnemy()
    {
        float angle = Random.Range(-4f, 4f);
        GameObject result = Instantiate(enemyObject,  this.transform.position - new Vector3(Mathf.Sin(angle),Mathf.Cos(angle),0),Quaternion.identity);
        NetworkServer.Spawn(result);
    }
}
