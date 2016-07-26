﻿using UnityEngine;
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
    public GameObject MultiplierText;
    public string NextLevel = "MainMenu";

    public double Score = 0;
    public int Lives = 3;
    public int Multiplier = 0;
    public bool LevelCompleted = false;

    public float ScoreSpeedModifier = 10f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        // calculate the score
        if (Player.GetComponent<Rigidbody>().velocity.magnitude > 4)
        {
            Score += Player.GetComponent<Rigidbody>().velocity.magnitude / ScoreSpeedModifier;
        }
        if (!TargetBuilding.IsStable())
        {
            foreach(Rigidbody rb in TargetBuilding.GetComponentsInChildren<Rigidbody>())
            {
                Score += Mathf.Round(rb.velocity.magnitude);
            }
        }

        // Reset the player if the death plane is triggered
        if (dPlane.Triggered && !LevelCompleted)
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
        if (Lives <= 0 && !LevelCompleted)
        {
            LevelFailed();
        }
	}

    void LevelComplete()
    {
        LevelCompleted = true;
        SuccessText.SetActive(true);
        MultiplierText.SetActive(true);
        Invoke("ApplyMultiplier", 4);
        //Invoke("LoadNextLevel", 4);
    }

    void ApplyMultiplier()
    {
        for (; Lives > 0; Lives--)
        {
            //Lives--;
            Multiplier++; 
        }

        Score = (Multiplier) * Score;

        LivesText.GetComponent<Text>().text = "Lives: " + Lives;
        MultiplierText.GetComponent<Text>().text = "" + Multiplier;
        ScoreText.GetComponent<Text>().text = "Score: " + Score;
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
