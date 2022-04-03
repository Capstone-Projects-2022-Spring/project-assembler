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

    [ServerCallback]
    private void FixedUpdate()
    {
        Collider2D[] collided;
        collided = Physics2D.OverlapBoxAll(this.transform.position, new Vector2(20f, 20f), 0);
        foreach (Collider2D obj in collided)
        {
            var selectedObj = obj.gameObject;
            if (selectedObj.GetComponent<PlayerControl>() != null)
            {
                PlayerControl player = selectedObj.GetComponent<PlayerControl>();
                Vector3 velcoity3 = player.transform.position - this.transform.position;
                if(velcoity3.magnitude > 5)
                {
                    rigidbody2d.velocity = new Vector3(velcoity3.x, velcoity3.y).normalized * 20f;
                }
                else
                {
                    rigidbody2d.velocity = new Vector3(0, 0);
                }
            }

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