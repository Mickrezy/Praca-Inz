using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour {
    public TankMovement leftTrack;
    public TankMovement rightTrack;
    public TurretController turret;
    public CannonController cannon;
    public GameObject body;
    //public List<float> bulletIDs;
    //public string tankID;




    public float shootCooldown = 3f;

    private float shootTimer = 3.1f;

    private void Update()
    {
        shootTimer += Time.deltaTime;
    }

    public void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Bullet")
        {
            /*bool myBullet = false;
            BulletController bullet = col.gameObject.GetComponent<BulletController>();
            float bulletID = bullet.id;
            foreach (float id in bulletIDs)
            {
                if (id == bulletID) myBullet = true;
            }
            if (!myBullet)
            {
                Destroy(gameObject);
                Destroy(col.gameObject);
                bullet.tank.bulletIDs.Remove(bulletID);
                
            }*/
			StartCoroutine(Kill());

        }
    }

	public IEnumerator Kill(){
		gameObject.SetActive(false);
		yield return new WaitForSeconds (0.1f);
		gameObject.SetActive(true);
		Destroy(gameObject);
	}

    //ustawia moc prawej gąsienicy w zakresie <-100,100>
	public void MocPrawegoSilnika(int moc)
    {
        moc = Mathf.Max(Mathf.Min(100, moc), -100);
        rightTrack.power = moc;
        Raportuj(string.Format("Ustawiłem moc prawego silnika na {0}%", moc));
    }

    //ustawia moc lewej gąsienicy w zakresie <-100,100>
    public void MocLewegoSilnika(int moc)
    {
        moc = Mathf.Max(Mathf.Min(100, moc),-100);
        leftTrack.power = moc;
        Raportuj(string.Format("Ustawiłem moc lewego silnika na {0}%", moc));
    }

    //powoduje czekanie z wykonaniem kolejnej komendy o zadaną liczbę milisekund
    public void CzekajNaKomende(float ms)
    {

        Raportuj(string.Format("Czekam na rozkazy przez {0} milisekund.", ms));

    }


    //powoduje obrót wieży w prawo o zadany kąt
    public void ObrocWiezeWPrawo(int stopnie)
    {
        turret.rotation = stopnie * -1;
        turret.SetRotationTime();
        Raportuj(string.Format("Obracam wieżę o {0} stopni w prawo", stopnie));
    }

    //powoduje obrót wieży w lewo o zadany kąt
    public void ObrocWiezeWLewo(int stopnie)
    {
        turret.rotation = stopnie;
        turret.SetRotationTime();
        Raportuj(string.Format("Obracam wieżę o {0} stopni w lewo", stopnie));
    }

    //powoduje wystrzelenie pocisku
    public void Strzel()
    {
        if (shootTimer > shootCooldown)
        {
            shootTimer = 0;
            cannon.isFiring = true;
            Raportuj("Odpalono pocisk!");
        }
        else
            Raportuj("Musisz poczekać " + (int)((shootCooldown - shootTimer) * 1000) + "ms na przeładowanie");
    }

    //raportowanie w celach debugowych
    private void Raportuj(string msg)
    {
        Debug.Log(msg);
    }
}
