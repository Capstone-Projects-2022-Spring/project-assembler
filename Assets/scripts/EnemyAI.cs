using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Pathfinding;

public class EnemyAI : NetworkBehaviour
{
    public Rigidbody2D rigidbody2d;
    public float speed = 800;
    int maxHealth = 100;
    public float nextWaypointDistance = 3f;
    [SyncVar(hook = nameof(onChangeHealth))]
    public int currentHealth = 100;
    public Animator animator;

    public Transform target;
    Path path;
    Seeker seeker;
    int currentWayPoint = 0;
    bool reachedEndOfPath = false;
    double lastRandMovePath;

    public override void OnStartServer()
    {
        seeker = GetComponent<Seeker>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        InvokeRepeating(nameof(detectPlayers), 0f, 0.4f);
        lastRandMovePath = Time.timeAsDouble;
    }

    public void onChangeHealth(int oldvalue, int newvalue)
    {
        if(newvalue <= 0)
        {
            destory();
        }
    }

    private void FixedUpdate()
    {
        if(rigidbody2d.velocity.magnitude > 5f)
        {
            animator.SetFloat("Speed", 0f);
        } else
        {
            animator.SetFloat("Speed", 10f);
        }

        if (path == null || !isServer)
            return;

        if(currentWayPoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        } else
        {
            reachedEndOfPath = false;
        }

        Vector2 directionToMove = ((Vector2)path.vectorPath[currentWayPoint] - rigidbody2d.position).normalized;
        Vector2 force = directionToMove * speed * Time.deltaTime;

        float distance = Vector2.Distance(rigidbody2d.position, path.vectorPath[currentWayPoint]);
        //rigidbody2d.AddForce(force);
        rigidbody2d.velocity = directionToMove * speed * Time.deltaTime;

        if (distance < nextWaypointDistance)
        {
            currentWayPoint++;
        }

    }
    
    [ServerCallback]
    void detectPlayers()
    {
        bool pass = false;
        Collider2D[] collided;
        collided = Physics2D.OverlapCircleAll(this.transform.position, 40f);
        foreach (Collider2D obj in collided)
        {
            var selectedObj = obj.gameObject;
            if (selectedObj.GetComponent<PlayerControl>() != null)
            {
                PlayerControl player = selectedObj.GetComponent<PlayerControl>();
                Vector3 velcoity3 = player.transform.position - this.transform.position;
                seeker.StartPath(this.transform.position, selectedObj.transform.position, OnPathFound);
                pass = true;
                if((this.transform.position - selectedObj.transform.position).magnitude < 3f)
                {
                    selectedObj.GetComponent<PlayerControl>().currentHealth -= 5;
                    if(selectedObj.GetComponent<PlayerControl>().currentHealth < 0)
                    {
                        respawn(selectedObj.GetComponent<NetworkIdentity>().netId);
                        selectedObj.GetComponent<PlayerControl>().currentHealth = selectedObj.GetComponent<PlayerControl>().MaxHealth;
                    }
                }
                break;
            }

        }
        if(Time.timeAsDouble - lastRandMovePath > 7 && !pass) 
        {
            seeker.StartPath(this.transform.position, new Vector3(this.transform.position.x + Random.Range(-100f, 100f), this.transform.position.y + Random.Range(-100f, 100f), 0), OnPathFound);
            lastRandMovePath = Time.timeAsDouble;
        }
    }

    void OnPathFound(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWayPoint = 0;
        }
    }


    [ClientRpc]
    public void respawn(uint conn)
    {
        if (NetworkClient.localPlayer.netId == conn)
        {
            NetworkClient.localPlayer.gameObject.transform.position = new Vector3(0, 0, 0);
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