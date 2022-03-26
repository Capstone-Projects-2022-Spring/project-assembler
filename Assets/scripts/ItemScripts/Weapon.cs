using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : GameItem
{
    public Transform firePoint;
    public GameObject bulletPrefab;

    public override void interact(PlayerControl player)
    {
        if (isOnGround == true)
        {
            if (player.addToInvenotry(this.gameObject, true))
            {


                //Add to the player inventory
                if (player.inventory.ContainsKey(this))
                {
                    player.inventory[this] += 1;
                }
                else
                {
                    player.inventory.Add(this, 1);
                }
                transform.position = new Vector3(0, 0, 1);
                gameObject.GetComponent<SpriteRenderer>().enabled = !gameObject.GetComponent<SpriteRenderer>().enabled;
                gameObject.GetComponent<Collider2D>().enabled = !gameObject.GetComponent<Collider2D>().enabled;
                isOnGround = false;

            }
        }


    }

    // Update is called once per frame
    //void Update()
    //{
    //    if(Input.GetButtonDown("Fire1")){
    //        Shoot();
    //    }
    //}

    //shooting class
    void Shoot(){
        GameObject generatedBullet = Instantiate(bulletPrefab, firePoint.position, this.transform.rotation);
        Mirror.NetworkServer.Spawn(generatedBullet);
        generatedBullet.GetComponent<Rigidbody2D>().velocity = 5 * new Vector2(Mathf.Cos(transform.rotation.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(transform.rotation.eulerAngles.z * Mathf.Deg2Rad));
    }


    
}
