using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum PlayState
{
	PS_MENU,
	PS_GAME,
	PS_VICTORY,
	PS_GAME_OVER,
    PS_DRAW,
    PS_CONNECTING_TO_SERVER,
    PS_SEARCHING_FOR_OPPONENT
}

public class GameManager : MonoBehaviour {

	public PlayState currentPlayState;
	public static GameManager instance;
	public Canvas menuCanvas;
	public Canvas winCanvas;
	public Canvas lossCanvas;
    public Canvas inGameCanvas;
    public Canvas connectingToServerCanvas;
    public Canvas searchingForOpponentCanvas;
    public Text timerText;
    public Canvas drawCanvas;
    public Text enemyName;

    public int yourHealth;
    public int enemyHealth;

    public bool training = false;

    // TODO: sync with parser tanktimer
    public float timer;
    private string seconds, minutes;

	// Use this for initialization
	void Start () {

	}

	void Awake()
	{
        instance = this;
		InGame();
               
	}

	// Update is called once per frame
	void Update () {
		if (currentPlayState == PlayState.PS_MENU && Input.GetKeyDown(KeyCode.Escape)) {
			InGame();
		}
		else if (currentPlayState == PlayState.PS_GAME && Input.GetKeyDown(KeyCode.Escape)) {
			
			InMenu();
			
		}
        if ((currentPlayState == PlayState.PS_GAME || currentPlayState == PlayState.PS_MENU)&& !training)
        {
            timer -= Time.deltaTime;
            minutes = Mathf.Floor(timer / 60).ToString("00");
            seconds = (timer % 60).ToString("00");
            if (seconds == "60") seconds = "59";
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            if (timer <= 0)
            {
                timerText.text = "00:00";
                if (yourHealth > enemyHealth)
                {
                    GameWon();
                }
                else if (yourHealth < enemyHealth)
                {
                    GameOver();
                }

                else Draw();
            }
        }
	}

	public void ExitGame() {
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif

	}

	void SetPlayState(PlayState newPlayState)
	{
		currentPlayState = newPlayState;
		//inGameCanvas.enabled = (currentPlayState == PlayState.PS_GAME);
		menuCanvas.gameObject.SetActive(currentPlayState == PlayState.PS_MENU);
		winCanvas.gameObject.SetActive(currentPlayState == PlayState.PS_VICTORY);
		lossCanvas.gameObject.SetActive(currentPlayState == PlayState.PS_GAME_OVER);
        drawCanvas.gameObject.SetActive(currentPlayState == PlayState.PS_DRAW);
        connectingToServerCanvas.gameObject.SetActive(currentPlayState == PlayState.PS_CONNECTING_TO_SERVER);
        searchingForOpponentCanvas.gameObject.SetActive(currentPlayState == PlayState.PS_SEARCHING_FOR_OPPONENT);
        inGameCanvas.GetComponent<GraphicRaycaster>().enabled = currentPlayState == PlayState.PS_GAME;
	}

	public void ReturnToMainMenu(){
		SceneManager.LoadScene("MainScreen");
	}

	public void OptionsMenu(){
		SceneManager.LoadScene("Options");
	}

	public void InGame()
	{
		SetPlayState(PlayState.PS_GAME);       
    }

	public void GameOver()
    {
        RestController.instance.SendYouLoose();
        SetPlayState(PlayState.PS_GAME_OVER);       
    }

    public void Draw()
    {
        RestController.instance.SendDraw();
        SetPlayState(PlayState.PS_DRAW);
    }

    public void InMenu()
	{
		SetPlayState(PlayState.PS_MENU);
	}

	public void GameWon()
    {
        RestController.instance.SendYouWon();
        SetPlayState(PlayState.PS_VICTORY);
        
    }

    public void Connecting()
    {
        SetPlayState(PlayState.PS_CONNECTING_TO_SERVER);

    }

    public void Searching()
    {
        SetPlayState(PlayState.PS_SEARCHING_FOR_OPPONENT);
    }
    public void CancelSearching()
    {
        RestController.instance.CancelLookingForOpponent();
        
    }
    public void OnResumeButtonClicked()
	{
		InGame ();
	}

	/*public void OnRestartButtonClicked()
	{
		SceneManager.LoadScene( SceneManager.GetActiveScene().name );
	}*/

}


