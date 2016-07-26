using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LevelController : MonoBehaviour {
    public Building TargetBuilding;
    public DeathPlane dPlane;
    public GameObject Player;
    public GameObject SuccessText;
    public GameObject FailureText;
    public GameObject ScoreText;
    public GameObject LivesText;
    public GameObject SpeedometerText;
    public string NextLevel = "MainMenu";

    public float Score = 0;
    public int Lives = 3;

    public float ScoreSpeedModifier = 10f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        // calculate the score
        Score += Player.GetComponent<Rigidbody>().velocity.magnitude/ScoreSpeedModifier;

        // Reset the player if the death plane is triggered
        if (dPlane.Triggered)
        {
            if (TargetBuilding.IsStable())
            {
                Player.GetComponent<PlayerControllerAlpha>().Reset();
                Lives--;
                dPlane.Triggered = false;
            }
        }

        if (TargetBuilding.IsDestroyed())
        {
            Invoke("LevelComplete", 4);
        }

        // Update Score and lives text
        LivesText.GetComponent<Text>().text = "Lives: " + Lives;
        ScoreText.GetComponent<Text>().text = "Score: " + (int)Score;
        SpeedometerText.GetComponent<Text>().text = Mathf.Round(Player.GetComponent<Rigidbody>().velocity.magnitude) + " MPH";

        // Check for Lose condition
        if (Lives <= 0)
        {
            LevelFailed();
        }
	}

    void LevelComplete()
    {
        SuccessText.SetActive(true);
        Invoke("LoadNextLevel", 4);
    }

    void LevelFailed()
    {
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
}
