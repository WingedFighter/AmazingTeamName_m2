using UnityEngine;
using System.Collections;

public class TutorialController : MonoBehaviour {
    public LevelController level;

    public GameObject[] PostDeathText;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (level.Lives < 3)
        {
            foreach(GameObject ob in PostDeathText)
            {
                ob.SetActive(true);
            }
        }
	}
}
