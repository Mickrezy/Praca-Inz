using System;
using System.Collections;
using System.Collections.Generic;
using CI.HttpClient;
using UnityEngine;

public enum GameState
{
    GS_GAME_NOT_INITIALIZED,
    GS_GAME_SEARCHNIG,
    GS_GAME_FOUND,
    GS_OPONENT_SEARCHING,
    GS_OPONENT_FOUND,
    GS_GAME_INITIALIZING,
    GS_GAME_INITIALIZED,
}

public class RestController : MonoBehaviour {

    public GameState currentGameState;

    public static RestController instance;
    public Parser myController;
    public ObstacleGenerator obstacleGenerator;

    public string serverUrl;
    public string playerID;

    public GameSchema gameData;
    public GameDataSchema myData;
    public GameDataSchema oponentData;
    public OponentController oponentController;
    public bool mirrorPlay;
    private string oponentID;

    public float waitingForOponentDelay = 5f;
    private float waitingForOponentTimer = 0f;

    public float reqDelay = 0.02f;
    private float reqTimer = 0f;

    private float prevOponentTimer = 0;
    private int reconnectTimer = 0;

    private bool isRequesting = false;


    private HttpClient client;

    // Use this for initialization
    void Start () {
        currentGameState = GameState.GS_GAME_NOT_INITIALIZED;
        client = new HttpClient();
        if (!serverUrl.StartsWith("http://"))
        {
            serverUrl = "http://" + serverUrl;
        }

        playerID = MainMenuController.instance.getPlayerID();
    }

    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update ()
    {
        if (mirrorPlay)
        {
            reqTimer += Time.deltaTime;
            if (reqTimer > reqDelay)
            {
                reqTimer = 0f;
                SendMyObjects();
            }
        }
        else if (currentGameState == GameState.GS_GAME_NOT_INITIALIZED)
        {
            RequestNewGame();
            GameManager.instance.Connecting();
        }
        else if (currentGameState == GameState.GS_GAME_FOUND)
        {
            waitingForOponentTimer += Time.deltaTime;
            if(waitingForOponentTimer > waitingForOponentDelay)
            {
                waitingForOponentTimer = 0f;
                RequestOponent();
            }
            GameManager.instance.Searching();
        }
        else if (currentGameState == GameState.GS_OPONENT_FOUND)
        {
            InitializeMyObjects();
            GameManager.instance.Searching();
        }
        else if (currentGameState == GameState.GS_GAME_INITIALIZED)
        {
            reqTimer += Time.deltaTime;
            //if (reqTimer > reqDelay)
            if(!isRequesting && reqTimer > reqDelay)
            {
                reqTimer = 0f;
                SendMyObjects();
                ReqestOponentsObjects();
            }
        }
    }

    public void RequestNewGame()
    {
        Debug.Log("requesting new game");
        System.Uri uri = new System.Uri(serverUrl + "/game?" + ParseQueryParam("playerID", playerID));
        currentGameState = GameState.GS_GAME_SEARCHNIG;

        client.GetString(uri, (response) =>
        {
            if(response.Exception != null)
            {
                Debug.Log(response.Exception);
                currentGameState = GameState.GS_GAME_NOT_INITIALIZED;
                return;
            }

            currentGameState = GameState.GS_GAME_FOUND;
            Debug.Log(response.Data);
            gameData = JsonUtility.FromJson<GameSchema>(response.Data);
            if(gameData.players.Count == 2)
            {
                oponentID= gameData.players[1 - gameData.players.FindIndex(x => x == playerID)];
                currentGameState = GameState.GS_GAME_FOUND;
            }
        });
    }

    public void RequestOponent()
    {
        //Debug.Log("requesting oponent");
        System.Uri uri = new System.Uri(serverUrl + "/game/" + gameData._id + "?" + ParseQueryParam("playerID", playerID));
        currentGameState = GameState.GS_OPONENT_SEARCHING;

        client.GetString(uri, (response) =>
        {
            if (response.Exception != null)
            {
                Debug.Log(response.Exception);
                currentGameState = GameState.GS_GAME_NOT_INITIALIZED;
                return;
            }

            //Debug.Log(response.Data);
            gameData = JsonUtility.FromJson<GameSchema>(response.Data);
            if (gameData.players.Count == 2)
            {
                oponentID = gameData.players[1 - gameData.players.FindIndex(x => x == playerID)];
                GameManager.instance.enemyName.text = oponentID;
                Debug.Log(oponentID);
                currentGameState = GameState.GS_OPONENT_FOUND;
            }
            else
            {
                currentGameState = GameState.GS_GAME_FOUND;
            }
        });
    }
    public void CancelLookingForOpponent()
    {
        //Debug.Log("requesting oponent");
        System.Uri uri = new System.Uri(serverUrl + "/cancel/" + gameData._id + "?" + ParseQueryParam("playerID", playerID));
        currentGameState = GameState.GS_GAME_NOT_INITIALIZED;
        client.GetString(uri, (response) =>
        {
            if (response.Exception != null)
            {
                Debug.Log(response.Exception);
                currentGameState = GameState.GS_GAME_NOT_INITIALIZED;
                return;
            }
            else
                return;
        });
    }
    public void InitializeMyObjects()
    {
        //Debug.Log("initializing my data");
        System.Uri uri = new System.Uri(serverUrl + "/game/" + gameData._id + "/" + playerID);
        currentGameState = GameState.GS_GAME_INITIALIZING;

        GameDataSchemaWithNoIdXD newData = new GameDataSchemaWithNoIdXD();
        newData.game = gameData._id;
        newData.player = playerID;
        newData.baseHealth = 5;

        newData.objects = new List<TankDataSchemaWithNoIdXD>();
        newData.objects.Add(new TankDataSchemaWithNoIdXD());
        newData.objects.Add(new TankDataSchemaWithNoIdXD());
        newData.objects.Add(new TankDataSchemaWithNoIdXD());

        newData.bullets = new List<BulletSchemaWithNoId>();
        newData.bullets.Add(new BulletSchemaWithNoId());
        newData.bullets.Add(new BulletSchemaWithNoId());
        newData.bullets.Add(new BulletSchemaWithNoId());


        StringContent content = new StringContent(JsonUtility.ToJson(newData), System.Text.Encoding.UTF8, "application/json");

        client.Post(uri, content, (response) =>
        {
            if (response.Exception != null)
            {
                Debug.Log(response.Exception);
                currentGameState = GameState.GS_OPONENT_FOUND;
                return;
            }
            
            myData = JsonUtility.FromJson<GameDataSchema>(response.Data);
            currentGameState = GameState.GS_GAME_INITIALIZED;
            myController.tankTimer = GameManager.instance.timer;
            GameManager.instance.InGame();
            //InitializeMyObstacles();
        });
    }

    public void InitializeMyObstacles()
    {
        Debug.Log("initializing my obstacles");
        System.Uri uri = new System.Uri(serverUrl + "/game/" + gameData._id + "/" + playerID + "/begining");

        GameDataBeginingSchema newData = new GameDataBeginingSchema();
        newData.game = gameData._id;
        newData.player = playerID;
        newData.obstacles = new List<ObstacleSchema>();
        foreach (var obst in obstacleGenerator.obstacles)
        {
            ObstacleSchema obstS = new ObstacleSchema();
            obstS.x = obst.transform.localPosition.x;
            obstS.y = obst.transform.localPosition.y;
            newData.obstacles.Add(obstS);
        }

        StringContent content = new StringContent(JsonUtility.ToJson(newData), System.Text.Encoding.UTF8, "application/json");

        client.Post(uri, content, (response) =>
        {
            if (response.Exception != null)
            {
                Debug.Log(response.Exception);
                currentGameState = GameState.GS_OPONENT_FOUND;
                return;
            }

            Debug.Log("game initailized");
            currentGameState = GameState.GS_GAME_INITIALIZED;
        });
    }

    public void ReqestOponentsObjects()
    {
        isRequesting = true;
        System.Uri uri = new System.Uri(serverUrl + "/game/" + gameData._id + "/" + oponentID);
        //Debug.Log("reqOp");

        client.GetString(uri, (response) =>
        {
            isRequesting = false;
            if (response.Exception != null)
            {
                Debug.Log(response.Exception);
                return;
            }
            try
            {
                oponentData = JsonUtility.FromJson<GameDataSchema>(response.Data);
            }catch(Exception e)
            {

            }
            //Debug.Log(oponentData.timer);
            if (Math.Abs(GameManager.instance.timer - oponentData.timer) < 10 && Math.Abs(GameManager.instance.timer - oponentData.timer) > 1)
                GameManager.instance.timer = (oponentData.timer + GameManager.instance.timer) / 2;

            if (oponentData.timer == prevOponentTimer)
                reconnectTimer++;
            else
                reconnectTimer = 0;
            if(reconnectTimer > 100)
            {
                GameManager.instance.GameWon();
            }

            prevOponentTimer = oponentData.timer;
            oponentController.ShowOponents();
        });
    }

    public void RegisterBullets()
    {
        var bullets = GameObject.FindObjectsOfType<BulletController>();

        for (int i = 0; i < 3; i++)
        {
            
            if (i >= bullets.Length || bullets[i].sent)
            {
                myData.bullets[i].isAlive = false;
                continue;
            }
            else if (!bullets[i].gameObject.activeSelf)
            {
                myData.bullets[i].isAlive = false;
                Destroy(bullets[i].gameObject);
                continue;
            }
            myData.bullets[i].id = bullets[i].id;
            myData.bullets[i].isAlive = true;
            myData.bullets[i].coX = bullets[i].transform.localPosition.x;
            myData.bullets[i].coY = bullets[i].transform.localPosition.y;
            myData.bullets[i].quat1 = bullets[i].transform.localRotation.w;
            myData.bullets[i].quatI = bullets[i].transform.localRotation.x;
            myData.bullets[i].quatJ = bullets[i].transform.localRotation.y;
            myData.bullets[i].quatK = bullets[i].transform.localRotation.z;
            myData.bullets[i].speed = bullets[i].speed;
        }
    }

    private void RegisterTanks()
    {
        for (int i = 0; i < 3; i++)
        {
            if (!myController.tanks[i])
            {
                myData.objects[i].isAlive = false;
                continue;
            }
            var tank = myController.tanks[i].GetComponent<TankController>();
            if (tank == null)
            {
                myData.objects[i].isAlive = false;
                continue;
            }
			if (!tank.gameObject.activeSelf)
			{
				myData.objects[i].isAlive = false;
				Destroy(tank.gameObject);
				continue;
			}
            myData.objects[i].isAlive = true;
            myData.objects[i].coX = tank.body.transform.localPosition.x;
            myData.objects[i].coY = tank.body.transform.localPosition.y;
            myData.objects[i].quat1 = tank.body.transform.localRotation.w;
            myData.objects[i].quatI = tank.body.transform.localRotation.x;
            myData.objects[i].quatJ = tank.body.transform.localRotation.y;
            myData.objects[i].quatK = tank.body.transform.localRotation.z;

            myData.objects[i].turret.turretQuat1 = tank.turret.transform.localRotation.w;
            myData.objects[i].turret.turretQuatI = tank.turret.transform.localRotation.x;
            myData.objects[i].turret.turretQuatJ = tank.turret.transform.localRotation.y;
            myData.objects[i].turret.turretQuatK = tank.turret.transform.localRotation.z;
            myData.objects[i].turret.turretAngle = tank.turret.rotation;
            myData.objects[i].turret.turretRotationTime = tank.turret.rotationTime;

            myData.objects[i].forceLeft = (float)tank.leftTrack.power;
            myData.objects[i].forceRight = tank.rightTrack.power;
        }
    }

    public void SendMyObjects()
    {
        myData.timer = GameManager.instance.timer;

        RegisterBullets();
        RegisterTanks();

        if (mirrorPlay)
        {
            oponentData = myData;
            oponentController.ShowOponents();
            return;
        }

        System.Uri uri = new System.Uri(serverUrl + "/game/" + gameData._id + "/" + playerID);

        StringContent content = new StringContent(JsonUtility.ToJson(myData), System.Text.Encoding.UTF8, "application/json");

        client.Put(uri, content, (response) =>
        {
            if (response.Exception != null)
            {
                Debug.Log(response.Exception);
                return;
            }
        });
    }

    public void SendYouWon()
    {
        System.Uri uri = new System.Uri(serverUrl + "/game/" + gameData._id);

        gameData.draw = false;
        gameData.winner = playerID;
        gameData.looser = oponentID;

        StringContent content = new StringContent(JsonUtility.ToJson(gameData), System.Text.Encoding.UTF8, "application/json");

        client.Put(uri, content, (response) =>
        {
            if (response.Exception != null)
            {
                Debug.Log(response.Exception);
                return;
            }
        });
    }

    public void SendDraw()
    {
        System.Uri uri = new System.Uri(serverUrl + "/game/" + gameData._id);

        gameData.draw = true;

        StringContent content = new StringContent(JsonUtility.ToJson(gameData), System.Text.Encoding.UTF8, "application/json");

        client.Put(uri, content, (response) =>
        {
            if (response.Exception != null)
            {
                Debug.Log(response.Exception);
                return;
            }
        });
    }

    public void SendYouLoose()
    {
        System.Uri uri = new System.Uri(serverUrl + "/game/" + gameData._id);

        gameData.draw = false;
        gameData.winner = oponentID;
        gameData.looser = playerID;

        StringContent content = new StringContent(JsonUtility.ToJson(gameData), System.Text.Encoding.UTF8, "application/json");

        client.Put(uri, content, (response) =>
        {
            if (response.Exception != null)
            {
                Debug.Log(response.Exception);
                return;
            }
        });
    }

    private string ParseQueryParam(string param, string value)
    {
        return param + "=" + value;
    }


    



    [System.Serializable]
    public class GameSchema
    {
        public string _id;
        public List<string> players;
        public String winner;
        public String looser;
        public bool draw;
    }

    [System.Serializable]
    public class GameDataSchemaWithNoIdXD
    {
        public String game;
        public String player;
        public float timer;
        public int baseHealth;
        public List<TankDataSchemaWithNoIdXD> objects;
        public List<BulletSchemaWithNoId> bullets;
    }

    [System.Serializable]
    public class TankDataSchemaWithNoIdXD
    {
        public bool isAlive;
        public float coX;
        public float coY;
        public float quat1;
        public float quatI;
        public float quatJ;
        public float quatK;
        public TurretSchema turret;
        public float forceLeft;
        public float forceRight;
    }

    [System.Serializable]
    public class GameDataSchema
    {
        public String _id;
        public String game;
        public String player;
        public float timer;
        public int baseHealth;
        public List<TankDataSchema> objects;
        public List<BulletSchema> bullets;
    }

    [System.Serializable]
    public class TankDataSchema
    {
        public String _id;
        public bool isAlive;
        public float coX;
        public float coY;
        public float quat1;
        public float quatI;
        public float quatJ;
        public float quatK;
        public TurretSchema turret;
        public float forceLeft;
        public float forceRight;
    }

    [System.Serializable]
    public class GameDataBeginingSchema
    {
        public String game;
        public String player;
        public List<ObstacleSchema> obstacles;
    }

    [System.Serializable]
    public class ObstacleSchema
    {
        public float x;
        public float y;
    }

    [System.Serializable]
    public class TurretSchema
    {
        public float turretQuat1;
        public float turretQuatI;
        public float turretQuatJ;
        public float turretQuatK;
        public float turretAngle;
        public float turretRotationTime;
    }

    [System.Serializable]
    public class BulletSchema
    {
        public String _id;
        public bool isAlive;
        public float id;
        public float coX;
        public float coY;
        public float quat1;
        public float quatI;
        public float quatJ;
        public float quatK;
        public float speed;
    }

    [System.Serializable]
    public class BulletSchemaWithNoId
    {
        public bool isAlive;
        public float id;
        public float coX;
        public float coY;
        public float quat1;
        public float quatI;
        public float quatJ;
        public float quatK;
        public float speed;
    }
}
