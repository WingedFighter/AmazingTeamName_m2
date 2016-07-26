using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuController : MonoBehaviour {
    // Buttons
    public Button PlayButton;
    public Button LevelSelectButton;
    public Button CreditsButton;

    public CreditsController creditsController;

    private EventSystem eventSystem;

    // Use this for initialization
    void Start () {
        eventSystem = EventSystem.current;
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void PlayGame()
    {
        SceneManager.LoadScene("Level2");
    }

    public void LevelSelect()
    {
        print("Going to level select");
    }

    public void Credits()
    {
        creditsController.Play();
    }
}
