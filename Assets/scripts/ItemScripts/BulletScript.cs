using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BulletScript : NetworkBehaviour
{

    public int damage = 10;

    public override void OnStartServer()
    {
        base.OnStartServer();
        Invoke(nameof(destory), 0.6f);
    }


    [ServerCallback]
    void OnCollisionEnter2D(Collision2D col)
    {
        EnemyAI collidedEnemy = col.gameObject.GetComponent<EnemyAI>();
        if (collidedEnemy != null)
        {
            collidedEnemy.currentHealth -= damage;
        }
        destory();
    }

    [ServerCallback]
    void destory()
    {
        NetworkServer.Destroy(this.gameObject);
    }
}
