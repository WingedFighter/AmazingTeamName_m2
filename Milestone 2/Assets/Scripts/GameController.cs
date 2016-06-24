using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/* Amazing Team Name
 * Kevin Curtin
 * Idan Mintz
 * Jackson Millsaps
 * Jessica Chin
 * Matthew Johnston
 */

public class GameController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        // Load the new scenes based on the keypress
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneManager.LoadScene("MattScene");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SceneManager.LoadScene("KevinsLevel");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            //SceneManager.LoadScene("JessicasLevel");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SceneManager.LoadScene("JacksonScene");
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SceneManager.LoadScene("IdanScene");
        }
        // Load the test scene when hitting 0
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            //SceneManager.LoadScene("Main");
        }
    }
}
