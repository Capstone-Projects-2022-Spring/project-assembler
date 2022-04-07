using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GunTurret : GameItem
{
    public GameObject bulletPrefab;
    double lastShot;

    private void FixedUpdate()
    {
        if (isServer && gameObject.activeSelf && Time.timeAsDouble - lastShot > 0.3)
        {
            Collider2D[] collided;
            collided = Physics2D.OverlapBoxAll(this.transform.position, new Vector2(30f, 30f), 0);
            foreach (Collider2D obj in collided)
            {
                var selectedObj = obj.gameObject;
                if (selectedObj.GetComponent<EnemyAI>() != null)
                {
                    EnemyAI enemy = selectedObj.GetComponent<EnemyAI>();
                    Shoot(this.transform, enemy.transform.position);
                    lastShot = Time.timeAsDouble;
                }

            }
        }
    }

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
