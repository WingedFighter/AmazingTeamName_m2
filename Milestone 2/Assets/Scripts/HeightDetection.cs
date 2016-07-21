using UnityEngine;
using System.Collections;

public class HeightDetection : MonoBehaviour {
	public GameObject levelStatus;

	// Use this for initialization
	void Start () {		
	}
	// Update is called once per frame
	void Update () {			
	}
	void OnTriggerExit(Collider other){
		//end game
		if (other.tag == "Building") {
			levelStatus.tag = "LevelStatusOff";
		}
	}
}
