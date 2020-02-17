using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour {

    public int health;
	public bool isMy;
    public void OnCollisionEnter2D(Collision2D col)
    {

        if (col.gameObject.tag == "Bullet")
        {
            health--;
			if (isMy == true) {
				Debug.Log("Uszkodzono naszą bazę");
			}
            else Debug.Log("Uszkodzono bazę przeciwnika");
            if (health <= 0)
            {
				if (isMy == true) {
					GameManager.instance.GameOver(); ;
					Debug.Log ("Zniszczono naszą bazę");


				} else {
					GameManager.instance.GameWon();
                    Debug.Log ("Zniszczono bazę przeciwnika");
				}
            }
        }
    }
    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
