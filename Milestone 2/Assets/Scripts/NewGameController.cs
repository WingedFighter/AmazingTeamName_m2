using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NewGameController : MonoBehaviour {
	public GameObject levelStatus;
	public GameObject UIText;
	public Button button;
	public PlayerController pc;
	private Text myText;

	// Use this for initialization
	void Start () {
		button.transform.localScale = new Vector3 (0, 0, 0);
		myText = UIText.GetComponent<Text> ();
		button.onClick.AddListener(delegate {OnButtonClick();});
	}
	
	// Update is called once per frame
	void Update () {
		if (levelStatus.tag == "LevelStatusOff") {
			//move to next level
			levelStatus.tag = "LevelStatusOn";
			pc.State = PlayerController.PlayerState.disabled;
			Win ();
		}
	}

	void Win(){
		//show text & button
		myText.text = "YOU WIN!";
		button.transform.localScale = new Vector3 (1, 1, 1);
	}
	void OnButtonClick(){
		//reload scene
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}
}
