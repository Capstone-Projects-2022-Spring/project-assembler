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
        if (NetworkManager.singleton.numPlayers == 0)
            return;
        float angle = Random.Range(-4f, 4f);
        GameObject result = Instantiate(enemyObject, this.transform.position - new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0), Quaternion.identity);
        result.GetComponent<EnemyAI>().currentHealth = GameObject.Find("UIscripts").GetComponent<UIManager>().enemyHealthvalue;
        result.GetComponent<EnemyAI>().speed = GameObject.Find("UIscripts").GetComponent<UIManager>().enemySpeedvalue;
        NetworkServer.Spawn(result);
        if(spawnIntervel != GameObject.Find("UIscripts").GetComponent<UIManager>().enemySpawnFrequencyvalue)
        {
            spawnIntervel = GameObject.Find("UIscripts").GetComponent<UIManager>().enemySpawnFrequencyvalue;
            CancelInvoke(nameof(SpawnEnemy));
            InvokeRepeating(nameof(SpawnEnemy), spawnIntervel, spawnIntervel);
        }
    }
}
