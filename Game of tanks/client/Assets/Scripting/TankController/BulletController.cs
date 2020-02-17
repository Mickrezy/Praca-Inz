using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

    public float speed;
    public bool sent = false;
    public float id;
    Rigidbody2D rb;
   // public TankController tank;
    // Use this for initialization
    void Start () {
        //newBullet.id = Random.Range(0.0f, float.MaxValue);
        //newBullet.tank.bulletIDs.Add(newBullet.id);
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(gameObject.transform.right * speed);
        if(RestController.instance)
        {
            RestController.instance.SendMyObjects();
        }


    }

    public IEnumerator Kill()
    {
        gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);
    }

    // Update is called once per frame
    void Update () {
        //rb.AddForce(gameObject.transform.right * speed);
    }
    void OnCollisionEnter2D(Collision2D col)
    {

        StartCoroutine(Kill());
       // gameObject.SetActive(false);
       // Destroy(gameObject);

        /*if (col.gameObject.tag != "Tank")
        {
            Destroy(gameObject);
            tank.bulletIDs.Remove(id);
        }*/
    }
}
