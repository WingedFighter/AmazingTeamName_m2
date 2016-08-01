using UnityEngine;
using System.Collections;

public class CreditsController : MonoBehaviour {
    public GameObject UICanvas;
    public GameObject[] CreditCubes;

    private bool bPlaying = false;
    private int index = 0;
    private CreditsCube currentCube;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (bPlaying)
        {
            if (currentCube == null || currentCube.bLanded == true)
            {
                if (index < CreditCubes.Length)
                {
                    currentCube = ((GameObject)Instantiate(CreditCubes[index], transform.position, transform.rotation)).GetComponent<CreditsCube>();
                    index++;
                } else
                {
                    EndCredits();
                }
            }
        }
	}

    public void Play()
    {
        UICanvas.SetActive(false);
        bPlaying = true;
    }

    private void EndCredits()
    {
        bPlaying = false;
        index = 0;
        UICanvas.SetActive(true);
    }
}
