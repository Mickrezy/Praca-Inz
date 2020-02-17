using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovement : MonoBehaviour
{

    // Use this for initialization
    public float maxSpeed;
    public float power;
    Rigidbody2D rb;
    public GameObject tank;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Bullet")
		{
			StartCoroutine(tank.GetComponent<TankController>().Kill());
            /*bool myBullet = false;
            BulletController bullet = col.gameObject.GetComponent<BulletController>();
            TankController tankC = tank.gameObject.GetComponent<TankController>();
            float bulletID = bullet.id;
            foreach (float id in tankC.bulletIDs)
            {
                if (id == bulletID) myBullet = true;
            }
            if (!myBullet)
            {
                Destroy(tank);
                Destroy(col.gameObject);
                bullet.tank.bulletIDs.Remove(bulletID);
            }*/
        }
    }
		

    void FixedUpdate()
    {

        rb.AddForce(gameObject.transform.right * maxSpeed * power / 100);
    }
}
