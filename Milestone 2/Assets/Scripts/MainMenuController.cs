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

    // Level SelectButton
    public Button Level1Button;

    public CreditsController creditsController;

    public GameObject MainMenuUI;
    public GameObject LevelSelectUI;

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
        MainMenuUI.SetActive(false);
        LevelSelectUI.SetActive(true);
        eventSystem.SetSelectedGameObject(Level1Button.gameObject);
    }

    public void ExitLevelSelect()
    {
        LevelSelectUI.SetActive(false);
        MainMenuUI.SetActive(true);
        eventSystem.SetSelectedGameObject(PlayButton.gameObject);
    }

    public void Credits()
    {
        creditsController.Play();
    }

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
