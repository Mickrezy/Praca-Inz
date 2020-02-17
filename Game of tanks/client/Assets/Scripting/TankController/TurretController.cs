using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    Rigidbody2D rb;
    public GameObject tank;
    public CannonController cannon;
    public float rotation;

    public float rotationTime;
    Vector3 rotationEuler;
    private float angle;
    public float rotationSpeed;
    //public bool isRotating;
        // Use this for initialization
    void Start()
    {
        angle = 0;        
    }

    // Update is called once per frame
    void Update()
    {
        if (rotationTime > 0)
        {
            if (rotation < 0 && rotationSpeed > 0) rotationSpeed *= -1;
            if (rotation > 0 && rotationSpeed < 0) rotationSpeed *= -1;
            rotationEuler += Vector3.forward * rotationSpeed * Time.deltaTime; 
            transform.rotation = Quaternion.Euler(rotationEuler) * tank.transform.rotation;
            rotationTime -= Time.deltaTime;
            if (rotationTime <= 0){
                angle += rotation;
                if(angle < 0)
                {
                    angle += 360;
                }
                else if (angle > 360)
                {
                    angle -= 360;
                }                

            }

        }
        else
        {
            transform.rotation = tank.transform.rotation * Quaternion.Euler(0,0, angle);
            //transform.rotation = Quaternion.Euler(angle, 0, 0);
       }
    }

    public void SetRotationTime()
    {
        rotationTime = System.Math.Abs(rotation / rotationSpeed);
    }
}
