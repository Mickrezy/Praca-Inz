using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;


public enum MenuState
{
    MS_LOGGED_IN,
    MS_NOT_LOGGED,
    MS_REGISTRATION,
    MS_TUTORIAL
}


public class MainMenuController : MonoBehaviour
{

    public MenuState currentMenuState;
    public static MainMenuController instance;
    public Canvas loggedCanvas;
    public Canvas notLoggedCanvas;
    public Canvas registrationCanvas;
    public Canvas loginPasswordCanvas;
    public Canvas tutorialCanvas;

    public Text failedLogin;
    public Text passwordsDontMatch;
    public Text emailAlreadyInUse;
    public Text idAlreadyInUse;
    public Text badPassword;
    public Text badEmail;
    public Text badID;

    public Text winsStat;
    public Text loseStat;
    public Text drawsStat;
    public Text usernameText;

    public InputField idField;
    public InputField emailField;
    public InputField passwordField;
    public InputField repeatPasswordField;

    public Button tutorialButton;
    public Button trainingButton;
    public Button previousButton;
    public Button nextButton;

    //private static bool isLoggedIn = false;

    //public static string username;
    public string email;
    public string stats;

    public int wins;
    public int loses;
    public int draws;
    private static string username;
    private static bool isLoggedIn;

    public List<Image> tutorialImages = new List<Image>();

    private int step = 0;

    // Use this for initialization
    void Start()
    {

    }

    void Awake()
    {
        instance = this;
        if (isLoggedIn) 
        {
            LoginController.instance.GetStats(username);
            LoggedIn();
        }
        else notLogged();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
        LogOut();
		UnityEditor.EditorApplication.isPlaying = false;
        #else
        LogOut();
        Application.Quit();
        #endif
    }

    void SetMenuState(MenuState newMenuState)
    {
        currentMenuState = newMenuState;
        notLoggedCanvas.gameObject.SetActive(currentMenuState == MenuState.MS_NOT_LOGGED);
        registrationCanvas.gameObject.SetActive(currentMenuState == MenuState.MS_REGISTRATION);
        loggedCanvas.gameObject.SetActive(currentMenuState == MenuState.MS_LOGGED_IN);  
        loginPasswordCanvas.gameObject.SetActive(currentMenuState == MenuState.MS_NOT_LOGGED || currentMenuState ==  MenuState.MS_REGISTRATION);
        tutorialCanvas.gameObject.SetActive(currentMenuState == MenuState.MS_TUTORIAL);
    }

    public void StartTutorial()
    {
        DisableErrors();
        inTutorial();
    }

    public void NextImage()
    {
        tutorialImages[step].gameObject.SetActive(false);
        step++;
        tutorialImages[step].gameObject.SetActive(true);
        if (step == tutorialImages.Count - 1)
        {
            nextButton.gameObject.SetActive(false);
        }
        else
        {
            nextButton.gameObject.SetActive(true);
        }
        previousButton.gameObject.SetActive(true);
        
    }
    
    public void PreviousImage()
    {
        tutorialImages[step].gameObject.SetActive(false);
        step--;
        tutorialImages[step].gameObject.SetActive(true);
        if (step > 0)
        {
            previousButton.gameObject.SetActive(true);
        }
        else
        {
            previousButton.gameObject.SetActive(false);
        }
        nextButton.gameObject.SetActive(true);
    }

    public void Training()
    {
        DisableErrors();
        SceneManager.LoadScene("Training");
    }

    public void LogIn()
    {
        DisableErrors();
        string login = idField.text;
        string logpassword = passwordField.text;
        LoginController.instance.LogIn(login, logpassword);
    }

    public void LogOut()
    {
        DisableErrors();
        LoginController.instance.LogOut();
    }

    public void Return()
    {
        DisableErrors();
        notLogged();
    }

    public void outOfTutorial()
    {
        DisableErrors();
        tutorialButton.gameObject.SetActive(true);
        trainingButton.gameObject.SetActive(true);
        step = 0;
        nextButton.gameObject.SetActive(false);
        previousButton.gameObject.SetActive(false);
        for(int i = 0; i < tutorialImages.Count; i++)
        {
            tutorialImages[i].gameObject.SetActive(false);
        }
        if (isLoggedIn) LoggedIn();
        else notLogged();
    }

    public void StartRegistration()
    {
        DisableErrors();
        Registration();
    }

    public void SendRegistration()
    {

        DisableErrors();
        string regid = idField.text;
        string regemail = emailField.text;
        string regpassword = passwordField.text;
        string regpasswordRepeat = repeatPasswordField.text;


        Regex regex = new Regex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        bool match = regex.IsMatch(regemail);




        //if (regemail.Length == 0)
        if (!match)
        {
            badEmail.gameObject.SetActive(true);
        }

        else if (regid.Length == 0)
        {

            badID.gameObject.SetActive(true);
        }

        else if (regpassword.Length < 6)
        {
            badPassword.gameObject.SetActive(true);
        }

        else if (regpassword != regpasswordRepeat)
        {
            passwordsDontMatch.gameObject.SetActive(true);
        }

        else
        {
            LoginController.instance.Register(regid, regemail, regpassword, regpasswordRepeat);                 
        }
    }

    public void FindMatch()
    {
        SceneManager.LoadScene("Chellenge");
    }

    public void notLogged()
    {
        //isLoggedIn = false;
        isLoggedIn = false;
        SetMenuState(MenuState.MS_NOT_LOGGED);
    }

    public void inTutorial()
    {
        step = 0;

        for (int i = 0; i < tutorialImages.Count; i++)
        {
            tutorialImages[i].gameObject.SetActive(false);
        }

        tutorialImages[step].gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);
        previousButton.gameObject.SetActive(false);
        tutorialButton.gameObject.SetActive(false);
        trainingButton.gameObject.SetActive(false);

        SetMenuState(MenuState.MS_TUTORIAL);
    }

    public void LoggedIn()
    {
        isLoggedIn = true;
        SetMenuState(MenuState.MS_LOGGED_IN);
    }

    public void Registration()
    {
        SetMenuState(MenuState.MS_REGISTRATION);
    }

    public void DisableErrors()
    {
        passwordsDontMatch.gameObject.SetActive(false);
        emailAlreadyInUse.gameObject.SetActive(false);
        idAlreadyInUse.gameObject.SetActive(false);
        badPassword.gameObject.SetActive(false);
        failedLogin.gameObject.SetActive(false);
        badEmail.gameObject.SetActive(false);
        badID.gameObject.SetActive(false);
    }

    public void setPlayerID(string id)
    {
        username = id;
    }

    public string getPlayerID()
    {
        return username;
    }

    public void SetStats(int winCount, int drawCount, int loseCount)
    {
        usernameText.text = username;
        winsStat.text = "Wygrane: " + winCount.ToString();
        loseStat.text = "Przegrane: " + loseCount.ToString();
        drawsStat.text = "Remisy: " + drawCount.ToString();
    }

}


