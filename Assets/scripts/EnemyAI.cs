using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EnemyAI : NetworkBehaviour
{
    public Rigidbody2D rigidbody2d;
    float speed = 200;
    int maxHealth = 100;
    [SyncVar(hook = nameof(onChangeHealth))]
    public int currentHealth = 100;

    public override void OnStartServer()
    {
        InvokeRepeating(nameof(randomMovement), 0, 4f);
    }

    public void onChangeHealth(int oldvalue, int newvalue)
    {
        if(newvalue <= 0)
        {
            destory();
        }
    }

    [Command(requiresAuthority = false)]
    public void destory()
    {
        NetworkServer.Destroy(this.gameObject);
    }

    void randomMovement()
    {
        rigidbody2d.velocity = new Vector2(Random.Range(-5,5), Random.Range(-5, 5)) * speed * Time.fixedDeltaTime;
    }

}