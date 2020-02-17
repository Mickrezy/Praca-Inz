using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyController : MonoBehaviour {

    public GameObject tank;

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

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
