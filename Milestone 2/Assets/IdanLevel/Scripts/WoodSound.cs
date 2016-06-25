using UnityEngine;
using System.Collections;

public class WoodSound : MonoBehaviour {

    private AudioSource footsteps;
    private bool bPlaying = false;

	// Use this for initialization
	void Start () {
        footsteps = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision collision)
    {
        if (!bPlaying)
        {
            footsteps.Play();
            bPlaying = true;
        }
    }
}
