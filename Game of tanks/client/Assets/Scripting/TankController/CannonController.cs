using UnityEngine;
using System.Collections;

public class CannonController : MonoBehaviour
{



    public bool isFiring;
    public BulletController bullet;
    public float bulletSpeed;
    public Transform firePoint;
    public GameObject battleGround;
    // Update is called once per frame
    void Update()
    {

        if (isFiring == true)
        {
            BulletController newBullet = Instantiate(bullet, firePoint.position, firePoint.rotation,battleGround.transform) as BulletController;

            if (checkIfPosEmpty(newBullet))
            {
                newBullet.gameObject.SetActive(true);
                newBullet.speed = bulletSpeed;
            }
            else
            {
                newBullet.gameObject.SetActive(false);
            }
            //newBullet.tank = this.GetComponentInParent<TankController>();
            isFiring = false;
        }
    }
    public bool checkIfPosEmpty(BulletController bullet)
    {
        //TankController tank = this.GetComponentInParent<TankController>();
        Collider2D bulletCol = bullet.GetComponent<Collider2D>();
        TankController[] tanks = FindObjectsOfType<TankController>();
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        Base[] bases = FindObjectsOfType<Base>();
        /*foreach (TankController otherTanks in tanks)
        {
            Collider2D tankCol = otherTanks.GetComponent<Collider2D>();
            if (otherTanks != tank && tankCol.bounds.Intersects(bulletCol.bounds)){
                Destroy(bullet);
                return false;
            }
                
        }*/
        foreach (GameObject wall in walls)
        {
            Collider2D wallCol = wall.gameObject.GetComponent<Collider2D>();
            if (wallCol.bounds.Intersects(bulletCol.bounds)){
                Destroy(bullet);
                return false;
            }

        }
        foreach (Base basee in bases)
        {
            Collider2D baseCol = basee.GetComponent<Collider2D>();
            if (baseCol.bounds.Intersects(bulletCol.bounds)){
                Destroy(bullet);
                return false;
            }

        }
        return true;
    }
}