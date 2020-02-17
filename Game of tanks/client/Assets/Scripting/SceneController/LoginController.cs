using System;
using System.Collections;
using System.Collections.Generic;
using CI.HttpClient;
using UnityEngine;
using System.Net;


public class LoginController : MonoBehaviour {

    public static LoginController instance;

    public string serverUrl;
    public RegisterData registerData;
    public LoginData loginData;
    public StatsData statsData;

    private HttpClient client;


    void Start () {

        System.Net.ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) =>
        {
            return true;
        };
    }

    private void Awake()
    {
        if (!serverUrl.StartsWith("http://"))
        {
            serverUrl = "http://" + serverUrl;
        }

        instance = this;
        client = new HttpClient();
    }


    void Update () {
		
	}

    public void Register(string regPlayerID, string regEmail, string regPassword, string regRepeatPassword)
    {
        
        System.Uri uri = new System.Uri(serverUrl + "/register");
        Debug.Log(uri);
        RegisterData newData = new RegisterData();
        newData.username = regPlayerID;
        newData.email = regEmail;
        newData.password = regPassword;
        newData.passwordConf = regRepeatPassword;

        Debug.Log(newData);

        StringContent content = new StringContent(JsonUtility.ToJson(newData), System.Text.Encoding.UTF8, "application/json");

        Debug.Log(content.ToString());

        client.Post(uri, content, (response) =>
        {
            if (response.Exception != null)
            {
                if(response.StatusCode == HttpStatusCode.PaymentRequired) MainMenuController.instance.emailAlreadyInUse.gameObject.SetActive(true);
                else if (response.StatusCode == HttpStatusCode.Unauthorized) MainMenuController.instance.idAlreadyInUse.gameObject.SetActive(true);
                return;
            }

            MainMenuController.instance.notLogged();

        });
    }

    public void LogIn(string username, string password)
    {

        System.Uri uri = new System.Uri(serverUrl + "/login");

        LoginData newData = new LoginData();
        newData.login = username;
        newData.logpassword = password;

        StringContent content = new StringContent(JsonUtility.ToJson(newData), System.Text.Encoding.UTF8, "application/json");

        client.Post(uri, content, (response) =>
        {
            if (response.Exception != null)
            {
                MainMenuController.instance.failedLogin.gameObject.SetActive(true);
                return;
            }

            MainMenuController.instance.setPlayerID(username);
            MainMenuController.instance.LoggedIn();
            GetStats(username);

        });
    }

    public void LogOut()
    {

        System.Uri uri = new System.Uri(serverUrl + "/logout");

        client.GetString(uri, (response) =>
        {
            if (response.Exception != null)
            {

            }

            MainMenuController.instance.notLogged();

        });
    }

    public void GetStats(string username)
    {

        System.Uri uri = new System.Uri(serverUrl + "/stats/" + username);
        StatsData newData = new StatsData();

        client.GetString(uri, (response) =>
        {
            if (response.Exception != null)
            {

            }

            try
            {
                newData = JsonUtility.FromJson<StatsData>(response.Data);
            }
            catch (Exception e)
            {

            }

            MainMenuController.instance.SetStats(newData.wins, newData.draws, newData.defeats);

        });
    }


    [System.Serializable]
    public class LoginData
    {
        public string login;
        public string logpassword;
    }

    [System.Serializable]
    public class RegisterData
    {
        public string username;
        public string email;
        public string password;
        public string passwordConf;

    }

    [System.Serializable]
    public class StatsData
    {
        public int wins;
        public int defeats;
        public int draws;

    }

}
