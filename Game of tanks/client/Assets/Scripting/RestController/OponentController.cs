using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OponentController : MonoBehaviour {


    public RestController restData;
    public TankController[] tanks;
    public BulletController[] bullets;
    public TankController tankPrefab;
    public BulletController bulletPrefab;
    public List<GameObject> obstacles;

    public Color tankColor;

    private void Awake()
    {

    }

    public void ShowOponents()
    {
        for(int i=0; i < restData.oponentData.objects.Count; i++)
        {

            var tank = restData.oponentData.objects[i];
            if (tank.isAlive && tanks[i] == null)
            {
                tanks[i] = GameObject.Instantiate(tankPrefab, transform);
                var rigidbodies = tanks[i].GetComponentsInChildren<Rigidbody2D>();
                foreach(var rig in rigidbodies)
                {
                    rig.interpolation = RigidbodyInterpolation2D.Extrapolate;
                }
                var sprites = tanks[i].GetComponentsInChildren<SpriteRenderer>();
                foreach (var sprite in sprites)
                {
                    sprite.color = tankColor;
                }
            }
            else if (!tank.isAlive)
            {
                if(tanks[i])
                    GameObject.Destroy(tanks[i].gameObject);
                continue;
            }

            //Debug.Log("current = " + tanks[i].body.transform.localPosition);
            //Debug.Log("wanted = " + tank.coX + " " + tank.coY);
            // if (Mathf.Sqrt(Mathf.Pow(tanks[i].body.transform.localPosition.x - tank.coX, 2) + Mathf.Pow(tanks[i].body.transform.localPosition.y - tank.coY,2)) > 0.16f)
            // {
            if (tanks[i].gameObject.activeSelf == true)
            {
                tanks[i].body.transform.localPosition = new Vector3(tank.coX, tank.coY, 0);
                tanks[i].body.GetComponent<Rigidbody2D>().MovePosition(tanks[i].body.transform.position);
                //   }
                //tanks[i].deltaPos = new Vector3(tank.coX - tanks[i].body.transform.localPosition.x, tank.coY - tanks[i].body.transform.localPosition.y);
                //tanks[i].opponent = true;
                tanks[i].body.transform.localRotation = new Quaternion(tank.quatI, tank.quatJ, tank.quatK, tank.quat1);
                tanks[i].turret.transform.localRotation = new Quaternion(tank.turret.turretQuatI, tank.turret.turretQuatJ, tank.turret.turretQuatK, tank.turret.turretQuat1);
                tanks[i].turret.rotation = tank.turret.turretAngle;
                tanks[i].turret.rotationTime = tank.turret.turretRotationTime;

                tanks[i].leftTrack.power = tank.forceLeft * 0.9f;
				tanks[i].rightTrack.power = tank.forceRight * 0.9f;
            }
            
        }
        for (int i = 0; i < restData.oponentData.bullets.Count; i++)
        {
         
            var spawnedBullet = restData.oponentData.bullets[i];
            if (spawnedBullet.isAlive && bullets[i] == null)
            {
                bullets[i] = GameObject.Instantiate(bulletPrefab, transform);
            }
            else if (!spawnedBullet.isAlive && !bullets[i].gameObject.activeSelf)
            {
                Destroy(bullets[i].gameObject);
                continue;
            }
            else if (!spawnedBullet.isAlive || spawnedBullet.id == bullets[i].id || !bullets[i].gameObject.activeSelf)
            {
                continue;
            }
            bullets[i].transform.localPosition = new Vector3(spawnedBullet.coX, spawnedBullet.coY);
            bullets[i].transform.localRotation = new Quaternion(spawnedBullet.quatI, spawnedBullet.quatJ, spawnedBullet.quatK, spawnedBullet.quat1);
            bullets[i].speed = spawnedBullet.speed;
            bullets[i].sent = true;
            bullets[i].id = spawnedBullet.id;
        }
    }

}
