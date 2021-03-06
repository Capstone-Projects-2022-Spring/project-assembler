using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GunTurret : GameItem
{
    public GameObject bulletPrefab;
    double lastShot;


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

    private void FixedUpdate()
    {
        if (isServer && gameObject.activeSelf && Time.timeAsDouble - lastShot > 0.3)
        {
            Collider2D[] collided;
            collided = Physics2D.OverlapCircleAll(this.transform.position, 40f);
            foreach (Collider2D obj in collided)
            {
                var selectedObj = obj.gameObject;
                if (selectedObj.GetComponent<EnemyAI>() != null)
                {
                    EnemyAI enemy = selectedObj.GetComponent<EnemyAI>();
                    Shoot(this.transform, enemy.transform.position);
                    lastShot = Time.timeAsDouble;
                    break;
                }

            }
        }
    }

    void Shoot(Transform shootPosition, Vector2 mousepos)
    {
        Quaternion angle = new Quaternion();
        Vector2 directionVector = new Vector2(mousepos.x - shootPosition.position.x, mousepos.y - shootPosition.position.y);
        angle.eulerAngles = new Vector3(0, 0, Vector2.Angle(new Vector3(1, 0, 0), directionVector));

        float offset = 3;
        Vector3 postionToSpawnOn = new Vector3(shootPosition.position.x + offset * directionVector.normalized.x, shootPosition.position.y + offset * directionVector.normalized.y, 0);
        GameObject generatedBullet = Instantiate(bulletPrefab, postionToSpawnOn, angle);
        generatedBullet.GetComponent<Rigidbody2D>().velocity = 200f * directionVector.normalized;
        NetworkServer.Spawn(generatedBullet);
    }

}
