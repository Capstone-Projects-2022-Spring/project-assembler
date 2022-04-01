using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Weapon : GameItem
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    double lastShot;

    private void Start()
    {
        lastShot = Time.timeAsDouble;
    }

    public override void interact(PlayerControl player)
    {
        if (isOnGround == true && Input.GetMouseButtonDown(0))
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
            }
        }


    }

    public override void actionFromInventroy(PlayerControl player)
    {
        if (Time.timeAsDouble - lastShot > 0.1)
        {
            Shoot(player.transform, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            lastShot = Time.timeAsDouble;
        }
    }


    [Command(requiresAuthority = false)]
    void Shoot(Transform shootPosition, Vector2 mousepos)
    {
        Quaternion angle = new Quaternion();
        Vector2 directionVector = new Vector2(mousepos.x - shootPosition.position.x, mousepos.y - shootPosition.position.y);
        angle.eulerAngles = new Vector3(0, 0, Vector2.Angle(new Vector3(1, 0, 0), directionVector));

        float offset = 4;
        Vector3 postionToSpawnOn = new Vector3(shootPosition.position.x + offset * directionVector.normalized.x, shootPosition.position.y + offset * directionVector.normalized.y, 0);
        GameObject generatedBullet = Instantiate(bulletPrefab, postionToSpawnOn, angle);
        generatedBullet.GetComponent<Rigidbody2D>().velocity = 100f * directionVector.normalized;
        NetworkServer.Spawn(generatedBullet);
    }
    
}
