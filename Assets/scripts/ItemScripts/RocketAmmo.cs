using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RocketAmmo : NetworkBehaviour
{

    public int damage = 100;

    public override void OnStartServer()
    {
        base.OnStartServer();
        this.GetComponent<Rigidbody2D>().velocity = this.GetComponent<Rigidbody2D>().velocity.normalized * 30f;
        Invoke(nameof(destory), 3f);
    }


    [ServerCallback]
    void OnCollisionEnter2D(Collision2D col)
    {
        destory();
    }


    [ServerCallback]
    void destory()
    {
        Collider2D[] collided;
        collided = Physics2D.OverlapCircleAll(this.transform.position, 20f);
        foreach (Collider2D obj in collided)
        {
            var selectedObj = obj.gameObject;
            if (selectedObj.GetComponent<EnemyAI>() != null)
            {
                EnemyAI collidedEnemy = selectedObj.GetComponent<EnemyAI>();
                collidedEnemy.currentHealth -= damage;
            }

        }
        NetworkServer.Destroy(this.gameObject);
    }
}
