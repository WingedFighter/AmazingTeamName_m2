using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class NewGameController : MonoBehaviour {
	public GameObject levelStatus;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (levelStatus.tag == "LevelStatusOff") {
			//move to next level
			levelStatus.tag = "LevelStatusOn";
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}
}
