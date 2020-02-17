using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{


    public GameObject obstaclePrefab;
    public float maxX;
    public float minX;
    public float maxY;
    public float minY;
    public int obstacleNumber;
    public GameObject battleGround;
    public List<GameObject> obstacles = new List<GameObject>();
    public bool generated = false;
    /*public float spawnRadiusX;
    public float spawnRadiusY;
    private Vector2[] generatedNumbers;
    private Vector2 generatedNumber;
    private int count;*/
    // Use this for initialization
    void Start()
    {

        for (int i = 0; i < obstacleNumber; i++)
        {
            GameObject newObstacle = Instantiate(obstaclePrefab, battleGround.transform) as GameObject;
            newObstacle.transform.localPosition = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0);
            obstacles.Add(newObstacle);
        }
        generated = true;

        /*count = 0;
        generatedNumbers = new Vector2[obstacleNumber];

        while (count < obstacleNumber)
        {
            generatedNumber = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
            if (count == 0)
            {
                generatedNumbers[0] = generatedNumber;
                GameObject newObstacle = Instantiate(obstaclePrefab, battleGround.transform) as GameObject;
                newObstacle.transform.localPosition = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0);
                count++;
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    if (Mathf.Abs(generatedNumber.x) < Mathf.Abs(generatedNumbers[i].x) + spawnRadiusX && Mathf.Abs(generatedNumber.x) > Mathf.Abs(generatedNumbers[i].x) - spawnRadiusX &&
                        Mathf.Abs(generatedNumber.y) < Mathf.Abs(generatedNumbers[i].y) + spawnRadiusY && Mathf.Abs(generatedNumber.y) > Mathf.Abs(generatedNumbers[i].y) - spawnRadiusY)
                    {
                        break;
                    }
                    if (i == count - 1)
                    {
                        generatedNumbers[count] = generatedNumber;
                        GameObject newObstacle = Instantiate(obstaclePrefab, battleGround.transform) as GameObject;
                        newObstacle.transform.localPosition = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0);
                        count++;
                    }
                }
            }

        }*/
    }
}
