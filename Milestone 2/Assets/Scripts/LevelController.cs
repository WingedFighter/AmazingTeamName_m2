using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class LevelController : MonoBehaviour {
    public Building TargetBuilding;
    public DeathPlane dPlane;
    public GameObject Player;
    public GameObject SuccessText;
    public GameObject FailureText;
    public GameObject GameUI;
    public GameObject ScoreText;
    public GameObject FinalScoreText;
    public GameObject LivesText;
    public GameObject SpeedometerText;
    public GameObject MultiplierText;
    public GameObject Cam;
    public GameObject PauseUI;
    public GameObject ResumeButton;
    public string NextLevel = "MainMenu";

    public float Score = 0;
    public int Lives = 3;
    public int Multiplier = 0;
    public bool LevelCompleted = false;

    public bool bPause = false;

	private float previousSpeed = 0f;
    public float ScoreSpeedModifier = 10f;

    private AudioSource MultiplierAudio;
	//ship stuff
	public DropShipController dropShipController;

	// Use this for initialization
	void Start () {
        MultiplierAudio = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button7))
        {
            bPause = !bPause;
            if (bPause)
            {
                Pause();
            }
            else
            {
                Unpause();
            }
        }

        // calculate the score
        if (Player.GetComponent<Rigidbody>().velocity.magnitude > 4)
        {
            Score += Player.GetComponent<Rigidbody>().velocity.z / ScoreSpeedModifier;
        }
        if (!TargetBuilding.IsStable())
        {
            foreach(Rigidbody rb in TargetBuilding.GetComponentsInChildren<Rigidbody>())
            {
                Score += Mathf.Round(rb.velocity.magnitude);
            }
        }

        if (dPlane.Triggered)
        {
            Player.GetComponent<PlayerControllerAlpha>().bDead = true;   
        }

        // Reset the player if the death plane is triggered
        if ((dPlane.Triggered || Player.GetComponent<PlayerControllerAlpha>().bDead) && !LevelCompleted)
        {
            if (TargetBuilding.IsStable())
            {
                Player.GetComponent<PlayerControllerAlpha>().Reset();
				dropShipController.Reset ();
                Lives--;
                dPlane.Triggered = false;
            } 

        }

        if (TargetBuilding.IsDestroyed() && !LevelCompleted)
        {
            LevelCompleted = true;
            Invoke("LevelComplete", 4);
        }

        // Update Score and lives text
        LivesText.GetComponent<Text>().text = "Lives: " + Lives;
        ScoreText.GetComponent<Text>().text = "Score: " + (int)Score;
		float currentSpeed = Player.GetComponent<Rigidbody>().velocity.z;
		previousSpeed = (Mathf.Lerp(previousSpeed, currentSpeed, Time.deltaTime));
		SpeedometerText.GetComponent<Text>().text = Mathf.Round(previousSpeed) + " MPH";
        // Check for Lose condition
        if (Lives <= 0 && !LevelCompleted)
        {
            LevelFailed();
        }
	}

    void LevelComplete()
    {
        SuccessText.SetActive(true);
        Invoke("ApplyMultiplier", 4);
        //Invoke("LoadNextLevel", 4);
    }

    void ApplyMultiplier()
    {
        /*for (; Lives > 0; Lives--)
        {
            //Lives--;
            Multiplier++; 
        }

        Score = (Multiplier) * Score;

        LivesText.GetComponent<Text>().text = "Lives: " + Lives;
        MultiplierText.GetComponent<Text>().text = "" + Multiplier;
        ScoreText.GetComponent<Text>().text = "Score: " + Score;
        Invoke("LoadNextLevel", 4);*/
        if (Lives > 0)
        {
            MultiplierText.SetActive(true);
            Lives--;
            Multiplier++;

            // 
            MultiplierAudio.Play();

            LivesText.GetComponent<Text>().text = "Lives: " + Lives;
            MultiplierText.GetComponent<Text>().text = "" + Multiplier;
            Invoke("ApplyMultiplier", 1);
        }
        else
        {
            Score = (Multiplier) * Score;
            FinalScoreText.GetComponent<Text>().text = "FINAL SCORE:\n" + Mathf.Round(Score); ;

            GameUI.SetActive(false);
            SuccessText.SetActive(false);
            FinalScoreText.SetActive(true);
            MultiplierAudio.Play();

            Invoke("LoadNextLevel", 4);
        }
    }

    void LevelFailed()
    {
        Player.GetComponent<PlayerControllerAlpha>().State = PlayerControllerAlpha.PlayerState.disabled;
        FailureText.SetActive(true);
        Invoke("ReloadLevel", 4);
    }

    void LoadNextLevel()
    {
        SceneManager.LoadScene(NextLevel);
    }
    void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Pause()
    {
        bPause = true;
        Cam.GetComponent<AudioSource>().Pause();
        Time.timeScale = 0;
        PauseUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(ResumeButton);
    }

    public void Unpause()
    {
        EventSystem.current.SetSelectedGameObject(null);
        bPause = false;
        PauseUI.SetActive(false);
        Cam.GetComponent<AudioSource>().UnPause();
        Time.timeScale = 1;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
